using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public enum POPUP
    {
        None = 99,
        솔레노이드밸브 = 0,
        미니솔레노이드밸브,
        동력제어반,
        완료,
        배수밸브,
        일차가압계,
        이차가압계
    }

    public class PopupManager : MonoBehaviour
    {
        private static PopupManager instance;
        public static PopupManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PopupManager();
                }
                return instance;
            }
        }

        List<Popup> list_Popup = new List<Popup>();

        [SerializeField] Popup_PowerControl powerControl_small;
        [SerializeField] Popup_PowerControl powerControl_big;
        [SerializeField] Button Button_PowerControl;

        [SerializeField] Popup_Gauge Popup_Gauge_1;
        [SerializeField] Popup_Gauge Popup_Gauge_2;

        [SerializeField] Hint Hint;

        public POPUP CurrentOpenPopup
        {
            get { return currentOpenPopup; }
        }
        POPUP currentOpenPopup = POPUP.None;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            foreach (Transform t in transform)
            {
                list_Popup.Add(t.GetComponent<Popup>());
                t.GetComponent<Popup>().FirstSetting();
            }

            Button_PowerControl.onClick.AddListener(OpenPowerControl);

            HighLight.Instance.OnHighLight += OnHighLight;
        }

        void OnHighLight()
        {
            if(currentOpenPopup != POPUP.동력제어반)
            {
                HighLight.Instance.On(Button_PowerControl);
            }
        }

        public void ResetPopup(POPUP index)
        {
            list_Popup[(int)index].Reset();
        }

        public void Open(POPUP index)
        {
            if(index == POPUP.완료)
            {
                SoundManager.Instance.PlayPump(false);
                
            }
            Hint.MuteHint(index == POPUP.완료);

            for (int i = 0; i < list_Popup.Count; i++)
            {
                if(i == (int)index)
                {
                    list_Popup[i].SET(true);
                    currentOpenPopup = index;
                }
                else
                {
                    list_Popup[i].SET(false);
                }
            }
        }

        public void MultyOpen(POPUP[] index)
        {
            for (int i = 0; i < index.Length; i++)
            {
                list_Popup[(int)index[i]].SET(true);
            }
        }

        /// <summary>
        /// 팝업창 모두 닫기
        /// </summary>
        public void CloseAll()
        {
            for (int i = 0; i < list_Popup.Count; i++)
            {
                list_Popup[i].SET(false);
            }
            currentOpenPopup = POPUP.None;
        }

        // 버튼 바인드
        public void OpenPowerControl()
        {
            Open(POPUP.동력제어반);
            ScenarioManager.Instance.CheckScenarioStep();
        }

        public void InteractivePowerControl(bool isActive)
        {
            if(Button_PowerControl != null)
            {
                Button_PowerControl.interactable = isActive;
            }
        }

        /// <summary>
        ///  동력제어반 파워 끄고 켜기
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        public void ControlPower(PUMP name, 점등 index, bool isActive)
        {
            powerControl_big.PumpOnOff(name, index, isActive);
            powerControl_small.PumpOnOff(name, index, isActive);
        }

        public void ControlPower(PUMP name, bool isActive)
        {
            if (name == PUMP.주펌프)
            {
                powerControl_big.OnOffMainPump(isActive);
                powerControl_small.OnOffMainPump(isActive);
            }
            if (name == PUMP.충압펌프)
            {
                powerControl_big.OnOffFillingPump(isActive);
                powerControl_small.OnOffFillingPump(isActive);
            }
        }

        /// <summary>
        /// 동력제어반 초기화 / 주펌프충압펌프 정지
        /// </summary>
        public void ControlPowerReset()
        {
            powerControl_big?.Reset();
            powerControl_small?.Reset();
            InteractivePowerControl(false);

            ControlChangeMainPumpState(상태.자동);
        }

        public bool ControlPowerCheck(PUMP pump, 점등 index)
        {
            return powerControl_big.CheckPump(pump, index);
        }

        public void ControlPowerStopButtonInteractive(bool isActive)
        {
            powerControl_big.InteractiveStopButton(isActive);
        }

        public void ControlFillingPumpStateInteractive(bool isActive)
        {
            powerControl_big.InteractiveFillingPumpState(isActive);
        }

        public bool ControlMainPumpStateCheck(상태 isState)
        {
            return powerControl_big.CheckState(isState);
        }

        public bool ControlFillingPumpStateCheck(상태 isState)
        {
            return powerControl_big.CheckState_Filling(isState);
        }

        public void ControlChangeMainPumpState(상태 state)
        {
            powerControl_big.ChanageMainPumpState(state);
        }

        public void ControlChangeFillingPumpState(상태 state)
        {
            powerControl_big.ChanageFillingPumpState(state);
        }

        // 세팅밸브

        public void FirstGauge(int gauge)
        {
            Popup_Gauge_1.ChangeNeedleRotate(gauge);
        }

        public void SecondGauge(int gauge)
        {
            Popup_Gauge_2.ChangeNeedleRotate(gauge);
        }

        public void FirstGaugeUp()
        {
            Popup_Gauge_1.Up();
        }
        public void SecondGaugeUp()
        {
            Popup_Gauge_2.Up();
        }
    }
}
