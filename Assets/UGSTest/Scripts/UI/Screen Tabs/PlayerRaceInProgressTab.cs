using TMPro;
using UnityEngine;

namespace UI.Screen.Tab
{
    public class PlayerRaceInProgressTab : BaseTab
    {
        [SerializeField] private TextMeshProUGUI horseNumberTxt;

        private void Start()
        {
            horseNumberTxt.text = $"Horse Number : {GameManager.Instance.GameData.HorseNumber}";
        }
    }
}
