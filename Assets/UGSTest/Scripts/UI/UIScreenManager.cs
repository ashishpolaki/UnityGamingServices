using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Screen
{
    [System.Serializable]
    public class UIScreenManager 
    {
        [SerializeField] private ScreenConfigSO screenConfig;
        [SerializeField] private UIHolder uiHolder;

        private Dictionary<ScreenType, BaseScreen> screenDictionary = new Dictionary<ScreenType, BaseScreen>();

        public void Initialize()
        {
            if (uiHolder == null)
            {
                uiHolder = GameObject.FindObjectOfType<UIHolder>();
            }
        }

        public void ScreenEvent(ScreenType _screenType, UIScreenEvent uIEvent)
        {
            if (uIEvent == UIScreenEvent.Open || uIEvent == UIScreenEvent.Show)
            {
                //If screen is not in the dictionary, instantiate it
                if (!screenDictionary.ContainsKey(_screenType))
                {
                    var screen = InstantiateScreen(_screenType);
                    screenDictionary.Add(_screenType, screen);
                }
              
                screenDictionary[_screenType].Open();
            }
        }

        public BaseScreen InstantiateScreen(ScreenType screenType)
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