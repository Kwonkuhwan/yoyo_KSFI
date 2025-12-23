using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UniRx;
using UnityEngine;
using VInspector;

public partial class GasSysISection
{
    [Foldout("수동")]
    private GasSysIOperationManualSection _curOperationManual;
    public HintScriptableObj operationManualHintObj;
    public CinemachineVirtualCamera[] manualCamPosObjs;
    [EndFoldout]
    public void InitOperationManual()
    {
        Init();
        IsAuto(true);
        _camList.AddRange(manualCamPosObjs);
        //_camList[0].gameObject.SetActive(true);
        //SetCamPos(_camList[0].gameObject);
        curGasSysIState = GasSysIState.수동조작함작동수동;
        _hintPanel.Init(operationManualHintObj, NextAni, PrevAni);
        int maxSection = Enum.GetValues(typeof(GasSysIOperationManualSection)).Length;
        _hintPanel.SetSectionRange(0, maxSection, maxSection);
        _hintPanel.ShowHint(true);
        _controlPanel.ShowPanel(false);
        //autoObj.SetActive(false);
        //manualObj.SetActive(true);
        

    }
    
    private void ShowOperationManual(int index)
    {
        // if (_.Equals(GasSysState.PracticeMode))
        // {
        //     prevBtn.gameObject.SetActive(true);
        //     GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(manualOperationHintPObj, (int)GasSysManualOperationPSection.감시제어반선택 + index));
        //     ChangeStateManualOperationP(GasSysManualOperationPSection.감시제어반선택 + index);
        // }
        // if (!_gasSysState.Equals(GasSysState.EvaluationMode))
        //     return;
        // prevBtn.gameObject.SetActive(false);
        // GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(manualOperationHintEObj, (int)GasSysSafetyCheckESection.Init + index));
        // ChangeStateManualOperationE(GasSysManualOperationESection.Init + index);
        //GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(manualOperationHintPObj, (int)GasSysManualOperationPSection.감시제어반선택 + index));
        ChangeOperationManual(GasSysIOperationManualSection.전체 + index);
    }

    private void ChangeOperationManual(GasSysIOperationManualSection state)
    {
        _curOperationManual = state;
        OnStateChangeOperationManual(_curOperationManual);
        
    }

