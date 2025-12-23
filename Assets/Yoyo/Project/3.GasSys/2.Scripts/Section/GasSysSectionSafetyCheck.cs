using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public partial class GasSysSection
{
    [Foldout("점검 전 안전 조치")]
    private GasSysSafetyCheckPSection _curSafetyCheckPSection;
    private GasSysSafetyCheckESection _curSafetyCheckESection;
    [FormerlySerializedAs("safetyCheckHintObj")]
    public HintScriptableObj safetyCheckHintPObj;
    public HintScriptableObj safetyCheckHintEObj;
    public GameObject 점검전안전조치평가1;
    [SerializeField] public Toggle 점검전안전조치토글1;
    [SerializeField] public Toggle 점검전안전조치토글2;
    [SerializeField] public Toggle 점검전안전조치토글3;
    [SerializeField] public Toggle 점검전안전조치토글4;
    [SerializeField] public Toggle 점검전안전조치토글5;

    [EndFoldout]

    //private GasSysControlPanelState _gasSysSafetyEvaluation = new GasSysControlPanelState();
    public void InitSafetyCheck()
    {
        InitEMode();
        curMainSection = GasSysMainSection.SafetyCheck;
        nextBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.RemoveAllListeners();
        areaManagerObj?.StartArea(GasSysAreaManager.StartAreaType.StorageRoom);
        areaManagerObj?.ShowPanel(true);
        ShowRoom(storageRoomObj?.gameObject);
        //storageRoomObj?.InitSafetyCheck();
        inventoryObj?.Init();
        inventoryObj?.ShowPanel(true);
        ControlPanel.Instance.Init();
        ControlPanel.Instance.ShowPanel(false);
        _maxSection = _gasSysState switch
        {
            //ChangeState(GasSysSafetyCheckSection.선택밸브조작동관선택);
            GasSysState.PracticeMode => System.Enum.GetValues(typeof(GasSysSafetyCheckPSection)).Length,
            GasSysState.EvaluationMode => System.Enum.GetValues(typeof(GasSysSafetyCheckESection)).Length,
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
        uiDragAndCollisionHandler.ResetEvent();
        ChangeValveState(true);
        ChangeStorageValveState(true);
        OpenActivationBox(false);
        GasSysGlobalCanvas.Instance.HideCheckObj();
    }


    private void ShowSafetyCheckSection(int index)
    {
        if (_gasSysState.Equals(GasSysState.PracticeMode))
        {
            prevBtn.gameObject.SetActive(true);
            GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(safetyCheckHintPObj, (int)GasSysSafetyCheckPSection.선택밸브조작동관선택 + index));
            ChangeStateSafetyCheckP(GasSysSafetyCheckPSection.선택밸브조작동관선택 + index);
        }
        if (!_gasSysState.Equals(GasSysState.EvaluationMode))
            return;
        prevBtn.gameObject.SetActive(false);
        GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(safetyCheckHintEObj, (int)GasSysSafetyCheckESection.Init + index));
        ChangeStateSafetyCheckE(GasSysSafetyCheckESection.Init + index);
    }

    public void ChangeStateSafetyCheckE(GasSysSafetyCheckESection state)
    {

        _curSafetyCheckESection = state;
        OnStateChangedSafetyCheckE(_curSafetyCheckESection);
    }

    private void OnStateChangedSafetyCheckE(GasSysSafetyCheckESection state)
    {

        ButtonManager.Instance.RemoveAllHighlights();
        switch (state)
        {
            case GasSysSafetyCheckESection.Init:
                {
                    InitStorageRoom();
                    NextEnable();
                    _gasSysEvaluation.Reset();
                    // ButtonManager.Instance.EnableSpecificImage(activationCylinderBoxPopup.aActivationObj,
                    //     activationCylinderBoxPopup.pinObj);

                    controlBoxBtn.gameObject.SetActive(true);
                    selectionValveBtn.gameObject.SetActive(true);
                    storageCylinderBtn.gameObject.SetActive(true);
                    activationBoxBtn.gameObject.SetActive(true);
                    areaManagerObj.area2Btn.interactable = false;
                    areaManagerObj.area1Btn.interactable = false;
                    areaManagerObj.area1CorridorBtn.interactable = false;
                    areaManagerObj.area2CorridorBtn.interactable = false;
                    //areaManagerObj.area2

                    controlBoxBtn.onClick.RemoveAllListeners();
                    selectionValveBtn.onClick.RemoveAllListeners();
                    storageCylinderBtn.onClick.RemoveAllListeners();
                    activationBoxBtn.onClick.RemoveAllListeners();

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
                    activationBoxBtn.onClick.AddListener(delegate
                    {
                        inventoryObj?.ShowSafetyPin(false);
                        activationCylinderBoxPopup.InitSafetyCheck(delegate
                        {
                            solenoidValvePopup.InitSafetyCheck();
                            solenoidValvePopup.gameObject.SetActive(true);
                        });

                    });

                    selectionValvePopup.SetOnOff(true);
                    storageCylinderPopup.SetOnOff(true);

                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(false);
                    ControlPanel.Instance.InitSolenoidValveControl();
                    ControlPanel.Instance.SetClose();
                    //NextEnable(fal);
                    ButtonManager.Instance.SetEvaluationButtons();
                    Next();
                }
                break;
            case GasSysSafetyCheckESection.E1:
                {
                    
                }
                break;
            case GasSysSafetyCheckESection.E2:
                {
                    _gasSysEvaluation.기동밸브 = !GetValveState();
                    _gasSysEvaluation.저장밸브 = !GetStorageState();
                    ControlPanel.Instance.ShowPanel(true);
                    var switchBtnList = ControlPanel.Instance.controlPanelButton;
                    _gasSysEvaluation.주경종 = switchBtnList.주경종;
                    _gasSysEvaluation.지구경종 = switchBtnList.지구경종;
                    _gasSysEvaluation.사이렌 = switchBtnList.사이렌;
                    _gasSysEvaluation.비상방송 = switchBtnList.비상방송;
                    _gasSysEvaluation.부저 = switchBtnList.부저;
                    _gasSysEvaluation.축적 = switchBtnList.축적;
                    _gasSysEvaluation.솔레노이드연동 = ControlMode.Stop.Equals(switchBtnList.솔레노이드밸브);
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    {
                        _gasSysEvaluation.주경종 = switchBtn.주경종;
                        _gasSysEvaluation.지구경종 = switchBtn.지구경종;
                        _gasSysEvaluation.사이렌 = switchBtn.사이렌;
                        _gasSysEvaluation.비상방송 = switchBtn.비상방송;
                        _gasSysEvaluation.부저 = switchBtn.부저;
                        _gasSysEvaluation.축적 = switchBtn.축적;
                        _gasSysEvaluation.솔레노이드연동 = ControlMode.Stop.Equals(switchBtn.솔레노이드밸브);
                    });
                    ChangeValveState(false);
                    ChangeStorageValveState(false);
                    selectionValvePopup.SetOnOff(false);
                    storageCylinderPopup.SetOnOff(false);
                }
                break;
            case GasSysSafetyCheckESection.E3:
                {
                    uiDragAndCollisionHandler.StartDragging();
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    ControlPanel.Instance.ShowPanel(false);
                    inventoryObj?.ShowSafetyPin(false);
                    activationCylinderBoxPopup.InitSafetyCheck(delegate
                    {
                        solenoidValvePopup.InitSafetyCheck();
                        solenoidValvePopup.gameObject.SetActive(true);
                        activationBoxBtn.gameObject.SetActive(false);
                    });

                }
                break;
            case GasSysSafetyCheckESection.E4:
                {
                    activationCylinderBoxPopup.gameObject.SetActive(false);
                    solenoidValvePopup.gameObject.SetActive(false);
                    점검전안전조치평가1.gameObject.SetActive(true);
                }
                break;
            case GasSysSafetyCheckESection.평가종료:
                {
                    점검전안전조치평가1.gameObject.SetActive(false);
                    var results = new List<ResultObject>();

                    if (_gasSysEvaluation.기동밸브 && _gasSysEvaluation.저장밸브)
                    {
                        Debug.Log("평가모드 1 완료");

                    }
                    else
                    {
                        Debug.Log("평가모드 1 실패");
                    }
                    if (_gasSysEvaluation.주경종 && _gasSysEvaluation.지구경종 && _gasSysEvaluation.사이렌 &&
                        _gasSysEvaluation.비상방송 && _gasSysEvaluation.부저)
                    {
                        Debug.Log("평가모드 2 완료");
                    }
                    else
                    {
                        Debug.Log("평가모드 2 실패");
                    }
                    if (_gasSysEvaluation.축적)
                    {
                        Debug.Log("평가모드 3 완료");
                    }
                    else
                    {
                        Debug.Log("평가모드 3 실패");
                    }
                    if (_gasSysEvaluation.솔레노이드연동)
                    {
                        Debug.Log("평가모드 4 완료");
                    }
                    else
                    {
                        Debug.Log("평가모드 4 실패");
                    }

                    if (inventoryObj.safetyPin.activeSelf)
                    {
                        Debug.Log("평가모드 5 완료");
                    }
                    else
                    {
                        Debug.Log("평가모드 5 실패");
                    }
                    GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.조작동관분리 = _gasSysEvaluation.기동밸브 && _gasSysEvaluation.저장밸브;
                    GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.감시제어반조치 = _gasSysEvaluation.주경종 && _gasSysEvaluation.지구경종 && _gasSysEvaluation.사이렌 &&
                                                                             _gasSysEvaluation.비상방송 && _gasSysEvaluation.부저 && _gasSysEvaluation.축적 && _gasSysEvaluation.솔레노이드연동;
                    //GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.비축적전환 = _gasSysEvaluation.축적;
                    //GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.솔레노이드연동정지 = _gasSysEvaluation.솔레노이드연동;
                    GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.솔분리및격발준비 = inventoryObj.safetyPin.activeSelf;
                    GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.안전조치상태확인 = !점검전안전조치토글1.isOn && !점검전안전조치토글2.isOn && !점검전안전조치토글3.isOn && !점검전안전조치토글4.isOn && 점검전안전조치토글5.isOn;
                    results.Add(new ResultObject()
                    {
                        Title = "동관 분리",
                        IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.조작동관분리
                    });
                    results.Add(new ResultObject()
                    {
                        Title = "감시제어반 조치",
                        IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.감시제어반조치
                    });
                    
                    // results.Add(new ResultObject()
                    // {
                    //     Title = "비축적 전환",
                    //     IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.비축적전환
                    // });
                    //
                    // results.Add(new ResultObject()
                    // {
                    //     Title = "솔레노이드연동 정지",
                    //     IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.솔레노이드연동정지
                    // });

                    results.Add(new ResultObject()
                    {
                        Title = "솔레노이드밸브 분리 및 격발준비",
                        IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.솔분리및격발준비
                    });
                    results.Add(new ResultObject()
                    {
                        Title = "안전조치 상태확인",
                        IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.안전조치상태확인
                    });
                    GasSysGlobalCanvas.Instance.totalScore.점검전안전조치List.Clear();
                    GasSysGlobalCanvas.Instance.totalScore.점검전안전조치List.AddRange(results);
                    GasSysGlobalCanvas.Instance.SetNextEvaluation(InitManualOperation, InitSafetyCheck);
                }
                break;
            /*
            case GasSysSafetyCheckESection.평가종료:
                {
                    // if(_gasSysSafetyEvaluation.기동밸브
                    // _gasSysSafetyEvaluation.저장밸브)
                    var results = new List<ResultObject>();

                    if (_gasSysEvaluation.기동밸브 && _gasSysEvaluation.저장밸브)
                    {
                        Debug.Log("평가모드 1 완료");

                    }
                    else
                    {
                        Debug.Log("평가모드 1 실패");
                    }
                    if (_gasSysEvaluation.주경종 && _gasSysEvaluation.지구경종 && _gasSysEvaluation.사이렌 &&
                        _gasSysEvaluation.비상방송 && _gasSysEvaluation.부저)
                    {
                        Debug.Log("평가모드 2 완료");
                    }
                    else
                    {
                        Debug.Log("평가모드 2 실패");
                    }
                    if (_gasSysEvaluation.축적)
                    {
                        Debug.Log("평가모드 3 완료");
                    }
                    else
                    {
                        Debug.Log("평가모드 3 실패");
                    }
                    if (_gasSysEvaluation.솔레노이드연동)
                    {
                        Debug.Log("평가모드 4 완료");
                    }
                    else
                    {
                        Debug.Log("평가모드 4 실패");
                    }

                    if (inventoryObj.safetyPin.activeSelf)
                    {
                        Debug.Log("평가모드 5 완료");
                    }
                    else
                    {
                        Debug.Log("평가모드 5 실패");
                    }
                    GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.조작동관분리 = _gasSysEvaluation.기동밸브 && _gasSysEvaluation.저장밸브;
                    GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.감시제어반조치 = _gasSysEvaluation.주경종 && _gasSysEvaluation.지구경종 && _gasSysEvaluation.사이렌 &&
                                                                             _gasSysEvaluation.비상방송 && _gasSysEvaluation.부저 && _gasSysEvaluation.축적 && _gasSysEvaluation.솔레노이드연동;
                    //GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.비축적전환 = _gasSysEvaluation.축적;
                    //GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.솔레노이드연동정지 = _gasSysEvaluation.솔레노이드연동;
                    GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.솔분리및격발준비 = inventoryObj.safetyPin.activeSelf;
                    results.Add(new ResultObject()
                    {
                        Title = "동관 분리",
                        IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.조작동관분리
                    });
                    results.Add(new ResultObject()
                    {
                        Title = "감시제어반 조치",
                        IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.감시제어반조치
                    });

                    // results.Add(new ResultObject()
                    // {
                    //     Title = "비축적 전환",
                    //     IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.비축적전환
                    // });
                    //
                    // results.Add(new ResultObject()
                    // {
                    //     Title = "솔레노이드연동 정지",
                    //     IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.솔레노이드연동정지
                    // });

                    results.Add(new ResultObject()
                    {
                        Title = "솔레노이드밸브 분리 및 격발준비",
                        IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검전안전조치.솔분리및격발준비
                    });
                    GasSysGlobalCanvas.Instance.totalScore.점검전안전조치List.Clear();
                    GasSysGlobalCanvas.Instance.totalScore.점검전안전조치List.AddRange(results);
                    GasSysGlobalCanvas.Instance.SetNextEvaluation(InitManualOperation);
                    //GasSysGlobalCanvas.Instance.SetResultPopup(results);

                    //Debug.Log(_gasSysSafetyEvaluation);

                }
                break;
                */
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }



    public void ChangeStateSafetyCheckP(GasSysSafetyCheckPSection state)
    {

        _curSafetyCheckPSection = state;
        OnStateChangedSafetyCheckP(_curSafetyCheckPSection);
    }




    private void OnStateChangedSafetyCheckP(GasSysSafetyCheckPSection state)
    {
        InitStorageRoom();
        uiDragAndCollisionHandler.StopDragging();
        ButtonManager.Instance.RemoveAllHighlights();
        //nextBtn.onClick.RemoveAllListeners();
        switch (state)
        {
            case GasSysSafetyCheckPSection.선택밸브조작동관선택:
                {
                    selectionValveBtn.gameObject.SetActive(true);
                    ChangeValveState(true);
                    ChangeStorageValveState(true);
                    ButtonManager.Instance.EnableSpecificButton(selectionValveBtn);
                    NextDisable();
                    // nextBtn.onClick.AddListener(delegate
                    // {
                    //     ChangeState(GasSysSafetyCheckSection.선택밸브조작동관분리);
                    // });
                    selectionValveBtn.onClick.AddListener(delegate
                    {
                        selectionValveBtn.gameObject.SetActive(false);
                        Next();
                    });
                }
                break;
            case GasSysSafetyCheckPSection.선택밸브조작동관분리전:
                {
                    ButtonManager.Instance.EnableSpecificButton(selectionValvePopup.GetButtons());
                    ChangeValveState(true);
                    NextDisable();
                    selectionValvePopup.InitPopup(true, isOn =>
                    {
                        ChangeValveState(isOn);
                        if (!isOn)
                        {
                            Next();
                        }
                    });
                }
                break;
            case GasSysSafetyCheckPSection.선택밸브조작동관분리:
                {
                    //ButtonManager.Instance.EnableSpecificButton(selectionValvePopup.GetButtons());
                    selectionValvePopup.GetButtons()[0].gameObject.SetActive(false);
                    selectionValvePopup.Init();
                    selectionValvePopup.gameObject.SetActive(true);
                    // selectionValvePopup.SetOnOff(false);
                    // selectionValve01.ChangeValveState(false);
                    ChangeValveState(false);
                    NextEnable();
                    // selectionValvePopup.InitPopup(true, isOn =>
                    // {
                    //     selectionValve01.ChangeValveState(isOn);
                    //     if (!isOn)
                    //     {
                    //         NextEnable();
                    //     }
                    // });
                    // nextBtn.onClick.AddListener(delegate
                    // {
                    //     ChangeState(GasSysSafetyCheckSection.저장용기조작동관선택);
                    // });
                }
                break;
            case GasSysSafetyCheckPSection.저장용기조작동관선택:
                {
                    storageCylinderBtn.gameObject.SetActive(true);
                    ChangeStorageValveState(true);
                    ButtonManager.Instance.EnableSpecificButton(storageCylinderBtn);
                    NextDisable();
                    storageCylinderBtn.onClick.AddListener(delegate
                    {
                        storageCylinderBtn.gameObject.SetActive(false);
                        Next();
                    });
                }
                break;
            case GasSysSafetyCheckPSection.저장용기조작동관분리전:
                {
                    ButtonManager.Instance.EnableSpecificButton(storageCylinderPopup.GetButtons());
                    NextDisable();
                    ChangeStorageValveState(true);
                    storageCylinderPopup.InitPopup(true, isOn =>
                    {
                        ChangeStorageValveState(isOn);
                        if (!isOn)
                        {
                            Next();
                        }
                    });
                }
                break;
            case GasSysSafetyCheckPSection.저장용기조작동관분리:
                {
                    storageCylinderPopup.GetButtons()[0].gameObject.SetActive(false);
                    storageCylinderPopup.Init();
                    storageCylinderPopup.gameObject.SetActive(true);
                    ChangeStorageValveState(false);
                    //storageCylinder.ChangeValveState(false);
                    NextEnable();
                }
                break;
            case GasSysSafetyCheckPSection.감시제어반선택:
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
            case GasSysSafetyCheckPSection.음향활성:
                {
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.ClearReceiverLog();
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(false);
                    ControlPanel.Instance.InitSolenoidValveControl();
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(true);
                    temp.축적 = false;
                    temp.복구 = false;
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    SetHighlightControlPanelCheck(temp);

                }
                break;
            case GasSysSafetyCheckPSection.축적비축적:
                {
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(false);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    temp.축적 = true;
                    temp.복구 = false;
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(true);
                    temp.복구 = false;
                    SetHighlightControlPanelCheck(temp);
                    /*
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
                        select = false
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
                    NextDisable();
                    ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    {
                        btnList.Clear();
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
                            select = switchBtn.주경종
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
                            select = switchBtn.지구경종
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
                            select = switchBtn.사이렌
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
                            select = switchBtn.비상방송
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
                            select = switchBtn.부저
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("축적/비축적").GetButton(),
                            select = switchBtn.축적
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSolenoidSelectBtn(),
                            select = switchBtn.솔레노이드밸브.Equals(ControlMode.Auto)
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetArea1SelectBtn(),
                            select = switchBtn.구역1.Equals(ControlMode.Auto)
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetArea2SelectBtn(),
                            select = switchBtn.구역2.Equals(ControlMode.Auto)
                        });
                        ButtonManager.Instance.EnableSpecificButton(btnList);

                        if (switchBtn.주경종 && switchBtn.지구경종 && switchBtn.사이렌 && switchBtn.비상방송 && switchBtn.부저 && switchBtn.축적 &&
                            switchBtn.구역1.Equals(ControlMode.Auto) && switchBtn.솔레노이드밸브.Equals(ControlMode.Auto) && switchBtn.구역2.Equals(ControlMode.Auto))
                        {
                            NextEnable();
                        }
                        else
                        {
                            NextDisable();
                        }
                    });
                    */
                }
                break;
            case GasSysSafetyCheckPSection.솔레노이드밸브메인정지:
                {
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    temp.솔레노이드밸브 = ControlMode.Stop;
                    temp.복구 = false;
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(true);
                    temp.복구 = false;
                    SetHighlightControlPanelCheck(temp);
                    /*
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
                        select = false
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
                    NextDisable();
                    ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    {
                        btnList.Clear();
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
                            select = switchBtn.주경종
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
                            select = switchBtn.지구경종
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
                            select = switchBtn.사이렌
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
                            select = switchBtn.비상방송
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
                            select = switchBtn.부저
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("축적/비축적").GetButton(),
                            select = switchBtn.축적
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSolenoidSelectBtn(),
                            select = switchBtn.솔레노이드밸브.Equals(ControlMode.Stop)
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetArea1SelectBtn(),
                            select = switchBtn.구역1.Equals(ControlMode.Auto)
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetArea2SelectBtn(),
                            select = switchBtn.구역2.Equals(ControlMode.Auto)
                        });
                        ButtonManager.Instance.EnableSpecificButton(btnList);
                        if (switchBtn.주경종 && switchBtn.지구경종 && switchBtn.사이렌 && switchBtn.비상방송 && switchBtn.부저 && switchBtn.축적 &&
                            switchBtn.구역1.Equals(ControlMode.Auto) && switchBtn.솔레노이드밸브.Equals(ControlMode.Stop) && switchBtn.구역2.Equals(ControlMode.Auto))
                        {
                            NextEnable();
                        }
                        else
                        {
                            NextDisable();
                        }

                    });
                    */
                }
                break;
            case GasSysSafetyCheckPSection.기동용기함선택:
                {
                    activationBoxBtn.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificButton(
                        activationBoxBtn);
                    NextDisable();

                    activationBoxBtn.onClick.AddListener(delegate
                    {
                        activationBoxBtn.gameObject.SetActive(false);
                        Next();
                    });
                }
                break;
            case GasSysSafetyCheckPSection.솔레노이드밸브1:
                {

                    bool isDetected = false;

                    ButtonManager.Instance.EnableSpecificButton(
                        activationBoxBtn);
                    ButtonManager.Instance.EnableSpecificImage(activationCylinderBoxPopup.aActivationObj,
                        activationCylinderBoxPopup.pinObj);
                    NextDisable();
                    activationCylinderBoxPopup.gameObject.SetActive(true);
                    activationCylinderBoxPopup.pinObj.SetActive(true);
                    activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                    uiDragAndCollisionHandler.StartDragging();
                    uiDragAndCollisionHandler.OnPicked += (obj) =>
                    {
                        activationCylinderBoxPopup.selectAttachPinObj.SetActive(true);
                    };
                    uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObjectHandleCollision) =>
                    {
                        if (isDetected)
                            return;
                        activationCylinderBoxPopup.selectPinObj.SetActive(false);
                        draggedObject.SetActive(false);
                        activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                        activationCylinderBoxPopup.selectAttachPinObj.SetActive(false);
                        ButtonManager.Instance.RemoveHighlightImage();
                        Next();
                        isDetected = true;
                    };
                }
                break;
            case GasSysSafetyCheckPSection.솔레노이드밸브2:
                inventoryObj.ShowSafetyPin(false);
                ButtonManager.Instance.EnableSpecificButton(activationCylinderBoxPopup.detachSolenoidBtn);
                NextDisable();
                activationCylinderBoxPopup.gameObject.SetActive(true);
                activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                activationCylinderBoxPopup.detachSolenoidBtn.gameObject.SetActive(true);
                activationCylinderBoxPopup.detachSolenoidBtn.onClick.AddListener(delegate
                {
                    activationCylinderBoxPopup.detachSolenoidBtn.gameObject.SetActive(false);
                    activationCylinderBoxPopup.solenoidValveObj.SetActive(false);
                    //NextEnable();
                    Next();
                });
                break;
            case GasSysSafetyCheckPSection.솔레노이드밸브3:
                ButtonManager.Instance.EnableSpecificButton(solenoidValvePopup.pinDetachBtn);
                NextDisable();
                solenoidValvePopup.gameObject.SetActive(true);
                solenoidValvePopup.aActivationPinObj.SetActive(true);
                solenoidValvePopup.clipObj.SetActive(true);
                solenoidValvePopup.pinDetachBtn.gameObject.SetActive(true);
                inventoryObj.ShowSafetyPin(false);
                solenoidValvePopup.pinDetachBtn.onClick.AddListener(delegate
                {
                    solenoidValvePopup.aActivationPinObj.SetActive(false);
                    inventoryObj.ShowSafetyPin(true);
                    solenoidValvePopup.pinDetachBtn.gameObject.SetActive(false);
                    //GlobalCanvas.Instance.ShowHint(false);
                    NextEnable();
                    //Next();
                });
                break;
            case GasSysSafetyCheckPSection.교육종료:
                {
                    NextDisable();
                    ButtonManager.Instance.EnableSpecificButton();
                    ButtonManager.Instance.EnableSpecificImage();
                    SetCompletePopup("점검 전 안전 조치를 모두 완료했습니다.", "기동용기 솔레노이드 밸브 격발시험을", InitManualOperation);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        //ButtonManager.Instance.HighlightObj();
    }

    private void EvaluationSafetyCheck(GasSysSafetyCheckPSection state)
    {

    }

    private void PracticeSafetyCheck(GasSysSafetyCheckPSection state)
    {

    }


    private void SwitchButton()
    {

    }

}
