using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using VInspector;

public partial class GasSysSection
{
    [Foldout("점검 후 복구")]
    private GasSysRecoveryCheckPSection _curRecoveryCheckPSection;
    private GasSysRecoveryCheckESection _curRecoveryCheckESection;
    [FormerlySerializedAs("recoveryHintObj")]
    public HintScriptableObj recoveryHintPObj;
    public HintScriptableObj recoveryHintEObj;
    [EndFoldout]
    private readonly Subject<(GameObject draggedObject, GameObject targetObject)> collisionSubject
        = new Subject<(GameObject draggedObject, GameObject targetObject)>();
    public void InitRecoveryCheck()
    {
        InitEMode();
        curMainSection = GasSysMainSection.RecoveryCheck;
        nextBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.RemoveAllListeners();
        areaManagerObj?.StartArea(GasSysAreaManager.StartAreaType.StorageRoom);
        areaManagerObj?.ShowPanel(true);
        ShowRoom(storageRoomObj?.gameObject);
        //storageRoomObj?.InitSafetyCheck();
        inventoryObj?.Init();
        inventoryObj?.ShowPanel(true);
        inventoryObj?.ShowRecoverySolenoidValve1(true);
        inventoryObj?.ShowSolenoidValve2(true);
        ControlPanel.Instance.Init();
        //ChangeState(GasSysSafetyCheckSection.선택밸브조작동관선택);
        _maxSection = _gasSysState switch
        {
            GasSysState.PracticeMode => Enum.GetValues(typeof(GasSysRecoveryCheckPSection)).Length,
            GasSysState.EvaluationMode => Enum.GetValues(typeof(GasSysRecoveryCheckESection)).Length,
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

        GasSysGlobalCanvas.Instance.HideCheckObj();
    }


    private void ShowRecoverySection(int index)
    {

        if (_gasSysState.Equals(GasSysState.PracticeMode))
        {
            prevBtn.gameObject.SetActive(true);
            GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(recoveryHintPObj, (int)GasSysRecoveryCheckPSection.감시제어반선택 + index));
            ChangeStateRecoveryCheckP(GasSysRecoveryCheckPSection.감시제어반선택 + index);
        }
        if (!_gasSysState.Equals(GasSysState.EvaluationMode))
            return;
        prevBtn.gameObject.SetActive(false);
        GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(recoveryHintEObj, (int)GasSysRecoveryCheckESection.Init + index));
        ChangeStateRecoveryCheckE(GasSysRecoveryCheckESection.Init + index);


    }


    private void ChangeStateRecoveryCheckE(GasSysRecoveryCheckESection state)
    {

        _curRecoveryCheckESection = state;
        OnStateChangedRecoveryE(_curRecoveryCheckESection);
    }


    public void TriggerCollision(GameObject draggedObject, GameObject targetObject)
    {
        // 이벤트 발생 시 Subject에 데이터 발행
        collisionSubject.OnNext((draggedObject, targetObject));
        Debug.Log($"draggedObject: {draggedObject} targetObject: {targetObject}");
    }

    private void OnStateChangedRecoveryE(GasSysRecoveryCheckESection state)
    {
        _disposable.Clear();
        switch (state)
        {

            case GasSysRecoveryCheckESection.Init:
                {
                    InitStorageRoom();
                    inventoryObj?.ShowRecoverySolenoidValve1(false);
                    inventoryObj?.ShowSolenoidValve1(true);
                    NextEnable(false);
                    _gasSysEvaluation.Reset();
                    ButtonManager.Instance.EnableSpecificImage(activationCylinderBoxPopup.aActivationObj,
                        activationCylinderBoxPopup.pinObj);

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
                    // activationBoxBtn.onClick.AddListener(delegate
                    // {
                    //     //inventoryObj?.ShowSafetyPin(false);
                    //     activationCylinderBoxPopup.Init();
                    //     activationCylinderBoxPopup.closeBtn.gameObject.SetActive(true);
                    //     activationCylinderBoxPopup.closeBtn.onClick.AddListener(delegate
                    //     {
                    //         activationCylinderBoxPopup.gameObject.SetActive(false);
                    //     });
                    //     activationCylinderBoxPopup.gameObject.SetActive(true);
                    //     // activationCylinderBoxPopup.InitSafetyCheck(delegate
                    //     // {
                    //     //     solenoidValvePopup.InitSafetyCheck();
                    //     //     solenoidValvePopup.gameObject.SetActive(true);
                    //     // });
                    //
                    // });

                    ChangeValveState(false);
                    ChangeStorageValveState(false);
                    selectionValvePopup.SetOnOff(false);
                    storageCylinderPopup.SetOnOff(false);
                    inventoryObj.ShowSafetyPin(true);

                    ControlPanel.Instance.ShowFire(true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                    //ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    ControlPanel.Instance.SetClose();
                    //NextEnable();
                    var switchBtnList = ControlPanel.Instance.controlPanelButton;
                    _gasSysEvaluation.주경종 = switchBtnList.주경종;
                    _gasSysEvaluation.지구경종 = switchBtnList.지구경종;
                    _gasSysEvaluation.사이렌 = switchBtnList.사이렌;
                    _gasSysEvaluation.비상방송 = switchBtnList.비상방송;
                    _gasSysEvaluation.부저 = switchBtnList.부저;
                    _gasSysEvaluation.축적 = switchBtnList.축적;
                    ButtonManager.Instance.SetEvaluationButtons();
                    Next();
                    //Next();
                }
                break;
            case GasSysRecoveryCheckESection.E1:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.RecoverySwitchEvent?.AddListener(delegate
                    {
                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, false);
                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                        ControlPanel.Instance.ShowFire(false);
                        _gasSysEvaluation.복구 = true;
                    });
                    uiDragAndCollisionHandler.ResetEvent();
                    uiDragAndCollisionHandler.OnCollisionDetected += TriggerCollision;
                }
                break;
            case GasSysRecoveryCheckESection.E2:
                {
                    ControlPanel.Instance.RecoverySwitchEvent?.RemoveAllListeners();
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    ControlPanel.Instance.ShowFire(false);
                    inventoryObj.ShowSafetyPin(true);
                    uiDragAndCollisionHandler.StartDragging();

                    var t = collisionSubject
                        // 조건 1: draggedObject의 태그가 "Player"인지 확인
                        .Where(collision => collision.draggedObject.name.Equals("InventoryItem(솔레노이드밸브1)") && collision.targetObject.name.Equals("SolenoidValveInventoryCol"))
                        .Do(collision =>
                        {
                            Debug.Log($"조건 1 만족: {collision.targetObject.name}");
                            solenoidValvePopup.gameObject.SetActive(true);
                            solenoidValvePopup.clipObj.SetActive(true);
                            solenoidValvePopup.wallObj.SetActive(true);
                            solenoidValvePopup.mActivationObj.SetActive(true);
                            solenoidValvePopup.SolenoidValveAActivation(false);
                            inventoryObj?.ShowSolenoidValve1(false);
                            inventoryObj?.ShowSolenoidValve2(false);
                        })
                        // 조건 2: 조건 1 만족 후 targetObject의 레이어가 "Enemy"인지 확인
                        .SelectMany(collision =>
                            collisionSubject.Where(nextCollision => nextCollision.draggedObject.name.Equals("InventoryItem(안전핀)") && nextCollision.targetObject.name.Equals("Image")))
                        .Do(collision =>
                        {
                            Debug.Log($"조건 2 만족: {collision.targetObject.name}");
                            if (!solenoidValvePopup.recoveryAni.isPlaying)
                            {
                                solenoidValvePopup.recoveryAni.clip = solenoidValvePopup.recoveryAni.GetClip("NewRecoveryAni");
                                solenoidValvePopup.recoveryAni.Play();
                            }
                            inventoryObj?.ShowSafetyPin(false);
                            SolenoidValvePopup.PinAttach?.RemoveAllListeners();
                            SolenoidValvePopup.PinAttach?.AddListener(delegate
                            {
                                ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                                _gasSysEvaluation.솔레노이드복구 = true;

                                //NextEnable();
                            });
                        })
                        // 조건 3: 조건 2 만족 후 draggedObject의 이름이 특정 값인지 확인
                        .SelectMany(collision =>
                            collisionSubject.Where(nextCollision => nextCollision.draggedObject.name.Equals("RecoverPin")))
                        .Subscribe(finalCollision =>
                        {
                            SolenoidValvePopup.PinAttach?.RemoveAllListeners();
                            Debug.Log($"조건 3 만족: {finalCollision.draggedObject.name} -> {finalCollision.targetObject.name}");
                            solenoidValvePopup.recoveryPinObj.SetActive(false);
                            solenoidValvePopup.aActivationPinObj.SetActive(true);
                            solenoidValvePopup.closeBtn.gameObject.SetActive(true);
                            solenoidValvePopup.closeBtn.onClick.RemoveAllListeners();
                            solenoidValvePopup.closeBtn.onClick.AddListener(delegate
                            {
                                inventoryObj?.ShowRecoverySolenoidValve1(true);
                                inventoryObj?.ShowSolenoidValve2(true);
                                solenoidValvePopup.gameObject.SetActive(false);
                            });
                            //HandleFinalCollision(finalCollision.draggedObject, finalCollision.targetObject);
                        })
                        .AddTo(this);

                    _disposable.Add(t);

                    /*
                    bool isDetected = false;
                    uiDragAndCollisionHandler.OnCollisionDetected += (_, _) =>
                    {
                        if (isDetected)
                            return;
                        isDetected = true;
                        solenoidValvePopup.gameObject.SetActive(true);
                        solenoidValvePopup.clipObj.SetActive(true);
                        solenoidValvePopup.wallObj.SetActive(true);
                        solenoidValvePopup.mActivationObj.SetActive(true);
                        solenoidValvePopup.SolenoidValveAActivation(false);
                        inventoryObj?.ShowSolenoidValve1(false);
                        inventoryObj?.ShowSolenoidValve2(false);
                    };
                    */

                }
                break;
            case GasSysRecoveryCheckESection.E3:
                {

                    solenoidValvePopup.gameObject.SetActive(false);
                    solenoidValvePopup.clipObj.SetActive(true);
                    solenoidValvePopup.wallObj.SetActive(true);
                    inventoryObj.ShowSafetyPin(false);
                    inventoryObj?.ShowSolenoidValve1(false);
                    inventoryObj?.ShowRecoverySolenoidValve1(true);
                    inventoryObj?.ShowSolenoidValve2(true);
                    ControlPanel.Instance.ShowPanel(false);
                    activationCylinderBoxPopup.Init();
                    activationCylinderBoxPopup.closeBtn.gameObject.SetActive(true);
                    activationCylinderBoxPopup.closeBtn.onClick.AddListener(delegate
                    {
                        activationCylinderBoxPopup.gameObject.SetActive(false);
                    });
                    activationCylinderBoxPopup.gameObject.SetActive(true);
                    var t = collisionSubject
                        // 조건 1: draggedObject의 태그가 "Player"인지 확인
                        .Where(collision => collision.draggedObject.name.Equals("InventoryItem(복구 솔레노이드밸브1)") && collision.targetObject.name.Equals("Body"))
                        .Subscribe(collision =>
                        {
                            collision.draggedObject.SetActive(false);
                            activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                            activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                            inventoryObj?.ShowSolenoidValve1(false);
                            inventoryObj?.ShowSolenoidValve2(false);
                            OpenActivationBox(true, true);
                            _gasSysEvaluation.솔레노이드결착 = true;

                        }).AddTo(this);
                    _disposable.Add(t);
                    //activationCylinderBoxPopup.attachBody.Invoke();

                    /*
                    inventoryObj?.ShowSolenoidValve1(false);
                    inventoryObj?.ShowSolenoidValve2(false);
                    inventoryObj?.ShowSafetyPin(true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    ControlPanel.Instance.ShowFire(false);
                    solenoidValvePopup.gameObject.SetActive(true);
                    solenoidValvePopup.clipObj.SetActive(true);
                    solenoidValvePopup.wallObj.SetActive(true);
                    solenoidValvePopup.mActivationObj.SetActive(true);
                    solenoidValvePopup.SolenoidValveAActivation(false);
                    uiDragAndCollisionHandler.ResetEvent();
                    uiDragAndCollisionHandler.StartDragging();
                    bool isDetected = false;
                    uiDragAndCollisionHandler.OnCollisionDetected += (_, _) =>
                    {
                        if (isDetected)
                            return;
                        isDetected = true;
                        if (!solenoidValvePopup.recoveryAni.isPlaying)
                        {
                            solenoidValvePopup.recoveryAni.clip = solenoidValvePopup.recoveryAni.GetClip("NewRecoveryAni");
                            solenoidValvePopup.recoveryAni.Play();
                        }
                        inventoryObj?.ShowSafetyPin(false);
                    };
                    SolenoidValvePopup.PinAttach?.RemoveAllListeners();
                    SolenoidValvePopup.PinAttach?.AddListener(delegate
                    {
                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                        _gasSysEvaluation.솔레노이드복구 = true;

                        //NextEnable();
                    });
                    */
                }
                break;
            case GasSysRecoveryCheckESection.E4:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    solenoidValvePopup.gameObject.SetActive(false);
                    activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                    activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                    activationCylinderBoxPopup.gameObject.SetActive(false);
                    inventoryObj?.ShowRecoverySolenoidValve1(false);
                    inventoryObj?.ShowSolenoidValve2(false);
                    inventoryObj?.ShowSafetyPin(false);
                    OpenActivationBox(true, true);
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    {
                        _gasSysEvaluation.주경종 = !switchBtn.주경종;
                        _gasSysEvaluation.지구경종 = !switchBtn.지구경종;
                        _gasSysEvaluation.사이렌 = !switchBtn.사이렌;
                        _gasSysEvaluation.비상방송 = !switchBtn.비상방송;
                        _gasSysEvaluation.부저 = !switchBtn.부저;
                        _gasSysEvaluation.축적 = !switchBtn.축적;
                        _gasSysEvaluation.솔레노이드연동 = ControlMode.Auto.Equals(switchBtn.솔레노이드밸브);
                    });
                    /*
                    SolenoidValvePopup.PinAttach?.RemoveAllListeners();
                    uiDragAndCollisionHandler.ResetEvent();
                    uiDragAndCollisionHandler.StartDragging();
                    solenoidValvePopup.gameObject.SetActive(true);
                    solenoidValvePopup.clipObj.SetActive(true);
                    solenoidValvePopup.wallObj.SetActive(true);
                    inventoryObj.ShowSafetyPin(false);
                    solenoidValvePopup.mActivationObj.SetActive(true);
                    solenoidValvePopup.recoveryPinObj.SetActive(true);
                    //ButtonManager.Instance.EnableSpecificImage(solenoidValvePopup.recoveryPinObj, solenoidValvePopup.aActivationObj);
                    bool isDetected = false;
                    uiDragAndCollisionHandler.OnCollisionDetected += (_, _) =>
                    {
                        if (isDetected)
                            return;
                        isDetected = true;
                        solenoidValvePopup.recoveryPinObj.SetActive(false);
                        solenoidValvePopup.aActivationPinObj.SetActive(true);
                        solenoidValvePopup.closeBtn.gameObject.SetActive(true);
                        solenoidValvePopup.closeBtn.onClick.RemoveAllListeners();
                        solenoidValvePopup.closeBtn.onClick.AddListener(delegate
                        {
                            inventoryObj?.ShowRecoverySolenoidValve1(true);
                            inventoryObj?.ShowSolenoidValve2(true);
                            solenoidValvePopup.gameObject.SetActive(false);
                        });
                    };
                    */
                }
                break;
            case GasSysRecoveryCheckESection.E5:
                {
                    ControlPanel.Instance.ShowPanel(false);
                    activationCylinderBoxPopup.Init();
                    activationCylinderBoxPopup.closeBtn.gameObject.SetActive(true);
                    activationCylinderBoxPopup.gameObject.SetActive(true);
                    activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                    activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                    activationCylinderBoxPopup.closeBtn.onClick.AddListener(delegate
                    {
                        activationCylinderBoxPopup.gameObject.SetActive(false);

                    });
                    uiDragAndCollisionHandler.ResetEvent();
                    uiDragAndCollisionHandler.StartDragging();
                    bool isDetected = false;
                    uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, _) =>
                    {
                        if (isDetected)
                            return;
                        isDetected = true;

                        uiDragAndCollisionHandler.StopDragging();
                        //activationCylinderBoxPopup.attachBody.Invoke();
                        draggedObject.SetActive(false);
                        activationCylinderBoxPopup.pinObj.SetActive(true);
                        activationCylinderBoxPopup.aActivationPinObj.SetActive(false);
                        _gasSysEvaluation.안전핀제거 = true;
                        activationCylinderBoxPopup.gameObject.SetActive(false);
                        OpenActivationBox(false);
                        Next();
                    };
                }
                break;

            case GasSysRecoveryCheckESection.E6:
                {
                    activationCylinderBoxPopup.gameObject.SetActive(false);
                    // {
                    //     SolenoidValvePopup.PinAttach?.RemoveAllListeners();
                    //     uiDragAndCollisionHandler.ResetEvent();
                    //     uiDragAndCollisionHandler.StartDragging();
                    //     solenoidValvePopup.gameObject.SetActive(false);
                    //     solenoidValvePopup.clipObj.SetActive(true);
                    //     solenoidValvePopup.wallObj.SetActive(true);
                    //     inventoryObj.ShowSafetyPin(false);
                    //     inventoryObj?.ShowRecoverySolenoidValve1(true);
                    //     inventoryObj?.ShowSolenoidValve2(true);
                    //     //solenoidValvePopup.mActivationObj.SetActive(true);
                    //     //solenoidValvePopup.recoveryPinObj.SetActive(true);
                    //     //ButtonManager.Instance.EnableSpecificImage(solenoidValvePopup.recoveryPinObj, solenoidValvePopup.aActivationObj);
                    //     bool isBody = false;
                    //     uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    //     {
                    //         if (targetObject.name.Equals("Body"))
                    //         {
                    //             if (isBody)
                    //                 return;
                    //             isBody = true;
                    //             uiDragAndCollisionHandler.StopDragging();
                    //             //activationCylinderBoxPopup.attachBody.Invoke();
                    //             draggedObject.SetActive(false);
                    //             activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                    //             activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                    //             inventoryObj?.ShowSolenoidValve1(false);
                    //             inventoryObj?.ShowSolenoidValve2(false);
                    //             OpenActivationBox(true, true);
                    //             _gasSysEvaluation.솔레노이드결착 = true;
                    //             //NextEnable();
                    //         }
                    //     };
                    // }
                }
                break;
            /*
            case GasSysRecoveryCheckESection.E7:
                {
                    solenoidValvePopup.gameObject.SetActive(false);
                    activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                    activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                    inventoryObj?.ShowRecoverySolenoidValve1(false);
                    inventoryObj?.ShowSolenoidValve2(false);
                    inventoryObj?.ShowSafetyPin(false);
                    OpenActivationBox(true, true);
                    // ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    // ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    // {
                    //     _gasSysEvaluation.주경종 = !switchBtn.주경종;
                    //     _gasSysEvaluation.지구경종 = !switchBtn.지구경종;
                    //     _gasSysEvaluation.사이렌 = !switchBtn.사이렌;
                    //     _gasSysEvaluation.비상방송 = !switchBtn.비상방송;
                    //     _gasSysEvaluation.부저 = !switchBtn.부저;
                    //     _gasSysEvaluation.축적 = !switchBtn.축적;
                    //     _gasSysEvaluation.솔레노이드연동 = ControlMode.Auto.Equals(switchBtn.솔레노이드밸브);
                    // });
                }
                break;
            case GasSysRecoveryCheckESection.E8:
                {
                    solenoidValvePopup.gameObject.SetActive(false);
                    activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                    activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                    inventoryObj?.ShowRecoverySolenoidValve1(false);
                    inventoryObj?.ShowSolenoidValve2(false);
                    inventoryObj?.ShowSafetyPin(false);
                    OpenActivationBox(true, true);
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    {
                        _gasSysEvaluation.주경종 = !switchBtn.주경종;
                        _gasSysEvaluation.지구경종 = !switchBtn.지구경종;
                        _gasSysEvaluation.사이렌 = !switchBtn.사이렌;
                        _gasSysEvaluation.비상방송 = !switchBtn.비상방송;
                        _gasSysEvaluation.부저 = !switchBtn.부저;
                        _gasSysEvaluation.축적 = !switchBtn.축적;
                        _gasSysEvaluation.솔레노이드연동 = ControlMode.Auto.Equals(switchBtn.솔레노이드밸브);
                    });
                }
                break;
            case GasSysRecoveryCheckESection.E9:
                {
                    uiDragAndCollisionHandler.ResetEvent();
                    activationBoxBtn.onClick.RemoveAllListeners();
                    activationBoxBtn.onClick.AddListener(delegate
                    {
                        activationCylinderBoxPopup.Init();
                        activationCylinderBoxPopup.closeBtn.gameObject.SetActive(true);
                        activationCylinderBoxPopup.gameObject.SetActive(true);
                        activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                        activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                        activationCylinderBoxPopup.closeBtn.onClick.AddListener(delegate
                        {
                            activationCylinderBoxPopup.gameObject.SetActive(false);
                            OpenActivationBox(false);
                        });
                    });

                    uiDragAndCollisionHandler.ResetEvent();
                    uiDragAndCollisionHandler.StartDragging();
                    bool isDetected = false;
                    uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, _) =>
                    {
                        if (isDetected)
                            return;
                        isDetected = true;

                        uiDragAndCollisionHandler.StopDragging();
                        //activationCylinderBoxPopup.attachBody.Invoke();
                        draggedObject.SetActive(false);
                        activationCylinderBoxPopup.pinObj.SetActive(true);
                        activationCylinderBoxPopup.aActivationPinObj.SetActive(false);
                        _gasSysEvaluation.안전핀제거 = true;
                    };
                }

                break;
            case GasSysRecoveryCheckESection.E10:
                {
                    activationBoxBtn.onClick.RemoveAllListeners();
                    activationBoxBtn.onClick.AddListener(delegate
                    {
                        activationCylinderBoxPopup.Init();
                        activationCylinderBoxPopup.closeBtn.gameObject.SetActive(true);
                        activationCylinderBoxPopup.gameObject.SetActive(true);
                        activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                        activationCylinderBoxPopup.pinObj.SetActive(true);
                        activationCylinderBoxPopup.aActivationPinObj.SetActive(false);
                        activationCylinderBoxPopup.closeBtn.onClick.AddListener(delegate
                        {
                            activationCylinderBoxPopup.gameObject.SetActive(false);
                            //OpenActivationBox(false);
                        });
                    });
                    OpenActivationBox(false);
                }
                break;
            */
            case GasSysRecoveryCheckESection.평가종료:
                {
                    _gasSysEvaluation.기동밸브 = GetValveState();
                    Debug.Log(_gasSysEvaluation.기동밸브);
                    _gasSysEvaluation.저장밸브 = GetStorageState();
                    Debug.Log(_gasSysEvaluation.저장밸브);
                    ChangeValveState(true);
                    ChangeStorageValveState(true);
                    selectionValvePopup.SetOnOff(true);
                    storageCylinderPopup.SetOnOff(true);

                    GasSysGlobalCanvas.Instance.totalScore.점검완료후복구.화재복구 = _gasSysEvaluation.복구;
                    GasSysGlobalCanvas.Instance.totalScore.점검완료후복구.솔복구 = _gasSysEvaluation.솔레노이드복구;
                    GasSysGlobalCanvas.Instance.totalScore.점검완료후복구.솔결합 = _gasSysEvaluation.솔레노이드결착;
                    GasSysGlobalCanvas.Instance.totalScore.점검완료후복구.제어반복구 = _gasSysEvaluation.주경종 && _gasSysEvaluation.지구경종 && _gasSysEvaluation.사이렌 &&
                                                                           _gasSysEvaluation.비상방송 && _gasSysEvaluation.부저 && _gasSysEvaluation.축적 && _gasSysEvaluation.솔레노이드연동;
                    GasSysGlobalCanvas.Instance.totalScore.점검완료후복구.솔안전핀분리 = _gasSysEvaluation.안전핀제거;
                    GasSysGlobalCanvas.Instance.totalScore.점검완료후복구.조작동관복구 = _gasSysEvaluation.기동밸브 && _gasSysEvaluation.저장밸브;
                    var results = new List<ResultObject>
                    {
                        new ResultObject()
                        {
                            Title = "감시제어반 화재복구",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검완료후복구.화재복구
                        },
                        new ResultObject()
                        {
                            Title = "솔레노이드밸브 복구",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검완료후복구.솔복구
                        },
                        new ResultObject()
                        {
                            Title = "솔레노이드밸브 기동용기 결합",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검완료후복구.솔결합
                        },
                        new ResultObject()
                        {
                            Title = "감시제어반 복구",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검완료후복구.제어반복구
                        },
                        new ResultObject()
                        {
                            Title = "솔레노이드밸브 안전핀 분리",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검완료후복구.솔안전핀분리
                        },
                        new ResultObject()
                        {
                            Title = "조작동관 복구",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.점검완료후복구.조작동관복구
                        }
                    };

                    //GasSysGlobalCanvas.Instance.SetResultPopup(results);
                    GasSysGlobalCanvas.Instance.totalScore.점검완료후복구List.Clear();
                    GasSysGlobalCanvas.Instance.totalScore.점검완료후복구List.AddRange(results);
                    //GasSysGlobalCanvas.Instance.SetNextEvaluation(InitDischargeLightTest);
                    //GasSysGlobalCanvas.Instance.InitTotalResult();
                    GasSysGlobalCanvas.Instance.SetNextEvaluation(GasSysGlobalCanvas.Instance.InitTotalResult, InitRecoveryCheck, true);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }


    private void ChangeStateRecoveryCheckP(GasSysRecoveryCheckPSection state)
    {

        _curRecoveryCheckPSection = state;
        OnStateChangedRecoveryP(_curRecoveryCheckPSection);
    }




    private void OnStateChangedRecoveryP(GasSysRecoveryCheckPSection state)
    {
        InitStorageRoom();
        uiDragAndCollisionHandler.StopDragging();
        ButtonManager.Instance.RemoveAllHighlights();
        ChangeValveState(false);
        ChangeStorageValveState(false);
        inventoryObj.ShowSafetyPin(true);
        ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
        //nextBtn.onClick.RemoveAllListeners();
        switch (state)
        {
            case GasSysRecoveryCheckPSection.감시제어반선택:
                {
                    controlBoxBtn.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificButton(controlBoxBtn);
                    inventoryObj?.ShowRecoverySolenoidValve1(true);
                    inventoryObj?.ShowSolenoidValve2(true);
                    NextDisable();
                    controlBoxBtn.onClick.RemoveAllListeners();
                    controlBoxBtn.onClick.AddListener(delegate
                    {
                        controlBoxBtn.gameObject.SetActive(false);
                        //ControlPanel.Instance.ShowPanel(true);
                        Next();
                    });
                }
                break;
            case GasSysRecoveryCheckPSection.복구전:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.ShowFire(true);
                    inventoryObj?.ShowRecoverySolenoidValve1(true);
                    inventoryObj?.ShowSolenoidValve2(true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                    //ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);

                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("복구").OnCheck(true);
                    ControlPanel.Instance.SoundCheck();
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    var temp = new ControlPanelButtonCheck(ControlMode.Stop);
                    temp.SetBtn(false);
                    //temp.축적 = false;
                    temp.복구 = true;
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(true);
                    temp.복구 = false;
                    //temp.솔레노이드밸브 = ControlMode.Auto;
                    SetHighlightControlPanelCheck(temp);
                   
                }
                break;
           case GasSysRecoveryCheckPSection.트리거복구:
                {
                    //inventoryObj?.ShowSolenoidValve1(false);
                    inventoryObj?.ShowRecoverySolenoidValve1(false);
                    inventoryObj?.ShowSolenoidValve2(true);
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    ControlPanel.Instance.ShowFire(false);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("복구").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
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
                    //     select = true
                    // });
                    //
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetArea2SelectBtn(),
                    //     select = true
                    // });
                    //
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
                    //     select = true
                    // });
                    // ButtonManager.Instance.EnableSpecificButton(btnList);
                    // NextDisable();
                    // ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    // {
                    //     btnList.Clear();
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
                    //         select = switchBtn.주경종
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
                    //         select = switchBtn.지구경종
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
                    //         select = switchBtn.사이렌
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
                    //         select = switchBtn.비상방송
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
                    //         select = switchBtn.부저
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
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
                    //         select = isRecvery
                    //     });
                    //ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    // var temp = new ControlPanelButtonCheck(ControlMode.Stop);
                    // temp.SetBtn(false);
                    // //temp.축적 = false;
                    // //temp.복구 = true;
                    // SetHighlightControlPanel(temp);
                    // NextDisable();
                    // temp.SetBtn(true);
                    // //temp.복구 = true;
                    // //temp.솔레노이드밸브 = ControlMode.Manual;
                    // SetHighlightControlPanelCheck(temp, false);
                    //     ControlPanel.Instance.RecoverySwitchEvent?.RemoveAllListeners();
                    //     ControlPanel.Instance.RecoverySwitchEvent?.AddListener(delegate
                    //     {
                    //         ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    //         ControlPanel.Instance.ShowFire(false);
                    //         isRecvery = true;
                    //     });
                    //     ButtonManager.Instance.EnableSpecificButton(btnList);
                    //
                    //     if (switchBtn.주경종 && switchBtn.지구경종 && switchBtn.사이렌 && switchBtn.비상방송 && switchBtn.부저 && switchBtn.축적 &&
                    //         isRecvery && switchBtn.구역1.Equals(ControlMode.Auto) && switchBtn.솔레노이드밸브.Equals(ControlMode.Manual) && switchBtn.구역2.Equals(ControlMode.Auto))
                    //     {
                    //         NextEnable();
                    //         isTriggerRecovery = true;
                    //     }
                    //     else
                    //     {
                    //         NextDisable();
                    //     }
                    // });
                    NextDisable();
                    ButtonManager.Instance.EnableSpecificImage(inventoryObj?.safetyPin, solenoidValvePopup.triggerOnObj);

                    solenoidValvePopup.gameObject.SetActive(true);
                    solenoidValvePopup.clipObj.SetActive(true);
                    solenoidValvePopup.wallObj.SetActive(true);
                    solenoidValvePopup.mActivationObj.SetActive(true);
                    solenoidValvePopup.SolenoidValveAActivation(false);
                    uiDragAndCollisionHandler.ResetEvent();
                    uiDragAndCollisionHandler.StartDragging();
                    uiDragAndCollisionHandler.OnCollisionDetected += (_, _) =>
                    {
                        if (!solenoidValvePopup.recoveryAni.isPlaying)
                        {
                            solenoidValvePopup.recoveryAni.clip = solenoidValvePopup.recoveryAni.GetClip("NewRecoveryAni");
                            solenoidValvePopup.recoveryAni.Play();
                        }
                        inventoryObj?.ShowSafetyPin(false);
                    };
                    SolenoidValvePopup.PinAttach?.RemoveAllListeners();
                    SolenoidValvePopup.PinAttach?.AddListener(delegate
                    {

                        uiDragAndCollisionHandler.StopDragging();
                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                        NextEnable();
                    });
                }
                break;
            case GasSysRecoveryCheckPSection.안전핀결착:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    inventoryObj?.ShowSafetyPin(false);
                    //inventoryObj?.ShowRecoverySolenoidValve1(true);
                    inventoryObj?.ShowSolenoidValve2(true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    ControlPanel.Instance.ShowFire(false);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("복구").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    // var temp = new ControlPanelButtonCheck();
                    // temp.SetBtn(false);
                    // //temp.축적 = false;
                    // //temp.복구 = true;
                    // SetHighlightControlPanel(temp);
                    // NextDisable();
                    // temp.SetBtn(true);
                    // //temp.복구 = false;
                    // temp.솔레노이드밸브 = ControlMode.Auto;
                    // SetHighlightControlPanelCheck(temp);
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

                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
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
                            select = switchBtn.솔레노이드밸브.Equals(ControlMode.Manual)
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
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
                            select = isRecvery
                        });
                        ControlPanel.Instance.RecoverySwitchEvent?.RemoveAllListeners();
                        ControlPanel.Instance.RecoverySwitchEvent?.AddListener(delegate
                        {
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                            ControlPanel.Instance.ShowFire(false);
                            isRecvery = true;
                        });
                        ButtonManager.Instance.EnableSpecificButton(btnList);

                        if (switchBtn.주경종 && switchBtn.지구경종 && switchBtn.사이렌 && switchBtn.비상방송 && switchBtn.부저 && switchBtn.축적 &&
                            isRecvery && switchBtn.구역1.Equals(ControlMode.Auto) && switchBtn.솔레노이드밸브.Equals(ControlMode.Manual) && switchBtn.구역2.Equals(ControlMode.Auto))
                        {
                            //NextEnable();
                            isTriggerRecovery = true;
                        }
                        else
                        {
                            NextDisable();
                        }
                    });
                    */
                    NextDisable();
                    uiDragAndCollisionHandler.ResetEvent();
                    uiDragAndCollisionHandler.StartDragging();
                    solenoidValvePopup.gameObject.SetActive(true);
                    solenoidValvePopup.clipObj.SetActive(true);
                    solenoidValvePopup.wallObj.SetActive(true);
                    solenoidValvePopup.mActivationObj.SetActive(true);
                    solenoidValvePopup.recoveryPinObj.SetActive(true);
                    ButtonManager.Instance.EnableSpecificImage(solenoidValvePopup.recoveryPinObj, solenoidValvePopup.aActivationObj);
                    uiDragAndCollisionHandler.OnCollisionDetected += (_, _) =>
                    {
                        solenoidValvePopup.recoveryPinObj.SetActive(false);
                        solenoidValvePopup.aActivationPinObj.SetActive(true);
                        NextEnable();
                    };
                }
                break;
            case GasSysRecoveryCheckPSection.솔레노이드자동:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    inventoryObj?.ShowSafetyPin(false);
                    inventoryObj?.ShowRecoverySolenoidValve1(true);
                    inventoryObj?.ShowSolenoidValve2(true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    ControlPanel.Instance.ShowFire(false);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("복구").OnCheck(true);
                    ControlPanel.Instance.CheckSwitchBtn();
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    //temp.축적 = false;
                    //temp.복구 = true;
                    SetHighlightControlPanel(temp);
                    NextDisable();
                    temp.SetBtn(true);
                    //temp.복구 = false;
                    temp.솔레노이드밸브 = ControlMode.Auto;
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
                    //
                    // btnList.Add(new SwitchButtonCheck()
                    // {
                    //     btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
                    //     select = true
                    // });
                    // ButtonManager.Instance.EnableSpecificButton(btnList);
                    // NextDisable();
                    // ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    // {
                    //     btnList.Clear();
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
                    //         select = switchBtn.주경종
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
                    //         select = switchBtn.지구경종
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
                    //         select = switchBtn.사이렌
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
                    //         select = switchBtn.비상방송
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
                    //         select = switchBtn.부저
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("축적/비축적").GetButton(),
                    //         select = switchBtn.축적
                    //     });
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSolenoidSelectBtn(),
                    //         select = switchBtn.솔레노이드밸브.Equals(ControlMode.Auto)
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
                    //     btnList.Add(new SwitchButtonCheck()
                    //     {
                    //         btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
                    //         select = isRecvery
                    //     });
                    //     ControlPanel.Instance.RecoverySwitchEvent?.RemoveAllListeners();
                    //     ControlPanel.Instance.RecoverySwitchEvent?.AddListener(delegate
                    //     {
                    //         ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    //         ControlPanel.Instance.ShowFire(false);
                    //         isRecvery = true;
                    //     });
                    //     ButtonManager.Instance.EnableSpecificButton(btnList);
                    //
                    //     if (switchBtn.주경종 && switchBtn.지구경종 && switchBtn.사이렌 && switchBtn.비상방송 && switchBtn.부저 && switchBtn.축적 &&
                    //         isRecvery && switchBtn.구역1.Equals(ControlMode.Auto) && switchBtn.솔레노이드밸브.Equals(ControlMode.Auto) && switchBtn.구역2.Equals(ControlMode.Auto))
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
            case GasSysRecoveryCheckPSection.감시제어반복구:
                {
                    bool isRecovery = true;
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.ShowPanel(true);
                    inventoryObj?.ShowSafetyPin(false);
                    inventoryObj?.ShowRecoverySolenoidValve1(true);
                    inventoryObj?.ShowSolenoidValve2(true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    ControlPanel.Instance.ShowFire(false);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.GetSwitchButton("복구").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl();
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

                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
                        select = true
                    });
                    ButtonManager.Instance.EnableSpecificButton(btnList);
                    NextDisable();
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
                    {
                        btnList.Clear();
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
                            select = !switchBtn.주경종
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
                            select = !switchBtn.지구경종
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
                            select = !switchBtn.사이렌
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
                            select = !switchBtn.비상방송
                        });
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
                            select = !switchBtn.부저
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
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
                            select = true
                        });
                        ControlPanel.Instance.RecoverySwitchEvent?.RemoveAllListeners();
                        ControlPanel.Instance.RecoverySwitchEvent?.AddListener(delegate
                        {
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                            ControlPanel.Instance.ShowFire(false);
                            isRecovery = true;
                        });
                        ButtonManager.Instance.EnableSpecificButton(btnList);

                        if (!switchBtn.주경종 && !switchBtn.지구경종 && !switchBtn.사이렌 && !switchBtn.비상방송 && !switchBtn.부저 && !switchBtn.축적 &&
                            isRecovery && switchBtn.구역1.Equals(ControlMode.Auto) && switchBtn.솔레노이드밸브.Equals(ControlMode.Auto) && switchBtn.구역2.Equals(ControlMode.Auto))
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
            case GasSysRecoveryCheckPSection.솔레노이드밸브복구:
                {
                    bool isRecovery = true;
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.ShowPanel(true);
                    inventoryObj?.ShowSafetyPin(false);
                    inventoryObj?.ShowRecoverySolenoidValve1(true);
                    inventoryObj?.ShowSolenoidValve2(true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    ControlPanel.Instance.ShowFire(false);
                    /*
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("복구").OnCheck(false);
                    ControlPanel.Instance.InitSolenoidValveControl();
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

                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
                        select = true
                    });
                    ButtonManager.Instance.EnableSpecificButton(btnList);
                    ButtonManager.Instance.EnableSpecificImage(inventoryObj?.recoverySolenoidValve1);
                    NextDisable();
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
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
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
                            select = isRecovery
                        });
                        ControlPanel.Instance.RecoverySwitchEvent?.RemoveAllListeners();
                        ControlPanel.Instance.RecoverySwitchEvent?.AddListener(delegate
                        {
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                            ControlPanel.Instance.ShowFire(false);
                            isRecovery = true;
                        });
                        ButtonManager.Instance.EnableSpecificButton(btnList);

                        if (switchBtn.주경종 && switchBtn.지구경종 && switchBtn.사이렌 && switchBtn.비상방송 && switchBtn.부저 && switchBtn.축적 &&
                            isRecovery && switchBtn.구역1.Equals(ControlMode.Auto) && switchBtn.솔레노이드밸브.Equals(ControlMode.Auto) && switchBtn.구역2.Equals(ControlMode.Auto))
                        {
                            NextEnable();
                        }
                        else
                        {
                            NextDisable();
                        }
                    });
                    */
                    uiDragAndCollisionHandler.ResetEvent();
                    uiDragAndCollisionHandler.StartDragging();
                    activationCylinderBoxPopup.Init();
                    activationCylinderBoxPopup.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificImage(inventoryObj?.recoverySolenoidValve1, activationCylinderBoxPopup.bodyObj);
                    uiDragAndCollisionHandler.ResetEvent();
                    uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (!targetObject.name.Equals("Body"))
                            return;
                        uiDragAndCollisionHandler.StopDragging();
                        //activationCylinderBoxPopup.attachBody.Invoke();
                        draggedObject.SetActive(false);
                        activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                        activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                        inventoryObj?.ShowRecoverySolenoidValve1(false);
                        inventoryObj?.ShowSolenoidValve2(false);
                        NextEnable();
                    };
                }
                break;
            case GasSysRecoveryCheckPSection.안전핀복구:
                {
                    bool isRecovery = true;
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.ShowPanel(true);
                    inventoryObj?.ShowSafetyPin(false);
                    inventoryObj?.ShowRecoverySolenoidValve1(false);
                    inventoryObj?.ShowSolenoidValve2(false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    ControlPanel.Instance.ShowFire(false);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("복구").OnCheck(false);
                    ControlPanel.Instance.InitSolenoidValveControl();
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

                    btnList.Add(new SwitchButtonCheck()
                    {
                        btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
                        select = true
                    });
                    ButtonManager.Instance.EnableSpecificButton(btnList);
                    NextDisable();
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
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
                        btnList.Add(new SwitchButtonCheck()
                        {
                            btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
                            select = isRecovery
                        });
                        ControlPanel.Instance.RecoverySwitchEvent?.RemoveAllListeners();
                        ControlPanel.Instance.RecoverySwitchEvent?.AddListener(delegate
                        {
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                            ControlPanel.Instance.ShowFire(false);
                            isRecovery = true;
                        });
                        ButtonManager.Instance.EnableSpecificButton(btnList);

                        if (switchBtn.주경종 && switchBtn.지구경종 && switchBtn.사이렌 && switchBtn.비상방송 && switchBtn.부저 && switchBtn.축적 &&
                            isRecovery && switchBtn.구역1.Equals(ControlMode.Auto) && switchBtn.솔레노이드밸브.Equals(ControlMode.Auto) && switchBtn.구역2.Equals(ControlMode.Auto))
                        {
                            NextEnable();
                        }
                        else
                        {
                            NextDisable();
                        }
                    });
                    */
                    uiDragAndCollisionHandler.ResetEvent();
                    uiDragAndCollisionHandler.StartDragging();
                    activationCylinderBoxPopup.Init();
                    activationCylinderBoxPopup.gameObject.SetActive(true);
                    activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                    activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                    ButtonManager.Instance.EnableSpecificImage(activationCylinderBoxPopup.recoveryPinObj, activationCylinderBoxPopup.aActivationPinObj);
                    bool isDetected = false;
                    uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, _) =>
                    {
                        if (isDetected)
                            return;
                        isDetected = true;

                        uiDragAndCollisionHandler.StopDragging();
                        //activationCylinderBoxPopup.attachBody.Invoke();
                        draggedObject.SetActive(false);
                        activationCylinderBoxPopup.pinObj.SetActive(true);
                        OpenActivationBox(false);
                        activationCylinderBoxPopup.aActivationPinObj.SetActive(false);
                        NextEnable();
                    };
                }
                break;

            case GasSysRecoveryCheckPSection.저장용기조작동관선택:
                {
                    storageCylinderBtn.gameObject.SetActive(true);
                    inventoryObj?.ShowSafetyPin(false);
                    inventoryObj?.ShowRecoverySolenoidValve1(false);
                    inventoryObj?.ShowSolenoidValve2(false);
                    ChangeValveState(false);
                    ChangeStorageValveState(false);
                    ButtonManager.Instance.EnableSpecificButton(storageCylinderBtn);
                    NextDisable();
                    storageCylinderBtn.onClick.AddListener(delegate
                    {
                        storageCylinderBtn.gameObject.SetActive(false);
                        Next();
                    });
                }
                break;
            case GasSysRecoveryCheckPSection.저장용기조작동관연결전:
                {
                    ButtonManager.Instance.EnableSpecificButton(storageCylinderPopup.GetButtons());
                    NextDisable();
                    inventoryObj?.ShowSafetyPin(false);
                    inventoryObj?.ShowRecoverySolenoidValve1(false);
                    inventoryObj?.ShowSolenoidValve2(false);
                    ChangeValveState(false);
                    ChangeStorageValveState(false);
                    storageCylinderPopup.InitPopup(false, isOn =>
                    {
                        storageCylinder.ChangeValveState(isOn);
                        if (isOn)
                        {
                            Next();
                        }
                    });
                }
                break;
            case GasSysRecoveryCheckPSection.저장용기조작동관연결:
                {
                    storageCylinderPopup.GetButtons()[0].gameObject.SetActive(false);
                    storageCylinderPopup.Init();
                    inventoryObj?.ShowSafetyPin(false);
                    inventoryObj?.ShowRecoverySolenoidValve1(false);
                    inventoryObj?.ShowSolenoidValve2(false);
                    storageCylinderPopup.gameObject.SetActive(true);
                    storageCylinderPopup.SetOnOff(true);
                    ChangeValveState(false);
                    ChangeStorageValveState(true);
                    storageCylinder.ChangeValveState(true);
                    NextEnable();
                }
                break;
            case GasSysRecoveryCheckPSection.선택밸브조작동관선택:
                {
                    selectionValveBtn.gameObject.SetActive(true);
                    ChangeValveState(false);
                    inventoryObj?.ShowSafetyPin(false);
                    inventoryObj?.ShowRecoverySolenoidValve1(false);
                    inventoryObj?.ShowSolenoidValve2(false);
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
            case GasSysRecoveryCheckPSection.선택밸브조작동관연결전:
                {
                    ButtonManager.Instance.EnableSpecificButton(selectionValvePopup.GetButtons());
                    ChangeValveState(false);
                    inventoryObj?.ShowSafetyPin(false);
                    inventoryObj?.ShowRecoverySolenoidValve1(false);
                    inventoryObj?.ShowSolenoidValve2(false);
                    ChangeStorageValveState(true);
                    NextDisable();
                    selectionValvePopup.InitPopup(false, isOn =>
                    {
                        selectionValve01.ChangeValveState(isOn);
                        if (isOn)
                        {
                            Next();
                        }
                    });
                }
                break;
            case GasSysRecoveryCheckPSection.선택밸브조작동관연결:
                {
                    selectionValvePopup.GetButtons()[0].gameObject.SetActive(false);
                    selectionValvePopup.Init();
                    inventoryObj?.ShowSafetyPin(false);
                    inventoryObj?.ShowRecoverySolenoidValve1(false);
                    inventoryObj?.ShowSolenoidValve2(false);
                    selectionValvePopup.gameObject.SetActive(true);
                    selectionValvePopup.SetOnOff(true);
                    ChangeValveState(true);
                    ChangeStorageValveState(true);
                    NextEnable();
                }
                break;
            case GasSysRecoveryCheckPSection.교육종료:
                {
                    NextDisable();
                    inventoryObj?.ShowSafetyPin(false);
                    inventoryObj?.ShowRecoverySolenoidValve1(false);
                    inventoryObj?.ShowSolenoidValve2(false);
                    SetCompletePopup("점검 후 복구를 모두 완료했습니다.", "하단의 버튼을 통해 다른 페이지로 이동해주세요.");
                }
                break;


            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }



}
