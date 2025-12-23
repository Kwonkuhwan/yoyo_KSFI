using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class SettingVavle : MonoBehaviour
    {
        [SerializeField] Button Button_on;
        [SerializeField] GameObject Button_SettingValve_on;
        [SerializeField] Button Button_off;

        public bool IsOpen
        {
            get { return isOpen; }
        }
        bool isOpen = false;

        bool isHighLight = false;

        public void Setting()
        {
            Button_on.onClick.AddListener(OnClickButtonOn);
            Button_off.onClick.AddListener(OnClickButtonOff);

            HighLight.Instance.OnHighLight += OnHighLight;
        }

        void OnHighLight()
        {
            if(isHighLight)
            {
                HighLight.Instance.On(Button_on);
                HighLight.Instance.On(Button_off);
            }
        }

        public bool Check(bool isCheck)
        {
            if(isOpen == isCheck)
            {
                isHighLight = false;
                return true;
            }
            else
            {
                isHighLight = true;
                return false;
            }
        }

        public void OnClickButtonOn()
        {
            isOpen = false;
            Button_on.gameObject.SetActive(false);
            Button_SettingValve_on.SetActive(false);
            Button_off.gameObject.SetActive(true);
            ScenarioManager.Instance.CheckScenarioStep();
        }

        public void OnClickButtonOff()
        {
            isOpen = true;
            Button_on.gameObject.SetActive(true);
            Button_SettingValve_on.SetActive(true);
            Button_off.gameObject.SetActive(false);
            ScenarioManager.Instance.CheckScenarioStep();
        }

        public void Reset()
        {
            isOpen = false;
            Button_on.gameObject.SetActive(false);
            Button_SettingValve_on.SetActive(false);
            Button_off.gameObject.SetActive(true);
        }
    }
}
