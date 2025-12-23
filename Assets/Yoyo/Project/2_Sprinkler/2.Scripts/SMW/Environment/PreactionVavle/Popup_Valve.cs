using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class Popup_Valve : MonoBehaviour
    {
        [SerializeField] Button vavle;
        [SerializeField] RectTransform valve_rect;
        [SerializeField] Image Image_Active;
        [SerializeField] GameObject Image_Compelete;

        // 개방된 상태
        public bool IsDefault
        {
            get { return isDefault; }
        }
        bool isDefault = false;

        public bool isHighLight = false;

        private void Awake()
        {
            if (vavle == null) return;
            vavle.onClick.AddListener(OnClickButtonEvent);
            HighLight.Instance.OnHighLight += OnHighLight;
        }

        void OnHighLight()
        {
            if (isHighLight)
            {
                HighLight.Instance.On(vavle);
            }
        }

        public void Reset()
        {
            isDefault = false;
            isHighLight = false;
            Image_Compelete.SetActive(false);
            OnOffSwitch();
        }
        
        public void Lock()
        {
            isDefault = true;
            OnOffSwitch();
        }

        /// <summary>
        /// 밸브의 개방/폐쇄 상태 확인
        /// </summary>
        /// <param name="isOpen"> false = 초기세팅, true = 인터렉션된상태 </param>
        /// <returns></returns>
        public bool Check(bool isOpen, bool isShow = false)
        {
            if(IsDefault == isOpen)
            {
                isHighLight = false;
                if (ScenarioManager.Instance.IsMode == false)
                {
                    Image_Compelete.SetActive(true);
                }
                else if(isShow)
                {
                    Image_Compelete.SetActive(true);
                }
                return true;
            }
            else
            {
                isHighLight = true;
                Image_Compelete.SetActive(false);
                return false;
            }
        }

        /// <summary>
        /// 2차 템퍼스위치 클릭
        /// </summary>
        void OnClickButtonEvent()
        {
            isDefault = !isDefault;
            OnOffSwitch();
            ScenarioManager.Instance.CheckScenarioStep();
        }

        void OnOffSwitch()
        {
            if (isDefault)
            {
                valve_rect.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, -45));
            }
            else
            {
                valve_rect.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }

            if (Image_Active != null)
            {
                Image_Active.gameObject.SetActive(isDefault);
            }
        }
    }
}
