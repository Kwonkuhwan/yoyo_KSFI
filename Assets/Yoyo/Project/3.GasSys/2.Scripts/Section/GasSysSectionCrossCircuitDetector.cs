using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public partial class GasSysSection
{
    [Foldout("교차회로 감지기")]
    [FormerlySerializedAs("crossCircuitDetectorHintObj")]
    public HintScriptableObj crossCircuitDetectorHintPObj;
    public HintScriptableObj crossCircuitDetectorHintEObj;
    private GasSysCrossCircuitDetectorESection _crossCircuitDetectorE;
    private GasSysCrossCircuitDetectorPSection _crossCircuitDetectorP;
    public GameObject 교차회로감지기평가1;
    [SerializeField] public Toggle 교차회로감지기토글1;
    [SerializeField] public Toggle 교차회로감지기토글2;
    [SerializeField] public Toggle 교차회로감지기토글3;
    [SerializeField] public Toggle 교차회로감지기토글4;
    [SerializeField] public Toggle 교차회로감지기토글5;
    [SerializeField] public Toggle 교차회로감지기토글6;
    [EndFoldout]
    public void InitCrossCircuitDetector()
    {
        InitEMode();
        curMainSection = GasSysMainSection.SolenoidValveTest;
        curSolenoidValveTestSection = GasSysSolenoidValveTestSection.CrossCircuitDetector;
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
            GasSysState.PracticeMode => System.Enum.GetValues(typeof(GasSysCrossCircuitDetectorPSection)).Length,
            GasSysState.EvaluationMode => System.Enum.GetValues(typeof(GasSysCrossCircuitDetectorESection)).Length,
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

    private void ShowCrossCircuitDetectorSection(int index)
    {
        if (_gasSysState.Equals(GasSysState.PracticeMode))
        {
            prevBtn.gameObject.SetActive(true);
            GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(crossCircuitDetectorHintPObj, (int)GasSysCrossCircuitDetectorPSection.감시제어반선택 + index));
            ChangeStateCrossCircuitDetectorP(GasSysCrossCircuitDetectorPSection.감시제어반선택 + index);
        }
        if (!_gasSysState.Equals(GasSysState.EvaluationMode))
            return;
        prevBtn.gameObject.SetActive(false);
        GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(crossCircuitDetectorHintEObj, (int)GasSysCrossCircuitDetectorESection.Init + index));
        ChangeStateCrossCircuitDetectorE(GasSysCrossCircuitDetectorESection.Init + index);
    }

    private void ChangeStateCrossCircuitDetectorE(GasSysCrossCircuitDetectorESection state)
    {
        _crossCircuitDetectorE = state;
        OnStateChangedCrossCircuitDetectorE(_crossCircuitDetectorE);
    }

    private void OnStateChangedCrossCircuitDetectorE(GasSysCrossCircuitDetectorESection state)
    {
        uiDragAndCollisionHandler.StartDragging();
        switch (state)
        {
            case GasSysCrossCircuitDetectorESection.Init:
                {
                    InitStorageRoom();
                    InitArea1();
                    NextEnable(false);
                    _gasSysEvaluation.Reset();

                    controlBoxBtn.gameObject.SetActive(true);
                    selectionValveBtn.gameObject.SetActive(true);
                    storageCylinderBtn.gameObject.SetActive(true);
                    activationBoxBtn.gameObject.SetActive(true);
                    manualControlBoxBtn.gameObject.SetActive(true);

                    inventoryObj.ShowSmokeDetect(true);
                    inventoryObj.ShowHeatDetect(true);

                    smokeDetectorOn.SetActive(false);
                    heatDetectorOn.SetActive(false);

                    smokeDetectorPopup.gameObject.SetActive(false);
                    heatDetectorPopup.gameObject.SetActive(false);

                    areaManagerObj.area1Btn.onClick.AddListener(delegate
                    {
                        inventoryObj.ShowSmokeDetect(true);
                        inventoryObj.ShowHeatDetect(true);
                        ControlPanel.Instance.SetArea1PopupParent();
                        ShowRoom(area1Obj?.gameObject);
                        bool isHeatDetected = false;
                        bool isSmokeDetected = false;
                        uiDragAndCollisionHandler.ResetEvent();
                        uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                        {
                            inventoryObj.ShowHeatDetect(!CheckDetector2());
                            inventoryObj.ShowSmokeDetect(!CheckDetector1());
                            if (heatDetectorObj.Equals(targetObject))
                            {
                                if (!isHeatDetected)
                                {
                                    isHeatDetected = true;
                                    ControlPanel.Instance.SetArea1PopupParent();
                                    ControlPanel.Instance.ShowPanel(true);

                                    //ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                                    ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                                    SoundManager.Instance.MuteSiren(true);
                                    smokeDetectorOn.SetActive(CheckDetector1());
                                    heatDetectorPopup.InitCrossCircuitDetector(isOn =>
                                    {
                                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, isOn);
                                        inventoryObj.ShowHeatDetect(!CheckDetector2());
                                        inventoryObj.ShowSmokeDetect(!CheckDetector1());
                                        //heatDetectorPopup.gameObject.SetActive(false);
                                        heatDetectorOn.SetActive(true);
                                        SoundManager.Instance.SetSirenVolume(0f);
                                        ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                                        /*
                                        if (CheckDetector1() && CheckDetector2())
                                        {
                                            solenoidValvePopup.SetArea1PopupParent();
                                            solenoidValvePopup.clipObj.SetActive(true);
                                            solenoidValvePopup.DefaultActivation();
                                            solenoidValvePopup.gameObject.SetActive(true);
                                            GasSysSoundManager.Instance.SetSirenVolume();
                                            ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                                            ControlPanel.Instance.SetTimeNum(30f);
                                            ControlPanel.Instance.StartTimer(30);
                                            ControlPanel.OnTimerEnd.RemoveAllListeners();
                                            ControlPanel.OnTimerEnd.AddListener(delegate
                                            {
                                                //storageRoomObj?.ManualActivateToSolenoidValve();
                                                solenoidValvePopup.SolenoidValveAActivation();
                                                solenoidValvePopup.closeBtn.gameObject.SetActive(true);
                                                ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                                                // Observable.Timer(System.TimeSpan.FromSeconds(3))
                                                //     .Subscribe(_ =>
                                                //     {
                                                //         NextEnable();
                                                //     })
                                                //     .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
                                                //NextEnable();
                                            });
                                        }
                                        */

                                    });
                                    heatDetectorPopup.gameObject.SetActive(true);
                                    heatDetectorPopup.closeBtn.gameObject.SetActive(true);
                                }

                            }
                            if (smokeDetectorObj.Equals(targetObject))
                            {
                                if (!isSmokeDetected)
                                {
                                    isSmokeDetected = true;
                                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                                    ControlPanel.Instance.CheckSwitchBtn();
                                    ControlPanel.Instance.SetArea1PopupParent();
                                    ControlPanel.Instance.ShowPanel(true);
                                    SoundManager.Instance.SetSirenVolume();
                                    inventoryObj.ShowSmokeDetect(false);
                                    smokeDetectorPopup.ResetSmokeDetectorTime();
                                    smokeDetectorPopup.InitCrossCircuitDetector(isOn =>
                                    {
                                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, isOn);
                                        inventoryObj.ShowHeatDetect(!CheckDetector2());
                                        inventoryObj.ShowSmokeDetect(!CheckDetector1());
                                        ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                                        SoundManager.Instance.SetSirenVolume(0f);
                                        smokeDetectorOn.SetActive(true);

                                        /*
                                        if (CheckDetector1() && CheckDetector2())
                                        {
                                            solenoidValvePopup.SetArea1PopupParent();
                                            solenoidValvePopup.clipObj.SetActive(true);
                                            solenoidValvePopup.DefaultActivation();
                                            solenoidValvePopup.gameObject.SetActive(true);
                                            GasSysSoundManager.Instance.SetSirenVolume();
                                            ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                                            ControlPanel.Instance.SetTimeNum(30f);
                                            ControlPanel.Instance.StartTimer(30);
                                            ControlPanel.OnTimerEnd.RemoveAllListeners();
                                            ControlPanel.OnTimerEnd.AddListener(delegate
                                            {
                                                //storageRoomObj?.ManualActivateToSolenoidValve();
                                                solenoidValvePopup.SolenoidValveAActivation();
                                                solenoidValvePopup.closeBtn.gameObject.SetActive(true);
                                                ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                                                // Observable.Timer(System.TimeSpan.FromSeconds(3))
                                                //     .Subscribe(_ =>
                                                //     {
                                                //         NextEnable();
                                                //     })
                                                //     .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
                                                //NextEnable();
                                            });
                                        }
                                        */
                                        //SoundManager.Instance.MuteSiren(true);


                                    }, delegate
                                    {

                                    });
                                    smokeDetectorPopup.gameObject.SetActive(true);
                                    smokeDetectorPopup.closeBtn.gameObject.SetActive(true);
                                }
                            }
                        };
                    });

                    areaManagerObj.storageRoomBtn.onClick.AddListener(delegate
                    {
                        ControlPanel.Instance.SetStorageRoomPopupParent();
                        ShowRoom(storageRoomObj?.gameObject);
                    });

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
            case GasSysCrossCircuitDetectorESection.E1: //감시 제어반 설정
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
                    /*
                    areaManagerObj.area1Btn.onClick.RemoveAllListeners();
                    if (areaManagerObj.area1EnableObj.activeSelf)
                    {
                        inventoryObj.ShowSmokeDetect(true);
                        inventoryObj.ShowHeatDetect(true);
                        ControlPanel.Instance.SetArea1PopupParent();
                        ShowRoom(area1Obj?.gameObject);
                        bool isHeatDetected = false;
                        bool isSmokeDetected = false;
                        uiDragAndCollisionHandler.ResetEvent();
                        uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                        {
                            inventoryObj.ShowHeatDetect(!CheckDetector2());
                            inventoryObj.ShowSmokeDetect(!CheckDetector1());
                            if (heatDetectorObj.Equals(targetObject))
                            {
                                if (!isHeatDetected)
                                {
                                    isHeatDetected = true;
                                    ControlPanel.Instance.SetArea1PopupParent();
                                    //ControlPanel.Instance.ShowPanel(true);

                                    //ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                                    ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                                    SoundManager.Instance.MuteSiren(true);
                                    smokeDetectorOn.SetActive(CheckDetector1());
                                    heatDetectorPopup.InitCrossCircuitDetector(isOn =>
                                    {
                                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, isOn);
                                        inventoryObj.ShowHeatDetect(!CheckDetector2());
                                        inventoryObj.ShowSmokeDetect(!CheckDetector1());
                                        //heatDetectorPopup.gameObject.SetActive(false);
                                        heatDetectorOn.SetActive(true);
                                        SoundManager.Instance.SetSirenVolume(0f);
                                        ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());

                                    });
                                    heatDetectorPopup.gameObject.SetActive(true);
                                    heatDetectorPopup.closeBtn.gameObject.SetActive(true);
                                }

                            }
                            if (smokeDetectorObj.Equals(targetObject))
                            {
                                if (!isSmokeDetected)
                                {
                                    isSmokeDetected = true;

                                    ControlPanel.Instance.CheckSwitchBtn();
                                    ControlPanel.Instance.SetArea1PopupParent();
                                    //ControlPanel.Instance.ShowPanel(true);
                                    SoundManager.Instance.SetSirenVolume();
                                    inventoryObj.ShowSmokeDetect(false);
                                    smokeDetectorPopup.ResetSmokeDetectorTime();
                                    smokeDetectorPopup.InitCrossCircuitDetector(isOn =>
                                    {
                                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, isOn);
                                        inventoryObj.ShowHeatDetect(!CheckDetector2());
                                        inventoryObj.ShowSmokeDetect(!CheckDetector1());
                                        ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                                        SoundManager.Instance.SetSirenVolume(0f);
                                        smokeDetectorOn.SetActive(true);

                                    }, delegate
                                    {

                                    });
                                    smokeDetectorPopup.gameObject.SetActive(true);
                                    smokeDetectorPopup.closeBtn.gameObject.SetActive(true);
                                }
                            }
                        };
                    }
                    areaManagerObj.area1Btn.onClick.AddListener(delegate
                    {
                        inventoryObj.ShowSmokeDetect(true);
                        inventoryObj.ShowHeatDetect(true);
                        ControlPanel.Instance.SetArea1PopupParent();
                        ShowRoom(area1Obj?.gameObject);
                        bool isHeatDetected = false;
                        bool isSmokeDetected = false;
                        uiDragAndCollisionHandler.ResetEvent();
                        uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                        {
                            inventoryObj.ShowHeatDetect(!CheckDetector2());
                            inventoryObj.ShowSmokeDetect(!CheckDetector1());
                            if (heatDetectorObj.Equals(targetObject))
                            {
                                if (!isHeatDetected)
                                {
                                    isHeatDetected = true;
                                    ControlPanel.Instance.SetArea1PopupParent();
                                    //ControlPanel.Instance.ShowPanel(true);

                                    //ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                                    ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                                    SoundManager.Instance.MuteSiren(true);
                                    smokeDetectorOn.SetActive(CheckDetector1());
                                    heatDetectorPopup.InitCrossCircuitDetector(isOn =>
                                    {
                                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, isOn);
                                        inventoryObj.ShowHeatDetect(!CheckDetector2());
                                        inventoryObj.ShowSmokeDetect(!CheckDetector1());
                                        //heatDetectorPopup.gameObject.SetActive(false);
                                        heatDetectorOn.SetActive(true);
                                        SoundManager.Instance.SetSirenVolume(0f);
                                        ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());

                                    });
                                    heatDetectorPopup.gameObject.SetActive(true);
                                    heatDetectorPopup.closeBtn.gameObject.SetActive(true);
                                }

                            }
                            if (smokeDetectorObj.Equals(targetObject))
                            {
                                if (!isSmokeDetected)
                                {
                                    isSmokeDetected = true;
                                    ControlPanel.Instance.CheckSwitchBtn();
                                    ControlPanel.Instance.SetArea1PopupParent();
                                    //ControlPanel.Instance.ShowPanel(true);
                                    SoundManager.Instance.SetSirenVolume();
                                    inventoryObj.ShowSmokeDetect(false);
                                    smokeDetectorPopup.ResetSmokeDetectorTime();
                                    smokeDetectorPopup.InitCrossCircuitDetector(isOn =>
                                    {
                                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, isOn);
                                        inventoryObj.ShowHeatDetect(!CheckDetector2());
                                        inventoryObj.ShowSmokeDetect(!CheckDetector1());
                                        ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                                        SoundManager.Instance.SetSirenVolume(0f);
                                        smokeDetectorOn.SetActive(true);


                                    }, delegate
                                    {

                                    });
                                    smokeDetectorPopup.gameObject.SetActive(true);
                                    smokeDetectorPopup.closeBtn.gameObject.SetActive(true);
                                }
                            }
                        };
                    });
                    */
                }
                break;
            case GasSysCrossCircuitDetectorESection.E2: //연기감지기
                {
                    ControlPanel.Instance.ShowPanel(false);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    ShowRoom(area1Obj?.gameObject);
                    inventoryObj.ShowSmokeDetect(true);
                    inventoryObj.ShowHeatDetect(true);
                    ControlPanel.Instance.SetArea1PopupParent();
                    areaManagerObj.area1Btn.onClick.RemoveAllListeners();
                    bool isHeatDetected = false;
                    bool isSmokeDetected = false;
                    uiDragAndCollisionHandler.ResetEvent();
                    uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        inventoryObj.ShowHeatDetect(!CheckDetector2());
                        inventoryObj.ShowSmokeDetect(!CheckDetector1());
                        if (smokeDetectorObj.Equals(targetObject))
                        {
                            if (!isSmokeDetected)
                            {
                                isSmokeDetected = true;

                                ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                                ControlPanel.Instance.CheckSwitchBtn();
                                ControlPanel.Instance.SetArea1PopupParent();
                                //ControlPanel.Instance.ShowPanel(true);
                                SoundManager.Instance.SetSirenVolume();
                                inventoryObj.ShowSmokeDetect(false);
                                smokeDetectorPopup.ResetSmokeDetectorTime();
                                smokeDetectorPopup.InitCrossCircuitDetector(isOn =>
                                {
                                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, isOn);
                                    inventoryObj.ShowHeatDetect(!CheckDetector2());
                                    inventoryObj.ShowSmokeDetect(!CheckDetector1());
                                    ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                                    SoundManager.Instance.SetSirenVolume(0f);
                                    smokeDetectorOn.SetActive(true);
                                    _gasSysEvaluation.연기감지 = true;
                                }, delegate
                                {

                                });
                                smokeDetectorPopup.gameObject.SetActive(true);
                                smokeDetectorPopup.closeBtn.gameObject.SetActive(true);
                            }
                        }
                    };
                }
                break;
            case GasSysCrossCircuitDetectorESection.E3: //열감지기
                {
                    inventoryObj.ShowSmokeDetect(false);
                    smokeDetectorOn.SetActive(true);
                    smokeDetectorPopup.gameObject.SetActive(false);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
                    areaManagerObj.area1Btn.onClick.RemoveAllListeners();
                    bool isHeatDetected = false;
                    bool isSmokeDetected = false;
                    uiDragAndCollisionHandler.ResetEvent();
                    uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        inventoryObj.ShowHeatDetect(!CheckDetector2());
                        inventoryObj.ShowSmokeDetect(!CheckDetector1());
                        if (heatDetectorObj.Equals(targetObject))
                        {
                            if (!isHeatDetected)
                            {
                                isHeatDetected = true;
                                ControlPanel.Instance.SetArea1PopupParent();
                                //ControlPanel.Instance.ShowPanel(true);

                                //ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                                ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                                ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                                ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                                SoundManager.Instance.MuteSiren(true);
                                smokeDetectorOn.SetActive(CheckDetector1());
                                heatDetectorPopup.InitCrossCircuitDetector(isOn =>
                                {
                                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, isOn);
                                    inventoryObj.ShowHeatDetect(!CheckDetector2());
                                    inventoryObj.ShowSmokeDetect(!CheckDetector1());
                                    //heatDetectorPopup.gameObject.SetActive(false);
                                    heatDetectorOn.SetActive(CheckDetector2());
                                    SoundManager.Instance.SetSirenVolume(0f);
                                    ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                                    _gasSysEvaluation.열감지 = true;

                                    if (CheckDetector1() && CheckDetector2())
                                    {
                                        heatDetectorPopup.gameObject.SetActive(false);
                                        solenoidValvePopup.SetArea1PopupParent();
                                        solenoidValvePopup.clipObj.SetActive(true);
                                        solenoidValvePopup.DefaultActivation();
                                        solenoidValvePopup.gameObject.SetActive(true);
                                        SoundManager.Instance.SetSirenVolume();
                                        ControlPanel.Instance.ShowPanel(true);
                                        ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                                        ControlPanel.Instance.SetTimeNum(30f);
                                        ControlPanel.Instance.StartTimer(30);
                                        ControlPanel.OnTimerEnd.RemoveAllListeners();
                                        ControlPanel.OnTimerEnd.AddListener(delegate
                                        {
                                            //storageRoomObj?.ManualActivateToSolenoidValve();
                                            solenoidValvePopup.SolenoidValveAActivation();
                                            solenoidValvePopup.closeBtn.gameObject.SetActive(true);
                                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                                            // Observable.Timer(System.TimeSpan.FromSeconds(3))
                                            //     .Subscribe(_ =>
                                            //     {
                                            //         NextEnable();
                                            //     })
                                            //     .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
                                            //NextEnable();
                                        });
                                    }

                                });
                                heatDetectorPopup.gameObject.SetActive(true);
                                heatDetectorPopup.closeBtn.gameObject.SetActive(true);
                            }

                        }
                    };
                }
                break;
            case GasSysCrossCircuitDetectorESection.E4:
                {
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, true);
                    heatDetectorPopup.gameObject.SetActive(false);
                    solenoidValvePopup.SetArea1PopupParent();
                    solenoidValvePopup.clipObj.SetActive(true);
                    solenoidValvePopup.DefaultActivation();
                    solenoidValvePopup.gameObject.SetActive(true);
                    solenoidValvePopup.closeBtn.gameObject.SetActive(true);
                    //SoundManager.Instance.SetSirenVolume();
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                    ControlPanel.Instance.SetTimeNum(00);
                    ControlPanel.Instance.ResetTimer();
                    inventoryObj.ShowHeatDetect(!CheckDetector2());
                    inventoryObj.ShowSmokeDetect(!CheckDetector1());
                    heatDetectorOn.SetActive(CheckDetector2());
                    ControlPanel.OnTimerEnd.RemoveAllListeners();
                    교차회로감지기평가1.SetActive(true);
                    solenoidValvePopup.SolenoidValveAActivation(false);
                    // ControlPanel.OnTimerEnd.AddListener(delegate
                    // {
                    //     //storageRoomObj?.ManualActivateToSolenoidValve();
                    //     solenoidValvePopup.SolenoidValveAActivation();
                    //     solenoidValvePopup.closeBtn.gameObject.SetActive(true);
                    //     ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                    //     // Observable.Timer(System.TimeSpan.FromSeconds(3))
                    //     //     .Subscribe(_ =>
                    //     //     {
                    //     //         NextEnable();
                    //     //     })
                    //     //     .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
                    //     //NextEnable();
                    // });
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    SoundManager.Instance.StopAllFireSound();
                    //ButtonManager.Instance.DisableAllButtons();
                    NextEnable(false);
                }
                break;
            /*
            case GasSysCrossCircuitDetectorESection.E5:
                {
                    if (CheckDetector1() && CheckDetector2())
                    {
                        solenoidValvePopup.SetArea1PopupParent();
                        solenoidValvePopup.clipObj.SetActive(true);
                        solenoidValvePopup.DefaultActivation();
                        solenoidValvePopup.gameObject.SetActive(true);
                        SoundManager.Instance.SetSirenVolume();
                        ControlPanel.Instance.ShowPanel(true);
                        ControlPanel.Instance.ShowFire(CheckDetector1() || CheckDetector2());
                        ControlPanel.Instance.SetTimeNum(30f);
                        ControlPanel.Instance.StartTimer(30);
                        ControlPanel.OnTimerEnd.RemoveAllListeners();
                        ControlPanel.OnTimerEnd.AddListener(delegate
                        {
                            //storageRoomObj?.ManualActivateToSolenoidValve();
                            solenoidValvePopup.SolenoidValveAActivation();
                            solenoidValvePopup.closeBtn.gameObject.SetActive(true);
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                            // Observable.Timer(System.TimeSpan.FromSeconds(3))
                            //     .Subscribe(_ =>
                            //     {
                            //         NextEnable();
                            //     })
                            //     .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
                            //NextEnable();
                        });
                    }
                }
                break;
                */
            case GasSysCrossCircuitDetectorESection.평가종료:
                {
                    교차회로감지기평가1.SetActive(false);
                    GasSysGlobalCanvas.Instance.totalScore.교차회로.감시제어반조치 = !_gasSysEvaluation.주경종 && !_gasSysEvaluation.지구경종 && !_gasSysEvaluation.사이렌 &&
                                                                          !_gasSysEvaluation.비상방송 && !_gasSysEvaluation.부저 && _gasSysEvaluation.축적 &&
                                                                          _gasSysEvaluation.솔레노이드연동;
                    //GasSysGlobalCanvas.Instance.totalScore.교차회로.화재표시동작확인 = _gasSysEvaluation.연기감지 && _gasSysEvaluation.열감지;
                    GasSysGlobalCanvas.Instance.totalScore.교차회로.연기감지기 = _gasSysEvaluation.연기감지;
                    GasSysGlobalCanvas.Instance.totalScore.교차회로.열감지기 = _gasSysEvaluation.열감지;
                    GasSysGlobalCanvas.Instance.totalScore.교차회로.동작확인 = !교차회로감지기토글1.isOn &&
                                                                       교차회로감지기토글2.isOn &&
                                                                       !교차회로감지기토글3.isOn &&
                                                                       !교차회로감지기토글4.isOn &&
                                                                       !교차회로감지기토글5.isOn &&
                                                                       !교차회로감지기토글6.isOn;
                    var results = new List<ResultObject>
                    {
                        new ResultObject()
                        {
                            Title = "감시제어반 조치",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.교차회로.감시제어반조치
                        },
                        new ResultObject()
                        {
                            Title = "연기감지기 테스트",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.교차회로.연기감지기
                        },
                        new ResultObject()
                        {
                            Title = "열감지기 테스트",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.교차회로.열감지기
                        },
                        new ResultObject()
                        {
                            Title = "정상작동 상태확인",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.교차회로.동작확인
                        },
                        // new ResultObject()
                        // {
                        //     Title = "열감지기 동작",
                        //     IsSuccess = GasSysGlobalCanvas.Instance.totalScore.교차회로.열감지기
                        // }

                    };
                    //GasSysGlobalCanvas.Instance.SetResultPopup(results);
                    GasSysGlobalCanvas.Instance.totalScore.교차회로List.Clear();
                    GasSysGlobalCanvas.Instance.totalScore.교차회로List.AddRange(results);
                    GasSysGlobalCanvas.Instance.SetNextEvaluation(InitControlPanelSwitch, InitCrossCircuitDetector);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        ControlPanel.Instance.CheckWarringSwitch();
    }

    private void ChangeStateCrossCircuitDetectorP(GasSysCrossCircuitDetectorPSection state)
    {
        _crossCircuitDetectorP = state;
        OnStateChangedCrossCircuitDetectorP(_crossCircuitDetectorP);
    }



    private void OnStateChangedCrossCircuitDetectorP(GasSysCrossCircuitDetectorPSection state)
    {
        InitStorageRoom();
        InitArea1Corridor();
        InitArea1();
        inventoryObj.ShowSmokeDetect(true);
        inventoryObj.ShowHeatDetect(true);
        inventoryObj.ShowSolenoidValve1(true);
        inventoryObj.ShowSolenoidValve2(true);
        //SoundManager.Instance.StopAllFireSound();
        // ControlPanel.Instance.ShowFire(false);
        // ControlPanel.Instance.InitTimer();
        ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
        ControlPanel.OnTimerCheck.RemoveAllListeners();
        ControlPanel.OnTimerEnd.RemoveAllListeners();
        ButtonManager.Instance.RemoveAllHighlights();
        uiDragAndCollisionHandler.ResetEvent();
        uiDragAndCollisionHandler.StopDragging();
        switch (state)
        {

            case GasSysCrossCircuitDetectorPSection.감시제어반선택:
                {
                    ShowRoom(storageRoomObj.gameObject);
                    controlBoxBtn.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificButton(controlBoxBtn);
                    NextDisable();
                    controlBoxBtn.onClick.AddListener(delegate
                    {
                        controlBoxBtn.gameObject.SetActive(false);
                        Next();
                    });
                }
                break;
            case GasSysCrossCircuitDetectorPSection.제어반음향및솔레노이드밸브연동:
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
            case GasSysCrossCircuitDetectorPSection.축적비축적유지:
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
                    temp.솔레노이드밸브 = ControlMode.Auto;
                    SetHighlightControlPanelCheck(temp);
                    NextEnable();
                }
                break;
                */
            case GasSysCrossCircuitDetectorPSection.방호구역1번:
                {
                    ShowRoom(storageRoomObj.gameObject);
                    ButtonManager.Instance.EnableSpecificButton(areaManagerObj.area1Btn);
                    NextDisable();
                    areaManagerObj.area1Btn.onClick.RemoveAllListeners();
                    areaManagerObj.area1Btn.onClick.AddListener(Next);
                }
                break;
            case GasSysCrossCircuitDetectorPSection.연기감지기:
                {

                    NextDisable();
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    uiDragAndCollisionHandler.StartDragging();
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    ControlPanel.Instance.ShowFire(false);
                    SoundManager.Instance.SetBuzzerVolume(0);
                    SoundManager.Instance.StopAllFireSound();
                    smokeDetectorPopup.ResetSmokeDetectorTime();

                    var btnList = new List<SwitchButtonCheck>();
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
                    ShowRoom(area1Obj.gameObject);
                    ButtonManager.Instance.EnableSpecificImage(inventoryObj.smokeDetect);
                    NextDisable();
                    bool isDetected = false;
                    uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (smokeDetectorObj.Equals(targetObject))
                        {
                            // ControlPanel.Instance.SetArea1PopupParent();
                            // ControlPanel.Instance.ShowPanel(true);
                            // inventoryObj.ShowSmokeDetect(false);
                            // smokeDetectorPopup.InitCrossCircuitDetector(isOn =>
                            // {
                            //     ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, isOn);
                            //     ControlPanel.Instance.ShowFire(true);
                            //     SoundManager.Instance.MuteSiren(true);
                            //     smokeDetectorOn.SetActive(true);
                            //     NextEnable();
                            // });
                            // smokeDetectorPopup.gameObject.SetActive(true);
                            if (isDetected)
                                return;
                            Next();
                            isDetected = true;
                        }
                    };

                }
                break;
            case GasSysCrossCircuitDetectorPSection.연기감지기2:
                {
                    ControlPanel.Instance.SetArea1PopupParent();
                    ControlPanel.Instance.ShowPanel(true);
                    inventoryObj.ShowSmokeDetect(false);
                    ButtonManager.Instance.EnableSpecificImage();
                    smokeDetectorPopup.InitCrossCircuitDetector(isOn =>
                    {
                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, isOn);
                        ControlPanel.Instance.ShowFire(true);
                        SoundManager.Instance.MuteSiren(true);
                        SoundManager.Instance.SetBuzzerVolume(0);
                        smokeDetectorOn.SetActive(true);
                        NextEnable(false);
                    });
                    smokeDetectorPopup.gameObject.SetActive(true);
                }
                break;
            case GasSysCrossCircuitDetectorPSection.열감지기:
                {
                    ControlPanel.Instance.SetArea1PopupParent();
                    ControlPanel.Instance.ShowPanel(false);
                    inventoryObj.ShowSmokeDetect(false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    ControlPanel.Instance.ShowFire(true);
                    smokeDetectorOn.SetActive(true);
                    ShowRoom(area1Obj.gameObject);
                    SoundManager.Instance.MuteSiren(true);
                    uiDragAndCollisionHandler.StartDragging();
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    var btnList = new List<SwitchButtonCheck>();
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
                    ButtonManager.Instance.EnableSpecificButton(btnList);
                    ButtonManager.Instance.EnableSpecificImage(inventoryObj.heatDetect);
                    NextDisable();
                    bool isDetected = false;
                    uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (heatDetectorObj.Equals(targetObject))
                        {
                            if (isDetected)
                                return;
                            Next();
                            isDetected = true;

                            // ControlPanel.Instance.SetArea1PopupParent();
                            // ControlPanel.Instance.ShowPanel(true);
                            // inventoryObj.ShowHeatDetect(false);
                            // heatDetectorPopup.InitCrossCircuitDetector(isOn =>
                            // {
                            //     ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, isOn);
                            //     ControlPanel.Instance.ShowFire(true);
                            //     heatDetectorPopup.gameObject.SetActive(false);
                            //     heatDetectorOn.SetActive(true);
                            //     solenoidValvePopup.SetArea1PopupParent();
                            //     solenoidValvePopup.clipObj.SetActive(true);
                            //     solenoidValvePopup.gameObject.SetActive(true);
                            //     SoundManager.Instance.MuteSiren(false);
                            //     ControlPanel.Instance.ShowFire(true);
                            //     ControlPanel.Instance.SetTimeNum(30f);
                            //     ControlPanel.Instance.StartTimer(30);
                            //     ControlPanel.OnTimerEnd.RemoveAllListeners();
                            //     ControlPanel.OnTimerEnd.AddListener(delegate
                            //     {
                            //         //storageRoomObj?.ManualActivateToSolenoidValve();
                            //         solenoidValvePopup.SolenoidValveAActivation();
                            //         ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                            //         Observable.Timer(System.TimeSpan.FromSeconds(3))
                            //             .Subscribe(_ =>
                            //             {
                            //                 NextEnable();
                            //             })
                            //             .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
                            //     });
                            // });
                            // heatDetectorPopup.gameObject.SetActive(true);
                        }
                    };

                }
                break;
            case GasSysCrossCircuitDetectorPSection.열감지기2:
                {
                    smokeDetectorOn.SetActive(true);
                    ControlPanel.Instance.SetArea1PopupParent();
                    ControlPanel.Instance.ShowPanel(true);
                    inventoryObj.ShowHeatDetect(false);
                    inventoryObj.ShowSmokeDetect(false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    ControlPanel.Instance.ShowFire(true);
                    SoundManager.Instance.MuteSiren(true);
                    smokeDetectorOn.SetActive(true);
                    NextDisable();
                    heatDetectorPopup.InitCrossCircuitDetector(isOn =>
                    {
                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, isOn);
                        heatDetectorPopup.gameObject.SetActive(false);
                        heatDetectorOn.SetActive(true);
                        solenoidValvePopup.SetArea1PopupParent();
                        solenoidValvePopup.clipObj.SetActive(true);
                        solenoidValvePopup.gameObject.SetActive(true);
                        SoundManager.Instance.MuteSiren(false);
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

                    });
                    heatDetectorPopup.gameObject.SetActive(true);
                    GasSysGlobalCanvas.Instance.HideCheckObj();
                }
                break;
            case GasSysCrossCircuitDetectorPSection.솔정상확인:
                {
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    smokeDetectorOn.SetActive(true);
                    heatDetectorOn.SetActive(true);
                    inventoryObj.ShowHeatDetect(false);
                    inventoryObj.ShowSmokeDetect(false);
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
                    solenoidValvePopup.SetArea1PopupParent();
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
                    //GasSysGlobalCanvas.Instance.ShowSol3(Next);
                }
                break;

            case GasSysCrossCircuitDetectorPSection.음향정지작동확인:
                {
                    GasSysGlobalCanvas.Instance.HideCheckObj();
                    smokeDetectorOn.SetActive(true);
                    heatDetectorOn.SetActive(true);
                    inventoryObj.ShowHeatDetect(false);
                    inventoryObj.ShowSmokeDetect(false);
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.SoundCheck();
                    // ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Auto);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    temp.축적 = false;
                    temp.복구 = false;
                    //SetHighlightControlPanel(temp, true);
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
            case GasSysCrossCircuitDetectorPSection.제어반화재표시:
                {
                    NextDisable();
                    smokeDetectorOn.SetActive(true);
                    heatDetectorOn.SetActive(true);
                    inventoryObj.ShowHeatDetect(false);
                    inventoryObj.ShowSmokeDetect(false);
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
                    temp.복구 = false;
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
            case GasSysCrossCircuitDetectorPSection.방호구역1:
                {
                    smokeDetectorOn.SetActive(true);
                    heatDetectorOn.SetActive(true);
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(true);
                    // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    GasSysGlobalCanvas.Instance.ShowArea1_2(Next);
                    SoundManager.Instance.MuteSiren(false);
                    var temp = new ControlPanelButtonCheck();
                    temp.SetBtn(false);
                    temp.축적 = false;
                    temp.복구 = false;
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
            case GasSysCrossCircuitDetectorPSection.방호구역1음향연동:
                {
                    smokeDetectorOn.SetActive(true);
                    heatDetectorOn.SetActive(true);
                    inventoryObj.ShowHeatDetect(false);
                    inventoryObj.ShowSmokeDetect(false);
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
                    // //SetHighlightControlPanel(temp, true);
                    // NextDisable();
                    // //NextEnable(false);
                    // temp.SetBtn(false);
                    // temp.축적 = true;
                    // temp.복구 = false;
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
                    GasSysGlobalCanvas.Instance.ShowArea1_2(Next);
                    SoundManager.Instance.MuteSiren(false);
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                    //ButtonManager.Instance.EnableSpecificButton(_switchButtons);
                }
                break;
            case GasSysCrossCircuitDetectorPSection.음향정지:
                {
                    smokeDetectorOn.SetActive(true);
                    heatDetectorOn.SetActive(true);
                    inventoryObj.ShowHeatDetect(false);
                    inventoryObj.ShowSmokeDetect(false);
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
                    //SetHighlightControlPanel(temp, true);
                    NextDisable();
                    //NextEnable(false);
                    temp.SetBtn(true);
                    temp.축적 = true;
                    temp.복구 = false;
                    SetHighlightControlPanel(temp, true);
                    SetHighlightControlPanelCheck(temp);
                    GasSysGlobalCanvas.Instance.ShowArea1_2(Next);
                    SoundManager.Instance.MuteSiren(false);
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                }
                break;
            case GasSysCrossCircuitDetectorPSection.방호구역2:
                {
                    smokeDetectorOn.SetActive(true);
                    heatDetectorOn.SetActive(true);
                    inventoryObj.ShowHeatDetect(false);
                    inventoryObj.ShowSmokeDetect(false);
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
                    // temp.축적 = true;
                    // temp.복구 = false;
                    //
                    // NextDisable();
                    // temp.SetBtn(false);
                    // temp.축적 = true;
                    // temp.복구 = false;
                    // SetHighlightControlPanel(temp, true);
                    // SetHighlightControlPanelCheck(temp);
                    // //NextEnable(false);
                    // ControlPanel.Instance.InitTimer();
                    // SetHighlightControlPanelCheck(temp);
                    ControlPanel.Instance.InitTimer();
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
                    ControlPanel.Instance.SetTimeNum(0f);
                    SoundManager.Instance.MuteSiren(true);
                    GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                    ControlPanel.OnTimerEnd.RemoveAllListeners();
                }
                break;
            /*
            case GasSysCrossCircuitDetectorPSection.지연장치:
                {
                    smokeDetectorOn.SetActive(true);
                    heatDetectorOn.SetActive(true);
                    smokeDetectorOn.SetActive(true);
                    heatDetectorOn.SetActive(true);
                    inventoryObj.ShowSmokeDetect(false);
                    inventoryObj.ShowHeatDetect(false);
                    inventoryObj.ShowSolenoidValve1(false);
                    ControlPanel.Instance.ShowPanel(true);
                    GasSysGlobalCanvas.Instance.ShowTimer(Next);
                    ControlPanel.Instance.SoundCheck();
                    //SoundManager.Instance.MuteSiren(false);
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
            case GasSysCrossCircuitDetectorPSection.솔레노이드작동여부:
                {
                    smokeDetectorOn.SetActive(true);
                    heatDetectorOn.SetActive(true);
                    inventoryObj.ShowSmokeDetect(false);
                    inventoryObj.ShowHeatDetect(false);
                    inventoryObj.ShowSolenoidValve1(false);
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
            case GasSysCrossCircuitDetectorPSection.교육종료:
                {
                    NextDisable();
                    inventoryObj.ShowHeatDetect(false);
                    inventoryObj.ShowSmokeDetect(false);
                    ControlPanel.Instance.ShowPanel(true);
                    SetCompletePopup("교차회로 감지기 작동 시험을 모두 완료했습니다.", "다음 점검을", InitControlPanelSwitch);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        ControlPanel.Instance.CheckWarringSwitch();
        //ButtonManager.Instance.HighlightObj();
    }
}
