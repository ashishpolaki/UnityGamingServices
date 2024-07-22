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

        public bool IsScreenOpen { get => gameObject.activeSelf; }
        public bool CantGoBack { get => cantGoBack; private set => cantGoBack = value; }
        public bool IsAnyTabOpened { get => tabs.Exists(tab => tab.IsOpen); }

        private  bool cantGoBack;

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
            foreach (var tab in Tabs)
            {
                if (tab.IsOpen)
                {
                    tab.Close();
                }
            }
            gameObject.SetActive(false);
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

        public virtual void OnScreenBack()
        {
            //Close the tab that is open and then return.
            foreach (var tab in Tabs)
            {
                if (tab.IsOpen)
                {
                    tab.Close();
                    CantGoBack = true;
                    return;
                }
            }
            CantGoBack = false;
        }
    }
    public interface IScreen
    {
        public List<BaseTab> Tabs { get; }
        public ScreenType ScreenType { get; }
        public ScreenTabType DefaultOpenTab { get; }
        public bool IsScreenOpen { get; }
        public bool CantGoBack { get; }
        public void OnScreenBack();
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
    Client,
    None
}
