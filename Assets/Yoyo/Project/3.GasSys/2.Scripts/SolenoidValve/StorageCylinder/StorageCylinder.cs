using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 기동용기에 저장용기에 연결된 개방용 동관
/// </summary>
public class StorageCylinder : MonoBehaviour
{
    [SerializeField] private GameObject onObj;
    [SerializeField] private GameObject offObj;

    
    [SerializeField] private Button attachBtn;
    [SerializeField] private Button detachBtn;
    
    private CompositeDisposable _attachDisposable = new CompositeDisposable();
    private CompositeDisposable _detachDisposable = new CompositeDisposable();

    private bool _isAttaching = true;
    

#region 점검 전 안전조치

    public void InitSafetyCheck()
    {
        attachBtn.interactable = false;
        detachBtn.interactable = true;
        _isAttaching = true;

        _attachDisposable?.Clear();
        _detachDisposable?.Clear();
    }

    public void InitNewSafetyCheck()
    {
        onObj.SetActive(true);
        offObj.SetActive(false);
    }

#endregion //점검전 안전조치

#region 점검 후 복구

    public void InitRecoveryCheck()
    {
        attachBtn.interactable = true;
        detachBtn.interactable = false;
        _isAttaching = false;
        
        _attachDisposable?.Clear();
        _detachDisposable?.Clear();
    }

#endregion //점검 후 복구

    public void SetAttachBtn(UnityAction action)
    {
        _attachDisposable?.Clear();
        var disposable = attachBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                _isAttaching = true;
                action?.Invoke();

            }).AddTo(this);
        _attachDisposable?.Add(disposable);
    }


    public void SetDetachBtn(UnityAction action)
    {
        _detachDisposable?.Clear();
        var disposable = detachBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                _isAttaching = false;
                action.Invoke();
            }).AddTo(this);
        _detachDisposable?.Add(disposable);
    }

    public bool IsAttach()
    {
        return _isAttaching;
    }
    
    public void ChangeValveState(bool isOn)
    {
        onObj.SetActive(isOn);
        offObj.SetActive(!isOn);
    }
}
