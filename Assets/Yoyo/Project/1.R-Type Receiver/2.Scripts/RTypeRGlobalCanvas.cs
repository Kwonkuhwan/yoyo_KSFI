using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RTypeRGlobalCanvas : MonoBehaviour
{
    #region singleton

    private static RTypeRGlobalCanvas instance;

    public static RTypeRGlobalCanvas Instance
    {
        get { return instance; }
    }

    #endregion

    [SerializeField] public HintPanel hintPanel;
    [SerializeField] public CompletePopup completePopup;
    [SerializeField] public RTypeRMenuPopup menuPopup;
    [SerializeField] public ResultPopup resultPopup;
    [SerializeField] public ExitPopup exitPopup;
    [SerializeField] public CheckPopup checkPopup;
    [SerializeField] public EvaluationNextPopup evaluationNextPopup;
    [SerializeField] public RTypeRTotalResultPopup totalResultPopup;

    public RTypeRTotalScore totalScore;

// #if UNITY_WEBGL
//     [DllImport("__Internal")]
//     private static extern void QuitGame();
// #endif
    private void Awake()
    {
        instance = this;
        menuPopup.Init();
        exitPopup.Init(OnExitYesButton, OnExitNoButton);
    }

    void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleExitPopup();
        }
    }

    public void SetHintPopup(HintTextAndAudio obj)
    {
        hintPanel.SetHintPopup(obj);
        ShowHint(true);
    }

    public void SetCompleteHint()
    {
        hintPanel.SetCompleteHint();
    }

    public void ShowHint(bool show)
    {
        hintPanel.ShowHint(show);
    }

    public void InitCompletePopup()
    {
        completePopup.Init();
    }

    public void SetStepBtn(UnityAction action)
    {
        completePopup.SetPrevStepBtn(action);
    }

    public void SetCompleteText(string text, string nextStr = "", Action action = null)
    {
        completePopup.SetCompleteText(text, nextStr, action);
    }

    public void ShowCompletePopup(bool isShow)
    {
        completePopup.ShowCompletePopup(isShow);
    }

    public bool IsShowCompletePopup()
    {
        return completePopup.IsShowCompletePopup();
    }

    public Button[] GetCompletePopupButtons()
    {
        return completePopup.GetButtons();
    }

    public void ShowMenuPopup(bool isShow)
    {
        menuPopup.ShowMenuPanel(isShow);
    }

    public bool IsShowResultPopup()
    {
        return resultPopup.gameObject.activeSelf;
    }

    public void SetResultPopup(List<ResultObject> results, string title = "")
    {
        resultPopup.SetResult(results, title);
    }


    public void ToggleExitPopup()
    {
        if (exitPopup == null)
            return;
        bool isActive = exitPopup.gameObject.activeSelf;
        exitPopup.gameObject.SetActive(!isActive);
    }

    public void OnExitYesButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
// #elif UNITY_WEBGL
//         QuitGame();
#else
        Application.Quit(0);
#endif
    }

    // 종료 팝업에서 "아니오" 버튼 클릭 시 팝업 닫기
    public void OnExitNoButton()
    {
        if (exitPopup != null)
        {
            exitPopup.gameObject.SetActive(false);
        }
    }

    public Button GetCheckAgreeBtn()
    {
        return checkPopup.GetCheckBtn();
    }

    public void HideCheckObj()
    {
        checkPopup.ShowObj(null);
    }

    public void SetNextEvaluation(Action action, Action reAction = null, bool complete = false)
    {
        evaluationNextPopup.SetNextEvaluation(action, reAction, complete);
    }

    public void InitTotalResult()
    {
        totalResultPopup.Init();
    }

    public bool IsShowTotalResultPopup()
    {
        return totalResultPopup.gameObject.activeSelf;
    }

    public void ShowTotalResultPopup(bool isShow)
    {
        totalResultPopup.gameObject.SetActive(isShow);
    }
}