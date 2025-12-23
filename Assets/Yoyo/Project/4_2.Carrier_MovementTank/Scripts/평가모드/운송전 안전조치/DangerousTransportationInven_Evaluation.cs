using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public enum EDangerousTransportation
    {
        소화기 = 0,
        메가폰,
        소석회,
        적색기및경광봉,
        삼각대,
        방독면,
        None
    }

    public class DangerousTransportationInven_Evaluation : MonoBehaviour
    {
        public List<EDangerousTransportation> dttList = new List<EDangerousTransportation>();
        public List<EDangerousTransportation> ansers = new List<EDangerousTransportation>();

        [SerializeField] private GameObject[] go_Buttons;
        [SerializeField] private GameObject[] go_Units;
        [SerializeField] private ButtonManager_KKH buttonManager_KKH;

        public bool isCheck = false;

        private void Awake()
        {
            dttList.Clear();
        }

        private void Update()
        {
            int totalCnt = 0;
            int check = 0;
            foreach (var cct in dttList)
            {
                foreach (var anser in ansers)
                {
                    if (cct == anser)
                    {
                        check++;
                        break;
                    }
                }
                totalCnt++;
            }

            if (check == ansers.Count && totalCnt == ansers.Count)
            {
                isCheck = true;
            }
            else
            {
                isCheck = false;
            }
        }

        public bool SetData(EDangerousTransportation dDtype)
        {
            if (dttList.Contains(dDtype)) return false;

            dttList.Add(dDtype);
            foreach (var unit in go_Units)
            {
                DangerousTransportationUnit_Evaluation DUnit = unit.GetComponent<DangerousTransportationUnit_Evaluation>();
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
