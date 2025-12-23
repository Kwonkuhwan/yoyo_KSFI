using RJH.Transporter;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SelectItemsQuiz : Quiz
{
    [SerializeField] private Toggle[] items;
    [SerializeField] private bool[] selectItems;
    [SerializeField] private GameObject wrongPopup;
    private void Awake()
    {
        selectItems = new bool[items.Length];
        int i = 0;
        foreach (var item in items)
        {
            int index = i;
            item.onValueChanged.AddListener((isOn) => { SelectItems(index, isOn); });
            i++;
        }

        
    }

    private void SelectItems(int index ,bool isOn)
    {
        selectItems[index] = isOn;
    }

    public override bool CheckQuestionAnswer()
    {
        int i = 0;
        foreach (var item in selectItems)
        {
            int answerValue = item ? 1 : 0;
            if(answerValue != answer[i])
            {
                StartCoroutine(PopupUpDown());
                return false;
            }
            i++;
        }

        SectionAndBackGroundManager.Instance.ReturnEvent -= CheckQuestionAnswer;
        ResetSection();
        return true;
    }

    private void OnEnable()
    {
        SectionAndBackGroundManager.Instance.ReturnEvent += CheckQuestionAnswer;
    }
    private void OnDisable()
    {
        ResetSection();
        SectionAndBackGroundManager.Instance.ReturnEvent -= CheckQuestionAnswer;
    }

    private void ResetSection()
    {
        foreach (var item in items)
        {
            item.isOn = false;  
        }
    }

    private IEnumerator PopupUpDown()
    {
        wrongPopup.SetActive(true);

        yield return new WaitForSeconds(1);

        wrongPopup.SetActive(false);
    }

}
