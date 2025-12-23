using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using VInspector;

public partial class GasSysISection
{
    [Foldout("자동")]
    private GasSysIOperationAutoSection _curOperationAuto;
    public HintScriptableObj operationAutoHintObj;
    public CinemachineVirtualCamera[] autoCamPosObjs;
    [EndFoldout]
    public void InitOperationAuto()
    {
        Debug.Log(autoCamPosObjs.Length);
        Init();
        IsAuto(true);
        _camList.AddRange(autoCamPosObjs);
        Debug.Log(_camList.Count);
        //SetCamPos(_camList[0].gameObject);
        
        //_camList[0].gameObject.SetActive(true);
        //autoObj.SetActive(true);
        //manualObj.SetActive(false);
        curGasSysIState = GasSysIState.감시기작동오토;
        _hintPanel.Init(operationAutoHintObj, NextAni, PrevAni);
        //_hintPanel.prevBtn.onClick.AddListener(PrevAni);
        // _hintPanel.prevBtn.onClick.AddListener(ResetSolAutoAni);
        //_hintPanel.nextBtn.onClick.AddListener(NextAni);
        // _hintPanel.nextBtn.onClick.AddListener(DefaultSolAutoAni);
        _hintPanel.ShowHint(true);
        _controlPanel.ShowPanel(false);
        int maxSection = Enum.GetValues(typeof(GasSysIOperationAutoSection)).Length;
        _hintPanel.SetSectionRange(0, maxSection, maxSection);
       


    }

