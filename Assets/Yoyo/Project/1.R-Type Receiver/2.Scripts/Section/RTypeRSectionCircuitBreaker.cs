using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine.UI;
using VInspector;
using RBtnType = RTypeRBtnType;
/// <summary>
/// 회로 차단
/// </summary>
public partial class RTypeRSection
{
    [Foldout("회로차단")]
    private RTypeRCircuitBreakerPSection _curRTypeRCircuitBreakerPSection;
    private RTypeRCircuitBreakerESection _curRTypeRCircuitBreakerESection;
    public HintScriptableObj rTypeRCircuitBreakerHintPObj;
    public HintScriptableObj rTypeRCircuitBreakerHintEObj;
    public Button C2CorrectBtn;
    public Button C2WrongBtn;
    public Button C3CorrectBtn;
    public Button C3WrongBtn;
    public Button C4CorrectBtn;
    public Button C4WrongBtn;
    [EndFoldout]
    public void InitCircuitBreaker()
    {
        Init();

        curMainSection = RTypeRMainSection.CircuitBreaker;
        InitEMode();
        _maxSection = rTypeRState switch
        {
            RTypeRState.PracticeMode => Enum.GetValues(typeof(RTypeRCircuitBreakerPSection)).Length,
            RTypeRState.EvaluationMode => Enum.GetValues(typeof(RTypeRCircuitBreakerESection)).Length,
            _ => _maxSection
        };
        SetSectionRange(0, _maxSection, _maxSection);
        ShowRoom(RTypeRRoomType.RTypeR);
        _soundManager.StopAllFireSound();
        _rPanel.ShowScreen(_rPanel.mainScreenImg);
        _rPanel.SetFireAlarmEventNum(0, true);
        _rPanel.SetEquipmentOperationEventNum(0);
    }
    private void SetCircuitBreakerSection(int index)
    {
        if (rTypeRState.Equals(RTypeRState.PracticeMode))
        {
            prevBtn.gameObject.SetActive(true);
            SetHint(GetHintTextAndAudio(rTypeRCircuitBreakerHintPObj, (int)RTypeRCircuitBreakerPSection.회로단선 + index));
            ChangeStateCircuitBreakerP(RTypeRCircuitBreakerPSection.회로단선 + index);
        }
        if (!rTypeRState.Equals(RTypeRState.EvaluationMode))
            return;
        prevBtn.gameObject.SetActive(false);
        SetHint(GetHintTextAndAudio(rTypeRCircuitBreakerHintEObj, (int)RTypeRCircuitBreakerESection.Init + index));
        ChangeStateCircuitBreakerE(RTypeRCircuitBreakerESection.Init + index);
    }

    public void ChangeStateCircuitBreakerE(RTypeRCircuitBreakerESection state)
    {

        _curRTypeRCircuitBreakerESection = state;
        OnStateChangedCircuitBreakerE(_curRTypeRCircuitBreakerESection);
    }

    public void CBButtonEventResetE()
    {
        C2CorrectBtn.onClick.RemoveAllListeners();
        C2WrongBtn.onClick.RemoveAllListeners();
        C3CorrectBtn.onClick.RemoveAllListeners();
        C3WrongBtn.onClick.RemoveAllListeners();
        C4CorrectBtn.onClick.RemoveAllListeners();
        C4WrongBtn.onClick.RemoveAllListeners();
        C2CorrectBtn.gameObject.SetActive(false);
        C2WrongBtn.gameObject.SetActive(false);
        C3CorrectBtn.gameObject.SetActive(false);
        C3WrongBtn.gameObject.SetActive(false);
        C4CorrectBtn.gameObject.SetActive(false);
        C4WrongBtn.gameObject.SetActive(false);
        RTypeRPanel.RTypeRPanelButtonAction.RemoveAllListeners();
        _rPanel.GetCircuitBreakerBtn().onClick.RemoveAllListeners();
        _rPanel.GetRelayControlBtn().onClick.RemoveAllListeners();
        _rPanel.GetCB1ScreenBtn().onClick.RemoveAllListeners();
        _rPanel.GetCB2ScreenBtn().onClick.RemoveAllListeners();
        areaManagerObj.GetArea3_2Btn().onClick.RemoveAllListeners();
        GetFirePlugBtn().onClick.RemoveAllListeners();
        firePlugTestPopup.GetOpenFirePlugBtn().onClick.RemoveAllListeners();
        firePlugTestPopup.GetPowerBtn().onClick.RemoveAllListeners();
        areaManagerObj.GetRTypeRBtn().onClick.RemoveAllListeners();
        uIDragAndCollisionHandler.StopDragging();
        uIDragAndCollisionHandler.ResetEvent();
    }

