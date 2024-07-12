using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class CharacterCustomisationScreen : BaseScreen
    {
        private void Start()
        {
            //If player name is empty then open player name tab
            if (string.IsNullOrEmpty(GameManager.Instance.PlayerLoginData.PlayerName))
            {
                OpenTab(ScreenTabType.PlayerName);
            }
            else
            {
                //Open Role Selection Tab
                OpenTab(ScreenTabType.RoleSelection);
            }
        }
    }
}
