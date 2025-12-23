using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public partial class GasSysSection
{
    [Foldout("수동조작함 작동")]
    private GasSysManualControlBoxPSection _curManualControlBoxPSection;
    private GasSysManualControlBoxESection _curManualControlBoxESection;
    [FormerlySerializedAs("manualControlBoxHintObj")]
    public HintScriptableObj manualControlBoxHintPObj;
    public HintScriptableObj manualControlBoxHintEObj;
    public GameObject 수동조작함작동평가1;
    [SerializeField] public Toggle 수동조작함작동토글1;
    [SerializeField] public Toggle 수동조작함작동토글2;
    [SerializeField] public Toggle 수동조작함작동토글3;
    [SerializeField] public Toggle 수동조작함작동토글4;
    [SerializeField] public Toggle 수동조작함작동토글5;
    [SerializeField] public Toggle 수동조작함작동토글6;
    [EndFoldout]
    public void InitManualControlBox()
    {
        InitEMode();
        curMainSection = GasSysMainSection.SolenoidValveTest;
        curSolenoidValveTestSection = GasSysSolenoidValveTestSection.ManualControlBox;
        nextBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.RemoveAllListeners();
        areaManagerObj?.Init();
        areaManagerObj?.StartArea(GasSysAreaManager.StartAreaType.StorageRoom);
        areaManagerObj?.ShowPanel(true);
        ShowRoom(storageRoomObj?.gameObject);
        //storageRoomObj?.InitSafetyCheck();
        inventoryObj?.Init();
        inventoryObj?.ShowPanel(true);
        ControlPanel.Instance.Init();
        //ChangeState(GasSysSafetyCheckSection.선택밸브조작동관선택);
        _maxSection = _gasSysState switch
        {
            //ChangeState(GasSysSafetyCheckSection.선택밸브조작동관선택);
            GasSysState.PracticeMode => System.Enum.GetValues(typeof(GasSysManualControlBoxPSection)).Length,
            GasSysState.EvaluationMode => System.Enum.GetValues(typeof(GasSysManualControlBoxESection)).Length,
            _ => _maxSection
        };
        SetSectionRange(0, _maxSection, _maxSection);
        nextBtn.onClick.AddListener(Next);
        prevBtn.onClick.AddListener(Prev);
        GasSysGlobalCanvas.Instance.ShowHint(true);
        GasSysGlobalCanvas.Instance.ShowCompletePopup(false);
        //SoundManager.Instance.MuteHint(true);
        ControlPanel.Instance.CheckWarringSwitch();
        ControlPanel.Instance.SoundCheck();
        ControlPanel.Instance.SetStorageRoomPopupParent();
        OpenActivationBox(true);
        ChangeValveState(false);
        ChangeStorageValveState(false);
        inventoryObj?.ShowSolenoidValve1(true);
        inventoryObj?.ShowSolenoidValve2(true);
        GasSysGlobalCanvas.Instance.HideCheckObj();
    }


    private void ShowManualControlBoxSection(int index)
    {
        if (_gasSysState.Equals(GasSysState.PracticeMode))
        {
            prevBtn.gameObject.SetActive(true);
            GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(manualControlBoxHintPObj, (int)GasSysManualControlBoxPSection.감시제어반선택 + index));
            ChangeStateManualControlBoxP(GasSysManualControlBoxPSection.감시제어반선택 + index);
        }
        if (!_gasSysState.Equals(GasSysState.EvaluationMode))
            return;
        prevBtn.gameObject.SetActive(false);
        GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(manualControlBoxHintEObj, (int)GasSysManualControlBoxESection.Init + index));
        ChangeStateManualControlBoxE(GasSysManualControlBoxESection.Init + index);
    }
    private void ChangeStateManualControlBoxE(GasSysManualControlBoxESection state)
    {
        _curManualControlBoxESection = state;
        OnStateChangedManualControlBoxE(_curManualControlBoxESection);
    }

    private void OnStateChangedManualControlBoxE(GasSysManualControlBoxESection state)
    {
        uiDragAndCollisionHandler.StopDragging();
        ControlPanel.OnTimerEnd.RemoveAllListeners();
        ButtonManager.Instance.RemoveAllHighlights();
        switch (state)
        {
            case GasSysManualControlBoxESection.Init:
                {
                    InitStorageRoom();
                    InitArea1Corridor();
                    NextEnable(false);
                    _gasSysEvaluation.Reset();
                    ButtonManager.Instance.EnableSpecificImage(activationCylinderBoxPopup.aActivationObj,
                        activationCylinderBoxPopup.pinObj);

                    controlBoxBtn.gameObject.SetActive(true);
                    selectionValveBtn.gameObject.SetActive(true);
                    storageCylinderBtn.gameObject.SetActive(true);
                    activationBoxBtn.gameObject.SetActive(true);
                    manualControlBoxBtn.gameObject.SetActive(true);
                    manualControlBoxPopup.gameObject.SetActive(false);
                    areaManagerObj.area2Btn.interactable = false;
                    areaManagerObj.area1Btn.interactable = false;
                    areaManagerObj.area2CorridorBtn.interactable = false;

                    areaManagerObj.area1CorridorBtn.onClick.RemoveAllListeners();
                    areaManagerObj.storageRoomBtn.onClick.RemoveAllListeners();
                    controlBoxBtn.onClick.RemoveAllListeners();
                    selectionValveBtn.onClick.RemoveAllListeners();
                    storageCylinderBtn.onClick.RemoveAllListeners();
                    activationBoxBtn.onClick.RemoveAllListeners();
                    manualControlBoxBtn.onClick.RemoveAllListeners();


                    selectionValveBtn.onClick.AddListener(delegate
                    {
                        selectionValvePopup.InitPopupE(ChangeValveState, true);
                    });
                    storageCylinderBtn.onClick.AddListener(delegate
                    {
                        storageCylinderPopup.InitPopupE(ChangeStorageValveState, true);
                    });
                    controlBoxBtn.onClick.AddListener(delegate
                    {
                        ControlPanel.Instance.ShowPanel(true);
                    });

                    selectionValvePopup.SetOnOff(false);
                    storageCylinderPopup.SetOnOff(false);

                    activationBoxBtn.onClick.AddListener(delegate
                    {
                        activationCylinderBoxPopup.Init();
                        activationCylinderBoxPopup.closeBtn.gameObject.SetActive(true);
                        activationCylinderBoxPopup.closeBtn.onClick.AddListener(delegate
                        {
                            activationCylinderBoxPopup.gameObject.SetActive(false);
                        });
                        activationCylinderBoxPopup.gameObject.SetActive(true);
                    });
                    areaManagerObj.area1CorridorBtn.onClick.AddListener(delegate
                    {
                        ControlPanel.Instance.SetArea1CorridorPopupParent();
                        ShowRoom(area1CorridorObj?.gameObject);
                    });
                    areaManagerObj.storageRoomBtn.onClick.AddListener(delegate
                    {
                        ControlPanel.Instance.SetStorageRoomPopupParent();
                        ShowRoom(storageRoomObj?.gameObject);
                    });
                    manualControlBoxBtn.onClick.AddListener(delegate
                    {
                        //manualControlBoxPopup.selectOpenBtn.gameObject.SetActive(true);
                        manualControlBoxPopup.InitManualControlBox(delegate
                        {
                            ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                            ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                            SoundManager.Instance.PlayBuzzer(true);
                            ControlPanel.Instance.SetArea1CorridorPopupParent();
                            ControlPanel.Instance.ShowPanel(true);
                            manualControlBoxPopup.closeObj.SetActive(true);
                            manualControlBoxPopup.selectOpenBtn.gameObject.SetActive(false);
                            manualControlBoxPopup.selectActivateDischargeBtn.gameObject.SetActive(true);
                            //manualControlBoxPopup.selectActivateDischargeBtn.onClick.AddListener(Next);
                            manualControlBoxPopup.gameObject.SetActive(true);
                            manualControlBoxPopup.selectOpenBtn.gameObject.SetActive(true);
                        }, delegate
                        {
                            manualControlBoxPopup.gameObject.SetActive(false);
                            ControlPanel.Instance.ShowPanel(true);
                            ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                            ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                            solenoidValvePopup.clipObj.SetActive(true);
                            solenoidValvePopup.SetArea1CorridorPopupParent();
                            solenoidValvePopup.gameObject.SetActive(true);
                            //storageRoomObj?.ShowSolenoidValvePopup(true);
                            inventoryObj?.ShowSolenoidValve1(false);
                            ControlPanel.Instance.ShowFire(true);
                            ControlPanel.Instance.SetTimeNum(30f);
                            ControlPanel.Instance.StartTimer(30);
                            ControlPanel.OnTimerEnd.RemoveAllListeners();
                            ControlPanel.OnTimerEnd.AddListener(delegate
                            {
                                solenoidValvePopup.SolenoidValveAActivation();
                                ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                                ControlPanel.Instance.ShowFire(true);
                                ControlPanel.Instance.SetTimeNum(0f);

                            });
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);
                            manualControlBoxPopup.selectActivateDischargeBtn.gameObject.SetActive(false);
                        }, false);
                        manualControlBoxPopup.gameObject.SetActive(true);
                    });
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    ControlPanel.Instance.CheckSwitchBtn();
                    ControlPanel.Instance.SetClose();
                    ButtonManager.Instance.SetEvaluationButtons();
                    Next();
                }
                break;
            case GasSysManualControlBoxESection.E1:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    var switchBtnList = ControlPanel.Instance.controlPanelButton;
                    _gasSysEvaluation.주경종 = switchBtnList.주경종;
                    _gasSysEvaluation.지구경종 = switchBtnList.지구경종;
                    _gasSysEvaluation.사이렌 = switchBtnList.사이렌;
                    _gasSysEvaluation.비상방송 = switchBtnList.비상방송;
                    _gasSysEvaluation.부저 = switchBtnList.부저;
                    _gasSysEvaluation.축적 = switchBtnList.축적;
                    _gasSysEvaluation.솔레노이드연동 = ControlMode.Auto.Equals(switchBtnList.솔레노이드밸브);
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    {
                        _gasSysEvaluation.주경종 = switchBtn.주경종;
                        _gasSysEvaluation.지구경종 = switchBtn.지구경종;
                        _gasSysEvaluation.사이렌 = switchBtn.사이렌;
                        _gasSysEvaluation.비상방송 = switchBtn.비상방송;
                        _gasSysEvaluation.부저 = switchBtn.부저;
                        _gasSysEvaluation.축적 = switchBtn.축적;
                        _gasSysEvaluation.솔레노이드연동 = ControlMode.Auto.Equals(switchBtn.솔레노이드밸브);
                    });
                }
                break;
            case GasSysManualControlBoxESection.E2:
                {
                    ShowRoom(area1CorridorObj?.gameObject);
                    activationCylinderBoxPopup.gameObject.SetActive(false);
                    manualControlBoxPopup.gameObject.SetActive(false);
                    solenoidValvePopup.gameObject.SetActive(false);
                    ControlPanel.Instance.InitTimer();
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    ControlPanel.Instance.ShowPanel(false);
                    SoundManager.Instance.StopAllFireSound();
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    ControlPanel.Instance.CheckSwitchBtn();


                    manualControlBoxBtn.gameObject.SetActive(true);
                    manualControlBoxBtn.interactable = true;
                    manualControlBoxBtn.onClick.RemoveAllListeners();
                    manualControlBoxBtn.onClick.AddListener(delegate
                    {
                        manualControlBoxPopup.selectOpenBtn.interactable = true;
                        manualControlBoxPopup.InitManualControlBox(delegate
                        {
                            ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                            ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                            SoundManager.Instance.PlayBuzzer(true);
                            ControlPanel.Instance.SetArea1CorridorPopupParent();
                            ControlPanel.Instance.ShowPanel(true);
                            manualControlBoxPopup.closeObj.SetActive(true);
                            manualControlBoxPopup.selectOpenBtn.gameObject.SetActive(false);
                            manualControlBoxPopup.selectActivateDischargeBtn.gameObject.SetActive(true);
                            //manualControlBoxPopup.selectActivateDischargeBtn.onClick.AddListener(Next);
                            manualControlBoxPopup.gameObject.SetActive(true);
                            manualControlBoxPopup.selectOpenBtn.gameObject.SetActive(true);
                        }, delegate
                        {
                            manualControlBoxPopup.gameObject.SetActive(false);
                            ControlPanel.Instance.ShowPanel(true);
                            ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                            ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                            ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                            solenoidValvePopup.clipObj.SetActive(true);
                            solenoidValvePopup.SetArea1CorridorPopupParent();
                            solenoidValvePopup.gameObject.SetActive(true);
                            //storageRoomObj?.ShowSolenoidValvePopup(true);
                            inventoryObj?.ShowSolenoidValve1(false);
                            ControlPanel.Instance.ShowFire(true);
                            ControlPanel.Instance.SetTimeNum(30f);
                            ControlPanel.Instance.StartTimer(30);
                            ControlPanel.OnTimerEnd.RemoveAllListeners();
                            _gasSysEvaluation.격발 = true;
                            ControlPanel.OnTimerEnd.AddListener(delegate
                            {
                                solenoidValvePopup.SolenoidValveAActivation();
                                ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                                ControlPanel.Instance.ShowFire(true);
                                ControlPanel.Instance.SetTimeNum(0f);

                            });
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);
                            manualControlBoxPopup.selectActivateDischargeBtn.gameObject.SetActive(false);
                        }, false);
                        manualControlBoxPopup.gameObject.SetActive(true);
                    });


                    // manualControlBoxPopup.selectOpenBtn.onClick.AddListener(delegate
                    // {
                    //     SoundManager.Instance.PlayBuzzer(true);
                    //     ControlPanel.Instance.SetArea1CorridorPopupParent();
                    //     ControlPanel.Instance.ShowPanel(true);
                    //     manualControlBoxPopup.openObj.SetActive(true);
                    //     manualControlBoxPopup.selectOpenBtn.gameObject.SetActive(false);
                    //     manualControlBoxPopup.selectActivateDischargeBtn.gameObject.SetActive(true);
                    //     ButtonManager.Instance.EnableSpecificButton(manualControlBoxPopup.selectActivateDischargeBtn);
                    // });
                    //ButtonManager.Instance.EnableSpecificButton(manualControlBoxPopup.selectOpenBtn);
                    //NextDisable();

                }
                break;
            case GasSysManualControlBoxESection.E3:
                {
                    ControlPanel.Instance.SetTimeNum(0f);
                    ControlPanel.Instance.ResetTimer();
                    solenoidValvePopup.SolenoidValveAActivation(false);
                    수동조작함작동평가1.SetActive(true);
                    uiDragAndCollisionHandler.StopDragging();
                    ControlPanel.Instance.SetArea1CorridorPopupParent();
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.ShowFire(true);
                    solenoidValvePopup.clipObj.SetActive(true);
                    solenoidValvePopup.SetArea1CorridorPopupParent();
                    solenoidValvePopup.gameObject.SetActive(true);
                    ControlPanel.Instance.closeBtn.gameObject.SetActive(false);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    SoundManager.Instance.StopAllFireSound();
                    //ButtonManager.Instance.DisableAllButtons();
                    NextEnable(false);
                    //solenoidValvePopup.gameObject.SetActive(false);
                }
                break;
            case GasSysManualControlBoxESection.평가종료:
                {
                    수동조작함작동평가1.SetActive(false);
                    GasSysGlobalCanvas.Instance.totalScore.수동조작함작동.감시제어반조치 = !_gasSysEvaluation.주경종 && !_gasSysEvaluation.지구경종 && !_gasSysEvaluation.사이렌 &&
                                                                             !_gasSysEvaluation.비상방송 && !_gasSysEvaluation.부저 && _gasSysEvaluation.축적 &&
                                                                             _gasSysEvaluation.솔레노이드연동;
                    GasSysGlobalCanvas.Instance.totalScore.수동조작함작동.격발 = _gasSysEvaluation.격발;
                    GasSysGlobalCanvas.Instance.totalScore.수동조작함작동.동작확인 = !수동조작함작동토글1.isOn &&
                                                                          !수동조작함작동토글2.isOn &&
                                                                          수동조작함작동토글3.isOn &&
                                                                          !수동조작함작동토글4.isOn &&
                                                                          !수동조작함작동토글5.isOn &&
                                                                          !수동조작함작동토글6.isOn;
                    var results = new List<ResultObject>
                    {
                        new ResultObject()
                        {
                            Title = "감시제어반 조치",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.수동조작함작동.감시제어반조치
                        },
                        new ResultObject()
                        {
                            Title = "수동조작함 작동 격발시험",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.수동조작함작동.격발
                        },
                        new ResultObject()
                        {
                            Title = "정상작동 상태확인",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.수동조작함작동.동작확인
                        }
                    };
                    GasSysGlobalCanvas.Instance.totalScore.수동조작함작동List.Clear();
                    GasSysGlobalCanvas.Instance.totalScore.수동조작함작동List.AddRange(results);
                    GasSysGlobalCanvas.Instance.SetNextEvaluation(InitCrossCircuitDetector, InitManualControlBox);
                    //GasSysGlobalCanvas.Instance.SetResultPopup(results);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void ChangeStateManualControlBoxP(GasSysManualControlBoxPSection state)
    {
        _curManualControlBoxPSection = state;
        OnStateChangedManualControlBoxP(_curManualControlBoxPSection);
    }

    private void OnStateChangedManualControlBoxP(GasSysManualControlBoxPSection state)
    {
        InitStorageRoom();
        InitArea1Corridor();
        ButtonManager.Instance.RemoveAllHighlights();
        ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
        ControlPanel.OnTimerCheck.RemoveAllListeners();
        //SoundManager.Instance.StopAllFireSound();
        //ControlPanel.Instance.ShowFire(false);
        //ControlPanel.Instance.SetTimeNum(30);
        //ControlPanel.Instance.ResetTimer();
        uiDragAndCollisionHandler.ResetEvent();
        switch (state)
        {

            case GasSysManualControlBoxPSection.감시제어반선택:
                {
                    
                    ShowRoom(storageRoomObj.gameObject);
                    controlBoxBtn.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificButton(controlBoxBtn);
                    NextDisable();
                    controlBoxBtn.onClick.AddListener(delegate
                    {
                        controlBoxBtn.gameObject.SetActive(false);
                        //ControlPanel.Instance.ShowPanel(true);
                        Next();
                    });
                }
                break;
            case GasSysManualControlBoxPSection.제어반음향및솔레노이드밸브연동:
                {
                    ShowRoom(storageRoomObj.gameObject);
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.SetStorageRoomPopupParent();
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(true);
                    temp.솔레노이드밸브 = ControlMode.Auto;
                    temp.축적 = false;
                    temp.복구 = false;
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(false);
                    temp.축적 = true;
                    temp.솔레노이드밸브 = ControlMode.Auto;
                    SetHighlightControlPanelCheck(temp);
                }
                break;
            /*
            case GasSysManualControlBoxPSection.축적비축적유지:
                {
                    ShowRoom(storageRoomObj.gameObject);
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.SetStorageRoomPopupParent();
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(false);
                    temp.축적 = true;
                    SetHighlightControlPanelCheck(temp);
                    NextEnable();
                }
                break;
                */
            case GasSysManualControlBoxPSection.방호구역1번복도:
                {
                    ShowRoom(storageRoomObj.gameObject);
                    ButtonManager.Instance.EnableSpecificButton(areaManagerObj.area1CorridorBtn);
                    NextDisable();
                    areaManagerObj.area1CorridorBtn.onClick.RemoveAllListeners();
                    areaManagerObj.area1CorridorBtn.onClick.AddListener(Next);
                }
                break;
            case GasSysManualControlBoxPSection.수동조작함선택:
                {
                    ShowRoom(area1CorridorObj.gameObject);
                    manualControlBoxBtn.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificButton(manualControlBoxBtn);
                    NextDisable();
                    manualControlBoxBtn.onClick.AddListener(delegate
                    {
                        //GlobalCanvas.Instance.SetHintPopup(5,5, _manualControlBoxControllerHint, _manualControlBoxControllerHintRects[0]);
                        //GlobalCanvas.Instance.ShowHint(true);
                        manualControlBoxBtn.gameObject.SetActive(false);
                        Next();
                    });
                }
                break;
            case GasSysManualControlBoxPSection.수동조작함개방및기동스위치:
                {
                    var btnList = new List<SwitchButtonCheck>();

                    SoundManager.Instance.PlayBuzzer(false);
                    ShowRoom(area1CorridorObj.gameObject);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    manualControlBoxPopup.InitManualControlBox(delegate
                    {
                        // SoundManager.Instance.PlayBuzzer(true);
                        // ControlPanel.Instance.SetArea1CorridorPopupParent();
                        // ControlPanel.Instance.ShowPanel(true);
                        // manualControlBoxPopup.closeObj.SetActive(true);
                        // manualControlBoxPopup.selectOpenBtn.gameObject.SetActive(false);
                        // manualControlBoxPopup.selectActivateDischargeBtn.gameObject.SetActive(true);
                        // ButtonManager.Instance.EnableSpecificButton(manualControlBoxPopup.selectActivateDischargeBtn);
                        Next();
                    });
                    manualControlBoxPopup.gameObject.SetActive(true);
                    manualControlBoxPopup.selectOpenBtn.gameObject.SetActive(true);
                    // manualControlBoxPopup.selectOpenBtn.onClick.AddListener(delegate
                    // {
                    //     SoundManager.Instance.PlayBuzzer(true);
                    //     ControlPanel.Instance.SetArea1CorridorPopupParent();
                    //     ControlPanel.Instance.ShowPanel(true);
                    //     manualControlBoxPopup.openObj.SetActive(true);
                    //     manualControlBoxPopup.selectOpenBtn.gameObject.SetActive(false);
                    //     manualControlBoxPopup.selectActivateDischargeBtn.gameObject.SetActive(true);
                    //     ButtonManager.Instance.EnableSpecificButton(manualControlBoxPopup.selectActivateDischargeBtn);
                    // });
                    ButtonManager.Instance.EnableSpecificButton(manualControlBoxPopup.selectOpenBtn);
                    NextDisable();
                    GasSysGlobalCanvas.Instance.HideCheckObj();
                }
                break;
            case GasSysManualControlBoxPSection.수동조작함개방및기동스위치1:
                {
                    var btnList = new List<SwitchButtonCheck>();

                    ShowRoom(area1CorridorObj.gameObject);
                    ControlPanel.Instance.ShowFire(false);
                    SoundManager.Instance.StopAllFireSound();
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    SoundManager.Instance.PlayBuzzer(true);
                    ControlPanel.Instance.SetArea1CorridorPopupParent();
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.SetTimeNum(30f);
                    ControlPanel.Instance.InitTimer();
                    manualControlBoxPopup.closeObj.SetActive(true);
                    manualControlBoxPopup.selectOpenBtn.gameObject.SetActive(false);
                    manualControlBoxPopup.selectActivateDischargeBtn.gameObject.SetActive(true);
                    manualControlBoxPopup.selectActivateDischargeBtn.onClick.RemoveAllListeners();
                    manualControlBoxPopup.selectActivateDischargeBtn.onClick.AddListener(Next);
                    ButtonManager.Instance.EnableSpecificButton(manualControlBoxPopup.selectActivateDischargeBtn);
                    manualControlBoxPopup.gameObject.SetActive(true);
                    manualControlBoxPopup.selectOpenBtn.gameObject.SetActive(true);
                    // manualControlBoxPopup.selectOpenBtn.onClick.AddListener(delegate
                    // {
                    //     SoundManager.Instance.PlayBuzzer(true);
                    //     ControlPanel.Instance.SetArea1CorridorPopupParent();
                    //     ControlPanel.Instance.ShowPanel(true);
                    //     manualControlBoxPopup.openObj.SetActive(true);
                    //     manualControlBoxPopup.selectOpenBtn.gameObject.SetActive(false);
                    //     manualControlBoxPopup.selectActivateDischargeBtn.gameObject.SetActive(true);
                    //     ButtonManager.Instance.EnableSpecificButton(manualControlBoxPopup.selectActivateDischargeBtn);
                    // });
                    //ButtonManager.Instance.EnableSpecificButton(manualControlBoxPopup.selectOpenBtn);
                    NextDisable();
                    GasSysGlobalCanvas.Instance.HideCheckObj();
                }
                break;
            case GasSysManualControlBoxPSection.수동조작함개방및기동스위치2:
                {
                    var btnList = new List<SwitchButtonCheck>();

                    ShowRoom(area1CorridorObj.gameObject);
                    manualControlBoxPopup.gameObject.SetActive(false);
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
                        select = true
                    });
                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
                        select = true
                    });
                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
                        select = true
                    });
                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
                        select = true
                    });
                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
                        select = true
                    });
                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("축적/비축적").GetButton(),
                        select = true
                    });

                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSolenoidSelectBtn(),
                        select = true
                    });

                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetArea1SelectBtn(),
                        select = true
                    });

                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetArea2SelectBtn(),
                        select = true
                    });
                    ButtonManager.Instance.EnableSpecificButton(btnList);
                    solenoidValvePopup.clipObj.SetActive(true);
                    solenoidValvePopup.SetArea1CorridorPopupParent();
                    solenoidValvePopup.gameObject.SetActive(true);
                    //storageRoomObj?.ShowSolenoidValvePopup(true);
                    inventoryObj?.ShowSolenoidValve1(false);
                    ControlPanel.Instance.ShowFire(true);
                    ControlPanel.Instance.SetTimeNum(30f);
                    ControlPanel.Instance.StartTimer(30);
                    ControlPanel.OnTimerCheck.RemoveAllListeners();
                    ControlPanel.OnTimerCheck.AddListener((time) =>
                    {
                        if (!(time < 10f))
                            return;
                        GasSysGlobalCanvas.Instance.SetHintTextAndAudio("지연장치의 지연시간을 확인하고 다음버튼을 누르세요.", hintClip);
                        ControlPanel.Instance.SetTimeNum(10f);
                        ControlPanel.Instance.ResetTimer();
                        ButtonManager.Instance.EnableSpecificImage(ControlPanel.Instance.timerPanelObj.numPanel.gameObject);
                        ButtonManager.Instance.HighlightObj();
                        NextEnable(false);
                    });

                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);
                    manualControlBoxPopup.selectActivateDischargeBtn.gameObject.SetActive(false);



                    // manualControlBoxPopup.selectOpenBtn.onClick.AddListener(delegate
                    // {
                    //     SoundManager.Instance.PlayBuzzer(true);
                    //     ControlPanel.Instance.SetArea1CorridorPopupParent();
                    //     ControlPanel.Instance.ShowPanel(true);
                    //     manualControlBoxPopup.openObj.SetActive(true);
                    //     manualControlBoxPopup.selectOpenBtn.gameObject.SetActive(false);
                    //     manualControlBoxPopup.selectActivateDischargeBtn.gameObject.SetActive(true);
                    //     ButtonManager.Instance.EnableSpecificButton(manualControlBoxPopup.selectActivateDischargeBtn);
                    // });
                    //ButtonManager.Instance.EnableSpecificButton(manualControlBoxPopup.selectOpenBtn);
                    NextDisable();
                    GasSysGlobalCanvas.Instance.HideCheckObj();
                }
                break;
            
            case GasSysManualControlBoxPSection.솔정상확인:
                {
                    
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    // ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    ControlPanel.Instance.ShowFire(true);
                    ControlPanel.Instance.SetTimeNum(10f);
                    ControlPanel.Instance.StartTimer(10);
                    ControlPanel.OnTimerEnd.RemoveAllListeners();
                    solenoidValvePopup.clipObj.SetActive(true);
                    solenoidValvePopup.SetArea1CorridorPopupParent();
                    solenoidValvePopup.gameObject.SetActive(true);
                    ControlPanel.OnTimerEnd.AddListener(delegate
                    {
                        ControlPanel.Instance.SetTimeNum(0f);
                        solenoidValvePopup.SolenoidValveAActivation();
                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                        //Next();
                        solenoidValvePopup.gameObject.SetActive(false);
                        GasSysGlobalCanvas.Instance.ShowSol2(Next);
                        NextEnable(false);

                        // Observable.Timer(System.TimeSpan.FromSeconds(3))
                        //     .Subscribe(_ =>
                        //     {
                        //         //GasSysManager.Instance.ChangeState(GasSysState.Init);
                        //     })
                        //     .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
                    });
                    
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(false);
                    temp.축적 = true;
                    SetHighlightControlPanelCheck(temp, false);
                    
                }
                break;
                
            case GasSysManualControlBoxPSection.음향정지작동확인:
                {
                    GasSysGlobalCanvas.Instance.HideCheckObj();
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    // ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(true);
                    temp.축적 = false;
                    temp.복구 = false;
                    //SetHighlightControlPanel(temp);
                    //NextEnable(false);
                    NextDisable();
                    temp.SetBtn(true);
                    temp.축적 = true;
                    temp.복구 = false;
                    SetHighlightControlPanel(temp, true);
                    SetHighlightControlPanelCheck(temp, true);
                    // _switchButtons.Add(new SwitchButtonCheck()
                    // {
                    //     btn = GlobalCanvas.Instance.GetCheckAgreeBtn(),
                    //     select = false
                    // });
                    //ButtonManager.Instance.EnableSpecificButton(_switchButtons);
                    //GasSysGlobalCanvas.Instance.ShowSol3(Next);
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                }
                break;
            case GasSysManualControlBoxPSection.제어반화재표시:
                {
                    NextDisable();
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    GasSysGlobalCanvas.Instance.ShowFire(Next);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    temp.축적 = false;
                    SetHighlightControlPanel(temp);
                    //NextDisable();
                    NextEnable(false);
                    temp.SetBtn(true);
                    temp.축적 = true;
                    temp.복구 = false;
                    SetHighlightControlPanelCheck(temp);
                    // _switchButtons.Add(new SwitchButtonCheck()
                    // {
                    //     btn = GlobalCanvas.Instance.GetCheckAgreeBtn(),
                    //     select = false
                    // });
                    //ButtonManager.Instance.EnableSpecificButton(_switchButtons);
                }
                break;
            /*
            case GasSysManualControlBoxPSection.방호구역1:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    GasSysGlobalCanvas.Instance.ShowArea1(Next);
                    SoundManager.Instance.SetSirenVolume();
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    temp.축적 = false;
                    temp.복구 = false;
                    SetHighlightControlPanel(temp);
                    //NextDisable();
                    NextEnable(false);
                    temp.SetBtn(true);
                    temp.축적 = true;
                    SetHighlightControlPanelCheck(temp);
                    // _switchButtons.Add(new SwitchButtonCheck()
                    // {
                    //     btn = GlobalCanvas.Instance.GetCheckAgreeBtn(),
                    //     select = false
                    // });
                    //ButtonManager.Instance.EnableSpecificButton(_switchButtons);
                }
                break;
                */
            case GasSysManualControlBoxPSection.방호구역1음향연동:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    // var temp = new ControlPanelButtonCheck();
                    // temp.SetBtn(true);
                    // temp.축적 = false;
                    // temp.복구 = false;
                    // //SetHighlightControlPanel(temp);
                    // NextDisable();
                    // //NextEnable(false);
                    // temp.SetBtn(false);
                    // temp.축적 = true;
                    // temp.복구 = false;
                    // SetHighlightControlPanel(temp, true);
                    // SetHighlightControlPanelCheck(temp);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    temp.주경종 = true;
                    temp.축적 = true;
                    temp.부저 = true;
                    temp.복구 = false;
                    SetHighlightControlPanel(temp, true);
                    NextDisable();
                    //NextEnable(false);
                    temp.SetBtn(false);
                    temp.주경종 = true;
                    temp.축적 = true;
                    temp.부저 = true;
                    temp.축적 = true;
                    temp.복구 = false;
                    SetHighlightControlPanelCheck(temp);
                    GasSysGlobalCanvas.Instance.ShowArea1(Next);
                    SoundManager.Instance.SetSirenVolume();
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                    //ButtonManager.Instance.EnableSpecificButton(_switchButtons);
                }
                break;
            case GasSysManualControlBoxPSection.음향정지:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(true);
                    temp.축적 = false;
                    temp.복구 = false;
                    //SetHighlightControlPanel(temp);
                    NextDisable();
                    //NextEnable(false);
                    temp.SetBtn(true);
                    temp.복구 = false;
                    SetHighlightControlPanel(temp, true);
                    SetHighlightControlPanelCheck(temp);
                    GasSysGlobalCanvas.Instance.ShowArea1(Next);
                    SoundManager.Instance.SetSirenVolume();
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                }
                break;
            case GasSysManualControlBoxPSection.방호구역2:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    SoundManager.Instance.MuteAll(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    // var temp = new ControlPanelButtonCheck();
                    // temp.SetBtn(true);
                    // temp.축적 = false;
                    // temp.복구 = false;
                    // //SetHighlightControlPanel(temp);
                    // NextDisable();
                    // temp.SetBtn(false);
                    // temp.축적 = true;
                    // temp.복구 = false;
                    // //NextEnable(false);
                    // SetHighlightControlPanel(temp, true);
                    // SetHighlightControlPanelCheck(temp);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(true);
                    temp.축적 = false;
                    temp.복구 = false;
                    //SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(true);
                    temp.지구경종 = false;
                    temp.사이렌 = false;
                    temp.비상방송 = false;
                    temp.축적 = true;
                    temp.복구 = false;
                    //NextEnable(false);
                    SetHighlightControlPanel(temp, true);
                    SetHighlightControlPanelCheck(temp);
                    ControlPanel.Instance.SetTimeNum(0f);
                    GasSysGlobalCanvas.Instance.ShowArea2(Next);
                    SoundManager.Instance.SetSirenVolume(0f);
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                    ControlPanel.OnTimerEnd.RemoveAllListeners();
                }
                break;
            /*
            case GasSysManualControlBoxPSection.지연장치:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    GasSysGlobalCanvas.Instance.ShowTimer(Next);
                    SoundManager.Instance.SetSirenVolume();
                    //GlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(true);
                    //SetHighlightControlPanelCheck(temp, false);
                    NextDisable();
                    _switchButtons.Add(new SwitchButtonCheck()
                    {
                        btn = GasSysGlobalCanvas.Instance.GetCheckAgreeBtn(),
                        select = false
                    });
                    ButtonManager.Instance.EnableSpecificButton(_switchButtons);
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                    ControlPanel.Instance.StartTimer(30);
                    ControlPanel.OnTimerEnd.RemoveAllListeners();
                    ControlPanel.OnTimerEnd.AddListener(delegate
                    {
                        //solenoidValvePopup.SolenoidValveAActivation();
                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                        NextEnable(false);
                        //GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(true);
                        // Observable.Timer(System.TimeSpan.FromSeconds(3))
                        //     .Subscribe(_ =>
                        //     {
                        //         //GasSysManager.Instance.ChangeState(GasSysState.Init);
                        //     })
                        //     .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
                    });
                }
                break;
            case GasSysManualControlBoxPSection.솔레노이드작동여부:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    GasSysGlobalCanvas.Instance.ShowSol2(Next);
                    SoundManager.Instance.MuteSiren(false);
                    // GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(true);
                    // _switchButtons.Add(new SwitchButtonCheck()
                    // {
                    //     btn = GasSysGlobalCanvas.Instance.GetCheckAgreeBtn(),
                    //     select = false
                    // });
                    // ButtonManager.Instance.EnableSpecificButton(_switchButtons);
                    NextEnable(false);
                }
                break;
                */
            case GasSysManualControlBoxPSection.교육종료:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    SetCompletePopup("수동조작함 작동 시험을 모두 완료했습니다.", "다음 점검을", InitCrossCircuitDetector);
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        ControlPanel.Instance.CheckWarringSwitch();
        //ButtonManager.Instance.HighlightObj();
    }
}
