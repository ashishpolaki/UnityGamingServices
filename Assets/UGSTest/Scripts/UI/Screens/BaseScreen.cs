using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Tabs;

namespace UI.Screen
{
    public class BaseScreen : MonoBehaviour, IScreen
    {
        [SerializeField] private ScreenType screenType;
        [SerializeField] private ScreenTabType defaultOpenTab;
        [SerializeField] private List<BaseTab> tabs;

        public ScreenType ScreenType => screenType;
        public List<BaseTab> Tabs { get => tabs; }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            if(defaultOpenTab != ScreenTabType.None)
            {
                OpenTab(defaultOpenTab);
            }
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OpenTab(ScreenTabType screenTabType)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                if (Tabs[i].ScreenTabType == screenTabType)
                {
                    Tabs[i].Open();
                    break;
                }
            }
        }

    }
    public interface IScreen
    {
        public ScreenType ScreenType { get; }
        public List<BaseTab> Tabs { get; }
        public void Open();
        public void Close();
        public void Show();
        public void Hide();
    }
    public enum ScreenType
    {
        Login,
        RegisterVenue,
        None
    }
}
