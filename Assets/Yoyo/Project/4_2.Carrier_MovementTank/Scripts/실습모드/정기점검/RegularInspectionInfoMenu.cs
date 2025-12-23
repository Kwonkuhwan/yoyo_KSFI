using UnityEngine;
using UnityEngine.UI;
using VInspector;

namespace KKH
{
    public class RegularInspectionInfoMenu : InfoMenu
    {
        private static RegularInspectionInfoMenu instance;
        public static RegularInspectionInfoMenu Inst => instance;

        [Foldout("점검표 팝업")]
        [SerializeField] private int nCheckListHideIdx = 0;
        [SerializeField] private int nCheckListShowIdx = 0;
        [SerializeField] private GameObject go_CheckList;

        [Foldout("점검표 이미지")]
        [SerializeField] private int nCheckList_Image_HideIdx = 0;
        [SerializeField] private int nCheckList_Image_ShowIdx = 0;
        [SerializeField] private GameObject go_CheckListImage;
        [SerializeField] private Button btn_CheckList;

        [Foldout("이미지들")]
        [SerializeField] private Sprite sprite_Background;
        [SerializeField] private Image image_BackGround;

        [SerializeField] private bool isDisabledidxClear = true;

        [Foldout("초기 세팅")]
        [SerializeField] private bool isEnableNext = true;

        protected override void Awake()
        {
            base.Awake();

            if (btn_CheckList != null)
            {
                btn_CheckList.onClick.AddListener(() => NextIndex());
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            instance = this;
            PageScroll(infoIdx);
        }

        private void OnDisable()
        {
            if (isDisabledidxClear)
                infoIdx = 0;
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
            if (idx == 0)
            {
                if (isEnableNext)
                {
                    btn_Next.gameObject.SetActive(true);
                }
                else
                {
                    btn_Next.gameObject.SetActive(false);
                }
            }            

            if (idx >= 1)
            {
                if (!btn_Next.gameObject.activeInHierarchy)
                {
                    btn_Next.gameObject.SetActive(true);
                }
            }

            if (obj_BackGrounds != null && obj_BackGrounds.Length > 0)
            {
                ShowActionPanel(obj_BackGrounds[idx]);
            }

            if (idx <= nCheckList_Image_HideIdx || idx > nCheckList_Image_ShowIdx)
            {
                CheckListImageShowHide(false);
            }
            else if (idx == nCheckList_Image_ShowIdx)
            {
                CheckListImageShowHide(true);
            }

            if (idx <= nCheckListHideIdx)
            {
                CheckListShowHide(false);
            }
            else if (idx == nCheckListShowIdx)
            {
                btn_Next.gameObject.SetActive(false);
                CheckListShowHide(true);
            }
        }

        private void CheckListImageShowHide(bool ison)
        {
            if (go_CheckListImage == null) return;
            go_CheckListImage.SetActive(ison);

            if (image_BackGround == null || sprite_Background == null) return;
            image_BackGround.sprite = sprite_Background;
        }

        private void CheckListShowHide(bool ison)
        {
            if (go_CheckList == null) return;
            go_CheckList.SetActive(ison);
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