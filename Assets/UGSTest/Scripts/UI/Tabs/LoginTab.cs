using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class LoginTab : BaseTab
    {
        [SerializeField] private InputField username_Input;
        [SerializeField] private InputField password_Input;
        [SerializeField] private Button loginBtn;

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
