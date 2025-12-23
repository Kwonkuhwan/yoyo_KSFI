using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public class CoverCheckSelectInven_Evaluation : MonoBehaviour
    {
        [SerializeField] private CoverCheckUnitItem_Evaluation[] coverCheckUnitItem_Evaluations;
        [SerializeField] private CoverCheckInven_Evaluation coverCheckInven;

        private void Awake()
        {
            coverCheckUnitItem_Evaluations = GetComponentsInChildren<CoverCheckUnitItem_Evaluation>();
        }

        public bool Rollback(CoverCheckType cct)
        {
            foreach (var go in coverCheckUnitItem_Evaluations)
            {
                if (go.ccType == cct)
                {
                    coverCheckInven.cctList.Remove(cct);
                    go.gameObject.SetActive(true);
                    return true;
                }
            }
            return false;
        }
    }
}