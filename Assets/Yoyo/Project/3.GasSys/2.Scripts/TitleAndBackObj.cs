using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TitleAndBackObj : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private TextMeshProUGUI titleText;

    private CompositeDisposable _backBtnDisposable = new CompositeDisposable(); 

    public void Init(string title)
    {
        titleText.text = title;
    }
    public void SetBackBtn(UnityAction backBtnAction)
    {
        _backBtnDisposable?.Clear();
        var disposable = backBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                backBtnAction?.Invoke();
            }).AddTo(this);
        _backBtnDisposable?.Add(disposable);
    }

}
