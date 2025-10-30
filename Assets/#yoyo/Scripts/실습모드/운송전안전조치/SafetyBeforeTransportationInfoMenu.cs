using UnityEngine;

namespace KKH
{
    public class SafetyBeforeTransportationInfoMenu : InfoMenu
    {
        private static SafetyBeforeTransportationInfoMenu instance;
        public static SafetyBeforeTransportationInfoMenu Inst => instance;

        public GameObject go_BtnParent;

        public CarryOnInspectionInven carryOnInspectionInven;

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

        private void OnDisable()
        {
            //infoIdx = 0;
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
            if (carryOnInspectionInven && carryOnInspectionInven.gameObject.activeInHierarchy)
            {
                if (!carryOnInspectionInven.AnserCheck())
                {
                    return;
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
                if (go_BtnParent != null)
                {
                    go_BtnParent.SetActive(true);
                }
            }
            else if (idx == 1)
            {
                if (go_BtnParent != null)
                {
                    go_BtnParent.SetActive(false);
                }
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