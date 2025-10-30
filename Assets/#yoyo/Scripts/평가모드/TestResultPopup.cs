using SMW.Sprinkler;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class TestResultPopup : MonoBehaviour
    {
        [SerializeField] private Button btn_Close;
        [SerializeField] private ResultBox[] go_ResultBoxs;

        [SerializeField] private TMP_Text text_Title;
        [SerializeField] private string[] strs_Title;
        

        [SerializeField] private string[] strs_Document;
        [SerializeField] private string[] strs_Regular;
        [SerializeField] private string[] strs_SM;
        [SerializeField] private string[] strs_ER;

        private void Awake()
        {
            if (btn_Close != null)
            {
                btn_Close.onClick.AddListener(() => transform.parent.gameObject.SetActive(false));
            }
        }

        private void OnDisable()
        {
            ClearContent();
        }

        public void ClearContent()
        {
            foreach(var go in go_ResultBoxs)
            {
                go.gameObject.SetActive(false);
            }
        }

        public void SetResult(EvaluationType testType)
        {
            if(testType == EvaluationType.서류점검)
            {
                text_Title.text = strs_Title[0];
                for (int i =0; i< strs_Document.Length; i++)
                {
                    int idx = i;
                    go_ResultBoxs[idx].Set(strs_Document[idx], EvaluationManager.Inst.DocumentPoint[idx]);
                }
            }
            else if(testType == EvaluationType.정기점검)
            {
                text_Title.text = strs_Title[1];
                for (int i = 0; i < strs_Regular.Length; i++)
                {
                    int idx = i;
                    go_ResultBoxs[idx].Set(strs_Regular[idx], EvaluationManager.Inst.RegularinspectionPoint[idx]);
                }
            }
            else if (testType == EvaluationType.운송전안전조치)
            {
                text_Title.text = strs_Title[2];
                for (int i = 0; i < strs_SM.Length; i++)
                {
                    int idx = i;
                    go_ResultBoxs[idx].Set(strs_SM[idx], EvaluationManager.Inst.SafetyMeasuresPoint[idx]);
                }
            }
            else if (testType == EvaluationType.비상대응)
            {
                text_Title.text = strs_Title[3];
                for (int i = 0; i < strs_ER.Length; i++)
                {
                    int idx = i;
                    go_ResultBoxs[idx].Set(strs_ER[idx], EvaluationManager.Inst.EmergencyResponsPoint[idx]);
                }
            }
        }

        
    }
}