using System.Collections.Generic;
using UI.Screen;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        #region Inspector Variables
        [SerializeField] private UIScreenManager screenManager;
        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            screenManager.Initialize();
        }
        private void OnDestroy()
        {
            //   currentActiveScreen = null;
        }

        public void ScreenEvent(ScreenType screenType, UIScreenEvent uIScreenEvent)
        {
            /*currentActiveScreen =*/
            screenManager.ScreenEvent(screenType, uIScreenEvent);
        }
    }
}
