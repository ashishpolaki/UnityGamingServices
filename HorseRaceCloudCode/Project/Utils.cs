﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

namespace HorseRaceCloudCode
{
    public class Utils
    {
        //The method will parse the string into a datetime format
        public static DateTime ParseDateTime(string dateTimeString)
        {
            DateTime dateTime = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(dateTimeString))
            {
                if(!DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    return dateTime;
                }
            }
            return dateTime;
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
         }
}
