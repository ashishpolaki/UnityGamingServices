using System.Collections;
using System.Collections.Generic;
using UI.Screen;
using UnityEngine;

namespace UI
{
    public enum UIScreenEvent
    {
        Open,
        Close,
        Show,
        Hide
    }
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;

        [SerializeField] private UIScreenManager screenManager;

        private Dictionary<ScreenType, BaseScreen> screenDictionary = new Dictionary<ScreenType, BaseScreen>();
        private BaseScreen currentActiveScreen;

        private void Awake()
        {
            // Check if instance already exists
            if (Instance == null)
            {
                // If not, set instance to this
                Instance = this;
                // Prevents the object from being destroyed when loading a new scene
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                // If instance already exists and it's not this, then destroy this to enforce the singleton pattern
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            screenDictionary.Clear();
            currentActiveScreen = null;
        }

        public void ScreenEvent(ScreenType screenType, UIScreenEvent uIScreenEvent)
        {
            currentActiveScreen = screenManager.ScreenEvent(screenType, uIScreenEvent);
        }

    }
}
