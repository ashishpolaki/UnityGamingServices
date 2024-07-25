using System;
using System.Collections.Generic;

namespace HorseRaceCloudCode
{
    internal class ClassUtils
    {
    }
    public class HostVenueData
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public float Radius { get; set; }

        public HostVenueData()
        {
        }
    }
    public class PlayerCheckIn
    {
        public string Date { get; set; }
        public string LastCheckInTime { get; set; }
        public int Count { get; set; }

        public PlayerCheckIn()
        {
            Date = string.Empty;
            LastCheckInTime = string.Empty;
        }
    }
    public class RaceScheduleData
    {
        public string ScheduleStart { get; set; }
        public string ScheduleEnd { get; set; }
        public int TimeGap { get; set; }
        public int PreRaceWaitTime { get; set; }

        public RaceScheduleData()
        {
            ScheduleStart = string.Empty;
            ScheduleEnd = string.Empty;
        }
    }
    public class JoinRaceResponse
    {
        public bool CanWaitInLobby { get; set; }
        public DateTime RaceTime { get; set; }
        public string Message { get; set; }

        public JoinRaceResponse()
        {
            CanWaitInLobby = false;
            Message = string.Empty;
        }
    }
    public class RaceLobbyParticipant
    {
        public string PlayerID { get; set; }
        public int HorseNumber { get; set; }
        public string PlayerName { get; set; }

        public RaceLobbyParticipant()
        {
            PlayerID = string.Empty;
            PlayerName = string.Empty;
        }
    }
    public class CurrentRacePlayerCheckIn
    {
        public string PlayerID { get; set; }
        public string PlayerName { get; set; }
        public int CurrentDayCheckIns { get; set; }

        public CurrentRacePlayerCheckIn()
        {
            PlayerID = string.Empty;
            PlayerName = string.Empty;
        }
    }
    public class RaceResult
    {
        public List<PlayerRaceResult> playerRaceResults { get; set; }
        public RaceResult()
        {
            playerRaceResults = new List<PlayerRaceResult>();
        }
    }
    public class PlayerRaceResult
    {
        public string PlayerID { get; set; }
        public int HorseNumber { get; set; }
        public int RacePosition { get; set; }

        public PlayerRaceResult()
        {
            PlayerID = string.Empty;
        }
    }
}
