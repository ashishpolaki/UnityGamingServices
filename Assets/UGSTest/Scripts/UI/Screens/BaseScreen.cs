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

        public virtual void Open(ScreenTabType screenTabType)
        {
            gameObject.SetActive(true);
            //If screenTabType is not None then open the tab
            if (screenTabType != ScreenTabType.None)
            {
                OpenTab(screenTabType);
            }
            else
            {
                //else if defaultOpenTab is not None then open the defaulttab
                if (defaultOpenTab != ScreenTabType.None)
                {
                    OpenTab(defaultOpenTab);
                }
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
        public virtual void Show(ScreenTabType screenTabType)
        {
            gameObject.SetActive(true);
            if (screenTabType != ScreenTabType.None)
            {
                OpenTab(screenTabType);
            }
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
        public void Open(ScreenTabType screenTabType);
        public void Close();
        public void Show(ScreenTabType screenTabType);
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
