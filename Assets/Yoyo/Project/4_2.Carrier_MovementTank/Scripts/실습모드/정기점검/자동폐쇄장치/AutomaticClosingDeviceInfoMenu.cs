using UnityEngine;
using UnityEngine.UI;
using VInspector;

namespace KKH
{
    public class AutomaticClosingDeviceInfoMenu : InfoMenu
    {
        private static AutomaticClosingDeviceInfoMenu instance;
        public static AutomaticClosingDeviceInfoMenu Inst => instance;

        [Foldout("비상스위치 팝업")]
        [SerializeField] private GameObject go_EmergencySwitchPopup;

        [Foldout("비상스위치 버튼")]
        [SerializeField] private Button btn_비상버튼;

        [Foldout("체크리스트")]
        [SerializeField] private int nCheckListHideIdx = 0;
        [SerializeField] private int nCheckListShowIdx = 0;
        [SerializeField] private GameObject go_CheckList;

        [Foldout("점검표 이미지")]
        [SerializeField] private int nCheckList_Image_HideIdx = 0;
        [SerializeField] private int nCheckList_Image_ShowIdx = 0;
        [SerializeField] private GameObject go_CheckListImage;
        [SerializeField] private Button btn_CheckList;

        [Foldout("사운드")]
        [SerializeField] private AudioSource audio_MultiTester;

        protected override void Awake()
        {
            base.Awake();

            //btn_비상버튼.onClick.AddListener(delegate { NextIndex(); btn_비상버튼.gameObject.SetActive(false); });
            btn_CheckList.onClick.AddListener(() => NextIndex());
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            instance = this;
            if (go_EmergencySwitchPopup.activeInHierarchy)
            {
                go_EmergencySwitchPopup.SetActive(false);
            }

            //if (!btn_비상버튼.gameObject.activeInHierarchy)
            //{
            //    btn_비상버튼.gameObject.SetActive(true);
            //}
        }

        private void OnDisable()
        {
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
            if(idx == 0)
            {
                go_EmergencySwitchPopup.SetActive(false);
                //btn_비상버튼.gameObject.SetActive(true);
            }
            else if(idx == 1)
            {
                if (!go_EmergencySwitchPopup.activeInHierarchy)
                {
                    go_EmergencySwitchPopup.SetActive(true);
                }

                EmergencySwitch es = go_EmergencySwitchPopup.GetComponent<EmergencySwitch>();
                es.infoIdx = 0;
                go_EmergencySwitchPopup.GetComponent<EmergencySwitch>().DashBoardChange();
                audio_MultiTester.Stop();
            }
            else if(idx == 2)
            {
                EmergencySwitch es = go_EmergencySwitchPopup.GetComponent<EmergencySwitch>();
                es.infoIdx = 1;
                go_EmergencySwitchPopup.GetComponent<EmergencySwitch>().DashBoardChange();

                if (!go_EmergencySwitchPopup.activeInHierarchy)
                {
                    go_EmergencySwitchPopup.SetActive(true);
                }
                audio_MultiTester.Play();
            }
            else if(idx == 3)
            {
                audio_MultiTester.Stop();
                go_EmergencySwitchPopup.SetActive(false);
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
                CheckListShowHide(true);
            }
        }

        private void CheckListImageShowHide(bool ison)
        {
            if (go_CheckListImage == null) return;
            go_CheckListImage.SetActive(ison);
        }

        private void CheckListShowHide(bool ison)
        {
            if (go_CheckList == null) return;
            go_CheckList.SetActive(ison);
        }
    }
}