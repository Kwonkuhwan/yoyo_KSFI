using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class SafetyBeforeTransportation : MonoBehaviour
    {
        [SerializeField] private GameObject[] panels;
        [SerializeField] private InfoMenu[] infoMenus;

        [SerializeField] protected Button btn_Mute;
        [SerializeField] protected Sprite[] sprites_Mute;

        private void Awake()
        {
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
            if (CanvasControl.Inst != null && CanvasControl.Inst.isSelectMode)
            {
                foreach (var info in infoMenus)
                {
                    info.infoIdx = 0;
                }

                if (panels != null && panels.Length > 0)
                {
                    AllHidePanels();
                    panels[0]?.SetActive(true);
                }
            }

            if (CanvasControl.Inst != null)
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

        private void AllHidePanels()
        {
            foreach (var obj in panels)
            {
                try
                {
                    obj.SetActive(false);
                }
                catch
                {
                    return;
                }
            }
        }
    }
}