using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.Tabs
{
    public class RegisterTab : BaseTab
    {
        [SerializeField] private TMP_InputField username_Input;
        [SerializeField] private TMP_InputField password_Input;
        [SerializeField] private TMP_InputField playername_Input;

        public override void Open()
        {
            base.Open();

        }
        public override void Close()
        {
            base.Close();

        }
    }
}
