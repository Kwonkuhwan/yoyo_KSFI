using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using VInspector;

/// <summary>
/// 점검 전 안전조치
/// </summary>
public partial class GasSysISection
{
    [Foldout("점검")]
    private GasSysICheckSection _curSafetyCheck;
    [FormerlySerializedAs("safetyCheckHintObj")]
    public HintScriptableObj checkHintObj;
    [FormerlySerializedAs("safetyCamPosObjs")]
    public CinemachineVirtualCamera[] checkCamPosObjs;
    public GameObject heatCheckObj;
    public GameObject smokeCheckObj;
    [EndFoldout]
    private float _interval = 1f;
    public void InitCheck()
    {
        Init();
        IsAuto(false);
        _camList.AddRange(checkCamPosObjs);
        //SetCamPos(_camList[0].gameObject);
        //_camList[0].gameObject.SetActive(true);
        curGasSysIState = GasSysIState.점검;
        _hintPanel.Init(checkHintObj, NextAni, PrevAni);
        _hintPanel.ShowHint(true);
        _controlPanel.ShowPanel(false);
        int maxSection = Enum.GetValues(typeof(GasSysICheckSection)).Length;
        _hintPanel.SetSectionRange(0, maxSection, maxSection);

    }

    private void ShowSafetyCheck(int index)
    {
        ChangeSafetyCheck(GasSysICheckSection.전체 + index);
    }

    private void ChangeSafetyCheck(GasSysICheckSection state)
    {
        _curSafetyCheck = state;
        OnStateChangeSafetyCheck(_curSafetyCheck);
    }
    
    void ExecuteAfterDelay(float delay, System.Action callback)
    {
        DOVirtual.DelayedCall(delay, () => callback?.Invoke());
    }

