using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogin : MonoBehaviour
{
    void Start()
    {
        UI.UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Open);
    }
}
