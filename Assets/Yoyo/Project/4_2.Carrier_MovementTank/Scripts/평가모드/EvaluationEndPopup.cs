using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KKH
{
    public class EvaluationEndPopup : MonoBehaviour
    {
        [SerializeField] private Sprite[] sprtes;
        [SerializeField] private Image[] image_Success;

        [SerializeField] private Button btn_Title;
        [SerializeField] private Button btn_SelectMode;
        [SerializeField] private Button btn_Exit;

        [SerializeField] private TableofContents tableofContents;

        [SerializeField] private Button[] btns;
        [SerializeField] private GameObject go_TestResultPopups;

        private void Awake()
        {
            btn_Title.onClick.AddListener(() => TitleBtnClick());
            btn_SelectMode.onClick.AddListener(() =>SelectModeBtnClick());
            btn_Exit.onClick.AddListener(() => CanvasControl_Evaluation.Inst.panel_Quit.SetActive(true));

            for(int i=0; i < btns.Length; i++)
            {
                int idx = i;
                btns[idx].onClick.AddListener(() => TestResultBtnClick(idx));
            }

            // 2025-03-18 RJH WEBGL 종료버튼 비활성화
#if UNITY_WEBGL
            btn_Exit.gameObject.SetActive(false);
#endif
        }

        private void TestResultBtnClick(int num)
        {
            EvaluationType testType = (EvaluationType)num;
            go_TestResultPopups.GetComponent<TestResultPopup>().SetResult(testType);
            go_TestResultPopups.transform.parent.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            image_Success[(int)EvaluationType.서류점검].sprite = sprtes[(int)EvaluationManager.Inst.GetDocumentSuccess()];
            image_Success[(int)EvaluationType.정기점검].sprite = sprtes[(int)EvaluationManager.Inst.GetRegularinspectionSuccess()];
            image_Success[(int)EvaluationType.운송전안전조치].sprite = sprtes[(int)EvaluationManager.Inst.GetSafetyMeasuresSuccess()];
            image_Success[(int)EvaluationType.비상대응].sprite = sprtes[(int)EvaluationManager.Inst.GetEmergencyResponsSuccess()];
        }

        private void TitleBtnClick()
        {
            SceneManager.LoadSceneAsync("TitleScene");
        }

        private void SelectModeBtnClick()
        {
            tableofContents.ShowPanel();
        }
    }
}