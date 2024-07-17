using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class HostScreen : BaseScreen
    {
        [SerializeField] private Button startRaceBtn;
        [SerializeField] private Button registerVenueBtn;
        [SerializeField] private Button scheduleRaceBtn;

        private void Awake()
        {
            startRaceBtn.onClick.AddListener(() => StartRace());
            registerVenueBtn.onClick.AddListener(() => RegisterVenue());
            scheduleRaceBtn.onClick.AddListener(() => ScheduleRace());
        }
        private void OnDestroy()
        {
            startRaceBtn.onClick.RemoveAllListeners();
            registerVenueBtn.onClick.RemoveAllListeners();
            scheduleRaceBtn.onClick.RemoveAllListeners();
        }
        private void RegisterVenue()
        {
           OpenTab(ScreenTabType.RegisterVenue);
        }
        private void StartRace()
        {
            OpenTab(ScreenTabType.RaceLobby);
        }
        private void ScheduleRace()
        {
            OpenTab(ScreenTabType.RaceSchedule);
        }
    }
}
