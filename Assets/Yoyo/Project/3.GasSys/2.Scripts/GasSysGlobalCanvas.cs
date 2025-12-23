using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GasSysGlobalCanvas : MonoBehaviour
{
    #region singleton

    private static GasSysGlobalCanvas instance;

    public static GasSysGlobalCanvas Instance
    {
        get { return instance; }
    }

    #endregion

    [SerializeField] private TitleAndBackObj titleAndBackObj;

    [FormerlySerializedAs("hintPopup")] [SerializeField]
    private HintPanel hintPanel;

    [SerializeField] private CompletePopup completePopup;

    [FormerlySerializedAs("menuPopup")] [SerializeField]
    private GasSysMenuPopup gasSysMenuPopup;

    [SerializeField] private ResultPopup resultPopup;
    [SerializeField] private ExitPopup exitPopup;
    [SerializeField] private CheckPopup checkPopup;
    [SerializeField] private EvaluationNextPopup evaluationNextPopup;
    [SerializeField] private GasSysTotalResultPopup totalResultPopup;

    public GasSysTotalScore totalScore;

    private string _strMainTitle;
// #if UNITY_WEBGL
//     [DllImport("__Internal")]
//     private static extern void QuitGame();
// #endif
    private void Awake()
    {
        instance = this;
        gasSysMenuPopup.Init();
        exitPopup.Init(OnExitYesButton, OnExitNoButton);
    }

    public void SetTitle(string title, string subTitle = "")
    {
        _strMainTitle = title;
        SetSubTitle(subTitle);
    }

    public void SetSubTitle(string subTitle)
    {
        if (string.IsNullOrEmpty(_strMainTitle))
            titleAndBackObj.Init(subTitle);
        else
        {
            titleAndBackObj.Init(!string.IsNullOrEmpty(subTitle)
                ? $"{_strMainTitle} | {subTitle}"
                : $"{_strMainTitle}");
        }
    }

    public void SetBackBtn(UnityAction backAction)
    {
        titleAndBackObj.SetBackBtn(backAction);
    }

    // public void SetHintPopup(string title, string hint, RectTransform pos)
    // {
    //     hintPopup.SetHintPopup(title, hint, pos);
    // }
    // public void SetHintPopup(string title, List<string> hits, RectTransform pos)
    // {
    //     hintPopup.SetHintPopup(title, hits, pos);
    // }
    // public void SetHintPopup(HintScriptableObj hintObj, RectTransform pos)
    // {
    //     hintPopup.SetHintPopup(hintObj, pos);
    // }
    //
    // public void SetHintPopup(int min, int max, HintScriptableObj hintObj, RectTransform pos)
    // {
    //     hintPopup.SetHintPopup(min, max, hintObj, pos);
    // }

    public void SetHintPopup(HintTextAndAudio obj)
    {
        hintPanel.SetHintPopup(obj);
        ShowHint(true);
    }

    public void SetHintTextAndAudio(string hintStr, AudioClip audioClip = null)
    {
        hintPanel.SetHintTextAndAudio(hintStr, audioClip);
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

    public void SetCompleteText(string text)
    {
        completePopup.SetCompleteText(text);
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
        gasSysMenuPopup.ShowMenuPanel(isShow);
    }

    public bool IsShowResultPopup()
    {
        return resultPopup.gameObject.activeSelf;
    }

    public void SetResultPopup(List<ResultObject> results, string title = "")
    {
        resultPopup.SetResult(results, title);
    }

    private void Update()
    {
        // Windows에서 ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleExitPopup();
        }

        // Android에서 뒤로 가기 버튼 입력 감지
        // if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
        // {
        //     ToggleExitPopup();
        // }
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

    public void ShowArea1(UnityAction action)
    {
        checkPopup.ShowArea1(action);
    }

    public void ShowArea2(UnityAction action)
    {
        checkPopup.ShowArea2(action);
    }

    public void ShowFire(UnityAction action)
    {
        checkPopup.ShowFire(action);
    }

    public void ShowTimer(UnityAction action)
    {
        checkPopup.ShowTimer(action);
    }

    public void ShowSol1(UnityAction action)
    {
        checkPopup.ShowSol1(action);
    }

    public void ShowSol2(UnityAction action)
    {
        checkPopup.ShowSol2(action);
    }

    public void ShowSol3(UnityAction action)
    {
        checkPopup.ShowSol3(action);
    }

    public void ShowArea1_2(UnityAction action)
    {
        checkPopup.ShowArea1_2(action);
    }

    public void ShowLightTestOn(UnityAction action)
    {
        checkPopup.ShowLightTestOn(action);
    }

    public void ShowLightTestOff(UnityAction action)
    {
        checkPopup.ShowLightTestOff(action);
    }

    public void ShowLightTestCheck(UnityAction action)
    {
        checkPopup.ShowLightTestCheck(action);
    }

    public bool CheckArea2Open()
    {
        return checkPopup.Area2Open();
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