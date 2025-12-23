using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class SelectMode : MonoBehaviour
    {
        private static SelectMode instance;
        public static SelectMode Inst => instance;

        [SerializeField] private Button btn_Document;
        [SerializeField] private Button btn_RegularInspection;
        [SerializeField] private Button btn_Safety_Measures;
        [SerializeField] private Button btn_Emergency_Response;

        [SerializeField] private GameObject panel_Document;
        [SerializeField] private GameObject panel_RegularInspection;
        [SerializeField] private GameObject panel_Safety_Measures;
        [SerializeField] private GameObject panel_Emergency_Response;

        private void Awake()
        {
            instance = this;
            btn_Document.onClick.AddListener(delegate
            {
                ShowSelectPanel(panel_Document);
            });
            btn_RegularInspection.onClick.AddListener(delegate
            {
                ShowSelectPanel(panel_RegularInspection);
            });
            btn_Safety_Measures.onClick.AddListener(delegate
            {
                ShowSelectPanel(panel_Safety_Measures);
            });
            btn_Emergency_Response.onClick.AddListener(delegate
            {
                ShowSelectPanel(panel_Emergency_Response);
            });
        }

        private void Start()
        {
            AllPanelHide();
        }

        public void AllPanelHide()
        {
            panel_Document.SetActive(false);
            panel_RegularInspection.SetActive(false);
            panel_Safety_Measures.SetActive(false);
            panel_Emergency_Response.SetActive(false);
        }

        public void ShowSelectPanel(GameObject panel)
        {
            CanvasControl.Inst.isSelectMode = true;

            AllPanelHide();
            panel.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
