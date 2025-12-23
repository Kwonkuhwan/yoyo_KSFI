using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using RBtnType = RTypeRBtnType;
using VInspector;
/// <summary>
/// 설비 동작
/// </summary>
public partial class RTypeRSection
{
    [Foldout("설비작동")]
    private RTypeREquipmentOperationPSection _curRTypeREquipmentOperationPSection;
    private RTypeREquipmentOperationESection _curRTypeREquipmentOperationESection;
    public HintScriptableObj rTypeREquipmentOperationHintPObj;
    public HintScriptableObj rTypeREquipmentOperationHintEObj;
    private UnityAction recoveryBtnAction;
    public GameObject 설비동작평가1;
    [FormerlySerializedAs("E3Toggle1")]
    [SerializeField] public Toggle 설비동작토글1;
    [FormerlySerializedAs("E3Toggle2")]
    [SerializeField] public Toggle 설비동작토글2;

    private bool dust1 = false;
    private bool dust2 = false;
    [EndFoldout]
    public void InitEquipmentOperation()
    {
        Init();
        
        ShowRoom(RTypeRRoomType.RTypeR);
        InitEMode();
        _soundManager.StopAllFireSound();
        _rPanel.ShowScreen(_rPanel.mainScreenImg);
        recoveryBtnAction = RecoveryBtn;
        curMainSection = RTypeRMainSection.EquipmentOperation;
        _maxSection = rTypeRState switch
        {
            RTypeRState.PracticeMode => Enum.GetValues(typeof(RTypeREquipmentOperationPSection)).Length,
            RTypeRState.EvaluationMode => Enum.GetValues(typeof(RTypeREquipmentOperationESection)).Length,
            _ => _maxSection
        };
        SetSectionRange(0, _maxSection, _maxSection);


    }
    private void SetEquipmentOperationSection(int index)
    {
        if (rTypeRState.Equals(RTypeRState.PracticeMode))
        {
            prevBtn.gameObject.SetActive(true);
            SetHint(GetHintTextAndAudio(rTypeREquipmentOperationHintPObj, (int)RTypeREquipmentOperationPSection.수신기선택 + index));
            ChangeStateEquipmentOperationP(RTypeREquipmentOperationPSection.수신기선택 + index);
        }
        if (!rTypeRState.Equals(RTypeRState.EvaluationMode))
            return;
        prevBtn.gameObject.SetActive(false);
        SetHint(GetHintTextAndAudio(rTypeREquipmentOperationHintEObj, (int)RTypeREquipmentOperationESection.Init + index));
        ChangeStateEquipmentOperationE(RTypeREquipmentOperationESection.Init + index);
    }

    public void ChangeStateEquipmentOperationE(RTypeREquipmentOperationESection state)
    {

        _curRTypeREquipmentOperationESection = state;
        OnStateChangedEquipmentOperationE(_curRTypeREquipmentOperationESection);
    }

