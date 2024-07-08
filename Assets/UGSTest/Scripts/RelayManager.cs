using UnityEngine;

public class RelayManager : MonoBehaviour
{
    private void Start()
    {
       UI.UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Open);
    }
}