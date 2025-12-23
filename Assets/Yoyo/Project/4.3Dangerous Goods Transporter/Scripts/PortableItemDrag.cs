using RJH.Transporter;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PortableItemDrag : MonoBehaviour
{
    [SerializeField] private UIDragAndCollisionHandler dragHandler;
    [SerializeField] private GameObject[] items;
    [SerializeField] private int[] answers;
    [SerializeField] private GameObject wrongPopup;

    private int count = 0;
    private bool checkWrong = false;
    private void OnEnable()
    {
        dragHandler.OnCollisionDetected += DragItemEvent;
        dragHandler.OnPicked += ResetBoolValue;
        SectionAndBackGroundManager.Instance.ReturnEvent += ReturnResult;
    }

    private void DragItemEvent(GameObject d, GameObject t)
    {
        if (answers.Contains(Array.IndexOf(items, d)))
        {
            d.SetActive(false);
        }
        else
        {
            
            if (checkWrong == false)
            {
                StartCoroutine(PopupUpDown());
                count++;
                checkWrong = true;
                if(count >= 3)
                    AutoSolve();
            }
        }
    }

    private bool ReturnResult()
    {
        int i = 0;
        foreach (var item in items)
        {
            if (answers.Contains(i))
            {
                if (items[i].activeSelf)
                {
                    StartCoroutine(PopupUpDown());
                    count++;
                    if (count >= 3)
                        AutoSolve();
                    return false;
                }
            }
            i++;
        }
        return true;
    }

    private void ResetBoolValue(GameObject b)
    {
        checkWrong = false;
    }

    private void OnDisable()
    {
        foreach (var item in items)
        {
            item.gameObject.SetActive(true);
        }
        count = 0;
        wrongPopup.SetActive(false);
        dragHandler.OnCollisionDetected -= DragItemEvent;
        dragHandler.OnPicked -= ResetBoolValue;
        SectionAndBackGroundManager.Instance.ReturnEvent -= ReturnResult;
    }

    private IEnumerator PopupUpDown()
    {
        wrongPopup.SetActive(true);

        yield return new WaitForSeconds(1);

        wrongPopup.SetActive(false);
    }

    private void AutoSolve()
    {
        for (int i = 0; i < answers.Length; i++)
        {
            items[answers[i]].SetActive(false);
        }
    }
}
