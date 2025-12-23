using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public enum DangerDocumentType
    {
        완공검사합격확인증 = 0,
        정기점검실시결과,
        소방계획서,
        소화기관리대장,
        차량운행일지,
        위험물안전카드,
        None
    }

    public class DangerInven_Evaluation : MonoBehaviour
    {
        public List<DangerDocumentType> dDtList = new List<DangerDocumentType>();
        public List<DangerDocumentType> ansers = new List<DangerDocumentType>();
        [SerializeField] private int quizNum;

        [SerializeField] private GameObject[] go_Buttons;
        [SerializeField] private GameObject[] go_Units;
        [SerializeField] private ButtonManager_KKH buttonManager_KKH;

        private void Awake()
        {
            dDtList.Clear();

            ansers.Add(DangerDocumentType.완공검사합격확인증);
            ansers.Add(DangerDocumentType.정기점검실시결과);
            ansers.Add(DangerDocumentType.위험물안전카드);
        }

        private void Update()
        {
            int totalCnt = 0;
            int check = 0;
            foreach(var ddt in dDtList)
            {
                foreach(var anser in ansers)
                {
                    if(ddt == anser)
                    {
                        check++;
                        break;
                    }
                }
                totalCnt++;
            }

            if(check == ansers.Count && totalCnt == ansers.Count)
            {
                EvaluationManager.Inst.DocumentPoint[quizNum-1] = true;
            }
            else
            {
                EvaluationManager.Inst.DocumentPoint[quizNum-1] = false;
            }
        }

        public bool SetData(DangerDocumentType dDtype)
        {
            if (dDtList.Count > 2) return false;
            if (dDtList.Contains(dDtype)) return false;

            dDtList.Add(dDtype);
            foreach (var unit in go_Units)
            {
                DangerUnit_Evaluation DUnit = unit.GetComponent<DangerUnit_Evaluation>();
                if (DUnit != null && !DUnit.isEnable)
                {
                    unit.SetActive(true);
                    DUnit.SetUnit(dDtype);
                    break;
                }
            }

            return true;
        }        
    }
}
