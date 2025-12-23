using UnityEngine;
using UnityEngine.UI;

namespace RJH.Transporter
{
    public class SectionManager : MonoBehaviour
    {
        [SerializeField] private int bgImageNumber; // 현재 섹션의 배경 이미지
        [SerializeField] private int lastDocNumber; // 마지막 Doc 번호(이 번호에서 넘어가면 다음 페이지로 이동)
        [SerializeField] private GameObject nextPage; // 다음 페이지
        [SerializeField] private Button nextButton; // Doc 넘기는 버튼
        [SerializeField] private int setDocumentNumber = -1;
        [SerializeField] private int popOutPageNumber = 0;
        [SerializeField] private GameObject popup;
        [SerializeField] private GameObject marker;
        private void OnEnable()
        {
            if (popup != null)
            {
                popup.SetActive(true);
            }
            SectionAndBackGroundManager.Instance.SetBackGroundAndSave(bgImageNumber);
            SectionAndBackGroundManager.Instance.SetSection(this.gameObject);
            //nextButton.onClick.AddListener(MoveNextPage);
            SectionAndBackGroundManager.Instance.sectionAction += MoveNextPage;
        }

        private void MoveNextPage(int docNum)
        {
            if(docNum != lastDocNumber + 1)
                return;

            PopupManager.Instance.PopupClose();

            this.gameObject.SetActive(false);
            nextPage.SetActive(true);
            if (setDocumentNumber != -1)
            {
                if (marker != null)
                {
                    marker.SetActive(true);
                }
                SectionAndBackGroundManager.Instance.PopStack(popOutPageNumber);
                SectionAndBackGroundManager.Instance.SetDocument(setDocumentNumber);
                if (popup != null)
                {
                    popup.SetActive(false);
                }
            }
            SectionAndBackGroundManager.Instance.sectionAction -= MoveNextPage;
        }
        private void OnDisable()
        {
            SectionAndBackGroundManager.Instance.sectionAction -= MoveNextPage;
        }

    }

}


