﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Npgsql;

namespace Backend.Models
{
    public class Meeting
    {
        public int meetingId { get; set; }
        public string naslov { get; set; }
        public string opis { get; set; }
        public string mjesto { get; set; }
        public DateTime? vrijeme { get; set; }
        public string status { get; set; }
        public int zgradaId { get; set; }
        public int kreatorId { get; set; }
        public string sazetak { get; set; }
        public List<TockaDnevnogReda> tockeDnevnogReda { get; set; }

        public Meeting(int meetingId, string naslov, string opis, string mjesto, DateTime? vrijeme, string status, int zgradaId, int kreatorId, string sazetak)
        {
            this.meetingId = meetingId;
            this.naslov = naslov;
            this.opis = opis;
            this.mjesto = mjesto;
            this.vrijeme = vrijeme;
            this.status = status;
            this.zgradaId = zgradaId;
            this.kreatorId = kreatorId;
            this.sazetak = sazetak;
            this.tockeDnevnogReda = new List<TockaDnevnogReda>();
        }

        public static List<Meeting> getMeetingsForBuilding(int buildingId)
        {
            List<Meeting> meetings = new List<Meeting>();
            var conn = Database.GetConnection();
            using (var cmd = new NpgsqlCommand("SELECT * FROM sastanak WHERE zgradaID = @buildingId", conn))
            {
                cmd.Parameters.AddWithValue("buildingId", buildingId);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string? sazetak = reader.IsDBNull(0) ? null : reader.GetString(0);
                    DateTime vrijeme = reader.GetDateTime(1);
                    string mjesto = reader.GetString(2);
                    string status = reader.GetString(3);
                    int id = reader.GetInt32(4);
                    string naslov = reader.GetString(5);
                    int zgradaId = reader.GetInt32(6);
                    int kreatorId = reader.GetInt32(7);

                    meetings.Add(new Meeting(id, naslov, sazetak, mjesto, vrijeme, status, zgradaId, kreatorId, sazetak));
                }
                reader.Close();
            }

            foreach (var meet in meetings){
                using (var cmd = new NpgsqlCommand("SELECT * FROM tocka_dnevnog_reda WHERE sastanakID = @meetingId", conn))
                {
                    cmd.Parameters.AddWithValue("meetingId", meet.meetingId);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string imeTocke = reader.GetString(0);
                        bool imaPravniUcinak = reader.GetBoolean(1);
                        string? sazetak = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? stanjeZakljucka = reader.IsDBNull(3) ? null : reader.GetString(3);
                        int id = reader.GetInt32(4);
                        string? url = reader.IsDBNull(5) ? null : reader.GetString(5);
                        int sastanakId = reader.GetInt32(6);

                        meet.tockeDnevnogReda.Add(new TockaDnevnogReda(id, imeTocke, imaPravniUcinak, sazetak, stanjeZakljucka, url, sastanakId));
                    }
                    reader.Close();
                }
            }
            
            return meetings;
        }

        public static Meeting getMeeting(int meetingId)
        {
            Meeting meeting = null;
            var conn = Database.GetConnection();
            using (var cmd = new NpgsqlCommand("SELECT * FROM sastanak WHERE sastanakID = @meetingId", conn))
            {
                cmd.Parameters.AddWithValue("meetingId", meetingId);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string? sazetak = reader.IsDBNull(0) ? null : reader.GetString(0);
                    DateTime vrijeme = reader.GetDateTime(1);
                    string mjesto = reader.GetString(2);
                    string status = reader.GetString(3);
                    int id = reader.GetInt32(4);
                    string naslov = reader.GetString(5);
                    int zgradaId = reader.GetInt32(6);
                    int kreatorId = reader.GetInt32(7);

                    meeting = new Meeting(id, naslov, sazetak, mjesto, vrijeme, status, zgradaId, kreatorId, sazetak);
                }
                reader.Close();
            }

            using (var cmd = new NpgsqlCommand("SELECT * FROM tocka_dnevnog_reda WHERE sastanakID = @meetingId", conn))
            {
                cmd.Parameters.AddWithValue("meetingId", meetingId);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string imeTocke = reader.GetString(0);
                    bool imaPravniUcinak = reader.GetBoolean(1);
                    string? sazetak = reader.IsDBNull(2) ? null : reader.GetString(2);
                    string? stanjeZakljucka = reader.IsDBNull(3) ? null : reader.GetString(3);
                    int id = reader.GetInt32(4);
                    string? url = reader.IsDBNull(5) ? null : reader.GetString(5);
                    int sastanakId = reader.GetInt32(6);

                    meeting.tockeDnevnogReda.Add(new TockaDnevnogReda(id, imeTocke, imaPravniUcinak, sazetak, stanjeZakljucka, url, sastanakId));
                    Console.WriteLine("Tocka dnevnog reda: " + imeTocke);
                    
                }

                reader.Close();
            }

            return meeting;
        }
        public static bool deleteMeeting(int meetingId)
        {
            try
            {
                var conn = Database.GetConnection();
                
                using (var transaction = conn.BeginTransaction())
                {
                    
                    using (var cmd = new NpgsqlCommand("DELETE FROM sastanak WHERE sastanakid = @meetingId", conn))
                    {
                        cmd.Parameters.AddWithValue("meetingId", meetingId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            transaction.Rollback();
                            return false; 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting meeting: " + ex.Message);
                return false; 
            }
        }

    }
}