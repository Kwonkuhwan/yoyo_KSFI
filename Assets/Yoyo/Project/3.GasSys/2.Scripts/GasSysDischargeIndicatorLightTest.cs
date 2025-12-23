using System;
using UnityEngine;
using UnityEngine.UI;

namespace GASSYS
{
    public class GasSysDischargeIndicatorLightTest : MonoBehaviour
    {
        [SerializeField] private ActivationCylinderBox activationCylinderBox;
        [SerializeField] private DischargeIndicatorLight dischargeIndicatorLight;
        [SerializeField] private ManualControlBox manualControlBox;
        [SerializeField] private Button openActivationCylinderBoxBtn; 
        public void Init()
        {
            ControlPanel.Instance.InitDischargeIndicatorLightTest();
            manualControlBox.gameObject.SetActive(true);
            manualControlBox.InitManualControlBox();
            dischargeIndicatorLight.gameObject.SetActive(true);
            dischargeIndicatorLight.Init();
            activationCylinderBox.InitDischargeIndicatorLightTest(() =>
            {
                ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, true);
                dischargeIndicatorLight.SetActivate(true);
                manualControlBox.SetDischarge(true);
                activationCylinderBox.gameObject.SetActive(false);
                ControlPanel.Instance.ShowFire(true);
            }, () =>
            {
                ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);
                
                dischargeIndicatorLight.SetActivate(false);
                manualControlBox.SetDischarge(false);
                activationCylinderBox.gameObject.SetActive(false);
                ControlPanel.Instance.ShowFire(false);
            });
            
            openActivationCylinderBoxBtn.onClick.RemoveAllListeners();
            openActivationCylinderBoxBtn.onClick.AddListener(() =>
            {
                dischargeIndicatorLight.gameObject.SetActive(true);
                activationCylinderBox.gameObject.SetActive(true);
            });

        }

        private void OnDisable()
        {
            dischargeIndicatorLight.gameObject.SetActive(false);
        }
    }
}
