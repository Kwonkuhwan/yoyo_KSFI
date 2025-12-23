using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace RJH.Transporter
{
    public class PopupManager : MonoBehaviour
    {
        #region singleton
        private static PopupManager instance;
        public static PopupManager Instance { get { return instance; } }
        #endregion

        [Header("Popup")]
        [SerializeField] private GameObject popupObject;
        [SerializeField] private RectTransform popupImageRect;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI descriptionText_side;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image exampleImage1;
        [SerializeField] private VerticalLayoutGroup verticalLayout;
        [SerializeField] private int popupNumber = -1;
        [SerializeField] private Sprite[] popupSprites; // 0:normal, 1:middle, 2: small
        [Space(10)]
        [SerializeField] private TransporterPopupData transporterPopup;
        [SerializeField] private Button confirmButton;

        public bool isExchangeBackGround = false;

        public void Awake()
        {
            instance = this;
            confirmButton.onClick.AddListener(PopupClose);
        }

        public void NormalSetPopup(int number)
        {
            Debug.Log("number : " + number);
            if (number == -1)
                return;
            popupNumber = number;
            TransporterPopup popup = transporterPopup.transporterPopups[number];

            backgroundImage.sprite = popupSprites[popup.popupBgNumber];
            backgroundImage.SetNativeSize();

            titleText.text = popup.titleText;
            if (popup.sideFormat)
            {
                verticalLayout.childAlignment = TextAnchor.UpperLeft;
                descriptionText.text = "";
                descriptionText_side.text = popup.descriptionText.Replace("\r", "");
            }
            else
            {
                verticalLayout.childAlignment = TextAnchor.UpperCenter;
                descriptionText.text = popup.descriptionText.Replace("\r", "");
                descriptionText_side.text = "";
            }

            if(popup.setImageMiddle)
            {
                popupImageRect.anchoredPosition = new Vector2(0, -150);
            }
            else
            {
                popupImageRect.anchoredPosition = new Vector2(0, -70);
            }

            if (string.IsNullOrEmpty(popup.documentTitleText) == false)
            {
                SectionAndBackGroundManager.Instance.SetDocument_text(popup.documentTitleText, popup.documentDescriptionText);
            }

            exampleImage1.sprite = popup.exampleImage_1;
            exampleImage1.SetNativeSize();
            popupObject.SetActive(true);

            switch (popup.buttonPosition)
            {
                case ButtonPosition.None:
                    confirmButton.gameObject.SetActive(false);
                    break;
                case ButtonPosition.Middle:
                    confirmButton.gameObject.SetActive(true);
                    confirmButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 72f);
                    break;
                case ButtonPosition.Right:
                    confirmButton.gameObject.SetActive(true);
                    confirmButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(287.5f, 72f);
                    break;
            }
            AudioManager.Instance.PlayDocs(popup.audioClip);
        }

        public void PopupClose()
        {
            popupObject.SetActive(false);
            SectionAndBackGroundManager.Instance.SetPopupData(-1);
            SectionAndBackGroundManager.Instance.SetSectionOnOff(true);
            if (isExchangeBackGround)
            {
                SectionAndBackGroundManager.Instance.SetBackGround();
                isExchangeBackGround = false;
            }
        }

        public void SetBackGround()
        {
            isExchangeBackGround = true;
        }
    }
}


