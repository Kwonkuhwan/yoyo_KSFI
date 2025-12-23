using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 기동용기함 팝업
/// 솔레노이드밸브
/// 압력수위치
/// </summary>
public class InsideBox : MonoBehaviour
{
    [SerializeField] private Button detachBtn;
    [SerializeField] private Button attachBtn;
    [SerializeField] private Button getSafetyPinBtn;
    [SerializeField] private Button setSafetyPinBtn;
    [SerializeField] private Button attachSafetyPinBtn;
    [SerializeField] private Button detachSafetyPinBtn;

    [SerializeField] private Button pressureUpBtn;
    [SerializeField] private Button pressureDownBtn;

    private CompositeDisposable _detachBtnDisposable = new CompositeDisposable();
    private CompositeDisposable _attachBtnDisposable = new CompositeDisposable();
    private CompositeDisposable _getSafetyPinBtnDisposable = new CompositeDisposable();
    private CompositeDisposable _setSafetyPinBtnDisposable = new CompositeDisposable();
    private CompositeDisposable _attachSafetyPinBtnDisposable = new CompositeDisposable();
    private CompositeDisposable _detachSafetyPinBtnDisposable = new CompositeDisposable();
    private CompositeDisposable _pressureUpBtnDisposable = new CompositeDisposable();
    private CompositeDisposable _pressureDownBtnDisposable = new CompositeDisposable();

    // Start is called before the first frame update
    // void Start()
    // {
    //     Init();
    // }

#region 점검 전 안전조치

    public void InitSafetyCheck()
    {
        gameObject.SetActive(true);
        detachBtn.interactable = false;
        attachBtn.gameObject.SetActive(false);
        attachSafetyPinBtn.interactable = true;
        getSafetyPinBtn.gameObject.SetActive(true);
        attachSafetyPinBtn.gameObject.SetActive(false);
        pressureUpBtn.interactable = false;
        pressureDownBtn.interactable = false;
        SetGetSafetyPinBtn(() =>
        {
            getSafetyPinBtn.gameObject.SetActive(false);
            attachSafetyPinBtn.gameObject.SetActive(true);
        });
        SetAttachSafetyPinBtn(() =>
        {
            attachSafetyPinBtn.interactable = false;
            detachBtn.interactable = true;
        });
    }

#endregion // 점검 전 안전조치

#region 방출표시등 작동시험

    public void InitDischargeIndicatorLightTest()
    {
        gameObject.SetActive(true);
        detachBtn.gameObject.SetActive(true);
        attachBtn.gameObject.SetActive(false);
        getSafetyPinBtn.gameObject.SetActive(false);
        setSafetyPinBtn.gameObject.SetActive(false);
        attachSafetyPinBtn.gameObject.SetActive(false);
        detachSafetyPinBtn.gameObject.SetActive(false);
        detachBtn.interactable = false;
        pressureUpBtn.gameObject.SetActive(true);
        pressureDownBtn.gameObject.SetActive(false);
        pressureUpBtn.interactable = true;
    }

#endregion //방출표시등 작동시험

#region 점검 후 복구

    public void InitRecoveryCheck()
    {
        gameObject.SetActive(true);
        attachBtn.gameObject.SetActive(true);
        setSafetyPinBtn.gameObject.SetActive(false);
        detachSafetyPinBtn.gameObject.SetActive(false);
        detachBtn.gameObject.SetActive(false);
        attachBtn.interactable = false;
        getSafetyPinBtn.gameObject.SetActive(true);
        attachSafetyPinBtn.interactable = false;
        attachSafetyPinBtn.gameObject.SetActive(true);
        SetGetSafetyPinBtn(() =>
        {
            getSafetyPinBtn.gameObject.SetActive(false);
            attachSafetyPinBtn.interactable = true;
        });
        SetAttachSafetyPinBtn(() =>
        {
            attachSafetyPinBtn.gameObject.SetActive(false);
            attachBtn.interactable = true;
        });
    }

    public void InitRecoveryCheck2()
    {
        gameObject.SetActive(true);
        detachBtn.gameObject.SetActive(false);
        attachBtn.interactable = false;
        attachSafetyPinBtn.gameObject.SetActive(false);
        detachSafetyPinBtn.gameObject.SetActive(true);
        getSafetyPinBtn.gameObject.SetActive(false);
        setSafetyPinBtn.gameObject.SetActive(true);
        setSafetyPinBtn.interactable = false;
        SetDetachSafetyPinBtn(() =>
        {
            detachSafetyPinBtn.gameObject.SetActive(false);
            setSafetyPinBtn.interactable = true;
        });
        
    }

#endregion //점검 후 복구
    
    public void SetDetachBtn(UnityAction action)
    {
        _detachBtnDisposable?.Clear();
        var disposable = detachBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                action?.Invoke();
            }).AddTo(this);

        _detachBtnDisposable?.Add(disposable);
    }

    public void SetAttachBtn(UnityAction action)
    {
        _attachBtnDisposable?.Clear();
        var disposable = attachBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                action?.Invoke();
            }).AddTo(this);
        _attachBtnDisposable?.Add(disposable);
    }

    public void SetGetSafetyPinBtn(UnityAction action)
    {
        _getSafetyPinBtnDisposable?.Clear();

        var disposable = getSafetyPinBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                action?.Invoke();
            }).AddTo(this);

        _getSafetyPinBtnDisposable?.Add(disposable);
    }
    
    public void SetSafetyPinBtn(UnityAction action)
    {
        _setSafetyPinBtnDisposable?.Clear();

        var disposable = setSafetyPinBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                action?.Invoke();
            }).AddTo(this);

        _setSafetyPinBtnDisposable?.Add(disposable);
    }

    public void SetAttachSafetyPinBtn(UnityAction action)
    {
        _attachSafetyPinBtnDisposable?.Clear();
        var disposable = attachSafetyPinBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                action?.Invoke();
            }).AddTo(this);

        _attachSafetyPinBtnDisposable?.Add(disposable);
    }
    
    public void SetDetachSafetyPinBtn(UnityAction action)
    {
        _detachSafetyPinBtnDisposable?.Clear();
        var disposable = detachSafetyPinBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                action?.Invoke();
            }).AddTo(this);

        _detachSafetyPinBtnDisposable?.Add(disposable);
    }

    public void SetPressureUpBtn(UnityAction action)
    {
        _pressureUpBtnDisposable?.Clear();
        var disposable = pressureUpBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                pressureUpBtn.gameObject.SetActive(false);
                pressureDownBtn.gameObject.SetActive(true);
                pressureDownBtn.interactable = true;
                action?.Invoke();
            }).AddTo(this);
        _pressureUpBtnDisposable?.Add(disposable);
    }

    public void SetPressureDownBtn(UnityAction action)
    {
        _pressureDownBtnDisposable?.Clear();
        var disposable = pressureDownBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                pressureUpBtn.gameObject.SetActive(true);
                pressureUpBtn.interactable = true;
                pressureDownBtn.gameObject.SetActive(false);
                action?.Invoke();
            }).AddTo(this);
        _pressureDownBtnDisposable?.Add(disposable);
    }

    private void OnDisable()
    {
        // _detachBtnDisposable?.Clear();
        // _getSafetyPinBtnDisposable?.Clear();
        // _attachSafetyPinBtnDisposable?.Clear();
    }


    // Update is called once per frame
    void Update()
    {

    }
}
