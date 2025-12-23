using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class ContentPopup : MonoBehaviour
    {
        [SerializeField] Button Button_Home;
        [SerializeField] Button Button_Exit;
        [SerializeField] Button Button_Pre;
        [SerializeField] ModeSelect ModeSelect;
        [SerializeField] GameObject ResultPopup;

        [Header("모드")]
        [SerializeField] Transform Group_Mode;
        [SerializeField] GameObject Select_mode;
        List<Button> list_button_mode = new List<Button>();

        [Header("메뉴")]
        [SerializeField] Transform Group_Menu;
        List<Button> list_button_menu = new List<Button>();

        [Header("평가모드")]
        [SerializeField] Transform Group_Content;
        List<Button> list_button_content = new List<Button>();

        // 그룹들 리스트
        List<GameObject> list_Group = new List<GameObject>();

        [SerializeField] TMP_Text Text_Title;
        [SerializeField] RectTransform Rect_Title;

        // false = 실습모드, true = 평가모드
        bool isMode = false;

        int pageindex = 0;

        bool isSkip = false;

        private void Awake()
        {
            list_Group.Add(Select_mode);
            list_Group.Add(Group_Menu.gameObject);
            list_Group.Add(Group_Content.gameObject);

            Button_Home.onClick.AddListener(() =>
            {
                SceneManager.LoadSceneAsync("TitleScene");
            });

            Button_Exit.onClick.AddListener(() =>
            {
                Application.Quit();
            });

            Button_Pre.onClick.AddListener(() =>
            {
                pageindex--;
                ChanageGroup(pageindex);
            });

            SettingMode();
            SettingMenu();
            SettingContent();

#if UNITY_WEBGL
            Button_Exit.gameObject.SetActive(false);
            Button_Home.GetComponent<RectTransform>().anchoredPosition = new Vector2(-32, 24);
#endif
        }

        private void OnEnable()
        {
#if KFSI_ALL
            ChanageGroup(0);
#elif KFSI_TEST
            isSkip = true;
            SelectMode((int)모드.평가모드);
#elif !KFSI_TEST
            isSkip = true;
            SelectMode((int)모드.실습모드);
#endif
        }

        void SettingMode()
        {
            foreach (Transform mode in Group_Mode)
            {
                list_button_mode.Add(mode.GetComponent<Button>());
            }
            for (int i = 0; i < list_button_mode.Count; i++)
            {
                int num = i;
                list_button_mode[i].onClick.AddListener(() =>
                {
                    SelectMode(num);
                });
            }
        }

        void SettingMenu()
        {
            foreach (Transform menu in Group_Menu)
            {
                list_button_menu.Add(menu.GetComponent<Button>());
            }
            for (int i = 0; i < list_button_menu.Count; i++)
            {
                int num = i;
                list_button_menu[i].onClick.AddListener(() =>
                {
                    SelectMenu(num);
                });
            }
        }

        void SettingContent()
        {
            foreach (Transform content in Group_Content)
            {
                list_button_content.Add(content.GetComponent<Button>());
            }
            for (int i = 0; i < list_button_content.Count; i++)
            {
                int num = i;
                list_button_content[i].onClick.AddListener(() =>
                {
                    SelectContent(num);
                });
            }
        }

        void SelectMode(int number)
        {
            switch ((모드)number)
            {
                case 모드.실습모드:
                    {
                        isMode = false;
                        Text_Title.text = "<b>준비작동식 스프링클러(실습모드)</b>";
                    }
                    break;
                case 모드.평가모드:
                    {
                        isMode = true;
                        Text_Title.text = "<b>준비작동식 스프링클러(평가모드)</b>";
                    }
                    break;
            }

            StartCoroutine(CorRebuildLayout(Rect_Title));
            ChanageGroup(1);
        }

        IEnumerator CorRebuildLayout(RectTransform obj)
        {
            yield return new WaitForEndOfFrame();
            LayoutRebuilder.ForceRebuildLayoutImmediate(obj);
        }

        void SelectMenu(int number)
        {
            switch ((메뉴)number)
            {
                case 메뉴.점검전조치:
                    {
                        if (isMode)
                        {
                            ScenarioManager.Instance.SelectScenario(SCENARIO.평가점검, isMode);
                        }
                        else
                        {
                            ScenarioManager.Instance.SelectScenario(SCENARIO.점검, isMode);
                        }
                        gameObject.SetActive(false);
                        ResultPopup.SetActive(false);
                        ModeSelect.gameObject.SetActive(false);
                    }
                    break;
                case 메뉴.작동방법:
                    {
                        ChanageGroup(2);
                    }
                    break;
                case 메뉴.복구:
                    {
                        if (isMode)
                        {
                            ScenarioManager.Instance.SelectScenario(SCENARIO.평가복구, isMode);
                        }
                        else
                        {
                            ScenarioManager.Instance.SelectScenario(SCENARIO.복구, isMode);
                        }
                        gameObject.SetActive(false);
                        ResultPopup.SetActive(false);
                        ModeSelect.gameObject.SetActive(false);
                    }
                    break;
            }
        }

        void SelectContent(int number)
        {
            switch ((작동방법)number)
            {
                case 작동방법.감지기:
                    if (isMode)
                    {
                        ScenarioManager.Instance.SelectScenario(SCENARIO.평가감지기, isMode);
                    }
                    else
                    {
                        ScenarioManager.Instance.SelectScenario(SCENARIO.감지기, isMode);
                    }
                    break;
                case 작동방법.수동조작함:
                    if (isMode)
                    {
                        ScenarioManager.Instance.SelectScenario(SCENARIO.평가수동조작함, isMode);
                    }
                    else
                    {
                        ScenarioManager.Instance.SelectScenario(SCENARIO.수동조작함, isMode);
                    }
                    break;
                case 작동방법.수동기동밸브:
                    if (isMode)
                    {
                        ScenarioManager.Instance.SelectScenario(SCENARIO.평가수동기동밸브, isMode);
                    }
                    else
                    {
                        ScenarioManager.Instance.SelectScenario(SCENARIO.수동기동밸브, isMode);
                    }
                    break;
                case 작동방법.수동기동스위치:
                    if (isMode)
                    {
                        ScenarioManager.Instance.SelectScenario(SCENARIO.평가수동기동스위치, isMode);
                    }
                    else
                    {
                        ScenarioManager.Instance.SelectScenario(SCENARIO.수동기동스위치, isMode);
                    }
                    break;
                case 작동방법.동작시험:
                    if (isMode)
                    {
                        ScenarioManager.Instance.SelectScenario(SCENARIO.평가동작시험, isMode);
                    }
                    else
                    {
                        ScenarioManager.Instance.SelectScenario(SCENARIO.동작시험, isMode);
                    }
                    break;
            }
            ModeSelect.gameObject.SetActive(false);
            gameObject.SetActive(false);
            ResultPopup.SetActive(false);
        }

        public void ChanageGroup(int index)
        {
            if (index == 0)
            {
                Text_Title.text = "<b>준비작동식 스프링클러</b>";
                StartCoroutine(CorRebuildLayout(Rect_Title));
            }

            for (int i = 0; i < list_Group.Count; i++)
            {
                if (i == index)
                {
                    pageindex = index;
                    list_Group[i].SetActive(true);
                }
                else
                {
                    list_Group[i].SetActive(false);
                }
            }

            if(pageindex > 0)
            {
                if(isSkip && pageindex == 1)
                {
                    Button_Pre.gameObject.SetActive(false);
                }
                else
                {
                    Button_Pre.gameObject.SetActive(true);
                }
            }
            else
            {
                Button_Pre.gameObject.SetActive(false);
            }
        }
    }
}

