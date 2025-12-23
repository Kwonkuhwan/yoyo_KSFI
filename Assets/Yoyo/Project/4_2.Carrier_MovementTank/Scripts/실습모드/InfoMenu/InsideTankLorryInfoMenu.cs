using TMPro;
using UnityEngine;
using VInspector;
using UnityEngine.UI;

namespace KKH
{
    public class InsideTankLorryInfoMenu : InfoMenu
    {
        private static InsideTankLorryInfoMenu instance;
        public static InsideTankLorryInfoMenu Inst => instance;

        [Foldout("클래스")]
        [SerializeField] private InsideTankLorry insideTankLorry;
        [SerializeField] private DocumentPopup documentPopup;

        [Foldout("판넬")]
        [SerializeField] private GameObject[] panels;

        [Foldout("토클")]
        [SerializeField] private ToggleControl[] toggleControls;

        [Foldout("버튼매니저")]
        [SerializeField] private ButtonManager_KKH[] buttonManagers;

        [Foldout("정기점검표 텍스트")]
        [SerializeField] private TMP_Text[] text_Infos;

        [Foldout("버튼")]
        [SerializeField] private Button[] buttons;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            instance = this;

            if (InsideTankLorry.Inst == null || CanvasControl.Inst.isSelectMode)
            {
                infoIdx = 0;
                AudioStart();
            }

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
            //base.NextBtnClick();
            if (CheckHighLight()) return;
            NextIndex();
            PageScroll(infoIdx);
        }

