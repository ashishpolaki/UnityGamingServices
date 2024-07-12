using UnityEngine;

namespace UI.Screen.Tab
{
    public class BaseTab : MonoBehaviour, IScreenTab
    {
        [SerializeField] private ScreenTabType screenTabType;

        public ScreenTabType ScreenTabType => screenTabType;
        public bool IsOpen { get; private set; }

        public virtual void Close()
        {
            gameObject.SetActive(false);
            IsOpen = false;
        }
        public virtual void Open()
        {
            gameObject.SetActive(true);
            IsOpen = true;
        }
    }
    public interface IScreenTab
    {
        public ScreenTabType ScreenTabType { get; }
        public bool IsOpen { get; }
        public void Open();
        public void Close();
    }
}
public enum ScreenTabType
{
    None,
    Login,
    Register,
    RaceSchedule,
    PlayerName,
    RoleSelection,
}
