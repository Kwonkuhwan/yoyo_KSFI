using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public class MultiInvenManager : MonoBehaviour
    {
        [SerializeField] private int quizNum;

        [SerializeField] private DangerousTransportationInven_Evaluation[] dangerousTransportationInven_Evaluations;

        private void Update()
        {
            bool isCheck = true;

            foreach(var obj in dangerousTransportationInven_Evaluations)
            {
                if (!obj.isCheck)
                {
                    isCheck = false;
                }

            }

            EvaluationManager.Inst.SafetyMeasuresPoint[quizNum - 1] = isCheck;
        }
    }
}