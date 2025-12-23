using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LJS;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public partial class RTypeRPanel : MonoBehaviour
{
#region singleton

    private static RTypeRPanel instance;
    public static RTypeRPanel Instance { get { return instance; } }

#endregion

    [Foldout("메인화면")]
    [SerializeField] public GameObject mainScreenImg;
    [SerializeField] public GameObject fireScreen1Img;
    [SerializeField] public GameObject fireScreen2Img;
    [FormerlySerializedAs("sbScreen1Img")]
    [SerializeField] public GameObject cbScreen1Img;

    [Foldout("화재경보 화면")]
    [SerializeField] public GameObject fireAlarmMapImg;
    [SerializeField] public GameObject fireAlarmMap2Img;

    [FormerlySerializedAs("equmentOperationEmptyListImg")]
    [FormerlySerializedAs("equmentOperationmptyListImg")]
    [Foldout("설비동작 화면")]
    [SerializeField] public GameObject equipmentOperationEmptyListImg;
    [FormerlySerializedAs("equmentOperationarea1ListImg")]
    [SerializeField] public GameObject equipmentOperationArea1ListImg;
    [SerializeField] public GameObject equipmentOperationArea1ListHighlightiImg;
    [FormerlySerializedAs("equmentOperationarea2ListImg")]
    [SerializeField] public GameObject equipmentOperationArea2ListImg;

    [Foldout("회로차단 화면")]
    [SerializeField] public GameObject cb1Img;
    [SerializeField] public GameObject cb2Img;
    [SerializeField] public GameObject cbAreaImg;
    [SerializeField] public Button cb1ScreenBtn;
    [SerializeField] public Button cb2ScreenBtn;
    [SerializeField] public GameObject cb2ListImg;
    //[SerializeField] public TextMeshProUGUI cb2ListText;

    [FormerlySerializedAs("searchObj")]
    [Foldout("운영기록")]
    [SerializeField] public GameObject operationSearchObj;
    [FormerlySerializedAs("resultObj")]
    [SerializeField] public GameObject operationResultObj;


    [Foldout("위")]
    [SerializeField] public Button initScreenBtn;
    [SerializeField] public Button operationLogBtn;
    [SerializeField] public Button relayControlBtn;
    [SerializeField] public Button systemSettingsBtn;

    [Foldout("아래")]
    [SerializeField] public List<SwitchButton> switchButtons = new List<SwitchButton>();
    [SerializeField] private NameListScriptableObj switchNameList;
    [SerializeField] private NameListScriptableObj switchModeList;
    private readonly Dictionary<string, SwitchButton> _switchButtonDict = new Dictionary<string, SwitchButton>();
    [HideInInspector] public UnityEvent<string, bool> onSwitchBtnValueChangeEvent;

    [Foldout("왼쪽")]
    [SerializeField] public RTypeRPanelBtn faBtn;
    [SerializeField] public RTypeRPanelBtn cbBtn;
    [SerializeField] public RTypeRPanelBtn eoBtn;

    [Foldout("오른쪽")]
    [SerializeField] public RTypeRModePanel 소화전주펌프;
    [SerializeField] public RTypeRModePanel 소화전충압펌프;
    [SerializeField] public RTypeRModePanel sp주펌프;
    [SerializeField] public RTypeRModePanel sp충압펌프;

    [SerializeField] public GameObject objPanel;


    private FireSoundCheck _fireSoundCheck;
    public UnityEvent<FireSoundCheck> soundCheckAction;

    static public readonly UnityEvent<RTypeRPanelButtonCheck> RTypeRPanelButtonAction = new UnityEvent<RTypeRPanelButtonCheck>();
    public RTypeRPanelButtonCheck rTypeRPanelButton = new RTypeRPanelButtonCheck();
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
        _fireSoundCheck ??= new FireSoundCheck();
        soundCheckAction?.RemoveAllListeners();
        soundCheckAction?.AddListener(PlaySound);
        InitOperationLog();
        InitTopPanel();
        InitBotPanel();
        InitLPanel();
        InitRPanel();
        ShowScreen(mainScreenImg);
    }

