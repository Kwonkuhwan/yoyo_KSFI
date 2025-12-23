using System.Collections.Generic;
using GASSYS;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VInspector;

public class GasSysMenuPopup : MonoBehaviour
{

    [SerializeField] private Button indexBtn;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button prevBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private TextMeshProUGUI titleText;


    [SerializeField] private GameObject menuPanel;

    [Foldout("가스계 메인 메뉴")]
    [SerializeField] private GameObject gssMenuParent;
    [SerializeField] private Button practiceModeBtn;
    [SerializeField] private Button evaluationModeBtn;

    [Foldout("모드 선택")]
    [SerializeField] private GameObject gasModeParent;
    [SerializeField] private Button safetyCheckModeBtn;
    [SerializeField] private Button solenoidValveTestBtn;
    [SerializeField] private Button dischargeIndicatorLightTestBtn;
    [SerializeField] private Button recoveryCheckBtn;

    [Foldout("격발 테스트 모드 선택")]
    [SerializeField] private GameObject solenoidValveTestParent;
    [SerializeField] private Button manualOperationBtn;
    [SerializeField] private Button manualControlBoxBtn;
    [SerializeField] private Button crossCircuitDetectorBtn;
    [SerializeField] private Button controlPanelSwitchBtn;

    [SerializeField] private GasSysSection sectionObj;
    [SerializeField] private GasSysMenu gasMenuObj;

    public RectTransform logoImg;

    private SoundCheck _soundCheck = new SoundCheck();
    // private enum MenuState
    // {
    //     None,
    //     PracticeMode, //실습 모드
    //     EvaluationMode, //평가 모드
    // }

    private GasSysState _gasSysState = GasSysState.None;

    private Stack<GameObject> _menuPanelStack = new Stack<GameObject>();


    public void Init()
    {
        indexBtn.onClick.RemoveAllListeners();
        homeBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();
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


        _gasSysState = GasSysState.None;
        prevBtn.gameObject.SetActive(false);

        indexBtn.onClick.AddListener(delegate
        {
            ShowMenuPanel(true);
        });
        closeBtn.onClick.AddListener(delegate
        {
            ShowMenuPanel(false);
        });

        homeBtn.onClick.AddListener(delegate
        {
            //메인으로 이동
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            SoundManager.Instance.StopAllFireSound();
            //GasSysManager.Instance.Init();
            SceneManager.LoadSceneAsync("TitleScene");
            GasSysGlobalCanvas.Instance.HideCheckObj();
            ShowMenuPanel(false);
        });

        exitBtn.onClick.AddListener(GasSysGlobalCanvas.Instance.ToggleExitPopup);

        prevBtn.onClick.AddListener(Prev);
#if UNITY_WEBGL
        var homePos = homeBtn.GetComponent<RectTransform>().anchoredPosition;
        homeBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(573, homePos.y);
        exitBtn.gameObject.SetActive(false);
#endif

        practiceModeBtn.onClick.AddListener(delegate
        {
            ShowObject(gasModeParent);
            titleText.text = $"가스계소화설비(실습모드)";
            _gasSysState = GasSysState.PracticeMode;
            sectionObj.SetGasSysState(_gasSysState);
        });
        evaluationModeBtn.onClick.AddListener(delegate
        {
            ShowObject(gasModeParent);
            titleText.text = $"가스계소화설비(평가모드)";
            _gasSysState = GasSysState.EvaluationMode;
            sectionObj.SetGasSysState(_gasSysState);
        });
        safetyCheckModeBtn.onClick.AddListener(delegate
        {
            SoundManager.Instance.StopAllFireSound();
            ShowMenuPanel(false);
            //GasSysManager.Instance.Init();
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            sectionObj?.InitSafetyCheck();
            gasMenuObj.HideObject();
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.SafetyCheck);
        });
        solenoidValveTestBtn.onClick.AddListener(delegate
        {
            ShowObject(solenoidValveTestParent);
        });
        dischargeIndicatorLightTestBtn.onClick.AddListener(delegate
        {
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            //GasSysManager.Instance.Init();
            SoundManager.Instance.StopAllFireSound();
            gasMenuObj.HideObject();
            sectionObj?.InitDischargeLightTest();
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.DischargeCheck);
            ShowMenuPanel(false);
        });
        recoveryCheckBtn.onClick.AddListener(delegate
        {
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            //GasSysManager.Instance.Init();
            SoundManager.Instance.StopAllFireSound();
            gasMenuObj.HideObject();
            sectionObj?.InitRecoveryCheck();
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.RecoveryCheck);
            ShowMenuPanel(false);
        });
        manualOperationBtn.onClick.AddListener(delegate
        {
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            //GasSysManager.Instance.Init();
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.SolenoidCheck);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.Init);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.ManualOperationController);
            SoundManager.Instance.StopAllFireSound();
            gasMenuObj.HideObject();
            sectionObj?.InitManualOperation();
            ShowMenuPanel(false);
        });
        manualControlBoxBtn.onClick.AddListener(delegate
        {
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            //GasSysManager.Instance.Init();
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.SolenoidCheck);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.Init);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.ManualControlBoxController);
            SoundManager.Instance.StopAllFireSound();
            gasMenuObj.HideObject();
            sectionObj?.InitManualControlBox();
            ShowMenuPanel(false);
        });
        crossCircuitDetectorBtn.onClick.AddListener(delegate
        {
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            //GasSysManager.Instance.Init();
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.SolenoidCheck);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.Init);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.CrossCircuitDetectorController);
            SoundManager.Instance.StopAllFireSound();
            gasMenuObj.HideObject();
            sectionObj?.InitCrossCircuitDetector();
            ShowMenuPanel(false);
        });
        controlPanelSwitchBtn.onClick.AddListener(delegate
        {
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            //GasSysManager.Instance.Init();
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.SolenoidCheck);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.Init);
            //GasSysManager.Instance.practiceMode.solenoidValveTestControllerObj.ChangeState(SolenoidValveTestState.ControlPanelSwitchController);
            SoundManager.Instance.StopAllFireSound();
            gasMenuObj.HideObject();
            sectionObj?.InitControlPanelSwitch();
            ShowMenuPanel(false);
        });
