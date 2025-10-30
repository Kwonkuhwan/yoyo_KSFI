using UnityEngine;

namespace KKH
{
    public class DangerousInfoMenu : InfoMenu
    {
        private static DangerousInfoMenu instance;
        public static DangerousInfoMenu Inst => instance;

        public GameObject go_BtnParent;

        public GameObject panel_AccidentReport;
        public GameObject panel_DangerousAccident;

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
            if (!GameStateCheck()) return;

            base.NextIndex();
            PageScroll(infoIdx);
        }

        public override void NextBtnClick()
        {
            if (!GameStateCheck()) return;

            base.NextBtnClick();
            PageScroll(infoIdx);
        }

        private bool GameStateCheck()
        {
            if (panel_AccidentReport != null)
            {
                if (panel_AccidentReport.activeInHierarchy)
                {
                    AccidentReportPopup arp = panel_AccidentReport.GetComponent<AccidentReportPopup>();
                    if (arp.count < 3)
                    {
                        if (!arp.isRightAnser)
                        {
                            arp.IncreaseErrorCount();
                            return false;
                        }
                    }
                }
            }

            if (panel_DangerousAccident != null)
            {
                if (panel_DangerousAccident.activeInHierarchy)
                {
                    DangerousAccidentPopup dap = panel_DangerousAccident.GetComponent<DangerousAccidentPopup>();
                    if (dap.count < 3)
                    {
                        if (!dap.isRightAnser)
                        {
                            dap.IncreaseErrorCount();
                            return false;
                        }
                    }
                }
            }

            return true;
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
        }
    }
}