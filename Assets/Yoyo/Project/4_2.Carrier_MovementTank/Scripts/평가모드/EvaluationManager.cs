using UnityEngine;

namespace KKH
{
    public enum EvalResult
    {
        Success = 0,
        Defer,
        Fail,
    }

    public class EvaluationManager : MonoBehaviour
    {
        private static EvaluationManager instacne;
        public static EvaluationManager Inst => instacne;

        public int DocumentSucessCnt = 2;
        public bool[] DocumentPoint = { false, false, false, false };

        public int RegularinspectionSucessCnt = 3;
        public bool[] RegularinspectionPoint = { false, false, false, false };

        public int SafetyMeasuresSucessCnt = 2;
        public bool[] SafetyMeasuresPoint = { false, false };

        public int EmergencyResponsSucessCnt = 4;
        public bool[] EmergencyResponsPoint = { false, false, false, false, false };

        private void Awake()
        {
            instacne = this;
        }

        private EvalResult GetEvalResult(int trueCnt, int point)
        {
            if (trueCnt == point)
            {
                return EvalResult.Success;
            }
            else if (trueCnt == 0)
            {
                return EvalResult.Fail;
            }
            else
            {
                return EvalResult.Defer;
            }
        }

        public EvalResult GetDocumentSuccess()
        {
            int trueCnt = 0;
            foreach (bool value in DocumentPoint)
            {
                if (value)
                {
                    trueCnt++;
                }
            }

            return GetEvalResult(trueCnt, DocumentPoint.Length);
        }

        public EvalResult GetRegularinspectionSuccess()
        {
            int trueCnt = 0;
            foreach (bool value in RegularinspectionPoint)
            {
                if (value)
                {
                    trueCnt++;
                }
            }

            return GetEvalResult(trueCnt, RegularinspectionPoint.Length);
        }

        public EvalResult GetSafetyMeasuresSuccess()
        {
            int trueCnt = 0;
            foreach (bool value in SafetyMeasuresPoint)
            {
                if (value)
                {
                    trueCnt++;
                }
            }

            return GetEvalResult(trueCnt, SafetyMeasuresPoint.Length);

        }

        public EvalResult GetEmergencyResponsSuccess()
        {
            int trueCnt = 0;
            foreach (bool value in EmergencyResponsPoint)
            {
                if (value)
                {
                    trueCnt++;
                }
            }

            return GetEvalResult(trueCnt, EmergencyResponsPoint.Length);
        }
    }
}