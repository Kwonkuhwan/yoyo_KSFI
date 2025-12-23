using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class ControlPanel : MonoBehaviour
    {
        [SerializeField] DisplayNaming DisplayManager;
        [SerializeField] SwichButtonNaming SwitchButtonManager;
        [SerializeField] PowerNaming PowerManager;
        [SerializeField] OperatingTool OperatingToolManager;

        float blinkTime;

        List<Button> list_highlight = new List<Button>();

        private void Awake()
        {
            HighLight.Instance.OnHighLight += OnHighLight;
        }

        void OnHighLight()
        {
            HighLight.Instance.On(list_highlight);
        }

        private void Update()
        {
            Blink();
        }

        private void OnEnable()
        {
            ScenarioManager.Instance.CheckScenarioStep();
        }

        public void Setting()
        {
            DisplayManager.Setting();
            SwitchButtonManager.Setting();
            PowerManager.Setting();
            OperatingToolManager.Setting();
            SettingDial();
        }

        public void Reset()
        {
            PowerManager.list_power[0].TurnOff();
            PowerManager.list_power[1].TurnOn();

            for(int i = 0; i < SwitchButtonManager.list_switch.Count; i++)
            {
                SwitchButtonManager.list_switch[i].Turn(false);
            }

            for (int j = 0; j < OperatingToolManager.List_Tool.Count; j++)
            {
                OperatingToolManager.List_Tool[j].Turn(0);
                OperatingToolManager.List_Tool[j]?.Display(false);
            }

            for(int i = 0; i < DisplayManager.list_display.Count; i++)
            {
                DisplayManager.list_display[i].TurnOff();
            }

            ResetDial();
        }

        #region 파워
        public void PowerOnOff(int index, bool isOn)
        {
            if(isOn)
            {
                PowerManager.list_power[index].TurnOn();
            }
            else
            {
                PowerManager.list_power[index].TurnOff();
            }
        }
        #endregion

        #region 디스플레이
        /// <summary>
        /// 디스플레이 점등
        /// </summary>
        /// <param name="index"> 디스플레이 번호 </param>
        public void DisplayLightOn(int index, bool isOn)
        {
            if (isOn)
            {
                DisplayManager.list_display[index].TurnOn();
            }
            else
            {
                DisplayManager.list_display[index].TurnOff();
            }
        }

        /// <summary>
        /// 디스플레이 점등 여부
        /// </summary>
        public bool CheckDisplayOn(int[] index)
        {
            for (int i = 0; i < DisplayManager.list_display.Count; i++)
            {
                if(index.Contains<int>(i))
                {
                    if (!DisplayManager.list_display[i].IsOn)
                    {
                        return false;
                    }
                }
                else
                {
                    if(DisplayManager.list_display[i].IsOn)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool CheckDisplayOn(int index)
        {
            return DisplayManager.list_display[index].IsOn;
        }
        #endregion

        #region 스위치 버튼
        /// <summary>
        /// 켜져있거나 꺼져있어야 되는 스위치버튼들 비교
        /// </summary>
        public bool CheckSwitchOn(int[] index)
        {
            bool isCheck = true;
            for (int i = 0; i < SwitchButtonManager.list_switch.Count; i++)
            {
                if (index.Contains<int>(i))
                {
                    if (!SwitchButtonManager.list_switch[i].IsOn)
                    {
                        if(!list_highlight.Contains(SwitchButtonManager.list_switch[i].GetComponent<Button>()))
                        {
                            list_highlight.Add(SwitchButtonManager.list_switch[i].GetComponent<Button>());
                        }
                        isCheck = false;
                    }
                    else
                    {
                        if (list_highlight.Contains(SwitchButtonManager.list_switch[i].GetComponent<Button>()))
                        {
                            list_highlight.Remove(SwitchButtonManager.list_switch[i].GetComponent<Button>());
                        }
                    }
                }
                else
                {
                    if (SwitchButtonManager.list_switch[i].IsOn)
                    {
                        isCheck = false;
                    }
                }
            }
            return isCheck;
        }

        /// <summary>
        /// 스위치버튼들 비활성화 체크
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool CheckSwitchOff(int[] index)
        {
            bool isCheck = true;
            for(int i = 0; i < index.Length; i++)
            {
                if(SwitchButtonManager.list_switch[index[i]].IsOn == false)
                {
                    if (list_highlight.Contains(SwitchButtonManager.list_switch[index[i]].GetComponent<Button>()))
                    {
                        list_highlight.Remove(SwitchButtonManager.list_switch[index[i]].GetComponent<Button>());
                    }
                }
                else
                {
                    if (!list_highlight.Contains(SwitchButtonManager.list_switch[index[i]].GetComponent<Button>()))
                    {
                        list_highlight.Add(SwitchButtonManager.list_switch[index[i]].GetComponent<Button>());
                    }
                    isCheck = false;
                }
            }
            return isCheck;
        }

        public bool CheckSwitchOn(SWITCH index)
        {
            if(SwitchButtonManager.list_switch[(int)index].IsOn == true)
            {
                if (list_highlight.Contains(SwitchButtonManager.list_switch[(int)index].GetComponent<Button>()))
                {
                    list_highlight.Remove(SwitchButtonManager.list_switch[(int)index].GetComponent<Button>());
                }
            }
            else
            {
                if (!list_highlight.Contains(SwitchButtonManager.list_switch[(int)index].GetComponent<Button>()))
                {
                    list_highlight.Add(SwitchButtonManager.list_switch[(int)index].GetComponent<Button>());
                }
            }
            return SwitchButtonManager.list_switch[(int)index].IsOn;
        }

        /// <summary>
        /// 스위치버튼들 활성화 및 비활성화 처리
        /// </summary>
        public void InteractiveSwitch(bool isInteractive, int index = -1)
        {
            if(index == -1)
            {
                for(int i = 0; i < SwitchButtonManager.list_switch.Count; i++)
                {
                    SwitchButtonManager.list_switch[i].Interactive(isInteractive);
                }
                list_highlight.Clear();
            }
            else
            {
                SwitchButtonManager.list_switch[index].Interactive(isInteractive);
            }
        }

        public void InteractiveSwitch(bool isInteractive, SWITCH name)
        {
            SwitchButtonManager.list_switch[(int)name].Interactive(isInteractive);
        }

        /// <summary>
        /// 스위치버튼들 활성화 및 비활성화 처리
        /// </summary>
        public void InteractiveSwitch(bool isInteractive, int[] index)
        {
            for (int i = 0; i < index.Length; i++)
            {
                SwitchButtonManager.list_switch[index[i]].Interactive(isInteractive);
            }
        }

        /// <summary>
        /// 스위치 버튼 On/Off
        /// </summary>
        public void SwitchButtonOnOff(int index, bool isOn)
        {
            SwitchButtonManager.list_switch[index].Turn(isOn);
        }

        public void SwitchButtonOnOff(SWITCH index, bool isOn)
        {
            SwitchButtonManager.list_switch[(int)index].Turn(isOn);
        }

        public void SwitchButtonOnOff(SWITCH[] index, bool isOn)
        {
            for(int i = 0; i < index.Length; i++)
            {
                SwitchButtonManager.list_switch[(int)index[i]].Turn(isOn);
            }
        }

        /// <summary>
        /// 스위치버튼의 열감지기 정상작동 여부
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool CheckSwitchNormalOperation(int[] index)
        {
            bool isCheck = true;
            for (int i = 0; i < SwitchButtonManager.list_switch.Count; i++)
            {
                if (index.Contains<int>(i))
                {
                    if (!SwitchButtonManager.list_switch[i].IsNormalOperation)
                    {
                        if (!list_highlight.Contains(SwitchButtonManager.list_switch[i].GetComponent<Button>()))
                        {
                            list_highlight.Add(SwitchButtonManager.list_switch[i].GetComponent<Button>());
                        }
                        isCheck = false;
                    }
                    else
                    {
                        if(list_highlight.Contains(SwitchButtonManager.list_switch[i].GetComponent<Button>()))
                        {
                            list_highlight.Remove(SwitchButtonManager.list_switch[i].GetComponent<Button>());
                        }
                    }
                }
                else
                {
                    if (SwitchButtonManager.list_switch[i].IsNormalOperation)
                    {
                        isCheck = false;
                    }
                }
            }
            return isCheck;
        }

        /// <summary>
        /// 스위치버튼들의 음향경보의 정상작동 여부 값 초기화.
        /// </summary>
        /// <param name="index"></param>
        public void ResetSwitchNormalOperation(int[] index)
        {
            for(int i = 0; i < index.Length; i++)
            {
                SwitchButtonManager.list_switch[index[i]].ResetNormalOperation();
            }
        }

        public void AllResetSwitchNormalOperation()
        {
            for (int i = 0; i < SwitchButtonManager.list_switch.Count; i++)
            {
                SwitchButtonManager.list_switch[i].ResetNormalOperation();
            }
        }

        public bool CheckSwitchResetButtonPress()
        {
            bool isPress = SwitchButtonManager.list_switch[(int)SWITCH.복구].IsResetPress;
            if (isPress)
            {
                if(list_highlight.Contains(SwitchButtonManager.list_switch[(int)SWITCH.복구].GetComponent<Button>()))
                {
                    list_highlight.Remove(SwitchButtonManager.list_switch[(int)SWITCH.복구].GetComponent<Button>());
                }
            }
            else
            {
                if (!list_highlight.Contains(SwitchButtonManager.list_switch[(int)SWITCH.복구].GetComponent<Button>()))
                {
                    list_highlight.Add(SwitchButtonManager.list_switch[(int)SWITCH.복구].GetComponent<Button>());
                }
            }

            return isPress;
        }

        public void ResetSwitchButtonReset()
        {
            SwitchButtonManager.list_switch[(int)SWITCH.복구].IsResetPress = false;
        }
        #endregion

        #region 작동 툴
        /// <summary>
        /// 작동 툴에서의 모드와 동일한지 비교
        /// </summary>
        public bool CheckTools(int[] index)
        {
            bool isCheck = true;
            for(int i = 0; i < OperatingToolManager.List_Tool.Count; i++)
            {
                if(OperatingToolManager.List_Tool[i].Mode != index[i])
                {
                    if(!list_highlight.Contains(OperatingToolManager.List_Tool[i].OperationButton))
                    {
                        list_highlight.Add(OperatingToolManager.List_Tool[i].OperationButton);
                    }
                    isCheck = false;
                }
                else
                {
                    if (list_highlight.Contains(OperatingToolManager.List_Tool[i].OperationButton))
                    {
                        list_highlight.Remove(OperatingToolManager.List_Tool[i].OperationButton);
                    }
                }
            }
            return isCheck;
        }

        public void ToolsChangeState(int index, 작동 state)
        {
            OperatingToolManager.List_Tool[index].Turn((int)state);
        }

        /// <summary>
        /// 작동 툴 활성화 및 비활성화 처리
        /// </summary>
        /// <param name="isInteractive"> 버튼 인터렉티브 활성화/비활성화 </param>
        /// <param name="tool"> 작동툴이름 </param>
        /// <param name="isMove"> 선택/기동 </param>
        public void InteractiveTool(bool isInteractive, 조작툴 tool = 조작툴.전체, bool isMove = false)
        {
            if(tool == 조작툴.전체)
            {
                for (int i = 0; i < OperatingToolManager.List_Tool.Count; i++)
                {
                    OperatingToolManager.List_Tool[i].Interactive(isInteractive);
                    OperatingToolManager.List_Tool[i].Interactive(isInteractive, true);
                }
            }
            else
            {
                OperatingToolManager.List_Tool[(int)tool].Interactive(isInteractive, isMove);
            }
        }
        
        public void ToolsDisplayOn(int index, bool isActive)
        {
            OperatingToolManager.List_Tool[index].Display(isActive);
        }

        public void ToolsReset()
        {
            for (int i = 0; i < OperatingToolManager.List_Tool.Count; i++)
            {
                OperatingToolManager.List_Tool[i].Reset();
            }
        }
        #endregion

        #region 다이얼
        [SerializeField] Transform Dial;
        Button Button_Dial;
        List<GameObject> list_dial = new List<GameObject>();
        public int DialNumber
        {
            get { return index_dial; }
        }
        int index_dial = 0;         // 0 = 정상 , 1 = 1, 2 = 2

        public void OnClickDialButton()
        {
            index_dial++;
            if (index_dial > 2)
            {
                index_dial = 0;
            }

            for(int i = 0; i < list_dial.Count;i++)
            {
                if(i == index_dial)
                {
                    list_dial[i].SetActive(true);
                }
                else
                {
                    list_dial[i].SetActive(false);
                }
            }
            ScenarioManager.Instance.CheckScenarioStep();
        }

        void SettingDial()
        {
            foreach(Transform t in Dial)
            {
                list_dial.Add(t.gameObject);
            }

            Button_Dial = Dial.GetComponent<Button>();
            Button_Dial.onClick.AddListener(OnClickDialButton);
        }

        public void InteractiveDial(bool isInteractive)
        {
            Button_Dial.interactable = isInteractive;
        }

        public bool CheckDial(int index)
        {
            if(index_dial == index)
            {
                if (list_highlight.Contains(Button_Dial))
                {
                    list_highlight.Remove(Button_Dial);
                }
                return true;
            }
            else
            {
                if (!list_highlight.Contains(Button_Dial))
                {
                    list_highlight.Add(Button_Dial);
                }
                return false;
            }
        }

        public void ResetDial()
        {
            index_dial = 0;
            for (int i = 0; i < list_dial.Count; i++)
            {
                if (i == index_dial)
                {
                    list_dial[i].SetActive(true);
                }
                else
                {
                    list_dial[i].SetActive(false);
                }
            }
        }

        public void MoveDial(int index)
        {
            index_dial = index;
            for (int i = 0; i < list_dial.Count; i++)
            {
                if (i == index_dial)
                {
                    list_dial[i].SetActive(true);
                }
                else
                {
                    list_dial[i].SetActive(false);
                }
            }
        }

        #endregion

        #region 스위치 주의
        /// <summary>
        /// 스위치 하나라도 켜져있는지 체크
        /// </summary>
        bool CheckSwitchBlink(int[] index)
        {
            for (int i = 0; i < SwitchButtonManager.list_switch.Count; i++)
            {
                if (index.Contains<int>(i))
                {
                    if (SwitchButtonManager.list_switch[i].IsOn)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void Blink()
        {
            if (CheckSwitchBlink(new int[] { 0,1,2,3,4 }))
            {
                if (blinkTime < 0.5f)
                {
                    blinkTime += Time.deltaTime;
                }
                else
                {
                    blinkTime = 0;
                    if (PowerManager.list_power[(int)파워.스위치주의].IsOn)
                    {
                        PowerManager.list_power[(int)파워.스위치주의].TurnOff();
                    }
                    else
                    {
                        PowerManager.list_power[(int)파워.스위치주의].TurnOn();
                    }
                }
            }
            else
            {
                PowerManager.list_power[(int)파워.스위치주의].TurnOff();
            }
        }
        #endregion
    }
}