#region 위

    public void InitTopPanel()
    {
        initScreenBtn.onClick.RemoveAllListeners();
        operationLogBtn.onClick.RemoveAllListeners();
        relayControlBtn.onClick.RemoveAllListeners();
        systemSettingsBtn.onClick.RemoveAllListeners();

        initScreenBtn.onClick.AddListener(delegate
        {
            ShowScreen(mainScreenImg);
        });
        operationLogBtn.onClick.AddListener(delegate
        {
            ShowScreen(operationSearchObj);
        });
        relayControlBtn.onClick.AddListener(delegate
        {
        });
        systemSettingsBtn.onClick.AddListener(delegate
        {
        });
    }

    public Button GetInitScreenBtn(Action action = null)
    {
        if (action == null)
            return initScreenBtn;
        initScreenBtn.onClick.RemoveAllListeners();
        initScreenBtn.onClick.AddListener(action.Invoke);
        return initScreenBtn;
    }

    public Button GetOperationLogBtn(Action action = null)
    {
        if (action == null)
            return operationLogBtn;
        operationLogBtn.onClick.RemoveAllListeners();
        operationLogBtn.onClick.AddListener(action.Invoke);
        return operationLogBtn;
    }

    public Button GetRelayControlBtn(Action action = null)
    {
        if (action == null)
            return relayControlBtn;
        relayControlBtn.onClick.RemoveAllListeners();
        relayControlBtn.onClick.AddListener(action.Invoke);
        return relayControlBtn;
    }

    public Button GetSystemSettingsBtn(Action action = null)
    {
        if (action == null)
            return systemSettingsBtn;
        systemSettingsBtn.onClick.RemoveAllListeners();
        systemSettingsBtn.onClick.AddListener(action.Invoke);
        return systemSettingsBtn;
    }

#endregion

#region 아래

    private void InitBotPanel()
    {
        _switchButtonDict.Clear();

        foreach (var panel in switchButtons)
        {
            panel.Init(SwitchButtonClick, false);
            var panelName = Util.RemoveWhitespaceUsingRegex(Util.GetObjectName(panel.name));
            _switchButtonDict.Add(panelName, panel);
        }
    }

    public void SwitchButtonClick(string btnName, bool isChecked)
    {
        onSwitchBtnValueChangeEvent?.Invoke(btnName, isChecked);
        CheckWarringSwitch();
        CheckSwitchBtn();
        // CheckWarringSwitch();
        // SafetCheckState();
        // ManualControlBoxState();
        // RecoveryCheckState();
        SoundCheck();
        btnName = Util.RemoveWhitespaceUsingRegex(Util.GetObjectName(btnName));

        // if (btnName.Equals("축적/비축적"))
        // {
        //     _sideDict["축적"].OnCheck(controlPanelButton.축적);
        // }
        //
        // if (btnName.Equals("복구"))
        // {
        //     RecoverySwitchEvent?.Invoke();
        //     ManualRecovery();
        // }
        Debug.Log(btnName);
    }

    public void CheckSwitchBtn()
    {
        rTypeRPanelButton.checkDic[RTypeRBtnType.예비전원시험] = GetSwitchButton("예비전원시험").IsChecked();
        rTypeRPanelButton.checkDic[RTypeRBtnType.자동복구] = GetSwitchButton("자동복구").IsChecked();
        rTypeRPanelButton.checkDic[RTypeRBtnType.축적소등_비축적점등] = GetSwitchButton("축적소등_비축적점등").IsChecked();
        rTypeRPanelButton.checkDic[RTypeRBtnType.수신기복구] = GetSwitchButton("수신기복구").IsChecked();
        rTypeRPanelButton.checkDic[RTypeRBtnType.방화문정지] = GetSwitchButton("방화문정지").IsChecked();
        rTypeRPanelButton.checkDic[RTypeRBtnType.주음향정지] = GetSwitchButton("주음향정지").IsChecked();
        rTypeRPanelButton.checkDic[RTypeRBtnType.지구음향정지] = GetSwitchButton("지구음향정지").IsChecked();
        rTypeRPanelButton.checkDic[RTypeRBtnType.사이렌정지] = GetSwitchButton("사이렌정지").IsChecked();
        rTypeRPanelButton.checkDic[RTypeRBtnType.비상방송정지] = GetSwitchButton("비상방송정지").IsChecked();
        rTypeRPanelButton.checkDic[RTypeRBtnType.부저정지] = GetSwitchButton("부저정지").IsChecked();


        // rTypeRPanelButton.checkDic["사이렌정지"] = GetSwitchButton("사이렌정지").IsChecked();
        // rTypeRPanelButton.checkDic["비상방송정지"] = GetSwitchButton("비상방송정지").IsChecked();
        // rTypeRPanelButton.checkDic["부저정지"] = GetSwitchButton("부저정지").IsChecked();
        RTypeRPanelButtonAction?.Invoke(rTypeRPanelButton);
    }

    public void SetSwitchButton(bool isChecked)
    {
        GetSwitchButton("예비전원시험").OnCheck(isChecked);
        GetSwitchButton("자동복구").OnCheck(isChecked);
        GetSwitchButton("축적소등_비축적점등").OnCheck(isChecked);
        GetSwitchButton("수신기복구").OnCheck(isChecked);
        GetSwitchButton("방화문정지").OnCheck(isChecked);
        GetSwitchButton("주음향정지").OnCheck(isChecked);
        GetSwitchButton("지구음향정지").OnCheck(isChecked);
        GetSwitchButton("사이렌정지").OnCheck(isChecked);
        GetSwitchButton("비상방송정지").OnCheck(isChecked);
        GetSwitchButton("부저정지").OnCheck(isChecked);
    }

    // public void SetSwitchButtons(bool isChecked, params RTypeRSwitchBtnType[] switchBtnTypes)
    // {
    //     if (null == switchBtnTypes)
    //         return;
    //
    //     foreach (var temp in switchBtnTypes)
    //     {
    //         GetSwitchButton(temp.ToString()).OnCheck(isChecked);
    //     }
    // }
    //
    // public SwitchButton GetSwitchButton(string btnName)
    // {
    //     return _switchButtonDict[btnName];
    // }
    public void SetSwitchButtons(bool isChecked, params RTypeRBtnType[] switchBtnTypes)
    {
        if (switchBtnTypes == null || !switchBtnTypes.Any())
            return;

        // LINQ를 사용하여 각 버튼을 찾아 상태를 설정
        switchBtnTypes
            .Select(temp => GetSwitchButton(temp.ToString())) // 버튼 객체 가져오기
            .Where(button => button != null) // null 검사
            .ToList() // foreach를 위해 리스트로 변환
            .ForEach(button => button.OnCheck(isChecked)); // 상태 설정
    }

    public SwitchButton GetSwitchButton(string btnName)
    {
        return _switchButtonDict.GetValueOrDefault(btnName);
    }

