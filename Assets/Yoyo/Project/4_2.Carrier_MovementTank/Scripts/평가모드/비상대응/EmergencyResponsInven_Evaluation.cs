using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public enum EEmergencyResponsType
    {
        전화 = 0,
        삼각대,
        위험물확산방지,
        위험경보,
        선택안함,
        None
    }
    public class EmergencyResponsInven_Evaluation : MonoBehaviour
    {
        public int quziNum = 0;
        public List<EEmergencyResponsType> eERTypeList = new List<EEmergencyResponsType>();
        public EmergencyResponsUnit_Evaluation[] units;

        private void Awake()
        {
            eERTypeList.Add(EEmergencyResponsType.전화);
            eERTypeList.Add(EEmergencyResponsType.삼각대);
            eERTypeList.Add(EEmergencyResponsType.위험물확산방지);
            eERTypeList.Add(EEmergencyResponsType.위험경보);
        }

        private void Update()
        {
            bool isCheck = false;
            for (int i = 0; i < eERTypeList.Count; i++)
            {
                if (units[i].eERType != eERTypeList[i])
                {
                    isCheck = false;
                    break;
                }

                isCheck = true;
            }

            EvaluationManager.Inst.EmergencyResponsPoint[quziNum - 1] = isCheck;
        }
    }
}
