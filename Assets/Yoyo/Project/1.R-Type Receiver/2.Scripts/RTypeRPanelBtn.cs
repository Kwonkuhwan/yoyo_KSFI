using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RTypeRPanelBtn : MonoBehaviour
{
    [SerializeField] public Button btn;
    [SerializeField] public TextMeshProUGUI btnText;

    [HideInInspector] public RTypeRPanelButtonType curBtnType;
    public void Init(RTypeRPanelButtonType type, Action action)
    {
        curBtnType = type;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action.Invoke);
    }

    public void SetEventNum(int index)
    {
        btnText.text = curBtnType switch
        {
            RTypeRPanelButtonType.화재경보 => $"화재경보 {index:D3}",
            RTypeRPanelButtonType.회로차단 => $"회로단선 {index:D3}",
            RTypeRPanelButtonType.설비동작 => $"설비작동 {index:D3}",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
