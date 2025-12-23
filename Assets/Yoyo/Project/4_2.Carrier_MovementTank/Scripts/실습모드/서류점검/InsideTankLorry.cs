using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class InsideTankLorry : MonoBehaviour
    {
        private static InsideTankLorry instance;
        public static InsideTankLorry Inst => instance;

        [SerializeField] private Button btn_Document;

        public GameObject documentBtnObj => btn_Document.gameObject;
        [SerializeField] private GameObject[] Popups;
        [SerializeField] private GameObject Popup_Document;

        [SerializeField] protected Button btn_Mute;
        [SerializeField] protected Sprite[] sprites_Mute;

        private void Awake()
        {
            instance = this;

            if (btn_Document != null)
            {
                btn_Document.onClick.AddListener(delegate
                {
                    InsideTankLorryInfoMenu.Inst.NextIndex();
                    //DocumentBtnClick();
                });
            }

            if (btn_Mute != null)
            {
                btn_Mute.onClick.AddListener(delegate
                {
                    CanvasControl.Inst.AudioMute();
                    MuteImageChange();
                });
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

        private void OnEnable()
        {
            AllHidePopups();
            if (CanvasControl.Inst != null && CanvasControl.Inst.isSelectMode)
            {
                if (!btn_Document.gameObject.activeInHierarchy) btn_Document.gameObject.SetActive(true);
            }

            if (CanvasControl.Inst != null)
            {
                MuteImageChange();
            }
        }

        private void OnDisable()
        {
        }

        private void AllHidePopups()
        {
            foreach (var obj in Popups)
            {
                obj.SetActive(false);
            }
        }

        public void DocumentBtnClick()
        {
            btn_Document.gameObject.SetActive(false);
            Popup_Document.SetActive(true);
        }
    }
}