        private void PageScroll(int idx)
        {
            if (!documentPopup.gameObject.activeInHierarchy)
            {
                documentPopup.gameObject.SetActive(true);
            }

            isEnd = false;

            if (idx == 0)
            {
                documentPopup.AllHidePopup();
                documentPopup.gameObject.SetActive(false);
                insideTankLorry.documentBtnObj.SetActive(true);
                AllPanelsHide();
            }
            else if (idx == 1)
            {
                AllPanelsHide();

                insideTankLorry.documentBtnObj.SetActive(false);
                documentPopup.gameObject.SetActive(true);

                toggleControls[0].isOn = false;
                buttonManagers[0].isCompelet = false;
            }
            else if (idx == 2)  // 위험물 운송자 자격 완료 처리,  실무 교육 버튼 비활성화
            {
                AllPanelsHide();
                panels[0].SetActive(true);

                toggleControls[0].isOn = true;

                buttonManagers[0].isCompelet = true;
                buttonManagers[1].isEnable = false;
                buttonManagers[1].GetComponent<Button>().interactable = false;
            }
            else if (idx == 3)  // 실무 교육 버튼 활성화
            {
                AllPanelsHide();

                buttonManagers[1].GetComponent<Button>().interactable = true;
                buttonManagers[1].isEnable = true;
                buttonManagers[1].isCompelet = false;

                toggleControls[1].isOn = false;
            }
            else if (idx == 4)  // 실무 교육 완료 처리, 위험물 안전 카드 비활성화
            {
                AllPanelsHide();
                panels[1].SetActive(true);

                toggleControls[1].isOn = true;

                buttonManagers[1].isCompelet = true;
                buttonManagers[2].isEnable = false;
                buttonManagers[2].GetComponent<Button>().interactable = false;
            }
            else if (idx == 5)  // 위험물 안전 카드 버튼 활성화
            {
                AllPanelsHide();

                toggleControls[2].isOn = false;

                buttonManagers[2].GetComponent<Button>().interactable = true;
                buttonManagers[2].isEnable = true;
                buttonManagers[2].isCompelet = false;
            }
            else if (idx == 6)  // 위험물 안전 카드 완료 처리, 완공검사 합격 확인증 비활성화
            {
                AllPanelsHide();

                panels[2].SetActive(true);
            }
            else if (idx == 7)
            {
                AllPanelsHide();

                panels[3].SetActive(true);
                toggleControls[2].isOn = true;
                buttonManagers[2].isCompelet = true;
                buttonManagers[3].isEnable = false;
                buttonManagers[3].GetComponent<Button>().interactable = false;
            }
            else if (idx == 8)  // 완공검사 합격 확인증 활성화
            {
                AllPanelsHide();

                buttonManagers[3].GetComponent<Button>().interactable = true;
                buttonManagers[3].isEnable = true;
                buttonManagers[3].isCompelet = false;

                toggleControls[3].isOn = false;
                //buttonManagers[4].isEnable = false;
            }
            else if (idx == 9) // 완공 검사 합격 확인증 완료 처리, 정기점검 버튼 활성화
            {
                AllPanelsHide();

                panels[4].SetActive(true);

                toggleControls[3].isOn = true;

                buttonManagers[3].isCompelet = true;
                buttonManagers[4].isEnable = false;
                buttonManagers[4].GetComponent<Button>().interactable = false;
            }
            else if (idx == 10) // 정기점검 완료처리
            {
                AllPanelsHide();

                buttonManagers[4].GetComponent<Button>().interactable = true;
                buttonManagers[4].isEnable = true;
                buttonManagers[4].isCompelet = false;

                toggleControls[4].isOn = false;

                foreach (var btn in buttons)
                {
                    ButtonManager_KKH buttonMng = btn.GetComponent<ButtonManager_KKH>();
                    buttonMng.isCompelet = false;
                    buttonMng.isEnable = true;
                    btn.interactable = true;
                }

                foreach (var text in text_Infos)
                {
                    text.gameObject.SetActive(false);
                }
            }
            else if (idx == 11)
            {
                AllPanelsHide();

                panels[5].SetActive(true);
                toggleControls[4].isOn = true;

                for (int i = 0; i < buttons.Length; i++)
                {
                    int j = i;
                    if (text_Infos[j].gameObject.activeInHierarchy)
                    {
                        buttons[j].GetComponent<ButtonManager_KKH>().isCompelet = true;
                    }
                }
                buttonManagers[4].isCompelet = true;
            }
            else if (idx == 12)
            {
                AllPanelsHide();
                documentPopup.btn_Compelet.GetComponent<ButtonControl_KKH>().interactable = true;
                isEnd = true;
            }

            #region 20241213 삭제
            //else if (idx == 11)
            //{
            //    AllPanelsHide();
            //    toggleControls[4].isOn = true;
            //    panels[5].SetActive(true);

            //    text_Infos[0].gameObject.SetActive(false);
            //    text_Infos[1].gameObject.SetActive(false);
            //    text_Infos[2].gameObject.SetActive(false);
            //    text_Infos[3].gameObject.SetActive(false);

            //    buttons[0].GetComponent<ButtonManager_KKH>().isCompelet = false;
            //    buttons[1].GetComponent<ButtonManager_KKH>().isCompelet = false;
            //    buttons[2].GetComponent<ButtonManager_KKH>().isCompelet = false;
            //    buttons[3].GetComponent<ButtonManager_KKH>().isCompelet = false;

            //    buttons[0].GetComponent<ButtonManager_KKH>().isEnable = true;
            //    buttons[1].GetComponent<ButtonManager_KKH>().isEnable = false;
            //    buttons[2].GetComponent<ButtonManager_KKH>().isEnable = false;
            //    buttons[3].GetComponent<ButtonManager_KKH>().isEnable = false;

            //    buttons[0].interactable = true;
            //    buttons[1].interactable = false;
            //    buttons[2].interactable = false;
            //    buttons[3].interactable = false;
            //}
            //else if (idx == 12)
            //{
            //    text_Infos[0].gameObject.SetActive(true);
            //    text_Infos[1].gameObject.SetActive(false);
            //    text_Infos[2].gameObject.SetActive(false);
            //    text_Infos[3].gameObject.SetActive(false);

            //    buttons[0].GetComponent<ButtonManager_KKH>().isCompelet = true;
            //    buttons[1].GetComponent<ButtonManager_KKH>().isCompelet = false;
            //    buttons[2].GetComponent<ButtonManager_KKH>().isCompelet = false;
            //    buttons[3].GetComponent<ButtonManager_KKH>().isCompelet = false;

            //    buttons[0].GetComponent<ButtonManager_KKH>().isEnable = true;
            //    buttons[1].GetComponent<ButtonManager_KKH>().isEnable = true;
            //    buttons[2].GetComponent<ButtonManager_KKH>().isEnable = false;
            //    buttons[3].GetComponent<ButtonManager_KKH>().isEnable = false;

            //    buttons[0].interactable = true;
            //    buttons[1].interactable = true;
            //    buttons[2].interactable = false;
            //    buttons[3].interactable = false;
            //}
            //else if (idx == 13)
            //{
            //    text_Infos[0].gameObject.SetActive(true);
            //    text_Infos[1].gameObject.SetActive(true);
            //    text_Infos[2].gameObject.SetActive(false);
            //    text_Infos[3].gameObject.SetActive(false);

            //    buttons[0].GetComponent<ButtonManager_KKH>().isCompelet = true;
            //    buttons[1].GetComponent<ButtonManager_KKH>().isCompelet = true;
            //    buttons[2].GetComponent<ButtonManager_KKH>().isCompelet = false;
            //    buttons[3].GetComponent<ButtonManager_KKH>().isCompelet = false;

            //    buttons[0].GetComponent<ButtonManager_KKH>().isEnable = true;
            //    buttons[1].GetComponent<ButtonManager_KKH>().isEnable = true;
            //    buttons[2].GetComponent<ButtonManager_KKH>().isEnable = true;
            //    buttons[3].GetComponent<ButtonManager_KKH>().isEnable = false;

            //    buttons[0].interactable = true;
            //    buttons[1].interactable = true;
            //    buttons[2].interactable = true;
            //    buttons[3].interactable = false;
            //}
            //else if (idx == 14)
            //{
            //    text_Infos[0].gameObject.SetActive(true);
            //    text_Infos[1].gameObject.SetActive(true);
            //    text_Infos[2].gameObject.SetActive(true);
            //    text_Infos[3].gameObject.SetActive(false);

            //    buttons[0].GetComponent<ButtonManager_KKH>().isCompelet = true;
            //    buttons[1].GetComponent<ButtonManager_KKH>().isCompelet = true;
            //    buttons[2].GetComponent<ButtonManager_KKH>().isCompelet = true;
            //    buttons[3].GetComponent<ButtonManager_KKH>().isCompelet = false;

            //    buttons[0].GetComponent<ButtonManager_KKH>().isEnable = true;
            //    buttons[1].GetComponent<ButtonManager_KKH>().isEnable = true;
            //    buttons[2].GetComponent<ButtonManager_KKH>().isEnable = true;
            //    buttons[3].GetComponent<ButtonManager_KKH>().isEnable = true;

            //    buttons[0].interactable = true;
            //    buttons[1].interactable = true;
            //    buttons[2].interactable = true;
            //    buttons[3].interactable = true;
            //}
            //else if (idx == 15)
            //{
            //    panels[5].SetActive(true);
            //    text_Infos[0].gameObject.SetActive(true);
            //    text_Infos[1].gameObject.SetActive(true);
            //    text_Infos[2].gameObject.SetActive(true);
            //    text_Infos[3].gameObject.SetActive(true);
            //}
            //else if(idx == 16)
            //{
            //    AllPanelsHide();
            //    documentPopup.btn_Compelet.GetComponent<ButtonControl_KKH>().interactable = true;
            //}
            #endregion
        }

        private void AllPanelsHide(GameObject go = null)
        {
            foreach (var panel in panels)
            {
                if (go != null && panel == go)
                {
                    continue;
                }
                panel.gameObject.SetActive(false);
            }
        }
    }
}