using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public partial class GasSysISection : MonoBehaviour
{
    private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
    [HideInInspector] public GasSysIState curGasSysIState;

    public Camera mainCamera;
    [SerializeField] private MeshRenderer[] detectorEmissions;
    [SerializeField] private MeshRenderer manualBoxDoorEmission;
    [SerializeField] private MeshRenderer manualBoxReleaseEmission;
    [SerializeField] private GameObject detectorLightObj;
    [SerializeField] private GameObject fireObj;
    [SerializeField] private MeshRenderer dischargeLampEmission;
    [SerializeField] private GameObject dischargeLampLightObj;
    [SerializeField] private GameObject[] fingerObjs;
    [FormerlySerializedAs("gasAnimation")]
    [SerializeField] private Animation gasAni;
    [SerializeField] private Animation solAutoAni;
    [SerializeField] private Animation solManualAni;
    [SerializeField] private Animation manualOpAni;
    [SerializeField] private Animation pipeAni;
    [SerializeField] private Animation smokeAni;
    [SerializeField] private Animation heatAni;
    [SerializeField] private Animation valveAni;
    [SerializeField] private List<AnimationState> gasAutoClips = new List<AnimationState>();
    [SerializeField] private List<AnimationState> solAutoClips = new List<AnimationState>();
    [SerializeField] private List<AnimationState> solManualClips = new List<AnimationState>();
    [SerializeField] private List<AnimationState> manualOpClips = new List<AnimationState>();
    [SerializeField] private List<AnimationState> pipeClips = new List<AnimationState>();
    [SerializeField] private List<AnimationState> smokeClips = new List<AnimationState>();
    [SerializeField] private List<AnimationState> heatClips = new List<AnimationState>();
    [SerializeField] private List<AnimationState> valveClips = new List<AnimationState>();
    [SerializeField] private Texture[] dischargeLampTextures;
    private AnimationState _curGasAniState;
    private AnimationState _curSolAutoAniState;
    private AnimationState _curSolManualAniState;
    private AnimationState _curManualOpAniState;
    private AnimationState _curPipeAniState;
    private AnimationState _curSmokeAniState;
    private AnimationState _curHeatAniState;
    private AnimationState _curValveAniState;

    [SerializeField] private GameObject pipe6_2Base;
    [SerializeField] private GameObject pipe6_2Alpha;
    


    [FormerlySerializedAs("manualObj")]
    [SerializeField] private GameObject[] manualObjs;
    [FormerlySerializedAs("autoObj")]
    [SerializeField] private GameObject[] autoObjs;

    [FormerlySerializedAs("gasRelease2On")]
    [SerializeField] private GameObject[] gasRelease2Ons;

    [SerializeField] private CinemachineBrain cinemachineBrain;
    [SerializeField] private GameObject 열감지기Popup;
    [SerializeField] private GameObject 연기감지기Popup;
    [SerializeField] private GameObject solPopup;
    [SerializeField] private GameObject sol2Popup;
    [SerializeField] private GameObject solValveManualObj;

    private UnityEvent OnBlendComplete = new UnityEvent();
    public UnityEvent<string> OnAnimationComplete = new UnityEvent<string>();

    [HideInInspector] public CompositeDisposable _disposable = new CompositeDisposable();

    private List<CinemachineVirtualCamera> _camList = new List<CinemachineVirtualCamera>();

    private GameObject _curCamPos;
    
    
    private GasSysIHintPanel _hintPanel;
    private SoundManager _soundManager;
    private GasSysIMenuPopup _menuPopup;
    private ControlPanel _controlPanel;
    [SerializeField] private CompletePopup _completePopup;
    private bool isBlending = false;
    private void Start()
    {
        _hintPanel = GasSysIHintPanel.Instance;
        _soundManager = SoundManager.Instance;
        _menuPopup = GasSysIMenuPopup.Instance;
        _controlPanel = ControlPanel.Instance;
        gasAutoClips.Clear();
        solAutoClips.Clear();
        solManualClips.Clear();
        manualOpClips.Clear();
        pipeClips.Clear();
        smokeClips.Clear();
        heatClips.Clear();
        valveClips.Clear();
        foreach (AnimationState state in gasAni)
        {
            //Debug.Log($"Animation Clip: {state.clip.name}, Length: {state.clip.length}s");
            gasAutoClips.Add(state);
        }
        foreach (AnimationState state in solAutoAni)
        {
            solAutoClips.Add(state);
        }
        foreach (AnimationState state in solManualAni)
        {
            solManualClips.Add(state);
        }
        foreach (AnimationState state in manualOpAni)
        {
            manualOpClips.Add(state);
        }
        foreach (AnimationState state in pipeAni)
        {
            pipeClips.Add(state);
        }
        foreach (AnimationState state in smokeAni)
        {
            smokeClips.Add(state);
        }
        foreach (AnimationState state in heatAni)
        {
            heatClips.Add(state);
        }
        foreach (AnimationState state in valveAni)
        {
            valveClips.Add(state);
        }
    }
    public void Init()
    {

        _controlPanel.Init();
        IsRelease2On(false);
        SetDetectorEmissionOrLight(false);
        SetDischargeOn(false);
        fireObj.SetActive(false);
        _hintPanel.ShowHint(true);
        _controlPanel.soundCheckAction.RemoveAllListeners();
        GasSysIHintPanel.onChangeSection.RemoveAllListeners();
        GasSysIHintPanel.onChangeSection.AddListener(SetSection);
        cinemachineBrain.m_CameraActivatedEvent.RemoveAllListeners();
        cinemachineBrain.m_CameraActivatedEvent.AddListener(OnCameraActivated);
        _soundManager.StopAllFireSound();
        _soundManager.PlayAlarm(true);
        _soundManager.PlayAlarm2(true);
        _soundManager.PlayBuzzer(true);
        _soundManager.PlayBroadcast(true);
        _soundManager.PlaySiren(true);
        _soundManager.SetAlarmVolume(0);
        _soundManager.SetAlarm2Volume(0);
        _soundManager.SetBuzzerVolume(0);
        _soundManager.SetBroadcastVolume(0);
        _soundManager.SetBuzzerVolume(0);
        _soundManager.SetSirenVolume(0);
        열감지기Popup.SetActive(false);
        연기감지기Popup.SetActive(false);
        solPopup.SetActive(false);
        StartCoroutine(ResetAniState());
        SetCamPos(null);
        _camList.Clear();
        pipe6_2Base.SetActive(true);
        pipe6_2Alpha.SetActive(false);
        partListObj.ShowPanel(false);
        _hintPanel.nextBtn.gameObject.SetActive(true);
        _hintPanel.prevBtn.gameObject.SetActive(true);
        partObj.SetActive(false);
        mainCamera.cullingMask = LayerMask.GetMask("Default", "저장용기", "기동용가스용기", "솔레노이드밸브", "selectValve", "압력스위치", "방출표시등",
            "수동조작함", "감시제어반", "sol", "particle", "test");
        ResetFinger();
        //ResetAni(gasAutoClips[5], gasAni);
    }

    private void ResetFinger()
    {
        foreach (var obj in fingerObjs)
        {
            obj.SetActive(false);
        }
    }

    private void ShowFingerObj(GameObject obj)
    {
        foreach (var fingerObj in fingerObjs)
        {
            fingerObj.SetActive(fingerObj.Equals(obj));
        }
    }


    public void IsRelease2On(bool isRelease2On)
    {
        foreach (var obj in gasRelease2Ons)
        {
            obj.SetActive(isRelease2On);
        }
    }

    public void IsAuto(bool isAuto)
    {
        foreach (var obj in manualObjs)
        {
            obj.gameObject.SetActive(!isAuto);
        }
        foreach (var obj in autoObjs)
        {
            obj.gameObject.SetActive(isAuto);
        }
    }

    public void SetDetectorEmissionOrLight(bool isOn)
    {
        if (isOn)
        {
            foreach (var detector in detectorEmissions)
            {
                detector.material.EnableKeyword("_EMISSION");
            }
        }
        else
        {
            foreach (var detector in detectorEmissions)
            {
                detector.material.DisableKeyword("_EMISSION");
            }
        }
        detectorLightObj.SetActive(isOn);
    }

    public void SetDischargeOn(bool isOn)
    {
        if (isOn)
        {
            dischargeLampEmission.material.EnableKeyword("_EMISSION");
            dischargeLampEmission.material.SetTexture(BaseMap, dischargeLampTextures[1]);
        }
        else
        {
            dischargeLampEmission.material.DisableKeyword("_EMISSION");
            dischargeLampEmission.material.SetTexture(BaseMap, dischargeLampTextures[0]);
        }
        dischargeLampLightObj.SetActive(isOn);
    }


    public void SetManualBoxDoorOn(bool isOn)
    {
        // if (isOn)
        // {
        //     manualBoxDoorEmission.material.EnableKeyword("_EMISSION");
        // }
        // else
        // {
        //     manualBoxDoorEmission.material.DisableKeyword("_EMISSION");
        // }
    }

    public void SetManualBoxReleaseOn(bool isOn)
    {
        if (isOn)
        {
            manualBoxReleaseEmission.material.EnableKeyword("_EMISSION");
        }
        else
        {
            manualBoxReleaseEmission.material.DisableKeyword("_EMISSION");
        }
    }

    private void SetSection(int index)
    {
        switch (curGasSysIState)
        {
            case GasSysIState.None:
                break;
            case GasSysIState.주요구성요소:
                {

                }
                break;
            case GasSysIState.감시기작동오토:
                {
                    ShowOperationAuto(index);
                }
                break;
            case GasSysIState.수동조작함작동수동:
                {
                    ShowOperationManual(index);
                }
                break;
            case GasSysIState.점검:
                {
                    ShowSafetyCheck(index);
                }
                break;
            case GasSysIState.즉시격발:
                break;
            case GasSysIState.수동조작함작동:
                break;
            case GasSysIState.교차회로:
                break;
            case GasSysIState.스위치동작:
                break;
            case GasSysIState.방출표시등:
                break;
            case GasSysIState.점검완료후복구:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // private void ShowOperationFlowSection(int index)
    // {
    //     switch (curGasSysIOperationFlowState)
    //     {
    //         case GasSysIOperationFlowState.None:
    //             break;
    //         case GasSysIOperationFlowState.Auto:
    //             ShowOperationAuto(index);
    //             break;
    //         case GasSysIOperationFlowState.Manual:
    //             ShowOperationManual(index);
    //             break;
    //         default:
    //             throw new ArgumentOutOfRangeException();
    //     }
    // }

    private void Update()
    {
        UpdateAnimationComplete();
        UpdateCinemachineBrainIsBlending();
    }

    private void UpdateAnimationComplete()
    {
        UpdateGasAnimationComplete();
        UpdateSolAutoAnimationComplete();
    }

    private void UpdateGasAnimationComplete()
    {
        if (gasAni == null)
            return;
        if (null == gasAni.clip)
            return;

        if (gasAni.isPlaying)
            return;

        OnAnimationComplete.Invoke(gasAni.clip.name);

        // foreach (AnimationState obj in gasAnimation)
        // {
        //     if (!gasAnimation.IsPlaying(obj.name))
        //         continue;
        //     OnAnimationComplete.Invoke(obj.name);
        //     break;
        // }
        // Debug.Log("애니메이션 종료!");
    }

    private void UpdateSolAutoAnimationComplete()
    {
        if (null == solAutoAni)
            return;
        if (null == solAutoAni.clip)
            return;
        if (solAutoAni.isPlaying)
            return;

        OnAnimationComplete.Invoke(solAutoAni.clip.name);
        // foreach (AnimationState obj in solAutoAni)
        // {
        //     if (!solAutoAni.IsPlaying(obj.name))
        //         continue;
        //     OnAnimationComplete.Invoke(obj.name);
        //     break;
        // }

    }

    private Animation PlayGasAni(AnimationState state)
    {
        // if (null != _curGasAniState)
        // {
        //     _curGasAniState.normalizedTime = 1f;
        //     _curGasAniState.time = 0f;
        //     gasAnimation.Play(_curGasAniState.name);
        //     gasAnimation[_curGasAniState.name].speed = 0f;
        // }
        //gasAnimation.Stop();
        _curGasAniState = state;
        _curGasAniState.normalizedTime = 0f;
        gasAni.clip = _curGasAniState.clip;
        gasAni.Play();
        gasAni[_curGasAniState.name].speed = 1f;
        return gasAni;
    }

    // private void ResetGasAni()
    // {
    //     if (null == _curGasAniState)
    //         return;
    //     StartCoroutine(StopGasAni());
    //     //_curGasAniState.normalizedTime = 0f;
    //     // gasAnimation[_curGasAniClip.name].normalizedTime = 0f; //
    //     // gasAnimation.Play(_curGasAniClip.name);
    //     // gasAnimation.Stop();
    // }

    private Animation PlaySolAutoAni(AnimationState state)
    {
        // if (null != _curSolAutoAniState)
        // {
        //     _curSolAutoAniState.normalizedTime = 1f;
        //     _curSolAutoAniState.time = 1f;
        //     solAutoAni.Play(_curSolAutoAniState.name);
        //     solAutoAni[_curSolAutoAniState.name].speed = 0f;
        // }
        //solAutoAni.Stop();
        state.normalizedTime = 0f;
        _curSolAutoAniState = state;
        //_curSolAutoAniState.normalizedTime = 0;
        solAutoAni.clip = _curSolAutoAniState.clip;
        Debug.Log(state.name);
        solAutoAni.Play();
        solAutoAni[_curSolAutoAniState.name].speed = 1f;
        return solAutoAni;
    }

    private Animation PlaySolManualAni(AnimationState state)
    {
        // if (null != _curSolAutoAniState)
        // {
        //     _curSolAutoAniState.normalizedTime = 1f;
        //     _curSolAutoAniState.time = 1f;
        //     solAutoAni.Play(_curSolAutoAniState.name);
        //     solAutoAni[_curSolAutoAniState.name].speed = 0f;
        // }
        //solAutoAni.Stop();
        _curSolManualAniState = state;
        _curSolManualAniState.normalizedTime = 0;
        solManualAni.clip = _curSolManualAniState.clip;
        Debug.Log(state.name);
        solManualAni.Play();
        solManualAni[_curSolManualAniState.name].speed = 1f;
        return solManualAni;
    }

    // private void PlayQueuedSolManual(params AnimationState[] states)
    // {
    //     _curSolManualAniState = states[^1];
    //     foreach (var clip in states)
    //     {
    //         clip.normalizedTime = 0f;
    //         solManualAni.PlayQueued(clip.name);
    //     }
    // }
    
    private IObservable<string> PlayQueuedSolManual(params AnimationState[] states)
    {
        _curSolManualAniState = states[^1];
        return Observable.Create<string>(observer =>
        {
            if (states == null || states.Length == 0)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            // 클립을 순차적으로 PlayQueued에 추가
            foreach (var clipName in states)
            {
                if (!solManualAni.GetClip(clipName.name))
                {
                    Debug.LogError($"클립 {clipName}이 Animation 컴포넌트에 없습니다.");
                    continue;
                }
                solManualAni.PlayQueued(clipName.name);
            }

            // 현재 재생 중인 애니메이션 상태를 감시
            var disposable = Observable.EveryUpdate()
                .Where(_ =>
                {
                    // 모든 클립이 재생되었는지 확인
                    return solManualAni.isPlaying == false;
                })
                .Subscribe(_ =>
                {
                    foreach (var clipName in states)
                    {
                        if (solManualAni[clipName.name].normalizedTime >= 1f)
                        {
                            observer.OnNext(clipName.name); // 클립 이름 전송
                        }
                    }
                    observer.OnCompleted(); // 스트림 종료
                });

            // IDisposable 반환: 스트림 해제 관리
            return Disposable.Create(() =>
            {
                disposable.Dispose();
            });
        });
    }
    
    private IObservable<string> PlayQueuedSmoke(params AnimationState[] states)
    {
        _curSmokeAniState = states[^1];
        return Observable.Create<string>(observer =>
        {
            if (states == null || states.Length == 0)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            // 클립을 순차적으로 PlayQueued에 추가
            foreach (var clipName in states)
            {
                if (!smokeAni.GetClip(clipName.name))
                {
                    Debug.LogError($"클립 {clipName}이 Animation 컴포넌트에 없습니다.");
                    continue;
                }
                smokeAni.PlayQueued(clipName.name);
            }

            // 현재 재생 중인 애니메이션 상태를 감시
            var disposable = Observable.EveryUpdate()
                .Where(_ =>
                {
                    // 모든 클립이 재생되었는지 확인
                    return smokeAni.isPlaying == false;
                })
                .Subscribe(_ =>
                {
                    foreach (var clipName in states)
                    {
                        if (smokeAni[clipName.name].normalizedTime >= 1f)
                        {
                            observer.OnNext(clipName.name); // 클립 이름 전송
                        }
                    }
                    observer.OnCompleted(); // 스트림 종료
                });

            // IDisposable 반환: 스트림 해제 관리
            return Disposable.Create(() =>
            {
                disposable.Dispose();
            });
        });
    }
    
    private IObservable<string> PlayQueuedHeat(params AnimationState[] states)
    {
        _curHeatAniState = states[^1];
        return Observable.Create<string>(observer =>
        {
            if (states == null || states.Length == 0)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            // 클립을 순차적으로 PlayQueued에 추가
            foreach (var clipName in states)
            {
                if (!heatAni.GetClip(clipName.name))
                {
                    Debug.LogError($"클립 {clipName}이 Animation 컴포넌트에 없습니다.");
                    continue;
                }
                heatAni.PlayQueued(clipName.name);
            }

            // 현재 재생 중인 애니메이션 상태를 감시
            var disposable = Observable.EveryUpdate()
                .Where(_ =>
                {
                    // 모든 클립이 재생되었는지 확인
                    return heatAni.isPlaying == false;
                })
                .Subscribe(_ =>
                {
                    foreach (var clipName in states)
                    {
                        if (heatAni[clipName.name].normalizedTime >= 1f)
                        {
                            observer.OnNext(clipName.name); // 클립 이름 전송
                        }
                    }
                    observer.OnCompleted(); // 스트림 종료
                });

            // IDisposable 반환: 스트림 해제 관리
            return Disposable.Create(() =>
            {
                disposable.Dispose();
            });
        });
    }
    
    private IObservable<string> PlayQueuedAni(Animation ani, params AnimationState[] states)
    {
        _curSolManualAniState = states[^1];
        return Observable.Create<string>(observer =>
        {
            if (states == null || states.Length == 0)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            // 클립을 순차적으로 PlayQueued에 추가
            foreach (var clipName in states)
            {
                if (!ani.GetClip(clipName.name))
                {
                    Debug.LogError($"클립 {clipName}이 Animation 컴포넌트에 없습니다.");
                    continue;
                }
                ani.PlayQueued(clipName.name);
            }

            // 현재 재생 중인 애니메이션 상태를 감시
            var disposable = Observable.EveryUpdate()
                .Where(_ =>
                {
                    // 모든 클립이 재생되었는지 확인
                    return ani.isPlaying == false;
                })
                .Subscribe(_ =>
                {
                    foreach (var clipName in states)
                    {
                        if (ani[clipName.name].normalizedTime >= 1f)
                        {
                            observer.OnNext(clipName.name); // 클립 이름 전송
                        }
                    }
                    observer.OnCompleted(); // 스트림 종료
                });

            // IDisposable 반환: 스트림 해제 관리
            return Disposable.Create(() =>
            {
                disposable.Dispose();
            });
        });
    }

    private Animation PlayManualOpAni(AnimationState state)
    {
        // if (null != _curSolAutoAniState)
        // {
        //     _curSolAutoAniState.normalizedTime = 1f;
        //     _curSolAutoAniState.time = 1f;
        //     solAutoAni.Play(_curSolAutoAniState.name);
        //     solAutoAni[_curSolAutoAniState.name].speed = 0f;
        // }
        //solAutoAni.Stop();
        _curManualOpAniState = state;
        _curManualOpAniState.normalizedTime = 0;
        manualOpAni.clip = _curManualOpAniState.clip;
        Debug.Log(state.name);
        manualOpAni.Play();
        manualOpAni[_curManualOpAniState.name].speed = 1f;
        return manualOpAni;
    }

    private Animation PlayPipeAni(AnimationState state)
    {
        _curPipeAniState = state;
        _curPipeAniState.normalizedTime = 0;
        pipeAni.clip = _curPipeAniState.clip;
        Debug.Log(state.name);
        pipeAni.Play();
        pipeAni[_curPipeAniState.name].speed = 1f;
        return pipeAni;
    }

    private Animation PlayValveAni(AnimationState state)
    {
        _curValveAniState = state;
        _curValveAniState.normalizedTime = 0;
        valveAni.clip = _curValveAniState.clip;
        Debug.Log(state.name);
        valveAni.Play();
        valveAni[_curValveAniState.name].speed = 1f;
        return valveAni;
    }
    
    private Animation PlaySmokeAni(AnimationState state)
    {
        _curSmokeAniState = state;
        _curSmokeAniState.normalizedTime = 0;
        smokeAni.clip = _curSmokeAniState.clip;
        Debug.Log(state.name);
        smokeAni.Play();
        smokeAni[_curSmokeAniState.name].speed = 1f;
        return smokeAni;
    }
    
    private Animation PlayHeatAni(AnimationState state)
    {
        _curHeatAniState = state;
        _curHeatAniState.normalizedTime = 0;
        heatAni.clip = _curHeatAniState.clip;
        Debug.Log(state.name);
        heatAni.Play();
        heatAni[_curHeatAniState.name].speed = 1f;
        return heatAni;
    }

    private void NextAni()
    {
        if (null != _curSolAutoAniState)
        {
            solAutoAni.Stop(_curSolAutoAniState.name);
            _curSolAutoAniState.normalizedTime = 1f;
            _curSolAutoAniState.time = _curSolAutoAniState.length;
            solAutoAni.Play(_curSolAutoAniState.name);
            solAutoAni[_curSolAutoAniState.name].speed = 0f;
        }
        if (null != _curGasAniState)
        {
            gasAni.Stop(_curGasAniState.name);
            _curGasAniState.normalizedTime = 1f;
            _curGasAniState.time = _curGasAniState.length;
            gasAni.Play(_curGasAniState.name);
            gasAni[_curGasAniState.name].speed = 0f;
        }
        if (null != _curSolManualAniState)
        {
            solManualAni.Stop(_curSolManualAniState.name);
            _curSolManualAniState.normalizedTime = 1f;
            _curSolManualAniState.time = _curSolManualAniState.length;
            solManualAni.Play(_curSolManualAniState.name);
            solManualAni[_curSolManualAniState.name].speed = 0f;
        }
        if (null != _curManualOpAniState)
        {
            manualOpAni.Stop(_curManualOpAniState.name);
            _curManualOpAniState.normalizedTime = 1f;
            _curManualOpAniState.time = _curManualOpAniState.length;
            manualOpAni.Play(_curManualOpAniState.name);
            manualOpAni[_curManualOpAniState.name].speed = 0f;
        }

        if (null != _curPipeAniState)
        {
            pipeAni.Stop(_curPipeAniState.name);
            _curPipeAniState.normalizedTime = 1f;
            _curPipeAniState.time = _curPipeAniState.length;
            pipeAni.Play(_curPipeAniState.name);
            pipeAni[_curPipeAniState.name].speed = 0f;
        }
        
        if (null != _curSmokeAniState)
        {
            smokeAni.Stop(_curSmokeAniState.name);
            _curSmokeAniState.normalizedTime = 1f;
            _curSmokeAniState.time = _curSmokeAniState.length;
            smokeAni.Play(_curSmokeAniState.name);
            smokeAni[_curSmokeAniState.name].speed = 0f;
        }

        if (null != _curHeatAniState)
        {
            heatAni.Stop(_curHeatAniState.name);
            _curHeatAniState.normalizedTime = 1f;
            _curHeatAniState.time = _curHeatAniState.length;
            heatAni.Play(_curHeatAniState.name);
            heatAni[_curHeatAniState.name].speed = 0f;
        }

        if (null != _curValveAniState)
        {
            valveAni.Stop(_curValveAniState.name);
            _curValveAniState.normalizedTime = 1f;
            _curValveAniState.time = _curValveAniState.length;
            valveAni.Play(_curValveAniState.name);
            valveAni[_curValveAniState.name].speed = 0f;
        }
    }

    private void PrevAni()
    {
        if (null != _curSolAutoAniState)
        {
            solAutoAni.Stop(_curSolAutoAniState.name);
            _curSolAutoAniState.normalizedTime = 0f;
            _curSolAutoAniState.time = 0f;
            solAutoAni.Play(_curSolAutoAniState.name);
            solAutoAni[_curSolAutoAniState.name].speed = 0f;
        }
        if (null != _curGasAniState)
        {
            gasAni.Stop(_curGasAniState.name);
            _curGasAniState.normalizedTime = 0f;
            _curGasAniState.time = 0f;
            gasAni.Play(_curGasAniState.name);
            gasAni[_curGasAniState.name].speed = 0f;
        }
        if (null != _curSolManualAniState)
        {
            solManualAni.Stop(_curSolManualAniState.name);
            _curSolManualAniState.normalizedTime = 0f;
            _curSolManualAniState.time = 0f;
            solManualAni.Play(_curSolManualAniState.name);
            solManualAni[_curSolManualAniState.name].speed = 0f;
        }
        if (null != _curManualOpAniState)
        {
            manualOpAni.Stop(_curManualOpAniState.name);
            _curManualOpAniState.normalizedTime = 0f;
            _curManualOpAniState.time = 0f;
            manualOpAni.Play(_curManualOpAniState.name);
            manualOpAni[_curManualOpAniState.name].speed = 0f;
        }

        if (null != _curPipeAniState)
        {
            pipeAni.Stop(_curPipeAniState.name);
            _curPipeAniState.normalizedTime = 0f;
            _curPipeAniState.time = 0f;
            pipeAni.Play(_curPipeAniState.name);
            pipeAni[_curPipeAniState.name].speed = 0f;
        }
        
        if (null != _curSmokeAniState)
        {
            smokeAni.Stop(_curSmokeAniState.name);
            _curSmokeAniState.normalizedTime = 0f;
            _curSmokeAniState.time = 0f;
            smokeAni.Play(_curSmokeAniState.name);
            smokeAni[_curSmokeAniState.name].speed = 0f;
        }

        if (null != _curHeatAniState)
        {
            heatAni.Stop(_curHeatAniState.name);
            _curHeatAniState.normalizedTime = 0f;
            _curHeatAniState.time = 0f;
            heatAni.Play(_curHeatAniState.name);
            heatAni[_curHeatAniState.name].speed = 0f;
        }

        if (null != _curValveAniState)
        {
            valveAni.Stop(_curValveAniState.name);
            _curValveAniState.normalizedTime = 0f;
            _curValveAniState.time = 0f;
            valveAni.Play(_curValveAniState.name);
            valveAni[_curValveAniState.name].speed = 0f;
        }
    }

    // private void ResetSolAutoAni()
    // {
    //     if (null == _curSolAutoAniState)
    //         return;
    //     StartCoroutine(StopSolAni());
    //             //_curSolAutoAniState.normalizedTime = 0f;
    //     // solAutoAni[_curSolAutoAniState.name].normalizedTime = 0f;
    //     // solAutoAni.Play(_curSolAutoAniState.name);
    //     // solAutoAni.Stop();
    // }
    //


    private void UpdateCinemachineBrainIsBlending()
    {
        if (!isBlending || cinemachineBrain.IsBlending)
            return;
        Debug.Log("블렌딩 완료!");
        isBlending = false;

        // 블렌딩 완료 이벤트 실행
        OnBlendComplete?.Invoke();
    }

    // private void DefaultGasAni()
    // {
    //     if (null == _curGasAniState)
    //         return;
    //     StartCoroutine(StartGasAni());
    // }
    //
    // private void DefaultSolAutoAni()
    // {
    //     if (null == _curSolAutoAniState)
    //         return;
    //     StartCoroutine(StartSolAni());
    // }
    //
    // IEnumerator StartGasAni()
    // {
    //     _curGasAniState.normalizedTime = 1f;
    //     gasAni.Play(_curGasAniState.name);
    //     yield return new WaitForEndOfFrame();
    //     _curGasAniState.normalizedTime = 1f;
    //     gasAni.Stop();
    // }
    //
    // IEnumerator StartSolAni()
    // {
    //     _curSolAutoAniState.normalizedTime = 1f;
    //     solAutoAni.Play(_curSolAutoAniState.name);
    //     yield return new WaitForEndOfFrame();
    //     _curSolAutoAniState.normalizedTime = 1f;
    //     solAutoAni.Stop();
    //     
    // }
    //
    // IEnumerator StopSolAni()
    // {
    //     _curSolAutoAniState.normalizedTime = 0f;
    //     solAutoAni.Play(_curSolAutoAniState.name);
    //     yield return new WaitForEndOfFrame();
    //     _curSolAutoAniState.normalizedTime = 0f;
    //     solAutoAni.Stop();
    //     
    // }
    //
    // IEnumerator StopGasAni()
    // {
    //     _curGasAniState.normalizedTime = 0f;
    //     gasAni.Play(_curGasAniState.name);
    //     yield return new WaitForEndOfFrame();
    //     _curGasAniState.normalizedTime = 0f;
    //     gasAni.Stop();
    //     
    // }

    private void OnCameraActivated(ICinemachineCamera fromCamera, ICinemachineCamera toCamera)
    {
        Debug.Log($"카메라 전환 시작: {fromCamera?.Name} -> {toCamera?.Name}");
        isBlending = true; // 블렌딩 시작으로 간주
    }

    private void SetCompletePopup(string text)
    {
        _completePopup.SetPrevStepBtn(delegate
        {
            //SoundManager.Instance.PlayAllFireSound(_soundCheck);
            //_soundManager.SetDefaultVolume();
            _hintPanel.Prev();
            _completePopup.ShowCompletePopup(false);
        });
        _soundManager.ZeroVolume();
        _completePopup.SetCompleteText(text);
        _completePopup.ShowCompletePopup(true);
        //_btnManager.EnableSpecificButton(_gCanvas.GetCompletePopupButtons());
    }

    private void SetCamPos(GameObject obj)
    {
        foreach (var camPos in _camList)
        {
            camPos.gameObject.SetActive(camPos.gameObject.Equals(obj));
        }
        _curCamPos = obj;
    }

    private void CheckCamIsBlending(GameObject obj, Action action = null)
    {
        if (null != _curCamPos)
        {
            if (_curCamPos.Equals(obj))
            {
                action?.Invoke();
                return;
            }
        }
        if (action != null)
            OnBlendComplete.AddListener(action.Invoke);

        SetCamPos(obj);
    }
    private void RestAni2(AnimationState state, Animation ani)
    {
        ani.Stop(state.name);
        state.normalizedTime = 0f;
        state.time = 0f;
        ani.Play(state.name);
        ani[state.name].speed = 0f;
    }

    private IEnumerator ResetAni(AnimationState state, Animation ani)
    {
        
        ani.Stop(state.name);
        state.normalizedTime = 0f;
        state.time = 0f;
        ani.Play(state.name);
        ani[state.name].speed = 0f;
        Debug.Log(state.name);
        /*
        ani.Stop(state.name);
        state.normalizedTime = 0f;
        state.time = 0f;
        ani.Play(state.name);
        ani[state.name].speed = 0f;
        ani[state.name].time = 0f;
        ani[state.name].normalizedTime = 0f;
        Debug.Log(state.name);
        */
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator ResetAniState()
    {
        //ResetAni(gasAutoClips[6], gasAni);
        foreach (var state in gasAutoClips.Where(state => state != gasAutoClips[9] && state != gasAutoClips[10]))
        {
            yield return ResetAni(state, gasAni);
        }
        foreach (AnimationState state in solAutoClips)
        {
            yield return  ResetAni(state, solAutoAni);
            //yield return new WaitForEndOfFrame();
        }

        // foreach (AnimationState state in solManualClips)
        // {
        //     yield return ResetAni(state, solManualAni);
        //     //yield return new WaitForEndOfFrame();
        // }


        foreach (AnimationState state in manualOpClips)
        {
            yield return ResetAni(state, manualOpAni);
            //yield return new WaitForEndOfFrame();
        }
        
        foreach (AnimationState state in pipeClips)
        {
            yield return ResetAni(state, pipeAni);
            //yield return new WaitForEndOfFrame();
        }

        
        // foreach (AnimationState state in smokeClips)
        // {
        //     yield return ResetAni(state, smokeAni);
        //     //yield return new WaitForEndOfFrame();
        // }
        //
        // foreach (AnimationState state in heatClips)
        // {
        //     yield return  ResetAni(state, heatAni);
        //     //yield return new WaitForEndOfFrame();
        // }

        foreach (AnimationState state in valveClips)
        {
            yield return ResetAni(state, valveAni);
        }
        
        _curGasAniState = null;
        _curSolAutoAniState = null;
        _curSolManualAniState = null;
        _curManualOpAniState = null;
        _curPipeAniState = null;
        _curSmokeAniState = null;
        _curHeatAniState = null;
        _curValveAniState = null;
        yield return new WaitForEndOfFrame();
    }

}
