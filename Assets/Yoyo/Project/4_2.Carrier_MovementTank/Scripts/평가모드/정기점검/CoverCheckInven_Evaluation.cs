using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KKH
{
    public enum CoverCheckType
    {
        위험물 = 0,
        일이공이,
        일이공삼,
        화기사용,
        가열금지,
        None
    }

    public class CoverCheckInven_Evaluation : MonoBehaviour
    {
        public List<CoverCheckType> cctList = new List<CoverCheckType>();
        public List<CoverCheckType> ansers = new List<CoverCheckType>();
        [SerializeField] private int quizNum;

        [SerializeField] private GameObject[] go_Buttons;
        [SerializeField] private GameObject[] go_Units;
        [SerializeField] private ButtonManager_KKH buttonManager_KKH;

        private void Awake()
        {
            cctList.Clear();
        }

        private void Update()
        {
            
            EvaluationManager.Inst.RegularinspectionPoint[quizNum - 1] = ansers.OrderBy(x => x).SequenceEqual(cctList.OrderBy(x => x));

            //int totalCnt = 0;
            //int check = 0;
            //foreach (var cct in cctList)
            //{
            //    foreach (var anser in ansers)
            //    {
            //        if (cct == anser)
            //        {
            //            check++;
            //            break;
            //        }
            //    }
            //    totalCnt++;
            //}

            //if (check == ansers.Count && totalCnt == ansers.Count)
            //{
            //    EvaluationManager.Inst.RegularinspectionPoint[quizNum - 1] = true;
            //}
            //else
            //{
            //    EvaluationManager.Inst.RegularinspectionPoint[quizNum - 1] = false;
            //}
        }

        public bool SetData(CoverCheckType cCType)
        {
            if (cctList.Contains(cCType)) return false;

            cctList.Add(cCType);
            foreach (var unit in go_Units)
            {
                CoverCheckUnit_Evaluation cCTypeUnit = unit.GetComponent<CoverCheckUnit_Evaluation>();
                if (cCTypeUnit != null && !cCTypeUnit.isEnable)
                {
                    if (cCTypeUnit.cCType == cCType)
                    {
                        unit.SetActive(true);
                        cCTypeUnit.SetUnit(cCType);
                        break;
                    }
                }
            }

            return true;
        }
    }
}