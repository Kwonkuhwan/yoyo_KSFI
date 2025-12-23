using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SmokeDetectorPopup : MonoBehaviour
{
    [SerializeField] public GameObject onObj;
    [SerializeField] public GameObject smokeCheckObj;
    [SerializeField] public Button closeBtn;
    private float _delayInSeconds = 3f;
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
                
            })
            .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
        _disposable?.Add(temp);
        SetSmokeDetector(false);
    }
    
    public void InitFireAlarmOff(UnityAction<bool> callback, UnityAction close = null)
    {
        smokeCheckObj.SetActive(true);
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
                onObj.SetActive(false);
                //closeBtn.gameObject.SetActive(true);
                callback?.Invoke(onObj.activeSelf);
                
            })
            .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
        _disposable?.Add(temp);
        SetSmokeDetector(false);
    }
    
    public void InitFireAlarmOn(UnityAction<bool> callback, UnityAction close = null)
    {
        
        smokeCheckObj.SetActive(true);
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
                
            })
            .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
        _disposable?.Add(temp);
        SetSmokeDetector(false);
    }

    public void ResetSmokeDetectorTime()
    {
        _disposable.Clear();
    }

    public void SetSmokeDetector(bool isOn)
    {
        _isOn = isOn;
        onObj.SetActive(_isOn);
    }

#endregion
    // Start is called before the first frame update
    // void Start()
    // {
    //     //InitCrossCircuitDetector(null, null);
    // }
    //
    // // Update is called once per frame
    // void Update()
    // {
    //     
    // }
}
