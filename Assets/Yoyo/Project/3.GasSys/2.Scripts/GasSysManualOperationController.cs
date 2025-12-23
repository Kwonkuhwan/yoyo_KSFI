using UnityEngine;

public class GasSysManualOperationController : MonoBehaviour
{
    [SerializeField] private ControlPanel controlPanel;
    [SerializeField] private ActivationCylinderBox activationCylinderBox;
    public void Init()
    {
        ControlPanel.Instance.InitManualOperationController();
        ControlPanel.Instance.SetSolenoidValveModeAndActivateBtn(mode =>
        {
            if (mode.Equals(ControlMode.Auto))
            {
                activationCylinderBox.InitManualOperationController(() =>
                {
                    //ControlPanel.Instance.SetArea1Check(new [] {"1번 구역 솔레노이드"}, true);
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                });
            }

        }, null);
    }
}