    private void OnStateChangedCircuitBreakerE(RTypeRCircuitBreakerESection state)
    {
        _btnManager.RemoveAllHighlights();
        RTypeRGlobalCanvas.Instance.menuPopup.indexBtn.interactable = true;
        SetCircuitBreakerSound();
        CBButtonEventResetE();
        switch (state)
        {
            case RTypeRCircuitBreakerESection.Init:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetCircuitBreakerEventNum(0);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    _rPanel.ShowScreen(_rPanel.mainScreenImg);
                    // Observable.Timer(System.TimeSpan.FromSeconds(1))
                    //     .Subscribe(_ =>
                    //     {
                    //         _soundManager.SetBuzzerVolume();
                    //         _rPanel.SetCircuitBreakerEventNum(1);
                    //         _soundManager.PlayBuzzer(true);
                    //         _rPanel.SoundCheck();
                    //         _rPanel.ShowScreen(_rPanel.cbScreen1Img);
                    //         Next();
                    //     }).AddTo(this);
                    NextEnable(false);
                    _btnManager.SetEvaluationButtons();
                }
                break;
            case RTypeRCircuitBreakerESection.부저음정지:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.ShowScreen(_rPanel.cbScreen1Img);
                    // _panelBtnCheck.SetBtn(false);
                    _soundManager.SetBuzzerVolume();
                    _rPanel.SetCircuitBreakerEventNum(1);
                    _soundManager.PlayBuzzer(true);
                    _rPanel.SoundCheck();
                    _rPanel.ShowScreen(_rPanel.cbScreen1Img);
                    _rPanel.SetSwitchButton(false);
                    _panelBtnCheck.checkDic[RBtnType.부저정지] = false;
                    // _panelBtnCheck.SetBtns(true, RBtnType.부저정지);
                    // SetHighlightControlPanel(_panelBtnCheck);
                    // _panelBtnCheck.SetBtns(true, RBtnType.부저정지, RBtnType.메인펌프오토, RBtnType.충압펌프오토, RBtnType.sp펌프오토, RBtnType.sp충압펌프오토);
                    // SetHighlightControlPanelCheck(_panelBtnCheck);
                    // RTypeRPanel.RTypeRPanelButtonAction.AddListener((btn) =>
                    // {
                    //     _gCanvas.totalScore.회로차단.평가1 = btn.checkDic[RBtnType.부저정지];
                    // });
                    NextEnable(false);
                }
                break;
            case RTypeRCircuitBreakerESection.회로차단:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButtons(true, RTypeRBtnType.부저정지);
                    _rPanel.ShowScreen(_rPanel.cbScreen1Img);
                    _soundManager.PlayBuzzer(false);
                    //_btnManager.EnableSpecificButton();
                    _rPanel.GetCircuitBreakerBtn(Next);
                    NextEnable(false);
                }
                break;
            case RTypeRCircuitBreakerESection.중계기제어선택:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.ShowScreen(_rPanel.cbAreaImg);
                    _rPanel.GetRelayControlBtn(Next);
                    NextEnable(false);
                }
                break;
            case RTypeRCircuitBreakerESection.이상신호중계기선택:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.ShowScreen(_rPanel.cb1Img);
                    _rPanel.GetCB1ScreenBtn(Next);
                    //_btnManager.EnableSpecificButton(_rPanel.GetCB1ScreenBtn(Next));
                    NextEnable(false);
                }
                break;
            case RTypeRCircuitBreakerESection.중계기통신재접속:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.ShowScreen(_rPanel.cb2Img);
                    //_btnManager.EnableSpecificButton();
                    _rPanel.GetCB2ScreenBtn(Next);
                    NextEnable(false);
                }
                break;
            case RTypeRCircuitBreakerESection.이상신호확인:
                {
                    //NextDisable();
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.ShowScreen(_rPanel.cb2Img);
                    areaManagerObj.GetArea3_2Btn(Next);
                    NextEnable(false);
                }
                break;
            case RTypeRCircuitBreakerESection.현장1:
                {
                    firePlugTestPopup.gameObject.SetActive(false);
                    firePlugTestPopup.testerObj.SetActive(false);
                    ShowRoom(RTypeRRoomType.Area3_2);
                    //_btnManager.EnableSpecificButton();
                    GetFirePlugBtn(Next);
                    NextEnable(false);
                }
                break;
            /*
            case RTypeRCircuitBreakerESection.소화전확인:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Default);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.None);
                    //_btnManager.EnableSpecificButton();
                    firePlugTestPopup.GetOpenFirePlugBtn(Next);
                    NextEnable(false);
                }
                break;
                */
            case RTypeRCircuitBreakerESection.소화전오픈:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.testerObj.SetActive(false);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.None);
                    inventoryObj.ShowTester(true);
                    uIDragAndCollisionHandler.StartDragging();
                    bool isDetected = false;
                    uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (isDetected)
                            return;

                        if (!targetObject.Equals(firePlugTestPopup.testerTargetObj))
                            return;

                        inventoryObj.ShowTester(false);
                        isDetected = true;
                        Next();
                    };
                    NextEnable(false);
                }
                break;
            case RTypeRCircuitBreakerESection.보관함테스터기:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    inventoryObj.ShowTester(false);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.testerObj.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.Default);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num0);
                    //_btnManager.EnableSpecificImage(firePlugTestPopup.testAreas[0], firePlugTestPopup.testerPenObjs[0].gameObject);
                    uIDragAndCollisionHandler.StartDragging();
                    bool isDetected = false;
                    uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (isDetected)
                            return;

                        if (!targetObject.Equals(firePlugTestPopup.testAreas[0]))
                            return;

                        isDetected = true;
                        Next();
                    };
                    NextEnable(false);
                }
                break;
            /*
            case RTypeRCircuitBreakerESection.중계기전원테스트1:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.중계기전원);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.Off);
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Off);
                    //_btnManager.EnableSpecificButton();
                    firePlugTestPopup.GetPowerBtn(Next);
                    NextEnable(false);
                }
                break;
                */
            case RTypeRCircuitBreakerESection.중계기전원테스트2:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.중계기전원);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    //_btnManager.EnableSpecificImage();
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num2390);
                    C2CorrectBtn.gameObject.SetActive(true);
                    C2WrongBtn.gameObject.SetActive(true);
                    C2CorrectBtn.onClick.AddListener(delegate
                    {
                        _gCanvas.totalScore.회로차단.평가1 = true;
                        Next();
                    });
                    C2WrongBtn.onClick.AddListener(delegate
                    {
                        _gCanvas.totalScore.회로차단.평가1 = false;
                        Next();
                    });
                    NextEnable(false);
                }
                break;
            case RTypeRCircuitBreakerESection.중계기통신테스트1:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.Default);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num0);
                    //_btnManager.EnableSpecificImage(firePlugTestPopup.testAreas[1], firePlugTestPopup.testerPenObjs[0].gameObject);
                    uIDragAndCollisionHandler.StartDragging();
                    bool isDetected = false;
                    uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (isDetected)
                            return;

                        if (!targetObject.Equals(firePlugTestPopup.testAreas[1]))
                            return;

                        isDetected = true;
                        Next();
                    };
                    NextEnable(false);
                }
                break;
            /*
            case RTypeRCircuitBreakerESection.중계기통신테스트2:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.중계기통신);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.Off);
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Off);
                    //_btnManager.EnableSpecificButton();
                    firePlugTestPopup.GetPowerBtn(Next);
                    NextEnable(false);
                }
                break;
                */
            case RTypeRCircuitBreakerESection.중계기통신테스트3:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.중계기통신);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    //_btnManager.EnableSpecificImage();
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num2710);
                    C3CorrectBtn.gameObject.SetActive(true);
                    C3WrongBtn.gameObject.SetActive(true);
                    C3CorrectBtn.onClick.AddListener(delegate
                    {
                        _gCanvas.totalScore.회로차단.평가2 = true;
                        Next();
                    });
                    C3WrongBtn.onClick.AddListener(delegate
                    {
                        _gCanvas.totalScore.회로차단.평가2 = false;
                        Next();
                    });
                    NextEnable(false);
                }
                break;
            case RTypeRCircuitBreakerESection.감지선로테스트1:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.Default);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num0);
                    //_btnManager.EnableSpecificImage(firePlugTestPopup.testAreas[2], firePlugTestPopup.testerPenObjs[0].gameObject);
                    uIDragAndCollisionHandler.StartDragging();
                    bool isDetected = false;
                    uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (isDetected)
                            return;

                        if (!targetObject.Equals(firePlugTestPopup.testAreas[2]))
                            return;

                        isDetected = true;
                        Next();
                    };
                    NextEnable(false);
                }
                break;
            /*
            case RTypeRCircuitBreakerESection.감지선로테스트2:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.감지기선로);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.Off);
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Off);
                    //_btnManager.EnableSpecificButton();
                    firePlugTestPopup.GetPowerBtn(Next);
                    NextEnable(false);
                }
                break;
                */
            case RTypeRCircuitBreakerESection.감지선로테스트3:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.감지기선로);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    //_btnManager.EnableSpecificImage();
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num0);
                    C4CorrectBtn.gameObject.SetActive(true);
                    C4WrongBtn.gameObject.SetActive(true);
                    C4CorrectBtn.onClick.AddListener(delegate
                    {
                        _gCanvas.totalScore.회로차단.평가3 = false;
                        Next();
                    });
                    C4WrongBtn.onClick.AddListener(delegate
                    {
                        _gCanvas.totalScore.회로차단.평가3 = true;
                        Next();
                    });
                    NextEnable(false);
                }
                break;
            case RTypeRCircuitBreakerESection.수신기이동:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.감지기선로);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num2390);
                    //_btnManager.EnableSpecificButton();
                    areaManagerObj.GetRTypeRBtn(Next);
                    _soundManager.SetBuzzerVolume();
                    _rPanel.GetSwitchButton("부저정지").OnCheck(true);
                    _rPanel.SoundCheck();
                    NextEnable(false);
                }
                break;
            
            case RTypeRCircuitBreakerESection.부저음복구:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetCircuitBreakerEventNum(0);
                    _rPanel.GetSwitchButton("부저정지").OnCheck(true);
                    _rPanel.ShowScreen(_rPanel.mainScreenImg);
                    //_panelBtnCheck.SetBtn(false);
                    _soundManager.SetBuzzerVolume(0);
                    //_panelBtnCheck.SetBtns(true, RBtnType.부저정지);
                    //SetHighlightControlPanel(_panelBtnCheck);
                    //_panelBtnCheck.checkDic[RBtnType.부저정지] = false;
                    //_panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토, RBtnType.충압펌프오토, RBtnType.sp펌프오토, RBtnType.sp충압펌프오토);
                    //SetHighlightControlPanelCheck(_panelBtnCheck);
                    NextEnable(false);
                }
                break;
                
            case RTypeRCircuitBreakerESection.평가완료:
                {
                    var results = new List<ResultObject>();

                    results.Add(new ResultObject()
                    {
                        Title = "중계기 전원 선로 확인",
                        IsSuccess = _gCanvas.totalScore.회로차단.평가1
                    });
                    results.Add(new ResultObject()
                    {
                        Title = "중계기 통신 선로 확인",
                        IsSuccess = _gCanvas.totalScore.회로차단.평가2
                    });
                    results.Add(new ResultObject()
                    {
                        Title = "중계기 감지기 선로 확인",
                        IsSuccess = _gCanvas.totalScore.회로차단.평가3
                    });
                    
                    _gCanvas.totalScore.회로차단List.Clear();
                    _gCanvas.totalScore.회로차단List.AddRange(results);
                    _gCanvas.SetNextEvaluation(InitEquipmentOperation, InitCircuitBreaker);
                    //_gCanvas.InitTotalResult();
                    //_gCanvas.SetNextEvaluation(InitCircuitBreaker);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }


    public void ChangeStateCircuitBreakerP(RTypeRCircuitBreakerPSection state)
    {

        _curRTypeRCircuitBreakerPSection = state;
        OnStateCircuitBreakerP(_curRTypeRCircuitBreakerPSection);
    }

    private void SetCircuitBreakerSound()
    {
        _soundManager.SetSirenVolume(0);
        _soundManager.SetBroadcastVolume(0);
        _soundManager.SetAlarmVolume(0);
        _soundManager.SetAlarm2Volume(0);
    }

    private void OnStateCircuitBreakerP(RTypeRCircuitBreakerPSection state)
    {
        _btnManager.RemoveAllHighlights();
        RTypeRPanel.RTypeRPanelButtonAction.RemoveAllListeners();
        RTypeRGlobalCanvas.Instance.menuPopup.indexBtn.interactable = true;
        uIDragAndCollisionHandler.StopDragging();
        uIDragAndCollisionHandler.ResetEvent();
        SetCircuitBreakerSound();
        _btnManager.EnableSpecificButton();
        _btnManager.EnableSpecificImage();
        switch (state)
        {
            case RTypeRCircuitBreakerPSection.회로단선:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    //RTypeRGlobalCanvas.Instance.menuPopup.indexBtn.interactable = false;
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetCircuitBreakerEventNum(0);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    _rPanel.ShowScreen(_rPanel.mainScreenImg);
                    _soundManager.SetBuzzerVolume(0f);
                    /*
                    _panelBtnCheck.SetBtn(false);
                    SetHighlightControlPanel(_panelBtnCheck);
                    _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토, RBtnType.충압펌프오토, RBtnType.sp펌프오토, RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    Observable.Timer(System.TimeSpan.FromSeconds(1))
                        .Subscribe(_ =>
                        {
                            _soundManager.SetBuzzerVolume();
                            _rPanel.SetCircuitBreakerEventNum(1);
                            _soundManager.PlayBuzzer(true);
                            _rPanel.SoundCheck();
                            _rPanel.ShowScreen(_rPanel.cbScreen1Img);
                            RTypeRGlobalCanvas.Instance.menuPopup.indexBtn.interactable = true;
                            NextEnable();
                        }).AddTo(this);
                    NextDisable();
                    */
                    NextEnable();
                }
                break;
            case RTypeRCircuitBreakerPSection.부저음정지:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _soundManager.SetBuzzerVolume();
                    _rPanel.ShowScreen(_rPanel.cbScreen1Img);
                    _rPanel.SetCircuitBreakerEventNum(1);
                    _soundManager.MuteBuzzer(false);
                    _soundManager.PlayBuzzer(true);
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.checkDic[RBtnType.부저정지] = false;
                    _panelBtnCheck.SetBtns(true, RBtnType.부저정지);
                    SetHighlightControlPanel(_panelBtnCheck);
                    _panelBtnCheck.SetBtns(true, RBtnType.부저정지, RBtnType.메인펌프오토, RBtnType.충압펌프오토, RBtnType.sp펌프오토, RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    NextDisable();
                }
                break;
            case RTypeRCircuitBreakerPSection.회로차단:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _soundManager.PlayBuzzer(false);
                    _rPanel.ShowScreen(_rPanel.cbScreen1Img);
                    _btnManager.EnableSpecificButton(_rPanel.GetCircuitBreakerBtn(Next));
                    NextDisable();
                }
                break;
            case RTypeRCircuitBreakerPSection.중계기제어선택:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.ShowScreen(_rPanel.cbAreaImg);
                    _btnManager.EnableSpecificButton(_rPanel.GetRelayControlBtn(Next));
                    NextDisable();
                }
                break;
            case RTypeRCircuitBreakerPSection.이상신호중계기선택:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.ShowScreen(_rPanel.cb1Img);
                    _btnManager.EnableSpecificButton(_rPanel.GetCB1ScreenBtn(Next));
                    NextDisable();
                }
                break;
            case RTypeRCircuitBreakerPSection.중계기통신재접속:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.ShowScreen(_rPanel.cb2Img);
                    _btnManager.EnableSpecificButton(_rPanel.GetCB2ScreenBtn(delegate
                    {
                        _btnManager.EnableSpecificButton();
                        //_btnManager.EnableSpecificImage(_rPanel.cb2ListImg);
                        //_btnManager.HighlightObj();
                        Next();
                    }));
                    NextDisable();
                }
                break;
            case RTypeRCircuitBreakerPSection.이상신호확인:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.ShowScreen(_rPanel.cb2Img);
                    _btnManager.EnableSpecificButton(areaManagerObj.GetArea3_2Btn(Next));
                    _btnManager.EnableSpecificImage(_rPanel.cb2ListImg);
                }
                break;
            case RTypeRCircuitBreakerPSection.현장1:
                {
                    firePlugTestPopup.gameObject.SetActive(false);
                    firePlugTestPopup.testerObj.SetActive(false);
                    ShowRoom(RTypeRRoomType.Area3_2);
                    _btnManager.EnableSpecificButton(GetFirePlugBtn(Next));
                    NextDisable();
                    inventoryObj.ShowTester(false);
                }
                break;
            /*
            case RTypeRCircuitBreakerPSection.소화전확인:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Default);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.None);
                    _btnManager.EnableSpecificButton(firePlugTestPopup.GetOpenFirePlugBtn(Next));
                    NextDisable();
                }
                break;
                */
            case RTypeRCircuitBreakerPSection.소화전오픈:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.testerObj.SetActive(false);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.None);
                    _btnManager.EnableSpecificImage(inventoryObj.ShowTester(true),
                        firePlugTestPopup.testerTargetObj);
                    uIDragAndCollisionHandler.StartDragging();
                    bool isDetected = false;
                    uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (isDetected)
                            return;

                        if (!targetObject.Equals(firePlugTestPopup.testerTargetObj))
                            return;

                        inventoryObj.ShowTester(false);
                        isDetected = true;
                        Next();
                    };
                }
                break;
            case RTypeRCircuitBreakerPSection.보관함테스터기:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.testerObj.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.Default);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.Off);
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Off);
                    _btnManager.EnableSpecificButton(firePlugTestPopup.GetPowerBtn(delegate
                    {
                        if (!firePlugTestPopup.powerObjs[0].activeSelf)
                            return;
                        _btnManager.EnableSpecificButton();
                        firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.Default);
                        firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                        firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num0);
                        _btnManager.EnableSpecificImage(firePlugTestPopup.testAreas[0], firePlugTestPopup.testerPenObjs[0].gameObject);
                        uIDragAndCollisionHandler.StartDragging();
                        bool isDetected = false;
                        uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                        {
                            if (isDetected)
                                return;

                            if (!targetObject.Equals(firePlugTestPopup.testAreas[0]))
                                return;

                            isDetected = true;
                            Next();
                        };
                    }));
   
                    NextDisable();
                }
                break;
            /*
            case RTypeRCircuitBreakerPSection.중계기전원테스트1:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.중계기전원);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.Off);
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Off);
                    _btnManager.EnableSpecificButton(firePlugTestPopup.GetPowerBtn(Next));
                    //_btnManager.EnableSpecificImage(firePlugTestPopup.testAreas[0]);
                    // uIDragAndCollisionHandler.StartDragging();
                    // bool isDetected = false;
                    // uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    // {
                    //     if (isDetected)
                    //         return;
                    //
                    //     if (!targetObject.Equals(firePlugTestPopup.testAreas[0]))
                    //         return;
                    //
                    //     Next();
                    //     isDetected = true;
                    // };
                    // NextDisable();
                }
                break;
                */
            case RTypeRCircuitBreakerPSection.중계기전원테스트2:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.중계기전원);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    _btnManager.EnableSpecificImage(firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num2390));
                    NextEnable();
                }
                break;
            case RTypeRCircuitBreakerPSection.중계기통신테스트1:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.Default);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num0);
                    _btnManager.EnableSpecificImage(firePlugTestPopup.testAreas[1], firePlugTestPopup.testerPenObjs[0].gameObject);
                    uIDragAndCollisionHandler.StartDragging();
                    bool isDetected = false;
                    uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (isDetected)
                            return;

                        if (!targetObject.Equals(firePlugTestPopup.testAreas[1]))
                            return;

                        isDetected = true;
                        Next();
                    };
                    NextDisable();
                }
                break;
            /*
            case RTypeRCircuitBreakerPSection.중계기통신테스트2:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.중계기통신);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.Off);
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Off);
                    _btnManager.EnableSpecificButton(firePlugTestPopup.GetPowerBtn(Next));
                    NextDisable();
                }
                break;
            */
            case RTypeRCircuitBreakerPSection.중계기통신테스트3:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.중계기통신);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    _btnManager.EnableSpecificImage(firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num2710));
                    NextEnable();
                }
                break;
            case RTypeRCircuitBreakerPSection.감지선로테스트1:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.Default);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num0);
                    _btnManager.EnableSpecificImage(firePlugTestPopup.testAreas[2], firePlugTestPopup.testerPenObjs[0].gameObject);
                    uIDragAndCollisionHandler.StartDragging();
                    bool isDetected = false;
                    uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (isDetected)
                            return;

                        if (!targetObject.Equals(firePlugTestPopup.testAreas[2]))
                            return;

                        isDetected = true;
                        Next();
                    };
                    NextDisable();
                }
                break;
            /*
            case RTypeRCircuitBreakerPSection.감지선로테스트2:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.감지기선로);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.Off);
                    firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Off);
                    _btnManager.EnableSpecificButton(firePlugTestPopup.GetPowerBtn(Next));
                    NextDisable();
                }
               break;
               */
            case RTypeRCircuitBreakerPSection.감지선로테스트3:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.감지기선로);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    //firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num2390);
                    _btnManager.EnableSpecificImage(firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num0));
                    NextEnable();
                }
                break;
            case RTypeRCircuitBreakerPSection.수신기이동:
                {
                    ShowRoom(RTypeRRoomType.Area3_2);
                    firePlugTestPopup.gameObject.SetActive(true);
                    _rPanel.SetCircuitBreakerEventNum(0);
                    firePlugTestPopup.ShowFirePlug(RTypeRFirePlugType.Inner);
                    firePlugTestPopup.ShowTesterPen(RTypeRTesterPenType.감지기선로);
                    firePlugTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    _btnManager.EnableSpecificImage(firePlugTestPopup.ShowTesterNum(RTypeRTesterNumType.Num2390));
                    _btnManager.EnableSpecificButton(areaManagerObj.GetRTypeRBtn(Next));
                    _soundManager.SetBuzzerVolume();
                    _rPanel.GetSwitchButton("부저정지").OnCheck(true);
                    _rPanel.SoundCheck();
                    NextDisable();
                }
                break;
            case RTypeRCircuitBreakerPSection.부저음복구:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.GetSwitchButton("부저정지").OnCheck(true);
                    _rPanel.ShowScreen(_rPanel.mainScreenImg);
                    _panelBtnCheck.SetBtn(false);
                    _soundManager.SetBuzzerVolume(0);
                    _panelBtnCheck.SetBtns(true, RBtnType.부저정지);
                    SetHighlightControlPanel(_panelBtnCheck);
                    _panelBtnCheck.checkDic[RBtnType.부저정지] = false;
                    _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토, RBtnType.충압펌프오토, RBtnType.sp펌프오토, RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    NextDisable();
                }
                break;
            case RTypeRCircuitBreakerPSection.교육완료:
                {
                    SetCompletePopup("회로단선 조치를 모두 완료했습니다.", "다음 시험을", InitEquipmentOperation);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        //_btnManager.HighlightObj();
    }
}
