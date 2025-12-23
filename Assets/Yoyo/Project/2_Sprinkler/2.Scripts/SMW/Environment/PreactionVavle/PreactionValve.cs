using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public enum 프리액션밸브
    {
        닫기 = 99,
        열기 = 100,
        이차템퍼스위치 = 0,
        배수밸브 = 1,
        일차템퍼스위치,
        세팅밸브,
    }

    public class PreactionValve : MonoBehaviour
    {
        [Header("프리액션밸브")]
        [SerializeField] Transform Group_Popup;
        List<Popup_Valve> list_Popup = new List<Popup_Valve>();

        [Header("그룹버튼")]
        [SerializeField] Transform Group_Button;
        List<Button> list_button = new List<Button>();

        [Header("세팅밸브")]
        [SerializeField] SettingVavle SettingVavle;

        [SerializeField] Popup_Valve Popup_Valve_1;
        [SerializeField] Popup_Valve Popup_Valve_2;

        [SerializeField] Button Button_SolenoidVavle;

        [Header("물")]
        [SerializeField] GameObject Water;

        프리액션밸브 currentPopup = 프리액션밸브.닫기;

        private void OnDisable()
        {
            CloseAllPopup();
        }

        public void Reset()
        {
            ResetValve(프리액션밸브.이차템퍼스위치);
            ResetValve(프리액션밸브.배수밸브);
            ResetValve(프리액션밸브.일차템퍼스위치);
            ResetValve(프리액션밸브.세팅밸브);

            InteractiveSolenoidValve(false);
            Water.gameObject.SetActive(false);
        }

        /// <summary>
        /// 프리액션밸브 세팅
        /// </summary>
        public void Setting()
        {
            foreach(Transform t in Group_Popup)
            {
                list_Popup.Add(t.GetComponent<Popup_Valve>());
            }

            foreach(Transform t in Group_Button)
            {
                list_button.Add(t.GetComponent<Button>());
            }

            for(int i = 0; i < list_button.Count; i++)
            {
                int index = i;
                list_button[i].onClick.AddListener(() =>
                {
                    OpenPopup((프리액션밸브)index);
                });
            }

            SettingVavle.Setting();

            Button_SolenoidVavle.onClick.AddListener(() =>
            {
                PopupManager.Instance.Open(POPUP.솔레노이드밸브);
            });

            HighLight.Instance.OnHighLight += OnHighLight;
        }

        void OnHighLight()
        {
            for(int i = 0; i < list_Popup.Count; i++)
            {
                if (!list_Popup[i].gameObject.activeSelf)
                {
                    HighLight.Instance.On(list_button[i]);
                }
            }

            if(PopupManager.Instance.CurrentOpenPopup != POPUP.솔레노이드밸브)
            {
                HighLight.Instance.On(Button_SolenoidVavle);
            }
        }

        /// <summary>
        /// 유수검지장치실 버튼들 인터렉티브 활성화/비활성화
        /// </summary>
        public void Interactive(프리액션밸브 index)
        {
            if(index == 프리액션밸브.열기)
            {
                for (int i = 0; i < list_button.Count; i++)
                {
                    list_button[i].interactable = true;
                }
                return;
            }

            for (int i = 0; i < list_button.Count; i++)
            {
                if(i == (int)index)
                {
                    list_button[i].interactable = true;
                }
                else
                {
                    list_button[i].interactable = false;
                }
            }
        }

        /// <summary>
        /// 솔레노이드 팝업열기위한 버튼 인터렉티브 활성화/비활성화
        /// </summary>
        public void InteractiveSolenoidValve(bool isInteractive)
        {
            Button_SolenoidVavle.interactable = isInteractive;
        }

        // 프리액션밸브 열기
        void OpenPopup(프리액션밸브 index)
        {
            currentPopup = index;
            for (int i = 0; i < list_Popup.Count; i++)
            {
                if(i == (int)index)
                {
                    list_Popup[i].gameObject.SetActive(true);
                }
                else
                {
                    list_Popup[i].gameObject.SetActive(false);
                }
            }
        }

        public void ResetValve(프리액션밸브 name)
        {
            switch (name)
            {
                case 프리액션밸브.이차템퍼스위치:
                    {
                        list_Popup[(int)name].Reset();
                    }
                    break;
                case 프리액션밸브.배수밸브:
                    {
                        list_Popup[(int)name].Reset();
                    }
                    break;
                case 프리액션밸브.일차템퍼스위치:
                    {
                        list_Popup[(int)name].Reset();
                    }
                    break;
                case 프리액션밸브.세팅밸브:
                    {
                        SettingVavle.Reset();
                    }
                    break;
            }
        }

        public void LockValve(프리액션밸브 name)
        {
            switch (name)
            {
                case 프리액션밸브.이차템퍼스위치:
                    list_Popup[(int)name].Lock();
                    break;
                case 프리액션밸브.배수밸브:
                    list_Popup[(int)name].Lock();
                    break;
                case 프리액션밸브.일차템퍼스위치:
                    list_Popup[(int)name].Lock();
                    break;
                case 프리액션밸브.세팅밸브:
                    SettingVavle.OnClickButtonOff();
                    break;
            }
        }

        public bool CheckValve(프리액션밸브 valve, bool isDefault, bool isShow = false)
        {
            switch (valve)
            {
                case 프리액션밸브.이차템퍼스위치:
                    return list_Popup[(int)valve].Check(isDefault, isShow);
                case 프리액션밸브.배수밸브:
                    return list_Popup[(int)valve].Check(isDefault, isShow);
                case 프리액션밸브.일차템퍼스위치:
                    return list_Popup[(int)valve].Check(isDefault, isShow);
                case 프리액션밸브.세팅밸브:
                    return SettingVavle.Check(isDefault);
            }
            return false;
        }

        // 모든프리액션밸브 닫기
        public void CloseAllPopup()
        {
            OpenPopup(프리액션밸브.닫기);
        }

        public void SetWater(bool isActive)
        {
            Water.SetActive(isActive);
        }
    }
}
