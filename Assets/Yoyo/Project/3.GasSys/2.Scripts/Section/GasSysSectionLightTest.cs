using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public partial class GasSysSection
{
    [Foldout("방출표시등 점검 확인")]
    private GasSysDischargeLightTestPSection _curDischargeLightTestPSection;
    private GasSysDischargeLightTestESection _curDischargeLightTestESection;
    [FormerlySerializedAs("dischargeLightTestHintObj")]
    public HintScriptableObj dischargeLightTestHintPObj;
    public HintScriptableObj dischargeLightTestHintEObj;
    public GameObject 방출표시등평가1;
    public Toggle 방출표시등토글1;
    public Toggle 방출표시등토글2;
    public Toggle 방출표시등토글3;
    [EndFoldout]
    public void InitDischargeLightTest()
    {
        InitEMode();
        curMainSection = GasSysMainSection.DischargeLightTest;
        nextBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.RemoveAllListeners();
        areaManagerObj?.StartArea(GasSysAreaManager.StartAreaType.StorageRoom);
        areaManagerObj?.ShowPanel(true);
        ShowRoom(area1CorridorObj?.gameObject);
        //storageRoomObj?.InitSafetyCheck();
        inventoryObj?.Init();
        inventoryObj?.ShowPanel(true);
        ControlPanel.Instance.Init();
        GasSysGlobalCanvas.Instance.HideCheckObj();
        //ChangeState(GasSysSafetyCheckSection.선택밸브조작동관선택);
        _maxSection = _gasSysState switch
        {
            GasSysState.PracticeMode => System.Enum.GetValues(typeof(GasSysDischargeLightTestPSection)).Length,
            GasSysState.EvaluationMode => System.Enum.GetValues(typeof(GasSysDischargeLightTestESection)).Length,
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
    }



    private void ShowDischargeLightTestSection(int index)
    {
        if (_gasSysState.Equals(GasSysState.PracticeMode))
        {
            prevBtn.gameObject.SetActive(true);
            if (!_maxSection.Equals(index))
            {
                GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(dischargeLightTestHintPObj, index));
            }
            ChangeDischargeLightTestP(GasSysDischargeLightTestPSection.방호구역1출입문소등 + index);
        }
        if (!_gasSysState.Equals(GasSysState.EvaluationMode))
            return;
        prevBtn.gameObject.SetActive(false);
        GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(dischargeLightTestHintEObj, index));
        ChangeDischargeLightTestE(GasSysDischargeLightTestESection.Init + index);

    }
    public void ChangeDischargeLightTestE(GasSysDischargeLightTestESection state)
    {

        _curDischargeLightTestESection = state;
        OnStateChangedDischargeLightTestE(_curDischargeLightTestESection);
    }

    private void OnStateChangedDischargeLightTestE(GasSysDischargeLightTestESection state)
    {
        switch (state)
        {

            case GasSysDischargeLightTestESection.Init:
                {
                    InitStorageRoom();
                    ShowRoom(storageRoomObj?.gameObject);
                    NextEnable();
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

                    // activationBoxBtn.onClick.AddListener(delegate
                    // {
                    //     activationCylinderBoxPopup.closeBtn.gameObject.SetActive(true);
                    //     activationCylinderBoxPopup.Init();
                    //     ControlPanel.Instance.ShowPanel(true);
                    //     if (!CheckVerifyDischarge())
                    //     {
                    //         activationCylinderBoxPopup.PressureSwitchDown();
                    //         // activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                    //         // activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                    //         activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(true);
                    //         activationCylinderBoxPopup.selectSwitchUpBtn.onClick.RemoveAllListeners();
                    //         activationCylinderBoxPopup.selectSwitchUpBtn.onClick.AddListener(delegate
                    //         {
                    //             ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, true);
                    //             activationCylinderBoxPopup.PressureSwitchUp();
                    //             SoundManager.Instance.PlaySiren(true);
                    //             activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(false);
                    //             activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(true);
                    //             GlobalCanvas.Instance.ShowLightTestOn(GlobalCanvas.Instance.HideCheckObj);
                    //             //Next();
                    //         });
                    //     }
                    //     else
                    //     {
                    //         activationCylinderBoxPopup.PressureSwitchUp();
                    //         activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(true);
                    //         activationCylinderBoxPopup.selectSwitchDownBtn.onClick.RemoveAllListeners();
                    //         activationCylinderBoxPopup.selectSwitchDownBtn.onClick.AddListener(delegate
                    //         {
                    //             ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);
                    //             activationCylinderBoxPopup.PressureSwitchDown();
                    //             SoundManager.Instance.StopAllFireSound();
                    //             activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(true);
                    //             activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(false);
                    //             GlobalCanvas.Instance.ShowLightTestOff(GlobalCanvas.Instance.HideCheckObj);
                    //             //Next();
                    //         });
                    //     }
                    //     activationCylinderBoxPopup.gameObject.SetActive(true);
                    // });


                    selectionValvePopup.SetOnOff(false);
                    storageCylinderPopup.SetOnOff(false);

                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);

                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.CheckSwitchBtn();
                    ControlPanel.Instance.SoundCheck();
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    ControlPanel.Instance.SetClose();
                    ButtonManager.Instance.SetEvaluationButtons();
                    Next();
                }
                break;
            case GasSysDischargeLightTestESection.E1:
                {
                    activationBoxBtn.onClick.RemoveAllListeners();
                    if (!activationCylinderBoxPopup.gameObject.activeSelf)
                    {
                        activationCylinderBoxPopup.closeBtn.gameObject.SetActive(true);
                        activationCylinderBoxPopup.Init();
                        activationCylinderBoxPopup.SetLightTestPos();
                        //ControlPanel.Instance.ShowPanel(true);
                        if (ControlPanel.Instance.CheckVerifyDischarge())
                            GasSysGlobalCanvas.Instance.ShowLightTestCheck(GasSysGlobalCanvas.Instance.HideCheckObj);
                        else
                        {
                            GasSysGlobalCanvas.Instance.ShowLightTestOff(GasSysGlobalCanvas.Instance.HideCheckObj);
                        }
                        GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                        if (!CheckVerifyDischarge())
                        {
                            activationCylinderBoxPopup.PressureSwitchDown();
                            // activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                            // activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                            activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(true);
                        }
                        else
                        {
                            activationCylinderBoxPopup.PressureSwitchUp();
                            activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(true);

                        }
                        activationCylinderBoxPopup.selectSwitchUpBtn.onClick.RemoveAllListeners();
                        activationCylinderBoxPopup.selectSwitchUpBtn.onClick.AddListener(delegate
                        {
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, true);
                            activationCylinderBoxPopup.PressureSwitchUp(true);
                            SoundManager.Instance.PlaySiren(true);
                            activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(false);
                            activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(true);
                            GasSysGlobalCanvas.Instance.ShowLightTestCheck(GasSysGlobalCanvas.Instance.HideCheckObj);
                            GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                            _gasSysEvaluation.스위치업 = true;
                            //Next();
                        });
                        activationCylinderBoxPopup.selectSwitchDownBtn.onClick.RemoveAllListeners();
                        activationCylinderBoxPopup.selectSwitchDownBtn.onClick.AddListener(delegate
                        {
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);
                            activationCylinderBoxPopup.PressureSwitchDown(true);
                            SoundManager.Instance.StopAllFireSound();
                            activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(false);
                            activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(true);
                            GasSysGlobalCanvas.Instance.ShowLightTestOff(GasSysGlobalCanvas.Instance.HideCheckObj);
                            GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                            _gasSysEvaluation.스위치업 = false;
                            //Next();
                        });
                        activationCylinderBoxPopup.gameObject.SetActive(true);
                    }
                    activationBoxBtn.onClick.AddListener(delegate
                    {
                        activationCylinderBoxPopup.closeBtn.gameObject.SetActive(true);
                        activationCylinderBoxPopup.Init();
                        activationCylinderBoxPopup.SetLightTestPos();
                        //ControlPanel.Instance.ShowPanel(true);
                        if (ControlPanel.Instance.CheckVerifyDischarge())
                            GasSysGlobalCanvas.Instance.ShowLightTestCheck(GasSysGlobalCanvas.Instance.HideCheckObj);
                        else
                        {
                            GasSysGlobalCanvas.Instance.ShowLightTestOff(GasSysGlobalCanvas.Instance.HideCheckObj);
                        }
                        GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                        if (!CheckVerifyDischarge())
                        {
                            activationCylinderBoxPopup.PressureSwitchDown();
                            // activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                            // activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                            activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(true);
                        }
                        else
                        {
                            activationCylinderBoxPopup.PressureSwitchUp();
                            activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(true);

                        }
                        activationCylinderBoxPopup.selectSwitchUpBtn.onClick.RemoveAllListeners();
                        activationCylinderBoxPopup.selectSwitchUpBtn.onClick.AddListener(delegate
                        {
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, true);
                            activationCylinderBoxPopup.PressureSwitchUp(true);
                            SoundManager.Instance.PlaySiren(true);
                            activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(false);
                            activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(true);
                            GasSysGlobalCanvas.Instance.ShowLightTestCheck(GasSysGlobalCanvas.Instance.HideCheckObj);
                            GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                            _gasSysEvaluation.스위치업 = true;
                            //Next();
                        });
                        activationCylinderBoxPopup.selectSwitchDownBtn.onClick.RemoveAllListeners();
                        activationCylinderBoxPopup.selectSwitchDownBtn.onClick.AddListener(delegate
                        {
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);
                            activationCylinderBoxPopup.PressureSwitchDown(true);
                            SoundManager.Instance.StopAllFireSound();
                            activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(false);
                            activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(true);
                            GasSysGlobalCanvas.Instance.ShowLightTestOff(GasSysGlobalCanvas.Instance.HideCheckObj);
                            GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                            _gasSysEvaluation.스위치업 = false;
                            //Next();
                        });
                        activationCylinderBoxPopup.gameObject.SetActive(true);
                    });
                }
                break;
            case GasSysDischargeLightTestESection.E2:
                {
                    ControlPanel.Instance.ShowPanel(true);
                    activationCylinderBoxPopup.gameObject.SetActive(false);
                    방출표시등평가1.SetActive(true);
                    SoundManager.Instance.MuteSiren(true);
                    GasSysGlobalCanvas.Instance.ShowLightTestCheck(null);
                }
                break;
            case GasSysDischargeLightTestESection.E3:
                {
                    ControlPanel.Instance.ShowPanel(false);
                    방출표시등평가1.SetActive(false);
                    SoundManager.Instance.MuteSiren(false);
                    activationCylinderBoxPopup.gameObject.SetActive(true);
                    activationBoxBtn.onClick.RemoveAllListeners();
                    if (activationCylinderBoxPopup.gameObject.activeSelf)
                    {
                        activationCylinderBoxPopup.closeBtn.gameObject.SetActive(true);
                        activationCylinderBoxPopup.Init();
                        activationCylinderBoxPopup.SetLightTestPos();
                        //ControlPanel.Instance.ShowPanel(true);
                        if (ControlPanel.Instance.CheckVerifyDischarge())
                            GasSysGlobalCanvas.Instance.ShowLightTestCheck(GasSysGlobalCanvas.Instance.HideCheckObj);
                        else
                        {
                            GasSysGlobalCanvas.Instance.ShowLightTestOff(GasSysGlobalCanvas.Instance.HideCheckObj);
                        }
                        GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                        if (!CheckVerifyDischarge())
                        {
                            activationCylinderBoxPopup.PressureSwitchDown();
                            // activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                            // activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                            activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(true);

                        }
                        else
                        {
                            activationCylinderBoxPopup.PressureSwitchUp();
                            activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(true);

                        }
                        activationCylinderBoxPopup.selectSwitchUpBtn.onClick.RemoveAllListeners();
                        activationCylinderBoxPopup.selectSwitchUpBtn.onClick.AddListener(delegate
                        {
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, true);
                            activationCylinderBoxPopup.PressureSwitchUp(true);
                            SoundManager.Instance.PlaySiren(true);
                            activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(false);
                            activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(true);
                            GasSysGlobalCanvas.Instance.ShowLightTestCheck(GasSysGlobalCanvas.Instance.HideCheckObj);
                            GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                            _gasSysEvaluation.스위치다운 = false;
                            //Next();
                        });
                        activationCylinderBoxPopup.selectSwitchDownBtn.onClick.RemoveAllListeners();
                        activationCylinderBoxPopup.selectSwitchDownBtn.onClick.AddListener(delegate
                        {
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);
                            activationCylinderBoxPopup.PressureSwitchDown(true);
                            SoundManager.Instance.StopAllFireSound();
                            activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(false);
                            activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(true);
                            GasSysGlobalCanvas.Instance.ShowLightTestOff(GasSysGlobalCanvas.Instance.HideCheckObj);
                            GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                            _gasSysEvaluation.스위치다운 = true;
                            //Next();
                        });
                    }
                    activationBoxBtn.onClick.AddListener(delegate
                    {
                        activationCylinderBoxPopup.closeBtn.gameObject.SetActive(true);
                        activationCylinderBoxPopup.Init();
                        activationCylinderBoxPopup.SetLightTestPos();
                        //ControlPanel.Instance.ShowPanel(true);
                        if (ControlPanel.Instance.CheckVerifyDischarge())
                            GasSysGlobalCanvas.Instance.ShowLightTestCheck(GasSysGlobalCanvas.Instance.HideCheckObj);
                        else
                        {
                            GasSysGlobalCanvas.Instance.ShowLightTestOff(GasSysGlobalCanvas.Instance.HideCheckObj);
                        }
                        GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                        if (!CheckVerifyDischarge())
                        {
                            activationCylinderBoxPopup.PressureSwitchDown();
                            // activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                            // activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                            activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(true);

                        }
                        else
                        {
                            activationCylinderBoxPopup.PressureSwitchUp();
                            activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(true);

                        }
                        activationCylinderBoxPopup.selectSwitchUpBtn.onClick.RemoveAllListeners();
                        activationCylinderBoxPopup.selectSwitchUpBtn.onClick.AddListener(delegate
                        {
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, true);
                            activationCylinderBoxPopup.PressureSwitchUp(true);
                            SoundManager.Instance.PlaySiren(true);
                            activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(false);
                            activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(true);
                            GasSysGlobalCanvas.Instance.ShowLightTestCheck(GasSysGlobalCanvas.Instance.HideCheckObj);
                            GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                            _gasSysEvaluation.스위치다운 = false;
                            //Next();
                        });
                        activationCylinderBoxPopup.selectSwitchDownBtn.onClick.RemoveAllListeners();
                        activationCylinderBoxPopup.selectSwitchDownBtn.onClick.AddListener(delegate
                        {
                            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);
                            activationCylinderBoxPopup.PressureSwitchDown(true);
                            SoundManager.Instance.StopAllFireSound();
                            activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(false);
                            activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(true);
                            GasSysGlobalCanvas.Instance.ShowLightTestOff(GasSysGlobalCanvas.Instance.HideCheckObj);
                            GasSysGlobalCanvas.Instance.GetCheckAgreeBtn().gameObject.SetActive(false);
                            _gasSysEvaluation.스위치다운 = true;
                            //Next();
                        });
                        activationCylinderBoxPopup.gameObject.SetActive(true);
                    });
                }
                break;
            case GasSysDischargeLightTestESection.평가종료:
                {
                    GasSysGlobalCanvas.Instance.totalScore.방출표시등.압력스위치작동 = _gasSysEvaluation.스위치업;
                    GasSysGlobalCanvas.Instance.totalScore.방출표시등.압력스위치복구 = _gasSysEvaluation.스위치다운;
                    GasSysGlobalCanvas.Instance.totalScore.방출표시등.동작확인 = !방출표시등토글1.isOn &&
                                                                        방출표시등토글2.isOn &&
                                                                        !방출표시등토글3.isOn;
                    var results = new List<ResultObject>
                    {
                        new ResultObject()
                        {
                            Title = "압력스위치 작동",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.방출표시등.압력스위치작동
                        },
                        new ResultObject()
                        {
                            Title = "압력스위치 복구",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.방출표시등.압력스위치복구
                        },
                        new ResultObject()
                        {
                            Title = "정상작동 상태확인",
                            IsSuccess = GasSysGlobalCanvas.Instance.totalScore.방출표시등.동작확인
                        }
                    };
                    //GasSysGlobalCanvas.Instance.SetResultPopup(results);
                     GasSysGlobalCanvas.Instance.totalScore.방출표시등List.Clear();
                     GasSysGlobalCanvas.Instance.totalScore.방출표시등List.AddRange(results);
                    GasSysGlobalCanvas.Instance.SetNextEvaluation(InitRecoveryCheck, InitDischargeLightTest);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void ChangeDischargeLightTestP(GasSysDischargeLightTestPSection state)
    {

        _curDischargeLightTestPSection = state;
        OnStateChangedDischargeLightTestP(_curDischargeLightTestPSection);
    }

    private void OnStateChangedDischargeLightTestP(GasSysDischargeLightTestPSection state)
    {
        InitStorageRoom();
        //ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
        //ControlPanel.Instance.InitTimer();
        //SoundManager.Instance.StopAllFireSound();
        //ControlPanel.Instance.ShowFire(false);
        uiDragAndCollisionHandler.StopDragging();
        ButtonManager.Instance.RemoveAllHighlights();
        //nextBtn.onClick.RemoveAllListeners();
        switch (state)
        {
            case GasSysDischargeLightTestPSection.방호구역1출입문소등:
                {
                    area1CorridorObj.SetDischarge(false);
                    NextEnable(false);
                }
                break;
            case GasSysDischargeLightTestPSection.수동조작함소등:
                {
                    manualControlBoxBtn.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificButton(manualControlBoxBtn);
                    manualControlBoxPopup.gameObject.SetActive(false);
                    NextDisable();
                    manualControlBoxBtn.onClick.RemoveAllListeners();
                    manualControlBoxBtn.onClick.AddListener(delegate
                    {
                        //GlobalCanvas.Instance.SetHintPopup(5,5, _manualControlBoxControllerHint, _manualControlBoxControllerHintRects[0]);
                        //GlobalCanvas.Instance.ShowHint(true);
                        manualControlBoxBtn.gameObject.SetActive(false);
                        Next();
                    });
                }
                break;
            case GasSysDischargeLightTestPSection.약제저장실:
                {
                    //SoundManager.Instance.PlaySiren(true);
                    //ShowRoom(area1CorridorObj.gameObject);
                    area1CorridorObj.SetDischarge(false);
                    manualControlBoxPopup.Init();
                    manualControlBoxPopup.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificButton(areaManagerObj.storageRoomBtn);
                    NextDisable();
                    areaManagerObj.storageRoomBtn.onClick.RemoveAllListeners();
                    areaManagerObj.storageRoomBtn.onClick.AddListener(delegate
                    {
                        manualControlBoxPopup.gameObject.SetActive(false);
                        Next();
                    });
                }
                break;

            case GasSysDischargeLightTestPSection.기동용기함선택:
                {
                    ShowRoom(storageRoomObj?.gameObject);
                    SoundManager.Instance.StopAllFireSound();
                    activationBoxBtn.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificButton(activationBoxBtn);
                    NextDisable();
                    activationBoxBtn.onClick.RemoveAllListeners();
                    activationBoxBtn.onClick.AddListener(delegate
                    {
                        activationBoxBtn.gameObject.SetActive(false);
                        //ControlPanel.Instance.ShowPanel(true);
                        Next();
                    });
                }
                break;
            case GasSysDischargeLightTestPSection.압력스위치업전:
                {
                    SoundManager.Instance.StopAllFireSound();
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.SoundCheck();
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
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
                        btn = activationCylinderBoxPopup.selectSwitchUpBtn,
                        select = false
                    });

                    activationCylinderBoxPopup.Init();
                    activationCylinderBoxPopup.PressureSwitchDown();
                    // activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                    // activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                    activationCylinderBoxPopup.gameObject.SetActive(true);
                    activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(true);
                    activationCylinderBoxPopup.selectSwitchUpBtn.onClick.AddListener(delegate
                    {
                        activationCylinderBoxPopup.PressureSwitchUp(true);
                        SoundManager.Instance.PlaySiren(true);
                        activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(false);
                        Next();
                    });

                    ButtonManager.Instance.EnableSpecificButton(btnList);
                    NextDisable();
                }
                break;
            case GasSysDischargeLightTestPSection.압력스위치업:
                {
                    SoundManager.Instance.PlaySiren(true);
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);
                    ControlPanel.Instance.ShowPanel(true);
                    ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.SoundCheck();
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
                    //ControlPanel.Instance.InitArea1Control(ControlMode.Manual);
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
                        btn = activationCylinderBoxPopup.selectSwitchUpBtn,
                        select = false
                    });
                    ButtonManager.Instance.EnableSpecificButton(btnList);

                    activationCylinderBoxPopup.Init();
                    // activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                    // activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                    activationCylinderBoxPopup.gameObject.SetActive(true);
                    activationCylinderBoxPopup.PressureSwitchUp();
                    NextEnable();
                    // activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(true);
                    // activationCylinderBoxPopup.selectSwitchUpBtn.onClick.AddListener(delegate
                    // {
                    //     activationCylinderBoxPopup.PressureSwitchUp();
                    //     SoundManager.Instance.PlaySiren(true);
                    //     activationCylinderBoxPopup.selectSwitchUpBtn.gameObject.SetActive(false);
                    // });

                }
                break;
            case GasSysDischargeLightTestPSection.방호구역1출입문:
                {
                    ShowRoom(storageRoomObj.gameObject);
                    SoundManager.Instance.PlaySiren(true);
                    ButtonManager.Instance.EnableSpecificButton(areaManagerObj.area1CorridorBtn);
                    NextDisable();
                    areaManagerObj.area1CorridorBtn.onClick.RemoveAllListeners();
                    areaManagerObj.area1CorridorBtn.onClick.AddListener(delegate
                    {
                        Next();
                    });
                }
                break;
            case GasSysDischargeLightTestPSection.방호구역1출입문점등:
                {
                    ShowRoom(area1CorridorObj?.gameObject);
                    SoundManager.Instance.PlaySiren(true);
                    area1CorridorObj?.SetDischarge(true);
                    ButtonManager.Instance.EnableSpecificButton();
                    manualControlBoxPopup.gameObject.SetActive(false);
                    NextEnable();
                }
                break;

            case GasSysDischargeLightTestPSection.수동조작함선택:
                {
                    SoundManager.Instance.PlaySiren(true);
                    area1CorridorObj.SetDischarge(true);
                    manualControlBoxPopup.Init();
                    manualControlBoxPopup.gameObject.SetActive(false);
                    ShowRoom(area1CorridorObj.gameObject);
                    manualControlBoxBtn.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificButton(manualControlBoxBtn);
                    NextDisable();
                    manualControlBoxBtn.onClick.RemoveAllListeners();
                    manualControlBoxBtn.onClick.AddListener(delegate
                    {
                        //GlobalCanvas.Instance.SetHintPopup(5,5, _manualControlBoxControllerHint, _manualControlBoxControllerHintRects[0]);
                        //GlobalCanvas.Instance.ShowHint(true);
                        manualControlBoxBtn.gameObject.SetActive(false);
                        Next();
                    });
                }
                break;
            case GasSysDischargeLightTestPSection.수동조작함방출등확인:
                {
                    SoundManager.Instance.PlaySiren(true);
                    area1CorridorObj.SetDischarge(true);
                    manualControlBoxPopup.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificButton();
                    NextEnable();
                    //manualControlBoxPopup.selectOpenBtn.gameObject.SetActive(true);
                }
                break;
            case GasSysDischargeLightTestPSection.저장용기실:
                {
                    SoundManager.Instance.PlaySiren(true);
                    ShowRoom(area1CorridorObj.gameObject);
                    manualControlBoxPopup.gameObject.SetActive(false);
                    ButtonManager.Instance.EnableSpecificButton(areaManagerObj.storageRoomBtn);
                    NextDisable();
                    areaManagerObj.storageRoomBtn.onClick.RemoveAllListeners();
                    areaManagerObj.storageRoomBtn.onClick.AddListener(delegate
                    {
                        Next();
                    });
                }
                break;
            case GasSysDischargeLightTestPSection.기동용기함선택2:
                {
                    SoundManager.Instance.PlaySiren(true);
                    ShowRoom(storageRoomObj.gameObject);
                    activationBoxBtn.gameObject.SetActive(true);
                    ButtonManager.Instance.EnableSpecificButton(activationBoxBtn);
                    NextDisable();
                    activationBoxBtn.onClick.AddListener(delegate
                    {
                        activationBoxBtn.gameObject.SetActive(false);
                        //ControlPanel.Instance.ShowPanel(true);
                        Next();
                    });
                }
                break;
            case GasSysDischargeLightTestPSection.압력스위치다운전:
                {
                    SoundManager.Instance.PlaySiren(true);
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.SoundCheck();
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
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
                        btn = activationCylinderBoxPopup.selectSwitchDownBtn,
                        select = false
                    });
                    activationCylinderBoxPopup.Init();
                    // activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                    // activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                    activationCylinderBoxPopup.gameObject.SetActive(true);
                    activationCylinderBoxPopup.PressureSwitchUp();
                    activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(true);
                    activationCylinderBoxPopup.selectSwitchDownBtn.onClick.AddListener(delegate
                    {
                        activationCylinderBoxPopup.PressureSwitchDown(true);
                        SoundManager.Instance.StopAllFireSound();
                        activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(false);
                        Next();
                    });

                    ButtonManager.Instance.EnableSpecificButton(btnList);
                    NextDisable();
                }
                break;
            case GasSysDischargeLightTestPSection.압력스위치다운:
                {
                    SoundManager.Instance.PlaySiren(false);
                    var btnList = new List<SwitchButtonCheck>();
                    ControlPanel.Instance.ShowPanel(true);
                    // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
                    // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
                    ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(true);
                    ControlPanel.Instance.SoundCheck();
                    ControlPanel.Instance.InitSolenoidValveControl(ControlMode.Stop);
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
                        btn = activationCylinderBoxPopup.selectSwitchDownBtn,
                        select = false
                    });
                    activationCylinderBoxPopup.Init();
                    // activationCylinderBoxPopup.solenoidValveObj.SetActive(true);
                    // activationCylinderBoxPopup.aActivationPinObj.SetActive(true);
                    activationCylinderBoxPopup.gameObject.SetActive(true);
                    activationCylinderBoxPopup.PressureSwitchDown();
                    // activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(true);
                    // activationCylinderBoxPopup.selectSwitchDownBtn.onClick.AddListener(delegate
                    // {
                    //     activationCylinderBoxPopup.PressureSwitchDown();
                    //     SoundManager.Instance.StopAllFireSound();
                    //     activationCylinderBoxPopup.selectSwitchDownBtn.gameObject.SetActive(false);
                    //     
                    // });

                    ButtonManager.Instance.EnableSpecificButton(btnList);
                    NextEnable();
                }
                break;
            case GasSysDischargeLightTestPSection.교육종료:
                {
                    SetCompletePopup("방출표시등 작동시험을 모두 완료했습니다.", "점검완료 후 복구를", InitRecoveryCheck);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }



}
