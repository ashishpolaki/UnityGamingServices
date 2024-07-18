using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;

public class GPS
{
    public double CurrentLocationLatitude
    {
        get; private set;
    }
    public double CurrentLocationLongitude
    {
        get; private set;
    }

    public void RequestPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
    }

    public async Task<bool> TryGetLocationAsync()
    {
        RequestPermission();

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        bool isInitialized = await WaitForLocationServiceToInitialize();
        if (!isInitialized)
        {
            Debug.Log("Timed out");
            return false;
        }

        if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation) || !Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
        {
            Debug.Log("Location Permission Denied");
            return false;
        }

        // Access granted and location value could be retrieved
        CurrentLocationLatitude = Input.location.lastData.latitude;
        CurrentLocationLongitude = Input.location.lastData.longitude;
        Debug.Log("Location Granted");
        return true;
    }

    private async Task<bool> WaitForLocationServiceToInitialize()
    {
        int maxWait = 10;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            await Task.Delay(1000); // Wait for 1 second
            maxWait--;
        }

        if (maxWait < 1 || Input.location.status == LocationServiceStatus.Failed)
        {
            return false;
        }

        return true;
    }
}

public class DistanceCalculator
{
    public const double EarthRadiusKm = 6371; // Earth radius in kilometers

    /// <summary>
    /// Return Distance in Meters
    /// </summary>
    /// <param name="lat1"></param>
    /// <param name="lon1"></param>
    /// <param name="lat2"></param>
    /// <param name="lon2"></param>
    /// <returns></returns>
    public static float CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Pow(Math.Sin(dLon / 2), 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = EarthRadiusKm * c;
        distance = ConvertKilometersToMeters(distance);
        return (float)distance;
    }

    public static double ToRadians(double angle)
    {
        return Math.PI * angle / 180.0;
    }

    public static double ConvertKilometersToMeters(double kilometers)
    {
        return kilometers * 1000; // 1 kilometer is 1000 meters
    }
}