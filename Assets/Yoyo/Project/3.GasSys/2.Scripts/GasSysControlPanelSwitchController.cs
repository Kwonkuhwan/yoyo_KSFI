using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GasSysControlPanelSwitchController : MonoBehaviour
{
    [SerializeField] private ActivationCylinderBox activationCylinderBox;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private ControlMode _curSolenoidValveMode = ControlMode.Stop;
    private ControlMode _curArea1Mode = ControlMode.Auto;

    public void Init()
    {
        _disposable?.Clear();

        
        _curSolenoidValveMode = ControlMode.Stop;
        _curArea1Mode = ControlMode.Auto;
        ControlPanel.Instance.InitControlPanelSwitch();
        ControlPanel.Instance.SetSolenoidValveModeAndActivateBtn(mode =>
        {
            _curSolenoidValveMode = mode;
        }, null);
        ControlPanel.Instance.SetArea1ModeAndActivateBtn(mode =>
        {
            _curArea1Mode = mode;
        }, () =>
        {
            if (_curSolenoidValveMode == ControlMode.Manual 
            && _curArea1Mode == ControlMode.Manual)
            {
                ControlPanel.Instance.StartTimer(30f);
                activationCylinderBox.InitControlPanelSwitch();
            }
        });
        ControlPanel.Instance.SetTimeNum(30f);
        var timeEnd = ControlPanel.OnTimerEnd.AsObservable()
            .Subscribe(_ =>
            {
                ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                activationCylinderBox.SetSolenoidValveActivationImg(true);
                ControlPanel.Instance.ShowFire(true);
            }).AddTo(this);
        _disposable?.Add(timeEnd);

    }

    private void OnDisable()
    {
        activationCylinderBox.gameObject.SetActive(false);
        _disposable?.Clear();
    }
}