    private void ShowOperationAuto(int index)
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
        ChangeOperationAuto(GasSysIOperationAutoSection.전체 + index);
    }

    private void ChangeOperationAuto(GasSysIOperationAutoSection state)
    {
        _curOperationAuto = state;
        OnStateChangeOperationAuto(_curOperationAuto);

    }

    private void OnStateChangeOperationAuto(GasSysIOperationAutoSection state)
    {
        OnBlendComplete?.RemoveAllListeners();
        OnAnimationComplete?.RemoveAllListeners();
        //ResetGasAni();
        //ResetSolAutoAni();
        _disposable.Clear();
        switch (state)
        {

            case GasSysIOperationAutoSection.전체:
                {
                    fireObj.SetActive(false);
                    SetDetectorEmissionOrLight(false);
                    CheckCamIsBlending(_camList[0].gameObject);
                    열감지기Popup.SetActive(false);
                    연기감지기Popup.SetActive(false);
                    _soundManager.SetAlarmVolume(0f);
                    _soundManager.SetAlarm2Volume(0f);
                    _soundManager.SetBroadcastVolume(0f);
                    _soundManager.SetSirenVolume(0f);
                    _soundManager.SetBuzzerVolume(0f);
                    _controlPanel.DisableAllButton();
                    //ResetAniState();
                }
                break;
            case GasSysIOperationAutoSection.화재발생:
                {
                    CheckCamIsBlending(_camList[0].gameObject);
                    fireObj.SetActive(true);
                }
                break;
            case GasSysIOperationAutoSection.감지기작동:
                {
                    _controlPanel.ShowPanel(false);
                    //ResetAniState();
                    CheckCamIsBlending(_camList[1].gameObject, delegate
                    {
                        fireObj.SetActive(true);
                        열감지기Popup.SetActive(true);
                        연기감지기Popup.SetActive(true);
                        var disposable = Observable.Timer(System.TimeSpan.FromSeconds(1))
                            .Subscribe(_ =>
                            {
                                _soundManager.SetAlarmVolume();
                                _soundManager.SetAlarm2Volume();
                                _soundManager.SetBroadcastVolume();
                                _soundManager.SetSirenVolume();
                                _soundManager.SetBuzzerVolume(0f);
                                SetDetectorEmissionOrLight(true);

                            }).AddTo(this);
                        _disposable.Add(disposable);
                    });
                    // cinemachineBrain.m_CameraActivatedEvent.AddListener((fromCamera, toCamera) =>
                    // {
                    //     if (camPosObjs[1].Equals(fromCamera))
                    //     {
                    //         fireObj.SetActive(true);
                    //         SetDetectorEmissionOrLight(true);
                    //     }
                    // });
                    //SetAutoCamPos(autoCamPosObjs[1].gameObject);
                }
                break;
            case GasSysIOperationAutoSection.감시제어반출력:
                {
                    _soundManager.SetAlarmVolume();
                    _soundManager.SetAlarm2Volume();
                    _soundManager.SetBroadcastVolume();
                    _soundManager.SetSirenVolume();
                    _soundManager.SetBuzzerVolume(0f);
                    열감지기Popup.SetActive(false);
                    연기감지기Popup.SetActive(false);
                    _controlPanel.ShowPanel(false);
                    _controlPanel.ShowFire(false, false);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector1, false);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                    //ResetAniState();
                    CheckCamIsBlending(_camList[2].gameObject, delegate
                    {
                        var t = Observable.Timer(System.TimeSpan.FromSeconds(0.2f))
                            .Subscribe(_ =>
                            {
                                _soundManager.SetAlarmVolume(0.01f);
                                _soundManager.SetAlarm2Volume(0.005f);
                                _soundManager.SetBroadcastVolume(0.05f);
                                _soundManager.SetSirenVolume(0.01f);
                                _soundManager.SetBuzzerVolume(0f);
                                SetDetectorEmissionOrLight(true);

                                _controlPanel.ShowPanel(true);
                                _controlPanel.ShowFire(true, false);
                                _controlPanel.InitTimer();
                                _controlPanel.StartTimer(30);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector2, true);

                            }).AddTo(this);
                        _disposable.Add(t);
                    });
                    _curSolAutoAniState = null;
                }
                break;
                    
            /*
            case GasSysIOperationAutoSection.기동용기함개방:
                {
                    // _soundManager.SetAlarmVolume(0);
                    // _soundManager.SetAlarm2Volume(0);
                    // _soundManager.SetBroadcastVolume(0);
                    // _soundManager.SetSirenVolume(0);
                    _controlPanel.ShowPanel(false);
                    solPopup.SetActive(false);
                    int indexAni = 2;
                    //ResetAniState();
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        PlaySolAutoAni(solAutoClips[0]);
                    });
                    //
                    // PlaySolAutoAni(solAutoClips[0]).Stop();
                    // if (CheckAutoCamIsBlending(camPosObjs[3].gameObject, delegate
                    //     {
                    //         PlaySolAutoAni(solAutoClips[0]);
                    //     }))
                    // {
                    //     PlaySolAutoAni(solAutoClips[0]);
                    // }
                    //

                    // if (_curCamPos.Equals(camPosObjs[3].gameObject))
                    // {
                    //     PlaySolAutoAni(solAutoClips[0]);
                    // }
                    // else
                    // {
                    //     OnBlendComplete?.AddListener(delegate
                    //     {
                    //     
                    //         //gasAnimation = gasAnimation.GetClip("New_Door_Open_2");
                    //         PlaySolAutoAni(solAutoClips[0]);
                    //
                    //     });
                    // }
                    //SetAutoCamPos(camPosObjs[2].gameObject);
                }
                break;
                */
            // case GasSysIOperationAutoSection.기동용기함개방완료:
            //     {
            //         PlaySolAutoAni(solAutoClips[1]);
            //         // OnAnimationComplete?.AddListener((clipName) =>
            //         // {
            //         //     if (clipName.Equals(solAutoClips[1].name))
            //         //         return;
            //         //     
            //         //     PlaySolAutoAni(solAutoClips[1]);
            //         // });
            //   
            //         //gasAnimation.Play("New_Solenoid_Action");
            //         SetAutoCamPos(camPosObjs[3].gameObject);
            //     }
            //     break;
            case GasSysIOperationAutoSection.솔밸브동작:
                {
                    // if ()
                    // {
                    //     PlaySolAutoAni(solAutoClips[1]);
                    // }
                    _controlPanel.ShowPanel(false);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        PlaySolAutoAni(solAutoClips[0]);
                        //solAutoClips[1].normalizedTime = 1f;
                    });
                    // OnAnimationComplete?.AddListener((clipName) =>
                    // {
                    //     if (clipName.Equals(solAutoClips[1].name))
                    //         return;
                    //     
                    //     PlaySolAutoAni(solAutoClips[1]);
                    // });

                    //gasAnimation.Play("New_Solenoid_Action");

                }
                break;
            case GasSysIOperationAutoSection.제어반표시등점등:
                {
                    _controlPanel.ShowPanel(false);
                  _controlPanel.ShowFire(true, false);
                    CheckCamIsBlending(_camList[2].gameObject, delegate
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
                              _controlPanel.ShowFire(true, false);
                                _controlPanel.InitTimer();
                                _controlPanel.StartTimer(0);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector2, true);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);

                            }).AddTo(this);
                        _disposable.Add(t);
                    });
                    // OnBlendComplete?.AddListener(delegate
                    // {
                    //     //fireObj.SetActive(true);
                    //     var t =Observable.Timer(System.TimeSpan.FromSeconds(0.3f))
                    //         .Subscribe(_ =>
                    //         {
                    //             _soundManager.SetAlarmVolume(0.2f);
                    //             _soundManager.SetAlarm2Volume(0.2f);
                    //             _soundManager.SetBroadcastVolume(0.2f);
                    //             _soundManager.SetSirenVolume(0.2f);
                    //             SetDetectorEmissionOrLight(true);
                    //             
                    //             _controlPanel.ShowPanel(true);
                    //           _controlPanel.ShowFire(true, false);
                    //             _controlPanel.InitTimer();
                    //             _controlPanel.StartTimer(0);
                    //             _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                    //             _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector2, true);
                    //             _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                    //
                    //         }).AddTo(this);
                    //     _disposable.Add(t);
                    // });
                }
                break;
            // case GasSysIOperationAutoSection.기동용기에가스동관이동:
            //     {
            //         _controlPanel.ShowPanel(false);
            //         //_controlPanel.ShowFire(true);
            //         CheckAutoCamIsBlending(camPosObjs[4].gameObject, delegate
            //         {
            //             PlayGasAni(gasAutoClips[0]);
            //         });
            //     }
            //     break;
            // case GasSysIOperationAutoSection.가스선택밸브로이동:
            //     break;
            case GasSysIOperationAutoSection.선택밸브개방:
                {
                    _controlPanel.ShowPanel(false);
                    //_controlPanel.ShowFire(true);
                    CheckCamIsBlending(_camList[4].gameObject, delegate
                    {
                        PlayGasAni(gasAutoClips[0]);
                    });

                }
                break;
            case GasSysIOperationAutoSection.선택밸브에서약제저장실이동:
                {
                    _controlPanel.ShowPanel(false);
                    //_controlPanel.ShowFire(true);
                    CheckCamIsBlending(_camList[5].gameObject, delegate
                    {
                        PlayGasAni(gasAutoClips[1]);
                    });

                }
                break;
            case GasSysIOperationAutoSection.저장용기개방:
                {
                    _controlPanel.ShowPanel(false);
                    //_controlPanel.ShowFire(true);
                    CheckCamIsBlending(_camList[6].gameObject, delegate
                    {
                        PlayGasAni(gasAutoClips[2]);
                    });
                }
                break;
            case GasSysIOperationAutoSection.집합관으로이동:
                {
                    _controlPanel.ShowPanel(false);
                    //_controlPanel.ShowFire(true);
                    CheckCamIsBlending(_camList[7].gameObject, delegate
                    {
                        PlayGasAni(gasAutoClips[3]);
                    });
                }
                break;
            // case GasSysIOperationAutoSection.집합관선택밸브:
            //     break;
            case GasSysIOperationAutoSection.압력스위치온:
                {
                    SetManualBoxReleaseOn(false);
                    _controlPanel.ShowPanel(false);
                    //_controlPanel.ShowFire(true);
                    CheckCamIsBlending(_camList[8].gameObject, delegate
                    {
                        PlayGasAni(gasAutoClips[4]);
                    });
                }
                break;
            case GasSysIOperationAutoSection.방출표시등점등:
                {
                    _controlPanel.ShowPanel(false);
                  _controlPanel.ShowFire(true, false);
                    SetManualBoxReleaseOn(true);
                    CheckCamIsBlending(_camList[2].gameObject, delegate
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
                              _controlPanel.ShowFire(true, false);
                                _controlPanel.InitTimer();
                                _controlPanel.StartTimer(0);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector2, true);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, true);

                            }).AddTo(this);
                        _disposable.Add(t);
                    });
                }
                break;
            case GasSysIOperationAutoSection.수동조작함가스방출표시등:
                {
                    _controlPanel.ShowPanel(false);
                    SetDischargeOn(false);
                    SetManualBoxReleaseOn(false);
                    CheckCamIsBlending(_camList[9].gameObject, delegate
                    {
                        var t = Observable.Timer(System.TimeSpan.FromSeconds(0.5f))
                            .Subscribe(_ =>
                            {
                                //ResetAniState();
                                SetDischargeOn(true);
                                SetManualBoxReleaseOn(true);

                            }).AddTo(this);
                        _disposable.Add(t);
                    });
                }
                break;
            case GasSysIOperationAutoSection.제어반:
                {
                    _controlPanel.ShowPanel(false);
                    //SetDischargeOn(false);
                    IsRelease2On(false);
                    fireObj.SetActive(true);
                    CheckCamIsBlending(_camList[10].gameObject, delegate
                    {
                        PlayGasAni(gasAutoClips[5]);

                    });
                }
                break;
            case GasSysIOperationAutoSection.가스방출화재진압:
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
            case GasSysIOperationAutoSection.실습완료:
                {
                    SetCompletePopup("자동기동방식(감지기 작동)에 의한 화재진압을 완료했습니다.");
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void SetAutoCamPos(GameObject obj)
    {
        foreach (var camPos in autoCamPosObjs)
        {
            camPos.gameObject.SetActive(camPos.gameObject.Equals(obj));
        }
        _curCamPos = obj;
    }

    // private bool CheckAutoCamIsBlending(GameObject obj, Action action = null)
    // {
    //     if (_curCamPos.Equals(obj))
    //     {
    //         action?.Invoke();
    //         return _curCamPos.Equals(obj);
    //     }
    //     if (action != null)
    //         OnBlendComplete.AddListener(action.Invoke);
    //
    //     SetAutoCamPos(obj);
    //     return _curCamPos.Equals(obj);
    // }

    /*
    private void CheckAutoCamIsBlending(GameObject obj, Action action = null)
    {
        if (_curCamPos.Equals(obj))
        {
            action?.Invoke();
            return;
        }
        if (action != null)
            OnBlendComplete.AddListener(action.Invoke);

        SetAutoCamPos(obj);
    }
    */

}
