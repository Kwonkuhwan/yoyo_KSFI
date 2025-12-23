using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EvaluationManager : MonoBehaviour
{
    #region Singleton
    private static EvaluationManager instance;
    public static EvaluationManager Instance { get { return instance; } }
    #endregion

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Quiz[] quizs;
    [SerializeField] private ResultList[] resultLists;
    [Space(10)]
    [SerializeField] private GameObject resultPage;
    [SerializeField] private Button[] resultButtons;
    [SerializeField] private Image[] resultImages;
    [SerializeField] private Sprite[] resultsprite;
    [SerializeField] private GameObject resultPopup;
    [SerializeField] private Button closeResultPopupButton;
    [SerializeField] private GameObject[] resultBoxs;
    [SerializeField] private TextMeshProUGUI resultPopupTitleText;
    [Space(10)]
    [SerializeField] private GameObject documentObj;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI documentText;
    [SerializeField] private DocumentListObject documentListObjects;
    [Space(10)]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button indexButton;
    [SerializeField] private Button indexButton2;
    [Space(10)]
    [SerializeField] private GameObject indexObject;
    [SerializeField] private SectionButton[] sectionButtons;
    [SerializeField] private Button indexCloseButton;
    [SerializeField] private Button indexexitButton;
    [SerializeField] private Button exitButton;
    [Space(10)]
    [SerializeField] private GameObject exitObject;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button popupexitButton;
    private int currentIndex;
    private SectionButton currentSectionButton;
    public delegate bool ReturnEventDelegate();
    public event ReturnEventDelegate ReturnEvent;
    //private int restartIndexPoint = 0;
    //private int limitIndex = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
#if !UNITY_WEBGL
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenExitPopup();
        }
