using RJH.Transporter;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectOneQuiz : MonoBehaviour
{
    [SerializeField] private int answerNumber;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private GameObject wrongPopup;

    private int count = 0;

    private bool CheckAnswer()
    {
        if (toggleGroup.ActiveToggles().Any())
        {
            int i = 0;
            foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                if (toggle.isOn == true && i == answerNumber)
                {
                    return true;
                }
                i++;
            }
        }
        StartCoroutine(PopupUpDown());
        count++;
        if(count >= 3)
        {
            int i = 0;
            foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                if(i == answerNumber)
                {
                    toggle.isOn = true;
                }
                else
                {
                    toggle.isOn = false;
                }
                i++;
            }
        }
        return false;
    }

    private void OnEnable()
    {
        SectionAndBackGroundManager.Instance.ReturnEvent += CheckAnswer;
    }

    private void OnDisable()
    {
        SectionAndBackGroundManager.Instance.ReturnEvent -= CheckAnswer;

        foreach (var toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            toggle.isOn = false;
        }
        count = 0;
        gameObject.SetActive(false);
        wrongPopup.SetActive(false);
    }

    private IEnumerator PopupUpDown()
    {
        wrongPopup.SetActive(true);

        yield return new WaitForSeconds(1);

        wrongPopup.SetActive(false);
    }
}
