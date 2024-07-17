namespace HorseRaceCloudCode
{
    internal class ClassUtils
    {
    }
    public class HostVenueData
    {
        public string PlayerID { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public float Radius { get; set; }

        public HostVenueData()
        {
            PlayerID = string.Empty;
        }
    }

    public class CheckInRequest
    {
        public string PlayerID { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public CheckInRequest()
        {
            PlayerID = string.Empty;
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
        public string RaceTime { get; set; }
        public string Message { get; set; }

        public JoinRaceResponse()
        {
            CanWaitInLobby = false;
            RaceTime = string.Empty;
            Message = string.Empty;
        }
    }
    public class RaceLobbyData
    {
        public string PlayerID { get; set; }
        public int HorseNumber { get; set; }

        public RaceLobbyData()
        {
            PlayerID = string.Empty;
        }
    }
}
