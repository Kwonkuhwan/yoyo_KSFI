using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SMW.Sprinkler
{
    public class Display : MonoBehaviour
    {
        [Header("텍스트")]
        [SerializeField] TMP_Text Text_Index;
        [SerializeField] TMP_Text Text_Area;
        [SerializeField] TMP_Text Text_Name;

        [Header("상태표시등")]
        [SerializeField] GameObject Obj_On;

        string logText = "";

        public bool IsOn
        {
            get { return isOn; }
        }
        bool isOn = false;

        public void TurnOn()
        {
            SetLog(true);
            isOn = true;
            Obj_On.SetActive(true);
        }

        public void TurnOff()
        {
            SetLog(false);
            isOn = false;
            Obj_On.SetActive(false);
        }

        void SetLog(bool _isOn)
        {
            if(isOn != _isOn)
            {
                if(_isOn == true)
                {
                    LogManager.Instance.SetLog($"{logText} 점등");
                }
                else
                {
                    LogManager.Instance.SetLog($"{logText} 소등");
                }
            }
        }

        public void Set(string index, string area, string name)
        {
            switch(index)
            {
                case "1":
                    index = "①";
                    logText = "프리액션밸브 감지기A";
                    break;
                case "2":
                    index = "②";
                    logText = "프리액션밸브 감지기B";
                    break;
                case "3":
                    index = "③";
                    logText = "프리액션밸브 SVP";
                    break;
                case "4":
                    index = "④";
                    logText = "프리액션밸브 밸브개방";
                    break;
                case "5":
                    index = "⑤";
                    logText = "주펌프흡입축 탬퍼스위치";
                    break;
                case "6":
                    index = "⑥";
                    logText = "주펌프토출측 탬퍼스위치";
                    break;
                case "7":
                    index = "⑦";
                    logText = "충압펌프 흡입축 탬퍼스위치";
                    break;
                case "8":
                    index = "⑧";
                    logText = "충압펌프 토출측 탬퍼스위치";
                    break;
                case "9":
                    index = "⑨";
                    logText = "프리액션밸브 1차측 탬퍼스위치";
                    break;
                case "10":
                    index = "⑩";
                    logText = "프리액션밸브 2차측 탬퍼스위치";
                    break;
                case "11":
                    index = "⑪";
                    logText = "주펌프 압력스위치";
                    break;
                case "12":
                    index = "⑫";
                    logText = "충압펌프 압력스위치";
                    break;
            }

            Text_Index.text = index;
            Text_Area.text = area;
            Text_Name.text = name;
        }
    }
}
