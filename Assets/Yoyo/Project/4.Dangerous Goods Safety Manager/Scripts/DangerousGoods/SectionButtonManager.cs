using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RJH.DangerousGoods
{
    public class SectionButtonManager : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private DocsButton docsButton;
        [SerializeField] private int nextSectionIndex; // 다음 섹션의 번호
        [SerializeField] private GameObject nextSection; // 다름 섹션
        [SerializeField] private bool resetSection;
        [SerializeField] private bool dontresetPage;
        [SerializeField] private bool isParentsButton; // 체크해야 하는 자식 버튼이 있는가?
        [SerializeField] private bool dontSaveSectionStack;
        [SerializeField] private GameObject checkImage; // 확인할 사항을 모두 확인했을때 활성화 
        [SerializeField] private Button[] childButtons; // 체크해야 하는 자식 버튼들


        [SerializeField] private int docNumber = 0;

        private List<Button> checkButtonList = new List<Button>();
        private int checkCount = 0;

        private void Awake()
        {
            if(docsButton != null)
            {
                docsButton.docsAction += MoveNextSection;
            }
            else
            {
                button = GetComponent<Button>();
                button.onClick.AddListener(MoveNextSection);
            }

        }

        private void MoveNextSection()
        {
            SectionAndBackGroundManager.Instance.SetDocument(docNumber);
            SectionAndBackGroundManager.Instance.MoveNextSection(nextSection, nextSectionIndex, resetSection, dontresetPage, dontSaveSectionStack);

            if (isParentsButton)
            {
                SectionAndBackGroundManager.Instance.SetSectionButton(this);
            }
            
        }

        public void SetOnMarkerImage()
        {
            if (!isParentsButton)
                return;

            checkImage.SetActive(true);

        }

        private void OnEnable()
        {
            if (checkImage != null && !checkImage.activeSelf)
            {
                foreach(var button in childButtons)
                {
                    button.transform.GetComponent<PopupButtonManager>().SetMarkerImageOff();
                }
            }

        }
       
    }
}


