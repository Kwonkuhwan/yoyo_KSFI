using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FirstPagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button practiceModeButton;
    [SerializeField] private Button evaluationModeButton;
    [SerializeField] private Button moveFirstPageButton;
    [SerializeField] private GameObject startPageObject;
    [SerializeField] private GameObject firstPageObject;

    [SerializeField] private string name_PracticeMode;
    [SerializeField] private string name_EvaluationMode;
    
    private void Awake()
    {
#if KFSI_ALL
        titleText.text = titleText.text.Replace("(실습모드)", "");
        firstPageObject.SetActive(true);
        startPageObject.SetActive(false);
        moveFirstPageButton.gameObject.SetActive(true);
#endif
        practiceModeButton.onClick.AddListener(PracticeMode);
        evaluationModeButton.onClick.AddListener(EvaluationMode);
        moveFirstPageButton.onClick.AddListener(MoveFirstPage);
    }

    private void PracticeMode()
    {
        if (SceneManager.GetActiveScene().name == name_PracticeMode)
        {
            
            titleText.text = titleText.text + "(실습모드)";
            firstPageObject.SetActive(false);
            startPageObject.SetActive(true);
        }
        else
        {
            SceneManager.LoadSceneAsync(name_PracticeMode);
        }
    }

    private void EvaluationMode()
    {
        //if (SceneManager.GetActiveScene().name == name_EvaluationMode)
        //{
        //    firstPageObject.SetActive(false);
        //    startPageObject.SetActive(true);
        //}
        //else
        //{
            SceneManager.LoadSceneAsync(name_EvaluationMode);
        //}
    }

    public void OnMoveHome()
    {
        SceneManager.LoadSceneAsync("TitleScene");
    }

    private void MoveFirstPage()
    {
#if KFSI_ALL
        titleText.text = titleText.text.Replace("(실습모드)", "");
        firstPageObject.SetActive(true);
        startPageObject.SetActive(false);
#elif !KFSI_Text
        //firstPageObject.SetActive(true);
        startPageObject.SetActive(true);
#endif
    }

    private void OnDisable()
    {
        MoveFirstPage();
    }

    public Button GetPracticeButton()
    {
        return practiceModeButton;
    }
}