    private void OnStateChangeOperationManual(GasSysIOperationManualSection state)
    {
        OnBlendComplete?.RemoveAllListeners();
        OnAnimationComplete?.RemoveAllListeners();
        ShowFingerObj(null);
        _disposable.Clear();
        switch (state)
        {

            case GasSysIOperationManualSection.전체:
                {
                    _soundManager.SetAlarmVolume(0);
                    _soundManager.SetAlarm2Volume(0);
                    _soundManager.SetBroadcastVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _soundManager.SetBuzzerVolume(0);
                    fireObj.SetActive(false);
                    SetDetectorEmissionOrLight(false);
                    CheckCamIsBlending(_camList[0].gameObject);
                    열감지기Popup.SetActive(false);
                    연기감지기Popup.SetActive(false);
                    _controlPanel.DisableAllButton();
                    //ResetAniState();
                }
                break;
            case GasSysIOperationManualSection.화재발생:
                {
                    CheckCamIsBlending(_camList[0].gameObject);
                    fireObj.SetActive(true);
                }
                break;
            case GasSysIOperationManualSection.화재발생확인:
                {
                    _soundManager.SetAlarmVolume(0);
                    _soundManager.SetAlarm2Volume(0);
                    _soundManager.SetBroadcastVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _soundManager.SetBuzzerVolume(0);
                    _controlPanel.ShowPanel(false);
                    //ResetAniState();
                    PlayManualOpAni(manualOpClips[0]);
                    CheckCamIsBlending(_camList[1].gameObject, delegate
                    {
                        fireObj.SetActive(true);
                        //열감지기Popup.SetActive(true);
                        //연기감지기Popup.SetActive(true);
                        // var disposable = Observable.Timer(System.TimeSpan.FromSeconds(1))
                        //     .Subscribe(_ =>
                        //     {
                        //         _soundManager.SetAlarmVolume(0.2f);
                        //         _soundManager.SetAlarm2Volume(0.2f);
                        //         _soundManager.SetBroadcastVolume(0.2f);
                        //         _soundManager.SetSirenVolume(0.2f);
                        //         SetDetectorEmissionOrLight(true);
                        //
                        //     }).AddTo(this);
                        // _disposable.Add(disposable);
                    });
                    // cinemachineBrain.m_CameraActivatedEvent.AddListener((fromCamera, toCamera) =>
                    // {
                    //     if (camPosObjs[1].Equals(fromCamera))
                    //     {
                    //         fireObj.SetActive(true);
                    //         SetDetectorEmissionOrLight(true);
                    //     }
                    // });
                }
                break;
            /*
            case GasSysIOperationManualSection.수동조작함개방:
                {
                    //ResetAniState();
                    SetManualBoxDoorOn(false);
                    SetManualBoxReleaseOn(false);
                    _soundManager.SetAlarmVolume(0);
                    _soundManager.SetAlarm2Volume(0);
                    _soundManager.SetBroadcastVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _soundManager.SetBuzzerVolume(0);
                    _controlPanel.ShowPanel(false);
                    CheckCamIsBlending(_camList[2].gameObject, delegate
                    {
                        //PlaySolManualAni()
                        _soundManager.SetBuzzerVolume(0.1f);
                        SetManualBoxDoorOn(true);
                        PlayManualOpAni(manualOpClips[1]);
                    });
                }
                break;
                */
            case GasSysIOperationManualSection.수동조작함방출:
                {
                    SetManualBoxDoorOn(true);
                    SetManualBoxReleaseOn(false);
                    _soundManager.SetAlarmVolume(0);
                    _soundManager.SetAlarm2Volume(0);
                    _soundManager.SetBroadcastVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _soundManager.SetBuzzerVolume(0);
                    _controlPanel.ShowPanel(false);
                    CheckCamIsBlending(_camList[2].gameObject, delegate
                    {
                        //PlaySolManualAni()
                        PlayManualOpAni(manualOpClips[5]);
                        var t = Observable.Timer(System.TimeSpan.FromSeconds(1.3f))
                            .Subscribe(_ =>
                            {
                                _soundManager.SetAlarmVolume(0.01f);
                                _soundManager.SetAlarm2Volume(0.005f);
                                _soundManager.SetBroadcastVolume(0.05f);
                                _soundManager.SetSirenVolume(0.03f);
                                _soundManager.SetBuzzerVolume(0.01f);
                                ShowFingerObj(fingerObjs[11]);
                                //SetManualBoxReleaseOn(true);
                            }).AddTo(this);
                        _disposable.Add(t);
                    });
                }
                break;
            case GasSysIOperationManualSection.감시제어반출력:
                {
                    SetManualBoxDoorOn(true);
                    //SetManualBoxReleaseOn(true);
                    _controlPanel.ShowPanel(false);
                    _controlPanel.ShowFire(false);
                    _soundManager.SetBuzzerVolume(0f);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        var t = Observable.Timer(System.TimeSpan.FromSeconds(0.3f))
                            .Subscribe(_ =>
                            {
                                // _soundManager.SetAlarmVolume(0.2f);
                                // _soundManager.SetAlarm2Volume(0.2f);
                                // _soundManager.SetBroadcastVolume(0.2f);
                                // _soundManager.SetSirenVolume(0.2f);
                                SetDetectorEmissionOrLight(true);

                                _controlPanel.ShowPanel(true);
                                _controlPanel.ShowFire(true);
                                _controlPanel.InitTimer();
                                _controlPanel.StartTimer(30);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);

                            }).AddTo(this);
                        _disposable.Add(t);
                    });
                }
                break;
            /*
            case GasSysIOperationManualSection.기동용기함개방:
                {
                    // _soundManager.SetAlarmVolume(0);
                    // _soundManager.SetAlarm2Volume(0);
                    // _soundManager.SetBroadcastVolume(0);
                    // _soundManager.SetSirenVolume(0);
                    // _soundManager.SetBuzzerVolume(0);
                    _controlPanel.ShowPanel(false);
                    CheckCamIsBlending(_camList[4].gameObject, delegate
                    {
                        PlaySolAutoAni(solAutoClips[0]);
                    });
                }
                break;
                */
            case GasSysIOperationManualSection.솔밸브동작:
                {
                    _controlPanel.ShowPanel(false);
                    CheckCamIsBlending(_camList[4].gameObject, delegate
                    {
                        PlaySolAutoAni(solAutoClips[0]);
                        //solAutoClips[1].normalizedTime = 1f;
                    });
                }
                break;
            case GasSysIOperationManualSection.제어반표시등점등:
                {
                    _controlPanel.ShowPanel(false);
                    _controlPanel.ShowFire(true);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        var t = Observable.Timer(System.TimeSpan.FromSeconds(0.1f))
                            .Subscribe(_ =>
                            {
                                // _soundManager.SetAlarmVolume(0.2f);
                                // _soundManager.SetAlarm2Volume(0.2f);
                                // _soundManager.SetBroadcastVolume(0.2f);
                                // _soundManager.SetSirenVolume(0.2f);
                                SetDetectorEmissionOrLight(true);

                                _controlPanel.ShowPanel(true);
                                _controlPanel.ShowFire(true);
                                _controlPanel.InitTimer();
                                _controlPanel.StartTimer(0);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);

                            }).AddTo(this);
                        _disposable.Add(t);
                    });
                }
                break;
            case GasSysIOperationManualSection.선택밸브개방:
                {
                    _controlPanel.ShowPanel(false);
                    //_controlPanel.ShowFire(true);
                    CheckCamIsBlending(_camList[5].gameObject, delegate
                    {
                        PlayGasAni(gasAutoClips[0]);
                    });
                }
                break;
            case GasSysIOperationManualSection.선택밸브에서약제저장실이동:
                {
                    _controlPanel.ShowPanel(false);
                    CheckCamIsBlending(_camList[6].gameObject, delegate
                    {
                        PlayGasAni(gasAutoClips[1]);
                    });

                }
                break;
            case GasSysIOperationManualSection.저장용기개방:
                {
                    _controlPanel.ShowPanel(false);
                    //_controlPanel.ShowFire(true);
                    CheckCamIsBlending(_camList[7].gameObject, delegate
                    {
                        PlayGasAni(gasAutoClips[2]);
                    });
                }
                break;
            case GasSysIOperationManualSection.집합관으로이동:
                {
                    _controlPanel.ShowPanel(false);
                    //_controlPanel.ShowFire(true);
                    CheckCamIsBlending(_camList[8].gameObject, delegate
                    {
                        PlayGasAni(gasAutoClips[3]);
                    });
                }
                break;
            case GasSysIOperationManualSection.압력스위치온:
                {
                    _controlPanel.ShowPanel(false);
                    //_controlPanel.ShowFire(true);
                    CheckCamIsBlending(_camList[9].gameObject, delegate
                    {
                        PlayGasAni(gasAutoClips[4]);
                    });
                }
                
                break;
            case GasSysIOperationManualSection.방출표시등점등:
                {
                    _controlPanel.ShowPanel(false);
                    _controlPanel.ShowFire(true);
                    _soundManager.SetBuzzerVolume(0f);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        var t = Observable.Timer(System.TimeSpan.FromSeconds(0.1f))
                            .Subscribe(_ =>
                            {
                                // _soundManager.SetAlarmVolume(0.2f);
                                // _soundManager.SetAlarm2Volume(0.2f);
                                // _soundManager.SetBroadcastVolume(0.2f);
                                // _soundManager.SetSirenVolume(0.2f);
                                SetDetectorEmissionOrLight(true);

                                _controlPanel.ShowPanel(true);
                                _controlPanel.ShowFire(true);
                                _controlPanel.InitTimer();
                                _controlPanel.StartTimer(0);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, true);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);

                            }).AddTo(this);
                        _disposable.Add(t);
                    });
                }
                break;
            case GasSysIOperationManualSection.수동조작함가스방출표시등:
                {
                    _controlPanel.ShowPanel(false);
                    SetDischargeOn(false);
                    SetManualBoxReleaseOn(false);
                    CheckCamIsBlending(_camList[10].gameObject, delegate
                    {
                        var t = Observable.Timer(System.TimeSpan.FromSeconds(0.5f))
                            .Subscribe(_ =>
                            {
                                //ResetAniState();
                                _soundManager.SetBuzzerVolume(0.01f);
                                SetDischargeOn(true);
                                SetManualBoxReleaseOn(true);

                            }).AddTo(this);
                        _disposable.Add(t);
                    });
                }
                break;
            case GasSysIOperationManualSection.제어반:
                {
                    _soundManager.SetBuzzerVolume(0f);
                    _controlPanel.ShowPanel(false);
                    //SetDischargeOn(false);
                    IsRelease2On(false);
                    fireObj.SetActive(true);
                    CheckCamIsBlending(_camList[11].gameObject, delegate
                    {
                        PlayGasAni(gasAutoClips[5]);

                    });
                }
                break;
            case GasSysIOperationManualSection.가스방출화재진압:
                {
                    _controlPanel.ShowPanel(false);
                    CheckCamIsBlending(_camList[1].gameObject, delegate
                    {
                        IsRelease2On(true);
                        var t = Observable.Timer(System.TimeSpan.FromSeconds(2f))
                            .Subscribe(_ =>
                            {
                                fireObj.SetActive(false);
                            }).AddTo(this);
                        _disposable.Add(t);
                    });
                }
                break;
            case GasSysIOperationManualSection.실습완료:
                {
                    SetCompletePopup("수동기동방식(수동조작함 작동)에 의한 화재진압을 완료했습니다.");
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    
    public void SetManualCamPos(GameObject obj)
    {
        foreach (var camPos in manualCamPosObjs)
        {
            camPos.gameObject.SetActive(camPos.gameObject.Equals(obj));
        }
        _curCamPos = obj;
    }
}
