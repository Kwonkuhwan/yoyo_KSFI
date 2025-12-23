using System;
using System.Collections;
using System.Collections.Generic;
using GLTFast.Schema;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SolenoidValve : MonoBehaviour
{
    [SerializeField] private GameObject activationImg;

    [SerializeField] private Button activationBtn;

    [SerializeField] private GameObject safetyPinObj;
    [FormerlySerializedAs("safetyPinAttachBtn")]
    [SerializeField] private Button attacSafetyPinhBtn;
    [FormerlySerializedAs("safetyPinDetachBtn")]
    [SerializeField] private Button detachSafetyPinBtn;
    [FormerlySerializedAs("GetsafetyPinBtn")]
    [SerializeField] private Button getSafetyPinBtn;

    [SerializeField] private GameObject safetyClipObj;
    [FormerlySerializedAs("safetyClipAttachBtn")]
    [SerializeField] private Button attachSafetyClipBtn;
    [FormerlySerializedAs("SafetyClipBtn")]
    [SerializeField] private Button detachSafetyClipBtn;

    [SerializeField] private DefaultPopupObj solenoidValve3DPopup;
    
    private CompositeDisposable _activationDisposable = new CompositeDisposable();
    private CompositeDisposable _attachSafetyPinDisposable = new CompositeDisposable();
    private CompositeDisposable _detachSafetyPinDisposable = new CompositeDisposable();
    private CompositeDisposable _getSafetyPinDisposable = new CompositeDisposable();
    private CompositeDisposable _attachSafetyClipDisposable = new CompositeDisposable();
    private CompositeDisposable _detachSafetyClipDisposable = new CompositeDisposable();

    public enum InitState
    {
        SafetyCheck,
        ManualOperationController,
        ManualControlBoxController,
        CrossCircuitDetector,
        ControlPanelSwitchController,

    }

    // Start is called before the first frame update
    void Start()
    {

        //Init();
    }

    public void Init(InitState state)
    {
        SetInitState(state);
    }

    public void SetInitState(InitState state)
    {
        switch (state)
        {
            case InitState.SafetyCheck:
                SetDefaultState();
                break;
            case InitState.ManualOperationController:
                SetManualOperationController();
                break;
            case InitState.ManualControlBoxController:
                SetManualControlBoxController();
                break;
            case InitState.CrossCircuitDetector:
                SetCrossCircuitDetector();
                break;
            case InitState.ControlPanelSwitchController:
                InitControlPanelSwitchController();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

#region 점검 전 안전조치

    public void SetDefaultState()
    {
        SetActivationImg(false);
        activationBtn.interactable = false;
        attacSafetyPinhBtn.gameObject.SetActive(false);
        detachSafetyPinBtn.gameObject.SetActive(false);
        getSafetyPinBtn.gameObject.SetActive(true);
        attachSafetyClipBtn.interactable = false;
        detachSafetyClipBtn.interactable = false;
        SetGetSafetyPinBtn(() =>
        {
            getSafetyPinBtn.gameObject.SetActive(false);
            attacSafetyPinhBtn.gameObject.SetActive(true);
        });
        SetAttachSafetyPinBtn(() =>
        {
            SetReleaseActivationCylinderFromSolenoidValveState();
        });

    }

    public void SetReleaseActivationCylinderFromSolenoidValveState()
    {
        activationImg.SetActive(false);
        detachSafetyPinBtn.gameObject.SetActive(true);
        attacSafetyPinhBtn.gameObject.SetActive(false);
        getSafetyPinBtn.gameObject.SetActive(false);
        attachSafetyClipBtn.interactable = false;
        detachSafetyClipBtn.interactable = false;
    }

#endregion //점검 전 안전조치

#region 기동용기 솔레노이드 밸브 격발시험-> 수동조작버튼작동[즉시격발]

    public void SetManualOperationController()
    {
        SetActivationImg(false);
        getSafetyPinBtn.gameObject.SetActive(false);
        detachSafetyClipBtn.gameObject.SetActive(true);
        attachSafetyClipBtn.gameObject.SetActive(false);
        detachSafetyClipBtn.interactable = true;
        detachSafetyPinBtn.gameObject.SetActive(false);
        activationBtn.interactable = true;
        activationBtn.gameObject.SetActive(true);
        SetDetachSafetyClipBtn(() =>
        {
            detachSafetyClipBtn.gameObject.SetActive(false);
        });
        
    }



#endregion // 기동용기 솔레노이드 밸브 격발시험-> 수동조작버튼작동[즉시격발]

#region 기동용기 솔레노이드밸브 격발시험 -> 수동조작함 작동

    public void SetManualControlBoxController()
    {
        SetActivationImg(false);
        getSafetyPinBtn.gameObject.SetActive(false);
        detachSafetyClipBtn.gameObject.SetActive(false);
        attachSafetyClipBtn.gameObject.SetActive(false);
        detachSafetyClipBtn.interactable = false;
        detachSafetyPinBtn.gameObject.SetActive(false);
        activationBtn.interactable = false;
        activationBtn.gameObject.SetActive(true);
    }

#endregion //기동용기 솔레노이드밸브 격발시험 -> 수동조작함 작동
    
#region 기동용기 솔레노이드밸브 격발시험 -> 교차회로 감지기 동작

    public void SetCrossCircuitDetector()
    {
        SetActivationImg(false);
        getSafetyPinBtn.gameObject.SetActive(false);
        detachSafetyClipBtn.gameObject.SetActive(false);
        attachSafetyClipBtn.gameObject.SetActive(false);
        detachSafetyClipBtn.interactable = false;
        detachSafetyPinBtn.gameObject.SetActive(false);
        activationBtn.interactable = false;
        activationBtn.gameObject.SetActive(true);
    }

#endregion //기동용기 솔레노이드밸브 격발시험 -> 교차회로 감지기 동작
    
#region 기동용기 솔레노이드밸브 격발시험 -> 제어반 수동조작스위치 동작

    public void InitControlPanelSwitchController()
    {
        SetActivationImg(false);
        getSafetyPinBtn.gameObject.SetActive(false);
        detachSafetyClipBtn.gameObject.SetActive(false);
        attachSafetyClipBtn.gameObject.SetActive(false);
        detachSafetyClipBtn.interactable = false;
        detachSafetyPinBtn.gameObject.SetActive(false);
        activationBtn.interactable = false;
        activationBtn.gameObject.SetActive(true);
    }

#endregion //기동용기 솔레노이드밸브 격발시험 -> 제어반 수동조작스위치 동작
    public void SetActivationImg(bool isOn)
    {
        activationImg.SetActive(isOn);
    }
    public void SetActivationBtn(UnityAction action)
    {
        _activationDisposable?.Clear();
        var disposable = activationBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                action?.Invoke();
            }).AddTo(this);
        _activationDisposable?.Add(disposable);
    }

    public void SetAttachSafetyPinBtn(UnityAction action)
    {
        _attachSafetyPinDisposable?.Clear();

        var disposable = attacSafetyPinhBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                action?.Invoke();
            }).AddTo(this);
        _attachSafetyPinDisposable?.Add(disposable);
    }

    public void SetDetachSafetyPinBtn(UnityAction action)
    {
        _detachSafetyPinDisposable?.Clear();
        var disposable = detachSafetyPinBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                action?.Invoke();
            }).AddTo(this);
        _detachSafetyPinDisposable?.Add(disposable);
    }

    public void SetGetSafetyPinBtn(UnityAction action)
    {
        _getSafetyPinDisposable?.Clear();
        var disposable = getSafetyPinBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                action?.Invoke();
            }).AddTo(this);
        _getSafetyPinDisposable?.Add(disposable);
    }

    public void SetAttachSafetyClipBtn(UnityAction action)
    {
        _attachSafetyClipDisposable?.Clear();
        var disposable = attachSafetyClipBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                action?.Invoke();
            }).AddTo(this);
        _attachSafetyClipDisposable?.Add(disposable);
    }

    public void SetDetachSafetyClipBtn(UnityAction action)
    {
        _detachSafetyClipDisposable?.Clear();
        var disposable = detachSafetyClipBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                action?.Invoke();
            }).AddTo(this);
        _detachSafetyClipDisposable?.Add(disposable);
    }
}
