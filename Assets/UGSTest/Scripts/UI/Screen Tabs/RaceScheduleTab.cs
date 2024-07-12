using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RaceScheduleTab : BaseTab
    {
        [SerializeField] private InputField startSchedule_Input;
        [SerializeField] private InputField endSchedule_Input;
        [SerializeField] private InputField timeGap_Input;
        [SerializeField] private InputField preRaceWaitTime_Input;
        [SerializeField] private Button setScheduleBtn;

        private void OnEnable()
        {
            setScheduleBtn.onClick.AddListener(OnSetScheduleBtnClick);
        }
        private void OnDisable()
        {
            setScheduleBtn.onClick.RemoveListener(OnSetScheduleBtnClick);
        }

        private void OnSetScheduleBtnClick()
        {

            Close();
            // Set the schedule
            //GameManager.Instance.CloudCode.SetRaceSchedule(new UGS.CloudCode.RaceSchedule
            //{
            //    ScheduleStart = startSchedule_Input.text,
            //    ScheduleEnd = endSchedule_Input.text,
            //    TimeGap = timeGap_Input.text
            //});
        }
    }
}
