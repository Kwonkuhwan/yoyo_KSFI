using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public class DeceleratingInfoMenu : InfoMenu
    {
        private static DeceleratingInfoMenu instance;
        public static DeceleratingInfoMenu Inst => instance;

        [SerializeField] private DeceleratingPopup popup;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            instance = this;

            PageScroll(infoIdx);
        }

        public override void PrevBtnClick()
        {
            base.PrevBtnClick();
            PageScroll(infoIdx);
        }

        public override void NextIndex()
        {
            base.NextIndex();
            PageScroll(infoIdx);
        }

        public override void NextBtnClick()
        {
            if (infoIdx == 1)
            {
                if (popup.SelectNum != 1)
                {
                    if (popup.IncreaseErrorCount())
                    {
                        return;
                    }
                }
            }

            base.NextBtnClick();
            PageScroll(infoIdx);
        }

        private void PageScroll(int idx)
        {
            if (obj_BackGrounds == null || obj_BackGrounds.Length <= 0) return;
            ShowActionPanel(obj_BackGrounds[idx]);

            if (idx == 0)
            {
                btn_Next.gameObject.SetActive(false);
            }
            else if (idx == 1)
            {
                btn_Next.gameObject.SetActive(true);
            }
        }

        private void ShowActionPanel(GameObject obj)
        {
            foreach (var go in obj_BackGrounds)
            {
                try
                {
                    if (obj == go)
                    {
                        if (!go.activeInHierarchy)
                        {
                            go.SetActive(true);
                        }
                    }
                    else
                    {
                        go.SetActive(false);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}