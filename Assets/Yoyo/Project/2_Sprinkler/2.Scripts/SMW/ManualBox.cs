using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class ManualBox : MonoBehaviour
    {
        [SerializeField] Button Button_SVP;
        [SerializeField] Button Button_Cover;

        [SerializeField] GameObject SVP_On;
        [SerializeField] GameObject Cover_On;
        [SerializeField] GameObject SVP_Light_On;
        [SerializeField] GameObject ManualBox_Light_On;

        public bool IsSVP
        {
            get { return isSVP; }
        }
        bool isSVP = false;
        bool isCover = false;

        private void Awake()
        {
            Setting();

            HighLight.Instance.OnHighLight += OnHighLight;
        }

        void OnHighLight()
        {
            if(IsSVP == false)
            {
                HighLight.Instance.On(Button_SVP);
            }

            if(isCover == false)
            {
                HighLight.Instance.On(Button_Cover);
            }
        }

        public void Setting()
        {
            Button_Cover.onClick.AddListener(OnClickButtonCover);
            Button_SVP.onClick.AddListener(OnClickButtonSVP);
        }

        public void Reset()
        {
            Cover_On.SetActive(false);
            SVP_On.SetActive(false);
            SVP_Light_On.SetActive(false);
            ManualBox_Light_On.SetActive(false);
            isCover = false;
            isSVP = false;
        }

        void OnClickButtonCover()
        {
            Cover_On.SetActive(true);
            isCover = true;
        }

        public void OnClickButtonSVP()
        {
            isSVP = true;
            SVP_On.SetActive(isSVP);
            ScenarioManager.Instance.CheckScenarioStep();
        }

        public void IsValveOpen(bool isOpen)
        {
            SVP_Light_On.SetActive(isOpen);
            ManualBox_Light_On.SetActive(isOpen);
        }
    }
}
