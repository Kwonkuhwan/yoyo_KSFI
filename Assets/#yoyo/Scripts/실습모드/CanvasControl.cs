using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KKH
{
    public class CanvasControl : MonoBehaviour
    {
        private static CanvasControl instance;
        public static CanvasControl Inst => instance;

        public GameObject panel_Selectmode;
        public GameObject panel_Document;
        public GameObject panel_RegularInspection;
        public GameObject panel_Safety_Measures;
        public GameObject panel_Emergency_Response;
        public GameObject panel_TableofContents;

        [SerializeField] private Button btn_Prev;
        [SerializeField] private Button btn_Home;
        [SerializeField] private Button btn_Exit;
        public GameObject panel_Quit;

        public bool isSelectMode = false;

        public bool isAudioMute = false;
        [SerializeField] private AudioSource[] menuAudioSource;

        private void Awake()
        {
            instance = this;

            btn_Prev.onClick.AddListener(() => BtnPrevClick());
            btn_Home.onClick.AddListener(() => BtnHomeClick());
            btn_Exit.onClick.AddListener(() => BtnExitClick());

#if KFSI_ALL

#else
            btn_Prev.gameObject.SetActive(false);
#endif
            // 2025-03-18 RJH WEBGL 종료버튼 비활성화
#if UNITY_WEBGL
            btn_Home.transform.position = btn_Exit.transform.position;
            btn_Exit.gameObject.SetActive(false);
#endif
        }

        private void Update()
        {
            // 2025-03-18 RJH WEBGL ESC 비활성화
#if !UNITY_WEBGL 
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BtnExitClick();
            }
#endif
            //if (panel_Selectmode.activeInHierarchy)
            //{
            //    if (!panel_TableofContents.activeInHierarchy) return;
            //    panel_TableofContents.SetActive(false);
            //}
            //else
            //{
            //    if (panel_TableofContents.activeInHierarchy) return;
            //    panel_TableofContents.SetActive(true);
            //}
        }

        private void BtnPrevClick()
        {
            SceneManager.LoadSceneAsync("TankLorry");
        }

        private void BtnHomeClick()
        {
            SceneManager.LoadSceneAsync("TitleScene");
        }

        private void BtnExitClick()
        {
            if (!panel_Quit.activeInHierarchy)
            {
                panel_Quit.SetActive(true);
            }
            else
            {
                panel_Quit.SetActive(false);
            }
        }

        public void AudioMute()
        {
            isAudioMute = !isAudioMute;
            foreach (var item in menuAudioSource)
            {
                item.mute = isAudioMute;
            }
        }
    }
}