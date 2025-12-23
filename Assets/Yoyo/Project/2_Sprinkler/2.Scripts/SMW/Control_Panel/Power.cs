using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SMW.Sprinkler
{
    public class Power : MonoBehaviour
    {
        [Header("텍스트")]
        [SerializeField] TMP_Text Text_name;

        [Header("전원")]
        [SerializeField] GameObject Obj_On;

        [Header("전압지시")]
        // Low, Normal, High
        [SerializeField] List<GameObject> Obj_PowerLevel = new List<GameObject>(3);
        int PowerLevelIndex = -1;

        enum LEVEL
        {
            LOW,
            NORMAL,
            HIGH
        }

        public bool IsOn
        {
            get { return isOn; }
        }
        bool isOn = false;

        public void TurnOn()
        {
            isOn = true;
            Obj_On.SetActive(true);
        }

        public void TurnOff()
        {
            isOn = false;
            Obj_On.SetActive(false);
        }

        /// <summary>
        /// 전압지시의 파워레벨을 바꿈
        /// </summary>
        /// <param name="level">  </param>
        public void ChangePowerLevel(int level)
        {
            for (int i = 0; i < Obj_PowerLevel.Count; i++)
            {
                if(level == i)
                {
                    Obj_PowerLevel[i].SetActive(true);
                    PowerLevelIndex = level;
                }
                else
                {
                    Obj_PowerLevel[i].SetActive(false);
                }
            }
        }

        public void Set(string name)
        {
            if (Text_name == null) return;
            Text_name.text = name;
        }
    }
}
