using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnablePopupTextChenage : MonoBehaviour
{
    [TextArea]
    public string enablePopupText;
    [TextArea]
    public string disablePopupText;

    public TextMeshProUGUI popupTMP;

    private void OnEnable()
    {
        popupTMP.text = enablePopupText;
    }

    private void OnDisable()
    {
        popupTMP.text = disablePopupText;
    }
}
