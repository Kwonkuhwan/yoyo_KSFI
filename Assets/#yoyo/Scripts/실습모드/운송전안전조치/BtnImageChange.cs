using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class BtnImageChange : MonoBehaviour
    {
        [SerializeField] private Button btn;

        private void Awake()
        {
            btn = GetComponent<Button>();
            btn.onClick.AddListener(() => SafetyBeforeTransportationInfoMenu.Inst.NextIndex());
        }
    }
}