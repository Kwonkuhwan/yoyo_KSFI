using System;
using System.Collections.Generic;
using System.Linq;
using LJS;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ControlPanel : MonoBehaviour
{
    public static ControlPanel Instance;
    [FormerlySerializedAs("parentPanel")]
    [SerializeField] private GameObject objPanel;
    [SerializeField] public Button closeBtn;
    [SerializeField] private Transform defaultParent;
    [SerializeField] private Transform storageRoomPopupParent;
    [SerializeField] private Transform area1CorridorPopupParent;
    [SerializeField] private Transform area1PopupParent;


#region 점검 전 안전 조치

    public UnityAction<bool> SafetyCheckAction;

#endregion

#region 수동조작함 작동

    public UnityAction<bool> ManualControlBoxAction;

#endregion

#region 복구

    public UnityAction<bool> RecoveryCheckAction;
    public UnityEvent RecoverySwitchEvent = new UnityEvent();

#endregion
    private FireSoundCheck _fireSoundCheck;
    [FormerlySerializedAs("SoundCheckAction")]
    public UnityEvent<FireSoundCheck> soundCheckAction;

#region 탑패널

    static public readonly UnityEvent<ControlPanelButtonCheck> ControlPanelButtonAction = new UnityEvent<ControlPanelButtonCheck>();
    public ControlPanelButtonCheck controlPanelButton = new ControlPanelButtonCheck();
    public enum EAreaName
    {
        Detector1, //감지기 A
        Detector2, //감지기 B
        ActivateSolenoidValve, //솔레노이드밸브 기동
        VerifyDischarge, //방출 확인
        ActivateManualControlBox // 수동조작함 기동
    }

    [Header("탑패널")]
    [SerializeField] private GameObject topAreaPanel;
    [SerializeField] private GameObject timerPanel;

    [SerializeField] private List<ConfirmationPanel> area1Panels = new List<ConfirmationPanel>();
    [SerializeField] private List<ConfirmationPanel> area2Panels = new List<ConfirmationPanel>();

    [SerializeField] private string time = "30.0";
    [FormerlySerializedAs("timerPanel")]
    [SerializeField] public TimerPanel timerPanelObj;
    [SerializeField] private ConfirmationPanel timerAbort;
    [SerializeField] private NameListScriptableObj areaNameList;
    public static readonly UnityEvent<float> OnTimerCheck = new UnityEvent<float>();
    public static readonly UnityEvent OnTimerEnd = new UnityEvent();
    public static readonly UnityEvent OnTimerStart = new UnityEvent();



    private readonly Dictionary<string, ConfirmationPanel> _area1Dict = new Dictionary<string, ConfirmationPanel>();
    private readonly Dictionary<string, ConfirmationPanel> _area2Dict = new Dictionary<string, ConfirmationPanel>();

    //private readonly string[] _areaNames = { "감지기 A", "감지기 B", "슬레노이드 밸브 기동", "방출 확인", "수동조작함 기동" };

#endregion // 탑패널

#region 사이드 패널

    [Header("사이드 패널")]
    [SerializeField] private GameObject sidePanel;
    [SerializeField] private List<ConfirmationPanel> sidePanels = new List<ConfirmationPanel>();
    [SerializeField] private VoltageIndicatorPanel voltageIndicatorPanel;
    [SerializeField] private NameListScriptableObj sideNameList;
    [SerializeField] private GameObject fireImg;


    private readonly Dictionary<string, ConfirmationPanel> _sideDict = new Dictionary<string, ConfirmationPanel>();
    //private readonly string[] _sideNames = { "교류전원", "예비전원", "발신기", "축적", "스위치주의", "선로단선" };

#endregion

#region 미드탑 패널

    [SerializeField] private List<SwitchButton> switchButtons = new List<SwitchButton>();
    [SerializeField] private NameListScriptableObj switchNameList;
    [SerializeField] private NameListScriptableObj switchModeList;

    public readonly Dictionary<string, SwitchButton> _switchButtonDict = new Dictionary<string, SwitchButton>();
    public UnityEvent<string, bool> onSwitchBtnValueChangeEvent;
    /*
    private readonly string[] _switchName =
    {
        "주경종", "지구경종", "사이렌", "비상방송", "부저", "축적/비축적", "예비전원",
        "유도등", "도통시험", "동작시험", "자동복구", "복구"
    };
    private readonly string[] _switchMode1 =
    {
        "주경종", "지구경종", "사이렌", "비상방송", "부저", "축적/비축적"
    };
    */

#endregion //미드탑 패널

#region 수신기록장치

    [SerializeField] private ReceiverLogPanel receiverLogPanel;

#endregion

#region 컨트롤 모드 패널

    [SerializeField] private ControlModePanel solenoidValveControl;
    [SerializeField] private ControlModePanel area1Control;
    [SerializeField] private ControlModePanel area2Control;

#endregion //컨트롤 모드 패널
    private void OnEnable()
    {
        if (null != Instance)
            return;
        Instance = this;
    }

    public void ShowPanel(bool show)
    {
        objPanel.SetActive(show);
        GetSwitchButton("복구").OnCheck(false);
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (null != Instance)
            return;
        Instance = this;
    }

    public void Init()
    {
        _fireSoundCheck ??= new FireSoundCheck();
        InitTopPanel();
        InitSidePanel();
        InitMidTopPanel();
        InitSolenoidValveControl();
        InitArea1Control();
        InitArea2Control();
        soundCheckAction?.RemoveAllListeners();
        OnTimerStart?.RemoveAllListeners();
        OnTimerEnd?.RemoveAllListeners();
        ControlPanelButtonAction?.RemoveAllListeners();
        soundCheckAction?.AddListener(PlaySound);
        OnTimerStart?.AddListener(PlaySiren);
        ShowPanel(true);
        ShowFire(false);
        if (defaultParent)
        {
            if (!transform.parent.Equals(defaultParent))
            {
                transform.SetParent(defaultParent);
            }
        }
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.gameObject.SetActive(false);
    }

    private void PlaySound(FireSoundCheck fireSoundCheck)
    {
        if (!fireImg.activeSelf)
            return;

        SoundManager.Instance.PlayAlarm(fireSoundCheck.alarm);
        SoundManager.Instance.PlayAlarm2(fireSoundCheck.alarm2);
        SoundManager.Instance.PlaySiren(fireSoundCheck.siren);
        SoundManager.Instance.PlayBroadcast(fireSoundCheck.broadCast);
        SoundManager.Instance.PlayBuzzer(fireSoundCheck.buzzer);
    }

    private void PlaySiren()
    {
        if (!fireImg.activeSelf)
            return;
        _area1Dict.TryGetValue(areaNameList.korNames[(int)EAreaName.ActivateSolenoidValve], out var activateSolenoidValve);
        _area1Dict.TryGetValue(areaNameList.korNames[(int)EAreaName.VerifyDischarge], out var verifyDischarge);
        /*
        if (verifyDischarge != null)
            _fireSoundCheck.siren = !_switchButtonDict["사이렌"].IsChecked() && activateSolenoidValve != null && (timerPanelObj.IsRunning() || activateSolenoidValve.IsChecked() || verifyDischarge.IsChecked());

        */
        _fireSoundCheck.siren = (activateSolenoidValve != null && verifyDischarge != null && (timerPanelObj.IsRunning() || activateSolenoidValve.IsChecked() || verifyDischarge.IsChecked()));
        SoundManager.Instance.PlaySiren(_fireSoundCheck.siren);
    }

    public bool SirenCheck()
    {
        _area1Dict.TryGetValue(areaNameList.korNames[(int)EAreaName.ActivateSolenoidValve], out var activateSolenoidValve);
        _area1Dict.TryGetValue(areaNameList.korNames[(int)EAreaName.VerifyDischarge], out var verifyDischarge);
        bool siren = (activateSolenoidValve != null && verifyDischarge != null && (timerPanelObj.IsRunning() || activateSolenoidValve.IsChecked() || verifyDischarge.IsChecked()));

        return siren;
    }

#region 점검 전 안전 조치

    public void InitSafetyCheck()
    {
        Init();
        InitSolenoidValveControl();
    }

    public void InitNewSafetCheck(UnityAction closeAction)
    {
        Init();
        InitSolenoidValveControl();
        SoundCheck();
        closeBtn.onClick.AddListener(delegate
        {
            closeAction?.Invoke();
            ShowPanel(false);
        });
        SafetyCheckAction = safetCheck =>
        {
            closeBtn.gameObject.SetActive(safetCheck);
        };
    }

    private void SafetCheckState()
    {
        string[] safetModeStr = new[]
        {
            "주경종", "지구경종",
            "사이렌", "비상방송",
            "부저", "축적/비축적"
        };

        bool safetyCheck = true;
        foreach (string switchBtn in safetModeStr)
        {
            if (null == _switchButtonDict || !_switchButtonDict.TryGetValue(switchBtn, out var obj))
                continue;
            if (obj.IsChecked())
                continue;
            safetyCheck = false;
            break;
        }

        SafetyCheckAction?.Invoke(safetyCheck && ControlMode.Stop.Equals(solenoidValveControl.GetMode()));
    }

#endregion // 점검 전 안전 조치

#region 기동용기 솔레노이드 밸브 격발시힘 -> 수동조작버튼작동[즉시격발]

    /// <summary>
    /// 기동용기 솔레노이드 밸브 격발시험 -> 수동조작버튼작동[즉시격발]
    /// </summary>
    public void InitManualOperationController()
    {
        // InitTopPanel();
        // InitSidePanel();
        // InitMidTopPanel();
        // InitArea1Control();
        // InitArea2Control();
        Init();
        InitSolenoidValveControl(ControlMode.Stop);
        _switchButtonDict["주경종"].OnCheck(true);
        _switchButtonDict["지구경종"].OnCheck(true);
        _switchButtonDict["사이렌"].OnCheck(true);
        _switchButtonDict["비상방송"].OnCheck(true);
        _switchButtonDict["부저"].OnCheck(true);
        _switchButtonDict["축적/비축적"].OnCheck(true);
        CheckWarringSwitch();
        //ShowPanel(true);

    }

    public void InitNewManualOperationController(UnityAction closeAction, UnityAction<ControlMode> modeAction)
    {
        // InitTopPanel();
        // InitSidePanel();
        // InitMidTopPanel();
        // InitArea1Control();
        // InitArea2Control();
        Init();
        transform.SetParent(storageRoomPopupParent);
        InitSolenoidValveControl(ControlMode.Stop);
        _switchButtonDict["주경종"].OnCheck(true);
        _switchButtonDict["지구경종"].OnCheck(true);
        _switchButtonDict["사이렌"].OnCheck(true);
        _switchButtonDict["비상방송"].OnCheck(true);
        _switchButtonDict["부저"].OnCheck(true);
        _switchButtonDict["축적/비축적"].OnCheck(true);
        CheckWarringSwitch();
        SoundCheck();

        closeBtn.onClick.AddListener(delegate
        {
            closeAction?.Invoke();
            ShowPanel(false);
        });
        solenoidValveControl.SetModeChangeAction(mode =>
        {
            modeAction?.Invoke(mode);
            if (ControlMode.Auto.Equals(mode))
            {
                //closeBtn.gameObject.SetActive(true);
            }
        });
        //ShowPanel(true);
    }

#endregion 기동용기 솔레노이드 밸브 격발시힘 -> 수동조작버튼작동[즉시격발]
    public void InitManualControlBox()
    {
        Init();
        InitSolenoidValveControl(ControlMode.Stop);
        _switchButtonDict["주경종"].OnCheck(true);
        _switchButtonDict["지구경종"].OnCheck(true);
        _switchButtonDict["사이렌"].OnCheck(true);
        _switchButtonDict["비상방송"].OnCheck(true);
        _switchButtonDict["부저"].OnCheck(true);
        _switchButtonDict["축적/비축적"].OnCheck(true);
        CheckWarringSwitch();
        SetTimeNum(0);
        ShowFire(false);
    }

    public void InitNewManualControlBox(UnityAction closeAction, UnityAction<ControlMode> modeAction)
    {
        Init();
        InitSolenoidValveControl(ControlMode.Stop);
        _switchButtonDict["주경종"].OnCheck(true);
        _switchButtonDict["지구경종"].OnCheck(true);
        _switchButtonDict["사이렌"].OnCheck(true);
        _switchButtonDict["비상방송"].OnCheck(true);
        _switchButtonDict["부저"].OnCheck(true);
        _switchButtonDict["축적/비축적"].OnCheck(true);
        CheckWarringSwitch();
        SetTimeNum(0);
        ShowFire(false);
        SoundCheck();
        closeBtn.onClick.AddListener(delegate
        {
            closeAction?.Invoke();
            ShowPanel(false);
            transform.SetParent(area1CorridorPopupParent);
        });
        solenoidValveControl.SetModeChangeAction(mode =>
        {
            ManualControlBoxState();
            modeAction?.Invoke(mode);
            if (ControlMode.Auto.Equals(mode))
            {
                //closeBtn.gameObject.SetActive(true);
            }
        });

        ManualControlBoxAction = check =>
        {
            closeBtn.gameObject.SetActive(check);
        };
    }


    private void ManualControlBoxState()
    {
        string[] safetModeStr = new[]
        {
            "주경종", "지구경종",
            "사이렌", "비상방송",
            "부저"
        };

        bool isCheck = true;
        foreach (string switchBtn in safetModeStr)
        {
            if (null == _switchButtonDict || !_switchButtonDict.TryGetValue(switchBtn, out var obj))
                continue;
            if (!obj.IsChecked())
                continue;
            isCheck = false;
            break;
        }

        ManualControlBoxAction?.Invoke(isCheck && ControlMode.Auto.Equals(solenoidValveControl.GetMode()));
    }

    public void InitCrossCircuitDetector()
    {
        Init();
        InitSolenoidValveControl(ControlMode.Stop);
        _switchButtonDict["주경종"].OnCheck(true);
        _switchButtonDict["지구경종"].OnCheck(true);
        _switchButtonDict["사이렌"].OnCheck(true);
        _switchButtonDict["비상방송"].OnCheck(true);
        _switchButtonDict["부저"].OnCheck(true);
        _switchButtonDict["축적/비축적"].OnCheck(true);
        CheckWarringSwitch();
        SetTimeNum(0);
        ShowFire(false);
    }

    public void InitNewCrossCircuitDetector(UnityAction closeAction, UnityAction<ControlMode> modeAction)
    {
        Init();
        InitSolenoidValveControl(ControlMode.Stop);
        _switchButtonDict["주경종"].OnCheck(true);
        _switchButtonDict["지구경종"].OnCheck(true);
        _switchButtonDict["사이렌"].OnCheck(true);
        _switchButtonDict["비상방송"].OnCheck(true);
        _switchButtonDict["부저"].OnCheck(true);
        _switchButtonDict["축적/비축적"].OnCheck(true);
        CheckWarringSwitch();
        SetTimeNum(0);
        ShowFire(false);
        SoundCheck();

        closeBtn.onClick.AddListener(delegate
        {
            closeAction?.Invoke();
            ShowPanel(false);
            transform.SetParent(area1CorridorPopupParent);
        });
        solenoidValveControl.SetModeChangeAction(mode =>
        {
            ManualControlBoxState();
            modeAction?.Invoke(mode);
            if (ControlMode.Auto.Equals(mode))
            {
                //closeBtn.gameObject.SetActive(true);
            }
        });

        ManualControlBoxAction = check =>
        {
            closeBtn.gameObject.SetActive(check);
        };
        ShowPanel(false);
    }

    public void InitControlPanelSwitch()
    {
        Init();
        InitSolenoidValveControl(ControlMode.Stop);
        _switchButtonDict["주경종"].OnCheck(true);
        _switchButtonDict["지구경종"].OnCheck(true);
        _switchButtonDict["사이렌"].OnCheck(true);
        _switchButtonDict["비상방송"].OnCheck(true);
        _switchButtonDict["부저"].OnCheck(true);
        _switchButtonDict["축적/비축적"].OnCheck(true);
        //SoundCheck();
        CheckWarringSwitch();
        SetTimeNum(0);
        ShowFire(false);
    }

    public void InitNewControlPanelSwitch()
    {
        Init();
        InitSolenoidValveControl(ControlMode.Stop);
        _switchButtonDict["주경종"].OnCheck(true);
        _switchButtonDict["지구경종"].OnCheck(true);
        _switchButtonDict["사이렌"].OnCheck(true);
        _switchButtonDict["비상방송"].OnCheck(true);
        _switchButtonDict["부저"].OnCheck(true);
        _switchButtonDict["축적/비축적"].OnCheck(true);
        SoundCheck();
        CheckWarringSwitch();
        SetTimeNum(0);
        ShowFire(false);
    }

    public void InitDischargeIndicatorLightTest()
    {
        Init();
        InitSolenoidValveControl();
        //_switchButtonDict["주경종"].OnCheck(true);
        //_switchButtonDict["지구경종"].OnCheck(true);
        //_switchButtonDict["사이렌"].OnCheck(true);
        //_switchButtonDict["비상방송"].OnCheck(true);
        //_switchButtonDict["부저"].OnCheck(true);
        _switchButtonDict["축적/비축적"].OnCheck(true);
        CheckWarringSwitch();
        SetTimeNum(0);
        ShowFire(false);
        _switchButtonDict["주경종"].OnCheck(false);
    }

    public void InitNewDischargeCheck()
    {
        Init();
        InitSolenoidValveControl();
        _switchButtonDict["축적/비축적"].OnCheck(true);
        CheckWarringSwitch();
        SetTimeNum(0);
        ShowFire(false);
        _switchButtonDict["주경종"].OnCheck(false);
        SetArea1Check(EAreaName.VerifyDischarge, true);
        SoundCheck();
        closeBtn.gameObject.SetActive(true);
        closeBtn.onClick.AddListener(delegate
        {
            ShowPanel(false);
        });
        ShowPanel(false);
    }

    public void InitRecoveryCheck()
    {
        Init();
        InitSolenoidValveControl();
        _switchButtonDict["지구경종"].OnCheck(true);
        _switchButtonDict["사이렌"].OnCheck(true);
        _switchButtonDict["비상방송"].OnCheck(true);
        _switchButtonDict["부저"].OnCheck(true);
        _switchButtonDict["축적/비축적"].OnCheck(true);
        CheckWarringSwitch();
        SetArea1Check(EAreaName.ActivateSolenoidValve, true);
        SetArea1Check(EAreaName.ActivateManualControlBox, true);
        SetTimeNum(0);
        ShowFire(true);
        _switchButtonDict["주경종"].OnCheck(true);
    }

    public void InitNewRecoveryCheck(UnityAction closeAction)
    {
        Init();
        InitSolenoidValveControl();
        _switchButtonDict["지구경종"].OnCheck(true);
        _switchButtonDict["사이렌"].OnCheck(true);
        _switchButtonDict["비상방송"].OnCheck(true);
        _switchButtonDict["부저"].OnCheck(true);
        _switchButtonDict["축적/비축적"].OnCheck(true);
        CheckWarringSwitch();
        SetArea1Check(EAreaName.ActivateSolenoidValve, true);
        SetArea1Check(EAreaName.ActivateManualControlBox, true);
        SetTimeNum(0);
        ShowFire(true);
        _switchButtonDict["주경종"].OnCheck(true);
        SoundCheck();
        RecoveryCheckAction = check =>
        {
            closeBtn.gameObject.SetActive(check);
        };
        closeBtn.onClick.AddListener(delegate
        {
            ShowPanel(false);
            closeAction?.Invoke();
        });
    }

    private void RecoveryCheckState()
    {
        string[] safetModeStr = new[]
        {
            "주경종", "지구경종",
            "사이렌", "비상방송",
            "부저", "축적/비축적"
        };

        bool isCheck = true;
        foreach (string switchBtn in safetModeStr)
        {
            if (null == _switchButtonDict || !_switchButtonDict.TryGetValue(switchBtn, out var obj))
                continue;
            if (!obj.IsChecked())
                continue;
            isCheck = false;
            break;
        }

        RecoveryCheckAction?.Invoke(isCheck && ControlMode.Auto.Equals(solenoidValveControl.GetMode()));
    }

    public void SoundCheck()
    {
        string[] safetModeStr = new[]
        {
            "주경종", "지구경종",
            "사이렌", "비상방송",
            "부저"
        };

        bool siren = false;
        _area1Dict.TryGetValue(areaNameList.korNames[(int)EAreaName.ActivateSolenoidValve], out var activateSolenoidValve);
        _area1Dict.TryGetValue(areaNameList.korNames[(int)EAreaName.ActivateManualControlBox], out var activateManualControlBox);
        if (activateSolenoidValve != null)
            if (activateManualControlBox != null)
                siren = !_switchButtonDict["사이렌"].IsChecked() && (activateSolenoidValve.IsChecked() || activateManualControlBox.IsChecked());

        siren = SirenCheck();
        // if (null != _area1Dict && _area1Dict.TryGetValue(areaNameList.korNames[(int)key], out var obj))
        // {
        //     obj.OnCheck(check);
        // }

        SoundManager.Instance.MuteAlarm(_switchButtonDict["주경종"].IsChecked());
        SoundManager.Instance.MuteAlarm2(_switchButtonDict["지구경종"].IsChecked());
        SoundManager.Instance.MuteSiren(_switchButtonDict["사이렌"].IsChecked());
        SoundManager.Instance.MuteBuzzer(_switchButtonDict["부저"].IsChecked());
        SoundManager.Instance.MuteBroadcast(_switchButtonDict["비상방송"].IsChecked());
        _fireSoundCheck.alarm = !_switchButtonDict["주경종"].IsChecked();
        _fireSoundCheck.alarm2 = !_switchButtonDict["지구경종"].IsChecked();
        _fireSoundCheck.siren = !_switchButtonDict["사이렌"].IsChecked();
        _fireSoundCheck.buzzer = !_switchButtonDict["부저"].IsChecked();
        _fireSoundCheck.broadCast = !_switchButtonDict["비상방송"].IsChecked();
        soundCheckAction.Invoke(_fireSoundCheck);
    }




    public void SetStorageRoomPopupParent()
    {
        if (!storageRoomPopupParent)
            return;
        if (!transform.parent.Equals(storageRoomPopupParent))
            transform.SetParent(storageRoomPopupParent);
    }

    public void SetArea1CorridorPopupParent()
    {
        if (!area1CorridorPopupParent)
            return;
        if (!transform.parent.Equals(area1CorridorPopupParent))
            transform.SetParent(area1CorridorPopupParent);
    }

    public void SetArea1PopupParent()
    {
        if (!area1PopupParent)
            return;
        if (!transform.parent.Equals(area1PopupParent))
            transform.SetParent(area1PopupParent);
    }

#region 탑 패널

    private void InitTopPanel()
    {
        _area1Dict.Clear();
        _area2Dict.Clear();
        // foreach (var panel in area1Panels)
        // {
        //     panel.InitTwoLine(false);
        //     _area1Dict.Add(Util.GetObjectName(panel.name), panel);
        // }
        
        foreach (var panel in area1Panels.Select((value, index) => new { value, index }))
        {
            panel.value.InitTwoLine(false);
            _area1Dict.Add(areaNameList.korNames[panel.index], panel.value);
        }

        foreach (var panel in area2Panels.Select((value, index) => new { value, index }))
        {
            panel.value.InitTwoLine(false);
            _area2Dict.Add(areaNameList.korNames[panel.index], panel.value);
        }
        InitTimer();
    }

    public void InitTimer()
    {
        timerPanelObj.Init();
    }

    public void SetTimeNum(float timeNum)
    {
        timerPanelObj.SetTimeNum(timeNum);
    }

    public void StartTimer(float timeNum)
    {
        timerPanelObj.StartTimer(timeNum);
    }

    public void ResetTimer()
    {
        timerPanelObj.ResetTimer();
    }

    public void SetArea1Check(int[] nums, bool check)
    {
        if (null == nums || 0 == nums.Length)
        {
            Debug.LogWarning("The names array is null or empty.");
            return;
        }

        foreach (int num in nums)
        {
            if (null != _area1Dict && _area1Dict.TryGetValue(areaNameList.korNames[num], out var obj))
            {
                obj.OnCheck(check);
            }
        }
    }

    public void SetArea1Check(EAreaName key, bool check)
    {
        bool siren = false;

        switch (key)
        {

            case EAreaName.Detector1:
                break;
            case EAreaName.Detector2:
                break;
            case EAreaName.ActivateSolenoidValve:
                siren = check && !_switchButtonDict["사이렌"].IsChecked();
                break;
            case EAreaName.VerifyDischarge:
                break;
            case EAreaName.ActivateManualControlBox:
                siren = check && !_switchButtonDict["사이렌"].IsChecked();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(key), key, null);
        }

        if (null != _area1Dict && _area1Dict.TryGetValue(areaNameList.korNames[(int)key], out var obj))
        {
            obj.OnCheck(check);
        }
        _fireSoundCheck.alarm = !_switchButtonDict["주경종"].IsChecked();
        _fireSoundCheck.alarm2 = !_switchButtonDict["지구경종"].IsChecked();
        _fireSoundCheck.siren = siren;
        _fireSoundCheck.buzzer = !_switchButtonDict["부저"].IsChecked();
        _fireSoundCheck.broadCast = !_switchButtonDict["비상방송"].IsChecked();
        soundCheckAction.Invoke(_fireSoundCheck);
        // SoundCheckAction.Invoke(!_switchButtonDict["주경종"].IsChecked() || !_switchButtonDict["지구경종"],
        //     siren, !_switchButtonDict["비상방송"].IsChecked(), !_switchButtonDict["부저"].IsChecked());
    }

    public bool CheckDetector1()
    {
        _area1Dict.TryGetValue(areaNameList.korNames[(int)EAreaName.Detector1], out var obj);
        return obj != null && obj.IsChecked();
    }

    public bool CheckDetector2()
    {
        _area1Dict.TryGetValue(areaNameList.korNames[(int)EAreaName.Detector2], out var obj);
        return obj != null && obj.IsChecked();
    }

    public bool CheckVerifyDischarge()
    {
        _area1Dict.TryGetValue(areaNameList.korNames[(int)EAreaName.VerifyDischarge], out var obj);
        return obj != null && obj.IsChecked();
    }

    public void SetArea1Check(string[] keys, bool check)
    {
        if (null == keys || 0 == keys.Length)
        {
            Debug.LogWarning("The names array is null or empty.");
            return;
        }

        foreach (string key in keys)
        {
            if (null != _area1Dict && _area1Dict.TryGetValue(key, out var obj))
            {
                obj.OnCheck(check);
            }
        }
    }

    public void SetArea1Check(string key, bool check)
    {
        if (null != _area1Dict && _area1Dict.TryGetValue(key, out var obj))
        {
            obj.OnCheck(check);
        }
    }

    public void SetArea2Check(int[] nums, bool check)
    {
        if (null == nums || 0 == nums.Length)
        {
            Debug.LogWarning("The names array is null or empty.");
            return;
        }

        foreach (int num in nums)
        {
            if (null != _area2Dict && _area2Dict.TryGetValue(areaNameList.korNames[num], out var obj))
            {
                obj.OnCheck(check);
            }
        }
    }

    public void SetArea2Check(EAreaName key, bool check)
    {
        if (null != _area2Dict && _area2Dict.TryGetValue(areaNameList.korNames[(int)key], out var obj))
        {
            obj.OnCheck(check);
        }
    }

    public void SetArea2Check(string[] keys, bool check)
    {
        if (null == keys || 0 == keys.Length)
        {
            Debug.LogWarning("The names array is null or empty.");
            return;
        }

        foreach (string key in keys)
        {
            if (null != _area2Dict && _area2Dict.TryGetValue(key, out var obj))
            {
                obj.OnCheck(check);
            }
        }
    }

    public void SetArea2Check(string key, bool check)
    {
        if (null != _area2Dict && _area2Dict.TryGetValue(key, out var obj))
        {
            obj.OnCheck(check);
        }
    }

    private void ManualRecovery()
    {
        ShowFire(false);

        foreach (var panel in _area1Dict.Where(panel => !areaNameList.korNames[(int)EAreaName.ActivateSolenoidValve].Equals(panel.Key)))
        {
            panel.Value.OnCheck(false);
        }

        foreach (var panel in _area2Dict.Where(panel => !areaNameList.korNames[(int)EAreaName.ActivateSolenoidValve].Equals(panel.Key)))
        {
            panel.Value.OnCheck(false);
        }
        /*
        foreach (var panel in area1Panels)
        {
            //panel.InitTwoLine();
            panel.OnCheck(false);
        }
        foreach (var panel in area2Panels)
        {
            panel.OnCheck(false);
        }
        */
    }

