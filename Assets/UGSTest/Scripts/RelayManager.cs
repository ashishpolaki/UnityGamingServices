using UI;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
   
    private void Start()
    {
        UIController.Instance.ScreenEvent(UI.Screen.ScreenType.Login, UIScreenEvent.Open);
    }
}