using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace RJH.DangerousGoods
{
    public class PopupManager : MonoBehaviour
    {
        #region singleton
        private static PopupManager instance;
        public static PopupManager Inastance { get { return instance; } }
        #endregion

        [Header("Popup")]
        [SerializeField] private GameObject popupObject;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image exampleImage1;
        [SerializeField] private Image exampleImage2;
        [SerializeField] private Image exampleImage3;
        [SerializeField] private int popupNumber = -1;
        [SerializeField] private Sprite[] popupSprites; // 0:normal, 1:middle, 2: small
        

        [Space(10)]
        [SerializeField] private PopupScriptableObject scriptableObject;

        [SerializeField] private Button confirmButton;
        [SerializeField] private Button subPopupButton;
        [SerializeField] private ResistanceMeasurementPopup resistanceMeasurementPopup;
        public bool isExchangeBackGround = false;

        public void Awake()
        {
            instance = this;
            confirmButton.onClick.AddListener(PopupClose);
            subPopupButton.onClick.AddListener(OpenSubPopup);
        }

        public void NormalSetPopup(int number)
        {
            Debug.Log("number : " + number);
            if (number == -1)
                return;
            popupNumber = number;
            Popup popup = scriptableObject.popups[number];

            if (popup.isbackGroundChange)
            {
                SectionAndBackGroundManager.Instance.SetBackGround(popup.backGroundNumber);
                SetBackGround();
            }

            backgroundImage.sprite = popupSprites[popup.popupBgnumber];
            backgroundImage.SetNativeSize();
            if(popup.popupBgnumber == 2 || popup.popupBgnumber == 1)
            {
                subPopupButton.gameObject.SetActive(false);
                confirmButton.gameObject.SetActive(false);
            }
            else
            {
                if (number == 13)
                {
                    confirmButton.gameObject.SetActive(false);
                    subPopupButton.gameObject.SetActive(true);
                }
                else
                {
                    confirmButton.gameObject.SetActive(true);
                    subPopupButton.gameObject.SetActive(false);
                }
            }

            titleText.text = popup.titile;
            descriptionText.text = popup.description.Replace("\r","");

            if(popup.isSingleExample)
            {
                exampleImage2.gameObject.SetActive(false);
                exampleImage3.gameObject.SetActive(false);
                exampleImage1.sprite = popup.exampleOne;
                exampleImage1.SetNativeSize();
            }
            else
            {
                exampleImage2.gameObject.SetActive(true);
                exampleImage1.sprite = popup.exampleOne;
                exampleImage2.sprite = popup.exampleTwo;
                exampleImage1.SetNativeSize();
                exampleImage2.SetNativeSize();
                if(popup.exampleThree != null)
                {
                    exampleImage3.gameObject.SetActive(true);
                    exampleImage3.sprite = popup.exampleThree;
                    exampleImage3.SetNativeSize();
                }
                else
                {
                    exampleImage3.gameObject.SetActive(false);
                }
            }
            

            popupObject.SetActive(true);

        }

        private void OpenSubPopup()
        {
            PopupClose();
            resistanceMeasurementPopup.PopupStart();
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


