using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Tabs
{
    public class BaseTab : MonoBehaviour,IScreenTab
    {
        [SerializeField] private ScreenTabType screenTabType;

        public ScreenTabType ScreenTabType => screenTabType;

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }
    }
    public interface IScreenTab
    {
        public ScreenTabType ScreenTabType { get; } 
        public void Open();
        public void Close();
    }
    public enum ScreenTabType
    {
        None,
        Login,
        Register,
    }
}
