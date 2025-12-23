using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public enum EvaluationType
    {
        서류점검 = 0,
        정기점검,
        운송전안전조치,
        비상대응
    }
    public class OX_Evaluation : MonoBehaviour
    {
        [SerializeField] private EvaluationType eType = EvaluationType.서류점검;
        [SerializeField] private Button button_O;
        [SerializeField] private GameObject outline_O;

        [SerializeField] private Button button_X;
        [SerializeField] private GameObject outline_X;

        [SerializeField] private int quizNum;
        [SerializeField] private string rightAnswer;

        [SerializeField] private bool isTotal = false;
        public bool isCheck = false;

        private void Awake()
        {
            button_O.onClick.AddListener(() => OBtnClick());
            button_X.onClick.AddListener(() => XBtnClick());
        }

        private void OBtnClick()
        {
            SetOImage();
        }

        private void XBtnClick()
        {
            SetXImage();
        }

        private void SetOImage()
        {
            outline_O.SetActive(true);
            outline_X.SetActive(false);

            AnserCheck("O");
        }

        private void SetXImage()
        {
            outline_X.SetActive(true);
            outline_O.SetActive(false);

            AnserCheck("X");
        }

        private void AnserCheck(string anser)
        {
            int num = quizNum - 1;

            if (rightAnswer == anser)
            {
                if (!isTotal)
                {
                    if (eType == EvaluationType.서류점검)
                    {
                        EvaluationManager.Inst.DocumentPoint[num] = true;
                    }
                    else if (eType == EvaluationType.정기점검)
                    {
                        EvaluationManager.Inst.RegularinspectionPoint[num] = true;
                    }
                    else if (eType == EvaluationType.운송전안전조치)
                    {
                        EvaluationManager.Inst.SafetyMeasuresPoint[num] = true;
                    }
                    else if (eType == EvaluationType.비상대응)
                    {
                        EvaluationManager.Inst.EmergencyResponsPoint[num] = true;
                    }
                }
                else
                {
                    isCheck = true;
                }
            }
            else
            {
                if (!isTotal)
                {
                    if (eType == EvaluationType.서류점검)
                    {
                        EvaluationManager.Inst.DocumentPoint[num] = false;
                    }
                    else if (eType == EvaluationType.정기점검)
                    {
                        EvaluationManager.Inst.RegularinspectionPoint[num] = false;
                    }
                    else if (eType == EvaluationType.운송전안전조치)
                    {
                        EvaluationManager.Inst.SafetyMeasuresPoint[num] = false;
                    }
                    else if (eType == EvaluationType.비상대응)
                    {
                        EvaluationManager.Inst.EmergencyResponsPoint[num] = false;
                    }
                }
                else
                {
                    isCheck = false;
                }
            }
        }
    }
}