#endregion //아래 왼쪽

#region 좌

    private void InitLPanel()
    {
        faBtn.Init(RTypeRPanelButtonType.화재경보, () =>
        {
            //ShowScreen(fireAlarmMapImg);
        });
        cbBtn.Init(RTypeRPanelButtonType.회로차단, () =>
        {
            //ShowScreen();
        });
        eoBtn.Init(RTypeRPanelButtonType.설비동작, () =>
        {

        });
        SetFireAlarmEventNum(0);
        SetCircuitBreakerEventNum(0);
        SetEquipmentOperationEventNum(0);
    }

    public void SetFireAlarmEventNum(int num, bool prev = false)
    {
        faBtn.SetEventNum(num);
        if (prev)
            return;
        if (0 == num)
        {
            SoundManager.Instance.ZeroVolume();
        }
        else
        {
            _switchButtonDict["주음향정지"].OnCheck(false);
            SoundManager.Instance.SetDefaultVolume();
            SoundCheck();
        }
    }

    public void SetCircuitBreakerEventNum(int num)
    {
        cbBtn.SetEventNum(num);
    }

    public void SetEquipmentOperationEventNum(int num)
    {
        eoBtn.SetEventNum(num);
    }

    public Button GetFireAlarmBtn(Action action = null)
    {
        if (null == action)
            return faBtn.btn;
        faBtn.btn.onClick.RemoveAllListeners();
        faBtn.btn.onClick.AddListener(action.Invoke);
        return faBtn.btn;
    }

    public Button GetCircuitBreakerBtn(Action action = null)
    {
        if (null == action)
            return cbBtn.btn;
        cbBtn.btn.onClick.RemoveAllListeners();
        cbBtn.btn.onClick.AddListener(action.Invoke);
        return cbBtn.btn;
    }

    public Button GetEquipmentOperationBtn(Action action = null)
    {
        if (null == action)
            return eoBtn.btn;
        eoBtn.btn.onClick.RemoveAllListeners();
        eoBtn.btn.onClick.AddListener(action.Invoke);
        return eoBtn.btn;
    }

