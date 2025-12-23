using System.Collections.Generic;
using GASSYS;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public class RTypeRMenuPopup : MonoBehaviour
{

    [SerializeField] public RectTransform logoImg;
    [SerializeField] public Button indexBtn;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button prevBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private TextMeshProUGUI titleText;


    //[SerializeField] private TextMeshProUGUI titleText;

    [SerializeField] private GameObject menuPanel;

    [Foldout("R형 수신기 메인 메뉴")]
    [SerializeField] private GameObject rTypeRMenuParent;
    [SerializeField] private Button practiceModeBtn;
    [SerializeField] private Button evaluationModeBtn;

    [Foldout("모드 선택")]
    [SerializeField] private GameObject rTypeRModeParent;
    [FormerlySerializedAs("equipmentOperationModeBtn")]
    [SerializeField] private Button equipmentOperationBtn;
    [FormerlySerializedAs("solenoidValveTestBtn")]
    [SerializeField] private Button fireAlarmBtn;
    [FormerlySerializedAs("dischargeIndicatorLightTestBtn")]
    [SerializeField] private Button circuitBreakerBtn;

    [SerializeField] private RTypeRSection sectionObj;
    [FormerlySerializedAs("gasMenuObj")]
    [SerializeField] private RTypeRMenu rTypeRMenuObj;

    private SoundCheck _soundCheck = new SoundCheck();
    // private enum MenuState
    // {
    //     None,
    //     PracticeMode, //실습 모드
    //     EvaluationMode, //평가 모드
    // }

    private RTypeRState _rTypeRState = RTypeRState.None;

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
        equipmentOperationBtn.onClick.RemoveAllListeners();
        fireAlarmBtn.onClick.RemoveAllListeners();
        circuitBreakerBtn.onClick.RemoveAllListeners();



        _rTypeRState = RTypeRState.None;
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
            RTypeRGlobalCanvas.Instance.HideCheckObj();
            ShowMenuPanel(false);
        });
        
#if UNITY_WEBGL
        var homePos = homeBtn.GetComponent<RectTransform>().anchoredPosition;
        homeBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(572, homePos.y);
        exitBtn.gameObject.SetActive(false);
#endif

        exitBtn.onClick.AddListener(RTypeRGlobalCanvas.Instance.ToggleExitPopup);

        prevBtn.onClick.AddListener(Prev);


        practiceModeBtn.onClick.AddListener(delegate
        {
            ShowObject(rTypeRModeParent);
            titleText.text = $"R형 수신기(실습모드)";
            sectionObj.SetRTypeRState(RTypeRState.PracticeMode);
            //sectionObj.SetGasSysState(_gasSysState);
        });
        evaluationModeBtn.onClick.AddListener(delegate
        {
            ShowObject(rTypeRModeParent);
            titleText.text = $"R형 수신기(평가모드)";
            sectionObj.SetRTypeRState(RTypeRState.EvaluationMode);
            //sectionObj.SetGasSysState(_gasSysState);
        });
        equipmentOperationBtn.onClick.AddListener(delegate
        {
            SoundManager.Instance.StopAllFireSound();
            ShowMenuPanel(false);
            //GasSysManager.Instance.Init();
            //GasSysManager.Instance.ChangeState(GasSysState.Init);
            //GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            //sectionObj?.InitSafetyCheck();
            sectionObj?.InitEquipmentOperation();
            rTypeRMenuObj.HideObject();
            //GasSysManager.Instance.practiceMode.ChangeState(GasSysPracticeModeState.SafetyCheck);
        });
        fireAlarmBtn.onClick.AddListener(delegate
        {
            SoundManager.Instance.StopAllFireSound();
            ShowMenuPanel(false);
            sectionObj?.InitFireAlarmSystem();
            rTypeRMenuObj.HideObject();
            //ShowObject(solenoidValveTestParent);
        });
        circuitBreakerBtn.onClick.AddListener(delegate
        {
            SoundManager.Instance.StopAllFireSound();
            ShowMenuPanel(false);
            sectionObj?.InitCircuitBreaker();
            rTypeRMenuObj.HideObject();
        });
#if KFSI_ALL
        ShowObject(rTypeRMenuParent);
#else
        practiceModeBtn.gameObject.SetActive(false);
        evaluationModeBtn.gameObject.SetActive(false);
        ShowObject(rTypeRModeParent);
#if KFSI_TEST
        titleText.text = $"R형 수신기(평가모드)";
        sectionObj.SetRTypeRState(RTypeRState.EvaluationMode);
#else
        titleText.text = $"R형 수신기(실습모드)";
        sectionObj.SetRTypeRState(RTypeRState.PracticeMode);
#endif
#endif
    }

    private void Prev()
    {
        ShowObject(rTypeRModeParent);
        if (!rTypeRModeParent.activeSelf)
            return;
        ShowObject(rTypeRMenuParent);
    }

    private void OnDisable()
    {
        //ShowObject(gssMenuParent);
    }

    public void ShowMenuPanel(bool isShow)
    {
        _rTypeRState = RTypeRState.None;
#if KFSI_ALL
        ShowObject(rTypeRMenuParent);
#else
        ShowObject(rTypeRModeParent);
#endif
        prevBtn.gameObject.SetActive(false);
        menuPanel.SetActive(isShow);
        bool isClose = !RTypeRGlobalCanvas.Instance.IsShowCompletePopup() && !RTypeRGlobalCanvas.Instance.IsShowResultPopup() &&
                       !RTypeRGlobalCanvas.Instance.IsShowTotalResultPopup();
        closeBtn.gameObject.SetActive(isClose);
        logoImg.anchoredPosition = isClose ? new Vector2(484.5f, 326.5f) : new Vector2(564.5f, 326.5f);
        RTypeRGlobalCanvas.Instance.ShowTotalResultPopup(false);
        RTypeRGlobalCanvas.Instance.ShowCompletePopup(false);
        RTypeRGlobalCanvas.Instance.HideCheckObj();
        if (isShow)
        {
            //SoundManager.Instance.StopAllFireSound(ref _soundCheck);
            SoundManager.Instance.ZeroVolume();
            SoundManager.Instance.StopHint();
        }
        else
        {
            SoundManager.Instance.RecoveryVolume();
        }
    }

    private void ShowObject(GameObject obj)
    {
        //titleText.text = $"모드선택";
        rTypeRMenuParent.SetActive(rTypeRMenuParent.Equals(obj));
        rTypeRModeParent.SetActive(rTypeRModeParent.Equals(obj));
#if KFSI_ALL
        prevBtn.gameObject.SetActive(!rTypeRMenuParent.activeSelf);
        if (rTypeRMenuParent.activeSelf)
        {
            titleText.text = $"R형 수신기";
        }
#else
        prevBtn.gameObject.SetActive(false);
#if KFSI_TEST
        titleText.text = $"R형 수신기(평가모드)";
        sectionObj.SetRTypeRState(RTypeRState.EvaluationMode);
#else
        titleText.text = $"R형 수신기(실습모드)";
        sectionObj.SetRTypeRState(RTypeRState.PracticeMode);
#endif
#endif

    }
}
