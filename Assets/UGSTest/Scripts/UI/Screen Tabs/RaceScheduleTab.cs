using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RaceScheduleTab : BaseTab
    {
        [SerializeField] private InputField timeGap_Input;
        [SerializeField] private InputField preRaceWaitTime_Input;
        [SerializeField] private Button setScheduleBtn;
        [SerializeField] private TextMeshProUGUI errorMessageTxt;

        [System.Serializable]
        private class ScheduleTime
        {
            public bool IsAM
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
            private bool isAM;
            public Button hourUpBtn;
            public Button hourDownBtn;
            public Button minutesUpBtn;
            public Button minutesDownBtn;
            public Button meridiemBtn;
            public TextMeshProUGUI hourText;
            public TextMeshProUGUI minutesText;
            public TextMeshProUGUI meridiemText;
        }
        [SerializeField] private ScheduleTime startSchedule;
        [SerializeField] private ScheduleTime endSchedule;
        [SerializeField] private int hourChangeStep = 1;
        [SerializeField] private int minuteChangeStep = 30;

        [SerializeField] private int hourMinLimit = 1;
        [SerializeField] private int hourMaxLimit = 12;
        [SerializeField] private int minuteMinLimit = 0;
        [SerializeField] private int minuteMaxLimit = 59;

        private void OnEnable()
        {
            setScheduleBtn.onClick.AddListener(OnSetScheduleBtnClick);

            //Start Schedule
            startSchedule.hourUpBtn.onClick.AddListener(() => AdjustTime(startSchedule.hourText, -hourChangeStep, hourMinLimit, hourMaxLimit));
            startSchedule.hourDownBtn.onClick.AddListener(() => AdjustTime(startSchedule.hourText, hourChangeStep, hourMinLimit, hourMaxLimit));
            startSchedule.minutesUpBtn.onClick.AddListener(() => AdjustTime(startSchedule.minutesText, -minuteChangeStep, minuteMinLimit, minuteMaxLimit));
            startSchedule.minutesDownBtn.onClick.AddListener(() => AdjustTime(startSchedule.minutesText, minuteChangeStep, minuteMinLimit, minuteMaxLimit));
            startSchedule.meridiemBtn.onClick.AddListener(() => startSchedule.IsAM = !startSchedule.IsAM);

            // End Schedule
            endSchedule.hourUpBtn.onClick.AddListener(() => AdjustTime(endSchedule.hourText, -hourChangeStep, hourMinLimit, hourMaxLimit));
            endSchedule.hourDownBtn.onClick.AddListener(() => AdjustTime(endSchedule.hourText, hourChangeStep, hourMinLimit, hourMaxLimit));
            endSchedule.minutesUpBtn.onClick.AddListener(() => AdjustTime(endSchedule.minutesText, -minuteChangeStep, minuteMinLimit, minuteMaxLimit));
            endSchedule.minutesDownBtn.onClick.AddListener(() => AdjustTime(endSchedule.minutesText, minuteChangeStep, minuteMinLimit, minuteMaxLimit));
            endSchedule.meridiemBtn.onClick.AddListener(() => endSchedule.IsAM = !endSchedule.IsAM);
        }

        private void OnDisable()
        {
            setScheduleBtn.onClick.RemoveListener(OnSetScheduleBtnClick);
            //Start Schedule
            startSchedule.hourUpBtn.onClick.RemoveAllListeners();
            startSchedule.hourDownBtn.onClick.RemoveAllListeners();
            startSchedule.minutesUpBtn.onClick.RemoveAllListeners();
            startSchedule.minutesDownBtn.onClick.RemoveAllListeners();
            startSchedule.meridiemBtn.onClick.RemoveAllListeners();

            //End Schedule
            endSchedule.hourUpBtn.onClick.RemoveAllListeners();
            endSchedule.hourDownBtn.onClick.RemoveAllListeners();
            endSchedule.minutesUpBtn.onClick.RemoveAllListeners();
            endSchedule.minutesDownBtn.onClick.RemoveAllListeners();
            endSchedule.meridiemBtn.onClick.RemoveAllListeners();
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

        /// <summary>
        /// Converts the local time string to UTC time
        /// </summary>
        /// <param name="timeString"></param>
        /// <returns></returns>
        private string ConvertToUTC(string timeString)
        {
            DateTime localDateTime = ConvertToDateTime(timeString);
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, localTimeZone);
            return utcDateTime.ToString("HH:mm");
        }

        /// <summary>
        /// Converts the time string to DateTime
        /// </summary>
        /// <param name="timeString"></param>
        /// <returns></returns>
        private DateTime ConvertToDateTime(string timeString)
        {
            DateTime dateTime = DateTime.ParseExact(timeString, "hh:mm tt", CultureInfo.InvariantCulture);
            return dateTime;
        }

        private void OnSetScheduleBtnClick()
        {
            string scheduleStartString = $"{startSchedule.hourText.text}:{startSchedule.minutesText.text} {startSchedule.meridiemText.text}";
            string scheduleEndString = $"{endSchedule.hourText.text}:{endSchedule.minutesText.text} {endSchedule.meridiemText.text}";
            //Convert to Utc
            scheduleStartString = ConvertToUTC(scheduleStartString);
            scheduleEndString = ConvertToUTC(scheduleEndString);

            // Check if start and end schedule strings are equal
            if (scheduleStartString.Equals(scheduleEndString, StringComparison.OrdinalIgnoreCase))
            {
                // If they are equal, return from the method
                errorMessageTxt.text = "Start and End time cannot be the same";
                return;
            }


            //Check if time gap is empty
            if (string.IsNullOrEmpty(timeGap_Input.text))
            {
                errorMessageTxt.text = "Please enter the time gap";
                return;
            }

            //Check if prerace wait time is empty
            if (string.IsNullOrEmpty(preRaceWaitTime_Input.text))
            {
                errorMessageTxt.text = "Please enter the Lobby wait Time";
                return;
            }

            //Set the schedule
            GameManager.Instance.CloudCode.ScheduleRaceTime(GameManager.Instance.PlayerLoginData.PlayerID, new UGS.CloudCode.RaceSchedule
            {
                ScheduleStart = scheduleStartString,
                ScheduleEnd = scheduleEndString,
                TimeGap = timeGap_Input.text,
                PreRaceWaitTime = preRaceWaitTime_Input.text
            });
            Close();
        }
    }
}
