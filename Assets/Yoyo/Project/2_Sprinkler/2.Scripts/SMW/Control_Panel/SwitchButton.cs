using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    [RequireComponent(typeof(Button))]
    public class SwitchButton : MonoBehaviour
    {
        [Header("텍스트")]
        [SerializeField] TMP_Text Text_Name;

        [Header("상태표시등")]
        [SerializeField] GameObject Obj_Light;

        Button button;

        public bool IsOn
        {
            get { return isOn; }
        }
        bool isOn = false;

        // 음향 경보의 정상작동 여부
        public bool IsNormalOperation
        {
            get { return isNormalOperation; }
        }
        bool isNormalOperation = false;

        public bool IsResetPress = false;

        // 버튼이름
        SWITCH device_name;

        private void Awake()
        {
            transform.GetComponent<Button>().onClick.AddListener(OnClickSwitchButton);
            button = GetComponent<Button>();
        }

        void OnClickSwitchButton()
        {
            if (device_name == SWITCH.복구)
            {
                IsResetPress = true;
                ScenarioManager.Instance.OnClickResetButton();
                return;
            }
            Turn(!isOn);
            ScenarioManager.Instance.CheckScenarioStep();
        }

        /// <summary>
        /// 스위치 상태 변경
        /// </summary>
        /// <param name="isOn"></param>
        public void Turn(bool isOn)
        {
            if(this.isOn != isOn)
            {
                SwitchLog();
            }

            this.isOn = isOn;
            Obj_Light.SetActive(isOn);
            isNormalOperation = isOn;

            // 사운드 끄기
            if (isOn == true)
            {
                SoundOff();
            }
            else
            {
                SoundOn();
            }
        }

        void SwitchLog()
        {
            if (device_name == SWITCH.축적비축적)
            {
                if (isOn)
                {
                    LogManager.Instance.SetLog("축적");
                }
                else
                {
                    LogManager.Instance.SetLog("비축적");
                }
            }
            else if(device_name == SWITCH.동작시험)
            {
                LogManager.Instance.SetLog(isOn ? "동작시험 OFF" : "동작시험 ON");
            }
            else
            {
                if (isOn)
                {
                    LogManager.Instance.SetLog(device_name + " ON");
                }
                else
                {
                    LogManager.Instance.SetLog(device_name + " OFF");
                }
            }
        }

        void SoundOn()
        {
            switch (device_name)
            {
                case SWITCH.주경종:
                    {
                        if (ScenarioManager.Instance.isHeatDetect || ScenarioManager.Instance.IsSVP ||
                            ScenarioManager.Instance.isOperationTest || ScenarioManager.Instance.isOperationTest2)
                        {
                            SoundManager.Instance.PlayAlarm(!isOn);
                        }
                        else
                        {
                            SoundManager.Instance.PlayAlarm(false);
                        }
                    }
                    break;
                case SWITCH.지구경종:
                    {
                        if (ScenarioManager.Instance.isHeatDetect || ScenarioManager.Instance.IsSVP ||
                            ScenarioManager.Instance.isOperationTest || ScenarioManager.Instance.isOperationTest2)
                        {
                            SoundManager.Instance.PlayAlarm2(!isOn);
                        }
                        else
                        {
                            SoundManager.Instance.PlayAlarm2(false);
                        }
                    }
                    break;
                case SWITCH.사이렌:
                    {
                        if (ScenarioManager.Instance.isSmokeDetect || ScenarioManager.Instance.IsSVP || 
                            ScenarioManager.Instance.IsOpenSolValve || ScenarioManager.Instance.isOperationTest2)
                        {
                            SoundManager.Instance.PlaySiren(!isOn);
                        }
                        else
                        {
                            SoundManager.Instance.PlaySiren(false);
                        }
                    }
                    break;
                case SWITCH.비상방송:
                    {
                        if (ScenarioManager.Instance.isHeatDetect || ScenarioManager.Instance.IsSVP ||
                            ScenarioManager.Instance.isOperationTest || ScenarioManager.Instance.isOperationTest2)
                        {
                            SoundManager.Instance.PlayBroadcast(!isOn);
                        }
                        else
                        {
                            SoundManager.Instance.PlayBroadcast(false);
                        }
                    }
                    break;
                case SWITCH.부저:
                    {
                        if (ScenarioManager.Instance.IsSeconValve || ScenarioManager.Instance.isHeatDetect || 
                            ScenarioManager.Instance.IsSVP || ScenarioManager.Instance.IsOpenSolValve ||
                            ScenarioManager.Instance.isOperationTest || ScenarioManager.Instance.isOperationTest2)
                        {
                            SoundManager.Instance.PlayBuzzer(!isOn);
                        }
                        else
                        {
                            SoundManager.Instance.PlayBuzzer(false);
                        }
                    }
                    break;
            }
        }

        void SoundOff()
        {
            switch (device_name)
            {
                case SWITCH.주경종:
                    SoundManager.Instance.PlayAlarm(false);
                    break;
                case SWITCH.지구경종:
                    SoundManager.Instance.PlayAlarm2(false);
                    break;
                case SWITCH.사이렌:
                    SoundManager.Instance.PlaySiren(false);
                    break;
                case SWITCH.비상방송:
                    SoundManager.Instance.PlayBroadcast(false);
                    break;
                case SWITCH.부저:
                    SoundManager.Instance.PlayBuzzer(false);
                    break;
            }
        }

        /// <summary>
        /// 동작시험 사운드
        /// </summary>
        void SoundTest(bool isSound)
        {
            switch (device_name)
            {
                case SWITCH.주경종:
                    Debug.Log($"{device_name.ToString()} 소리 : {isSound}");
                    break;
                case SWITCH.지구경종:
                    Debug.Log($"{device_name.ToString()} 소리 : {isSound}");
                    break;
                case SWITCH.비상방송:
                    Debug.Log($"{device_name.ToString()} 소리 : {isSound}");
                    break;
                case SWITCH.부저:
                    Debug.Log($"{device_name.ToString()} 소리 : {isSound}");
                    break;
            }
        }

        /// <summary>
        /// 음향경보의 정상작동 여부 초기화.
        /// </summary>
        public void ResetNormalOperation()
        {
            isNormalOperation = false;
        }

        /// <summary>
        /// 스위치 버튼 세팅
        /// </summary>
        /// <param name="name"></param>
        public void Set(string name, int index)
        {
            device_name = (SWITCH)index;
            Text_Name.text = name;
        }

        /// <summary>
        /// 버튼 활성화 및 비활성화 처리
        /// </summary>
        public void Interactive(bool isInteractive)
        {
            if(button == null) button = GetComponent<Button>();
            button.interactable = isInteractive;
        }
    }
}
