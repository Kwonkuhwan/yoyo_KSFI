using UnityEngine;
using UnityEngine.UI;
using VInspector;

namespace KKH
{
    public class DocumentPopup : MonoBehaviour
    {
        private static DocumentPopup instance;
        public static DocumentPopup Inst => instance;

        [SerializeField] private InfoMenu infoMenu;

        [Foldout("토글 관련")]
        [SerializeField] private ToggleControl[] toggleControls;

        [Foldout("버튼 관련")]
        [SerializeField] private Button[] buttons;
        public Button btn_Compelet;

        [Foldout("팝업 관련")]
        [SerializeField] private GameObject[] popups;

        private void Awake()
        {
            instance = this;

            btn_Compelet.onClick.AddListener(delegate
            {
                gameObject.SetActive(false);
                CanvasControl.Inst.panel_RegularInspection.SetActive(true);
                CanvasControl.Inst.panel_Document.SetActive(false);
            });

            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }

            buttons[0].onClick.AddListener(delegate
            {
                infoMenu.infoIdx = 1;
            });
            buttons[1].onClick.AddListener(delegate
            {
                infoMenu.infoIdx = 3;
            });
            buttons[2].onClick.AddListener(delegate
            {
                infoMenu.infoIdx = 5;
            });
            buttons[3].onClick.AddListener(delegate
            {
                infoMenu.infoIdx = 8;
            });
            buttons[4].onClick.AddListener(delegate
            {
                infoMenu.infoIdx = 10;
            });
        }

        private void OnEnable()
        {
            AllHidePopup();

            if (CanvasControl.Inst.isSelectMode)
            {
                foreach (ToggleControl toggle in toggleControls)
                {
                    toggle.isOn = false;
                }
            }

            btn_Compelet.GetComponent<ButtonControl_KKH>().interactable = false;
        }

        private void Start()
        {
            for (int idx = 0; idx < buttons.Length; idx++)
            {
                int i = idx;
                buttons[i].onClick.AddListener(delegate
                {
                    bool ischeck = false;
                    foreach (var popup in popups)
                    {
                        if (popup.activeInHierarchy)
                        {
                            ischeck = true;
                            break;
                        }
                    }

                    ButtonManager_KKH btnMng = buttons[i].GetComponent<ButtonManager_KKH>();
                    if (btnMng == null)
                    {
                        if (ischeck)
                        {
                            InsideTankLorryInfoMenu.Inst.NextBtnClick();
                        }
                        else
                        {
                            toggleControls[i].isOn = true;
                            //ShowPopUp(i);
                            InsideTankLorryInfoMenu.Inst.NextIndex();
                        }
                    }
                    else
                    {
                        if (ischeck || btnMng.isCompelet)
                        {
                            InsideTankLorryInfoMenu.Inst.NextBtnClick();
                        }
                        else
                        {
                            toggleControls[i].isOn = true;
                            //ShowPopUp(i);
                            InsideTankLorryInfoMenu.Inst.NextIndex();
                        }
                    }
                });
                if (popups.Length <= idx) break;
            }
        }

        private void Update()
        {
            if (toggleControls != null && toggleControls.Length > 0)
            {
                bool check = true;
                foreach (var toc in toggleControls)
                {
                    if (!toc.isOn)
                    {
                        check = false;
                    };
                }

                if (!check)
                {
                    btn_Compelet.GetComponent<ButtonControl_KKH>().interactable = false;
                }
            }
        }

        public void AllHidePopup()
        {
            foreach (GameObject popup in popups)
            {
                popup.SetActive(false);
            }
        }

        public void ShowPopUp(int idx)
        {
            AllHidePopup();
            popups[idx].SetActive(true);
        }

        public void ButtonEnable(int idx)
        {
            if (idx < buttons.Length)
            {
                buttons[idx].interactable = true;
            }
        }
    }
}