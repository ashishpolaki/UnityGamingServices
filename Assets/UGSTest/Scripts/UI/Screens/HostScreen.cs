using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class HostScreen : BaseScreen
    {
        [SerializeField] private Button startRaceBtn;
        [SerializeField] private Button registerVenueBtn;
        [SerializeField] private Button scheduleRaceBtn;
        [SerializeField] private Button backButton;

        
        private void OnEnable()
        {
            startRaceBtn.onClick.AddListener(() => StartRace());
            registerVenueBtn.onClick.AddListener(() => RegisterVenue());
            scheduleRaceBtn.onClick.AddListener(() => ScheduleRace());
            backButton.onClick.AddListener(() => OnScreenBack());
        }
        private void OnDisable()
        {
            startRaceBtn.onClick.RemoveAllListeners();
            registerVenueBtn.onClick.RemoveAllListeners();
            scheduleRaceBtn.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
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
        public override void OnScreenBack()
        {
            base.OnScreenBack();
            if (!CantGoBack)
            {
                UIController.Instance.ScreenEvent(this.ScreenType, UIScreenEvent.Close);
                UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Show);
            }
        }
    }
}
