using System.Collections;
using System.Collections.Generic;
using LJS;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomPopupMonoBehaviour : MonoBehaviour, IPointerClickHandler
{
    [Header("팝업 부모 GUI Object 등록")]
    [SerializeField] protected Button closeBtn;
    [SerializeField] protected TextMeshProUGUI closeBtnText;

    protected bool fistNickName = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        Util.Log(gameObject.name);
        if (fistNickName) return;
        if (!eventData.pointerEnter.Equals(this.gameObject))
            return;
        //this.gameObject.SetActive(false);

    }
}
