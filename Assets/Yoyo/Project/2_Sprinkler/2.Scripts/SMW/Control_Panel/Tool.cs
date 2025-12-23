using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public enum 작동
    {
        자동,
        정지,
        수동
    }

    public enum 조작툴
    {
        전체 = 99,
        SP주펌프 = 0,
        SP충압펌프,
        솔레노이드밸브
    }

    public class Tool : MonoBehaviour
    {
        [Header("버튼")]
        [SerializeField] Transform Group_Button;
        List<Button> list_button = new List<Button>();
        public Button OperationButton
        {
            get { return list_button[0]; }
        }

        [Header("디스플레이")]
        // 0 = Auto, 1 = Stop, 2 = Manual
        [SerializeField] List<GameObject> List_Display = new List<GameObject>();

        public 조작툴 ToolName;
        [SerializeField] GameObject Display_On;

        public int Mode
        {
            get { return indexDisplayMode; }
        }
        int indexDisplayMode = 0;

        public void Setting()
        {
            if (list_button.Count == 0)
            {
                foreach (Transform trs in Group_Button)
                {
                    list_button.Add(trs.GetComponent<Button>());
                }
            }

            // 선택 버튼
            list_button[0].onClick.AddListener(() =>
            {
                indexDisplayMode++;
                if (indexDisplayMode >= List_Display.Count)
                {
                    indexDisplayMode = 0;
                }
                OnClickSelectButton(indexDisplayMode);

                ScenarioManager.Instance.CheckScenarioStep();
            });

            // 기동 버튼
            list_button[1].onClick.AddListener(() =>
            {
                OnClickStartButton();
            });

            Reset();

            HighLight.Instance.OnHighLight += OnHighLight;
        }

        void OnHighLight()
        {
            HighLight.Instance.On(list_button[1]);
        }

        public void SetName(int index)
        {
            ToolName = (조작툴)index;
        }

        public void Reset()
        {
            indexDisplayMode = 0;
            OnClickSelectButton(indexDisplayMode);
        }

        void OnClickSelectButton(int index)
        {
            switch (ToolName)
            {
                case 조작툴.SP주펌프:
                    LogManager.Instance.SetLog($"S/P 주펌프 {(작동)indexDisplayMode}");
                    break;
                case 조작툴.SP충압펌프:
                    LogManager.Instance.SetLog($"S/P 충압펌프 {(작동)indexDisplayMode}");
                    break;
                case 조작툴.솔레노이드밸브:
                    LogManager.Instance.SetLog($"솔레노이드 밸브 {(작동)indexDisplayMode}");
                    break;
            }

            for (int i = 0; i < List_Display.Count; i++)
            {
                if(i == index)
                {
                    List_Display[i].SetActive(true);
                }
                else
                {
                    List_Display[i].SetActive(false);
                }
            }
        }

        /// <summary>
        /// 동작버튼 상태 변경
        /// </summary>
        /// <param name="index"> 0 - 자동, 1 - 정지, 2 - 수동  </param>
        public void Turn(int index)
        {
            indexDisplayMode = index;
            for (int i = 0; i < List_Display.Count; i++)
            {
                if (i == index)
                {
                    List_Display[i].SetActive(true);
                }
                else
                {
                    List_Display[i].SetActive(false);
                }
            }
        }

        public void Interactive(bool isInteractive, bool isMove = false)
        {
            if(isMove)
            {
                list_button[1].interactable = isInteractive;
            }
            else
            {
                list_button[0].interactable = isInteractive;
            }
        }

        void OnClickStartButton()
        {
            if (indexDisplayMode != 2) return;
            switch (ToolName)
            {
                case 조작툴.SP주펌프:
                    LogManager.Instance.SetLog("S/P 주펌프 기동");
                    break;
                case 조작툴.SP충압펌프:
                    LogManager.Instance.SetLog("S/P 충압펌프 기동");
                    break;
                case 조작툴.솔레노이드밸브:
                    LogManager.Instance.SetLog("솔레노이드 밸브 기동");
                    ScenarioManager.Instance.OnClickSolVavleTool();
                    break;
            }
        }

        public void Display(bool isActive)
        {
            if(Display_On != null)
            {
                Display_On.SetActive(isActive);
            }
        }
    }
}
