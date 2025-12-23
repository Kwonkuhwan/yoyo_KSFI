using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public class RTypeRMenu : MonoBehaviour
{
    #region singleton

    private static RTypeRMenu instance;

    public static RTypeRMenu Instance
    {
        get { return instance; }
    }

    #endregion

    [SerializeField] private Button prevBtn;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button exitBtn;

    [Foldout("R형 수신기 모드 메뉴")] [SerializeField]
    private GameObject rTypeRModeParent;

    [SerializeField] private Button practiceModeBtn;
    [SerializeField] private Button evaluationModeBtn;

    [Foldout("메뉴 선택")] [SerializeField] private GameObject rTypeRMenuParent;
    [SerializeField] private Button equipmentOperationBtn;
    [SerializeField] private Button fireAlarmBtn;
    [SerializeField] private Button circuitBreakerBtn;
    [SerializeField] private TextMeshProUGUI rTypeRMenuText;

    [SerializeField] private RTypeRSection sectionObj;

    private List<Button> _btnList = new List<Button>();

    private RTypeRState _rTypeRState;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _btnList.Clear();
        _btnList.Add(practiceModeBtn);
        _btnList.Add(evaluationModeBtn);
        _btnList.Add(evaluationModeBtn);
        _btnList.Add(equipmentOperationBtn);
        _btnList.Add(fireAlarmBtn);
        _btnList.Add(circuitBreakerBtn);

        //ButtonManager.Instance.EnableSpecificButton(_btnList.ToArray());
        gameObject.SetActive(true);
        //ShowObject(rTypeRModeParent);
        practiceModeBtn.onClick.RemoveAllListeners();
        evaluationModeBtn.onClick.RemoveAllListeners();
        equipmentOperationBtn.onClick.RemoveAllListeners();
        fireAlarmBtn.onClick.RemoveAllListeners();
        circuitBreakerBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.RemoveAllListeners();
        homeBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();

#if UNITY_WEBGL
        var homePos = homeBtn.GetComponent<RectTransform>().anchoredPosition;
        homeBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(398, homePos.y);
        exitBtn.gameObject.SetActive(false);
#endif

        _rTypeRState = RTypeRState.None;
        practiceModeBtn.onClick.AddListener(delegate
        {
            ShowObject(rTypeRMenuParent);
            rTypeRMenuText.text = "R형 수신기(실습모드)";
            _rTypeRState = RTypeRState.PracticeMode;
            sectionObj.SetRTypeRState(_rTypeRState);
        });
        evaluationModeBtn.onClick.AddListener(delegate
        {
            ShowObject(rTypeRMenuParent);
            rTypeRMenuText.text = "R형 수신기(평가모드)";
            _rTypeRState = RTypeRState.EvaluationMode;
            sectionObj.SetRTypeRState(_rTypeRState);
        });

        equipmentOperationBtn.onClick.AddListener(delegate
        {
            HideObject();
            sectionObj.InitEquipmentOperation();
            //설비 동작 초기화
        });
        fireAlarmBtn.onClick.AddListener(delegate
        {
            HideObject();
            sectionObj.InitFireAlarmSystem();
        });
        circuitBreakerBtn.onClick.AddListener(delegate
        {
            HideObject();
            sectionObj.InitCircuitBreaker();
        });
        prevBtn.onClick.AddListener(delegate
        {
            if (rTypeRMenuParent.activeSelf)
            {
                ShowObject(rTypeRModeParent);
            }
        });
        homeBtn.onClick.AddListener(delegate
        {
            SoundManager.Instance.StopAllFireSound();
            //GasSysManager.Instance.Init();
            SceneManager.LoadSceneAsync("TitleScene");
        });
        exitBtn.onClick.AddListener(RTypeRGlobalCanvas.Instance.ToggleExitPopup);

#if KFSI_ALL
        ShowObject(rTypeRModeParent);
#else
        practiceModeBtn.gameObject.SetActive(false);
        evaluationModeBtn.gameObject.SetActive(false);
        ShowObject(rTypeRMenuParent);
#if KFSI_TEST
        rTypeRMenuText.text = "R형 수신기(평가모드)";
        _rTypeRState = RTypeRState.EvaluationMode;
        sectionObj.SetRTypeRState(_rTypeRState);
#else
        rTypeRMenuText.text = "R형 수신기(실습모드)";
        _rTypeRState = RTypeRState.PracticeMode;
        sectionObj.SetRTypeRState(_rTypeRState);
#endif
#endif
    }

    private void ShowObject(GameObject obj)
    {
        rTypeRModeParent.SetActive(rTypeRModeParent.Equals(obj));
        rTypeRMenuParent.SetActive(rTypeRMenuParent.Equals(obj));
#if KFSI_ALL
        prevBtn.gameObject.SetActive(!rTypeRModeParent.activeSelf);
#else
        prevBtn.gameObject.SetActive(false);
#endif
        homeBtn.gameObject.SetActive(!rTypeRModeParent.activeSelf);
        exitBtn.gameObject.SetActive(!rTypeRModeParent.activeSelf);

#if UNITY_WEBGL
        exitBtn.gameObject.SetActive(false);
#endif
    }

    public void HideObject()
    {
        gameObject.SetActive(false);
        ShowObject(null);
    }
}