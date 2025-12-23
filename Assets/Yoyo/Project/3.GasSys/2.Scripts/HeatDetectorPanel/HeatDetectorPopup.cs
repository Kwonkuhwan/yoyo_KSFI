using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HeatDetectorPopup : MonoBehaviour
{
    [SerializeField] private GameObject onObj;
    [SerializeField] public Button closeBtn;
    private float _delayInSeconds = 4f;
    private bool _isOn = false;
    private CompositeDisposable _disposable = new CompositeDisposable();
#region 교차회로 감지기 동작

    public void InitCrossCircuitDetector(UnityAction<bool> callback, UnityAction close = null)
    {
        
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(delegate
        {
            close?.Invoke();
            this.gameObject.SetActive(false);
        });
        
        closeBtn.gameObject.SetActive(false);
        onObj.SetActive(false);
        _disposable?.Clear(); 
        var temp = Observable.Timer(System.TimeSpan.FromSeconds(_delayInSeconds))
            .Subscribe(_ =>
            {
                onObj.SetActive(true);
                //closeBtn.gameObject.SetActive(true);
                callback?.Invoke(onObj.activeSelf);
                
            }).AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
        _disposable?.Add(temp);
        SetHeatDetector(false);
    }

    public void SetHeatDetector(bool isOn)
    {
        _isOn = isOn;
        onObj.SetActive(_isOn);
    }

#endregion

    private void OnDisable()
    {
        _disposable?.Clear();
    }
    // Start is called before the first frame update
    void Start()
    {
        //InitCrossCircuitDetector(null, null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
