using RJH.Transporter;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SelectOne : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject wrongPopup;
    [SerializeField] private GameObject correctCheck;   
    [SerializeField] private Button mixingStandardsButton;

    private void Awake()
    {
        SetButton();
    }

    private void SetButton()
    {
        for(int i = 0; i < buttons.Length;i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => SelectedButton(index));
        }
    }

    private void SelectedButton(int index)
    {
        if(index == 3)
        {
            correctCheck.SetActive(true);
            mixingStandardsButton.gameObject.SetActive(true);
            SectionAndBackGroundManager.Instance.OnNextDocument();
        }
        else
        {
            StartCoroutine(PopupUpDown());
        }
    }

    private IEnumerator PopupUpDown()
    {
        wrongPopup.SetActive(true);

        yield return new WaitForSeconds(1);

        wrongPopup.SetActive(false);
    }
}