#endregion

#region 우

    private void InitRPanel(ControlMode mp = ControlMode.Auto, ControlMode rp = ControlMode.Auto,
        ControlMode sp = ControlMode.Auto, ControlMode spr = ControlMode.Auto)
    {
        소화전주펌프.Init(mp, mode =>
        {
            rTypeRPanelButton.checkDic[RTypeRBtnType.메인펌프오토] = ControlMode.Auto == mode;
            rTypeRPanelButton.checkDic[RTypeRBtnType.메인펌프수동] = ControlMode.Manual == mode;
            rTypeRPanelButton.checkDic[RTypeRBtnType.메인펌프정지] = ControlMode.Stop == mode;
            rTypeRPanelButton.mainPump = mode;
            RTypeRPanelButtonAction?.Invoke(rTypeRPanelButton);
        });

        소화전충압펌프.Init(rp, mode =>
        {
            rTypeRPanelButton.checkDic[RTypeRBtnType.충압펌프오토] = ControlMode.Auto == mode;
            rTypeRPanelButton.checkDic[RTypeRBtnType.충압펌프수동] = ControlMode.Manual == mode;
            rTypeRPanelButton.checkDic[RTypeRBtnType.충압펌프정지] = ControlMode.Stop == mode;
            rTypeRPanelButton.jockeyPump = mode;
            RTypeRPanelButtonAction?.Invoke(rTypeRPanelButton);
        });

        sp주펌프.Init(sp, mode =>
        {
            rTypeRPanelButton.checkDic[RTypeRBtnType.sp펌프오토] = ControlMode.Auto == mode;
            rTypeRPanelButton.checkDic[RTypeRBtnType.sp펌프수동] = ControlMode.Manual == mode;
            rTypeRPanelButton.checkDic[RTypeRBtnType.sp펌프정지] = ControlMode.Stop == mode;
            rTypeRPanelButton.spMainPump = mode;
            RTypeRPanelButtonAction?.Invoke(rTypeRPanelButton);
        });
        sp충압펌프.Init(spr, mode =>
        {
            rTypeRPanelButton.checkDic[RTypeRBtnType.sp충압펌프오토] = ControlMode.Auto == mode;
            rTypeRPanelButton.checkDic[RTypeRBtnType.sp충압펌프수동] = ControlMode.Manual == mode;
            rTypeRPanelButton.checkDic[RTypeRBtnType.sp충압펌프정지] = ControlMode.Stop == mode;
            rTypeRPanelButton.spJockeyPump = mode;
            RTypeRPanelButtonAction?.Invoke(rTypeRPanelButton);
        });

    }

