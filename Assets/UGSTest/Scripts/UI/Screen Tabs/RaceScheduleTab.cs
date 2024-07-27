using System;
using System.Globalization;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RaceScheduleTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private InputField timeGap_Input;
        [SerializeField] private InputField preRaceWaitTime_Input;
        [SerializeField] private Button setScheduleBtn;
        [SerializeField] private TextMeshProUGUI errorMessageTxt;

        [SerializeField] private TimeAdjustmentSettings startSchedule;
        [SerializeField] private TimeAdjustmentSettings endSchedule;
        #endregion

        #region Private Variables
        private string raceStartSchedule;
        private string raceEndSchedule;
        private int timeGap;
        private int preRaceWaitTime;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            setScheduleBtn.onClick.AddListener(OnSetScheduleBtnClick);
        }
        private void OnDisable()
        {
            setScheduleBtn.onClick.RemoveListener(OnSetScheduleBtnClick);
        }
        #endregion

        #region Private Methods
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

        private async void OnSetScheduleBtnClick()
        {
            if (!IsRaceScheduleValid() || !IsTimeGapValid() || !IsPreRaceWaitTimeValid())
            {
                return;
            }

            //Set the schedule
            Func<Task> method = () => UGSManager.Instance.CloudCode.ScheduleRaceTime(new UGS.CloudCode.HostScheduleRace
            {
                ScheduleStart = raceStartSchedule,
                ScheduleEnd = raceEndSchedule,
                TimeGap = timeGap,
                PreRaceWaitTime = preRaceWaitTime
            });
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
            Close();
        }

        private bool IsRaceScheduleValid()
        {
            raceStartSchedule = ConvertToUTC(startSchedule.ReturnTime()); 
            raceEndSchedule = ConvertToUTC(endSchedule.ReturnTime());

            // Check if start and end schedule strings are equal
            if (raceStartSchedule.Equals(raceEndSchedule, StringComparison.OrdinalIgnoreCase))
            {
                // If they are equal, return from the method
                errorMessageTxt.text = "Start and End time cannot be the same";
                return false;
            }

            return true;
        }
        private bool IsTimeGapValid()
        {
            //Check if time gap is empty
            if (string.IsNullOrEmpty(timeGap_Input.text))
            {
                errorMessageTxt.text = "Please enter the time gap";
                return false;
            }
            timeGap = int.Parse(timeGap_Input.text);

            //Check if time gap is less than 0
            if (timeGap <= 0)
            {
                errorMessageTxt.text = "The duration between races should be greater than zero";
                return false;
            }

            //Check if duration between races is less than schedule time.
            DateTime startTime = DateTime.ParseExact(raceStartSchedule, "HH:mm", CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(raceEndSchedule, "HH:mm", CultureInfo.InvariantCulture);
            //If end time is less than start time, add a day to end time
            if (endTime < startTime)
            {
                endTime = endTime.AddDays(1);
            }
            TimeSpan raceTimeSpan = endTime - startTime;
            TimeSpan timeGapSpan = TimeSpan.FromMinutes(timeGap);
            if (raceTimeSpan < timeGapSpan)
            {
                errorMessageTxt.text = "The duration between races should be less than the race schedule.";
                return false;
            }

            return true;
        }
        private bool IsPreRaceWaitTimeValid()
        {
            //Check if prerace wait time is empty
            if (string.IsNullOrEmpty(preRaceWaitTime_Input.text))
            {
                errorMessageTxt.text = "Please enter the Lobby wait Time";
                return false;
            }
            preRaceWaitTime = int.Parse(preRaceWaitTime_Input.text);

            //Check if prerace wait time is less than 0
            if (preRaceWaitTime <= 0)
            {
                errorMessageTxt.text = "Lobby wait time should be greater than zero";
                return false;
            }

            //Check if the prerace wait time is greater than the time gap
            if (preRaceWaitTime >= timeGap)
            {
                errorMessageTxt.text = "Lobby wait time should be less than the time gap";
                return false;
            }
            return true;
        }
        #endregion


    }
}
