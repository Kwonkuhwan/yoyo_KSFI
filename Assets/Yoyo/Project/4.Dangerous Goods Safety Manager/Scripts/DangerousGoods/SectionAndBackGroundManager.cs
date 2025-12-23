using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RJH.DangerousGoods
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
        //[SerializeField] private ButtonListObject buttonListObj;
        [Space(10)]
        [Header("Document")]
        [SerializeField] private GameObject documentObject; // 설명 오브젝트
        [SerializeField] private TextMeshProUGUI docTitleText; // 설명문 제목
        [SerializeField] private TextMeshProUGUI docText; // 설명문
        [SerializeField] private Button nextButton; // 다음 버튼
        [SerializeField] private Button prevButton; // 이전 버튼
        [SerializeField] private int currentDocNum = 0; // 현재 문서 번호
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
        [Header("Inventory")]
        [SerializeField] private Inventory inventory;
        [Space(10)]
        [Header("Alarm")]
        [SerializeField] private GameObject alarm1;
        [SerializeField] private GameObject alarm2;
        [Space(10)]
        [SerializeField] private GameObject highlightObject;
        [SerializeField] private DocsButton[] buttonList;
        [SerializeField] private GameObject[] sectionObject;
        [SerializeField] private GameObject[] allCompleteObject;
        private Dictionary<DocsButton, GameObject> _highlightedButtons = new Dictionary<DocsButton, GameObject>();
        private List<string> nameCheck = new List<string>();
        public delegate bool ReturnEventDelegate();
        public event ReturnEventDelegate ReturnEvent;
        public event ReturnEventDelegate ReturnEventPrev;

        private float timer = 0f;
        private float interval = 5f;

        private void Awake()
        {
#if KFSI_ALL
            firstPage.SetActive(true);
            startPage.SetActive(false);      
#endif
            instance = this;
            previouseButton.onClick.AddListener(() => { MovePreviousSection(); });
            nextButton.onClick.AddListener(OnNextDocument);
            prevButton.onClick.AddListener(OnPrevDocument);
            exitButton.onClick.AddListener(OpenQuitPopup);
#if UNITY_WEBGL
            quitButton2.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(false);
#endif
            //SetDocument(-1);
            SetAllButton();

            checkListButton.onClick.AddListener(CheckList);
            indexHomeButton.onClick.AddListener(OnMoveHome);
            indexButton.onClick.AddListener(OpenIndexPopup);
            indexcloseButton.onClick.AddListener(CloseIndexPopup);
            closeButton.onClick.AddListener(CloseQuitPopup);
            quitButton.onClick.AddListener(OnExit);
            quitButton2.onClick.AddListener(OpenQuitPopup);
            moveIndexButton.onClick.AddListener(OnMoveHome);
        }

        private void Update()
        {
#if !UNITY_WEBGL
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OpenQuitPopup();
            }
