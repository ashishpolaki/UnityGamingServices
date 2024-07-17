using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

namespace HorseRaceCloudCode
{
    public class Utils
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
        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static async Task<T> GetCustomDataWithKey<T>(IExecutionContext context, IGameApiClient gameApiClient, string _customID, string key)
        {
            T? item = Activator.CreateInstance<T>();
            var getResponse = await gameApiClient.CloudSaveData.GetCustomItemsAsync(context, context.ServiceToken, context.ProjectId, _customID, new List<string> { key });
            if (getResponse.Data.Results.Count > 0)
            {
                string? jsonString = getResponse.Data.Results[0].Value?.ToString();
                if (jsonString != null)
                {
                    item = JsonConvert.DeserializeObject<T>(jsonString);
                    if (item == null)
                    {
                        item = Activator.CreateInstance<T>();
                    }
                }
            }
            return item;
        }
        public static async Task<T> GetProtectedDataWithKey<T>(IExecutionContext context, IGameApiClient gameApiClient, string _customID, string key)
        {
            T? item = Activator.CreateInstance<T>();
            var getResponse = await gameApiClient.CloudSaveData.GetProtectedItemsAsync(context, context.ServiceToken, context.ProjectId, _customID, new List<string> { key });
            if (getResponse.Data.Results.Count > 0)
            {
                string? jsonString = getResponse.Data.Results[0].Value?.ToString();
                if (jsonString != null)
                {
                    item = JsonConvert.DeserializeObject<T>(jsonString);
                    if (item == null)
                    {
                        item = Activator.CreateInstance<T>();
                    }
                }
            }
            return item;
        }

        public static async Task<string> GetHostID<T>(IExecutionContext context, IGameApiClient gameApiClient, ILogger<T> _logger, double playerLatitude, double playerLongitude)
        {
            string hostID = string.Empty;

            var getResponse = await gameApiClient.CloudSaveData.GetCustomItemsAsync(context, context.ServiceToken, context.ProjectId, "HostVenue",null);
            // Check if there is any data in HostVenue and checkInData is not null
            if (getResponse.Data.Results.Count > 0)
            {
                foreach (var item in getResponse.Data.Results)
                {
                    try
                    {
                        //Check if hostDataString is not null and empty
                        string? hostDataString = item.Value.ToString();
                        if (hostDataString != null && !string.IsNullOrEmpty(item.Value.ToString()))
                        {
                            HostVenueData? hostVenueData = JsonConvert.DeserializeObject<HostVenueData>(hostDataString);
                            if (hostVenueData != null)
                            {
                                float distance = CalculateHaversineDistance(hostVenueData.Latitude, hostVenueData.Longitude, playerLatitude, playerLongitude);
                                if (distance <= hostVenueData.Radius)
                                {
                                    hostID = item.Key;
                                    break;
                                }
                            }
                        }
                    }
                    catch (JsonReaderException ex)
                    {
                        _logger.LogError($"Error deserializing RegisterResponse: {ex.Message}");
                    }
                }
            }
            return hostID;
        }
    }
}