#endregion //탑 패널

#region 사이드 패널

    private void InitSidePanel()
    {
        _sideDict.Clear();
        foreach (var panel in sidePanels)
        {
            panel.InitOneLine(false);
            _sideDict.Add(Util.GetObjectName(panel.name), panel);
        }
        _sideDict[sideNameList.korNames[0]].OnCheck(true);
        voltageIndicatorPanel.Init(VoltageIndicatorPanel.VoltageState.Default);
    }

    public void CheckWarringSwitch()
    {
        if (null == switchModeList.korNames || 0 == switchModeList.korNames.Length)
        {
            Debug.LogWarning("The names array is null or empty.");
            return;
        }

        bool check = false;
        foreach (string switchBtn in switchModeList.korNames)
        {
            if (null == _switchButtonDict || !_switchButtonDict.TryGetValue(switchBtn, out var obj))
                continue;
            if (!obj.IsChecked())
                continue;
            check = true;
            break;
        }
        if (_switchButtonDict != null)
        {
            _switchButtonDict.TryGetValue("축적/비축적", out var temp);
            _sideDict["축적"].OnCheck(temp != null && temp.IsChecked());
        }

        _sideDict["스위치주의"].OnCheck(check);
    }

    public void ShowFire(bool show, bool soundCheck = true)
    {
        if (show)
        {
            _switchButtonDict["주경종"].OnCheck(false);
        }
        fireImg.SetActive(show);
        if(soundCheck)
            SoundCheck();
    }

