using System;
using System.Collections.Generic;
using GASSYS;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public class GasSysMenu : MonoBehaviour
{
    [Foldout("가스계 메인 메뉴")] [SerializeField]
    private GameObject gssMenuParent;

    [SerializeField] private Button practiceModeBtn;
    [SerializeField] private Button evaluationModeBtn;
    [SerializeField] private Button prevBtn;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private TextMeshProUGUI[] titleTexts;

    [Foldout("모드 선택")] [SerializeField] private GameObject gasModeParent;
    [SerializeField] private Button safetyCheckModeBtn;
    [SerializeField] private Button solenoidValveTestBtn;
    [SerializeField] private Button dischargeIndicatorLightTestBtn;
    [SerializeField] private Button recoveryCheckBtn;

    [Foldout("격발 테스트 모드 선택")] [SerializeField]
    private GameObject solenoidValveTestParent;

    [SerializeField] private Button manualOperationBtn;
    [SerializeField] private Button manualControlBoxBtn;
    [SerializeField] private Button crossCircuitDetectorBtn;
    [SerializeField] private Button controlPanelSwitchBtn;

    private List<Button> _btnList = new List<Button>();
    [SerializeField] private GasSysSection sectionObj;


    private GasSysMainSection _curMainMenuState;
    private GasSysState _gasSysState;

    public void Init()
    {
        _btnList.Clear();

        sectionObj.InitEMode();
        _btnList.Add(practiceModeBtn);
        _btnList.Add(evaluationModeBtn);
        _btnList.Add(safetyCheckModeBtn);
        _btnList.Add(solenoidValveTestBtn);
        _btnList.Add(dischargeIndicatorLightTestBtn);
        _btnList.Add(recoveryCheckBtn);
        _btnList.Add(manualOperationBtn);
        _btnList.Add(manualControlBoxBtn);
        _btnList.Add(crossCircuitDetectorBtn);
        _btnList.Add(controlPanelSwitchBtn);

        //ButtonManager.Instance.EnableSpecificButton(_btnList.ToArray());
        gameObject.SetActive(true);
        practiceModeBtn.onClick.RemoveAllListeners();
        evaluationModeBtn.onClick.RemoveAllListeners();
        safetyCheckModeBtn.onClick.RemoveAllListeners();
        solenoidValveTestBtn.onClick.RemoveAllListeners();
        dischargeIndicatorLightTestBtn.onClick.RemoveAllListeners();
        recoveryCheckBtn.onClick.RemoveAllListeners();
        manualOperationBtn.onClick.RemoveAllListeners();
        manualControlBoxBtn.onClick.RemoveAllListeners();
        crossCircuitDetectorBtn.onClick.RemoveAllListeners();
        controlPanelSwitchBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.RemoveAllListeners();
        homeBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();

#if UNITY_WEBGL
        var homePos = homeBtn.GetComponent<RectTransform>().anchoredPosition;
        homeBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(573, homePos.y);
        exitBtn.gameObject.SetActive(false);
#endif
        _gasSysState = GasSysState.None;

        GasSysGlobalCanvas.Instance.ShowCompletePopup(false);
        practiceModeBtn.onClick.AddListener(delegate
        {
            ShowObject(gasModeParent);
            ChangeTitleText("실습모드");
            _gasSysState = GasSysState.PracticeMode;
            sectionObj.SetGasSysState(_gasSysState);
        });
        evaluationModeBtn.onClick.AddListener(delegate
        {
            ShowObject(gasModeParent);
            ChangeTitleText("평가모드");
            _gasSysState = GasSysState.EvaluationMode;
            sectionObj.SetGasSysState(_gasSysState);
        });
        safetyCheckModeBtn.onClick.AddListener(delegate
        {
            HideObject();
            //GasSysManager.Instance.Init();
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            sectionObj?.InitSafetyCheck();
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.SafetyCheck);
        });
        solenoidValveTestBtn.onClick.AddListener(delegate { ShowObject(solenoidValveTestParent); });
        dischargeIndicatorLightTestBtn.onClick.AddListener(delegate
        {
            //GasSysManager.Instance.Init();
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            sectionObj?.InitDischargeLightTest();
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.DischargeCheck);
            HideObject();
        });
        recoveryCheckBtn.onClick.AddListener(delegate
        {
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            //GasSysManager.Instance.Init();
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            sectionObj?.InitRecoveryCheck();
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.RecoveryCheck);
            HideObject();
        });
        manualOperationBtn.onClick.AddListener(delegate
        {
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            //GasSysManager.Instance.Init();
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.SolenoidCheck);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.Init);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.ManualOperationController);
            sectionObj?.InitManualOperation();
            HideObject();
        });
        manualControlBoxBtn.onClick.AddListener(delegate
        {
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            //GasSysManager.Instance.Init();
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.SolenoidCheck);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.Init);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.ManualControlBoxController);
            sectionObj?.InitManualControlBox();
            HideObject();
        });
        crossCircuitDetectorBtn.onClick.AddListener(delegate
        {
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            //GasSysManager.Instance.Init();
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.SolenoidCheck);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.Init);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.CrossCircuitDetectorController);
            sectionObj?.InitCrossCircuitDetector();
            HideObject();
        });
        controlPanelSwitchBtn.onClick.AddListener(delegate
        {
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            //GasSysManager.Instance.Init();
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.SolenoidCheck);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.Init);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.ControlPanelSwitchController);
            sectionObj?.InitControlPanelSwitch();
            HideObject();
        });

        prevBtn.onClick.AddListener(delegate
        {
            if (solenoidValveTestParent.activeSelf)
            {
                ShowObject(gasModeParent);
            }
            else if (gasModeParent.activeSelf)
            {
                ShowObject(gssMenuParent);
            }
        });
        homeBtn.onClick.AddListener(delegate
        {
            SoundManager.Instance.StopAllFireSound();
            //GasSysManager.Instance.Init();
            SceneManager.LoadSceneAsync("TitleScene");
            GasSysGlobalCanvas.Instance.HideCheckObj();
            //ShowMenuPanel(false);
        });
        exitBtn.onClick.AddListener(GasSysGlobalCanvas.Instance.ToggleExitPopup);
