using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tabs
{
    public class LoginTab : BaseTab
    {
        [SerializeField] private TMP_InputField username_Input;
        [SerializeField] private TMP_InputField password_Input;
        [SerializeField] private Button loginBtn;

        private void Awake()
        {
           
        }
        public override void Open()
        {
            base.Open();
        }

        public override void Close()
        {
            base.Close();
        }
        private void OnDestroy()
        {
            
        }
    }
}
