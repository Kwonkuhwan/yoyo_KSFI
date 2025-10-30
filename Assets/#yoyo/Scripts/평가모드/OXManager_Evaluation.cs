using KKH;
using UnityEngine;

namespace KKH
{
    public class OXManager_Evaluation : MonoBehaviour
    {
        [SerializeField] private EvaluationType eType = EvaluationType.서류점검;
        [SerializeField] private OX_Evaluation[] ox_Evaluations;

        public int quizNum = 0;

        private void Update()
        {
            bool isCheck = true;
            if(ox_Evaluations != null && ox_Evaluations.Length >= 0)
            {
                foreach(var obj in ox_Evaluations)
                {
                    if (!obj.isCheck)
                    {
                        isCheck = false;
                        break;
                    }
                }
            }
            SetEvaluationPoint(isCheck);
        }

        private void SetEvaluationPoint(bool anser)
        {
            int num = quizNum - 1;

            if (eType == EvaluationType.서류점검)
            {
                EvaluationManager.Inst.DocumentPoint[num] = anser;
            }
            else if (eType == EvaluationType.정기점검)
            {
                EvaluationManager.Inst.RegularinspectionPoint[num] = anser;
            }
            else if (eType == EvaluationType.운송전안전조치)
            {
                EvaluationManager.Inst.SafetyMeasuresPoint[num] = anser;
            }
            else if (eType == EvaluationType.비상대응)
            {
                EvaluationManager.Inst.EmergencyResponsPoint[num] = anser;
            }
        }
    }
}
