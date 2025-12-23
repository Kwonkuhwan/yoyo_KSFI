using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class Popup_Complete : Popup
    {
        enum 버튼
        {
            이전단계,
            목차선택,
            다음단계,
            메인화면
        }

        [SerializeField] Transform Group_Button;
        List<Button> list_button = new List<Button>();

        [SerializeField] ModeSelect ModeSelect;
        [SerializeField] TMP_Text Text_Main;
        [SerializeField] TMP_Text Text_Sub;
        [SerializeField] GameObject Obj_ContentPopup;

        protected override void Setting()
        {
            foreach(Transform t in Group_Button)
            {
                list_button.Add(t.GetComponent<Button>());
            }

            for (int i = 0; i < list_button.Count; i++)
            {
                int index = i;
                list_button[i].onClick.AddListener(() =>
                {
                    OnClickButton((버튼)index);
                });
            }
        }

        private void OnEnable()
        {
            switch (ScenarioManager.Instance.GetScenarioName)
            {
                case SCENARIO.점검:
                    Text_Main.text = "점검 전 안전 조치를 모두 완료했습니다.";
                    Text_Sub.text = "다음 단계 버튼을 눌러 유수검지장치 작동시험을 진행하세요.";
                    CheckEnable(false);
                    break;
                case SCENARIO.감지기:
                    Text_Main.text = "교차회로 감지기 작동 시험를 완료했습니다.";
                    Text_Sub.text = "다음 단계 버튼을 눌러 다음 점검을 진행하세요.";
                    CheckEnable(false);
                    break;
                case SCENARIO.수동조작함:
                    Text_Main.text = "수동조작함 작동 시험을 완료했습니다.";
                    Text_Sub.text = "다음 단계 버튼을 눌러 다음 점검을 진행하세요.";
                    CheckEnable(false);
                    break;
                case SCENARIO.수동기동밸브:
                    Text_Main.text = "수동기동밸브 작동 시험을 완료했습니다.";
                    Text_Sub.text = "다음 단계 버튼을 눌러 다음 점검을 진행하세요.";
                    CheckEnable(false);
                    break;
                case SCENARIO.수동기동스위치:
                    Text_Main.text = "수동기동스위치 작동 시험을 완료했습니다.";
                    Text_Sub.text = "다음 단계 버튼을 눌러 다음 점검을 진행하세요.";
                    CheckEnable(false);
                    break;
                case SCENARIO.동작시험:
                    Text_Main.text = "동작시험을 완료했습니다.";
                    Text_Sub.text = "다음 단계 버튼을 눌러 다음 점검을 진행하세요.";
                    CheckEnable(false);
                    break;
                case SCENARIO.복구:
                    Text_Main.text = "준비작동식 스프링클러 점검를 완료했습니다.";
                    Text_Sub.text = "하단의 버튼을 통해 다른 페이지로 이동해 주세요.";
                    CheckEnable(true);
                    break;
            }
        }

        void CheckEnable(bool isEnd)
        {
            list_button[2].gameObject.SetActive(!isEnd);
            list_button[3].gameObject.SetActive(isEnd);
        }

        void OnClickButton(버튼 index)
        {
            switch (index)
            {
                case 버튼.이전단계:
                    {
                        ScenarioManager.Instance.PreSelectScenario();
                        gameObject.SetActive(false);
                    }
                    break;
                case 버튼.목차선택:
                    {
                        Obj_ContentPopup.SetActive(true);
                        gameObject.SetActive(false);
                    }
                    break;
                case 버튼.다음단계:
                    {
                        ScenarioManager.Instance.NextSelectScenario();
                        gameObject.SetActive(false);
                    }
                    break;
                case 버튼.메인화면:
                    {
                        BackToMain();
                    }
                    break;
            }
        }

        public void BackToMain()
        {
#if KFSI_ALL
            ModeSelect.ChanageGroup(0);
#elif KFSI_TEST
            ModeSelect.SelectMode((int)모드.평가모드);
#elif !KFSI_TEST
            ModeSelect.SelectMode((int)모드.실습모드);
#endif
            ModeSelect.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public void BackToContent()
        {
            OnClickButton(버튼.목차선택);
        }

        protected override void RESET()
        {
            throw new System.NotImplementedException();
        }
    }
}
