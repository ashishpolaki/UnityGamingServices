using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Screen
{
    public class BaseScreen : MonoBehaviour, IScreen
    {
        [SerializeField] private ScreenType screenType;

        public ScreenType ScreenType => screenType;

        public virtual void Open()
        {
            gameObject.SetActive(true);
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

    }
    public interface IScreen
    {
        public ScreenType ScreenType { get; }

        public void Open();
        public void Close();
        public void Show();
        public void Hide();
    }
    public enum ScreenType
    {
        Login,
        None
    }
}
