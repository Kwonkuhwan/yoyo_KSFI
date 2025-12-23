using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KKH {
    public class GroundTerminalBoxPopup : MonoBehaviour
    {
        [SerializeField] private Button btn_Done;

        private void Awake()
        {
            btn_Done.onClick.AddListener(() => DoneBtnClick());
        }

        private void DoneBtnClick()
        {
            SafetyBeforeTransportationInfoMenu.Inst.NextIndex();
        }
    }
}
