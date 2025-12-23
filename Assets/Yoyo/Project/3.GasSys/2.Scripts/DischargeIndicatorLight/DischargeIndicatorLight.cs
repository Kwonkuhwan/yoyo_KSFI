using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DischargeIndicatorLight : MonoBehaviour
{
    [SerializeField] private GameObject activateImg;
    [SerializeField] private Button btn;

    // ReSharper disable once CollectionNeverQueried.Local
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private CompositeDisposable _compositeDisposable = new CompositeDisposable();
    public void Init()
    {
        activateImg.SetActive(false);
        _compositeDisposable?.Clear();
        var disposable = btn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                this.gameObject.SetActive(false);
            }).AddTo(this);
        _compositeDisposable?.Add(disposable);
    }

    public void SetActivate(bool activate)
    {
        activateImg.SetActive(activate);
    }

    private void OnDisable()
    {
        _compositeDisposable?.Clear();
    }
}