#endif

            timer += Time.deltaTime; // 경과 시간을 누적

            if (timer >= interval)
            {
                CheckButton2();
                timer = 0f; // 타이머 초기화
            }
        }

        private void OpenQuitPopup()
        {
            quitPopup.SetActive(true);
        }

        private void CloseQuitPopup()
        {
            quitPopup.SetActive(false);
        }

        public void MoveNextSection(GameObject section, int index, bool resetSection, bool dontresetPage, bool dontSaveSectionStack)
        {
            if(firstPage.activeSelf)
                firstPage.SetActive(false);

            if (section == null)
                return;
            if (dontSaveSectionStack == false)
            {
                sectionStack.Push(currentPage);
                sectionindexStack.Push(currentSectionIndex);   
            }

            if (index == 2 && resetSection == true)
            {
                foreach (GameObject obj in allCompleteObject)
                {
                    obj.SetActive(false);
                }

            }
            else if (currentSectionIndex == 20 && resetSection == true)
            {
                currentSection.transform.GetComponentInChildren<ConditionCheck>().CloseCheckList();
            }
            if (resetSection)
            {
                sectionindexStack.Clear();
                sectionindexStack.Clear();
            }

            indexButton.gameObject.SetActive(true);
            currentPage.SetActive(false);

            currentPage = section;


            if (dontresetPage == false)
                ClearStack();
            // 임시
            switch (index)
            {
                case -1: // 목차
                    sectionStack.Clear();
                    currentSection = sectionObject[0];
                    AudioManager.Instance.StopDocs();
                    //indexButton.gameObject.SetActive(false);
                    ResetCompleteObj();
                    break;
                case 1: // 서류점검
                    currentSection = sectionObject[1];
                    ResetCompleteObj();
                    break;
                case 2: // 정기점검
                    currentSection = sectionObject[2];
                    break;
                case 12: // 비상대응(화재) 시나리오
                    currentSection = sectionObject[3];
                    ResetCompleteObj();
                    break;
                case 20: // 비상대응(화재) 주유소 전체
                    currentSection = sectionObject[4];
                    ResetCompleteObj();
                    break;
                case 25: // 비상대응(누출) 시나리오
                    currentSection = sectionObject[5];
                    ResetCompleteObj();
                    break;
                case 37: // 비상대응(누출) 주유소 전체
                    currentSection = sectionObject[6];
                    ResetCompleteObj();
                    break;

            }
            currentSectionIndex = index;

            if (indexPopup.activeSelf)
            {
                indexPopup.SetActive(false);
            }
            // 
            //SetDocument(docIndex);
            PopupManager.Inastance.PopupClose();
            SetTitleAndBackGround(currentSectionIndex);
            currentPage.SetActive(true);
            SavePageData();
        }

        public void MovePreviousSection(bool clearPageStack = true, int popcount = 0)
        {
            indexButton.gameObject.SetActive(true);
            //checkListManager.MoveIndex();
            currentPage.SetActive(false);
            //int indexStack = sectionindexStack.Pop();
            //// 정기점검 중간에 목차로 정기점검 들어가면 뒤로 갈때 또 정기점검 화면이 나오는 버그 해결 로직
            //if (indexStack == 2 && currentSectionIndex == indexStack)
            //{
            //    currentPage = sectionObject[0];
            //    currentSectionIndex = -1;
            //    sectionStack.Clear();
            //    sectionindexStack.Clear();
            //}
            //else
            //{
            //    currentPage = sectionStack.Pop();
            //    currentSectionIndex = indexStack;
            //}
            if (sectionindexStack.Count <= 0)
            {
                MoveNextSection(sectionObject[0], -1, true, false, true);
                SetOffDocument();
                return;
            }
            currentPage = sectionStack.Pop();
            currentSectionIndex = sectionindexStack.Pop();

            SetTitleAndBackGround(currentSectionIndex);
            currentPage.SetActive(true);

            // 임시
            switch (currentSectionIndex)
            {
                case -1: // 안전물 관리자 초기 화면
                    sectionStack.Clear();
                    SetDocument(-1);
                    AudioManager.Instance.StopDocs();
                    //indexButton.gameObject.SetActive(false);
                    break;
                case 1: // 서류 점검
                    SetDocument(0);
                    break;
                case 2: // 정기 점검
                    bool isAllComplete = true;
                    foreach (GameObject obj in allCompleteObject)
                    {
                        if (!obj.activeSelf)
                            isAllComplete = false;
                    }
                    if (isAllComplete)
                    {
                        SetDocument(69);
                        //checkListManager.ShowMoveIndexButton();
                    }
                    else
                    {
                        SetDocument(15);
                    }
                    break;
            }

            ResetCheckButton();
            SetCheckButton();
            if (clearPageStack)
                ClearStack();
            else
                PopStack(popcount);

        }

        public void ResetCompleteObj()
        {
            foreach (GameObject obj in allCompleteObject)
            {
                obj.SetActive(false);
            }
        }

        public void SetSection(GameObject section)
        {
            currentPage = section;
        }

        public void SetTitleAndBackGround(int num)
        {
            if (num == -1)
                num = 0;
            else if (num == 0)
                num = 1;
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
            if (num == 122)
            {
                MoveNextSection(sectionObject[0], -1, true, false, true);
                SetOffDocument();
                return;
            }
            //checkListManager.MoveIndex();
            if (num == -1)
            {
                SetOffDocument();
                return;
            }

            if (num == 15)
            {
                currentSection = sectionObject[2];
            }
            if (num == 70)
            {
                currentSection = sectionObject[3];
            }
            //if(num == 83)
            //{
            //    alarm1.SetActive(true);
            //}
            if (num == 95)
            {
                currentSection = sectionObject[5];
            }
            //if(num == 111)
            //{
            //    alarm2.SetActive(true);
            //}

            if ((currentDocNum == 15|| currentDocNum == 69) && num != 15)
            {
                PushPageStack();
            }

            currentDocNum = num;
            if (currentDocNum == -1)
            {
                SetOffDocument();
            }
            else
            {
                SetOnDocument();
                docTitleText.text = documentList.documentList[currentDocNum].title;
                docText.text = documentList.documentList[currentDocNum].document;
            }
            AudioManager.Instance.PlayDocs(documentList.documentList[currentDocNum].audioClip);
            ResetCheckButton();
            SetCheckButton();
            SetDocumentButton();

        }

        public void SetDocument()
        {
            docTitleText.text = documentList.documentList[currentDocNum].title;
            docText.text = documentList.documentList[currentDocNum].document;
            AudioManager.Instance.PlayDocs(documentList.documentList[currentDocNum].audioClip);
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
            if (currentDocNum != 116)
                PushPageStack();
            if (currentDocNum != 15)
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
                Debug.Log("마지막");
                MovePreviousSection();
                return;
            }
            if (currentDocNum == 15)
            {
                currentPage.SetActive(false);
                currentSection = sectionObject[1];
                currentPage = sectionObject[1];
                currentPage.SetActive(true);
            }
            if (currentDocNum == 70)
            {
                currentPage.SetActive(false);
                currentSection = sectionObject[2];
                currentPage = sectionObject[4];
                currentPage.SetActive(true);
            }
            if (currentDocNum == 95)
            {
                currentPage.SetActive(false);
                currentSection = sectionObject[3];
                currentPage = sectionObject[6];
                currentPage.SetActive(true);
            }
            PopPageStack();
            if (currentDocNum == 15)
            {
                currentPage = sectionStack.Pop();
                currentSectionIndex = sectionindexStack.Pop();
            }
            //currentDocNum--;
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
                    if (buttonList[i].isSkippable && buttonList[i].isActiveAndEnabled)
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
            if (currentSection == sectionObject[1] && popupData == 1)
            {
                popupData = 6;
            }
            else
            {
                popupData = -1;
            }


            pageData.bgnumber = currentSectionIndex;
            CheckAllChildObjects(currentSection.transform, pageData);
        }

        public void PushPageStack()
        {
            Debug.Log("Push");
            prevPageStack.Push(pageData);
        }

        public void PopPageStack()
        {
            Debug.Log("+++++" + prevPageStack.Count);
            PrevPage prevPage = prevPageStack.Pop();
            Debug.Log(prevPage.popupnumber);
            SetTitleAndBackGround(currentSectionIndex); // 2024.11.08 위치 수정
            SetAllChildObjects(currentSection.transform, prevPage.isActive);
            if (currentSection == sectionObject[1])
            {
                documentCheck.SetOnDocument(prevPage.popupnumber);
                popupData = prevPage.popupnumber;
            }
            else
            {
                PopupManager.Inastance.PopupClose();
                PopupManager.Inastance.NormalSetPopup(prevPage.popupnumber);
                popupData = prevPage.popupnumber;
            }
            currentSectionIndex = prevPage.bgnumber;
            currentDocNum = prevPage.documentNumber;
            // SetTitleAndBackGround(currentSectionIndex); // 2024.11.08 위치 수정
        }

        public void ClearStack()
        {
            Debug.Log("초기화");
            prevPageStack.Clear();
        }

        public void PopStack(int index)
        {
            Debug.Log("일부 삭제");
            for (int i = 0; i < index; i++)
            {
                prevPageStack.Pop();
            }
            SavePageData();
        }

        private void CheckAllChildObjects(Transform parent, PrevPage prevPage)
        {

            foreach (Transform child in parent)
            {
                if (child.name.Contains("HighlightObj"))
                    continue;
                if (child.name.Contains("NanumSquare"))
                    continue;
                Debug.Log(child.name);
                nameCheck.Add(child.name);
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
                Debug.Log(child.name);
                if (child.name != nameCheck[0])
                {
                    Debug.Log("다른 부분 발견!!!" + child.name);
                }
                if (isactive[0])
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }

                isactive.RemoveAt(0);
                nameCheck.RemoveAt(0);
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
    }

    public class PrevPage
    {
        public List<bool> isActive = new List<bool>();
        public int bgnumber;
        public int popupnumber;
        public int documentNumber;
    }
}