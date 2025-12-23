using RJH.DangerousGoods;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChooseOneQuiz : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject[] correctObj;
    [SerializeField] private GameObject[] resultObj;
    [SerializeField] private int[] answers;
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject wrongPopup;
    [SerializeField] private DocsButton docsButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private QuizDocument[] quizDocument;

    [SerializeField] private string titleText;
    [SerializeField] private string docsText;
    [SerializeField] private AudioClip audioClip;
    private bool checkAllAnswer = false;
    private bool firstCheck = true;

    private int count = 0;
    private void Awake()
    {
        Init();
    }

    private void OnDisable()
    {
        count = 0;
        SectionAndBackGroundManager.Instance.ReturnEvent -= CheckOpenQuizPopup;
        gameObject.SetActive(false);
        ResetButton();

        // 팝업이 비활성화 되면 경고 팝업 비활성화
        if(wrongPopup.activeSelf)
            wrongPopup.SetActive(false);
    }

    private void OnEnable()
    {
        //SectionAndBackGroundManager.Instance.OnNextDocument();
        SectionAndBackGroundManager.Instance.ReturnEvent += CheckOpenQuizPopup;
        popup.SetActive(true);
        OnPopupOpened();
    }

    public bool CheckOpenQuizPopup()
    {
        if (checkAllAnswer)
        {
            return true;
        }
        else
        {
            popup.SetActive(true);
            OnPopupOpened();
            return false;
        }
    }

    private void Init()
    {
        int i = 0;
        foreach (var button in buttons)
        {
            int index = i;
            button.onClick.AddListener(() => { SetButton(index); });
            i++;
        }
    }

    private void SetButton(int number)
    {

        if (answers.Contains(number))
        {
            if (resultObj[number].activeSelf)
                return;

            correctObj[number].SetActive(true);
            resultObj[number].SetActive(true);
            popup.SetActive(false);
            nextButton.SetActive(true);

            if (firstCheck)
            {
                firstCheck = false;
            }
            else
            {
                //checkAllAnswer = true;
                //SectionAndBackGroundManager.Instance.OnNextDocument();
            }
            SectionAndBackGroundManager.Instance.SetDocument_text(quizDocument[number].titleText, quizDocument[number].descriptionText);
            AudioManager.Instance.PlayDocs(quizDocument[number].audioClip);
        }
        else
        {

            StartCoroutine(PopupUpDown());
            count++;
            if(count >=3)
            {
                for (int i = 0; i < answers.Length; i++)
                {
                    correctObj[answers[i]].SetActive(true);
                }
            }
        }
    }

    private IEnumerator PopupUpDown()
    {
        wrongPopup.SetActive(true);

        yield return new WaitForSeconds(1);

        wrongPopup.SetActive(false);
    }

    private void ResetButton()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            correctObj[i].SetActive(false);
        }
        for (int i = 0; i < resultObj.Length; i++)
        {
            if (resultObj[i] != null)
                resultObj[i].SetActive(false);
        }
        firstCheck = true;
        checkAllAnswer = false;
    }

    private void OnPopupOpened()
    {
        int count = 0;
        SectionAndBackGroundManager.Instance.SetDocument_text(titleText, docsText);
        AudioManager.Instance.PlayDocs(audioClip,true);
        for (int i = 0; i < resultObj.Length; i++)
        {
            if (resultObj[i].activeSelf)
                count++;   
        }
        
        if(count == answers.Length)
        {
            checkAllAnswer = true;
            SectionAndBackGroundManager.Instance.SetDocument_text(titleText, docsText + "(완료)");
        }
    }
}

[System.Serializable]
public class QuizDocument
{
    public string titleText;
    public string descriptionText;
    public AudioClip audioClip;
}
