using UnityEngine;
using VInspector;

namespace KKH
{
    public class TankFrameInfoMenu : InfoMenu
    {
        private static TankFrameInfoMenu instance;
        public static TankFrameInfoMenu Inst => instance;

        [SerializeField] private TankBody tankBody;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            instance = this;
        }

        protected override void Start()
        {
            base.Start();
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
            else if(idx == 1)
            {
                tankBody.checkList.SetActive(true);
            }
        }
    }
}