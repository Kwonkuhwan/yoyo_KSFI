using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public enum AREA
    {
        감시제어반 = 0,
        주차장,
        유수검지장치실,
        펌프실
    }
    public class Teleport : MonoBehaviour
    {
        [SerializeField] Transform GroupArea;
        List<Button> list_areaButton = new List<Button>();

        [Header("환경")]
        [SerializeField] Transform Environment;
        List<GameObject> list_Area = new List<GameObject>();

        // 0 = 구획실, 1 = 펌프실
        public AREA Area
        {
            get { return (AREA)areaIndex; }
        }
        int areaIndex = -1;
        bool isHighlight;

        private void Awake()
        {
            foreach(Transform t in GroupArea)
            {
                list_areaButton.Add(t.GetComponent<Button>());
            }

            for(int i = 0; i < list_areaButton.Count; i++)
            {
                int index = i;
                list_areaButton[i].onClick.AddListener(() =>
                {
                    OnClickSelectArea(index);
                });
            }

            foreach(Transform t in Environment)
            {
                list_Area.Add(t.gameObject);
            }

            HighLight.Instance.OnHighLight += OnHighLight;
        }

        void OnHighLight()
        {
            if(!isHighlight)
            {
                HighLight.Instance.On(list_areaButton);
            }
        }

        public bool CompareArea(AREA area)
        {
            isHighlight = (AREA)areaIndex == area;
            return isHighlight;
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 버튼 비활성화
        /// </summary>
        public void Interactive(bool isInteractable, int index = -1)
        {
            if(index == -1)
            {
                for(int i = 0; i < list_areaButton.Count; i++)
                {
                    list_areaButton[i].interactable = isInteractable;
                }
            }
            else
            {
                list_areaButton[index].interactable = isInteractable;
            }
        }

        /// <summary>
        /// 환경이동
        /// </summary>
        /// <param name="area"></param>
        public void MoveArea(AREA area)
        {
            if (areaIndex == (int)area) return;
            OnClickSelectArea((int)area);
        }

        void OnClickSelectArea(int AreaNumber)
        {
            areaIndex = AreaNumber;

            SoundManager.Instance.MuteAll(areaIndex != (int)AREA.감시제어반);
            SoundManager.Instance.PlayPump(areaIndex == (int)AREA.펌프실);

            for (int i = 0; i < list_Area.Count; i++)
            {
                if(i == AreaNumber)
                {
                    list_Area[i].SetActive(true);
                    list_areaButton[i].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    list_Area[i].SetActive(false);
                    list_areaButton[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }

            ScenarioManager.Instance.CheckScenarioStep();
        }
    }
}
