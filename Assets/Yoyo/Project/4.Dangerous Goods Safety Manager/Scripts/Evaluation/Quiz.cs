using UnityEngine;
using UnityEngine.Events;

public class Quiz : MonoBehaviour
{
    public string quizTitle_1;
    public string quizTitle_2;
    public int[] answer;
    public QuestionType questionType;
    public bool isDoubleAnswer = false;

    public UnityAction resetAction;

    public virtual bool CheckQuestionAnswer()
    {
        return true;
    }

    public virtual (bool, bool) CheckDoubleQuestionAnswer()
    {
        return (true, true);
    }

    public virtual void QuizReset()
    {
        resetAction?.Invoke();
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

public enum QuestionType
{
    Part1,
    Part2,
    Part3,
    Part4
}
