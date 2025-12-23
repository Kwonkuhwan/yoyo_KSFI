using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class EvaluationManager : MonoBehaviour
    {
        private static EvaluationManager instance;
        public static EvaluationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EvaluationManager();
                }
                return instance;
            }
        }

        [Header("다음평가")]
        [SerializeField] GameObject NextPopup;
        [SerializeField] Button Button_Next;
        [SerializeField] Button Button_Again;

        [Header("결과창")]
        [SerializeField] GameObject ResultPopup;
        [SerializeField] List<GameObject> list_success = new List<GameObject>();
        [SerializeField] List<GameObject> list_danger = new List<GameObject>();
        [SerializeField] List<Button> list_button_result = new List<Button>();

        [Header("목차결과창")]
        [SerializeField] GameObject ContentResultPopup;
        [SerializeField] TMP_Text Text_Main;

        [SerializeField] Hint Hint;

        [SerializeField] List<ResultBox> resultBoxes = new List<ResultBox>();
        public Content[] Box = new Content[7];
        [Serializable]
        public struct Content
        {
            public bool[] Success;
            public string[] text;
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            Button_Next.onClick.AddListener(OnClickNextButton);
            Button_Again.onClick.AddListener(OnClickAgainButton);

            for (int i = 0; i < list_button_result.Count; i++)
            {
                int index = i;
                list_button_result[i].onClick.AddListener(() =>
                {
                    OnClickResultButton(index);
                });
            }
        }

        public void OpenNext(bool isOpen)
        {
            NextPopup.SetActive(isOpen);
            SoundManager.Instance.PlayPump(false);
            Hint.MuteHint(isOpen);
        }

        public void OpenResult(bool isOpen)
        {
            SetResult();
            ResultPopup.SetActive(isOpen);
            Hint.MuteHint(isOpen);
        }

        void OnClickNextButton()
        {
            ScenarioManager.Instance.NextSelectScenario();
        }

        void OnClickAgainButton()
        {
            ScenarioManager.Instance.ResetSelectScenario();
        }

        bool isSuccess = false;
        void SetResult()
        {
            for (int i = 0; i < Box.Length; i++)
            {
                int index = 0;
                for (int j = 0; j < Box[i].Success.Length; j++)
                {
                    if (Box[i].Success[j] == true)
                    {
                        index++;
                    }
                }

                if (Box[i].Success.Length == index)
                {
                    list_danger[i].SetActive(false);
                    list_success[i].SetActive(true);
                }
                else if(index >= 1)
                {
                    list_danger[i].SetActive(true);
                }
                else
                {
                    list_success[i].SetActive(false);
                    list_danger[i].SetActive(false);
                }
            }
        }

        void OnClickResultButton(int index)
        {
            ContentResultPopup.SetActive(true);
            SetContentResult(index);

            switch(index)
            {
                case 0:
                    Text_Main.text = "점검 전 안전조치 결과";
                    break;
                case 1:
                    Text_Main.text = "교차회로 감지기 결과";
                    break;
                case 2:
                    Text_Main.text = "수동조작함 결과";
                    break;
                case 3:
                    Text_Main.text = "수동기동밸브 결과";
                    break;
                case 4:
                    Text_Main.text = "수동기동스위치 결과";
                    break;
                case 5:
                    Text_Main.text = "감시제어반 동작시험 결과";
                    break;
                case 6:
                    Text_Main.text = "점검 후 복구 세부결과";
                    break;
            }
        }

        void SetContentResult(int index)
        {
            for(int i = 0; i < resultBoxes.Count; i++)
            {
                resultBoxes[i].gameObject.SetActive(false);
            }

            for(int i = 0; i < Box[index].Success.Length; i++)
            {
                resultBoxes[i].Set(Box[index].text[i], Box[index].Success[i]);
            }
        }
    }
}
