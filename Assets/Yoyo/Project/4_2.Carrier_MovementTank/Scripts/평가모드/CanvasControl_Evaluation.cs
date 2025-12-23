using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public class CanvasControl_Evaluation : MonoBehaviour
    {
        private static CanvasControl_Evaluation instance;
        public static CanvasControl_Evaluation Inst => instance;

        public GameObject panel_Evaluation;
        public GameObject panel_EvalEndpopup;
        public GameObject panel_Quit;

        private void Awake()
        {
            instance = this;
            panel_Evaluation.SetActive(true);
            panel_EvalEndpopup.SetActive(false);
            panel_Quit.SetActive(false);
        }

        private void Update()
        {
            // 2025-03-18 RJH WEBGL ESC 비활성화
#if !UNITY_WEBGL
            if (Input.GetKeyDown(KeyCode.Escape))
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
#endif
        }
    }
}