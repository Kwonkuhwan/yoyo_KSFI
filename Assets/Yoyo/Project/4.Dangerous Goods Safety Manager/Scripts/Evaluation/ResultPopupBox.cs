using TMPro;
using UnityEngine;

public class ResultPopupBox : MonoBehaviour
{
    [SerializeField] private GameObject successObj;
    [SerializeField] private TextMeshProUGUI titleText;
    public void ResultBoxSet(QuizResult quizResult)
    {
        titleText.text = quizResult.title;
        successObj.SetActive(quizResult.result);
    }
}
