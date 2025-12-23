using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RJH.Transporter
{
    public class SectionAndBackGroundManager : MonoBehaviour, IPointerDownHandler
    {
        #region singleton
        private static SectionAndBackGroundManager instance;
        public static SectionAndBackGroundManager Instance { get { return instance; } }
        #endregion
        private Stack<GameObject> sectionStack = new Stack<GameObject>(); // 이전 섹션 저장 스택
        private Stack<int> sectionindexStack = new Stack<int>(); // 이전 섹션의 번호 저장 스택 (배경화면 변경에 사용)
        private Stack<PrevPage> prevPageStack = new Stack<PrevPage>(); // 이전 페이지 저장 스택
        private int popupData = -1; // 팝업 정보
        private PrevPage pageData; // 현재 페이지 임시 저장
        [SerializeField] private GameObject quitPopup;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button quitButton;
        [Space(10)]
        [SerializeField] private Button moveIndexButton;
        [SerializeField] private Button quitButton2;
        [Space(10)]
        [SerializeField] private GameObject firstPage;
        [SerializeField] private GameObject startPage;
        [Space(10)]
        [SerializeField] private GameObject currentSection; // 현재 활성화된 섹션 오브젝트 (페이지 정보를 스택에 저장할때 사용할 오브젝트)
        [SerializeField] private GameObject currentPage; // 현재 활성화된 페이지 
        [SerializeField] private int currentSectionIndex; //현재 활성화된 섹션 번호
        [SerializeField] private Button previouseButton; // 이전 섹션으로 가는 버튼
        [SerializeField] private TextMeshProUGUI titleText; // 섹션 이름
        [SerializeField] private Image backGroundImage; // 배경화면
        [SerializeField] private DocumentCheck documentCheck; // 서류점검 팝업
        [SerializeField] private TextSpriteObject titleAndBackGround; // 섹션과 배경화면 스크립트오브젝트
        [SerializeField] private DocumentListObject documentList; // 페이지 문서 스크립트 오브젝트
        [SerializeField] private Button exitButton; // 종료 버튼
        [Space(10)]
        [Header("Document")]
        [SerializeField] private GameObject documentObject; // 설명 오브젝트
        [SerializeField] private TextMeshProUGUI docTitleText; // 설명문 제목
        [SerializeField] private TextMeshProUGUI docText; // 설명문
        [SerializeField] private Button nextButton; // 다음 버튼
        [SerializeField] private Button prevButton; // 이전 버튼
        public int currentDocNum = 0; // 현재 문서 번호
        [SerializeField] private Button checkListButton; // 점검표 활성화 버튼
        [SerializeField] private GameObject checkListObj; // 점검표
        [SerializeField] private CheckListManager checkListManager; // 점검표 관리자
        [SerializeField] private SectionButtonManager sectionButtonManager; // 점검표 체크가 끝나면 비활성화가 필요한 섹션버튼
        public UnityAction<int> sectionAction;
        //[SerializeField] private ToggleGroup checkToggleGroup;
        [Space(10)]
        [Header("Index")]
        [SerializeField] private Button indexButton;
        [SerializeField] private Button indexcloseButton;
        [SerializeField] private GameObject indexPopup;
        [SerializeField] private Button indexHomeButton;
        [Space(10)]
        [SerializeField] private CheckSectionMarker sectionMarker;
        [Space(10)]
        [SerializeField] private GameObject highlightObject;
        [SerializeField] private DocsButton[] buttonList;
        [SerializeField] private GameObject[] sectionObject;

        private float timer = 0f; 
        private float interval = 5f; 

        private readonly int[] PREVEXCEPTIONNUMBER = { 24, 32, 35, 38 };
        //private List<string> nameCheck = new List<string>();
        private Dictionary<DocsButton, GameObject> _highlightedButtons = new Dictionary<DocsButton, GameObject>();

        public delegate bool ReturnEventDelegate();
        public event ReturnEventDelegate ReturnEvent;
        public event ReturnEventDelegate ReturnEventPrev;

        private void Awake()
        {
#if KFSI_ALL
            firstPage.SetActive(true);
            startPage.SetActive(false);
#endif

            instance = this;

            previouseButton.onClick.AddListener(MovePreviousSection);
            nextButton.onClick.AddListener(OnNextDocument);
            prevButton.onClick.AddListener(OnPrevDocument);
            exitButton.onClick.AddListener(OnQiutPopupOpen);
            closeButton.onClick.AddListener(OnQuitPopupClose);
            SetDocument(0);
            SetAllButton();

            checkListButton.onClick.AddListener(CheckList);
            indexHomeButton.onClick.AddListener(OnMoveHome);
            indexButton.onClick.AddListener(OpenIndexPopup);
            indexcloseButton.onClick.AddListener(CloseIndexPopup);
            quitButton.onClick.AddListener(OnExit);
            quitButton2.onClick.AddListener(OnQiutPopupOpen);
            moveIndexButton.onClick.AddListener(OnMoveHome);
#if UNITY_WEBGL
            quitButton2.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(false);
#endif
        }
        private void Update()
        {
#if !UNITY_WEBGL
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnQiutPopupOpen();
            }
#endif
            timer += Time.deltaTime; // 경과 시간을 누적

            if (timer >= interval)
            {
                CheckButton2();
                timer = 0f; // 타이머 초기화
            }
        }
        public void MoveNextSection(GameObject section, int index, bool resetSection, bool dontSaveSectionStack)
        {
            if(firstPage.activeSelf)
                firstPage.SetActive(false);

            if (section == null)
                return;
            if (dontSaveSectionStack == false)
            {
                sectionStack.Push(currentPage);
                sectionindexStack.Push(currentSectionIndex);
                Debug.Log("섹션 저장됨");
            }
            indexButton.gameObject.SetActive(true);
            currentPage.SetActive(false);

            currentPage = section;
            currentSectionIndex = index;

            if (resetSection)
            {
                ClearStack();
                sectionMarker.ResetMarker();
            }
            // 임시
            switch (currentSectionIndex)
            {
                case 0: // 목차
                    sectionStack.Clear();
                    currentSection = sectionObject[0];
                    break;
                case 1: // 서류점검
                    currentSection = sectionObject[1];
                    break;
                case 2: // 정기점검
                    currentSection = sectionObject[2];
                    break;
                case 9: // 비상대응 시나리오
                    currentSection = sectionObject[3];
                    break;
            }


            if (indexPopup.activeSelf)
            {
                indexPopup.SetActive(false);
            }
            // 
            //SetDocument(docIndex);
            PopupManager.Instance.PopupClose();
            SetTitleAndBackGround(currentSectionIndex);
            Debug.Log("켜짐");
            currentPage.SetActive(true);
            SavePageData();
        }

        public void MovePreviousSection()
        {
            indexButton.gameObject.SetActive(true);
            checkListManager.MoveIndex();
            currentPage.SetActive(false);
            if(sectionStack.Count == 0)
            {
                Debug.Log("처음 섹션으로");
                currentPage = sectionObject[0];
                currentSectionIndex = 0;
                AudioManager.Instance.StopDocs();
            }
            else
            {
                currentPage = sectionStack.Pop();
                currentSectionIndex = sectionindexStack.Pop();
            }
            
            SetTitleAndBackGround(currentSectionIndex);
            currentPage.SetActive(true);
            ResetCheckButton();
            SetCheckButton();
            ClearStack();

            switch (currentSectionIndex)
            {
                case 0:
                    sectionStack.Clear();
                    currentSection = sectionObject[0];
                    SetDocument(0);
                    AudioManager.Instance.StopDocs();
                    break;
                case 1:
                    currentSection = sectionObject[1];
                    break;
                case 2:
                    currentSection = sectionObject[2];
                    break;
                case 9: // 비상대응 시나리오
                    currentSection = sectionObject[3];
                    break;
            }

        }

        public void SetSection(GameObject section)
        {
            currentPage = section;
        }

        public void SetTitleAndBackGround(int num)
        {
            backGroundImage.sprite = titleAndBackGround.textSprites[num].sprite;
            titleText.text = titleAndBackGround.textSprites[num].title;
        }

        public void SetBackGround(int num)
        {
            backGroundImage.sprite = titleAndBackGround.textSprites[num].sprite;
        }

        public void SetBackGroundAndSave(int num)
        {
            currentSectionIndex = num;
            backGroundImage.sprite = titleAndBackGround.textSprites[currentSectionIndex].sprite;

        }

        public void SetBackGround()
        {
            backGroundImage.sprite = titleAndBackGround.textSprites[currentSectionIndex].sprite;
        }

        public void SetSectionOnOff(bool isOn)
        {
            currentPage.SetActive(isOn);
        }

        public void SetDocument(int num)
        {
            currentDocNum = num;

            if (num == 53)
            {
                MoveNextSection(sectionObject[0], 0, true, false);
                SetOffDocument();
                AudioManager.Instance.StopDocs();
                return;
            }

            if (currentDocNum == 0)
            {
                SetOffDocument();
            }
            else
            {
                if (currentDocNum == 18 && sectionMarker.ReturnBool())
                {
                    currentDocNum = 39;
                    
                }
                SetOnDocument();
                docTitleText.text = documentList.documentList[currentDocNum].title;
                docText.text = documentList.documentList[currentDocNum].document;
            }
            if (currentDocNum == 12)
            {
                currentSection = sectionObject[2];
            }
            if (currentDocNum == 41)
            {
                currentSection = sectionObject[3];
            }

            AudioManager.Instance.PlayDocs(documentList.documentList[currentDocNum].audioClip, documentList.documentList[currentDocNum].checkDuplication);
            ResetCheckButton();
            SetCheckButton();
            SetDocumentButton();

        }

        public void SetDocumentAndSavePage(int num)
        {

            PushPageStack();
            currentDocNum = num;
            if (currentDocNum == 0)
            {
                SetOffDocument();
            }
            else
            {
                SetOnDocument();
                docTitleText.text = documentList.documentList[currentDocNum].title;
                docText.text = documentList.documentList[currentDocNum].document;
            }

            AudioManager.Instance.PlayDocs(documentList.documentList[currentDocNum].audioClip, documentList.documentList[currentDocNum].checkDuplication);
            ResetCheckButton();
            SetCheckButton();
            SetDocumentButton();

            SavePageData();
        }

        public void SetDocument_text(string title, string docs)
        {
            docTitleText.text = title;
            docText.text = docs;
        }

        public void OnNextDocument()
        {
            if (ReturnEvent != null)
            {
                bool quizCollect = ReturnEvent.Invoke();
                if (quizCollect == false)
                    return;
                else
                    ReturnEvent = null;
            }
            if (CheckButton() == false)
            { return; }
            Debug.Log("PopupData" + popupData);
            PushPageStack();
            currentDocNum++;
            SetDocument(currentDocNum);
            sectionAction?.Invoke(currentDocNum);
            SavePageData();
        }

        public void OnNextDocument2()
        {
            if (CheckButton2() == false)
            { return; }
            Debug.Log("PopupData" + popupData);
            PushPageStack();
            currentDocNum++;
            SetDocument(currentDocNum);
            sectionAction?.Invoke(currentDocNum);
            SavePageData();
        }

        public void OnPrevDocument()
        {
            timer = 0f;
            if (ReturnEventPrev != null)
            {
                bool quizCollect = ReturnEventPrev.Invoke();
                if (quizCollect == false)
                    return;
                else
                    ReturnEventPrev = null;
            }

            if (prevPageStack.Count == 0)
            {
                Debug.Log("페이지 스택 끝");
                MovePreviousSection();
                return;
            }
            if (currentDocNum == 12)
            {
                currentPage.SetActive(false);
                currentSection = sectionObject[1];
            }
            if(currentDocNum== 41)
            {
                currentPage.SetActive(false);
                currentSection = sectionObject[2];
            }

            PopPageStack();

            if (PREVEXCEPTIONNUMBER.Contains(currentDocNum))
            {
                currentDocNum = 18;
            }
            SetDocument(currentDocNum);
            SavePageData();
        }

        public void SetOffDocument()
        {
            if (documentObject.activeSelf)
                documentObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            prevButton.gameObject.SetActive(false);
        }

        public void SetOnDocument()
        {
            if (!documentObject.activeSelf)
                documentObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
            prevButton.gameObject.SetActive(true);
        }

        public void SetDocumentButton()
        {
            switch (documentList.documentList[currentDocNum].removeButton)
            {
                case RemoveButton.None:
                    nextButton.gameObject.SetActive(true);
                    prevButton.gameObject.SetActive(true);
                    break;
                case RemoveButton.Right:
                    nextButton.gameObject.SetActive(false);
                    prevButton.gameObject.SetActive(true);
                    break;
                case RemoveButton.Left:
                    nextButton.gameObject.SetActive(true);
                    prevButton.gameObject.SetActive(false);
                    break;
                case RemoveButton.Both:
                    nextButton.gameObject.SetActive(false);
                    prevButton.gameObject.SetActive(false);
                    break;

            }

            if (!documentList.documentList[currentDocNum].isCheckListOn)
            {
                checkListManager.SetOffCheckList();
            }
            else
            {
                checkListManager.SetCheckList(currentDocNum);
            }

        }

        public void SetAllButton()
        {
            for (int i = 0; i < buttonList.Length; i++)
            {
                buttonList[i].SetButton();
            }
        }

        public void SetCheckButton()
        {
            int num = 0;

            for (int i = 0; i < buttonList.Length; i++)
            {
                if (!(documentList.documentList[currentDocNum].buttonNames.Length > num))
                    return;

                if (buttonList[i].name == documentList.documentList[currentDocNum].buttonNames[num])
                {
                    if (buttonList[i].name == "CheckListButton")
                    {
                        buttonList[i].gameObject.SetActive(true);
                    }

                    buttonList[i].SetClickAble();
                    ++num;
                }
            }
        }

        public void ResetCheckButton()
        {
            for (int i = 0; i < buttonList.Length; i++)
            {
                buttonList[i].OnRevert();
            }
        }

        public bool CheckButton()
        {
            bool isAllCheck = true;
            timer = 0f;
            for (int i = 0; i < buttonList.Length; i++)
            {
                if (buttonList[i].isChecked == false)
                {
                    if (buttonList[i].isSkippable)
                    {
                        buttonList[i].OnClicked();
                    }
                    else
                    {
                        StartCoroutine(BlinkHighlight(buttonList[i]));
                        isAllCheck = false;
                    }
                }
            }
            return isAllCheck;
        }

        public bool CheckButton2()
        {
            bool isAllCheck = true;
            timer = 0f;
            for (int i = 0; i < buttonList.Length; i++)
            {
                if (buttonList[i].isChecked == false)
                {
                    StartCoroutine(BlinkHighlight(buttonList[i]));
                    isAllCheck = false;
                }
            }
            return isAllCheck;
        }

        public void CheckList()
        {
            //checkListObj.SetActive(true);

            checkListManager.SetCheckList(currentDocNum);
            checkListButton.gameObject.SetActive(false);
            //OnNextDocument();
        }

        public void SavePageData()
        {
            pageData = new PrevPage();
            Debug.Log("SavePage");
            pageData.popupnumber = popupData;
            pageData.documentNumber = currentDocNum;
            pageData.bgnumber = currentSectionIndex;
            CheckAllChildObjects(currentSection.transform, pageData);
        }

        public void PushPageStack()
        {
            prevPageStack.Push(pageData);
        }

        public void PopPageStack()
        {
            Debug.Log(prevPageStack.Count);
            PrevPage prevPage = prevPageStack.Pop();
            Debug.Log(prevPage.popupnumber);
            SetTitleAndBackGround(currentSectionIndex); // 2024.11.08 위치 수정
            SetAllChildObjects(currentSection.transform, prevPage.isActive);
            if (currentSection == sectionObject[1])
            {
                //documentCheck.SetOnDocument(prevPage.popupnumber); // 2024.11.21 임시로 비활성화 
                //popupData = prevPage.popupnumber;
            }
            else
            {
                PopupManager.Instance.PopupClose();
                PopupManager.Instance.NormalSetPopup(prevPage.popupnumber);
                popupData = prevPage.popupnumber;
            }
            currentSectionIndex = prevPage.bgnumber;
            currentDocNum = prevPage.documentNumber;
            // SetTitleAndBackGround(currentSectionIndex); // 2024.11.08 위치 수정
        }

        public void PopStack(int number)
        {
            for (int i = 0; i < number; i++)
            {
                prevPageStack.Pop();
            }

        }

        public void ClearStack()
        {
            prevPageStack.Clear();
        }

        private void CheckAllChildObjects(Transform parent, PrevPage prevPage)
        {

            foreach (Transform child in parent)
            {
                if (child.name.Contains("HighlightObj"))
                    continue;
                if(child.name.Contains("NanumSquare"))
                    continue;
                if (child.name.Contains("Marker"))
                    continue;
                if (child.GetComponent<TextMeshProUGUI>() != null)
                    continue;
                Debug.Log(child.name);
                //nameCheck.Add(child.name);
                prevPage.isActive.Add(child.gameObject.activeSelf);

                CheckAllChildObjects(child, prevPage);
            }
        }

        private void SetAllChildObjects(Transform parent, List<bool> isactive)
        {

            foreach (Transform child in parent)
            {
                if (child.name.Contains("HighlightObj"))
                    continue;
                if (child.name.Contains("NanumSquare"))
                    continue;
                if (child.name.Contains("Marker"))
                    continue;
                if (child.GetComponent<TextMeshProUGUI>() != null)
                    continue;
                Debug.Log(child.name);
                //if (child.name != nameCheck[0])
                //{
                //    Debug.Log("다른 부분 발견!!!" + child.name);
                //}
                if (isactive[0])
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }

                isactive.RemoveAt(0);
                //nameCheck.RemoveAt(0);
                SetAllChildObjects(child, isactive);
            }
        }

        public void SetPopupData(int number)
        {
            popupData = number;
        }

        public void SetSectionButton(SectionButtonManager sectionButton)
        {
            sectionButtonManager = sectionButton;
        }

        public void SetCheckListDone()
        {
            if (sectionButtonManager != null)
            {
                sectionButtonManager.SetOnMarkerImage();
                sectionButtonManager = null;
            }
        }

        public void BackGroundChange()
        {
            backGroundImage.sprite = titleAndBackGround.textSprites[currentSectionIndex].subSprite;
        }

        private void OpenIndexPopup()
        {
            indexPopup.SetActive(true);
            indexPopup.GetComponent<FirstPagePopup>().GetPracticeButton().onClick.AddListener(() =>
            {
                if (firstPage.activeSelf)
                {
                    indexPopup.SetActive(false);
                    firstPage.SetActive(false);
                    startPage.SetActive(true);
                }
            });
        }
        
        private void OnMoveHome()
        {
            SceneManager.LoadSceneAsync("TitleScene");
        }

        private void CloseIndexPopup()
        {
            indexPopup.SetActive(false);
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Canvas touched!");
            
            CheckButton2();
        }

        private void SetHighlightObject(DocsButton targetButton)
        {
            if (_highlightedButtons.ContainsKey(targetButton))
                return;

            if (!targetButton.gameObject.activeInHierarchy)
                return;

            var highlightImage = GameObject.Instantiate(highlightObject, targetButton.transform);

            Image image = highlightImage.GetComponent<Image>();
            image.rectTransform.sizeDelta = targetButton.GetComponent<RectTransform>().sizeDelta;
            //image.color = new Color(0, 0.6f, 0, 0.3f); // 반투명한 노란색

            highlightImage.transform.localPosition = Vector3.zero;
            highlightImage.transform.localScale = Vector3.one;
            _highlightedButtons.Add(targetButton, highlightImage);
        }

        private IEnumerator BlinkHighlight(DocsButton targetButton)
        {

            if (_highlightedButtons.ContainsKey(targetButton))
                yield break;

            if (!targetButton.gameObject.activeInHierarchy)
                yield break;

            var highlightImage = GameObject.Instantiate(highlightObject, targetButton.buttonImage.transform);

            Image image = highlightImage.GetComponent<Image>();
            image.rectTransform.sizeDelta = targetButton.buttonImage.GetComponent<RectTransform>().sizeDelta;
            //image.color = new Color(0, 0.6f, 0, 0.3f); // 반투명한 노란색

            highlightImage.transform.localPosition = Vector3.zero;
            highlightImage.transform.localScale = Vector3.one;
            _highlightedButtons.Add(targetButton, highlightImage);
            image.enabled = true;
            yield return new WaitForSeconds(0.5f);
            if (null != highlightImage)
            {
                image.enabled = false;
                //ReturnHighlightImageToPool(highlightImage);
                Destroy(highlightImage);
            }
            _highlightedButtons.Remove(targetButton);
        }

        private void OnExit()
        {
            Application.Quit();
        }

        private void OnQuitPopupClose()
        {
            quitPopup.SetActive(false);
        }

        private void OnQiutPopupOpen()
        {
            quitPopup.SetActive(true);
        }
    }

    public class PrevPage
    {
        public List<bool> isActive = new List<bool>();
        public int bgnumber;
        public int popupnumber;
        public int documentNumber;
    }
}