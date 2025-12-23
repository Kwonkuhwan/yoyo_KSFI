using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMW.Sprinkler
{
    public class PowerControlPump : MonoBehaviour
    {
        [SerializeField] List<GameObject> list_Power = new List<GameObject>();

        /// <summary>
        /// 기동, 정지, 펌프기동 Off
        /// </summary>
        public void OnOff(int index, bool isActive)
        {
            list_Power[index].SetActive(isActive);
        }

        public bool Check(int index)
        {
            return list_Power[index].activeSelf;
        }

        public void Reset()
        {
            list_Power[0]?.SetActive(false);
            list_Power[1]?.SetActive(true);
            list_Power[2]?.SetActive(false);
        }
    }
}
