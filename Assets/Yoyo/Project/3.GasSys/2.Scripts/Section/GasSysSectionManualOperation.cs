using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public partial class GasSysSection
{
    [Foldout("즉시격발")]
    private GasSysManualOperationPSection _curManualOperationPSection;
    private GasSysManualOperationESection _curManualOperationESection;
    [FormerlySerializedAs("manualOperationHintObj")]
    public HintScriptableObj manualOperationHintPObj;
    public HintScriptableObj manualOperationHintEObj;
    public GameObject 즉시격발평가1;
    [SerializeField] public Toggle 즉시격발토글1;
    [SerializeField] public Toggle 즉시격발토글2;
    [SerializeField] public Toggle 즉시격발토글3;
    [SerializeField] public Toggle 즉시격발토글4;
    [SerializeField] public Toggle 즉시격발토글5;
    [SerializeField] public Toggle 즉시격발토글6;
    [EndFoldout]
    private GasSysControlPanelState _gasSysMOE = new GasSysControlPanelState();
    public void InitManualOperation()
    {
        InitEMode();
        curMainSection = GasSysMainSection.SolenoidValveTest;
        curSolenoidValveTestSection = GasSysSolenoidValveTestSection.ManualOperation;
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
            GasSysState.PracticeMode => System.Enum.GetValues(typeof(GasSysManualOperationPSection)).Length,
            GasSysState.EvaluationMode => System.Enum.GetValues(typeof(GasSysManualOperationESection)).Length,
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



    private void ShowManualOperationSection(int index)
    {
        if (_gasSysState.Equals(GasSysState.PracticeMode))
        {
            prevBtn.gameObject.SetActive(true);
            GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(manualOperationHintPObj, (int)GasSysManualOperationPSection.감시제어반선택 + index));
            ChangeStateManualOperationP(GasSysManualOperationPSection.감시제어반선택 + index);
        }
        if (!_gasSysState.Equals(GasSysState.EvaluationMode))
            return;
        prevBtn.gameObject.SetActive(false);
        GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(manualOperationHintEObj, (int)GasSysSafetyCheckESection.Init + index));
        ChangeStateManualOperationE(GasSysManualOperationESection.Init + index);
    }


    private void ChangeStateManualOperationE(GasSysManualOperationESection state)
    {
        _curManualOperationESection = state;
        OnStateChangedManualOperationE(_curManualOperationESection);
    }

    private void OnStateChangedManualOperationE(GasSysManualOperationESection state)
    {
        ButtonManager.Instance.RemoveAllHighlights();
        //uiDragAndCollisionHandler.StartDragging();
        Debug.Log(state);
        switch (state)
        {
            case GasSysManualOperationESection.Init:
                {
                    InitStorageRoom();
                    NextEnable(false);
                    _gasSysEvaluation.Reset();

                    controlBoxBtn.gameObject.SetActive(true);
                    selectionValveBtn.gameObject.SetActive(true);
                    storageCylinderBtn.gameObject.SetActive(true);
                    activationBoxBtn.gameObject.SetActive(true);
                    solenoidValvePopup.mActivateBtn.gameObject.SetActive(true);

                    controlBoxBtn.onClick.RemoveAllListeners();
                    selectionValveBtn.onClick.RemoveAllListeners();
                    storageCylinderBtn.onClick.RemoveAllListeners();
                    activationBoxBtn.onClick.RemoveAllListeners();
                    solenoidValvePopup.clipDetachBtn.onClick.RemoveAllListeners();
                    solenoidValvePopup.mActivateBtn.onClick.RemoveAllListeners();



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
                        activationCylinderBoxPopup.Init();
                    });

                    selectionValvePopup.SetOnOff(false);
                    storageCylinderPopup.SetOnOff(false);

                    uiDragAndCollisionHandler.ResetEvent();
                    bool isDetected = false;
                    uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObjectHandleCollision) =>
                    {
                        if (isDetected)
                            return;
                        isDetected = true;
                        solenoidValvePopup.Init();
                        solenoidValvePopup.gameObject.SetActive(true);
                        solenoidValvePopup.clipObj.SetActive(true);
                        solenoidValvePopup.closeBtn.onClick.RemoveAllListeners();
                        solenoidValvePopup.closeBtn.onClick.AddListener(delegate
                        {
                            solenoidValvePopup.gameObject.SetActive(false);
                        });
                        activationCylinderBoxPopup.selectPinObj.SetActive(false);
                        draggedObject.SetActive(false);
                        activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                        activationCylinderBoxPopup.selectAttachPinObj.SetActive(false);
                        solenoidValvePopup.clipDetachBtn.gameObject.SetActive(true);
                        ButtonManager.Instance.RemoveHighlightImage();
                        solenoidValvePopup.clipDetachBtn.onClick.RemoveAllListeners();
                        solenoidValvePopup.clipDetachBtn.onClick.AddListener(delegate
                        {
                            solenoidValvePopup.clipObj.SetActive(false);
                            solenoidValvePopup.clipDetachBtn.gameObject.SetActive(false);
                            solenoidValvePopup.mActivateBtn.gameObject.SetActive(true);
                            inventoryObj?.ShowSafetyClip(true);
                        });
                        solenoidValvePopup.mActivateBtn.onClick.RemoveAllListeners();
                        solenoidValvePopup.mActivateBtn.onClick.AddListener(delegate
                        {
                            //solenoidValvePopup.mActivationObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(158, -51);
                            solenoidValvePopup.SolenoidValveMActivation();
                            solenoidValvePopup.mActivateBtn.gameObject.SetActive(false);
                            ControlPanel.Instance.ShowFire(true);
                            ControlPanel.Instance.SetTimeNum(0f);
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                        });
                        ButtonManager.Instance.SetEvaluationButtons();
                    };


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
                    // ButtonManager.Instance.EnableSpecificImage(activationCylinderBoxPopup.aActivationObj,
                    //     activationCylinderBoxPopup.pinObj, inventoryObj.solenoidValve1);
                    //NextEnable();
                    Next();

                }
                break;
            case GasSysManualOperationESection.E1:
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
            case GasSysManualOperationESection.E2:
                {
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    ControlPanel.Instance.ShowPanel(true);
                    SoundManager.Instance.StopAllFireSound();
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    ControlPanel.Instance.CheckSwitchBtn();
                    solenoidValvePopup.gameObject.SetActive(false);
                    inventoryObj?.ShowSolenoidValve1(true);
                    inventoryObj?.ShowSafetyClip(false);
                    uiDragAndCollisionHandler.StartDragging();
                    uiDragAndCollisionHandler.ResetEvent();
                    bool isDetected = false;
                    uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObjectHandleCollision) =>
                    {
                        if (isDetected)
                            return;
                        isDetected = true;
                        draggedObject.SetActive(false);
                        Next();

                        //ButtonManager.Instance.SetEvaluationButtons();
                    };
                    ControlPanel.Instance.SetClose();
                    //ButtonManager.Instance.SetEvaluationButtons();
                    ButtonManager.Instance.EnableSpecificImage(activationCylinderBoxPopup.aActivationObj,
                        activationCylinderBoxPopup.pinObj, inventoryObj.solenoidValve1);

                }
                break;
            case GasSysManualOperationESection.E3:
                {
                    uiDragAndCollisionHandler.StopDragging();
                    uiDragAndCollisionHandler.ResetEvent();
                    solenoidValvePopup.Init();
                    solenoidValvePopup.gameObject.SetActive(true);
                    solenoidValvePopup.clipObj.SetActive(true);
                    solenoidValvePopup.closeBtn.onClick.RemoveAllListeners();
                    solenoidValvePopup.closeBtn.onClick.AddListener(delegate
                    {
                        solenoidValvePopup.gameObject.SetActive(false);
                    });
                    activationCylinderBoxPopup.selectPinObj.SetActive(false);
                    activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                    activationCylinderBoxPopup.selectAttachPinObj.SetActive(false);
                    solenoidValvePopup.clipDetachBtn.gameObject.SetActive(true);
                    ButtonManager.Instance.RemoveHighlightImage();
                    solenoidValvePopup.clipDetachBtn.onClick.RemoveAllListeners();
                    solenoidValvePopup.clipDetachBtn.onClick.AddListener(delegate
                    {
                        solenoidValvePopup.clipObj.SetActive(false);
                        solenoidValvePopup.clipDetachBtn.gameObject.SetActive(false);
                        solenoidValvePopup.mActivateBtn.gameObject.SetActive(true);
                        inventoryObj?.ShowSafetyClip(true);
                    });
                    solenoidValvePopup.mActivateBtn.onClick.RemoveAllListeners();
                    solenoidValvePopup.mActivateBtn.onClick.AddListener(delegate
                    {
                        //solenoidValvePopup.mActivationObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(158, -51);
                        solenoidValvePopup.SolenoidValveMActivation();
                        solenoidValvePopup.mActivateBtn.gameObject.SetActive(false);
                        ControlPanel.Instance.ShowFire(true);
                        ControlPanel.Instance.SetTimeNum(0f);
                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                        _gasSysEvaluation.격발 = true;
                    });
                }
                break;
            case GasSysManualOperationESection.E4:
                {
                    uiDragAndCollisionHandler.StopDragging();
                    즉시격발평가1.SetActive(true);
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.closeBtn.gameObject.SetActive(false);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    SoundManager.Instance.StopAllFireSound();
                    //ButtonManager.Instance.DisableAllButtons();
                    NextEnable(false);
                    solenoidValvePopup.gameObject.SetActive(false);
                }
                break;
            case GasSysManualOperationESection.평가종료:
                {
                    즉시격발평가1.SetActive(false);
                    ButtonManager.Instance.EnableSpecificButton();
                    NextDisable();
                    GasSysGlobalCanvas.Instance.totalScore.즉시격발.감시제어반조치 = !_gasSysEvaluation.주경종 && !_gasSysEvaluation.지구경종 && !_gasSysEvaluation.사이렌 &&
                                                                          !_gasSysEvaluation.비상방송 && !_gasSysEvaluation.부저 && _gasSysEvaluation.축적 &&
                                                                          _gasSysEvaluation.솔레노이드연동;
                    GasSysGlobalCanvas.Instance.totalScore.즉시격발.격발 = _gasSysEvaluation.격발;
                    GasSysGlobalCanvas.Instance.totalScore.즉시격발.동작확인 = !즉시격발토글1.isOn &&
                                                                       !즉시격발토글2.isOn &&
                                                                       즉시격발토글3.isOn &&
                                                                       !즉시격발토글4.isOn &&
                                                                       !즉시격발토글5.isOn &&
                                                                       !즉시격발토글6.isOn;
                    var results = new List<ResultObject>
                    {
                        new ResultObject()
                        {
                            Title = "감시제어반 조치",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.즉시격발.감시제어반조치
                        },
                        new ResultObject()
                        {
                            Title = "수동조작버튼작동[즉시격발] 시험",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.즉시격발.격발
                        },
                        new ResultObject()
                        {
                            Title = "정상작동 상태확인",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.즉시격발.동작확인
                        }
                    };

                    GasSysGlobalCanvas.Instance.totalScore.즉시격발List.Clear();
                    GasSysGlobalCanvas.Instance.totalScore.즉시격발List.AddRange(results);
                    GasSysGlobalCanvas.Instance.SetNextEvaluation(InitManualControlBox, InitManualOperation);
                    //GasSysGlobalCanvas.Instance.SetResultPopup(results);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void ChangeStateManualOperationP(GasSysManualOperationPSection state)
    {

        _curManualOperationPSection = state;
        OnStateChangedManualOperationP(_curManualOperationPSection);
    }


    private void OnStateChangedManualOperationP(GasSysManualOperationPSection state)
    {
        InitStorageRoom();
        ButtonManager.Instance.RemoveAllHighlights();
        uiDragAndCollisionHandler.StopDragging();
        ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
        switch (state)
        {

            case GasSysManualOperationPSection.감시제어반선택:
                {

                    ChangeValveState(false);
                    ChangeStorageValveState(false);
                    inventoryObj?.ShowSolenoidValve1(true);
                    inventoryObj?.ShowSolenoidValve2(true);
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
            case GasSysManualOperationPSection.제어반음향및솔레노이드밸브연동:
                {
                    inventoryObj?.ShowSolenoidValve1(true);
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    temp.축적 = true;
                    temp.복구 = false;
                    SetHighlightControlPanel(temp, true);
                    NextDisable();
                    temp.SetBtn(false);
                    temp.축적 = true;
                    SetHighlightControlPanelCheck(temp);
                }
                break;
            // case GasSysManualOperationPSection.축적비축적유지:
            //     {
            //         inventoryObj?.ShowSolenoidValve1(true);
            //         var btnList = new List<SwitchButtonCheck>();
            //         ControlPanel.Instance.ShowPanel(true);
            //         ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
            //         ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
            //         ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
            //         ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
            //         ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
            //         ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
            //         ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
            //         var temp = new ControlPanelButtonCheck();
            //         temp.SetBtn(false);
            //         SetHighlightControlPanel(temp);
            //         NextDisable();
            //         temp.SetBtn(false);
            //         temp.축적 = true;
            //         SetHighlightControlPanelCheck(temp);
            //         NextEnable();
            //     }
            //     break;
            case GasSysManualOperationPSection.보관함에서솔레노이드:
                {
                    inventoryObj?.ShowSolenoidValve1(true);
                    inventoryObj?.ShowSafetyClip(false);
                    bool isNext = false;
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.ShowFire(false);
                    ControlPanel.Instance.InitTimer();
                    SoundManager.Instance.StopAllFireSound();
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
                    bool isDetected = false;
                    ButtonManager.Instance.EnableSpecificImage(inventoryObj?.solenoidValve1);
                    uiDragAndCollisionHandler.ResetEvent();
                    uiDragAndCollisionHandler.StartDragging();
                    uiDragAndCollisionHandler.OnPicked += (obj) =>
                    {


                    };
                    uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObjectHandleCollision) =>
                    {
                        if (isDetected)
                            return;
                        isDetected = true;
                        solenoidValvePopup.Init();
                        solenoidValvePopup.gameObject.SetActive(true);
                        solenoidValvePopup.clipObj.SetActive(true);
                        activationCylinderBoxPopup.selectPinObj.SetActive(false);
                        draggedObject.SetActive(false);
                        activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                        activationCylinderBoxPopup.selectAttachPinObj.SetActive(false);
                        ButtonManager.Instance.RemoveHighlightImage();
                        Next();
                    };
                }
                break;
            case GasSysManualOperationPSection.안전클립제거및격발:
                {
                    ControlPanel.Instance.ShowFire(false);
                    inventoryObj?.ShowSafetyClip(false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
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

                    ButtonManager.Instance.EnableSpecificImage(inventoryObj?.solenoidValve1);

                    NextDisable();
                    inventoryObj?.ShowSolenoidValve1(false);
                    solenoidValvePopup.Init();
                    solenoidValvePopup.gameObject.SetActive(true);
                    solenoidValvePopup.clipObj.SetActive(true);
                    solenoidValvePopup.clipDetachBtn.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificButton(solenoidValvePopup.clipDetachBtn);
                    solenoidValvePopup.clipDetachBtn.onClick.AddListener(delegate
                    {
                        ButtonManager.Instance.EnableSpecificButton(solenoidValvePopup.mActivateBtn);
                        solenoidValvePopup.clipObj.SetActive(false);
                        solenoidValvePopup.clipDetachBtn.gameObject.SetActive(false);
                        solenoidValvePopup.mActivateBtn.gameObject.SetActive(true);
                        inventoryObj?.ShowSafetyClip(true);
                    });
                    solenoidValvePopup.mActivateBtn.onClick.AddListener(delegate
                    {
                        ButtonManager.Instance.EnableSpecificButton(_switchButtons);
                        //solenoidValvePopup.mActivationObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(158, -51);
                        solenoidValvePopup.SolenoidValveMActivation();
                        solenoidValvePopup.mActivateBtn.gameObject.SetActive(false);
                        ControlPanel.Instance.ShowFire(true);
                        ControlPanel.Instance.SetTimeNum(0f);
                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                        NextEnable();
                    });
                    GasSysGlobalCanvas.Instance.HideCheckObj();
                }
                break;
            /*
            case GasSysManualOperationPSection.솔정상확인:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    // ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    ControlPanel.Instance.ShowFire(true);
                    ControlPanel.Instance.SetTimeNum(0f);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(false);
                    temp.축적 = true;
                    SetHighlightControlPanelCheck(temp, false);
                    _switchButtons.Add(new SwitchButtonCheck()
                    {
                        btn = GasSysGlobalCanvas.Instance.GetCheckAgreeBtn(),
                        select = false
                    });
                    ButtonManager.Instance.EnableSpecificButton(_switchButtons);
                    //GasSysGlobalCanvas.Instance.ShowSol3(Next);
                    Next();
                }
                break;
                */
            case GasSysManualOperationPSection.음향정지작동확인:
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
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(true);
                    temp.축적 = false;
                    temp.복구 = false;
                    SetHighlightControlPanel(temp);
                    //NextEnable(false);
                    NextDisable();
                    temp.SetBtn(true);
                    temp.축적 = true;
                    temp.복구 = false;
                    SetHighlightControlPanel(temp, true);
                    SetHighlightControlPanelCheck(temp);
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
            case GasSysManualOperationPSection.제어반화재표시:
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
            case GasSysManualOperationPSection.방호구역1:
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
                    // _switchButtons.Add(new SwitchButtonCheck()
                    // {
                    //     btn = GlobalCanvas.Instance.GetCheckAgreeBtn(),
                    //     select = false
                    // });
                    //ButtonManager.Instance.EnableSpecificButton(_switchButtons);
                }
                break;
            /*
            case GasSysManualOperationPSection.방호구역1음향연동:
                {
                    ControlPanel.Instance.SetTimeNum(0f);
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(true);
                    temp.축적 = false;
                    temp.복구 = false;
                    //SetHighlightControlPanel(temp);
                    NextDisable();
                    //NextEnable(false);
                    temp.SetBtn(false);
                    temp.축적 = true;
                    temp.복구 = false;
                    SetHighlightControlPanel(temp, true);
                    SetHighlightControlPanelCheck(temp);
                    GasSysGlobalCanvas.Instance.ShowArea1(Next);
                    //GasSysSoundManager.Instance.MuteSiren(false);
                    SoundManager.Instance.SetDefaultVolume();
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                    //ButtonManager.Instance.EnableSpecificButton(_switchButtons);
                }
                break;
                */
            case GasSysManualOperationPSection.음향정지:
                {
                    ControlPanel.Instance.SetTimeNum(0f);
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
                    //temp.지구경종 = true;
                    //temp.사이렌 = true;
                    //temp.비상방송 = true;
                    temp.복구 = false;
                    SetHighlightControlPanel(temp, true);
                    SetHighlightControlPanelCheck(temp);
                    GasSysGlobalCanvas.Instance.ShowArea1(Next);
                    //GasSysSoundManager.Instance.MuteSiren(false);
                    SoundManager.Instance.SetDefaultVolume();
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                }
                break;
            case GasSysManualOperationPSection.방호구역2:
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
                    GasSysGlobalCanvas.Instance.ShowArea2(Next);
                    //GasSysSoundManager.Instance.MuteSiren(true);
                    SoundManager.Instance.SetSirenVolume(0f);
                    ControlPanel.OnTimerEnd.RemoveAllListeners();
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                }
                break;
            /*
            case GasSysManualOperationPSection.지연장치:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    GasSysGlobalCanvas.Instance.ShowTimer(Next);
                    GasSysSoundManager.Instance.SetDefaultVolume();
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
                        GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(true);
                        // Observable.Timer(System.TimeSpan.FromSeconds(3))
                        //     .Subscribe(_ =>
                        //     {
                        //         //GasSysManager.Instance.ChangeState(GasSysState.Init);
                        //     })
                        //     .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
                    });
                }
                break;
                */
            case GasSysManualOperationPSection.솔레노이드작동여부:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    GasSysGlobalCanvas.Instance.ShowSol1(Next);
                    SoundManager.Instance.SetDefaultVolume();
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    ControlPanel.Instance.InitArea1Control(ControlMode.Auto);
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
            //case GasSysManualOperationSection.수동조작버튼:
            //case GasSysManualOperationSection.격발테스트:
            case GasSysManualOperationPSection.교육종료:
                {
                    NextDisable();
                    SetCompletePopup("수동조작버튼작동[즉시격발] 시험을 모두 완료했습니다.", "다음 점검을", InitManualControlBox);
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        //ButtonManager.Instance.HighlightObj();
    }

    /*


    private HintTextAndAudio GetHintTextAndAudio(int index)
    {
        return new HintTextAndAudio()
        {
            title = hintScriptableObj.hintData[index].title,
            text = hintScriptableObj.hintData[index].text,
            audioClip = hintScriptableObj.hintData[index].audioClip
        };
    }

    public void ChangeState(GasSysSafetyCheckSection state)
    {

        _curState = state;
        OnStateChanged(_curState);
    }

    private void OnStateChanged(GasSysSafetyCheckSection state)
    {
        InitStorageRoom();
        ButtonManager.Instance.RemoveAllHighlights();
        //nextBtn.onClick.RemoveAllListeners();
        switch (state)
        {
            case GasSysSafetyCheckSection.선택밸브조작동관선택:
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
                        NextEnable();
                    });
                }
                break;
            case GasSysSafetyCheckSection.선택밸브조작동관분리:
                {
                    ButtonManager.Instance.EnableSpecificButton(selectionValvePopup.GetButtons());
                    NextDisable();
                    selectionValvePopup.InitPopup(true, isOn =>
                    {
                        selectionValve01.ChangeValveState(isOn);
                        if (!isOn)
                        {
                            NextEnable();
                        }
                    });
                    // nextBtn.onClick.AddListener(delegate
                    // {
                    //     ChangeState(GasSysSafetyCheckSection.저장용기조작동관선택);
                    // });
                }
                break;
            case GasSysSafetyCheckSection.저장용기조작동관선택:
                {
                    storageCylinderBtn.gameObject.SetActive(true);
                    ChangeStorageValveState(true);
                    ButtonManager.Instance.EnableSpecificButton(storageCylinderBtn);
                    NextDisable();
                    storageCylinderBtn.onClick.AddListener(delegate
                    {
                        storageCylinderBtn.gameObject.SetActive(false);
                        NextEnable();
                    });
                }
                break;
            case GasSysSafetyCheckSection.저장용기조작동관분리:
                {
                    ButtonManager.Instance.EnableSpecificButton(storageCylinderPopup.GetButtons());
                    NextDisable();
                    storageCylinderPopup.InitPopup(true, isOn =>
                    {
                        storageCylinder.ChangeValveState(isOn);
                        if (!isOn)
                        {
                            NextEnable();
                        }
                    });
                }
                break;
            case GasSysSafetyCheckSection.감시제어반선택:
                {
                    controlBoxBtn.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificButton(controlBoxBtn);
                    NextDisable();
                    controlBoxBtn.onClick.AddListener(delegate
                    {
                        controlBoxBtn.gameObject.SetActive(false);
                        //ControlPanel.Instance.ShowPanel(true);
                        NextEnable();
                    });
                }
                break;
            case GasSysSafetyCheckSection.음향활성:
                {
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(false);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
                        select = false
                    });
                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
                        select = false
                    });
                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
                        select = false
                    });
                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
                        select = false
                    });
                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
                        select = false
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
                            select = !switchBtn.축적
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

                        if (switchBtn.주경종 && switchBtn.지구경종 && switchBtn.사이렌 && switchBtn.비상방송 && switchBtn.부저 && !switchBtn.축적 &&
                            switchBtn.구역1.Equals(ControlMode.Auto) && switchBtn.솔레노이드밸브.Equals(ControlMode.Auto) && switchBtn.구역2.Equals(ControlMode.Auto))
                        {
                            NextEnable();
                        }
                        else
                        {
                            NextDisable();
                        }
                    });
                }
                break;
            /*
            case GasSysSafetyCheckSection.주경종:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(false);
                    ButtonManager.Instance.EnableSpecificButton(
                        ControlPanel.Instance.GetSwitchButton("주경종").GetButton());
                    NextDisable();

                    _disposable.Clear();
                    var disposable = ControlPanel.Instance.onSwitchBtnValueChangeEvent.AsObservable()
                        .Subscribe(data =>
                        {
                            if (!data.Item1.Equals("주경종"))
                                return;
                            if (data.Item2)
                            {
                                NextEnable();
                            }
                            else
                            {
                                NextDisable();
                            }
                        }).AddTo(this);
                    _disposable?.Add(disposable);
                }
                break;
            case GasSysSafetyCheckSection.지구경종:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(false);
                    ButtonManager.Instance.EnableSpecificButton(
                        ControlPanel.Instance.GetSwitchButton("지구경종").GetButton());
                    NextDisable();

                    _disposable.Clear();
                    var disposable = ControlPanel.Instance.onSwitchBtnValueChangeEvent.AsObservable()
                        .Subscribe(data =>
                        {
                            if (!data.Item1.Equals("지구경종"))
                                return;
                            if (data.Item2)
                            {
                                NextEnable();
                            }
                            else
                            {
                                NextDisable();
                            }
                        }).AddTo(this);
                    _disposable?.Add(disposable);
                }
                break;
            case GasSysSafetyCheckSection.사이렌:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(false);
                    ButtonManager.Instance.EnableSpecificButton(
                        ControlPanel.Instance.GetSwitchButton("사이렌").GetButton());
                    NextDisable();

                    _disposable.Clear();
                    var disposable = ControlPanel.Instance.onSwitchBtnValueChangeEvent.AsObservable()
                        .Subscribe(data =>
                        {
                            if (!data.Item1.Equals("사이렌"))
                                return;
                            if (data.Item2)
                            {
                                NextEnable();
                            }
                            else
                            {
                                NextDisable();
                            }
                        }).AddTo(this);
                    _disposable?.Add(disposable);
                }
                break;
            case GasSysSafetyCheckSection.비상방송:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(false);
                    ButtonManager.Instance.EnableSpecificButton(
                        ControlPanel.Instance.GetSwitchButton("비상방송").GetButton());
                    NextDisable();

                    _disposable.Clear();
                    var disposable = ControlPanel.Instance.onSwitchBtnValueChangeEvent.AsObservable()
                        .Subscribe(data =>
                        {
                            if (!data.Item1.Equals("비상방송"))
                                return;
                            if (data.Item2)
                            {
                                NextEnable();
                            }
                            else
                            {
                                NextDisable();
                            }
                        }).AddTo(this);
                    _disposable?.Add(disposable);
                }
                break;
            case GasSysSafetyCheckSection.부저:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(false);
                    ButtonManager.Instance.EnableSpecificButton(
                        ControlPanel.Instance.GetSwitchButton("부저").GetButton());
                    NextDisable();

                    _disposable.Clear();
                    var disposable = ControlPanel.Instance.onSwitchBtnValueChangeEvent.AsObservable()
                        .Subscribe(data =>
                        {
                            if (!data.Item1.Equals("부저"))
                                return;
                            if (data.Item2)
                            {
                                NextEnable();
                            }
                            else
                            {
                                NextDisable();
                            }
                        }).AddTo(this);
                    _disposable?.Add(disposable);
                }
                break;

            case GasSysSafetyCheckSection.축적비축적:
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
                }
                break;
            case GasSysSafetyCheckSection.솔레노이드밸브메인정지:
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
                }
                break;
            case GasSysSafetyCheckSection.기동용기함선택:
                {
                    activationBoxBtn.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificButton(
                        activationBoxBtn);
                    NextDisable();

                    activationBoxBtn.onClick.AddListener(delegate
                    {
                        activationBoxBtn.gameObject.SetActive(false);
                        NextEnable();
                    });
                }
                break;
            case GasSysSafetyCheckSection.솔레노이드밸브1:

                ButtonManager.Instance.EnableSpecificButton(
                    activationBoxBtn);
                ButtonManager.Instance.EnableSpecificImage(activationCylinderBoxPopup.aActivationObj,
                    activationCylinderBoxPopup.pinObj);
                NextDisable();
                activationCylinderBoxPopup.gameObject.SetActive(true);
                activationCylinderBoxPopup.pinObj.SetActive(true);
                activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                uiDragAndCollisionHandler.OnPicked += (obj) =>
                {
                    activationCylinderBoxPopup.selectAttachPinObj.SetActive(true);
                };
                uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObjectHandleCollision) =>
                {
                    activationCylinderBoxPopup.selectPinObj.SetActive(false);
                    draggedObject.SetActive(false);
                    activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                    activationCylinderBoxPopup.selectAttachPinObj.SetActive(false);
                    ButtonManager.Instance.RemoveHighlightImage();
                    NextEnable();
                };
                break;
            case GasSysSafetyCheckSection.솔레노이드밸브2:
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
                    NextEnable();
                });
                break;
            case GasSysSafetyCheckSection.솔레노이드밸브3:
                ButtonManager.Instance.EnableSpecificButton(solenoidValvePopup.pinDetachBtn);
                NextDisable();
                solenoidValvePopup.gameObject.SetActive(true);
                solenoidValvePopup.aActivationPinObj.SetActive(true);
                solenoidValvePopup.clipObj.SetActive(true);
                solenoidValvePopup.pinDetachBtn.gameObject.SetActive(true);
                solenoidValvePopup.pinDetachBtn.onClick.AddListener(delegate
                {
                    solenoidValvePopup.aActivationPinObj.SetActive(false);
                    solenoidValvePopup.inventory.ShowSafetyPin(true);
                    solenoidValvePopup.pinDetachBtn.gameObject.SetActive(false);
                    //GlobalCanvas.Instance.ShowHint(false);
                    NextEnable();
                });
                break;
            case GasSysSafetyCheckSection.교육종료:

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void NextDisable()
    {
        //ButtonManager.Instance.NextDisable();
        nextBtn.interactable = false;
        ColorBlock colors = nextBtn.colors;
        //colors.disabledColor = colors.normalColor;
        nextImg.color = colors.disabledColor;
        nextText.color = colors.disabledColor;
    }

    public void NextEnable()
    {
        //ButtonManager.Instance.NextEnable();
        nextBtn.interactable = true;
        ColorBlock colors = nextBtn.colors;
        //colors.disabledColor = colors.normalColor;
        nextImg.color = colors.normalColor;
        nextText.color = colors.normalColor;
        ButtonManager.Instance.HighlightButton(nextBtn);
    }

    private void ShowRoom(GameObject obj)
    {
        storageRoomObj.gameObject.SetActive(storageRoomObj.gameObject.Equals(obj));
        area1Obj.gameObject.SetActive(area1Obj.gameObject.Equals(obj));
        area1CorridorObj.gameObject.SetActive(area1CorridorObj.gameObject.Equals(obj));
    }

    public void Prev()
    {
        if (_curSection <= 0)
            return;
        _curSection--;
        ShowSection(_curSection);
        UpdateBtn();
    }

    public void Next()
    {
        if (_curSection >= _maxSection - 1)
            return;
        _curSection++;
        ShowSection(_curSection);
        UpdateBtn();
    }

    private void ShowSection(int index)
    {
        ChangeState(GasSysSafetyCheckSection.선택밸브조작동관선택 + index);
        GlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio((int)GasSysSafetyCheckSection.선택밸브조작동관선택 + index));
        //safetySections[index].Init();
    }

    public void SetSectionRange(int minIndex, int maxIndex, int value)
    {
        _minSection = Mathf.Clamp(minIndex, 0, value - 1); // 최소 인덱스가 범위를 벗어나지 않도록 제한
        _maxSection = Mathf.Clamp(maxIndex, 0, value - 1); // 최대 인덱스도 제한
        _curSection = _minSection; // 범위 내에서 처음 페이지로 이동
        UpdateBtn();
        ShowSection(_curSection);
    }

    private void UpdateBtn()
    {
        prevBtn.interactable = _curSection > _minSection; // 첫 번째 페이지일 때 이전 버튼 비활성화
        //nextBtn.interactable = _curSection < _maxSection; // 마지막 페이지일 때 다음 버튼 비활성화


        if (!prevBtn.interactable)
        {
            ColorBlock colors = prevBtn.colors;
            //colors.disabledColor = colors.normalColor;
            prevImg.color = colors.disabledColor;
            prevText.color = colors.disabledColor;
        }
        else
        {
            ColorBlock colors = prevBtn.colors;
            //colors.disabledColor = colors.normalColor;
            prevImg.color = colors.normalColor;
            prevText.color = colors.normalColor;
        }

        if (_minSection.Equals(_maxSection))
        {
            prevBtn.gameObject.SetActive(false);
            nextBtn.gameObject.SetActive(false);
        }
        else
        {
            prevBtn.gameObject.SetActive(true);
            nextBtn.gameObject.SetActive(true);
        }
    }

    private void ChangeValveState(bool isOn)
    {
        selectionValve01.ChangeValveState(isOn);
    }

    private void ChangeStorageValveState(bool isOn)
    {
        storageCylinder.ChangeValveState(isOn);
    }

    private void SwitchButton()
    {

    }

*/
}
