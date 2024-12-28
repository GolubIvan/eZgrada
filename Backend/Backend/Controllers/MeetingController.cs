using Backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Controllers
{
    [Route("meetings")]
    public class MeetingController : Controller
    {
        private readonly ILogger<MeetingController> _logger;

        public MeetingController(ILogger<MeetingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{meetingId}")]
        public IActionResult GetMeeting(int meetingId)
        {
            Backend.Models.Meeting meeting = Backend.Models.Meeting.getMeeting(meetingId);

            if (meeting == null)
            {
                return NotFound(new { error = "Meeting not found", message = "Meeting with the specified ID not found." });
            }

            return Ok(new { meetingId = meetingId, meeting = meeting });
        }

        [HttpPost("create")]
        public IActionResult CreateMeeting([FromBody] MeetingRequest meetingRequest)
        {
            
            string token = Request.Headers["token"].ToString() ?? "";
       
            if (token == "undefined" || token == "")        //postoji token
            {
                return Unauthorized(new { error = "Invalid token", message = "The user token is invalid or has expired." });
            }

            if (meetingRequest == null)                     //postoji meeting
            {
                return BadRequest(new { error = "Invalid data", message = "Meeting data required." });
            }

            string email = JWTGenerator.ParseGoogleJwtToken(token);
            string uloga = Backend.Models.User.getRole(email, meetingRequest.ZgradaId);

            Console.WriteLine(meetingRequest.Naslov);

            if(uloga != "Predstavnik")                      //dobra rola
            {
                return Unauthorized(new { error = "Invalid role", message = "The user role is not high enough." });
            }
            int creatorId = Backend.Models.Racun.getID(email);
            //int creatorId = 1;

            //ne dobar format
            //if (meetingRequest.Status != "Planiran")  { return BadRequest(new { error = "Invalid data", message = "Meeting has to be Planiran." }); }

            try
            {
                MeetingRequest.AddMeeting(meetingRequest, creatorId);
                return Ok(new { message = "Meeting has been added." });
            }
            catch (Exception ex) { 
                return BadRequest(new { error = "Invalid data"});
            }
            
        }


        [HttpPost("delete/{meetingId}")]
        public IActionResult DeleteMeeting(int meetingId)
        {
            string token = Request.Headers["token"].ToString() ?? "";
            if (token == "undefined" || token == "")
            {
                return Unauthorized(new { error = "Invalid token", message = "The user token is invalid or has expired." });
            }

            string email = JWTGenerator.ParseGoogleJwtToken(token); //problematicno jer ce raditi i za istekle tokene

            Meeting meeting = Backend.Models.Meeting.getMeeting(meetingId);

            if(meeting == null) return BadRequest(new {error = "Deletion failed", message = "Meeting doesnt exist." });

            string uloga = Backend.Models.User.getRole(email, meeting.zgradaId);
            

            if (uloga == "Predstavnik")
            {
                if (meeting == null)
                {
                    return NotFound(new { error = "Meeting not found", message = "Meeting with the specified ID not found." });
                }
                bool isDeleted = Backend.Models.Meeting.deleteMeeting(meetingId);

                if (!isDeleted)
                {
                    return StatusCode(500, new { error = "Deletion failed", message = "Failed to delete the meeting." });
                }
                return Ok(new { message = "Meeting with the specified ID was deleted." });
            }
            return Unauthorized(new { error = "Invalid role", message = "The user role is not high enough." });
        }

    }
}