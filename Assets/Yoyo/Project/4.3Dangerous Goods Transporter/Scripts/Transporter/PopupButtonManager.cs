using UnityEngine;

namespace RJH.Transporter
{
    public class PopupButtonManager : MonoBehaviour
    {
        [SerializeField] private int popupNumber;
        [SerializeField] private int backGroundNumber;
        [SerializeField] private bool isneedSectionOpen = false;
        [SerializeField] private GameObject markerImage;
        private DocsButton button;

        private void Awake()
        {
            button = GetComponent<DocsButton>();
            if (button == null)
            {
                Debug.Log("DocsButton이 없습니다.");
            }
            else
            {
                button.docsAction += ShowPopup;
            }

            //button.onClick.AddListener(ShowPopup);
        }

        public void ShowPopup()
        {

            PopupManager.Instance.NormalSetPopup(popupNumber);


            SectionAndBackGroundManager.Instance.SetPopupData(popupNumber);
            SectionAndBackGroundManager.Instance.SetSectionOnOff(isneedSectionOpen);

            Debug.Log("켜짐");
            if (markerImage != null)
                markerImage?.SetActive(true);
        }
        public void SetMarkerImageOff()
        {
            markerImage.SetActive(false);
        }
    }
}

