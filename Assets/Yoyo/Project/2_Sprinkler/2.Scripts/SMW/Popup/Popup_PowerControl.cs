using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public enum PUMP
    {
        주펌프,
        충압펌프
    }

    public enum 점등
    {
        기동,
        정지,
        펌프기동
    }
    public enum 상태
    {
        자동,
        수동
    }

    public class Popup_PowerControl : Popup
    {
        [Header("주펌프")]
        [SerializeField] PowerControlPump powerControlPump_main;
        [SerializeField] Button Button_MainPump_State;
        [Header("충압펌프")]
        [SerializeField] PowerControlPump powerControlPump_filling;
        [SerializeField] Button Button_FillingPump_State;
        [Header("주펌프 정지 버튼")]
        [SerializeField] Button Button_Stop_main;

        [SerializeField] List<Sprite> list_sprite_state;

        public 상태 MainPumpState
        {
            get { return mainPump_State; }
        }
        상태 mainPump_State = 상태.자동;
        상태 fillingPump_State = 상태.자동;

        List<Button> list_highLight = new List<Button>();

        public void PumpOnOff(PUMP name, 점등 index, bool isActive)
        {
            switch (name)
            {
                case PUMP.주펌프:
                    {
                        powerControlPump_main.OnOff((int)index, isActive);
                    }
                    break;
                case PUMP.충압펌프:
                    {
                        powerControlPump_filling.OnOff((int)index, isActive);
                    }
                    break;
            }
        }

        public bool CheckPump(PUMP pump, 점등 index)
        {
            bool isOn = false;
            switch (pump)
            {
                case PUMP.주펌프:
                    {
                        isOn = powerControlPump_main.Check((int)index);
                    }
                    break;
                case PUMP.충압펌프:
                    {
                        isOn = powerControlPump_filling.Check((int)index);
                    }
                    break;
            }
            return isOn;
        }

        public bool CheckState(상태 state)
        {
            if(mainPump_State == state)
            {
                if(list_highLight.Contains(Button_MainPump_State))
                {
                    list_highLight.Remove(Button_MainPump_State);
                }
                return true;
            }
            else
            {
                if(!list_highLight.Contains(Button_MainPump_State))
                {
                    list_highLight.Add(Button_MainPump_State);
                }
                return false;
            }
        }

        public bool CheckState_Filling(상태 state)
        {
            if (fillingPump_State == state)
            {
                if (list_highLight.Contains(Button_FillingPump_State))
                {
                    list_highLight.Remove(Button_FillingPump_State);
                }
                return true;
            }
            else
            {
                if (!list_highLight.Contains(Button_FillingPump_State))
                {
                    list_highLight.Add(Button_FillingPump_State);
                }
                return false;
            }
        }

        protected override void Setting()
        {
            if(Button_Stop_main != null)
            {
                Button_Stop_main.onClick.AddListener(OnClickStopMain);
            }
            if(Button_MainPump_State != null)
            {
                Button_MainPump_State.onClick.AddListener(ChangeMainPumpState);
            }
            if(Button_FillingPump_State != null)
            {
                Button_FillingPump_State.onClick.AddListener(ChangeFillingPumpState);
            }

            HighLight.Instance.OnHighLight += OnHighLight;
        }

        /// <summary>
        /// 하이라이트
        /// </summary>
        void OnHighLight()
        {
            if (Button_Stop_main == null) return;
            if(mainPump_State == 상태.수동 && !CheckPump(PUMP.주펌프, 점등.정지))
            {
                HighLight.Instance.On(Button_Stop_main);
            }
            HighLight.Instance.On(list_highLight);
        }

        public void InteractiveStopButton(bool isInteractive)
        {
            if(Button_MainPump_State != null)
            {
                Button_MainPump_State.interactable = isInteractive;
            }
        }

        public void InteractiveFillingPumpState(bool isInteractive)
        {
            if(Button_FillingPump_State != null)
            {
                Button_FillingPump_State.interactable = isInteractive;
            }
        }

        /// <summary>
        /// 주펌프 상태변환
        /// </summary>
        void ChangeMainPumpState()
        {
            mainPump_State++;
            if (mainPump_State > 상태.수동)
            {
                mainPump_State = 상태.자동;
            }
            if(mainPump_State == 상태.수동)
            {
                Button_Stop_main.interactable = true;
            }
            else
            {
                Button_Stop_main.interactable = false;
            }

            Button_MainPump_State.transform.GetComponent<Image>().sprite = list_sprite_state[(int)mainPump_State];
            ScenarioManager.Instance.CheckScenarioStep();
        }

        void ChangeFillingPumpState()
        {
            fillingPump_State++;
            if (fillingPump_State > 상태.수동)
            {
                fillingPump_State = 상태.자동;
            }

            Button_FillingPump_State.transform.GetComponent<Image>().sprite = list_sprite_state[(int)fillingPump_State];
            ScenarioManager.Instance.CheckScenarioStep();
        }

        public void ChanageMainPumpState(상태 state)
        {
            mainPump_State = state;
            Button_MainPump_State.transform.GetComponent<Image>().sprite = list_sprite_state[(int)state];
        }

        public void ChanageFillingPumpState(상태 state)
        {
            fillingPump_State = state;
            Button_FillingPump_State.transform.GetComponent<Image>().sprite = list_sprite_state[(int)state];
        }

        /// <summary>
        /// 주펌프 정지 버튼 클릭 이벤트
        /// </summary>
        void OnClickStopMain()
        {
            if(mainPump_State == 상태.수동)
            {
                OnOffMainPump(false);
                ScenarioManager.Instance.CheckScenarioStep();
            }
        }

        /// <summary>
        /// 주펌프 ON/OFF
        /// </summary>
        public void OnOffMainPump(bool isOn)
        {
            PumpOnOff(PUMP.주펌프, 점등.기동, isOn);
            PumpOnOff(PUMP.주펌프, 점등.정지, !isOn);
            PumpOnOff(PUMP.주펌프, 점등.펌프기동, isOn);
        }

        /// <summary>
        /// 충압펌프 ON/OFF
        /// </summary>
        public void OnOffFillingPump(bool isOn)
        {
            PumpOnOff(PUMP.충압펌프, 점등.기동, isOn);
            PumpOnOff(PUMP.충압펌프, 점등.정지, !isOn);
            PumpOnOff(PUMP.충압펌프, 점등.펌프기동, isOn);
        }

        protected override void RESET()
        {
            powerControlPump_filling.Reset();
            powerControlPump_main.Reset();
            ChanageMainPumpState(상태.자동);

            if (Button_Stop_main != null)
            {
                InteractiveStopButton(false);
            }

            InteractiveFillingPumpState(false);
        }
    }
}
