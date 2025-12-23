using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExitPopup : MonoBehaviour
{
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button cancelBtn;

    public void Init(Action exitAction, Action cancelAction)
    {
        exitBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();


        exitBtn.onClick.AddListener(exitAction.Invoke);
        cancelBtn.onClick.AddListener(cancelAction.Invoke);
    }
}