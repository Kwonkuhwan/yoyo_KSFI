using UnityEngine.SceneManagement;

namespace KKH
{
    public class TableofContentsPopup_Mode : TableofContentsPopup
    {
        protected override void Awake()
        {
            if (btn_Practice)
            {
                btn_Practice?.onClick.AddListener(delegate
                {
                    SceneManager.LoadSceneAsync("Carrier_Movenment");

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
#if KSIF_TEST

#else
                btn_Evaluation.gameObject.SetActive(false);
#endif
#endif
            // 2025-03-18 RJH WEBGL 종료버튼 비활성화
#if UNITY_WEBGL
            btn_Title.transform.position = btn_Exit.transform.position;
            btn_Exit.gameObject.SetActive(false);
#endif
        }

        protected override void ExitBtnClick()
        {
            CanvasControl_Mode.Inst.panel_Quit.SetActive(true);
        }
    }
}