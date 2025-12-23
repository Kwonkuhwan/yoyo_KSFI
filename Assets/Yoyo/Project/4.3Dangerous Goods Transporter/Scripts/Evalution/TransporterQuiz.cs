using UnityEngine;

public class TransporterQuiz : MonoBehaviour
{
    public int[] answer;
    public TransporterQuestionType questionType;

    public virtual bool CheckQuestionAnswer()
    {
        return true;
    }

    public virtual void QuizReset()
    {

    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
}
public enum TransporterQuestionType
{
    Document,
    Safety,
    Emergency
}
