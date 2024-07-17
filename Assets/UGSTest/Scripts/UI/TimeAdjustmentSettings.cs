using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeAdjustmentSettings : MonoBehaviour
{
    [SerializeField] private Button hourUpBtn;
    [SerializeField] private Button hourDownBtn;
    [SerializeField] private Button minutesUpBtn;
    [SerializeField] private Button minutesDownBtn;
    [SerializeField] private Button meridiemBtn;
    [SerializeField] private TextMeshProUGUI hourText;
    [SerializeField] private TextMeshProUGUI minutesText;
    [SerializeField] private TextMeshProUGUI meridiemText;

    [SerializeField] private int hourChangeStep = 1;
    [SerializeField] private int minuteChangeStep = 30;

    [SerializeField] private int hourMinLimit = 1;
    [SerializeField] private int hourMaxLimit = 12;
    [SerializeField] private int minuteMinLimit = 0;
    [SerializeField] private int minuteMaxLimit = 59;

    private bool isAM;
    private bool IsAM
    {
        get
        {
            return isAM;
        }
        set
        {
            isAM = value;
            meridiemText.text = isAM ? "AM" : "PM";
        }
    }
    private void OnEnable()
    {
        hourUpBtn.onClick.AddListener(() => AdjustTime(hourText, -hourChangeStep, hourMinLimit, hourMaxLimit));
        hourDownBtn.onClick.AddListener(() => AdjustTime(hourText, hourChangeStep, hourMinLimit, hourMaxLimit));
        minutesUpBtn.onClick.AddListener(() => AdjustTime(minutesText, -minuteChangeStep, minuteMinLimit, minuteMaxLimit));
        minutesDownBtn.onClick.AddListener(() => AdjustTime(minutesText, minuteChangeStep, minuteMinLimit, minuteMaxLimit));
        meridiemBtn.onClick.AddListener(() => IsAM = !IsAM);
    }
    private void OnDisable()
    {
        hourUpBtn.onClick.RemoveAllListeners();
        hourDownBtn.onClick.RemoveAllListeners();
        minutesUpBtn.onClick.RemoveAllListeners();
        minutesDownBtn.onClick.RemoveAllListeners();
        meridiemBtn.onClick.RemoveAllListeners();
    }

    private void AdjustTime(TextMeshProUGUI timeText, int adjustment, int min, int max)
    {
        int timeValue = int.Parse(timeText.text);
        timeValue += adjustment;

        // Adjust for the min-max format
        if (timeValue < min)
        {
            timeValue = max;
        }
        else if (timeValue > max)
        {
            timeValue = min;
        }
        timeText.text = timeValue.ToString("D2"); // Formats the number to have at least two digits
    }

    public string ReturnTime()
    {
        return $"{hourText.text}:{minutesText.text} {meridiemText.text}";
    }

}
