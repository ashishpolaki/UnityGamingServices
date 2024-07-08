using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace UI.Screen
{
    [System.Serializable]
    public class UIScreenManager
    {
        [SerializeField] private ScreenConfigSO screenConfig;

        private UIHolder uiHolder;
        private Dictionary<ScreenType, BaseScreen> screenDictionary = new Dictionary<ScreenType, BaseScreen>();
        private BaseScreen currentActiveScreen;

        public void Initialize()
        {
            if (uiHolder == null)
            {
                uiHolder = GameObject.FindObjectOfType<UIHolder>();
            }
        }

        public void ScreenEvent(ScreenType _screenType, UIScreenEvent uIEvent)
        {
            switch (uIEvent)
            {
                case UIScreenEvent.Open:
                    OpenScreen(_screenType);
                    break;
                case UIScreenEvent.Close:
                    CloseScreen(_screenType);
                    break;
                case UIScreenEvent.Show:
                    ShowScreen(_screenType);
                    break;
                case UIScreenEvent.Hide:
                    HideScreen(_screenType);
                    break;
            }
        }

        private void OpenScreen(ScreenType _screenType)
        {
            InstantiateIfDoesntExist(_screenType);
            currentActiveScreen = screenDictionary[_screenType];
            currentActiveScreen.Open();
        }
        private void CloseScreen(ScreenType _screenType)
        {
            if (screenDictionary.ContainsKey(_screenType))
            {
                screenDictionary[_screenType].Close();
            }
        }
        private void ShowScreen(ScreenType _screenType)
        {
            InstantiateIfDoesntExist(_screenType);
            currentActiveScreen = screenDictionary[_screenType];
            currentActiveScreen.Show();
        }
        private void HideScreen(ScreenType _screenType)
        {
            if (screenDictionary.ContainsKey(_screenType))
            {
                screenDictionary[_screenType].Hide();
            }
        }


        /// <summary>
        /// Instantiate the screen if it doesn't exist in the dictionary
        /// </summary>
        /// <param name="_screenType"></param>
        private void InstantiateIfDoesntExist(ScreenType _screenType)
        {
            if (!screenDictionary.ContainsKey(_screenType))
            {
                var screen = InstantiateScreen(_screenType);
                screenDictionary.Add(_screenType, screen);
            }
        }

        /// <summary>
        /// Instantiate the screen based on the screen type
        /// </summary>
        /// <param name="screenType"></param>
        /// <returns></returns>
        private BaseScreen InstantiateScreen(ScreenType screenType)
        {
            BaseScreen screen = null;

            foreach (var item in screenConfig.screens)
            {
                if (item.ScreenType == screenType)
                {
                    screen = GameObject.Instantiate(item, uiHolder.transform);
                    break;
                }
            }
            return screen;
        }

    }
}