#endif
    }

    private void Init()
    {
        currentSectionButton = null;
        resultLists = new ResultList[4];
        for (int i = 0; i < resultLists.Length; i++)
        {
            resultLists[i] = new ResultList();
        }

        //foreach(var quiz in quizs)
        //{
        //    if(quiz.isDoubleAnswer)
        //    {
        //        string title_1 = quiz.quizTitle_1;
        //        string title_2 = quiz.quizTitle_2;
        //        resultLists[(int)quiz.questionType].Add(title_1, false);
        //        resultLists[(int)quiz.questionType].Add(title_2, false);
        //    }
        //    else
        //    {
        //        string title = quiz.quizTitle_1;
        //        resultLists[(int)quiz.questionType].Add(title, false);
        //    }
        //}

        currentIndex = 0;
        SetTextAndButton();
        nextButton.onClick.AddListener(OnNext);
        prevButton.onClick.AddListener(OnPrev);


#if KFSI_ALL
        indexButton.onClick.AddListener(OpenIndex);
        indexButton2.onClick.AddListener(OpenIndex);
#elif KFSI_TEST
        indexButton2.onClick.AddListener(OnMoveHome);
        indexButton.onClick.AddListener(OnMoveHome);
#endif

        for (int i = 0; i < sectionButtons.Length; i++)
        {
            int index = i;
            sectionButtons[i].button.onClick.AddListener(() => { OnSectionButtonClicked(index); });
        }

        for (int i = 0; i < resultButtons.Length; i++)
        {
            int index = i;
            resultButtons[i].onClick.AddListener(() => { OpenResultPopup(index, resultButtons[index].GetComponentInChildren<TextMeshProUGUI>().text); });
        }
        closeResultPopupButton.onClick.AddListener(CloseResultPopup);

        indexCloseButton.onClick.AddListener(CloseIndex);
        exitButton.onClick.AddListener(OpenExitPopup);
        indexexitButton.onClick.AddListener(OpenExitPopup);
#if UNITY_WEBGL
        exitButton.gameObject.SetActive(false);
        indexexitButton.gameObject.SetActive(false);
#endif
        closeButton.onClick.AddListener(CloseExitPopup);
        popupexitButton.onClick.AddListener(Quit);
    }

    private void OnPrev()
    {
        if (currentIndex == 0)
            return;
        if (currentSectionButton != null && currentSectionButton.startSection == currentIndex)
            return;
        
        quizs[currentIndex].Close();
        currentIndex--;
        quizs[currentIndex].Open();

        SetTextAndButton();
        if (quizs[currentIndex].isDoubleAnswer)
        {
            resultLists[(int)quizs[currentIndex].questionType].RemoveLastDoubleValue(quizs[currentIndex].quizTitle_1, quizs[currentIndex].quizTitle_2);
        }
        else
        {
            resultLists[(int)quizs[currentIndex].questionType].RemoveLastValue(quizs[currentIndex].quizTitle_1);
        }
    }

    private void OnNext()
    {
        if (ReturnEvent != null)
        {
            bool quizCollect = ReturnEvent.Invoke();
            if (quizCollect == false)
                return;
            else
                ReturnEvent = null;
        }

        if (quizs[currentIndex].isDoubleAnswer)
        {
            var bools = quizs[currentIndex].CheckDoubleQuestionAnswer();
            var title_1 = quizs[currentIndex].quizTitle_1;
            var title_2 = quizs[currentIndex].quizTitle_2;
            Debug.Log(bools.Item1);
            resultLists[(int)quizs[currentIndex].questionType].Add(title_1, bools.Item1);
            Debug.Log(bools.Item2);
            resultLists[(int)quizs[currentIndex].questionType].Add(title_2, bools.Item2);
        }
        else
        {
            bool result = quizs[currentIndex].CheckQuestionAnswer();
            string title = quizs[currentIndex].quizTitle_1;
            Debug.Log(result);
            //resultLists[(int)quizs[currentIndex].questionType].results.Add(result);
            resultLists[(int)quizs[currentIndex].questionType].Add(title, result);
        }

        if (currentSectionButton != null)
        {
            if (currentIndex >= currentSectionButton.endSection)
            {
                quizs[currentIndex].Close();
                documentObj.SetActive(false);
                ShowResult();
                return;
            }
        }

        if (quizs.Length - 1 == currentIndex)
        {
            quizs[currentIndex].Close();
            documentObj.SetActive(false);
            ShowResult();
            return;
        }

        quizs[currentIndex].Close();
        currentIndex++;
        quizs[currentIndex].Open();

        if (currentSectionButton != null)
        {
            quizs[currentIndex].QuizReset();
        }
        SetTextAndButton();
    }

    private void ShowResult()
    {
        //restartIndexPoint = 0;
        //limitIndex = 0;
        currentSectionButton = null;
        resultPage.SetActive(true);
        for (int i = 0; i < resultImages.Length; i++)
        {
            int count = 0;
            foreach (var bo in resultLists[i].quizResultList)
            {
                if (bo.result)
                    count++;
            }
            //if (resultLists[i].quizResultList.Count < 5)
            //{
            //    if (count == resultLists[i].quizResultList.Count)
            //    {
            //        resultImages[i].sprite = resultsprite[0];
            //    }
            //    else if (count < 1)
            //    {
            //        resultImages[i].sprite = resultsprite[1];
            //    }
            //    else
            //    {
            //        resultImages[i].sprite = resultsprite[2];
            //    }
            //}
            //else
            //{
            //    if (count == resultLists[i].quizResultList.Count)
            //    {
            //        resultImages[i].sprite = resultsprite[0];
            //    }
            //    else if(count < 1)
            //    {
            //        resultImages[i].sprite = resultsprite[1];
            //    }
            //    else
            //    {
            //        resultImages[i].sprite = resultsprite[2];
            //    }
            //}
            if(count == resultLists[i].quizResultList.Count)
            {
                resultImages[i].sprite = resultsprite[0];
            }
            else if(count != 0)
            {
                resultImages[i].sprite = resultsprite[2];
            }
            else
            {
                resultImages[i].sprite = resultsprite[1];
            }
            
        }
    }

    private void CloseResult()
    {
        documentObj.SetActive(true);
        resultPage.SetActive(false);
    }

    private void OpenResultPopup(int index, string title)
    {
        resultPopup.SetActive(true);
        resultPopupTitleText.text = title + " 평가 결과";
        for (int i = 0 ;i < resultLists[index].quizResultList.Count;i++)
        {
            GameObject obj = resultBoxs[i];
            obj.SetActive(true);
            obj.GetComponent<ResultPopupBox>().ResultBoxSet(resultLists[index].quizResultList[i]);
        }

    }

    private void CloseResultPopup()
    {
        resultPopup.SetActive(false);
        for (int i = 0; i < resultBoxs.Length; i++)
        {
            GameObject obj = resultBoxs[i];
            obj.SetActive(false);
        }
    }

    private void SetTextAndButton()
    {
        titleText.text = documentListObjects.documentList[currentIndex].title;
        documentText.text = documentListObjects.documentList[currentIndex].document;
        AudioManager.Instance.PlayDocs(documentListObjects.documentList[currentIndex].audioClip);
        if(currentIndex == 0)
        {
            prevButton.gameObject.SetActive(false);
        }
        else
        {
            prevButton.gameObject.SetActive(true);
        }
    }

    private void OpenIndex()
    {
        indexObject.SetActive(true);
    }

    private void CloseIndex()
    {
        indexObject.SetActive(false);
    }

    private void OnSectionButtonClicked(int index)
    {
        CloseResult();
        CloseIndex();
        currentSectionButton = sectionButtons[index];
        //resultLists[index].quizResultList.Clear();
        currentIndex = currentSectionButton.startSection;
        quizs[currentIndex].Open();
        quizs[currentIndex].QuizReset();
        SetTextAndButton();
    }

    private void OpenExitPopup()
    {
        exitObject.SetActive(true);
    }

    private void CloseExitPopup()
    {
        exitObject.SetActive(false);
    }

    private void Quit()
    {
#if !UNITY_WEBGL
        Application.Quit();
        Debug.Log("종료");
#endif
    }

    private void OnMoveHome()
    {
        SceneManager.LoadSceneAsync("TitleScene");
    }
}

[System.Serializable]
public class ResultList
{
    public List<QuizResult> quizResultList = new List<QuizResult>();

    public void RemoveLastValue(string title)
    {
        quizResultList.RemoveAt(quizResultList.Count - 1);
    }

    public void RemoveLastDoubleValue(string title_1, string title_2)
    {
        quizResultList.RemoveAt(quizResultList.Count - 1);
        quizResultList.RemoveAt(quizResultList.Count - 1);

    }

    public void Add(string title, bool value)
    {
        QuizResult quizResult = new QuizResult();
        quizResult.title = title;
        quizResult.result = value;
        quizResultList.Add(quizResult);
    }
}

[System.Serializable]
public class QuizResult
{
    public string title;
    public bool result;
}


[System.Serializable]
public class SectionButton
{
    public Button button;
    public int startSection;
    public int endSection;
}



