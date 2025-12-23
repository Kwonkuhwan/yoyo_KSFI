using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public class DangerousTransportationSelectInven_Evaluation : MonoBehaviour
    {
        [SerializeField] private DangerousTransportationUnitItem_Evaluation[] dTUnitItem_Evaluations;
        [SerializeField] private DangerousTransportationInven_Evaluation dTRightInven;
        [SerializeField] private DangerousTransportationInven_Evaluation dTErrorInven;

        private void Awake()
        {
            dTUnitItem_Evaluations = GetComponentsInChildren<DangerousTransportationUnitItem_Evaluation>();
        }

        public bool Rollback(EDangerousTransportation ddtType, bool isRight)
        {
            if (isRight)
            {
                foreach (var go in dTUnitItem_Evaluations)
                {
                    if (go.ddtType == ddtType)
                    {
                        dTRightInven.dttList.Remove(ddtType);
                        go.gameObject.SetActive(true);
                        return true;
                    }
                }
            }
            else
            {
                foreach (var go in dTUnitItem_Evaluations)
                {
                    if (go.ddtType == ddtType)
                    {
                        dTErrorInven.dttList.Remove(ddtType);
                        go.gameObject.SetActive(true);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}