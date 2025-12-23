using UnityEngine;

namespace KKH
{
    public class TankFixInfoMenu : InfoMenu
    {
        private static TankFixInfoMenu instance;
        public static TankFixInfoMenu Inst => instance;

        [SerializeField] private TankBody tankBody;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            instance = this;
        }

        public override void PrevBtnClick()
        {
            base.PrevBtnClick();

            PageScroll(infoIdx);
        }

        public override void NextBtnClick()
        {
            base.NextBtnClick();

            PageScroll(infoIdx);
        }

        private void PageScroll(int idx)
        {
            if (idx <= 0)
            {
                tankBody.checkList.SetActive(false);
            }
            else if (idx == 1)
            {
                tankBody.checkList.SetActive(true);
            }
        }
    }
}