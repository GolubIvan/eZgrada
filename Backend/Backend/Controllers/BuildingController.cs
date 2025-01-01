using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Backend.Controllers
{
    [Route("buildings")]
    public class BuildingController : Controller
    {
        private readonly ILogger<BuildingController> _logger;

        public BuildingController(ILogger<BuildingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{buildingId}")]
        public IActionResult GetMeetingsForBuilding(int buildingId)
        {
            var token = Request.Headers["token"];
            if (token == "undefined")
            {
                return Unauthorized(new { error = "Invalid token", message = "The user token is invalid or has expired." });
            }
            //Console.WriteLine(token);
            _logger.LogInformation("Checking user with token: {Token}", token);

            string email = JWTGenerator.ParseGoogleJwtToken(token);
            List<KeyValuePair<Backend.Models.Zgrada, string>> zgrade = Backend.Models.Racun.getUserData(email);

            string uloga = Backend.Models.User.getRole(email, buildingId);
            if (uloga == "") { return Unauthorized(new { error = "Invalid role", message = "The user role is not high enough." }); }

            List<Backend.Models.Meeting> meetings = Backend.Models.Meeting.getMeetingsForBuilding(buildingId);

            if (meetings == null)
            {
                return NotFound(new { error = "No meetings found", message = "No meetings found for the specified building." });
            }
            int userId = Racun.getID(email);
            var modifiedMeetings = meetings.Select(meeting => new
            {
                meetingId = meeting.meetingId,
                naslov = meeting.naslov,
                opis = meeting.opis,
                mjesto = meeting.mjesto,
                vrijeme = meeting.vrijeme,
                status = meeting.status,
                zgradaId = meeting.zgradaId,
                kreatorId = meeting.kreatorId,
                sazetak = meeting.sazetak,
                sudjelovanje = Meeting.checkSudjelovanje(buildingId,userId, meeting.meetingId), 
                brojSudionika = Meeting.checkSudioniciCount(buildingId,meeting.meetingId),
                tockeDnevnogReda = meeting.tockeDnevnogReda,
                isCreator = meeting.kreatorId
            }).ToList();


            return Ok(new { buildingId = buildingId, role = uloga, meetings = modifiedMeetings });
        }
    }
}