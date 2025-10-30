using UnityEngine;

namespace KKH {
    public class DangerSelectInven_Evaluation : MonoBehaviour
    {
        [SerializeField] private DangerUnitItem_Evaluation[] dangerUnitItem_Evaluations;
        [SerializeField] private DangerInven_Evaluation dangerInven;

        private void Awake()
        {
            dangerUnitItem_Evaluations = GetComponentsInChildren<DangerUnitItem_Evaluation>();
        }

        public bool Rollback(DangerDocumentType dDtype)
        {
            foreach (var go in dangerUnitItem_Evaluations)
            {
                if (go.dDtype == dDtype)
                {
                    dangerInven.dDtList.Remove(dDtype);
                    go.gameObject.SetActive(true);
                    return true;
                }
            }
            return false;
        }
    }
}