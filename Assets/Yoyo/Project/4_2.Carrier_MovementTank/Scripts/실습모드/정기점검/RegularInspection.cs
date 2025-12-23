
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

namespace KKH
{
    public enum RegularInspectionType
    {
        탱크본체 = 0,
        맨홀,
        주입구,
        접지도선,
        표시표지,
        소화기,
        자동폐쇄장치
    }

    public class RegularInspection : MonoBehaviour
    {
        private static RegularInspection instance;
        public static RegularInspection Inst => instance;

        [Foldout("버튼")]
        [SerializeField] private GameObject buttonParentObj;
        public GameObject ButtonParent => buttonParentObj;
        [SerializeField] private Button[] buttons;

        public Button btn_Prev;
        public Button btn_Next;

        [Foldout("텍스트")]
        [SerializeField] private TMP_Text text_info;
        [SerializeField] private string[] strs_info;

        [Foldout("버튼 이미지")]
        public Sprite[] sprites_Defualt;
        public Sprite[] sprites_Done;

        [Foldout("팝업")]
        [SerializeField] private GameObject[] popups;

        [Foldout("완료 목록")]
        [SerializeField] private List<bool> isDones = new List<bool>();

        [Foldout("오디오")]
        public bool isCheck = false;
        [SerializeField] private AudioClip[] audioClips;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] protected Button btn_Mute;
        [SerializeField] protected Sprite[] sprites_Mute;

        private void Awake()
        {
            instance = this;

            buttons = buttonParentObj.GetComponentsInChildren<Button>();

            btn_Next.onClick.AddListener(delegate
            {
                CanvasControl.Inst.panel_Safety_Measures.SetActive(true);
                CanvasControl.Inst.panel_RegularInspection.SetActive(false);
            });

            btn_Prev.onClick.AddListener(delegate
            {
                if (CanvasControl.Inst.isSelectMode)
                {
                    CanvasControl.Inst.panel_Selectmode.SetActive(true);
                }
                else
                {
                    CanvasControl.Inst.panel_Document.SetActive(true);
                }
                CanvasControl.Inst.panel_RegularInspection.SetActive(false);
            });

            if (btn_Mute != null)
            {
                btn_Mute.onClick.AddListener(delegate
                {
                    CanvasControl.Inst.AudioMute();
                    MuteImageChange();
                });
            }
        }

        private void OnEnable()
        {
            isCheck = false;
            if (audioSource != null)
            {
                audioSource.clip = audioClips[0];
                audioSource.Play();
            }

            AllHidePopup();
            if (!buttonParentObj.activeInHierarchy)
            {
                buttonParentObj.SetActive(true);
            }

            if (CanvasControl.Inst != null && CanvasControl.Inst.isSelectMode)
            {
                text_info.text = strs_info[0];
                isDones.Clear();

                for (int idx = 0; idx < buttons.Length; idx++)
                {
                    int i = idx;
                    buttons[i].GetComponent<Image>().sprite = sprites_Defualt[i];
                    isDones.Add(false);
                }

                btn_Next.gameObject.SetActive(false);
            }

            if(CanvasControl.Inst != null)
            {
                MuteImageChange();
            }
        }

        private void MuteImageChange()
        {
            if (CanvasControl.Inst.isAudioMute)
            {
                btn_Mute.GetComponent<Image>().sprite = sprites_Mute[1];
            }
            else
            {
                btn_Mute.GetComponent<Image>().sprite = sprites_Mute[0];
            }
        }

        private void OnDisable()
        {
        }

        private void Start()
        {
            for (int idx = 0; idx < buttons.Length; idx++)
            {
                int i = idx;
                buttons[i].onClick.AddListener(delegate
                {
                    popups[i].GetComponentInChildren<InfoMenu>().infoIdx = 0;
                    buttonParentObj.SetActive(false);
                    ShowPopUp(i);
                    audioSource.Stop();
                });
                if (popups.Length <= idx) break;
            }
        }

        private void Update()
        {
            if (btn_Next.gameObject.activeInHierarchy) return;

            foreach (var obj in isDones)
            {
                if (!obj)
                {
                    return;
                }
            }


            if (!isCheck && !btn_Next.gameObject.activeInHierarchy)
            {
                isCheck = true;
                text_info.text = strs_info[1];
                //btn_Prev.gameObject.SetActive(false);
                btn_Next.gameObject.SetActive(true);

                if (audioSource != null)
                {
                    audioSource.clip = audioClips[1];
                    audioSource.Play();
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

        public void SetBtnImage(RegularInspectionType rit)
        {
            buttons[(int)rit].image.sprite = sprites_Done[(int)rit];
            isDones[(int)rit] = true;
        }
    }
}
