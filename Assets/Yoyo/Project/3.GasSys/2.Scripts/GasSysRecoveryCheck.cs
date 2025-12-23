using System;
using System.Collections;
using System.Collections.Generic;
using LJS;
using UniRx;
using UnityEngine;
namespace GASSYS
{
    public class GasSysRecoveryCheck : MonoBehaviour
    {
        [SerializeField] private ActivationCylinderBox activationCylinderBox;
        [SerializeField] private DefaultPopupObj solenoidRecoveryPopup;
        [SerializeField] private StorageCylinder storageCylinder;
        [SerializeField] private SelectionValve selectionValve;
        private CompositeDisposable _disposable = new CompositeDisposable();
        public void Init()
        {
            ControlPanel.Instance.InitRecoveryCheck();
            ControlPanel.Instance.SetSolenoidValveModeAndActivateBtn(UpdateSolenoidValveControl, null);
            _disposable?.Clear();
            var disposable = ControlPanel.Instance.onSwitchBtnValueChangeEvent.AsObservable()
                .Subscribe(data =>
                {
                    var dataName = Util.RemoveWhitespaceUsingRegex(data.Item1);
                    Debug.Log($"{data.Item1}, {data.Item2}");
                    switch (dataName)
                    {
                      case "축적/비축적":
                          selectionValve?.gameObject.SetActive(true);
                          selectionValve?.InitRecoveryCheck();
                          selectionValve?.SetAttachBtn(() =>
                          {
                              selectionValve?.gameObject.SetActive(false);
                              storageCylinder?.gameObject.SetActive(true);
                              storageCylinder?.InitRecoveryCheck();
                              storageCylinder?.SetAttachBtn(() =>
                              {
                                  storageCylinder?.gameObject.SetActive(false);
                              });
                          }, 0);
                          break;
                          
                    }
                }).AddTo(this);
            _disposable?.Add(disposable);
            
        }

        private void UpdateSolenoidValveControl(ControlMode mode)
        {
            switch (mode)
            {
                case ControlMode.Manual:
                    solenoidRecoveryPopup.gameObject.SetActive(true);
                    solenoidRecoveryPopup.Init(() =>
                    {
                        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
                        solenoidRecoveryPopup.gameObject.SetActive(false);
                        activationCylinderBox.InitRecoveryCheck();
                    });
                    break;
                case ControlMode.Auto:
                    activationCylinderBox.InitRecoveryCheck2();
                    break;
                case ControlMode.Stop:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

        }
    }
}