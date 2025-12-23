using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RJH.Transporter
{
    public class CheckListManager : MonoBehaviour
    {

        [SerializeField] private Button checkCompleteButton;
        [SerializeField] private GameObject checkListPopup;
        [SerializeField] private Button moveIndexButton; // 목차로 이동하는 버튼
        [SerializeField] private GameObject[] checkListObjs;
        [SerializeField] private Answer[] answersList;
        [SerializeField] private Answer answer;
        [SerializeField] private GameObject wrongPopup;
        [SerializeField] private Sprite[] buttonSprites;
        public int currentCheckListIndex = 0;
        public UnityAction<int> nextAction;

        private void Awake()
        {
            checkCompleteButton.onClick.AddListener(CheckComplete);
            moveIndexButton.onClick.AddListener(MoveIndex);
        }

        public void SetCheckList(int number)
        {
            if (number == 26 || number == 27)
            {
                currentCheckListIndex = 0; 
            }
            else if(number == 29 || number == 30)
            {
                currentCheckListIndex = 1;
            }
            else if(number == 31 || number == 32)
            {
                currentCheckListIndex = 2;
            }
            else if (number == 34 || number == 35)
            {
                currentCheckListIndex = 3;
            }
            else if(number == 37 || number == 38)
            {
                currentCheckListIndex = 4;
            }
            checkListPopup = checkListObjs[currentCheckListIndex].gameObject;
            answer = answersList[currentCheckListIndex];
            checkListPopup.SetActive(true);
            SetCheckList();
            checkCompleteButton.gameObject.SetActive(true);
        }

        public void SetCheckList()
        {
            foreach (ToggleGroup group in checkListPopup.GetComponentsInChildren<ToggleGroup>())
            {
                foreach (Toggle toggle in group.GetComponentsInChildren<Toggle>())
                {
                    toggle.onValueChanged.RemoveListener(CheckButtonOn);
                    toggle.onValueChanged.AddListener(CheckButtonOn);
                }

            }
        }

        public void CheckButtonOn(bool isChange)
        {
            foreach (ToggleGroup group in checkListPopup.GetComponentsInChildren<ToggleGroup>())
            {
                if (!group.ActiveToggles().Any())
                {
                    checkCompleteButton.GetComponent<Image>().sprite = buttonSprites[0];
                    return;
                }
            }
            checkCompleteButton.GetComponent<Image>().sprite = buttonSprites[1];
        }

        public void SetOffCheckList()
        {
            if (checkListPopup != null && checkListPopup.activeSelf)
            {
                foreach (ToggleGroup group in checkListPopup.GetComponentsInChildren<ToggleGroup>())
                {
                    if (group.ActiveToggles().Any())
                        group.ActiveToggles().FirstOrDefault().isOn = false;
                }
                checkListPopup.SetActive(false);
                checkCompleteButton.GetComponent<Image>().sprite = buttonSprites[0];
                checkCompleteButton.gameObject.SetActive(false);
            }
        }

        public void ShowMoveIndexButton()
        {
            moveIndexButton.gameObject.SetActive(true);
        }

        public void CheckComplete()
        {
            int j = 0;
            foreach (ToggleGroup group in checkListPopup.GetComponentsInChildren<ToggleGroup>())
            {
                if (!group.ActiveToggles().Any())
                {
                    StartCoroutine(PopupUpDown());
                    return;
                }

                int i = 0;
                foreach (Toggle toggle in group.transform.GetComponentsInChildren<Toggle>())
                {
                    if (toggle.isOn == true)
                    {
                        if (i != answer.answers[j])
                        {
                            StartCoroutine(PopupUpDown());
                            return;
                        }
                    }
                    ++i;
                }
                ++j;
            }
            nextAction?.Invoke(currentCheckListIndex);
            SetOffCheckList();
            
        }

        public void MoveIndex()
        {
            moveIndexButton.gameObject.SetActive(false);
        }

        private IEnumerator PopupUpDown()
        {
            wrongPopup.SetActive(true);

            yield return new WaitForSeconds(1);

            wrongPopup.SetActive(false);
        }

    }

    [Serializable]
    public class Answer
    {
        public int[] answers;
    }

}


