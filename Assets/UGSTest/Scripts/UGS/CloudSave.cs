using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;

namespace UGS
{
    public class CloudSave
    {
        public class HostVenueData
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public float Radius { get; set; }
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
        }
        public async Task<string> GetHostID(string customID, double latitude, double longitude)
        {
            var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAllAsync(customID);
            foreach (var customItem in customItemData)
            {
                string customItemValue = customItem.Value.Value.GetAsString();
                if (!string.IsNullOrEmpty(customItemValue) && !string.IsNullOrWhiteSpace(customItemValue))
                {
                    HostVenueData hostVenueData = JsonConvert.DeserializeObject<HostVenueData>(customItemValue);
                    float distance = DistanceCalculator.CalculateHaversineDistance(hostVenueData.Latitude, hostVenueData.Longitude, latitude, longitude);
                    if (distance <= hostVenueData.Radius)
                    {
                        return customItem.Key;
                    }
                }
            }
            return string.Empty;
        }

        public async Task<string> TryGetPlayerLobbyData(string hostID, string playerID, string key)
        {
            var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAsync(hostID, new HashSet<string> { key });

            if (customItemData.TryGetValue(key, out var item))
            {
                string raceItemValue = item.Value.GetAs<string>();
                if (!string.IsNullOrEmpty(raceItemValue) && !string.IsNullOrWhiteSpace(raceItemValue))
                {
                    List<RaceLobbyParticipant> raceLobbyData = JsonConvert.DeserializeObject<List<RaceLobbyParticipant>>(raceItemValue);
                    foreach (var raceLobbyParticipant in raceLobbyData)
                    {
                        if (raceLobbyParticipant.PlayerID == playerID)
                        {
                            return JsonConvert.SerializeObject(raceLobbyParticipant);
                        }
                    }
                }
            }
            return string.Empty;
        }

        public async Task<bool> IsPlayerAlreadyCheckIn(string hostID, string playerID, string key)
        {
            var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAsync(hostID, new HashSet<string> { key });

            if (customItemData.TryGetValue(key, out var item))
            {
                string raceItemValue = item.Value.GetAs<string>();
                if (!string.IsNullOrEmpty(raceItemValue) && !string.IsNullOrWhiteSpace(raceItemValue))
                {
                    List<CurrentRacePlayerCheckIn> raceLobbyData = JsonConvert.DeserializeObject<List<CurrentRacePlayerCheckIn>>(raceItemValue);
                    foreach (var raceLobby in raceLobbyData)
                    {
                        if (raceLobby.PlayerID == playerID)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public async Task<string> GetRaceCheckInParticipants(string hostID, string key)
        {
            var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAsync(hostID, new HashSet<string> { key });

            if (customItemData.TryGetValue(key, out var item))
            {
                string raceItemValue = item.Value.GetAs<string>();
                if (!string.IsNullOrEmpty(raceItemValue) && !string.IsNullOrWhiteSpace(raceItemValue))
                {
                    List<CurrentRacePlayerCheckIn> checkinPlayers = JsonConvert.DeserializeObject<List<CurrentRacePlayerCheckIn>>(raceItemValue);
                    return JsonConvert.SerializeObject(checkinPlayers);
                }
            }
            return string.Empty;
        }

        public async Task<string> GetRaceParticipants(string hostID, string key)
        {
            var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAsync(hostID, new HashSet<string> { key });

            if (customItemData.TryGetValue(key, out var item))
            {
                string raceItemValue = item.Value.GetAs<string>();
                if (!string.IsNullOrEmpty(raceItemValue) && !string.IsNullOrWhiteSpace(raceItemValue))
                {
                    List<CurrentRacePlayerCheckIn> checkinPlayers = JsonConvert.DeserializeObject<List<CurrentRacePlayerCheckIn>>(raceItemValue);
                    return JsonConvert.SerializeObject(checkinPlayers);
                }
            }
            return string.Empty;
        }

        public async Task<string> TryGetRaceLobby(string hostID, string key)
        {
            var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAsync(hostID, new HashSet<string> { key });

            if (customItemData.TryGetValue(key, out var item))
            {
                string raceItemValue = item.Value.GetAs<string>();
                if (!string.IsNullOrEmpty(raceItemValue) && !string.IsNullOrWhiteSpace(raceItemValue))
                {
                    List<RaceLobbyParticipant> raceLobbyData = JsonConvert.DeserializeObject<List<RaceLobbyParticipant>>(raceItemValue);
                    return JsonConvert.SerializeObject(raceLobbyData);
                }
            }
            return string.Empty;
        }

        public async Task<string> TryGetPlayerRaceResult(string hostID, string playerID, string key)
        {
            var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAsync(hostID, new HashSet<string> { key });

            if (customItemData.TryGetValue(key, out var item))
            {
                string raceResult = item.Value.GetAs<string>();
                if (!string.IsNullOrEmpty(raceResult) && !string.IsNullOrWhiteSpace(raceResult))
                {
                    RaceResult raceResults = JsonConvert.DeserializeObject<RaceResult>(raceResult);
                    foreach (var raceResultParticipant in raceResults.playerRaceResults)
                    {
                        if (raceResultParticipant.PlayerID == playerID)
                        {
                            return JsonConvert.SerializeObject(raceResultParticipant);
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}
