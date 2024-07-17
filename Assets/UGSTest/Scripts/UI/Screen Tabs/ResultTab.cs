using TMPro;
using UnityEngine;

namespace UI.Screen.Tab
{
    public class ResultTab : BaseTab
    {
        [SerializeField] private TextMeshProUGUI resultText;

        private void Start()
        {
            resultText.text = GameManager.Instance.GameData.IsRaceWinner() ?
                 "Victory is yours! Well done!" : "Better luck next time!";
        }
    }
}