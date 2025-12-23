using RJH.DangerousGoods;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RJH
{
    public class DocumentCheck : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image exampleImage;
        [SerializeField] private Image subExampleImage;
        [SerializeField] private Image subExampleImage2;
        [SerializeField] private Image backGroundImage;
        [SerializeField] private Sprite[] backGroundSprites; // 0: middle, 1: small, 2: big
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button exampleButton;
        [SerializeField] private CheckListObject checkList;
        [SerializeField] private GameObject buttonGroup;
        [SerializeField] private SubDocumentCheck subdocumentCheck;
        private UnityAction action;
        public void Awake()
        {
            confirmButton.onClick.AddListener(CloseDocument);
        }

        public void SetOnDocument(int num)
        {
            Debug.Log(num);
            CloseButtonGroup();
            exampleImage.gameObject.SetActive(false);
            subExampleImage.gameObject.SetActive(false);
            subExampleImage2.gameObject.SetActive(false);
            exampleButton.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(false);
            subdocumentCheck.gameObject.SetActive(false);
            if (num == -1)
            {
                gameObject.SetActive(false);
                return;
            }
            if(num >= 6)
            {
                SetOnSubDocument(num - 5);
                return;
            }

            gameObject.SetActive(true);
            titleText.text = checkList.checkList[num].title;

            
            if (checkList.checkList[num].subSprite != null)
            {
                
                action = () => SetOnSubDocument(num);
                exampleButton.onClick.AddListener(action);
                backGroundImage.sprite = backGroundSprites[1];

                
                exampleButton.gameObject.SetActive(true);
                subExampleImage.gameObject.SetActive(true);
                subExampleImage.sprite = checkList.checkList[num].sprite;
                subExampleImage.SetNativeSize();
            }
            else if(checkList.checkList[num].subTitle.Length != 0)
            {

                backGroundImage.sprite = backGroundSprites[2];

                exampleImage.gameObject.SetActive(true);
                exampleImage.sprite = checkList.checkList[num].sprite;
                exampleImage.SetNativeSize();
                buttonGroup.SetActive(true);
                int index = 0;
                foreach (Transform button in buttonGroup.transform)
                {
                    int i = index;
                    button.GetComponent<ShowObjectButton>().SetText(checkList.checkList[num].subTitle[i], checkList.checkList[num].subText[i]);
                    button.GetComponent<Button>().onClick.AddListener(CheckActivateCloseButton);
                    index++;
                }
            }
            else
            {
                confirmButton.gameObject.SetActive(true);
                backGroundImage.sprite = backGroundSprites[0];
                exampleImage.gameObject.SetActive(true);
                exampleImage.sprite = checkList.checkList[num].sprite;
                exampleImage.SetNativeSize();
                //exampleImage.transform.localPosition = new Vector3(-exampleImage.GetComponent<RectTransform>().sizeDelta.x/2, 332, 0);
            }
            if(num == 3)
            {
                backGroundImage.sprite = backGroundSprites[2];
            }
            backGroundImage.SetNativeSize();
        }

        public void SetOnSubDocument(int num)
        {
            
            //backGroundImage.sprite = backGroundSprites[2];
            //backGroundImage.SetNativeSize();
            //subExampleImage2.gameObject.SetActive(true);
            //subExampleImage2.sprite = checkList.checkList[num].subSprite;
            //subExampleImage2.SetNativeSize();
            gameObject.SetActive(false);
            subdocumentCheck.SetOnSubDocument(checkList.checkList[num].title, subExampleImage.sprite , checkList.checkList[num].subSprite);
            confirmButton.gameObject.SetActive(true);
            exampleButton.onClick.RemoveListener(action);
        }

        public void CloseDocument()
        {
            gameObject.SetActive(false);
            //SectionAndBackGroundManager.Instance.SetPopupData(-1);
            CloseButtonGroup();
        }

        public void CheckActivateCloseButton()
        {
            if (buttonGroup.activeSelf)
            {
                int checkNumber = 0;
                foreach (Transform button in buttonGroup.transform)
                {
                    if (button.GetComponent<ShowObjectButton>().CheckActiveObject())
                    {
                        checkNumber++;
                    }
                }
                if (checkNumber < 4)
                    return;
            }

            confirmButton.gameObject.SetActive(true);
        }

        public void CloseButtonGroup()
        {
            if (buttonGroup.activeSelf)
            {
                foreach (Transform button in buttonGroup.transform)
                {
                    button.GetComponent<ShowObjectButton>().HideObject();
                }
                buttonGroup.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if (gameObject.activeSelf)
            {
                CloseButtonGroup();
                gameObject.SetActive(false);
            }
        }
    }
}


