using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Screen.Tab;

namespace UI.Screen
{
    public class BaseScreen : MonoBehaviour, IScreen
    {
        [SerializeField] private ScreenType screenType;
        [SerializeField] private ScreenTabType defaultOpenTab;
        [SerializeField] private List<BaseTab> tabs;

        public ScreenType ScreenType => screenType;
        public List<BaseTab> Tabs { get => tabs; }
        public ScreenTabType DefaultOpenTab { get => defaultOpenTab; }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            if (defaultOpenTab != ScreenTabType.None)
            {
                OpenTab(defaultOpenTab);
            }
        }
        public virtual void Close()
        {
            gameObject.SetActive(false);
            foreach (var tab in Tabs)
            {
                if (tab.IsOpen)
                {
                    tab.Close();
                }
            }
        }
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OpenTab(ScreenTabType screenTabType)
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
        public void CloseTab(ScreenTabType screenTabType)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                if (Tabs[i].ScreenTabType == screenTabType)
                {
                    Tabs[i].Close();
                    break;
                }
            }
        }
        public void CloseAllTabs()
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                Tabs[i].Close();
            }
        }
    }
    public interface IScreen
    {
        public ScreenType ScreenType { get; }
        public List<BaseTab> Tabs { get; }
        public ScreenTabType DefaultOpenTab { get; }
        public void Open();
        public void Close();
        public void Show();
        public void Hide();
    }
}
public enum ScreenType
{
    Login,
    RegisterVenue,
    CharacterCustomization,
    Host,
    None
}
