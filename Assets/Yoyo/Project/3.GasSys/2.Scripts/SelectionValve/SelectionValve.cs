using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 기동용기함 연결 하는 선택벨브(조작동관)
/// </summary>
public class SelectionValve : MonoBehaviour
{
    [SerializeField] private GameObject onObj;
    [SerializeField] private GameObject offObj;
    
    
    [SerializeField] private Button[] attachBtns;
    [SerializeField] private Button[] detachBtns;

    private CompositeDisposable[] _attachDisposables = new CompositeDisposable[2];
    private CompositeDisposable[] _detachDisposables = new CompositeDisposable[2];

    private bool[] _isAttaching = new bool[2];

    private bool _isOnOff = false;
    // Start is called before the first frame update

    public void Init()
    {
        onObj.SetActive(true);
        offObj.SetActive(false);
    }
    
#region 점검 전 안전조치

    /// <summary>
    /// 기동용기함과 연결된 모든 동관 연결 상태로 초기화
    /// </summary>
    public void InitSafetyCheck()
    {
        foreach (var btn in attachBtns)
        {
            btn.interactable = false;
        }
        
        foreach (var btn in detachBtns)
        {
            btn.interactable = true;
        }

        for (int i = 0; i < _isAttaching.Length; ++i)
        {
            _isAttaching[i] = true;
        }

        foreach (var attach in _attachDisposables)
        {
            attach?.Clear();
        }

        foreach (var detach in _detachDisposables)
        {
            detach?.Clear();
        }
    }

    public void InitNewSafetyCheck()
    {
        Init();
        onObj.SetActive(true);
        offObj.SetActive(false);
    }

#endregion //점검 전 안전조치

#region 점검 후 복구

    public void InitRecoveryCheck()
    {
        
        foreach (var btn in attachBtns)
        {
            btn.interactable = true;
        }
        
        foreach (var btn in detachBtns)
        {
            btn.interactable = false;
        }

        for (int i = 0; i < _isAttaching.Length; ++i)
        {
            _isAttaching[i] = true;
        }

        foreach (var attach in _attachDisposables)
        {
            attach?.Clear();
        }

        foreach (var detach in _detachDisposables)
        {
            detach?.Clear();
        }
    }

#endregion //점검 후 복구


    public void SetAttachBtn(UnityAction action, int index)
    {
        _attachDisposables[index]?.Clear();
        var disposable = attachBtns[index].OnClickAsObservable()
            .Subscribe(_ =>
            {
                _isAttaching[index] = true;
                action?.Invoke();

            }).AddTo(this);
        _attachDisposables[index]?.Add(disposable);
    }


    public void SetDetachBtn(UnityAction action, int index)
    {
        _detachDisposables[index]?.Clear();
        var disposable = detachBtns[index].OnClickAsObservable()
            .Subscribe(_ =>
            {
                _isAttaching[index] = false;
                action.Invoke();
            }).AddTo(this);
        _detachDisposables[index]?.Add(disposable);
    }

    public bool IsAttach(int index)
    {
        return _isAttaching[index];
    }

    public void ChangeValveState(bool isOn)
    {
        _isOnOff = isOn;
        onObj.SetActive(isOn);
        offObj.SetActive(!isOn);
    }
}
