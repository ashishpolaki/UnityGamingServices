using TMPro;
using UnityEngine;

namespace UI.Screen.Tab
{
    public class ResultTab : BaseTab
    {
        [SerializeField] private TextMeshProUGUI resultText;

        private void Start()
        {
            resultText.text = $"Your race position is {GameManager.Instance.GameData.RaceResult.RacePosition}";
        }
    }
}