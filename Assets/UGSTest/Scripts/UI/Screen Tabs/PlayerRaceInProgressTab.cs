using TMPro;
using UnityEngine;

namespace UI.Screen.Tab
{
    public class PlayerRaceInProgressTab : BaseTab
    {
        [SerializeField] private TextMeshProUGUI horseNumberTxt;

        private void Start()
        {
            if (UGSManager.Instance.GameData.HorseNumber == 0)
            {
                horseNumberTxt.text = $"“Your ticket was not selected. The longer you spend at the venue and check-in, the greater the odds.”";
            }
            else
            {
                horseNumberTxt.text = $"Horse Number : {UGSManager.Instance.GameData.HorseNumber}";
            }
        }
    }
}
