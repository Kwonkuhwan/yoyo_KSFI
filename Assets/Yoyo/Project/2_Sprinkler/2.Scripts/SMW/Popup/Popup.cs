using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMW.Sprinkler
{
    public abstract class Popup : MonoBehaviour
    {
        public bool IsOpenSolValve
        {
            get { return isOpenSolVavle; }
        }
        protected bool isOpenSolVavle;

        abstract protected void Setting();
        abstract protected void RESET();

        public void FirstSetting()
        {
            Setting();
        }

        /// <summary>
        /// 팝업창 켜고 끄기
        /// </summary>
        public void SET(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void Reset()
        {
            RESET();
        }
    }
}
