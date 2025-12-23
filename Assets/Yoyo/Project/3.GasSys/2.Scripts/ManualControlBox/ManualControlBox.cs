using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ManualControlBox : MonoBehaviour
{
    public enum State
    {
        SafetyCheck,
        Mode1,
        Mode2,
        Mode3,
    }
#region 메인 패널

    [SerializeField] private ConfirmationPanel power;
    [SerializeField] private ConfirmationPanel discharge;

#endregion //메인 패널

#region 오픈 박스

    [SerializeField] private GameObject openPanel;
    [SerializeField] private Button openBtn;
    [SerializeField] private Button delayBtn;
    [SerializeField] private Button dischargeActivationBtn;

#endregion //오픈 박스

    private Button closeBtn;
    private CompositeDisposable _openBtnDisposable = new CompositeDisposable();
    private Dictionary<ManualControlBox.State, IDisposable> _delayBtnSubscriptions = new Dictionary<ManualControlBox.State, IDisposable>();
    private Dictionary<ManualControlBox.State, IDisposable> _dischargeActivationBtnSubscriptions = new Dictionary<ManualControlBox.State, IDisposable>();

    private CompositeDisposable _delayBtnDisposable = new CompositeDisposable();
    private CompositeDisposable _dischargeActivationBtnDisposable = new CompositeDisposable();
    // Start is called before the first frame update
    void Start()
    {
        // Init();
        // SetOpenBox(null);
        // SetOpenBox(null);
        // SetOpenBox(null);
        // SetDelayBtn(delegate
        // {
        //     //delayBtn.interactable = false;
        //     discharge.OnCheck(!discharge.IsChecked());
        // }, State.SafetyCheck);
        //
        // SetDelayBtn(delegate
        // {
        //     dischargeActivationBtn.interactable = false;
        // },State.Mode1);
        //
        // SetDischargeBtn(null, State.SafetyCheck);
    }

    public void InitManualControlBox()
    {
        power.InitOneLine(true);
        discharge.InitOneLine(false);

        //openBtn.onClick.RemoveAllListeners();
        //delayBtn.onClick.RemoveAllListeners();
        //dischargeActivationBtn.onClick.RemoveAllListeners();

        openBtn.gameObject.SetActive(true);
        openPanel.SetActive(false);
    }

    public void InitDischargeIndicatorLightTest()
    {
        power.InitOneLine(true);
        discharge.InitOneLine(false);
        openBtn.interactable = false;
        openPanel.SetActive(false);
    }

    public void SetDischarge(bool isOn)
    {
        discharge.OnCheck(isOn);
    }

    private void OpenPanel()
    {
        openBtn.gameObject.SetActive(false);
        openPanel.SetActive(true);
    }

    public void SetOpenBox(UnityAction action)
    {
        _openBtnDisposable?.Clear();
        var disposable = openBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                OpenPanel();
                action?.Invoke();

            }).AddTo(this);
        _openBtnDisposable?.Add(disposable);
    }

    public void SetDelayBtn(UnityAction action)
    {
        _delayBtnDisposable?.Clear();
        var disposable = delayBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                OpenPanel();
                action?.Invoke();

            }).AddTo(this);
        _delayBtnDisposable?.Add(disposable);
    }

    public void SetDischargeBtn(UnityAction action)
    {
        _dischargeActivationBtnDisposable?.Clear();
        var disposable = dischargeActivationBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                OpenPanel();
                action?.Invoke();

            }).AddTo(this);
        _dischargeActivationBtnDisposable?.Add(disposable);
    }

    public void SetDelayBtn(UnityAction action, ManualControlBox.State state)
    {
        if (_delayBtnSubscriptions.ContainsKey(state))
        {
            _delayBtnSubscriptions[state].Dispose();
            _delayBtnSubscriptions.Remove(state);
        }
        var disposable = delayBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                action?.Invoke();

            }).AddTo(this);
        _delayBtnSubscriptions[state] = disposable;
    }

    public void SetDischargeBtn(UnityAction action, State state)
    {
        if (_dischargeActivationBtnSubscriptions.ContainsKey(state))
        {
            _dischargeActivationBtnSubscriptions[state].Dispose();
            _dischargeActivationBtnSubscriptions.Remove(state);
        }
        var disposable = dischargeActivationBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                discharge.OnCheck(true);
                action?.Invoke();

            }).AddTo(this);
        _dischargeActivationBtnSubscriptions[state] = disposable;
    }
    
    
    public void RemoveDelaySubscription(State state)
    {
        if (!_delayBtnSubscriptions.ContainsKey(state))
            return;
        _delayBtnSubscriptions[state].Dispose();
        _delayBtnSubscriptions.Remove(state);
    }
    
    public void RemoveDischargeSubscription(State state)
    {
        if (!_dischargeActivationBtnSubscriptions.ContainsKey(state))
            return;
        _dischargeActivationBtnSubscriptions[state].Dispose();
        _dischargeActivationBtnSubscriptions.Remove(state);
    }
    
}
