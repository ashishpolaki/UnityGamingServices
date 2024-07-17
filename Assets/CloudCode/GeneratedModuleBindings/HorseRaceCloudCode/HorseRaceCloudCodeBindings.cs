// This file was generated by Cloud Code Bindings Generator. Modifications will be lost upon regeneration.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unity.Services.CloudCode.GeneratedBindings
{
    public class HorseRaceCloudCodeBindings
    {
        readonly ICloudCodeService k_Service;

        public HorseRaceCloudCodeBindings(ICloudCodeService service)
        {
            k_Service = service;
        }
        public async Task RegisterVenue(string _venueData)
        {
            await k_Service.CallModuleEndpointAsync("HorseRaceCloudCode", "RegisterVenue",
                new Dictionary<string, object>() { { "venueData", _venueData } });
        }
        public async Task ScheduleRaceTimings(string _raceData)
        {
            await k_Service.CallModuleEndpointAsync("HorseRaceCloudCode", "RaceScheduleTimings",
               new Dictionary<string, object>() { { "raceData", _raceData } });
        }
        public async Task<string> VenueCheckIn(double latitude, double longitude)
        {
            return await k_Service.CallModuleEndpointAsync("HorseRaceCloudCode", "CheckedInVenue",
                 new Dictionary<string, object>() { { "latitude", latitude }, { "longitude", longitude }, });
        }
        public async Task<string> JoinRace(double latitude, double longitude)
        {
            return await k_Service.CallModuleEndpointAsync<string>("HorseRaceCloudCode", "RaceJoinRequest",
                                 new Dictionary<string, object>() { { "latitude", latitude }, { "longitude", longitude }, });
        }
        public async Task<string> ConfirmRaceCheckIn(string playerID)
        {
            return await k_Service.CallModuleEndpointAsync("HorseRaceCloudCode", "ConfirmRaceCheckIn",
                                 new Dictionary<string, object>() { { "playerID", playerID } });
        }
        public async Task<string> GetLobbyPlayers()
        {
            return await k_Service.CallModuleEndpointAsync("HorseRaceCloudCode", "GetRaceCheckInPlayers");
        }
        public async Task StartRace(string horsesInRaceOrder)
        {
            await k_Service.CallModuleEndpointAsync("HorseRaceCloudCode", "StartRace",
                 new Dictionary<string, object>() { { "horsesInRaceOrder", horsesInRaceOrder } });
        }
        public async Task SendRaceResults(int raceWinnerHorse)
        {
            await k_Service.CallModuleEndpointAsync("HorseRaceCloudCode", "RaceResults",
                 new Dictionary<string, object>() { { "winnerHorseNumber", raceWinnerHorse } });
        }
        #region CheatCode
        public async Task<string> CheatVenueCheckIn(string checkInData, string _currentTime)
        {
            return await k_Service.CallModuleEndpointAsync("HorseRaceCloudCode", "CheatCheckInVenue",
                new Dictionary<string, object>() { { "checkInData", checkInData }, { "currentTime", _currentTime } });
        }

        #endregion
    }
}
