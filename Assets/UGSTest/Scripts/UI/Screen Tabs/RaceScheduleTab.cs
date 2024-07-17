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

        [SerializeField] private TimeAdjustmentSettings startSchedule;
        [SerializeField] private TimeAdjustmentSettings endSchedule;

        private void OnEnable()
        {
            setScheduleBtn.onClick.AddListener(OnSetScheduleBtnClick);
        }

        private void OnDisable()
        {
            setScheduleBtn.onClick.RemoveListener(OnSetScheduleBtnClick);
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
            string scheduleStartString = $"{startSchedule.ReturnTime()}";
            string scheduleEndString = $"{endSchedule.ReturnTime()}";
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
            GameManager.Instance.CloudCode.ScheduleRaceTime(new UGS.CloudCode.RaceSchedule
            {
                ScheduleStart = scheduleStartString,
                ScheduleEnd = scheduleEndString,
                TimeGap = int.Parse(timeGap_Input.text),
                PreRaceWaitTime = int.Parse(preRaceWaitTime_Input.text)
            });
            Close();
        }
    }
}
