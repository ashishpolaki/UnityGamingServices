using System;

namespace HorseRaceCloudCode
{
    internal class ClassUtils
    {
    }
    public class RegisterHostItem
    {
        public string PlayerID { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public float Radius { get; set; }
    }

    public class CheckInResponse
    {
        public string PlayerID { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
    public class CheckInAttendance
    {
        public string Date { get; set; }
        public string LastCheckInTime { get; set; }
        public int Count { get; set; }
    }
    public static class DistanceCalculator
    {
        public static float CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double EarthRadiusKm = 6371;

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Pow(Math.Sin(dLon / 2), 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = EarthRadiusKm * c;
            distance = distance * 1000; //Convert to meters
            return (float)distance;
        }
        public static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