#endregion

#region 미드탑 패널

    private void InitMidTopPanel()
    {
        _switchButtonDict.Clear();

        foreach (var panel in switchButtons)
        {
            panel.Init(SwitchButtonClick, false);
            var panelName = Util.RemoveWhitespaceUsingRegex(Util.GetObjectName(panel.name));
            _switchButtonDict.Add(panelName, panel);
        }
        //SetMode(switchModeList.korNames);
    }

    public void CheckSwitchBtn()
    {
        controlPanelButton.주경종 = GetSwitchButton("주경종").IsChecked();
        controlPanelButton.지구경종 = GetSwitchButton("지구경종").IsChecked();
        controlPanelButton.사이렌 = GetSwitchButton("사이렌").IsChecked();
        controlPanelButton.비상방송 = GetSwitchButton("비상방송").IsChecked();
        controlPanelButton.부저 = GetSwitchButton("부저").IsChecked();
        controlPanelButton.축적 = GetSwitchButton("축적/비축적").IsChecked();
        controlPanelButton.예비전원 = GetSwitchButton("예비전원").IsChecked();
        controlPanelButton.유도등 = GetSwitchButton("유도등").IsChecked();
        controlPanelButton.도통시험 = GetSwitchButton("도통시험").IsChecked();
        controlPanelButton.자동복구 = GetSwitchButton("자동복구").IsChecked();
        controlPanelButton.복구 = GetSwitchButton("복구").IsChecked();
        ControlPanelButtonAction?.Invoke(controlPanelButton);
    }
    private void SwitchButtonClick(string btnName, bool isChecked)
    {
        onSwitchBtnValueChangeEvent?.Invoke(btnName, isChecked);
        CheckSwitchBtn();
        CheckWarringSwitch();
        SafetCheckState();
        ManualControlBoxState();
        RecoveryCheckState();
        SoundCheck();
        btnName = Util.RemoveWhitespaceUsingRegex(Util.GetObjectName(btnName));

        if (btnName.Equals("축적/비축적"))
        {
            _sideDict["축적"].OnCheck(controlPanelButton.축적);
        }

        if (btnName.Equals("복구"))
        {
            RecoverySwitchEvent?.Invoke();
            ManualRecovery();
        }
        Debug.Log(btnName);
    }

    public void SetMode(string[] mode)
    {
        if (null == mode || 0 == mode.Length)
        {
            Debug.LogWarning("The names array is null or empty.");
            return;
        }
        foreach (var obj in _switchButtonDict)
        {
            obj.Value.SetMode(false);
        }

        foreach (string objName in mode)
        {
            if (null != _switchButtonDict && _switchButtonDict.TryGetValue(objName, out var obj))
            {
                obj.SetMode(true);
            }
        }
    }

