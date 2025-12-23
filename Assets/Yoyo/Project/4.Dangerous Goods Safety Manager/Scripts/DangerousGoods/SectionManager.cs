using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RJH.DangerousGoods
{
    public class SectionManager : MonoBehaviour
    {
        [SerializeField] private int bgImageNumber; // 현재 섹션의 배경 이미지
        [SerializeField] private int lastDocNumber; // 마지막 Doc 번호(이 번호에서 넘어가면 다음 페이지로 이동)
        [SerializeField] private GameObject nextPage; // 다음 페이지
        [SerializeField] private Button nextButton; // Doc 넘기는 버튼
        
        private void OnEnable()
        {
            SectionAndBackGroundManager.Instance.SetBackGroundAndSave(bgImageNumber);
            SectionAndBackGroundManager.Instance.SetSection(this.gameObject);
            //nextButton.onClick.AddListener(MoveNextPage);
            SectionAndBackGroundManager.Instance.sectionAction += MoveNextPage;
        }

        private void MoveNextPage(int docNum)
        {
            if(docNum != lastDocNumber + 1)
                return;

            PopupManager.Inastance.PopupClose();

            this.gameObject.SetActive(false);
            nextPage.SetActive(true);
            
            SectionAndBackGroundManager.Instance.sectionAction -= MoveNextPage;

        }
        private void OnDisable()
        {
            SectionAndBackGroundManager.Instance.sectionAction -= MoveNextPage;
        }

    }

}


