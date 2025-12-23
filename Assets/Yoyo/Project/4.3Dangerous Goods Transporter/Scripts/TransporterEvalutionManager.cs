using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransporterEvalutionManager : MonoBehaviour
{
    #region Singleton
    private static TransporterEvalutionManager instance;
    public static TransporterEvalutionManager Instance { get { return instance; } }
    #endregion

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Quiz[] quizs;
    [SerializeField] private ResultList[] resultLists;
    [Space(10)]
    [SerializeField] private GameObject resultPage;
    [SerializeField] private Image[] resultImages;
    [SerializeField] private Sprite[] resultsprite;
    [Space(10)]
    [SerializeField] private GameObject documentObj;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI documentText;
    [SerializeField] private DocumentListObject documentListObjects;
    [Space(10)]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button indexButton;
    [Space(10)]
    [SerializeField] private GameObject indexObject;
    [SerializeField] private Button documentButton;
    [SerializeField] private Button safetyButton;
    [SerializeField] private Button emergencyButton;
    [SerializeField] private Button indexCloseButton;
    [SerializeField] private Button exitButton;
    [Space(10)]
    [SerializeField] private GameObject exitObject;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button popupexitButton;
    private int currentIndex;
    private int restartIndexPoint = 0;
    private int limitIndex = 0;

    private void Awake()
    {
        instance = this;
        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenExitPopup();
        }
    }

    private void Init()
    {

        resultLists = new ResultList[4];
        for (int i = 0; i < resultLists.Length; i++)
        {
            resultLists[i] = new ResultList();
        }
        currentIndex = 0;
        SetText();
        nextButton.onClick.AddListener(OnNext);
        prevButton.onClick.AddListener(OnPrev);

        indexButton.onClick.AddListener(OpenIndex);
        documentButton.onClick.AddListener(GoDocumentPart);
        safetyButton.onClick.AddListener(GoSafetyPart);
        emergencyButton.onClick.AddListener(GoEmergencyPart);
        indexCloseButton.onClick.AddListener(CloseIndex);
        exitButton.onClick.AddListener(OpenExitPopup);

        closeButton.onClick.AddListener(CloseExitPopup);
        popupexitButton.onClick.AddListener(Quit);
    }

    private void OnPrev()
    {
        if (currentIndex == 0)
            return;

        quizs[currentIndex].Close();
        currentIndex--;
        quizs[currentIndex].Open();

        SetText();
        resultLists[(int)quizs[currentIndex].questionType].RemoveLastValue(quizs[currentIndex].quizTitle_1);
    }

    private void OnNext()
    {
        if (limitIndex > 0)
        {
            if (currentIndex >= restartIndexPoint + limitIndex - 1)
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

        bool result = quizs[currentIndex].CheckQuestionAnswer();
        string title = quizs[currentIndex].quizTitle_1;
        Debug.Log(result);
        resultLists[(int)quizs[currentIndex].questionType].Add(title, result);
        quizs[currentIndex].Close();
        currentIndex++;
        quizs[currentIndex].Open();
        if (limitIndex > 0)
        {
            quizs[currentIndex].QuizReset();
        }
        SetText();
    }

    private void ShowResult()
    {
        restartIndexPoint = 0;
        limitIndex = 0;

        resultPage.SetActive(true);
        for (int i = 0; i < resultLists.Length; i++)
        {
            int count = 0;
            foreach (var bo in resultLists[i].quizResultList)
            {
                if (bo.result)
                    count++;
            }

            if (count >= 2)
            {
                resultImages[i].sprite = resultsprite[0];
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

    private void SetText()
    {
        titleText.text = documentListObjects.documentList[currentIndex].title;
        documentText.text = documentListObjects.documentList[currentIndex].document;
    }

    private void OpenIndex()
    {
        indexObject.SetActive(true);
    }

    private void CloseIndex()
    {
        indexObject.SetActive(false);
    }

    private void GoDocumentPart()
    {

        CloseResult();
        CloseIndex();
        restartIndexPoint = 0;
        limitIndex = 4;
        currentIndex = restartIndexPoint;
        quizs[currentIndex].Open();
        quizs[currentIndex].QuizReset();
        SetText();
    }

    private void GoSafetyPart()
    {
        CloseResult();
        CloseIndex();
        restartIndexPoint = 4;
        limitIndex = 4;
        currentIndex = restartIndexPoint;
        quizs[currentIndex].Open();
        quizs[currentIndex].QuizReset();
        SetText();
    }

    private void GoEmergencyPart()
    {
        CloseResult();
        CloseIndex();
        restartIndexPoint = 8;
        limitIndex = 5;
        currentIndex = restartIndexPoint;
        quizs[currentIndex].Open();
        quizs[currentIndex].QuizReset();
        SetText();
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

        Application.Quit();
        Debug.Log("Á¾·á");
    }
}