#endregion //미드탑 패널

#region 리시브 패널

    public void SetReceiverLog(string msg)
    {
        receiverLogPanel.SetLog(msg);
    }

    public void ClearReceiverLog()
    {
        receiverLogPanel.ClearLogText();
    }

#endregion //리시브 패널

#region 컨트롤 모드 패널

    public void InitSolenoidValveControl(UnityAction<ControlMode> action, ControlMode mode = ControlMode.Auto)
    {
        solenoidValveControl.Init(mode, action);
    }
    public void InitSolenoidValveControl(ControlMode mode = ControlMode.Auto)
    {
        controlPanelButton.솔레노이드밸브 = mode;
        solenoidValveControl.Init(mode, UpdateSolenoidValveMode);
    }

    public void InitArea1Control(ControlMode mode = ControlMode.Auto)
    {
        controlPanelButton.구역1 = mode;
        area1Control.Init(mode, UpdateArea1ControlMode, Area1Activate);
    }

    public void InitArea2Control(ControlMode mode = ControlMode.Auto)
    {
        controlPanelButton.구역2 = mode;
        area2Control.Init(mode, UpdateArea2ControlMode, Area2Activate);
    }

    public void SetSolenoidValveModeAndActivateBtn(UnityAction<ControlMode> action, UnityAction btnAction)
    {
        solenoidValveControl.SetModeChangeAction(action);
        solenoidValveControl.SetActivateBtn(btnAction);
    }

    public void SetSolenoidValveMode(UnityAction<ControlMode> action)
    {
        solenoidValveControl.SetModeChangeAction(action);
    }

    public void SetArea1ModeAndActivateBtn(UnityAction<ControlMode> action, UnityAction btnAction)
    {
        area1Control.SetModeChangeAction(action);
        area1Control.SetActivateBtn(btnAction);
    }

    public void SetArea2ModeAndActivateBtn(UnityAction<ControlMode> action, UnityAction btnAction)
    {
        area2Control.SetModeChangeAction(action);
        area2Control.SetActivateBtn(btnAction);
    }

    private void UpdateSolenoidValveMode(ControlMode mode)
    {
        SafetCheckState();
        controlPanelButton.솔레노이드밸브 = mode;
        ControlPanelButtonAction?.Invoke(controlPanelButton);
        Debug.Log($"eSolenoidValve {mode}");
    }

    private void UpdateArea1ControlMode(ControlMode mode)
    {
        controlPanelButton.구역1 = mode;
        ControlPanelButtonAction?.Invoke(controlPanelButton);
        Debug.Log($"Area1 {mode}");
    }

    private void UpdateArea2ControlMode(ControlMode mode)
    {
        controlPanelButton.구역2 = mode;
        ControlPanelButtonAction?.Invoke(controlPanelButton);
        Debug.Log($"Area2 {mode}");
    }

    private void Area1Activate()
    {
        controlPanelButton.구역1기동 = true;
        ControlPanelButtonAction?.Invoke(controlPanelButton);
        Debug.Log($"Area1 기동");
    }

    private void Area2Activate()
    {
        Debug.Log($"Area2 기동");
    }

