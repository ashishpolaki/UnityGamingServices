using UI.Screen;
using UnityEngine;

namespace UI
{
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

        public void ScreenEvent(ScreenType screenType, UIScreenEvent uIScreenEvent)
        {
            /*currentActiveScreen =*/
            screenManager.ScreenEvent(screenType, uIScreenEvent);
        }
    }
}
public enum UIScreenEvent
{
    Open,
    Close,
    Show,
    Hide
}