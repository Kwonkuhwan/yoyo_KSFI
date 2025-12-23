using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GasSysManualControlBoxController : MonoBehaviour
{
    [SerializeField] private ManualControlBox manualControlBox;
    [SerializeField] private ActivationCylinderBox activationCylinderBox;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private ControlMode _curMode = ControlMode.Auto;
    public void Init()
    {
        _curMode = ControlMode.Stop;
        ControlPanel.Instance.InitManualControlBox();
        _disposable?.Clear();
        var disposable = ControlPanel.Instance.onSwitchBtnValueChangeEvent.AsObservable()
            .Subscribe(data =>
            {
                Debug.Log($"{data.Item1}, {data.Item2}");
                switch (data.Item1)
                {
                    case "주경종":
                        {
                            string isOn = !data.Item2 ? "On" : "Off";
                            ControlPanel.Instance.SetReceiverLog($"{data.Item1} 음향 {isOn}");
                            break;
                        }
                    case "지구경종":
                        {
                            string isOn = !data.Item2 ? "On" : "Off";
                            ControlPanel.Instance.SetReceiverLog($"{data.Item1} 음향 {isOn}");
                            break;
                        }
                    case "사이렌":
                        {
                            string isOn = !data.Item2 ? "On" : "Off";
                            if (_curMode.Equals(ControlMode.Auto))
                            {
                                ControlPanel.Instance.SetReceiverLog($"{data.Item1} 음향 {isOn}");
                                if(data.Item2)
                                    activationCylinderBox.InitManualControlBox();
                            }
                            break;
                        }
                    case "비상방송":
                        {
                            string isOn = !data.Item2 ? "On" : "Off";
                            ControlPanel.Instance.SetReceiverLog($"{data.Item1} 음향 {isOn}");
                            break;
                        }
                }
            }).AddTo(this);
        _disposable?.Add(disposable);
        
        manualControlBox.SetDischargeBtn(() =>
        {
            ControlPanel.Instance.ShowFire(true);
            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);
            ControlPanel.Instance.SetTimeNum(30f);
            manualControlBox.gameObject.SetActive(false);
            activationCylinderBox.InitManualControlBox();
            ControlPanel.Instance.StartTimer(30f);
        });
        
        var timeEnd = ControlPanel.OnTimerEnd.AsObservable().Subscribe(_ =>
        {
            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
            activationCylinderBox.SetSolenoidValveActivationImg(true);
        }).AddTo(this);
        _disposable?.Add(timeEnd);
        ControlPanel.Instance.SetSolenoidValveModeAndActivateBtn(mode =>
        {
            _curMode = mode;
            if (!_curMode.Equals(ControlMode.Auto))
                return;
            ControlPanel.Instance.SetTimeNum(30f);
            manualControlBox.InitManualControlBox();
            manualControlBox.gameObject.SetActive(true);
            manualControlBox.SetOpenBox(() =>
            {
                //경보음
            });
            //ControlPanel.Instance.StartTimer(30f);
           

        }, null);
    }

    private void OnDisable()
    {
        manualControlBox.gameObject.SetActive(false);
        _disposable?.Clear();
    }
}
