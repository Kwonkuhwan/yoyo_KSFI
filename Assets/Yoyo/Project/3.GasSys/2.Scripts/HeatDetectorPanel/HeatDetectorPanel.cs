using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HeatDetectorPanel : MonoBehaviour
{
    [SerializeField] private GameObject activateImg;
    [SerializeField] private Button agreeBtn;
    
    private CompositeDisposable _disposable = new CompositeDisposable();
    public void OnEnable()
    {
        SetActivate(false);
        _disposable?.Clear();
        var disposable = agreeBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                gameObject.SetActive(false);
            }).AddTo(this);
        _disposable?.Add(disposable);
    }

    public void SetActivate(bool isActivate)
    {
        activateImg.SetActive(isActivate);
    }
}
