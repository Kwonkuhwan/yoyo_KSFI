using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class DangerousGoodsSafetyCardPopup : PopUpControl
    {
        [SerializeField] private Button btn_DangerousGoodsSafetyCard;
        [SerializeField] private GameObject popup_DangerousGoodsSafetyCard;

        protected override void Start()
        {
            base.Start();
            btn_DangerousGoodsSafetyCard.onClick.AddListener(() => ShowPopup());
        }

        protected override void CloseBtnClick()
        {
            base.CloseBtnClick();
        }

        private void ShowPopup()
        {
            infoMenu.NextIndex();
            popup_DangerousGoodsSafetyCard.SetActive(true);
        }
    }
}