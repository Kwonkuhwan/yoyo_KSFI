using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class EmergencySwitch : MonoBehaviour
    {
        //비상스위치 관련
        [SerializeField] private Button btn_EmergencySwitch;
        [SerializeField] private Image image_EmergencySwitch;
        [SerializeField] private Sprite[] sprites_EmergencySwitch;

        // 동작 문구
        [SerializeField] private TMP_Text text_Activeted;
        [SerializeField] private string[] strs_Activeted;

        // 내용
        [SerializeField] private TMP_Text text_Info;
        [SerializeField] private Image image_Dashboard;
        [SerializeField] private Sprite[] sprites_Dashboard;

        //손
        [SerializeField] private Image image_Hand;
        [SerializeField] private Sprite[] sprites_Hands;

        //완료버튼
        //[SerializeField] private Button btn_Done;

        public InfoDataScriptableObject infoData;

        public int infoIdx = 0;

        [SerializeField] private InfoMenu infoMenu;

        private void Awake()
        {
            infoIdx = 0;
            btn_EmergencySwitch.onClick.AddListener(() => EmergencySwitchBtnClick());
            //btn_Done.onClick.AddListener(() => DoneBtnClick());
        }

        private void OnDisable()
        {
            infoIdx = 0;
        }

        private void OnEnable()
        {            
            DashBoardChange();
            text_Info.text = infoData.str_infodatas[infoIdx];
            //if (infoIdx == 0)
            //{
            //    btn_Done.gameObject.SetActive(false);
            //}
            //else
            //{
            //    btn_Done.gameObject.SetActive(true);
            //}
        }

        private void EmergencySwitchBtnClick()
        {
            if (infoIdx >= infoData.str_infodatas.Length - 1)
            {
                return;
            }

            infoIdx++;
            text_Info.text = infoData.str_infodatas[infoIdx];
            DashBoardChange();

            //if (infoIdx == 0)
            //{
            //    btn_Done.gameObject.SetActive(false);
            //}
            //else
            //{
            //    btn_Done.gameObject.SetActive(true);
            //}

            infoMenu.NextIndex();
        }

        public void DashBoardChange()
        {
            if (sprites_Dashboard == null || sprites_Dashboard.Length <= 0) return;
            image_Dashboard.sprite = sprites_Dashboard[infoIdx];

            if (sprites_Hands == null || sprites_Hands.Length <= 0) return;
            image_Hand.sprite = sprites_Hands[infoIdx];
            image_Hand.SetNativeSize();

            if (strs_Activeted == null || strs_Activeted.Length <= 0) return;
            text_Activeted.text = strs_Activeted[infoIdx];

            if (sprites_EmergencySwitch == null || sprites_EmergencySwitch.Length <= 0) return;
            image_EmergencySwitch.sprite = sprites_EmergencySwitch[infoIdx];
        }

        private void DoneBtnClick()
        {
            AutomaticClosingDeviceInfoMenu.Inst.NextBtnClick();
            gameObject.SetActive(false);
        }
    }
}
