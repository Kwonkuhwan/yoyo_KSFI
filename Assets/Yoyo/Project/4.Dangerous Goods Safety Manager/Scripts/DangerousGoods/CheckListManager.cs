using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RJH.DangerousGoods
{
    public class CheckListManager : MonoBehaviour
    {
        //[SerializeField] private Button checkListButton;

        [SerializeField] private Button checkCompleteButton;
        //[SerializeField] private Button moveIndexButton; // 목차로 이동하는 버튼
        [SerializeField] private GameObject[] checkListObjs;
        [SerializeField] private Answer[] answersList;
        [SerializeField] private int[] popCountsList;
        [SerializeField] private GameObject wrongPopup;
        [SerializeField] private Sprite[] buttonSprites;
        private Answer answer;
        private int popCount = 0;
        private GameObject checkListPopup;

        private void Awake()
        {
            checkCompleteButton.onClick.AddListener(CheckComplete);
            //moveIndexButton.onClick.AddListener(MoveIndex);
        }

        public void SetCheckList(int number)
        {
            if (number == 21 || number == 22)
            {
                number = 0;
            }
            else if (number == 27 || number == 28)
            {
                number = 1;
            }
            else if (number == 33 || number == 34)
            {
                number = 2;
            }
            else if (number == 42 || number == 43)
            {
                number = 3;
            }
            else if (number == 48 || number == 49)
            {
                number = 4;
            }
            else if (number == 59 || number == 60)
            {
                number = 5;
            }
            else if (number == 63 || number == 64)
            {
                number = 6;
            }
            else if (number == 67 || number == 68)
            {
                number = 7;
            }

            checkListPopup = checkListObjs[number];
            answer = answersList[number];
            popCount = popCountsList[number];

            SectionAndBackGroundManager.Instance.SetSectionOnOff(false);
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

        public void CheckButtonOn(bool isChnage)
        {
            foreach (ToggleGroup group in checkListPopup.GetComponentsInChildren<ToggleGroup>())
            {
                if (!group.ActiveToggles().Any())
                {
                    checkCompleteButton.GetComponent<Image>().sprite = buttonSprites[0];
                    //checkCompleteButton.interactable = false;
                    return;
                }
            }
            checkCompleteButton.GetComponent<Image>().sprite = buttonSprites[1];
           // checkCompleteButton.interactable = true;
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
                //checkCompleteButton.interactable = false;
                checkCompleteButton.gameObject.SetActive(false);
            }
        }

        //public void ShowMoveIndexButton()
        //{
        //    moveIndexButton.gameObject.SetActive(true);
        //}

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
            SetOffCheckList();

            SectionAndBackGroundManager.Instance.SetSectionOnOff(true);
            SectionAndBackGroundManager.Instance.SetCheckListDone();
            SectionAndBackGroundManager.Instance.MovePreviousSection(false, popCount);
        }

        //public void MoveIndex()
        //{
        //    moveIndexButton.gameObject.SetActive(false);
        //}

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


