using System;
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
            UIController.Instance.ScreenEvent(ScreenType, UIScreenEvent.Hide);
            UIController.Instance.ScreenEvent(ScreenType.RegisterVenue, UIScreenEvent.Open);
        }
        private void StartRace()
        {

        }
        private void ScheduleRace()
        {
            OpenTab(ScreenTabType.RaceSchedule);
        }
    }
}
