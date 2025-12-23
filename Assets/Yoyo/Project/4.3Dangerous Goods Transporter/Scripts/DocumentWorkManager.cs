using RJH.Transporter;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DocumentWorkManager : MonoBehaviour
{
    [Header("위험물 운반 요청")]
    [SerializeField] private GameObject phoneCallObject;
    [SerializeField] private GameObject popup;
    [SerializeField] private DocsButton dangerousGoodconFirmButton;
    [SerializeField] private GameObject extendedPopup;
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject wrongPopup;
    [SerializeField] private GameObject correctCheck;
    [SerializeField] private DocsButton mixingStandardsButton;
    [Space(10)]
    [Header("위험물 혼재 기준")]
    [SerializeField] private GameObject mixingStandardPopup;
    [SerializeField] private DocsButton pictogramButton;
    [Space(10)]
    [Header("그림문자")]
    [SerializeField] private GameObject pictogramPopup;
    [SerializeField] private DocsButton confirmButton;
    [Space(10)]
    [Header("체크리스트")]
    //[SerializeField] private DocsButton checkListActiveButton;
    [SerializeField] private GameObject checkListPopup;
    [SerializeField] private DocsButton[] checkListButtons;
    [SerializeField] private GameObject[] checkObjects;
    [SerializeField] private DocsButton allCheckButton;
    [Space(10)]
    [Header("체크 팝업")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject checkPopup;
    [SerializeField] private Image checkPopupBgImage;
    [SerializeField] private Sprite[] checkPopupBgSprites;
    [SerializeField] private Image checkPopupImage;
    [SerializeField] private DocsButton checkListConfirmButton;
    [SerializeField] private TransporterPopupData transporterDocumentPopupData;
    private void Awake()
    {
        SetButton();
    }

    private void Update()
    {
        
    }

    private void SetButton()
    {
        dangerousGoodconFirmButton.docsAction += ShowExtendedPopup;
        mixingStandardsButton.docsAction += ShowMixingStandardPopup;
        pictogramButton.docsAction += ShowPictogramPopup;
        //confirmButton.docsAction += ShowCheckList;
        //checkListActiveButton.docsAction += ShowCheckList;
        checkListConfirmButton.docsAction += CloseChekPopup;
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => SelectedButton(index));
        }
        for (int i = 0; i < checkListButtons.Length; i++)
        {
            int index = i;
            checkListButtons[i].docsAction += () => SelectedCheckList(index);
        }
    }

    private void ShowExtendedPopup()
    {
        Debug.Log("111");
        phoneCallObject.SetActive(false);
        popup.SetActive(false);
        extendedPopup.SetActive(true);
    }

    private void ShowMixingStandardPopup()
    {
        Debug.Log("222");
        extendedPopup.SetActive(false);
        mixingStandardPopup.SetActive(true);
    }

    private void ShowPictogramPopup()
    {
        Debug.Log("333");
        mixingStandardPopup.SetActive(false);
        pictogramPopup.SetActive(true);
    }

    private void SelectedButton(int index)
    {
        if (correctCheck.activeSelf)
            return;

        if (index == 3)
        {
            correctCheck.SetActive(true);
            mixingStandardsButton.gameObject.SetActive(true);
            SectionAndBackGroundManager.Instance.OnNextDocument();
        }
        else
        { 
            Debug.Log("틀림");
            StartCoroutine(PopupUpDown());
        }

    }

    private void ShowCheckList()
    {
        Debug.Log("555");
        pictogramPopup.SetActive(false);
        checkListPopup.gameObject.SetActive(true);
    }

    private void SelectedCheckList(int index)
    {
        checkObjects[index].SetActive(true);
        TransporterPopup popupData = transporterDocumentPopupData.transporterPopups[index];
        checkPopupBgImage.sprite = checkPopupBgSprites[popupData.popupBgNumber];
        checkPopupImage.sprite = popupData.exampleImage_1;
        checkPopupBgImage.SetNativeSize();
        checkPopupImage.SetNativeSize();
        checkPopup.SetActive(true);
        titleText.text = popupData.titleText;
        descriptionText.text = popupData.descriptionText;
    }

    private void CloseChekPopup()
    {
        checkPopup.SetActive(false);
    }

    private IEnumerator PopupUpDown()
    {
        wrongPopup.SetActive(true);

        yield return new WaitForSeconds(1);

        wrongPopup.SetActive(false);
    }
}
