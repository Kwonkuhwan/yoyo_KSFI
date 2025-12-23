using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KKH
{
    public class TableofContentsPopup : MonoBehaviour
    {
        [SerializeField] protected Button btn_Practice;
        [SerializeField] protected Button btn_Evaluation;

        [SerializeField] protected GameObject panel_Mode;
        [SerializeField] protected GameObject panel_Practice;

        [SerializeField] protected Button btn_Title;
        [SerializeField] protected Button btn_cancle;
        [SerializeField] protected Button btn_Exit;

        [SerializeField] protected Button btn_PracticePrev;
        

        protected virtual void Awake()
        {
            if (btn_Practice)
            {
                btn_Practice?.onClick.AddListener(delegate
                {
                    panel_Mode?.SetActive(false);
                    panel_Practice?.SetActive(true);
                });
            }

            if (btn_Evaluation)
            {
                btn_Evaluation?.onClick.AddListener(delegate
                {
                    SceneManager.LoadSceneAsync("Carrier_Movenment_Evaluation");
                });
            }

            if (btn_Title)
            {
                btn_Title.onClick?.AddListener(delegate
                {
                    SceneManager.LoadSceneAsync("TitleScene");
                });
            }

            if (btn_PracticePrev)
            {
                btn_PracticePrev?.onClick.AddListener(delegate
                {
                    panel_Mode?.SetActive(true);
                    panel_Practice?.SetActive(false);
                });
            }

            if (btn_cancle)
            {
                btn_cancle?.onClick.AddListener(() => CancleBtnClick());
            }

            if (btn_Exit)
            {
                btn_Exit?.onClick.AddListener(() => ExitBtnClick());
            }

#if KFSI_ALL
#else
#if KFSI_TEST
#else
                btn_PracticePrev.gameObject.SetActive(false);
#endif
#endif
            // 2025-03-18 RJH WEBGL 종료버튼 비활성화
#if UNITY_WEBGL
            btn_Title.transform.position = btn_Exit.transform.position;
            btn_Exit.gameObject.SetActive(false);
#endif
        }

        protected virtual void OnEnable()
        {
#if KFSI_ALL
            if (panel_Mode != null)
            {
                panel_Mode?.SetActive(true);
            }

            if (panel_Practice != null)
            {
                panel_Practice?.SetActive(false);
            }
#else
    #if KFSI_TEST
    #else
                if (panel_Mode != null)
                {
                    panel_Mode?.SetActive(false);
                }

                if (panel_Practice != null)
                {
                    panel_Practice?.SetActive(true);
                }
    #endif
#endif
        }

        protected virtual void CancleBtnClick()
        {
            gameObject.SetActive(false);
        }

        protected virtual void ExitBtnClick()
        {
            CanvasControl.Inst.panel_Quit.SetActive(true);
        }
    }
}
