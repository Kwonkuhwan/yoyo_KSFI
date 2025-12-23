using UnityEngine;
using UnityEngine.UI;

namespace RJH.DangerousGoods
{
    public class DocumentInspection : MonoBehaviour
    {
        [SerializeField] private DocsButton documentButton;
        [Space(10)]
        [SerializeField] private GameObject documentCheckList;
        [SerializeField] private Button closeDocuementButton;
        [SerializeField] private Button checkCompleteButton;
        [SerializeField] private DocsButton[] buttonGroup;
        [SerializeField] private DocumentCheck documentCheck;
        [SerializeField] private SubDocumentCheck subDocumentCheck;
        [SerializeField] private int checkNumber = 0;
        private void Awake()
        {
            checkNumber = 0;
            //checkCompleteButton.GetComponent<ButtonController>().SetInteractable(false);
            for (int i = 0; i < buttonGroup.Length; i++)
            {
                int num = i;

                //buttonGroup[i].onClick.AddListener(() => { CheckOn(num); });
                buttonGroup[i].docsAction += () => CheckOn(num);
            }

            documentButton.docsAction += ShowDocument;
            closeDocuementButton.onClick.AddListener(HideDocuement);
        }

        private void CheckOn(int num)
        {
            //if (!buttonGroup[num].gameObject.GetComponent<ShowObjectButton>().CheckActiveObject())
            //{
            //    checkNumber++;
            //}
            int checkNumber = 0;
            foreach(var button in buttonGroup)
            {
                if(button.gameObject.GetComponent<ShowObjectButton>().CheckActiveObject())
                {
                    checkNumber++;
                }
            }
            SectionAndBackGroundManager.Instance.SetPopupData(num);
            documentCheck.SetOnDocument(num);

            CompleteCheck(checkNumber);
        }

        private void CompleteCheck(int number)
        {
            Debug.Log(number);
            //if (number >= 5)
            //{
            //    checkCompleteButton.GetComponent<ButtonController>().SetInteractable(true);
            //}
        }

        private void ShowDocument()
        {
            documentCheckList.SetActive(true);
        }

        private void HideDocuement()
        {
            documentCheckList.SetActive(false);
            documentCheck.CloseDocument();
            subDocumentCheck.CloseSubDocument();
        }

        private void OnDisable()
        {
            HideDocuement();
            foreach (var button in buttonGroup)
            {
                button.gameObject.GetComponent<ShowObjectButton>().HideObject();
            }
            //checkCompleteButton.GetComponent<ButtonController>().SetInteractable(false);
            checkNumber = 0;
        }
    }
}


