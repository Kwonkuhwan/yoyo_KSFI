using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public enum 문제
    {
        감지기동작표시등,
        수동조작함,
        밸브개방표시등,
        화재표시등,
        음향장치,
        템퍼스위치,
        펌프압력스위치,
        펌프기동표시등
    }

    public class Question : MonoBehaviour
    {
        [SerializeField] List<Toggle> list_toggle = new List<Toggle>();

        private void Awake()
        {
            for(int i = 0; i < list_toggle.Count; i++)
            {
                list_toggle[i].onValueChanged.AddListener(ToggleValueChanged);
            }
        }

        void ToggleValueChanged(bool isOn)
        {
            ScenarioManager.Instance.CheckScenarioStep();
        }

        public void Reset()
        {
            for(int i = 0; i < list_toggle.Count; i++)
            {
                list_toggle[i].isOn = false;
                list_toggle[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 문제세팅
        /// </summary>
        /// <param name="list"></param>
        public void SetQuestion(문제[] list)
        {
            for(int i = 0; i < list_toggle.Count; i++)
            {
                if (list.Contains((문제)i))
                {
                    list_toggle[i].gameObject.SetActive(true);
                }
                else
                {
                    list_toggle[i].gameObject.SetActive(false);
                }
            }
        }

        public void SetALL(문제[] list_exception = null)
        {
            for (int i = 0; i < list_toggle.Count; i++)
            {
                if(list_exception != null)
                {
                    if (list_exception.Contains((문제)i))
                    {
                        list_toggle[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        list_toggle[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    list_toggle[i].gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 정답체크
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        public bool CheckAnswer(문제[] answer)
        {
            bool isCorrect = true;
            for (int i = 0; i < list_toggle.Count; i++)
            {
                if (answer.Contains((문제)i))
                {
                    if (list_toggle[i].isOn == false)
                    {
                        isCorrect = false;
                    }
                }
                else
                {
                    if (list_toggle[i].isOn == true)
                    {
                        isCorrect = false;
                    }
                }
            }
            return isCorrect;
        }
    }
}