#endregion //아래 왼쪽

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

        //_sideDict["스위치주의"].OnCheck(check);
    }

    public void ShowScreen(GameObject obj)
    {
        mainScreenImg.SetActive(mainScreenImg.Equals(obj));
        fireScreen1Img.SetActive(fireScreen1Img.Equals(obj));
        fireScreen2Img.SetActive(fireScreen2Img.Equals(obj));
        fireAlarmMapImg.SetActive(fireAlarmMapImg.Equals(obj));
        fireAlarmMap2Img.SetActive(fireAlarmMap2Img.Equals(obj));
        equipmentOperationEmptyListImg.SetActive(equipmentOperationEmptyListImg.Equals(obj));
        equipmentOperationArea1ListImg.SetActive(equipmentOperationArea1ListImg.Equals(obj));
        equipmentOperationArea2ListImg.SetActive(equipmentOperationArea2ListImg.Equals(obj));
        operationSearchObj.SetActive(operationSearchObj.Equals(obj));
        operationResultObj.SetActive(operationResultObj.Equals(obj));
        cb1Img.SetActive(cb1Img.Equals(obj));
        cb2Img.SetActive(cb2Img.Equals(obj));
        cbAreaImg.SetActive(cbAreaImg.Equals(obj));
        cbScreen1Img.SetActive(cbScreen1Img.Equals(obj));
    }

    private void PlaySound(FireSoundCheck fireSoundCheck)
    {
        SoundManager.Instance.PlayAlarm(fireSoundCheck.alarm);
        SoundManager.Instance.PlayAlarm2(fireSoundCheck.alarm2);
        SoundManager.Instance.PlaySiren(fireSoundCheck.siren);
        SoundManager.Instance.PlayBroadcast(fireSoundCheck.broadCast);
        SoundManager.Instance.PlayBuzzer(fireSoundCheck.buzzer);
    }

    public void SoundCheck()
    {
        string[] safetModeStr = new[]
        {
            "주경종", "지구경종",
            "사이렌", "비상방송",
            "부저"
        };

        // if (null != _area1Dict && _area1Dict.TryGetValue(areaNameList.korNames[(int)key], out var obj))
        // {
        //     obj.OnCheck(check);
        // }

        SoundManager.Instance.MuteAlarm(_switchButtonDict["주음향정지"].IsChecked());
        SoundManager.Instance.MuteAlarm2(_switchButtonDict["지구음향정지"].IsChecked());
        SoundManager.Instance.MuteSiren(_switchButtonDict["사이렌정지"].IsChecked());
        SoundManager.Instance.MuteBuzzer(_switchButtonDict["부저정지"].IsChecked());
        SoundManager.Instance.MuteBroadcast(_switchButtonDict["비상방송정지"].IsChecked());
        // _fireSoundCheck.alarm = _switchButtonDict["주음향정지"].IsChecked();
        // _fireSoundCheck.alarm2 = _switchButtonDict["지구음향정지"].IsChecked();
        // _fireSoundCheck.siren = _switchButtonDict["사이렌정지"].IsChecked();
        // _fireSoundCheck.buzzer = _switchButtonDict["부저정지"].IsChecked();
        // _fireSoundCheck.broadCast = _switchButtonDict["비상방송정지"].IsChecked();
        _fireSoundCheck.alarm = !_switchButtonDict["주음향정지"].IsChecked();
        _fireSoundCheck.alarm2 = !_switchButtonDict["지구음향정지"].IsChecked();
        _fireSoundCheck.siren = !_switchButtonDict["사이렌정지"].IsChecked();
        _fireSoundCheck.buzzer = !_switchButtonDict["부저정지"].IsChecked();
        _fireSoundCheck.broadCast = !_switchButtonDict["비상방송정지"].IsChecked();
        soundCheckAction.Invoke(_fireSoundCheck);
    }

    public Button GetMainPumpAutoBtn()
    {
        return 소화전주펌프.GetAutoBtn();
    }
    public Button GetMainPumpManualBtn()
    {
        return 소화전주펌프.GetManualBtn();
    }
    public Button GetMainPumpStopBtn()
    {
        return 소화전주펌프.GetStopBtn();
    }

    public Button GetJockeyPumpAutoBtn()
    {
        return 소화전충압펌프.GetAutoBtn();
    }
    public Button GetJockeyPumpManualBtn()
    {
        return 소화전충압펌프.GetManualBtn();
    }
    public Button GetJockeyPumpStopBtn()
    {
        return 소화전충압펌프.GetStopBtn();
    }

    public Button GetSpPumpAutoBtn()
    {
        return sp주펌프.GetAutoBtn();
    }
    public Button GetSpPumpManualBtn()
    {
        return sp주펌프.GetManualBtn();
    }
    public Button GetSpPumpStopBtn()
    {
        return sp주펌프.GetStopBtn();
    }

    public Button GetSpJockeyPumpAutoBtn()
    {
        return sp충압펌프.GetAutoBtn();
    }
    public Button GetSpJockeyPumpManualBtn()
    {
        return sp충압펌프.GetManualBtn();
    }
    public Button GetSpJockeyPumpStopBtn()
    {
        return sp충압펌프.GetStopBtn();
    }

#region 중계기제어

    public Button GetCB1ScreenBtn(Action action =null)
    {
        if (null == action)
            return cb1ScreenBtn;
        cb1ScreenBtn.onClick.RemoveAllListeners();
        cb1ScreenBtn.onClick.AddListener(action.Invoke);
        return cb1ScreenBtn;

    }

    public Button GetCB2ScreenBtn(Action action = null)
    {
        if(null == action)
            return cb2ScreenBtn;
        cb2ScreenBtn.onClick.RemoveAllListeners();
        cb2ScreenBtn.onClick.AddListener(action.Invoke);
        return cb2ScreenBtn;
    }

    public void SetCB2ListText()
    {
        //cb2ListText.text = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 중계기 회로 단선";
    }
    

#endregion

    public void ShowPanel(bool show)
    {
        objPanel.SetActive(show);
    }
}
