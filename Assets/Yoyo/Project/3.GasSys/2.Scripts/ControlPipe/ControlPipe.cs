using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

/// <summary>
/// 사용 안함
/// </summary>
public class ControlPipe : MonoBehaviour
{

    [SerializeField] private Button[] attachBtns;
    [SerializeField] private Button[] detachBtns;
    
    private CompositeDisposable[] _attachDisposables = new CompositeDisposable[2];
    private CompositeDisposable[] _detachDisposables = new CompositeDisposable[2];

    private bool[] _isAttaching = new bool[2];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {
        for (int i = 0; i < _isAttaching.Length; ++i)
        {
            _isAttaching[i] = true;
        }
  
        foreach (var attach in _attachDisposables)
        {
            attach.Clear();
        }

        foreach (var detach in _detachDisposables)
        {
            detach.Clear();
        }
    }

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
}