    private void OnStateChangedEquipmentOperationE(RTypeREquipmentOperationESection state)
    {
        _btnManager.RemoveAllHighlights();
        EOButtonEventResetE();
        switch (state)
        {
            case RTypeREquipmentOperationESection.Init:
                {
                    dust1 = false;
                    dust2 = false;
                    ShowRoom(RTypeRRoomType.RTypeR);
                    OpenFireDoor(false);
                    //_gCanvas.menuPopup.indexBtn.interactable = false;
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetFireAlarmEventNum(0);
                    _rPanel.SetEquipmentOperationEventNum(0);
                    _soundManager.PlayAlarm(false);
                    _soundManager.PlayAlarm2(false);
                    _soundManager.PlayBroadcast(false);
                    _soundManager.SetBuzzerVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    _rPanel.ShowScreen(_rPanel.mainScreenImg);
                    /*
                    Observable.Timer(System.TimeSpan.FromSeconds(1))
                        .Subscribe(_ =>
                        {
                            _rPanel.SetFireAlarmEventNum(1);
                            _soundManager.PlayAlarm(true);
                            _soundManager.PlayAlarm2(true);
                            _soundManager.PlayBroadcast(true);
                            _soundManager.SetBuzzerVolume(0);
                            _soundManager.SetSirenVolume(0);
                            _rPanel.ShowScreen(_rPanel.fireScreen1Img);
                            _gCanvas.menuPopup.indexBtn.interactable = true;
                            Next();
                        }).AddTo(this);
                        */
                    NextEnable();
                    _btnManager.SetEvaluationButtons();
                }
                break;
            case RTypeREquipmentOperationESection.화재발생:
                {
                    _rPanel.SetFireAlarmEventNum(1);
                    _rPanel.SetEquipmentOperationEventNum(1);
                    _soundManager.PlayAlarm(true);
                    _soundManager.PlayAlarm2(true);
                    _soundManager.PlayBroadcast(true);
                    _soundManager.PlayBuzzer(true);
                    //_soundManager.SetBuzzerVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _rPanel.ShowScreen(_rPanel.fireScreen1Img);
                    _gCanvas.menuPopup.indexBtn.interactable = true;
                    _rPanel.ShowScreen(_rPanel.fireScreen1Img);
                    _rPanel.GetFireAlarmBtn(delegate
                    {
                        _gCanvas.totalScore.설비동작.평가1 = true;
                        Next();
                    });
                    //_panelBtnCheck.SetBtn(false);
                    //_btnManager.EnableSpecificButton();
                    NextEnable(false);
                }
                break;
            case RTypeREquipmentOperationESection.화재여부확인및현장이동:
                {
                    _rPanel.GetFireAlarmBtn().onClick.RemoveAllListeners();
                    _rPanel.ShowScreen(_rPanel.fireAlarmMapImg);
                    // _rPanel.GetEquipmentOperationBtn(delegate
                    // {
                    //     Next();
                    // });
                    areaManagerObj.GetFireDoorBtn(Next);
                    //NextDisable();
                }
                break;
            case RTypeREquipmentOperationESection.현장1:
                {
                    ShowRoom(RTypeRRoomType.FireDoor);
                    SetFDSmokeDetector(true);
                    //_btnManager.EnableSpecificButton(GetFDSmokeDetector(),areaManagerObj.GetRTypeRBtn(Next));
                    areaManagerObj.GetRTypeRBtn(Next);
                    //NextEnable();
                    //NextDisable();
                    //_rPanel.ShowPanel(false);
                }
                break;
            case RTypeREquipmentOperationESection.수신기이동1: //음향정지
                {
                    //NextDisable();
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    SoundManager.Instance.SetDefaultVolume();
                    //_soundManager.SetBuzzerVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _rPanel.SoundCheck();
                    _rPanel.ShowScreen(_rPanel.equipmentOperationEmptyListImg);
                    // RTypeRPanel.RTypeRPanelButtonAction.AddListener((btn) =>
                    // {
                    //     _gCanvas.totalScore.설비동작.평가2 = btn.checkDic[RBtnType.주음향정지] && btn.checkDic[RBtnType.지구음향정지] && btn.checkDic[RBtnType.비상방송정지] && btn.checkDic[RBtnType.부저정지];
                    // });
                    NextEnable(false);
                    // _panelBtnCheck.SetBtn(false);
                    // _panelBtnCheck.SetBtns(true, RBtnType.주음향정지,
                    //     RBtnType.지구음향정지,
                    //     RBtnType.비상방송정지,
                    //     RBtnType.부저정지,
                    //     RBtnType.메인펌프오토,
                    //     RBtnType.충압펌프오토,
                    //     RBtnType.sp펌프오토,
                    //     RBtnType.sp충압펌프오토);
                    // SetHighlightControlPanel(_panelBtnCheck, true);
                    // _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토,
                    //     RBtnType.충압펌프오토,
                    //     RBtnType.sp펌프오토,
                    //     RBtnType.sp충압펌프오토);
                    // _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    // SetHighlightControlPanelCheck(_panelBtnCheck);
                }
                break;
            case RTypeREquipmentOperationESection.음향정지1: //평가3
                {
                    _soundManager.PlayAlarm(false);
                    _soundManager.PlayAlarm2(false);
                    _soundManager.PlayBroadcast(false);
                    _soundManager.PlayBuzzer(false);
                    //_soundManager.SetBuzzerVolume(0);
                    _soundManager.SetSirenVolume(0);
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetEquipmentOperationEventNum(1);
                    설비동작평가1.SetActive(true);
                    설비동작토글1.gameObject.SetActive(true);
                    설비동작토글2.gameObject.SetActive(true);
                    _rPanel.GetSwitchButton("주음향정지").OnCheck(true);
                    _rPanel.GetSwitchButton("지구음향정지").OnCheck(true);
                    _rPanel.GetSwitchButton("비상방송정지").OnCheck(true);
                    _rPanel.GetSwitchButton("부저정지").OnCheck(true);
                    // E3Toggle3.gameObject.SetActive(true);
                    // E3Toggle3.onValueChanged.AddListener((isOn) =>
                    // {
                    //     _gCanvas.totalScore.설비동작.평가3 = isOn;
                    // });

                    // _btnManager.EnableSpecificButton();
                    //_rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    areaManagerObj.GetFireDoorBtn(Next);
                    NextEnable(false);
                    //NextDisable();
                }
                break;
            case RTypeREquipmentOperationESection.현장2:
                {
                    //NextDisable();
                    설비동작평가1.SetActive(false);
                    ShowRoom(RTypeRRoomType.FireDoor);
                    _rPanel.SetEquipmentOperationEventNum(1);
                    smokeDetectorPopup.gameObject.SetActive(false);
                    //_btnManager.EnableSpecificButton();
                    GetFDSmokeDetector(delegate
                    {
                        smokeDetectorPopup.Init(RTypeRSmokeDetectorPopupType.On);
                        dust1 = true;
                        Next();
                    });
                    NextEnable(false);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                }
                break;
            case RTypeREquipmentOperationESection.감지기먼지제거1:
                {

                    //NextDisable();
                    ShowRoom(RTypeRRoomType.FireDoor);
                    SetFDSmokeDetector(true);
                    smokeDetectorPopup.gameObject.SetActive(true);
                    smokeDetectorPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.On);
                    //_btnManager.EnableSpecificButton( );
                    smokeDetectorPopup.GetSmokeDetectorBtn(delegate
                    {
                        smokeDetectorPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOnDust);
                        dust2 = true;
                        Next();
                    });
                    NextEnable(false);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                }
                break;
            case RTypeREquipmentOperationESection.감지기먼지제거2:
                {
                    //NextDisable();
                    ShowRoom(RTypeRRoomType.FireDoor);
                    SetFDSmokeDetector(false);
                    OpenFireDoor(false);
                    smokeDetectorPopup.gameObject.SetActive(true);
                    smokeDetectorPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOnDust);
                    smokeDetectorPopup.GetSmokeDetectorBtn(delegate
                    {
                        smokeDetectorPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.Default);
                        //Next();
                    });
                    //_btnManager.EnableSpecificButton();
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    NextEnable(false);
                }
                break;
            case RTypeREquipmentOperationESection.방화문복구1:
                {
                    //NextDisable();
                    smokeDetectorPopup.gameObject.SetActive(false);
                    OpenFireDoor(false);
                    //_rPanel.ShowScreen(_rPanel.equipmentOperationArea1ListImg);
                    //_btnManager.EnableSpecificButton();
                    GetFireDoorBtn(delegate
                    {
                        //_btnManager.EnableSpecificButton();
                        OpenFireDoor(true);
                        Next();
                    });
                    //_btnManager.EnableSpecificImage(_rPanel.equipmentOperationArea1ListHighlightiImg);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;

                }
                break;
            case RTypeREquipmentOperationESection.방화문복구2:
                {
                    //_btnManager.EnableSpecificImage();
                    ShowRoom(RTypeRRoomType.FireDoor);
                    _rPanel.SetFireAlarmEventNum(0);
                    _rPanel.SetEquipmentOperationEventNum(0);
                    _rPanel.SoundCheck();
                    //SoundManager.Instance.SetDefaultVolume();
                    SoundManager.Instance.SetBuzzerVolume(0);
                    SoundManager.Instance.SetSirenVolume(0);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    //_btnManager.EnableSpecificButton(GetFDSmokeDetector(),);
                    areaManagerObj.GetRTypeRBtn(Next);
                    _rPanel.ShowScreen(_rPanel.equipmentOperationArea1ListImg);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.RemoveListener(recoveryBtnAction);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    //NextDisable();
                }
                break;
            case RTypeREquipmentOperationESection.수신기이동2:
                {
                    //NextDisable();
                    ShowRoom(RTypeRRoomType.RTypeR);
                    //_rPanel.SetSwitchButton(false);
                    _rPanel.SetFireAlarmEventNum(1);
                    _rPanel.SetEquipmentOperationEventNum(0);
                    _rPanel.GetSwitchButton("주음향정지").OnCheck(true);
                    _rPanel.GetSwitchButton("지구음향정지").OnCheck(true);
                    _rPanel.GetSwitchButton("비상방송정지").OnCheck(true);
                    _rPanel.GetSwitchButton("부저정지").OnCheck(true);
                    _rPanel.GetSwitchButton("수신기복구").OnCheck(false);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = true;
                    _rPanel.ShowScreen(_rPanel.fireScreen1Img);
                    _rPanel.SoundCheck();
                    _soundManager.SetSirenVolume(0f);
                    // _panelBtnCheck.SetBtn(false);
                    // _panelBtnCheck.SetBtns(true, RBtnType.수신기복구);
                    //_rPanel.GetSwitchButton("수신기복구").GetButton().onClick.RemoveAllListeners();
                    _rPanel.GetSwitchButton("수신기복구").Init(_rPanel.SwitchButtonClick, false);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.AddListener(recoveryBtnAction);

                    //_panelBtnCheck.checkDic["수신기복구"] = true;
                    // SetHighlightControlPanel(_panelBtnCheck);
                    // _panelBtnCheck.SetBtns(true,
                    //     RBtnType.수신기복구,
                    //     RBtnType.주음향정지,
                    //     RBtnType.지구음향정지,
                    //     RBtnType.비상방송정지,
                    //     RBtnType.부저정지,
                    //     RBtnType.메인펌프오토,
                    //     RBtnType.충압펌프오토,
                    //     RBtnType.sp펌프오토,
                    //     RBtnType.sp충압펌프오토);
                    // SetHighlightControlPanelCheck(_panelBtnCheck);
                    NextEnable(false);
                }
                break;
            case RTypeREquipmentOperationESection.수신기복구:
                {
                    //NextDisable();
                    _rPanel.SetFireAlarmEventNum(0);
                    _rPanel.SetEquipmentOperationEventNum(0);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.RemoveListener(recoveryBtnAction);
                    _rPanel.ShowScreen(_rPanel.fireAlarmMap2Img);
                    _soundManager.ZeroVolume();
                    //_btnManager.EnableSpecificButton();
                    _rPanel.GetInitScreenBtn(delegate
                    {
                        _rPanel.ShowScreen(_rPanel.mainScreenImg);
                        _btnManager.EnableSpecificButton();
                        Next();
                    });
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                }
                break;
            /*
            case RTypeREquipmentOperationESection.음향복구1:
                {
                    //NextDisable();
                    _rPanel.ShowScreen(_rPanel.equipmentOperationEmptyListImg);
                    _soundManager.ZeroVolume();
                    //_btnManager.EnableSpecificButton();
                    _rPanel.GetInitScreenBtn(delegate
                    {
                        _rPanel.ShowScreen(_rPanel.mainScreenImg);
                        _btnManager.EnableSpecificButton();
                        Next();
                    });
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                }
                break;
                */
            case RTypeREquipmentOperationESection.평가종료:
                {
                    var results = new List<ResultObject>();
                    _gCanvas.totalScore.설비동작.평가2 = 설비동작토글2.isOn;
                    _gCanvas.totalScore.설비동작.평가3 = dust1 && dust2;
                    results.Add(new ResultObject()
                    {
                        Title = "화재 발생 위치 확인",
                        IsSuccess = _gCanvas.totalScore.설비동작.평가1
                    });
                    
                    results.Add(new ResultObject()
                    {
                        Title = "수신기의 작동 상태 확인",
                        IsSuccess = _gCanvas.totalScore.설비동작.평가2
                    });
                    
                    results.Add(new ResultObject()
                    {
                        Title = "오동작 감지기 조치",
                        IsSuccess = _gCanvas.totalScore.설비동작.평가3
                    });
                    results.Add(new ResultObject()
                    {
                        Title = "수신기 복구",
                        IsSuccess = _gCanvas.totalScore.설비동작.평가4
                    });
                    _gCanvas.totalScore.설비동작List.Clear();
                    _gCanvas.totalScore.설비동작List.AddRange(results);
                    _btnManager.SetEvaluationButtons();
                    _gCanvas.SetNextEvaluation(_gCanvas.InitTotalResult, InitEquipmentOperation, true);
                    //_gCanvas.SetNextEvaluation(Total);
                    //GasSysGlobalCanvas.Instance.SetNextEvaluation(InitManualOperation);
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void EOButtonEventResetE()
    {
        _rPanel.GetFireAlarmBtn().onClick.RemoveAllListeners();
        _rPanel.GetEquipmentOperationBtn().onClick.RemoveAllListeners();
        areaManagerObj.GetFireDoorBtn().onClick.RemoveAllListeners();
        GetFDSmokeDetector().onClick.RemoveAllListeners();
        smokeDetectorPopup.GetSmokeDetectorBtn().onClick.RemoveAllListeners();
        GetFireDoorBtn().onClick.RemoveAllListeners();
        areaManagerObj.GetRTypeRBtn().onClick.RemoveAllListeners();
        _rPanel.GetInitScreenBtn().onClick.RemoveAllListeners();
        //E3Toggle3.onValueChanged.RemoveAllListeners();
        // 설비동작토글1.gameObject.SetActive(false);
        // 설비동작토글2.gameObject.SetActive(false);
        // //E3Toggle3.gameObject.SetActive(false);
        // 설비동작토글1.isOn = false;
        // 설비동작토글2.isOn = false;
        //E3Toggle3.isOn = false;
        smokeDetectorPopup.gameObject.SetActive(false);
    }


    public void ChangeStateEquipmentOperationP(RTypeREquipmentOperationPSection state)
    {

        _curRTypeREquipmentOperationPSection = state;
        OnStateChangedEquipmentOperationP(_curRTypeREquipmentOperationPSection);
    }

    private void OnStateChangedEquipmentOperationP(RTypeREquipmentOperationPSection state)
    {
        _btnManager.RemoveAllHighlights();
        _btnManager.EnableSpecificButton();
        _btnManager.EnableSpecificImage();
        RTypeRPanel.RTypeRPanelButtonAction.RemoveAllListeners();
        //_rPanel.GetSwitchButton("수신기복구").GetButton().onClick.RemoveListener(recoveryBtnAction);;
        //_gCanvas.menuPopup.indexBtn.interactable = true;
        switch (state)
        {
            case RTypeREquipmentOperationPSection.수신기선택:
                {
                    OpenFireDoor(false);
                    smokeDetectorPopup.gameObject.SetActive(false);
                    //_gCanvas.menuPopup.indexBtn.interactable = false;
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetFireAlarmEventNum(0);
                    _rPanel.SetEquipmentOperationEventNum(0);
                    _soundManager.PlayAlarm(false);
                    _soundManager.PlayAlarm2(false);
                    _soundManager.PlayBroadcast(false);
                    _soundManager.PlayBuzzer(false);
                    //_soundManager.SetBuzzerVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _panelBtnCheck.SetBtn(false);
                    SetHighlightControlPanel(_panelBtnCheck);
                    _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토, RBtnType.충압펌프오토, RBtnType.sp펌프오토, RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    _btnManager.EnableSpecificButton(_switchButtons);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    _rPanel.ShowScreen(_rPanel.mainScreenImg);
                    // Observable.Timer(System.TimeSpan.FromSeconds(1))
                    //     .Subscribe(_ =>
                    //     {
                    //         // _rPanel.SetFireAlarmEventNum(1);
                    //         // _rPanel.SetEquipmentOperationEventNum(1);
                    //         // _soundManager.PlayAlarm(true);
                    //         // _soundManager.PlayAlarm2(true);
                    //         // _soundManager.PlayBroadcast(true);
                    //         // _soundManager.SetBuzzerVolume(0);
                    //         // _soundManager.SetSirenVolume(0);
                    //         // _rPanel.ShowScreen(_rPanel.fireScreen1Img);
                    //         // _gCanvas.menuPopup.indexBtn.interactable = true;
                    //         NextEnable();
                    //     }).AddTo(this);
                    NextEnable(true, false);
                    // 이 스크립트가 파괴될 때 구독을 자동으로 해제
                }
                break;
            case RTypeREquipmentOperationPSection.화재발생:
                {
                    _rPanel.SetFireAlarmEventNum(1);
                    _rPanel.SetEquipmentOperationEventNum(1);
                    _soundManager.PlayAlarm(true);
                    _soundManager.PlayAlarm2(true);
                    _soundManager.PlayBroadcast(true);
                    _soundManager.PlayBuzzer(true);
                    //_soundManager.SetBuzzerVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _rPanel.ShowScreen(_rPanel.fireScreen1Img);
                    _gCanvas.menuPopup.indexBtn.interactable = true;
                    _rPanel.ShowScreen(_rPanel.fireScreen1Img);
                    _panelBtnCheck.SetBtn(false);
                    _btnManager.EnableSpecificButton(_rPanel.GetFireAlarmBtn(Next));
                    NextDisable();
                }
                break;
            case RTypeREquipmentOperationPSection.화재발생지점확인:
                {
                    _rPanel.ShowScreen(_rPanel.fireAlarmMapImg);
                    _panelBtnCheck.SetBtn(false);
                    _btnManager.EnableSpecificButton(_rPanel.GetEquipmentOperationBtn(Next));
                    NextDisable();
                }
                break;
            case RTypeREquipmentOperationPSection.작동설비확인:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.ShowScreen(_rPanel.equipmentOperationArea1ListImg);
                    _btnManager.EnableSpecificButton();
                    _btnManager.EnableSpecificButton(areaManagerObj.GetFireDoorBtn(Next));
                    _btnManager.EnableSpecificImage(_rPanel.equipmentOperationArea1ListHighlightiImg);
                    NextDisable();
                }
                break;
            /*
            case RTypeREquipmentOperationPSection.화재여부확인및현장이동:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _btnManager.EnableSpecificButton(areaManagerObj.GetFireDoorBtn(Next));
                    NextDisable();
                }
                break;
                */
            case RTypeREquipmentOperationPSection.현장1:
                {
                    ShowRoom(RTypeRRoomType.FireDoor);
                    SetFDSmokeDetector(true);
                    //_btnManager.EnableSpecificButton(GetFDSmokeDetector(), areaManagerObj.GetRTypeRBtn(Next));
                    _btnManager.EnableSpecificButton(areaManagerObj.GetRTypeRBtn(Next));
                    NextDisable();
                    //_rPanel.ShowPanel(false);
                }
                break;
            case RTypeREquipmentOperationPSection.수신기이동1:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    SoundManager.Instance.SetDefaultVolume();
                    //_soundManager.SetBuzzerVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _rPanel.SoundCheck();
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.SetBtns(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지,
                        RBtnType.부저정지,
                        RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanel(_panelBtnCheck, true);
                    _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                }
                break;
            case RTypeREquipmentOperationPSection.음향정지1:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _btnManager.EnableSpecificButton(areaManagerObj.GetFireDoorBtn(Next));
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    NextDisable();
                }
                break;
            case RTypeREquipmentOperationPSection.현장2:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.FireDoor);
                    smokeDetectorPopup.gameObject.SetActive(false);
                    _btnManager.EnableSpecificButton(GetFDSmokeDetector(delegate
                    {
                        smokeDetectorPopup.Init(RTypeRSmokeDetectorPopupType.On);
                        Next();
                    }));
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                }
                break;
            case RTypeREquipmentOperationPSection.감지기먼지제거1:
                {

                    NextDisable();
                    ShowRoom(RTypeRRoomType.FireDoor);
                    SetFDSmokeDetector(true);
                    smokeDetectorPopup.gameObject.SetActive(true);
                    smokeDetectorPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.On);
                    smokeDetectorPopup.GetSmokeDetectorBtn().interactable = true;
                    //smokeDetectorPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.OpenAni);
                    _btnManager.EnableSpecificButton(smokeDetectorPopup.GetSmokeDetectorBtn(delegate
                    {
                        smokeDetectorPopup.GetSmokeDetectorBtn().interactable = false;
                        //smokeDetectorPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOnDust);
                        smokeDetectorPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.OpenAni);
                        smokeDetectorPopup.OpenAni(delegate
                        {
                            NextEnable();
                        });
                       
                        
                    }));
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                }
                break;
            case RTypeREquipmentOperationPSection.감지기먼지제거2:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.FireDoor);
                    SetFDSmokeDetector(false);
                    OpenFireDoor(false);
                    smokeDetectorPopup.gameObject.SetActive(true);
                    smokeDetectorPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOnDust);
                    //smokeDetectorPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.Default);
                    _btnManager.EnableSpecificButton();
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    NextEnable();
                }
                break;
            case RTypeREquipmentOperationPSection.감지기먼지제거3:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.FireDoor);
                    SetFDSmokeDetector(false);
                    OpenFireDoor(false);
                    smokeDetectorPopup.gameObject.SetActive(true);
                    _rPanel.GetSwitchButton("수신기복구").OnCheck(false);
                    _rPanel.GetSwitchButton("주음향정지").OnCheck(true);
                    _rPanel.GetSwitchButton("지구음향정지").OnCheck(true);
                    _rPanel.GetSwitchButton("비상방송정지").OnCheck(true);
                    _rPanel.GetSwitchButton("부저정지").OnCheck(true);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = true;
                    _rPanel.SoundCheck();
                    //smokeDetectorPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOnDust);
                    smokeDetectorPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.Default);
                    _btnManager.EnableSpecificButton();
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    NextEnable();
                }
                break;
            case RTypeREquipmentOperationPSection.수신기이동:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.GetSwitchButton("수신기복구").OnCheck(false);
                    _rPanel.GetSwitchButton("주음향정지").OnCheck(true);
                    _rPanel.GetSwitchButton("지구음향정지").OnCheck(true);
                    _rPanel.GetSwitchButton("비상방송정지").OnCheck(true);
                    _rPanel.GetSwitchButton("부저정지").OnCheck(true);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = true;
                    //_rPanel.SetSwitchButton(false);
                    _rPanel.SetFireAlarmEventNum(1, true);
                    _soundManager.SetBuzzerVolume();
                    _soundManager.SetAlarm2Volume();
                    _soundManager.SetAlarmVolume();
                    _soundManager.SetBroadcastVolume();
                    _rPanel.SoundCheck();
                    _rPanel.SetEquipmentOperationEventNum(0);

                    _rPanel.ShowScreen(_rPanel.equipmentOperationArea1ListImg);
                    //_rPanel.SoundCheck();
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.SetBtns(true, RBtnType.수신기복구);
                    //_rPanel.GetSwitchButton("수신기복구").GetButton().onClick.RemoveAllListeners();
                    _rPanel.GetSwitchButton("수신기복구").Init(_rPanel.SwitchButtonClick, false);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.AddListener(recoveryBtnAction);

                    //_panelBtnCheck.checkDic["수신기복구"] = true;
                    SetHighlightControlPanel(_panelBtnCheck);
                    _panelBtnCheck.SetBtns(true,
                        RBtnType.수신기복구,
                        RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지,
                        RBtnType.부저정지,
                        RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                }
                break;
            case RTypeREquipmentOperationPSection.수신기복구:
                {
                     NextDisable();
                     ShowRoom(RTypeRRoomType.RTypeR);
                    //  _rPanel.SetFireAlarmEventNum(1);
                    // _soundManager.SetBuzzerVolume(0);
                    // _soundManager.SetAlarm2Volume();
                    // _soundManager.SetAlarmVolume();
                    // _soundManager.SetBroadcastVolume();
                    _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.RemoveListener(recoveryBtnAction);
                    _rPanel.ShowScreen(_rPanel.equipmentOperationEmptyListImg);
                    _rPanel.SetSwitchButtons(true, RBtnType.수신기복구,
                        RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지,
                        RBtnType.부저정지);
                    // _rPanel.GetSwitchButton("수신기복구").OnCheck(true);
                    _rPanel.GetSwitchButton("주음향정지").OnCheck(true);
                    _rPanel.GetSwitchButton("지구음향정지").OnCheck(true);
                    _rPanel.GetSwitchButton("비상방송정지").OnCheck(true);
                    _rPanel.GetSwitchButton("부저정지").OnCheck(true);
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.SetBtns(true,
                        RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지,
                        RBtnType.부저정지);
                    // _panelBtnCheck.checkDic["주음향정지"] = true;
                    // _panelBtnCheck.checkDic["지구음향정지"] = true;
                    // _panelBtnCheck.checkDic["비상방송정지"] = true;
                    // _panelBtnCheck.checkDic["부저정지"] = true;
                    SetHighlightControlPanel(_panelBtnCheck);
                    _panelBtnCheck.SetBtns(false,
                        RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지,
                        RBtnType.부저정지);
                    _panelBtnCheck.SetBtns(true,
                        RBtnType.수신기복구,
                        RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    // _panelBtnCheck.checkDic["주음향정지"] = false;
                    // _panelBtnCheck.checkDic["지구음향정지"] = false;
                    // _panelBtnCheck.checkDic["비상방송정지"] = false;
                    // _panelBtnCheck.checkDic["부저정지"] = false;
                    // _panelBtnCheck.checkDic["수신기복구"] = true;
                    // _panelBtnCheck.checkDic["메인펌프오토"] = true;
                    // _panelBtnCheck.checkDic["충압펌프오토"] = true;
                    // _panelBtnCheck.checkDic["sp펌프오토"] = true;
                    // _panelBtnCheck.checkDic["sp충압펌프오토"] = true;
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                }
                break;
            case RTypeREquipmentOperationPSection.방화문복구1:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.FireDoor);
                    smokeDetectorPopup.gameObject.SetActive(false);
                    OpenFireDoor(false);
                    //_rPanel.ShowScreen(_rPanel.equipmentOperationArea1ListImg);
                    _btnManager.EnableSpecificButton(GetFireDoorBtn(delegate
                    {
                        _btnManager.EnableSpecificButton();
                        OpenFireDoor(true);
                        NextEnable();
                    }));
                    _btnManager.EnableSpecificImage(_rPanel.equipmentOperationArea1ListHighlightiImg);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;

                }
                break;
            case RTypeREquipmentOperationPSection.방화문복구2:
                {
                    _btnManager.EnableSpecificImage();
                    ShowRoom(RTypeRRoomType.FireDoor);
                    //_rPanel.SetFireAlarmEventNum(1, true);
                    //_rPanel.SetEquipmentOperationEventNum(1);
                    //_rPanel.SoundCheck();
                    // SoundManager.Instance.SetDefaultVolume();
                    // SoundManager.Instance.SetBuzzerVolume(0);
                    // SoundManager.Instance.SetSirenVolume(0);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    _btnManager.EnableSpecificButton(areaManagerObj.GetRTypeRBtn(Next));
                    _rPanel.ShowScreen(_rPanel.equipmentOperationArea1ListImg);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.RemoveListener(recoveryBtnAction);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    NextDisable();
                }
                break;
            case RTypeREquipmentOperationPSection.초기화면:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.ShowScreen(_rPanel.equipmentOperationEmptyListImg);
                    _soundManager.ZeroVolume();
                    _btnManager.EnableSpecificButton(_rPanel.GetInitScreenBtn(Next));
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                }
                break;
            case RTypeREquipmentOperationPSection.초기화면이동:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.ShowScreen(_rPanel.mainScreenImg);
                    _btnManager.EnableSpecificButton();
                    
                    _soundManager.ZeroVolume();
                    
                    NextEnable(true, false);
                }
                break;
            // case RTypeREquipmentOperationPSection.수신기복구2:
            //     {
            //         NextDisable();
            //         ShowRoom(RTypeRRoomType.RTypeR);
            //         //_rPanel.SetSwitchButton(false);
            //         _rPanel.SetFireAlarmEventNum(1, true);
            //         _rPanel.SetEquipmentOperationEventNum(0);
            //         _rPanel.GetSwitchButton("수신기복구").OnCheck(false);
            //         _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = true;
            //         _rPanel.ShowScreen(_rPanel.equipmentOperationEmptyListImg);
            //         //_rPanel.SoundCheck();
            //         _panelBtnCheck.SetBtn(false);
            //         _panelBtnCheck.SetBtns(true, RBtnType.수신기복구);
            //         //_rPanel.GetSwitchButton("수신기복구").GetButton().onClick.RemoveAllListeners();
            //         _rPanel.GetSwitchButton("수신기복구").Init(_rPanel.SwitchButtonClick, false);
            //         _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.AddListener(recoveryBtnAction);
            //
            //         //_panelBtnCheck.checkDic["수신기복구"] = true;
            //         SetHighlightControlPanel(_panelBtnCheck);
            //         _panelBtnCheck.SetBtns(true,
            //             RBtnType.수신기복구,
            //             RBtnType.주음향정지,
            //             RBtnType.지구음향정지,
            //             RBtnType.비상방송정지,
            //             RBtnType.부저정지,
            //             RBtnType.메인펌프오토,
            //             RBtnType.충압펌프오토,
            //             RBtnType.sp펌프오토,
            //             RBtnType.sp충압펌프오토);
            //         SetHighlightControlPanelCheck(_panelBtnCheck);
            //     }
            //     break;
            // case RTypeREquipmentOperationPSection.음향복구1:
            //     {
            //         NextDisable();
            //         _soundManager.SetBuzzerVolume(0);
            //         _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.RemoveListener(recoveryBtnAction);
            //         _rPanel.ShowScreen(_rPanel.equipmentOperationEmptyListImg);
            //         _rPanel.SetSwitchButtons(true, RBtnType.수신기복구,
            //             RBtnType.주음향정지,
            //             RBtnType.지구음향정지,
            //             RBtnType.비상방송정지,
            //             RBtnType.부저정지);
            //         // _rPanel.GetSwitchButton("수신기복구").OnCheck(true);
            //         // _rPanel.GetSwitchButton("주음향정지").OnCheck(true);
            //         // _rPanel.GetSwitchButton("지구음향정지").OnCheck(true);
            //         // _rPanel.GetSwitchButton("비상방송정지").OnCheck(true);
            //         // _rPanel.GetSwitchButton("부저정지").OnCheck(true);
            //         _panelBtnCheck.SetBtn(false);
            //         _panelBtnCheck.SetBtns(true,
            //             RBtnType.주음향정지,
            //             RBtnType.지구음향정지,
            //             RBtnType.비상방송정지,
            //             RBtnType.부저정지);
            //         // _panelBtnCheck.checkDic["주음향정지"] = true;
            //         // _panelBtnCheck.checkDic["지구음향정지"] = true;
            //         // _panelBtnCheck.checkDic["비상방송정지"] = true;
            //         // _panelBtnCheck.checkDic["부저정지"] = true;
            //         SetHighlightControlPanel(_panelBtnCheck);
            //         _panelBtnCheck.SetBtns(false,
            //             RBtnType.주음향정지,
            //             RBtnType.지구음향정지,
            //             RBtnType.비상방송정지,
            //             RBtnType.부저정지);
            //         _panelBtnCheck.SetBtns(true,
            //             RBtnType.수신기복구,
            //             RBtnType.메인펌프오토,
            //             RBtnType.충압펌프오토,
            //             RBtnType.sp펌프오토,
            //             RBtnType.sp충압펌프오토);
            //         // _panelBtnCheck.checkDic["주음향정지"] = false;
            //         // _panelBtnCheck.checkDic["지구음향정지"] = false;
            //         // _panelBtnCheck.checkDic["비상방송정지"] = false;
            //         // _panelBtnCheck.checkDic["부저정지"] = false;
            //         // _panelBtnCheck.checkDic["수신기복구"] = true;
            //         // _panelBtnCheck.checkDic["메인펌프오토"] = true;
            //         // _panelBtnCheck.checkDic["충압펌프오토"] = true;
            //         // _panelBtnCheck.checkDic["sp펌프오토"] = true;
            //         // _panelBtnCheck.checkDic["sp충압펌프오토"] = true;
            //         SetHighlightControlPanelCheck(_panelBtnCheck);
            //         _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
            //     }
            //     break;
            // case RTypeREquipmentOperationPSection.초기화면2:
            //     {
            //         NextDisable();
            //         _rPanel.ShowScreen(_rPanel.equipmentOperationEmptyListImg);
            //         _soundManager.ZeroVolume();
            //         _btnManager.EnableSpecificButton(_rPanel.GetInitScreenBtn(delegate
            //         {
            //             _rPanel.ShowScreen(_rPanel.mainScreenImg);
            //             _btnManager.EnableSpecificButton();
            //             Next();
            //         }));
            //         _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
            //     }
            //     break;
            case RTypeREquipmentOperationPSection.교육완료:
                {
                    NextDisable();
                    SetCompletePopup("설비작동 조치를 모두 완료했습니다.", "하단의 버튼을 통해 다른 페이지로 이동해주세요.");
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        //_btnManager.HighlightObj();
    }

    private void RecoveryBtn()
    {
        RTypeRPanel.Instance.ShowScreen(RTypeRPanel.Instance.fireAlarmMap2Img);
        _gCanvas.totalScore.설비동작.평가4 = true;
        _rPanel.SetFireAlarmEventNum(0);
        _rPanel.SetEquipmentOperationEventNum(0);
        _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.RemoveListener(recoveryBtnAction);
    }
}
