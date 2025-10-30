using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public class EmergencyResponseSelectInven : MonoBehaviour
    {
        [SerializeField] private EmergencyResponsUnitItem_Evaluation[] dangerUnitItem_Evaluations;
        [SerializeField] private EmergencyResponsInven_Evaluation dangerInven;

        private void Awake()
        {
            dangerUnitItem_Evaluations = GetComponentsInChildren<EmergencyResponsUnitItem_Evaluation>();
        }

        public bool Rollback(EEmergencyResponsType erType)
        {
            foreach (var go in dangerUnitItem_Evaluations)
            {
                if (go.eERType == erType)
                {
                    //dangerInven.eERTypeList.Remove(erType);
                    go.gameObject.SetActive(true);
                    return true;
                }
            }
            return false;
        }
    }
}