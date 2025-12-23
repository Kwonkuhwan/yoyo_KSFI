using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public enum 모드
    {
        실습모드,
        평가모드
    }

    public enum 메뉴
    {
        점검전조치,
        작동방법,
        복구
    }

    public enum 작동방법
    {
        감지기,
        수동조작함,
        수동기동밸브,
        수동기동스위치,
        동작시험
    }

    public class ModeSelect : MonoBehaviour
    {
        // false = 실습모드, true = 평가모드
        bool isMode = false;

        [Header("모드")]
        [SerializeField] Transform Group_Mode;
        [SerializeField] GameObject Select_mode;
        //[SerializeField] Button Button_Pre;
        List<Button> list_button_mode = new List<Button>();

        [Header("메뉴")]
        [SerializeField] Transform Group_Menu;
        [SerializeField] List<Button> list_button_menu = new List<Button>();

        [Header("평가모드")]
        [SerializeField] Transform Group_Content;
        [SerializeField] List<Button> list_button_content = new List<Button>();

        [Header("버튼들")]
        [SerializeField] List<Button> list_button_exit = new List<Button>();
        [SerializeField] List<Button> list_button_home = new List<Button>();
        [SerializeField] List<Button> list_button_pre = new List<Button>();

        [Header("제목")]
        [SerializeField] List<TMP_Text> list_text_title = new List<TMP_Text>();
        //[SerializeField] Button Button_Exit;
        //[SerializeField] Button Button_Home;

        // 그룹들 리스트
        List<GameObject> list_Group = new List<GameObject>();

        int pageIndex = 0;

        bool isSkip = false;

        void Awake()
        {
            // 그룹UI리스트 세팅
            list_Group.Add(Select_mode);
            list_Group.Add(Group_Menu.gameObject);
            list_Group.Add (Group_Content.gameObject);

            // 모드 세팅
            foreach (Transform mode in Group_Mode)
            {
                list_button_mode.Add(mode.GetComponent<Button>());
            }
            for(int i = 0; i < list_button_mode.Count; i++)
            {
                int num = i;
                list_button_mode[i].onClick.AddListener(()=>
                {
                    SelectMode(num);
                });
            }

            // 메뉴 세팅
            for (int i = 0; i < list_button_menu.Count; i++)
            {
                int num = i;
                list_button_menu[i].onClick.AddListener(() =>
                {
                    SelectMenu(num);
                });
            }

            for (int i = 0; i < list_button_content.Count; i++)
            {
                int num = i;
                list_button_content[i].onClick.AddListener(() =>
                {
                    SelectContent(num);
                });
            }

            // 홈 버튼
            for (int i = 0; i < list_button_home.Count; i++)
            {
                list_button_home[i].onClick.AddListener(() =>
                {
                    SceneManager.LoadSceneAsync("TitleScene");
                });
            }

#if UNITY_WEBGL
            for (int i = 0; i < list_button_exit.Count; i++)
            {
                list_button_exit[i].gameObject.SetActive(false);
            }

            list_button_home[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(1466, -908);
            list_button_home[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(1779, -908);
#else
            for (int i = 0; i < list_button_exit.Count; i++)
            {
                list_button_exit[i].onClick.AddListener(() =>
                {
                    Application.Quit();
                });
            }
#endif
            for (int i = 0; i < list_button_pre.Count; i++)
            {
                list_button_pre[i].onClick.AddListener(() =>
                {
                    pageIndex--;
                    ChanageGroup(pageIndex);
                });
            }

#if KFSI_ALL
            isSkip = false;
#elif KFSI_TEST
            isSkip = true;
            SelectMode((int)모드.평가모드);
#elif !KFSI_TEST
            isSkip = true;
            SelectMode((int)모드.실습모드);
#endif
            //HighLight.Instance.OnHighLight += OnHighLight;
        }

        private void OnEnable()
        {
            SoundManager.Instance.MuteAll(true);
            SoundManager.Instance.PlayPump(false);
        }

        void OnHighLight()
        {
            HighLight.Instance.On(list_button_content);
            HighLight.Instance.On(list_button_menu);
            HighLight.Instance.On(list_button_mode);
        }

        /// <summary>
        /// 모드 선택
        /// </summary>
        public void SelectMode(int number)
        {
            switch ((모드)number)
            {
                case 모드.실습모드:
                    {
                        isMode = false;
                        for(int i = 0; i < list_text_title.Count; i++)
                        {
                            list_text_title[i].text = "<b>준비작동식 스프링클러(실습모드)</b>";
                        }
                    }
                    break;
                case 모드.평가모드:
                    {
                        isMode = true;
                        for (int i = 0; i < list_text_title.Count; i++)
                        {
                            list_text_title[i].text = "<b>준비작동식 스프링클러(평가모드)</b>";
                        }
                    }
                    break;
            }
            ChanageGroup(1);
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
                    }
                    break;
            }
        }

        void SelectContent(int number)
        {
            switch ((작동방법)number)
            {
                case 작동방법.감지기:
                    if(isMode)
                    {
                        ScenarioManager.Instance.SelectScenario(SCENARIO.평가감지기, isMode);
                    }
                    else
                    {
                        ScenarioManager.Instance.SelectScenario(SCENARIO.감지기, isMode);
                    }
                    break;
                case 작동방법.수동조작함:
                    if(isMode)
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
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 메뉴 및 모드 선택 UI변경
        /// </summary>
        /// <param name="index"> 0 = 모드선택, 1 = 메뉴선택, 2 = 컨텐츠 선택 </param>
        public void ChanageGroup(int index)
        {
            if(index > 0)
            {
                for(int i = 0; i < list_button_home.Count; i++)
                {
                    list_button_home[i].gameObject.SetActive(true);
#if !UNITY_WEBGL
                    list_button_exit[i].gameObject.SetActive(true);
#endif
                }
            }
            else
            {
                for (int i = 0; i < list_button_home.Count; i++)
                {
                    list_button_home[i].gameObject.SetActive(false);
#if !UNITY_WEBGL
                    list_button_exit[i].gameObject.SetActive(true);
#endif
                }
            }

            for (int i = 0; i < list_Group.Count; i++)
            {
                if (i == index)
                {
                    pageIndex = index;
                    list_Group[i].SetActive(true);
                }
                else
                {
                    list_Group[i].SetActive(false);
                }
            }

            if (pageIndex > 0)
            {
                for(int i = 0; i < list_button_pre.Count; i++)
                {
                    if(isSkip && i == 0)
                    {
                        list_button_pre[i].gameObject.SetActive(false);
                        continue;
                    }
                    list_button_pre[i].gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < list_button_pre.Count; i++)
                {
                    list_button_pre[i].gameObject.SetActive(false);
                }
            }
        }
    }
}