#endregion // 컨트롤 모드 패널

    public SwitchButton GetSwitchButton(string btnName)
    {
        return _switchButtonDict[btnName];
    }

    public void DisableAllButton()
    {
        foreach (var obj in _switchButtonDict.Values)
        {
            obj.GetButton().interactable = false;
        }
        GetSolenoidSelectBtn().interactable = false;
        GetArea1SelectBtn().interactable = false;
        GetArea1ActivateBtn().interactable = false;
        GetArea2SelectBtn().interactable = false;
        GatArea2ActivateBtn().interactable = false;
    }

    public Button GetSolenoidSelectBtn()
    {
        return solenoidValveControl.GetSelectBtn();
    }

    public Button GetSolenoidActivateBtn()
    {
        return solenoidValveControl.GetActivateBtn();
    }

    public Button GetArea1SelectBtn()
    {
        return area1Control.GetSelectBtn();
    }

    public Button GetArea1ActivateBtn()
    {
        return area1Control.GetActivateBtn();
    }

    public Button GetArea2SelectBtn()
    {
        return area2Control.GetSelectBtn();
    }

    public Button GatArea2ActivateBtn()
    {
        return area2Control.GetActivateBtn();
    }

    public void SetClose()
    {
        closeBtn.gameObject.SetActive(true);
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(delegate
        {
            ShowPanel(false);
        });
    }

    public ConfirmationPanel GetTimerAbort()
    {
        return timerAbort;
    }

}
