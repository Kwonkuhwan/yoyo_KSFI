using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public partial class GasSysSection
{
    [Foldout("제어반 수동조작 스위치 작동")]
    private GasSysControlPanelSwitchPSection _curControlPanelSwitchPSection;
    private GasSysControlPanelSwitchESection _curControlPanelSwitchESection;
    [FormerlySerializedAs("controlPanelSwitchPHintObj")]
    [FormerlySerializedAs("controlPanelSwitchHintObj")]
    [FormerlySerializedAs("hintScriptableObj")]
    public HintScriptableObj controlPanelSwitchHintPObj;
    public HintScriptableObj controlPanelSwitchHintEObj;
    public GameObject 제어반수동조작평가1;
    [SerializeField] public Toggle 제어반수동조작토글1;
    [SerializeField] public Toggle 제어반수동조작토글2;
    [SerializeField] public Toggle 제어반수동조작토글3;
    [SerializeField] public Toggle 제어반수동조작토글4;
    [SerializeField] public Toggle 제어반수동조작토글5;
    [SerializeField] public Toggle 제어반수동조작토글6;
    [EndFoldout]
    public void InitControlPanelSwitch()
    {
        InitEMode();
        curMainSection = GasSysMainSection.SolenoidValveTest;
        curSolenoidValveTestSection = GasSysSolenoidValveTestSection.ControlPanelSwitch;
        nextBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.RemoveAllListeners();
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
            GasSysState.PracticeMode => System.Enum.GetValues(typeof(GasSysControlPanelSwitchPSection)).Length,
            GasSysState.EvaluationMode => System.Enum.GetValues(typeof(GasSysControlPanelSwitchESection)).Length,
            _ => _maxSection
        };
        SetSectionRange(0, _maxSection, _maxSection);
        nextBtn.onClick.AddListener(Next);
        prevBtn.onClick.AddListener(Prev);
        GasSysGlobalCanvas.Instance.ShowCompletePopup(false);
        GasSysGlobalCanvas.Instance.ShowHint(true);
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



    private void ShowControlPanelSwitchSection(int index)
    {
        if (_gasSysState.Equals(GasSysState.PracticeMode))
        {
            prevBtn.gameObject.SetActive(true);
            if (!_maxSection.Equals(index))
            {
                GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(controlPanelSwitchHintPObj, index));
            }
            ChangeStateControlPanelSwitchP(GasSysControlPanelSwitchPSection.감시제어반선택 + index);
        }
        if (!_gasSysState.Equals(GasSysState.EvaluationMode))
            return;
        prevBtn.gameObject.SetActive(false);
        GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(controlPanelSwitchHintEObj, (int)GasSysControlPanelSwitchESection.Init + index));
        ChangeStateControlPanelSwitchE(GasSysControlPanelSwitchESection.Init + index);

    }

    public void ChangeStateControlPanelSwitchE(GasSysControlPanelSwitchESection state)
    {

        _curControlPanelSwitchESection = state;
        OnStateChangedControlPanelSwitchE(_curControlPanelSwitchESection);
    }

    private void OnStateChangedControlPanelSwitchE(GasSysControlPanelSwitchESection state)
    {
        switch (state)
        {

            case GasSysControlPanelSwitchESection.Init:
                {
                    InitStorageRoom();
                    NextEnable(false);
                    _gasSysEvaluation.Reset();
                    controlBoxBtn.gameObject.SetActive(true);
                    selectionValveBtn.gameObject.SetActive(true);
                    storageCylinderBtn.gameObject.SetActive(true);
                    activationBoxBtn.gameObject.SetActive(true);
                    manualControlBoxBtn.gameObject.SetActive(true);

                    areaManagerObj.area1CorridorBtn.onClick.RemoveAllListeners();
                    areaManagerObj.storageRoomBtn.onClick.RemoveAllListeners();
                    controlBoxBtn.onClick.RemoveAllListeners();
                    selectionValveBtn.onClick.RemoveAllListeners();
                    storageCylinderBtn.onClick.RemoveAllListeners();
                    activationBoxBtn.onClick.RemoveAllListeners();
                    manualControlBoxBtn.onClick.RemoveAllListeners();

                    controlBoxBtn.onClick.AddListener(delegate
                    {
                        ControlPanel.Instance.ShowPanel(true);
                    });

                    selectionValveBtn.onClick.AddListener(delegate
                    {
                        selectionValvePopup.InitPopupE(ChangeValveState, true);
                    });
                    storageCylinderBtn.onClick.AddListener(delegate
                    {
                        storageCylinderPopup.InitPopupE(ChangeStorageValveState, true);
                    });

                    selectionValvePopup.SetOnOff(false);
                    storageCylinderPopup.SetOnOff(false);
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
            case GasSysControlPanelSwitchESection.E1:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    var switchBtnList = ControlPanel.Instance.controlPanelButton;
                    _gasSysEvaluation.주경종 = switchBtnList.주경종;
                    _gasSysEvaluation.지구경종 = switchBtnList.지구경종;
                    _gasSysEvaluation.사이렌 = switchBtnList.사이렌;
                    _gasSysEvaluation.비상방송 = switchBtnList.비상방송;
                    _gasSysEvaluation.부저 = switchBtnList.부저;
                    _gasSysEvaluation.축적 = switchBtnList.축적;
                    //_gasSysEvaluation.솔레노이드연동 = ControlMode.Auto.Equals(switchBtnList.솔레노이드밸브);
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    {
                        _gasSysEvaluation.주경종 = switchBtn.주경종;
                        _gasSysEvaluation.지구경종 = switchBtn.지구경종;
                        _gasSysEvaluation.사이렌 = switchBtn.사이렌;
                        _gasSysEvaluation.비상방송 = switchBtn.비상방송;
                        _gasSysEvaluation.부저 = switchBtn.부저;
                        _gasSysEvaluation.축적 = switchBtn.축적;
                        _gasSysEvaluation.솔레노이드연동 = ControlMode.Manual.Equals(switchBtn.솔레노이드밸브);
                        _gasSysEvaluation.구역1연동 = ControlMode.Manual.Equals(switchBtn.구역1);
                        if (_gasSysEvaluation.구역1연동)
                        {
                            ControlPanel.Instance.GetArea1ActivateBtn().interactable = true;
                            ControlPanel.Instance.GetArea1ActivateBtn().onClick.RemoveAllListeners();
                            ControlPanel.Instance.GetArea1ActivateBtn().onClick.AddListener(delegate
                            {
                                _gasSysEvaluation.구역1기동 = true;
                                ControlPanel.Instance.InitTimer();
                                ControlPanel.Instance.StartTimer(30f);
                                solenoidValvePopup.InitControlPanelSwitch();
                                solenoidValvePopup.gameObject.SetActive(true);
                                ControlPanel.OnTimerEnd.RemoveAllListeners();
                                ControlPanel.OnTimerEnd.AddListener(delegate
                                {
                                    //storageRoomObj.ManualActivateToSolenoidValve();
                                    solenoidValvePopup.SolenoidValveAActivation();
                                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                                    ControlPanel.Instance.ShowFire(true);
                                    NextEnable();
                                });
                            });
                        }
                        
                    });
                }
                break;
            /*
            case GasSysControlPanelSwitchESection.E2:
                {
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    ControlPanel.Instance.CheckSwitchBtn();
                    ControlPanel.Instance.SetClose();
                    ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    {
                        _gasSysEvaluation.솔레노이드연동 = ControlMode.Manual.Equals(switchBtn.솔레노이드밸브);
                    });
                }
                break;
            case GasSysControlPanelSwitchESection.E3:
                {
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Manual);
                    ControlPanel.Instance.InitArea1Control(ControlMode.Auto);
                    ControlPanel.Instance.CheckSwitchBtn();
                    ControlPanel.Instance.SetClose();
                    ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    {
                        _gasSysEvaluation.구역1연동 = ControlMode.Manual.Equals(switchBtn.구역1);
                        if (_gasSysEvaluation.구역1연동)
                        {
                            ControlPanel.Instance.GetArea1ActivateBtn().interactable = true;
                            ControlPanel.Instance.GetArea1ActivateBtn().onClick.RemoveAllListeners();
                            ControlPanel.Instance.GetArea1ActivateBtn().onClick.AddListener(delegate
                            {
                                _gasSysEvaluation.구역1기동 = true;
                                ControlPanel.Instance.InitTimer();
                                ControlPanel.Instance.StartTimer(30f);
                                solenoidValvePopup.InitControlPanelSwitch();
                                solenoidValvePopup.gameObject.SetActive(true);
                                ControlPanel.OnTimerEnd.RemoveAllListeners();
                                ControlPanel.OnTimerEnd.AddListener(delegate
                                {
                                    //storageRoomObj.ManualActivateToSolenoidValve();
                                    solenoidValvePopup.SolenoidValveAActivation();
                                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                                    //ControlPanel.Instance.ShowFire(true);
                                    NextEnable();
                                });
                            });
                        }
                    });
                }
                break;
                */
            case GasSysControlPanelSwitchESection.E2:
                {
                    ControlPanel.Instance.SetTimeNum(0f);
                    ControlPanel.Instance.ResetTimer();
                    제어반수동조작평가1.SetActive(true);
                    ControlPanel.Instance.ShowPanel(true);
                    solenoidValvePopup.gameObject.SetActive(true);
                    solenoidValvePopup.SolenoidValveAActivation(false);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Manual);
                    ControlPanel.Instance.InitArea1Control(ControlMode.Manual);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    ControlPanel.Instance.ShowFire(true);
                    solenoidValvePopup.clipObj.SetActive(true);
                    SoundManager.Instance.StopAllFireSound();
                    //ButtonManager.Instance.DisableAllButtons();
                    NextEnable(false);
                }
                break;
            case GasSysControlPanelSwitchESection.평가종료:
                {
                    제어반수동조작평가1.SetActive(false);
                    GasSysGlobalCanvas.Instance.totalScore.스위치동작.감시제어반조치 = !_gasSysEvaluation.주경종 && !_gasSysEvaluation.지구경종 && !_gasSysEvaluation.사이렌 &&
                                                                           !_gasSysEvaluation.비상방송 && !_gasSysEvaluation.부저 && _gasSysEvaluation.축적 && _gasSysEvaluation.솔레노이드연동;
                    GasSysGlobalCanvas.Instance.totalScore.스위치동작.솔수동 = _gasSysEvaluation.솔레노이드연동;
                    GasSysGlobalCanvas.Instance.totalScore.스위치동작.수동기동 = _gasSysEvaluation.구역1연동 && _gasSysEvaluation.구역1기동;
                    GasSysGlobalCanvas.Instance.totalScore.스위치동작.격발시험 = _gasSysEvaluation.솔레노이드연동 && _gasSysEvaluation.구역1연동 && _gasSysEvaluation.구역1기동;
                    GasSysGlobalCanvas.Instance.totalScore.스위치동작.동작확인 = !제어반수동조작토글1.isOn &&
                                                                        제어반수동조작토글2.isOn &&
                                                                        !제어반수동조작토글3.isOn &&
                                                                        !제어반수동조작토글4.isOn &&
                                                                        !제어반수동조작토글5.isOn &&
                                                                        !제어반수동조작토글6.isOn;
                    var results = new List<ResultObject>
                    {
                        new ResultObject()
                        {
                            Title = "감시제어반 조치",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.스위치동작.감시제어반조치
                        },
                        new ResultObject()
                        {
                            Title = "제어반 수동조작 스위치 작동 격발시험",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.스위치동작.격발시험 
                        },
                        new ResultObject()
                        {
                            Title = "정상작동 상태확인",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.스위치동작.동작확인
                        }
                    };
                    //GasSysGlobalCanvas.Instance.SetResultPopup(results);
                    GasSysGlobalCanvas.Instance.totalScore.스위치동작List.Clear();
                    GasSysGlobalCanvas.Instance.totalScore.스위치동작List.AddRange(results);
                    GasSysGlobalCanvas.Instance.SetNextEvaluation(InitDischargeLightTest, InitControlPanelSwitch);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        ControlPanel.Instance.CheckWarringSwitch();
    }


    public void ChangeStateControlPanelSwitchP(GasSysControlPanelSwitchPSection state)
    {

        _curControlPanelSwitchPSection = state;
        OnStateChangedControlPanelSwitchP(_curControlPanelSwitchPSection);
    }

    private void OnStateChangedControlPanelSwitchP(GasSysControlPanelSwitchPSection state)
    {
        InitStorageRoom();
        //ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
        ControlPanel.Instance.InitTimer();
        //SoundManager.Instance.StopAllFireSound();
        //ControlPanel.Instance.ShowFire(false);
        uiDragAndCollisionHandler.StopDragging();
        ButtonManager.Instance.RemoveAllHighlights();
        ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
        ControlPanel.OnTimerEnd.RemoveAllListeners();
        ControlPanel.OnTimerCheck.RemoveAllListeners();
        //nextBtn.onClick.RemoveAllListeners();
        switch (state)
        {
            case GasSysControlPanelSwitchPSection.감시제어반선택:
                {
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
            case GasSysControlPanelSwitchPSection.제어반음향:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    var temp = new ControlPanelButtonCheck(ControlMode.Stop);
                    temp.SetBtn(true);
                    //temp.솔레노이드밸브 = ControlMode.Auto;
                    temp.축적 = false;
                    temp.복구 = false;
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(false);
                    temp.축적 = true;
                    SetHighlightControlPanelCheck(temp);
                }
                break;
            case GasSysControlPanelSwitchPSection.솔레노이드밸브수동:
                {
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.ShowPanel(true);

                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    ControlPanel.Instance.InitArea1Control(ControlMode.Auto);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    temp.솔레노이드밸브 = ControlMode.Manual;
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(false);
                    temp.축적 = true;
                    temp.솔레노이드밸브 = ControlMode.Manual;
                    SetHighlightControlPanelCheck(temp);
                    //NextEnable();
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
                    //     select = true
                    // });
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
                    //     select = true
                    // });
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
                    //     select = true
                    // });
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
                    //     select = true
                    // });
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
                    //     select = true
                    // });
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSwitchButton("축적/비축적").GetButton(),
                    //     select = true
                    // });
                    //
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSolenoidSelectBtn(),
                    //     select = false
                    // });
                    //
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetArea1SelectBtn(),
                    //     select = true
                    // });
                    //
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetArea2SelectBtn(),
                    //     select = true
                    // });
                    // ButtonManager.Instance.EnableSpecificButton(btnList);
                    // NextDisable();
                    // ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    // ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    // {
                    //     btnList.Clear();
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
                    //         select = !switchBtn.주경종
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
                    //         select = !switchBtn.지구경종
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
                    //         select = !switchBtn.사이렌
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
                    //         select = !switchBtn.비상방송
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
                    //         select = !switchBtn.부저
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("축적/비축적").GetButton(),
                    //         select = switchBtn.축적
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSolenoidSelectBtn(),
                    //         select = switchBtn.솔레노이드밸브.Equals(ControlMode.Manual)
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetArea1SelectBtn(),
                    //         select = switchBtn.구역1.Equals(ControlMode.Auto)
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetArea2SelectBtn(),
                    //         select = switchBtn.구역2.Equals(ControlMode.Auto)
                    //     });
                    //     ButtonManager.Instance.EnableSpecificButton(btnList);
                    //
                    //     if (!switchBtn.주경종 && !switchBtn.지구경종 && !switchBtn.사이렌 && !switchBtn.비상방송 && !switchBtn.부저 && switchBtn.축적 &&
                    //         switchBtn.구역1.Equals(ControlMode.Auto) && switchBtn.솔레노이드밸브.Equals(ControlMode.Manual) && switchBtn.구역2.Equals(ControlMode.Auto))
                    //     {
                    //         NextEnable();
                    //     }
                    //     else
                    //     {
                    //         NextDisable();
                    //     }
                    // });
                }
                break;
            case GasSysControlPanelSwitchPSection.방호구역1번수동:
                {
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.ShowFire(false);
                    SoundManager.Instance.StopAllFireSound();
                    //ControlPanel.Instance.SoundCheck();
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Manual);
                    ControlPanel.Instance.InitArea1Control(ControlMode.Auto);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    temp.솔레노이드밸브 = ControlMode.Manual;
                    temp.구역1 = ControlMode.Manual;
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(false);
                    temp.축적 = true;
                    temp.솔레노이드밸브 = ControlMode.Manual;
                    temp.구역1 = ControlMode.Manual;
                    SetHighlightControlPanelCheck(temp);
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
                    //     select = true
                    // });
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
                    //     select = true
                    // });
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
                    //     select = true
                    // });
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
                    //     select = true
                    // });
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
                    //     select = true
                    // });
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSwitchButton("축적/비축적").GetButton(),
                    //     select = true
                    // });
                    //
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSolenoidSelectBtn(),
                    //     select = true
                    // });
                    //
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetArea1SelectBtn(),
                    //     select = false
                    // });
                    //
                    //
                    //
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetArea2SelectBtn(),
                    //     select = true
                    // });
                    // ButtonManager.Instance.EnableSpecificButton(btnList);
                    // NextDisable();
                    // ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    // ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    // {
                    //     btnList.Clear();
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
                    //         select = !switchBtn.주경종
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
                    //         select = !switchBtn.지구경종
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
                    //         select = !switchBtn.사이렌
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
                    //         select = !switchBtn.비상방송
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
                    //         select = !switchBtn.부저
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("축적/비축적").GetButton(),
                    //         select = switchBtn.축적
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSolenoidSelectBtn(),
                    //         select = switchBtn.솔레노이드밸브.Equals(ControlMode.Manual)
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetArea1SelectBtn(),
                    //         select = switchBtn.구역1.Equals(ControlMode.Manual)
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetArea2SelectBtn(),
                    //         select = switchBtn.구역2.Equals(ControlMode.Auto)
                    //     });
                    //     ButtonManager.Instance.EnableSpecificButton(btnList);
                    //
                    //     if (!switchBtn.주경종 && !switchBtn.지구경종 && !switchBtn.사이렌 && !switchBtn.비상방송 && !switchBtn.부저 && switchBtn.축적 &&
                    //         switchBtn.구역1.Equals(ControlMode.Manual) && switchBtn.솔레노이드밸브.Equals(ControlMode.Manual) && switchBtn.구역2.Equals(ControlMode.Auto))
                    //     {
                    //         NextEnable();
                    //     }
                    //     else
                    //     {
                    //         NextDisable();
                    //     }
                    // });
                }
                break;
            case GasSysControlPanelSwitchPSection.방호구역1번기동:
                {
                    bool isComplete = false;
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.ShowFire(false);
                    SoundManager.Instance.StopAllFireSound();
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    //ControlPanel.Instance.SoundCheck();
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Manual);
                    ControlPanel.Instance.InitArea1Control(ControlMode.Manual);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    temp.솔레노이드밸브 = ControlMode.Manual;
                    temp.구역1 = ControlMode.Manual;
                    temp.구역1기동 = true;
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(false);
                    temp.축적 = true;
                    temp.솔레노이드밸브 = ControlMode.Manual;
                    temp.구역1 = ControlMode.Manual;
                    temp.구역1기동 = true;
                    SetHighlightControlPanelCheck(temp);
                    ControlPanel.Instance.GetArea1ActivateBtn().onClick.RemoveAllListeners();
                    ControlPanel.Instance.GetArea1ActivateBtn().onClick.AddListener(delegate
                    {
                        isComplete = true;
                        // ControlPanel.Instance.ShowFire(true);
                        // ControlPanel.Instance.StartTimer(30f);
                        // solenoidValvePopup.InitControlPanelSwitch();
                        // solenoidValvePopup.gameObject.SetActive(true);
                        // ControlPanel.OnTimerEnd.RemoveAllListeners();
                        // ControlPanel.OnTimerEnd.AddListener(delegate
                        // {
                        //     //storageRoomObj.ManualActivateToSolenoidValve();
                        //     solenoidValvePopup.SolenoidValveAActivation();
                        //     ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                        //     //ControlPanel.Instance.ShowFire(true);
                        //     NextEnable();
                        // });
                        Next();
                    });

                    GasSysGlobalCanvas.Instance.HideCheckObj();
                }
                break;
            case GasSysControlPanelSwitchPSection.방호구역1번기동2:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.ShowFire(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Manual);
                    ControlPanel.Instance.InitArea1Control(ControlMode.Manual);
                    var temp = new ControlPanelButtonCheck(ControlMode.Manual, ControlMode.Manual);
                    temp.SetBtn(false);
                    //temp.솔레노이드밸브 = ControlMode.Manual;
                    //temp.구역1 = ControlMode.Manual;
                    //temp.구역1기동 = true;
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(false);
                    temp.축적 = true;
                    //temp.솔레노이드밸브 = ControlMode.Manual;
                    //temp.구역1 = ControlMode.Manual;
                    SetHighlightControlPanelCheck(temp);
                    ControlPanel.Instance.StartTimer(30f);
                    solenoidValvePopup.InitControlPanelSwitch();
                    solenoidValvePopup.gameObject.SetActive(true);
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
       
                    GasSysGlobalCanvas.Instance.HideCheckObj();
                }
                break;
            case GasSysControlPanelSwitchPSection.솔정상확인:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
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
                    solenoidValvePopup.InitControlPanelSwitch();
                    solenoidValvePopup.gameObject.SetActive(true);
                    ControlPanel.OnTimerEnd.RemoveAllListeners();
                    ControlPanel.OnTimerEnd.AddListener(delegate
                    {
                        //storageRoomObj.ManualActivateToSolenoidValve();
                        solenoidValvePopup.SolenoidValveAActivation();
                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                        solenoidValvePopup.gameObject.SetActive(false);
                        GasSysGlobalCanvas.Instance.ShowSol2(Next);
                        NextEnable(false);
                        //ControlPanel.Instance.ShowFire(true);
                    });
                    var temp = new ControlPanelButtonCheck(ControlMode.Manual, ControlMode.Manual);
                    temp.SetBtn(false);
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(false);
                    temp.축적 = true;
                    SetHighlightControlPanelCheck(temp, false);
                }
                break;
            case GasSysControlPanelSwitchPSection.음향정지작동확인:
                {
                    GasSysGlobalCanvas.Instance.HideCheckObj();
                    ControlPanel.Instance.SetTimeNum(0f);
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    // ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    var temp = new ControlPanelButtonCheck(ControlMode.Manual, ControlMode.Manual);
                    temp.SetBtn(true);
                    temp.축적 = false;
                    temp.복구 = false;
                    temp.구역1기동 = false;
                    SetHighlightControlPanel(temp);
                    //NextEnable(false);
                    NextDisable();
                    temp.SetBtn(true);
                    temp.축적 = true;
                    temp.복구 = false;
                    temp.구역1기동 = true;
                    SetHighlightControlPanel(temp, true);
                    SetHighlightControlPanelCheck(temp, true);
                    // _switchButtons.Add(new SwitchButtonCheck()
                    // {
                    //     btn = GlobalCanvas.Instance.GetCheckAgreeBtn(),
                    //     select = false
                    // });
                    ButtonManager.Instance.EnableSpecificButton(_switchButtons);
                    //GasSysGlobalCanvas.Instance.ShowSol3(Next);
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                }
                break;
            case GasSysControlPanelSwitchPSection.제어반화재표시:
                {
                    NextDisable();
                    ControlPanel.Instance.SetTimeNum(0f);
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    GasSysGlobalCanvas.Instance.ShowFire(Next);
                    var temp = new ControlPanelButtonCheck(ControlMode.Manual, ControlMode.Manual);
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
            case GasSysControlPanelSwitchPSection.방호구역1:
                {
                    ControlPanel.Instance.SetTimeNum(0f);
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    GasSysGlobalCanvas.Instance.ShowArea1(Next);
                    //GasSysSoundManager.Instance.MuteSiren(false);
                    SoundManager.Instance.SetDefaultVolume();
                    var temp = new ControlPanelButtonCheck(ControlMode.Manual, ControlMode.Manual);
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
                */
            case GasSysControlPanelSwitchPSection.방호구역1음향연동:
                {
                    ControlPanel.Instance.SetTimeNum(0f);
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    var temp = new ControlPanelButtonCheck(ControlMode.Manual, ControlMode.Manual);
                    temp.SetBtn(true);
                    temp.축적 = false;
                    temp.복구 = false;
                    temp.구역1기동 = true;
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
                    // temp.SetBtn(true);
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
                    GasSysGlobalCanvas.Instance.ShowArea1(Next);
                    //GasSysSoundManager.Instance.MuteSiren(false);
                    SoundManager.Instance.SetDefaultVolume();
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                    //ButtonManager.Instance.EnableSpecificButton(_switchButtons);
                }
                break;
            case GasSysControlPanelSwitchPSection.음향정지:
                {
                    ControlPanel.Instance.SetTimeNum(0f);
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    var temp = new ControlPanelButtonCheck(ControlMode.Manual, ControlMode.Manual);
                    temp.SetBtn(true);
                    temp.축적 = false;
                    temp.복구 = false;
                    //SetHighlightControlPanel(temp);
                    NextDisable();
                    //NextEnable(false);
                    temp.SetBtn(true);
                    temp.복구 = false;
                    temp.구역1기동 = true;
                    SetHighlightControlPanel(temp, true);
                    SetHighlightControlPanelCheck(temp);
                    GasSysGlobalCanvas.Instance.ShowArea1(Next);
                    //GasSysSoundManager.Instance.MuteSiren(false);
                    SoundManager.Instance.SetDefaultVolume();
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                }
                break;
            case GasSysControlPanelSwitchPSection.방호구역2:
                {
                    ControlPanel.Instance.SetTimeNum(0f);
                    ControlPanel.Instance.ShowPanel(true);
                    SoundManager.Instance.MuteAll(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    var temp = new ControlPanelButtonCheck(ControlMode.Manual, ControlMode.Manual);
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
                    temp.구역1기동 = true;
                    //NextEnable(false);
                    SetHighlightControlPanel(temp, true);
                    SetHighlightControlPanelCheck(temp);
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
                    GasSysGlobalCanvas.Instance.ShowArea2(Next);
                    //GasSysSoundManager.Instance.MuteSiren(true);
                    SoundManager.Instance.SetSirenVolume(0f);
                    ControlPanel.OnTimerEnd.RemoveAllListeners();
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                }
                break;
            /*
            case GasSysControlPanelSwitchPSection.지연장치:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    GasSysGlobalCanvas.Instance.ShowTimer(Next);
                    SoundManager.Instance.SetDefaultVolume();
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
                        solenoidValvePopup.SolenoidValveAActivation();
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
            case GasSysControlPanelSwitchPSection.솔레노이드작동여부:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    GasSysGlobalCanvas.Instance.ShowSol2(Next);
                    SoundManager.Instance.SetDefaultVolume();
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Manual);
                    ControlPanel.Instance.InitArea1Control(ControlMode.Manual);
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
            case GasSysControlPanelSwitchPSection.교육종료:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    SetCompletePopup("제어반 수동조작 스위치 작동 시험을 모두 완료했습니다.", "다음 점검을", InitDischargeLightTest);
                }
                break;
            // case GasSysControlPanelSwitchSection.솔정상확인:
            // case GasSysControlPanelSwitchSection.음향정지작동확인:
            // case GasSysControlPanelSwitchSection.제어반화재표시:
            // case GasSysControlPanelSwitchSection.방호구역1:
            // case GasSysControlPanelSwitchSection.방호구역1음향연동:
            // case GasSysControlPanelSwitchSection.음향정지:
            // case GasSysControlPanelSwitchSection.방호구역2:
            // case GasSysControlPanelSwitchSection.지연장치:
            // case GasSysControlPanelSwitchSection.솔레노이드작동여부:
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }



}
