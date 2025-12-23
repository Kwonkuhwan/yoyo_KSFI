using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class PracticeMode : MonoBehaviour
    {
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
            btn_Document.onClick.AddListener(delegate
            {
                ShowSelectPanel(panel_Document);
                transform.parent.parent.gameObject.SetActive(false);
            });
            btn_RegularInspection.onClick.AddListener(delegate
            {
                ShowSelectPanel(panel_RegularInspection);
                transform.parent.parent.gameObject.SetActive(false);
            });
            btn_Safety_Measures.onClick.AddListener(delegate
            {
                ShowSelectPanel(panel_Safety_Measures);
                transform.parent.parent.gameObject.SetActive(false);
            });
            btn_Emergency_Response.onClick.AddListener(delegate
            {
                ShowSelectPanel(panel_Emergency_Response);
                transform.parent.parent.gameObject.SetActive(false);
            });
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

            if (panel.activeInHierarchy)
            {
                panel.SetActive(false);
            }
            panel?.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}