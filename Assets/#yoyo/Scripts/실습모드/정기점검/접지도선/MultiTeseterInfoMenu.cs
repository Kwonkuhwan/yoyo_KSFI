using UnityEngine;
using VInspector;

namespace KKH
{
    public class MultiTeseterInfoMenu : InfoMenu
    {
        private static MultiTeseterInfoMenu instance;
        public static MultiTeseterInfoMenu Inst => instance;

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
            base.NextBtnClick();

            PageScroll(infoIdx);
        }

        private void PageScroll(int idx)
        {
            if (obj_BackGrounds == null || obj_BackGrounds.Length <= 0) return;
            ShowActionPanel(obj_BackGrounds[idx]);
            //if (idx <= 0)
            //{
            //    ShowActionPanel(go_Action1);
            //}
            //else if (idx >= 1)
            //{
            //    ShowActionPanel(go_Action2);
            //}
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