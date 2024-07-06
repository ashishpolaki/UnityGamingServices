using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Screen
{
    [System.Serializable]
    public class UIScreenManager
    {
        [SerializeField] private ScreenConfigSO screenConfig;
        [SerializeField] private Canvas canvas;

        public BaseScreen ScreenEvent(ScreenType _screenType, UIScreenEvent uIEvent)
        {
            BaseScreen screen = default;

            foreach (var item in screenConfig.screens)
            {
                if (item.ScreenType == _screenType)
                {
                    screen = item;
                    break;
                }
            }

            switch (uIEvent)
            {
                case UIScreenEvent.Open:
                    screen.Open();
                    break;
                case UIScreenEvent.Close:
                    screen.Close();
                    break;
                case UIScreenEvent.Show:
                    screen.Show();
                    break;
                case UIScreenEvent.Hide:
                    screen.Hide();
                    break;
            }
            return screen;
        }
    }
}