    private void OnStateChangeSafetyCheck(GasSysICheckSection state)
    {
        OnBlendComplete?.RemoveAllListeners();
        OnAnimationComplete?.RemoveAllListeners();
        ControlPanel.OnTimerEnd?.RemoveAllListeners();
        연기감지기Popup.SetActive(false);
        열감지기Popup.SetActive(false);
        solPopup.SetActive(false);
        sol2Popup.SetActive(false);
        _controlPanel.ShowPanel(false);
        //ResetGasAni();
        //ResetSolAutoAni();
        _disposable.Clear();
        ShowFingerObj(null);
        heatAni.Stop();
        heatCheckObj.SetActive(false);
        smokeAni.Stop();
        smokeCheckObj.SetActive(false);
        switch (state)
        {
            case GasSysICheckSection.전체:
                {
                    fireObj.SetActive(false);
                    SetDetectorEmissionOrLight(false);
                    CheckCamIsBlending(_camList[0].gameObject);
                    열감지기Popup.SetActive(false);
                    연기감지기Popup.SetActive(false);
                    _controlPanel.DisableAllButton();
                }
                break;

            case GasSysICheckSection.선택밸브동관분리:
                {
                    CheckCamIsBlending(_camList[1].gameObject, delegate
                    {
                        PlayValveAni(valveClips[0]);
                    });
                }
                break;
            case GasSysICheckSection.저장용기동관분리:
                {
                    CheckCamIsBlending(_camList[2].gameObject, delegate
                    {
                        PlayPipeAni(pipeClips[1]);
                    });
                    _controlPanel.ShowPanel(false);
                }
                break;
            case GasSysICheckSection.음향정지전:
                {
                    _controlPanel.ShowPanel(false);
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    _controlPanel.InitSolenoidValveControl(ControlMode.Auto);
                    foreach (var switchBtn in switchBtnList)
                    {
                        switchBtn.OnCheck(false);
                    }
                    _controlPanel.CheckWarringSwitch();

                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                        // var t = Observable.Interval(TimeSpan.FromSeconds(_interval))
                        //     .Take(6) // UIImage 개수만큼 반복
                        //     .Subscribe(index =>
                        //         {
                        //             ShowFingerObj(fingerObjs[index]);
                        //             switchBtnList[(int)index].OnCheck(true); // 해당 인덱스의 UIImage 활성화
                        //             _controlPanel.CheckWarringSwitch();
                        //         },
                        //         () =>
                        //         {
                        //             ShowFingerObj(null);
                        //             Debug.Log("모든 이미지가 활성화되었습니다."); // 완료 메시지
                        //         })
                        //     .AddTo(this); // GameObject와 생명주기 연동
                        var t = Observable.Interval(TimeSpan.FromSeconds(_interval))
                            .Take(6) // UIImage 개수만큼 반복
                            .Do(index =>
                            {
                                ShowFingerObj(fingerObjs[index]);
                                switchBtnList[(int)index].OnCheck(true); // 해당 인덱스의 UIImage 활성화
                                ExecuteAfterDelay(_interval*0.3f, ()=>_controlPanel.CheckWarringSwitch());
                            }).Concat(Observable.Timer(TimeSpan.FromSeconds(_interval)) // 마지막 Interval 후 추가 지연
                            )
                            .Subscribe(
                                _ => { }, // 반복 중에는 별도 처리 필요 없음 (Do에서 이미 처리)
                                () =>
                                {
                                    ShowFingerObj(null);
                                    Debug.Log("모든 이미지가 활성화되었습니다."); // 완료 메시지
                                })
                            .AddTo(this); // GameObject와 생명주기 연동
                        _disposable.Add(t);
                    });
                }
                break;
            case GasSysICheckSection.솔정지:
                {
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    _controlPanel.InitSolenoidValveControl();
                    for (int i = 0; i < 6; ++i)
                    {
                        switchBtnList[i].OnCheck(true);
                    }
                    _controlPanel.CheckWarringSwitch();
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                        ShowFingerObj(fingerObjs[6]);
                        var t = Observable.Timer(TimeSpan.FromSeconds(0.3f))
                            .Subscribe(index =>
                            {
                                _controlPanel.InitSolenoidValveControl(ControlMode.Stop);
                            })
                            .AddTo(this); // GameObject와 생명주기 연동
                        _disposable.Add(t);
                        // var t = Observable.Timer(TimeSpan.FromSeconds(_interval))
                        //     .Take(1)
                        //     .Do(index =>
                        //     {
                        //         //ShowFingerObj(fingerObjs[6]);
                        //         _controlPanel.InitSolenoidValveControl(ControlMode.Stop);
                        //     }).Concat(Observable.Timer(TimeSpan.FromSeconds(_interval)) // 마지막 Interval 후 추가 지연
                        //     ).Subscribe(
                        //         _ => { }, // 반복 중에는 별도 처리 필요 없음 (Do에서 이미 처리)
                        //         () =>
                        //         {
                        //             ShowFingerObj(null);
                        //             Debug.Log("모든 이미지가 활성화되었습니다."); // 완료 메시지
                        //         })
                        //     .AddTo(this); // GameObject와 생명주기 연동
                        // _disposable.Add(t);
                    });
                }
                break;
            // case GasSysICheckSection.기동용기함개방:
            //     {
            //         _controlPanel.ShowPanel(false);
            //         CheckCamIsBlending(_camList[4].gameObject, delegate
            //         {
            //             PlaySolManualAni(solManualClips[0]);
            //         });
            //     }
            //     break;
            case GasSysICheckSection.안전핀및격발준비:
                {
                    _controlPanel.ShowPanel(false);
                    CheckCamIsBlending(_camList[9].gameObject, delegate
                    {
                        var t = PlayQueuedSolManual(solManualClips[0], solManualClips[1], solManualClips[2]).Subscribe(
                                clipName => Debug.Log($"{clipName} 재생 완료!"),
                                () => Debug.Log("모든 클립 재생 완료!")
                            )
                            .AddTo(this);
                        _disposable.Add(t);
                    });
                }
                break;
            case GasSysICheckSection.제어반솔자동:
                {
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    _controlPanel.InitSolenoidValveControl(ControlMode.Stop);
                    for (int i = 0; i < 6; ++i)
                    {
                        switchBtnList[i].OnCheck(true);
                    }
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);

                        ShowFingerObj(fingerObjs[6]);
                        var t = Observable.Timer(TimeSpan.FromSeconds(0.3f))
                            .Subscribe(index =>
                            {
                                _controlPanel.InitSolenoidValveControl();
                            })
                            .AddTo(this); // GameObject와 생명주기 연동
                        _disposable.Add(t);
                        // var t = Observable.Timer(TimeSpan.FromSeconds(_interval))
                        //     .Subscribe(index =>
                        //     {
                        //         _controlPanel.InitSolenoidValveControl();
                        //     })
                        //     .AddTo(this); // GameObject와 생명주기 연동
                        // _disposable.Add(t);
                    });
                }
                break;
            case GasSysICheckSection.제어반정상:
                {
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    for (int i = 0; i < 6; ++i)
                    {
                        switchBtnList[i].OnCheck(true);
                    }
                    _controlPanel.InitSolenoidValveControl();
                    _soundManager.SetBuzzerVolume(0f);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {

                        _controlPanel.ShowPanel(true);
                        var t = Observable.Interval(TimeSpan.FromSeconds(_interval))
                            .Take(5) // UIImage 개수만큼 반복
                            .Do(index =>
                            {
                                ShowFingerObj(fingerObjs[index]);
                                switchBtnList[(int)index].OnCheck(false); // 해당 인덱스의 UIImage 활성화
                                _controlPanel.CheckWarringSwitch();
                            }).Concat(Observable.Timer(TimeSpan.FromSeconds(_interval))) // 마지막 Interval 후 추가 지연
                            .Subscribe(_ => { }, // 반복 중에는 별도 처리 필요 없음 (Do에서 이미 처리)
                                () =>
                                {
                                    ShowFingerObj(null);
                                    _controlPanel.CheckWarringSwitch();
                                    Debug.Log("모든 이미지가 활성화되었습니다."); // 완료 메시지
                                })
                            .AddTo(this); // GameObject와 생명주기 연동
                        _disposable.Add(t);
                    });
                    _curManualOpAniState = null;
                    SetManualBoxReleaseOn(false);
                }
                break;
            // case GasSysICheckSection.수동조작함개방:
            //     {
            //         SetManualBoxReleaseOn(false);
            //         var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
            //         for (int i = 0; i < 5; ++i)
            //         {
            //             switchBtnList[i].OnCheck(false);
            //         }
            //         _controlPanel.CheckWarringSwitch();
            //         _controlPanel.InitSolenoidValveControl();
            //         _controlPanel.ShowPanel(false);
            //         CheckCamIsBlending(_camList[5].gameObject, delegate
            //         {
            //             _soundManager.SetBuzzerVolume(0.2f);
            //             PlayManualOpAni(manualOpClips[1]);
            //         });
            //     }
            //     break;
            case GasSysICheckSection.수동조작함방출_부저:
                {
                    SetManualBoxReleaseOn(false);
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    for (int i = 0; i < 5; ++i)
                    {
                        switchBtnList[i].OnCheck(false);
                    }
                    _controlPanel.CheckWarringSwitch();
                    _controlPanel.InitSolenoidValveControl();
                    _soundManager.SetAlarmVolume(0);
                    _soundManager.SetAlarm2Volume(0);
                    _soundManager.SetBroadcastVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _soundManager.SetBuzzerVolume(0);
                    _controlPanel.ShowPanel(false);
                    _controlPanel.InitTimer();
                    CheckCamIsBlending(_camList[5].gameObject, delegate
                    {
                        _soundManager.SetBuzzerVolume(0.2f);
                        PlayManualOpAni(manualOpClips[5]);
                        var t = Observable.Timer(TimeSpan.FromSeconds(1.3f))
                            .Subscribe(index =>
                            {
                                ShowFingerObj(fingerObjs[11]);
                            })
                            .AddTo(this); // GameObject와 생명주기 연동
                        _disposable.Add(t);
                        // var p = Observable.Timer(TimeSpan.FromSeconds(1.5f))
                        //     .Subscribe(index =>
                        //     {
                        //         SetManualBoxReleaseOn(true);
                        //     })
                        //     .AddTo(this); // GameObject와 생명주기 연동
                        // _disposable.Add(p);

                    });
                }
                break;
            case GasSysICheckSection.방호구역1이동_사이렌_경종_방송_부저:
                {
                    _controlPanel.ShowPanel(false);
                    _soundManager.SetBuzzerVolume(0f);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    CheckCamIsBlending(_camList[6].gameObject, delegate
                    {
                        _soundManager.SetAlarmVolume(0.2f);
                        _soundManager.SetAlarm2Volume(0.1f);
                        _soundManager.SetBroadcastVolume(0.2f);
                        _soundManager.SetSirenVolume(0.2f);
                    });
                }
                break;
            case GasSysICheckSection.제어반_화재_수동조작함기동:
                {

                    _controlPanel.ShowPanel(false);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _soundManager.SetAlarmVolume(0.01f);
                        _soundManager.SetAlarm2Volume(0.005f);
                        _soundManager.SetBroadcastVolume(0.05f);
                        _soundManager.SetSirenVolume(0.03f);
                        _soundManager.SetBuzzerVolume(0f);
                        _controlPanel.ShowPanel(true);
                        _controlPanel.ShowFire(true, false);
                        _controlPanel.StartTimer(30f);
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);
                        ControlPanel.OnTimerCheck.RemoveAllListeners();
                        ControlPanel.OnTimerCheck.AddListener((timer) =>
                        {
                            if (timer < 12.8f)
                            {
                                _controlPanel.SetTimeNum(12.8f);
                                _controlPanel.ResetTimer();
                            }
                        });
                    });
                }
                break;
            case GasSysICheckSection.수동조작함지연:
                {
                    ControlPanel.OnTimerCheck.RemoveAllListeners();
                    _controlPanel.ShowPanel(false);
                    _controlPanel.GetTimerAbort().OnCheck(false);
                    CheckCamIsBlending(_camList[5].gameObject, delegate
                    {
                        ShowFingerObj(fingerObjs[12]);
                        PlayManualOpAni(manualOpClips[3]);


                    });
                }
                break;
            case GasSysICheckSection.감시제어반타이머정지:
                {
                    _controlPanel.ShowPanel(false);
                    _soundManager.SetBuzzerVolume(0f);
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    for (int i = 0; i < 5; ++i)
                    {
                        switchBtnList[i].OnCheck(false);
                    }
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                        _controlPanel.ShowFire(true, false);
                        _controlPanel.SetTimeNum(12.8f);
                        _controlPanel.ResetTimer();
                        _controlPanel.GetTimerAbort().OnCheck(true);
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);
                    });
                }
                break;
            case GasSysICheckSection.수동조작함지연정지:
                {
                    _controlPanel.ShowPanel(false);
                    _controlPanel.GetTimerAbort().OnCheck(false);
                    _soundManager.SetBuzzerVolume(0.01f);
                    CheckCamIsBlending(_camList[5].gameObject, delegate
                    {
                        ShowFingerObj(fingerObjs[13]);
                        PlayManualOpAni(manualOpClips[4]);

                    });
                }
                break;
            case GasSysICheckSection.감시제어반타이머완료및솔작동확인:
                {
                    _controlPanel.ShowPanel(false);
                    _soundManager.SetBuzzerVolume(0f);
                    _soundManager.SetAlarmVolume(0.01f);
                    _soundManager.SetAlarm2Volume(0.005f);
                    _soundManager.SetBroadcastVolume(0.05f);
                    _soundManager.SetSirenVolume(0.03f);
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    for (int i = 0; i < 5; ++i)
                    {
                        switchBtnList[i].OnCheck(false);
                    }
                    RestAni2(solManualClips[5], solManualAni);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                        _controlPanel.ShowFire(true, false);
                        solPopup.SetActive(true);
                        _controlPanel.StartTimer(12.8f);
                        _controlPanel.GetTimerAbort().OnCheck(false);
                        ControlPanel.OnTimerEnd.AddListener(delegate
                        {
                            PlaySolManualAni(solManualClips[5]);
                            _soundManager.SetBangVolume();
                            _soundManager.PlayBang();
                            _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                        });

                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);
                    });
                }
                break;
            case GasSysICheckSection.수동조작함방출복구:
                {

                    _controlPanel.ShowPanel(false);
                    _controlPanel.InitTimer();
                    CheckCamIsBlending(_camList[5].gameObject, delegate
                    {
                        PlayManualOpAni(manualOpClips[6]);
                        SetManualBoxReleaseOn(false);
                        ShowFingerObj(fingerObjs[11]);

                        _soundManager.SetBuzzerVolume(0.05f);
                    });
                }
                break;
            case GasSysICheckSection.감시제어반수동조작함기동비활성:
                {
                    _soundManager.SetBuzzerVolume(0);
                    _soundManager.SetAlarmVolume(0.01f);
                    _soundManager.SetAlarm2Volume(0.005f);
                    _soundManager.SetBroadcastVolume(0.05f);
                    _soundManager.SetSirenVolume(0.03f);
                    _controlPanel.ShowPanel(false);
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    for (int i = 0; i < 5; ++i)
                    {
                        switchBtnList[i].OnCheck(false);
                    }
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, false);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                        _controlPanel.ShowFire(true, false);
                        //solPopup.SetActive(true);
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                        _controlPanel.SetTimeNum(0f);
                        _controlPanel.ResetTimer();
                        _controlPanel.GetTimerAbort().OnCheck(false);
                    });
                }
                break;
            case GasSysICheckSection.감시제어반복구:
                {
                    _controlPanel.ShowPanel(false);
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    for (int i = 0; i < 5; ++i)
                    {
                        switchBtnList[i].OnCheck(false);
                    }
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                        ShowFingerObj(fingerObjs[9]);
                        _controlPanel.ShowFire(false);
                        _soundManager.SetAlarmVolume(0);
                        _soundManager.SetAlarm2Volume(0);
                        _soundManager.SetBroadcastVolume(0);
                        _soundManager.SetSirenVolume(0);
                        //solPopup.SetActive(true);
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                        _controlPanel.SetTimeNum(0f);
                        _controlPanel.ResetTimer();
                        _controlPanel.GetTimerAbort().OnCheck(false);
                    });
                }
                break;
            case GasSysICheckSection.솔복구:
                {
                    _controlPanel.ShowPanel(false);
                    CheckCamIsBlending(_camList[4].gameObject, delegate
                    {
                        sol2Popup.SetActive(true);
                        PlaySolManualAni(solManualClips[4]);
                    });
                }
                break;
            case GasSysICheckSection.감시제어반정상복구확인:
                {
                    _controlPanel.ShowPanel(false);
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    for (int i = 0; i < 6; ++i)
                    {
                        switchBtnList[i].OnCheck(false);
                    }
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                        _controlPanel.ShowFire(false);
                        //solPopup.SetActive(true);
                        _controlPanel.InitTimer();
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    });
                }
                break;
            case GasSysICheckSection.축적:
                {
                    _soundManager.SetAlarmVolume(0);
                    _soundManager.SetAlarm2Volume(0);
                    _soundManager.SetBroadcastVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                        _controlPanel.ShowFire(false);
                        _controlPanel.GetSwitchButton("축적/비축적").OnCheck(true);
                        //solPopup.SetActive(true);
                        _controlPanel.InitTimer();
                        //_controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    });
                }
                break;
            case GasSysICheckSection.연기감지기:
                {
                    _soundManager.SetAlarm2Volume(0f);
                    _soundManager.SetAlarmVolume(0f);
                    _soundManager.PlayBroadcast(false);
                    _soundManager.PlayBroadcast(true);
                    _soundManager.SetBroadcastVolume(0f);
                    _soundManager.SetSirenVolume(0f);
                    _controlPanel.GetSwitchButton("축적/비축적").OnCheck(true);
                    SetDetectorEmissionOrLight(false);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector1, false);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                    CheckCamIsBlending(_camList[8].gameObject, delegate
                    {
                        연기감지기Popup.SetActive(true);
                        //PlaySmokeAni(smokeClips[0]);
                        var t = PlayQueuedSmoke(smokeClips[0], smokeClips[1], smokeClips[2]).Subscribe(
                                clipName => Debug.Log($"{clipName} 재생 완료!"),
                                () =>
                                {
                                    _soundManager.SetAlarm2Volume(0.1f);
                                    _soundManager.SetAlarmVolume(0.2f);
                                    _soundManager.PlayBroadcast(false);
                                    _soundManager.PlayBroadcast(true);
                                    _soundManager.SetBroadcastVolume(0.2f);
                                    SetDetectorEmissionOrLight(true);
                                    Debug.Log("모든 클립 재생 완료!");
                                })
                            .AddTo(this);
                        _disposable.Add(t);
                        ;
                        //solPopup.SetActive(true);
                        //_controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    });
                }
                break;
            case GasSysICheckSection.감시제어반_화재_감지기A:
                {
                    // var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    // for (int i = 0; i < 6; ++i)
                    // {
                    //     switchBtnList[i].OnCheck(false);
                    // }
                    SetDetectorEmissionOrLight(false);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _soundManager.SetAlarmVolume(0.01f);
                        _soundManager.SetAlarm2Volume(0.005f);
                        _soundManager.SetBroadcastVolume(0.05f);
                        _soundManager.SetSirenVolume(0.03f);
                        _soundManager.SetSirenVolume(0f);
                        _controlPanel.GetSwitchButton("축적/비축적").OnCheck(true);
                        _controlPanel.ShowPanel(true);
                        _controlPanel.ShowFire(true, false);
                        //solPopup.SetActive(true);
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                        //_soundManager.SetBuzzerVolume(0.05f);
                        _controlPanel.SetTimeNum(0f);
                        _controlPanel.ResetTimer();
                        _controlPanel.GetTimerAbort().OnCheck(false);
                    });
                }
                break;
            case GasSysICheckSection.열감지기:
                {
                    _soundManager.SetSirenVolume(0f);
                    _controlPanel.GetSwitchButton("축적/비축적").OnCheck(true);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                    SetDetectorEmissionOrLight(false);
                    CheckCamIsBlending(_camList[7].gameObject, delegate
                    {
                        열감지기Popup.SetActive(true);
                        //PlaySmokeAni(smokeClips[0]);
                        var t = PlayQueuedHeat(heatClips[0], heatClips[1], heatClips[2]).Subscribe(
                                clipName => Debug.Log($"{clipName} 재생 완료!"),
                                () =>
                                {
                                    _soundManager.SetAlarm2Volume(0.1f);
                                    _soundManager.SetAlarmVolume(0.2f);
                                    _soundManager.PlayBroadcast(false);
                                    _soundManager.PlayBroadcast(true);
                                    _soundManager.SetBroadcastVolume(0.2f);
                                    _soundManager.SetSirenVolume(0.2f);
                                    SetDetectorEmissionOrLight(true);
                                    Debug.Log("모든 클립 재생 완료!");
                                })
                            .AddTo(this);
                        _disposable.Add(t);
                        ;
                        //solPopup.SetActive(true);
                        //_controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    });
                }
                break;
            case GasSysICheckSection.감시제어반_화재_감지기AB:
                {
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _soundManager.SetAlarmVolume(0.01f);
                        _soundManager.SetAlarm2Volume(0.005f);
                        _soundManager.SetBroadcastVolume(0.05f);
                        _soundManager.SetSirenVolume(0.03f);
                        _controlPanel.GetSwitchButton("축적/비축적").OnCheck(true);
                        _controlPanel.ShowPanel(true);
                        _controlPanel.ShowFire(true, false);
                        //solPopup.SetActive(true);
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector2, true);
                        //_soundManager.SetBuzzerVolume(0.05f);
                        _controlPanel.SetTimeNum(0f);
                        _controlPanel.ResetTimer();
                        _controlPanel.GetTimerAbort().OnCheck(false);
                    });
                }
                break;
            case GasSysICheckSection.감시제어반방출지연:
                {
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.GetSwitchButton("축적/비축적").OnCheck(true);
                        _controlPanel.ShowPanel(true);
                        _controlPanel.ShowFire(true, false);
                        //solPopup.SetActive(true);
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector2, true);
                        _soundManager.SetBuzzerVolume(0f);
                        //_controlPanel.SetTimeNum(30f);
                        _controlPanel.StartTimer(30f);
                        _controlPanel.GetTimerAbort().OnCheck(false);
                        var t = Observable.Timer(TimeSpan.FromSeconds(3f))
                            .Subscribe(index =>
                            {
                                ShowFingerObj(fingerObjs[10]);
                                //_controlPanel.InitSolenoidValveControl(ControlMode.Stop);
                                _controlPanel.SetTimeNum(12.8f);
                                _controlPanel.ResetTimer();
                                _controlPanel.GetTimerAbort().OnCheck(true);
                            })
                            .AddTo(this); // GameObject와 생명주기 연동
                        _disposable.Add(t);
                    });
                }
                break;
            case GasSysICheckSection.감시제어반방출:
                {
                    _soundManager.SetAlarmVolume(0.01f);
                    _soundManager.SetAlarm2Volume(0.005f);
                    _soundManager.SetBroadcastVolume(0.05f);
                    _soundManager.SetSirenVolume(0.03f);
                    //_soundManager.SetBuzzerVolume(0.05f);
                    RestAni2(solManualClips[5], solManualAni);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.GetSwitchButton("축적/비축적").OnCheck(true);
                        _controlPanel.ShowPanel(true);
                        _controlPanel.ShowFire(true, false);
                        //solPopup.SetActive(true);
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector2, true);
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                        //_controlPanel.SetTimeNum(30f);
                        //_controlPanel.ResetTimer(30f);
                        _controlPanel.GetTimerAbort().OnCheck(true);
                        solPopup.SetActive(true);
                        ShowFingerObj(fingerObjs[14]);
                        var t = Observable.Timer(TimeSpan.FromSeconds(0.5f))
                            .Subscribe(index =>
                            {
                                ShowFingerObj(null);
                                //_controlPanel.InitSolenoidValveControl(ControlMode.Stop);
                                _controlPanel.StartTimer(12.8f);
                                _controlPanel.GetTimerAbort().OnCheck(false);
                            })
                            .AddTo(this); // GameObject와 생명주기 연동
                        _disposable.Add(t);
                    });
                    ControlPanel.OnTimerEnd?.AddListener(delegate
                    {
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                        _soundManager.PlayBang();
                        PlaySolManualAni(solManualClips[5]);
                    });
                }
                break;
            // case GasSysICheckSection.솔밸브기동:
            //     break;
            case GasSysICheckSection.복구2:
                {
                    // var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    // for (int i = 0; i < 6; ++i)
                    // {
                    //     switchBtnList[i].OnCheck(false);
                    // }
                    _soundManager.SetAlarmVolume(0.01f);
                    _soundManager.SetAlarm2Volume(0.005f);
                    _soundManager.SetBroadcastVolume(0.05f);
                    _soundManager.SetSirenVolume(0.03f);
                    _soundManager.SetBuzzerVolume(0f);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector2, true);
                    _controlPanel.GetTimerAbort().OnCheck(false);
                    _controlPanel.GetSwitchButton("축적/비축적").OnCheck(true);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        ShowFingerObj(fingerObjs[9]);
                        _controlPanel.ShowPanel(true);
                        var t = Observable.Timer(TimeSpan.FromSeconds(1.5f))
                            .Subscribe(index =>
                            {
                                ShowFingerObj(null);
                                _soundManager.SetAlarm2Volume(0);
                                _soundManager.SetAlarmVolume(0);
                                _soundManager.SetBroadcastVolume(0);
                                _soundManager.SetSirenVolume(0);
                                //_controlPanel.InitSolenoidValveControl(ControlMode.Stop);
                                _controlPanel.ShowFire(false);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector1, false);
                                _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                            })
                            .AddTo(this); // GameObject와 생명주기 연동
                        _disposable.Add(t);
                        //solPopup.SetActive(true);
                        _controlPanel.InitTimer();
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                        //_controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    });
                }
                break;
            case GasSysICheckSection.비축적:
                {
                    _soundManager.SetAlarm2Volume(0);
                    _soundManager.SetAlarmVolume(0);
                    _soundManager.SetBroadcastVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _controlPanel.ShowFire(false);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector1, false);
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.Detector2, false);
                    _controlPanel.GetSwitchButton("축적/비축적").OnCheck(true);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        ShowFingerObj(fingerObjs[5]);
                        _controlPanel.ShowPanel(true);
                        var t = Observable.Timer(TimeSpan.FromSeconds(1f))
                            .Subscribe(index =>
                            {
                                ShowFingerObj(null);
                                //_controlPanel.InitSolenoidValveControl(ControlMode.Stop);
                                _controlPanel.GetSwitchButton("축적/비축적").OnCheck(false);

                            })
                            .AddTo(this); // GameObject와 생명주기 연동
                        _disposable.Add(t);
                        _controlPanel.InitTimer();
                        _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                        //_controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    });
                }
                break;
            case GasSysICheckSection.솔복구2:
                {
                    _soundManager.SetBuzzerVolume(0f);
                    _controlPanel.ShowPanel(false);
                    CheckCamIsBlending(_camList[4].gameObject, delegate
                    {
                        sol2Popup.SetActive(true);
                        PlaySolManualAni(solManualClips[4]);
                    });
                }
                break;
            case GasSysICheckSection.감시제어반정상확인:
                {
                    _soundManager.SetAlarm2Volume(0);
                    _soundManager.SetAlarmVolume(0);
                    _soundManager.SetBroadcastVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _soundManager.SetBuzzerVolume(0);
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    for (int i = 0; i < 6; ++i)
                    {
                        switchBtnList[i].OnCheck(false);
                    }
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                        _controlPanel.ShowFire(false);
                        //solPopup.SetActive(true);
                        _controlPanel.InitTimer();
                    });
                }
                break;
            case GasSysICheckSection.방출표시등작동시험:
                {
                    _soundManager.SetAlarm2Volume(0);
                    _soundManager.SetAlarmVolume(0);
                    _soundManager.SetBroadcastVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _soundManager.SetBuzzerVolume(0);
                    SetDischargeOn(false);
                    SetManualBoxReleaseOn(false);
                    solValveManualObj.SetActive(false);
                    CheckCamIsBlending(_camList[11].gameObject);
                }
                break;
            case GasSysICheckSection.방출표시등확인:
                {
                    CheckCamIsBlending(_camList[10].gameObject);
                }
                break;
            case GasSysICheckSection.감시제어반방출표시등확인:
                {
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    for (int i = 0; i < 6; ++i)
                    {
                        switchBtnList[i].OnCheck(false);
                    }
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                    });
                    _soundManager.SetSirenVolume(0);
                }
                break;
            case GasSysICheckSection.압력스위치작동:
                {
                    _soundManager.SetSirenVolume(0);
                    CheckCamIsBlending(_camList[12].gameObject, delegate
                    {
                        PlayGasAni(gasAutoClips[8]);
                        var t = Observable.Timer(TimeSpan.FromSeconds(0.5f))
                            .Subscribe(index =>
                            {
                                _soundManager.SetSirenVolume(0.2f);
                            })
                            .AddTo(this); // GameObject와 생명주기 연동
                        _disposable.Add(t);

                    });
                }
                break;
            case GasSysICheckSection.방출표시등확인2:
                {
                    _soundManager.SetSirenVolume(0.2f);
                    CheckCamIsBlending(_camList[10].gameObject);
                    SetDischargeOn(true);
                    SetManualBoxReleaseOn(true);
                }
                break;
            case GasSysICheckSection.감시제어반방출표시등확인2:
                {
                    _soundManager.SetSirenVolume(0.2f);
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    for (int i = 0; i < 6; ++i)
                    {
                        switchBtnList[i].OnCheck(false);
                    }
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, true);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                    });
                }
                break;
            case GasSysICheckSection.압력스위치복구:
                {
                    _soundManager.SetSirenVolume(0.2f);
                    CheckCamIsBlending(_camList[12].gameObject, delegate
                    {
                        _soundManager.SetSirenVolume(0);
                        PlayGasAni(gasAutoClips[9]);
                    });
                }
                break;
            case GasSysICheckSection.방출표시등확인3:
                {
                    SetDischargeOn(false);
                    SetManualBoxReleaseOn(false);
                    CheckCamIsBlending(_camList[10].gameObject);
                }
                break;
            case GasSysICheckSection.감시제어반방출표시등확인3:
                {
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    for (int i = 0; i < 6; ++i)
                    {
                        switchBtnList[i].OnCheck(false);
                    }
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                    });
                }
                break;
            case GasSysICheckSection.제어반정상복구:
                {
                    _controlPanel.InitSolenoidValveControl(ControlMode.Auto);
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    for (int i = 0; i < 6; ++i)
                    {
                        switchBtnList[i].OnCheck(false);
                    }
                    _controlPanel.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                    });
                }
                break;
            case GasSysICheckSection.점검후복구:
                {
                    _controlPanel.InitSolenoidValveControl(ControlMode.Auto);
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    for (int i = 0; i < 6; ++i)
                    {
                        switchBtnList[i].OnCheck(false);
                    }
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                        ShowFingerObj(fingerObjs[6]);
                        var t = Observable.Timer(TimeSpan.FromSeconds(0.3f))
                            .Subscribe(index =>
                            {
                                _controlPanel.InitSolenoidValveControl(ControlMode.Stop);
                            })
                            .AddTo(this); // GameObject와 생명주기 연동
                        _disposable.Add(t);
                        
                    });
                }
                break;
            // case GasSysICheckSection.연동정지:
            //     {
            //         var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
            //         for (int i = 0; i < 6; ++i)
            //         {
            //             switchBtnList[i].OnCheck(false);
            //         }
            //         CheckCamIsBlending(_camList[3].gameObject, delegate
            //         {
            //             _controlPanel.ShowPanel(true);
            //             _controlPanel.InitSolenoidValveControl(ControlMode.Stop);
            //         });
            //     }
            //     break;
            case GasSysICheckSection.솔장착:
                {
                    _controlPanel.InitSolenoidValveControl(ControlMode.Stop);
                    solValveManualObj.SetActive(true);
                    CheckCamIsBlending(_camList[9].gameObject, delegate
                    {
                        PlaySolManualAni(solManualClips[3]);
                    });
                }
                break;
            case GasSysICheckSection.연동자동:
                {
                    _controlPanel.InitSolenoidValveControl(ControlMode.Stop);
                    var switchBtnList = _controlPanel._switchButtonDict.Values.ToList();
                    for (int i = 0; i < 6; ++i)
                    {
                        switchBtnList[i].OnCheck(false);
                    }
                    CheckCamIsBlending(_camList[3].gameObject, delegate
                    {
                        _controlPanel.ShowPanel(true);
                        ShowFingerObj(fingerObjs[6]);
                        var t = Observable.Timer(TimeSpan.FromSeconds(0.3f))
                            .Subscribe(index =>
                            {
                                _controlPanel.InitSolenoidValveControl();
                            })
                            .AddTo(this); // GameObject와 생명주기 연동
                        _disposable.Add(t);
                        //_controlPanel.InitSolenoidValveControl();
                    });
                }
                break;
            case GasSysICheckSection.안전핀분리:
                {
                    _controlPanel.InitSolenoidValveControl();
                    autoObjs[2].gameObject.SetActive(false);
                    autoObjs[3].gameObject.SetActive(false);
                    CheckCamIsBlending(_camList[9].gameObject, delegate
                    {
                        PlaySolManualAni(solManualClips[6]);
                    });
                }
                break;
            case GasSysICheckSection.저장용기동관결합:
                {
                    autoObjs[2].gameObject.SetActive(true);
                    autoObjs[3].gameObject.SetActive(true);

                    CheckCamIsBlending(_camList[2].gameObject, delegate
                    {
                        PlayPipeAni(pipeClips[3]);
                    });
                }
                break;
            case GasSysICheckSection.선택밸브동관결합:
                {
                    CheckCamIsBlending(_camList[1].gameObject, delegate
                    {
                        PlayValveAni(valveClips[1]);
                    });
                }
                break;
            case GasSysICheckSection.실습완료:
                {
                    SetCompletePopup("가스계소화설비 점검");
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        //_controlPanel.CheckWarringSwitch();
    }

}