#if KFSI_ALL
        ShowObject(gssMenuParent);
#else
        practiceModeBtn.gameObject.SetActive(false);
        evaluationModeBtn.gameObject.SetActive(false);
        ShowObject(gasModeParent);
#if KFSI_TEST
        titleText.text = $"가스계소화설비(평가모드)";
        _gasSysState = GasSysState.EvaluationMode;
        sectionObj.SetGasSysState(_gasSysState);
#else
        titleText.text = $"가스계소화설비(실습모드)";
        _gasSysState = GasSysState.PracticeMode;
        sectionObj.SetGasSysState(_gasSysState);
#endif
#endif


    }

    private void Prev()
    {
        if (solenoidValveTestParent.activeSelf)
        {
            ShowObject(gasModeParent);
            return;
        }
        if (!gasModeParent.activeSelf)
            return;
        ShowObject(gssMenuParent);
        return;
    }

    private void Next()
    {
        prevBtn.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        //ShowObject(gssMenuParent);
    }

    public void ShowMenuPanel(bool isShow)
    {
        _gasSysState = GasSysState.None;
#if KFSI_ALL
        ShowObject(gssMenuParent);
#else
        ShowObject(gasModeParent);
#endif
        prevBtn.gameObject.SetActive(false);
        menuPanel.SetActive(isShow);
        bool isClose = !GasSysGlobalCanvas.Instance.IsShowCompletePopup() && !GasSysGlobalCanvas.Instance.IsShowResultPopup() &&
                       !GasSysGlobalCanvas.Instance.IsShowTotalResultPopup();
        closeBtn.gameObject.SetActive(isClose);
        logoImg.anchoredPosition = isClose ? new Vector2(484.5f, 326.5f) : new Vector2(564.5f, 326.5f);
        GasSysGlobalCanvas.Instance.ShowTotalResultPopup(false);
        GasSysGlobalCanvas.Instance.ShowCompletePopup(false);
        GasSysGlobalCanvas.Instance.HideCheckObj();
        if (isShow)
        {
            //SoundManager.Instance.StopAllFireSound(ref _soundCheck);
            SoundManager.Instance.ZeroVolume();
            SoundManager.Instance.StopHint();
        }
        else
        {
            SoundManager.Instance.SetDefaultVolume();
        }
    }

    private void ShowObject(GameObject obj)
    {
        gssMenuParent.SetActive(gssMenuParent.Equals(obj));
        gasModeParent.SetActive(gasModeParent.Equals(obj));
        solenoidValveTestParent.SetActive(solenoidValveTestParent.Equals(obj));
#if KFSI_ALL
        prevBtn.gameObject.SetActive(!gssMenuParent.activeSelf);
        if (gssMenuParent.activeSelf)
        {
            titleText.text = $"가스계소화설비";
        }
#else
        prevBtn.gameObject.SetActive(solenoidValveTestParent.activeSelf);
#if KFSI_TEST
        titleText.text = $"가스계소화설비(평가모드)";
        sectionObj.SetGasSysState(GasSysState.EvaluationMode);
#else
        titleText.text = $"가스계소화설비(실습모드)";
        sectionObj.SetGasSysState(GasSysState.PracticeMode);
#endif
#endif
    }

    private void ExitBtn()
    {
        ;
// #if UNITY_EDITOR
//         UnityEditor.EditorApplication.isPlaying = false;
// #else
//         Application.Quit(0);
// #endif
    }

}
