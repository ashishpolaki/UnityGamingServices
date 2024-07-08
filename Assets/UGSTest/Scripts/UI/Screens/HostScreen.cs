using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class HostScreen : BaseScreen
    {
        [SerializeField] private Button startRaceBtn;
        [SerializeField] private Button registerVenueBtn;

        private void Awake()
        {
            startRaceBtn.onClick.AddListener(() => StartRace());
            registerVenueBtn.onClick.AddListener(() => RegisterVenue());
        }
        private void OnDestroy()
        {
            startRaceBtn.onClick.RemoveAllListeners();
            registerVenueBtn.onClick.RemoveAllListeners();
        }
        private void RegisterVenue()
        {
            UIController.Instance.ScreenEvent(ScreenType, UIScreenEvent.Hide);
            UIController.Instance.ScreenEvent(ScreenType.RegisterVenue, UIScreenEvent.Open);
        }
        private void StartRace()
        {

        }
    }
}
