using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace UI.Screen
{
    public class LoginAuthenticationScreen : BaseScreen
    {
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public Button signUpBtn;
        public Button signInBtn;
        public Button signOutBtn;
        public Button signInAnonymousBtn;

        private void Awake()
        {
            signUpBtn.onClick.AddListener(() => SignUp());
            signInBtn.onClick.AddListener(() => SignIn());
            signOutBtn.onClick.AddListener(() => SignOut());
            signInAnonymousBtn.onClick.AddListener(() => SignUpAnonymously());
        }

        private void SignOut()
        {
        }
        private void SignIn()
        {
            string username = usernameInput.text;
            string password = passwordInput.text;
        }
        private void SignUp()
        {
            string username = usernameInput.text;
            string password = passwordInput.text;

        }
        private void SignUpAnonymously()
        {

        }

    }
}