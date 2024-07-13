using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogin : MonoBehaviour
{
    [SerializeField] private ScreenType openScreenType = ScreenType.Login;
    void Start()
    {
        UI.UIController.Instance.ScreenEvent(openScreenType, UIScreenEvent.Open);
    }
}