#if KFSI_ALL
        ShowObject(gssMenuParent);
#else
        practiceModeBtn.gameObject.SetActive(false);
        evaluationModeBtn.gameObject.SetActive(false);
        ShowObject(gasModeParent);
#if KFSI_TEST
        ChangeTitleText("평가모드");
        _gasSysState = GasSysState.EvaluationMode;
        sectionObj.SetGasSysState(_gasSysState);
#else
        ChangeTitleText("실습모드");
        _gasSysState = GasSysState.PracticeMode;
        sectionObj.SetGasSysState(_gasSysState);
#endif
#endif
    }

    private void ChangeTitleText(string title)
    {
        foreach (var obj in titleTexts)
        {
            obj.text = $"가스계소화설비({title})";
        }
    }

    private void ChangeState(GasSysMainSection state)
    {
        _curMainMenuState = state;
    }

    private void OnStateChanged(GasSysMainSection state)
    {
    }


    private void ShowObject(GameObject obj)
    {
        gssMenuParent.SetActive(gssMenuParent.Equals(obj));
        gasModeParent.SetActive(gasModeParent.Equals(obj));
        solenoidValveTestParent.SetActive(solenoidValveTestParent.Equals(obj));

#if KFSI_ALL
        prevBtn.gameObject.SetActive(!gssMenuParent.activeSelf);
#else
        prevBtn.gameObject.SetActive(solenoidValveTestParent.activeSelf);
#endif
        homeBtn.gameObject.SetActive(!gssMenuParent.activeSelf);
        exitBtn.gameObject.SetActive(!gssMenuParent.activeSelf);

#if UNITY_WEBGL
        exitBtn.gameObject.SetActive(false);
#endif
    }


    public void HideObject()
    {
        gameObject.SetActive(false);
        ShowObject(null);
    }


    public void OnEnable()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}