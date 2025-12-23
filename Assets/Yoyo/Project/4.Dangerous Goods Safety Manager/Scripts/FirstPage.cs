using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstPage : MonoBehaviour
{
    [SerializeField] private Button practiceModeButton;
    [SerializeField] private Button evaluationModeButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private GameObject startPageObject;
    [SerializeField] private string sceneName;
    private void Awake()
    {
#if KFSI_ALL
        prevButton.gameObject.SetActive(true);
#endif
        practiceModeButton.onClick.AddListener(PracticeMode);
        evaluationModeButton.onClick.AddListener(EvaluationMode);
        prevButton.onClick.AddListener(PrevButton);
    }

    private void PracticeMode()
    {
        gameObject.SetActive(false);
        startPageObject.SetActive(true);
    }

    private void PrevButton()
    {
        startPageObject.SetActive(false);
        gameObject.SetActive(true);
    }

    private void EvaluationMode()
    {
        //SceneManager.LoadScene("SafetyManagerScene_EvaluationMode");
        SceneManager.LoadSceneAsync(sceneName);
    }
}
