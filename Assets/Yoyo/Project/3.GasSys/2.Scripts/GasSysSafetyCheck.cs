using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GASSYS
{
    public enum GasSysSafetyCheckState
    {
        Init, //초기화
        DetachSelectorValve, //기동용기함에서 선택밸브 분리
        DetachStorageCylinder, //기동용기함에서 저장용기 분리
        OpenControlPanel, //제어반 열기
        OpenActivationCylinderBox, // 기동용기함 열기
        DetachSolenoidValve, // 기동용기함에서 솔레노이드밸브 분리
        Complete, // 점검 전 안전점검 완료
    }
    
    public class GasSysSafetyCheck : MonoBehaviour
    {
        [SerializeField] private SelectionValve selectionValveObj;
        [SerializeField] private StorageCylinder storageCylinderObj;
        [SerializeField] private ControlPanel controlPanelObj;
        [SerializeField] private ActivationCylinderBox activationCylinderBoxObj;
        [SerializeField] private SolenoidValve solenoidValveObj;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private GameObject test1;
        [SerializeField] private Button test2;

        
        private GasSysSafetyCheckState _curState = GasSysSafetyCheckState.Init;
        /*
        public void Init()
        {
            //인트로?
            selectionValveObj?.InitSafetyCheck();
            selectionValveObj?.SetDetachBtn(delegate
            {
                ChangeState(GasSysSafetyCheckState.DetachStorageCylinder);
            }, 0);
            selectionValveObj?.SetDetachBtn(delegate
            {
                ChangeState(GasSysSafetyCheckState.DetachStorageCylinder);
            }, 1);

            storageCylinderObj?.InitSafetyCheck();
            storageCylinderObj?.SetDetachBtn(delegate
            {
                ChangeState(GasSysSafetyCheckState.OpenControlPanel);
            });
            controlPanelObj?.InitSafetyCheck();
            controlPanelObj?.SetSolenoidValveModeAndActivateBtn(UpdateSolenoidValveMode, null);
            activationCylinderBoxObj?.InitSafetyCheck(delegate
            {
                ChangeState(GasSysSafetyCheckState.DetachSolenoidValve);
            });
            solenoidValveObj?.Init(SolenoidValve.InitState.SafetyCheck);

            ChangeState(GasSysSafetyCheckState.DetachSelectorValve);
        }

        private void UpdateSolenoidValveMode(ControlMode mode)
        {
            if (ControlMode.Stop.Equals(mode))
            {
                ChangeState(GasSysSafetyCheckState.OpenActivationCylinderBox);
            }
            Debug.Log($"SolenoidValve {mode}");
        }

        public void ChangeState(GasSysSafetyCheckState state)
        {
            _curState = state;
            OnStateChange(_curState);
        }

        private void OnStateChange(GasSysSafetyCheckState state)
        {
            switch (state)
            {

                case GasSysSafetyCheckState.Init:
                    //Init();
                    break;
                case GasSysSafetyCheckState.DetachSelectorValve:
                    ShowObject(selectionValveObj.gameObject);
                    break;
                case GasSysSafetyCheckState.DetachStorageCylinder:
                    ShowObject(storageCylinderObj.gameObject);
                    break;
                case GasSysSafetyCheckState.OpenControlPanel:
                    ShowObject(controlPanelObj.gameObject);
                    break;
                case GasSysSafetyCheckState.OpenActivationCylinderBox:
                    ShowObject(activationCylinderBoxObj.gameObject);
                    break;
                case GasSysSafetyCheckState.DetachSolenoidValve:
                    activationCylinderBoxObj.ShowSafetyCheckSolenoidValvePopup(delegate
                    {
                        ChangeState(GasSysSafetyCheckState.Complete);
                    });
                    break;
                case GasSysSafetyCheckState.Complete:
                    test1.gameObject.SetActive(true);
                    test2.onClick.RemoveAllListeners();
                    test2.onClick.AddListener(delegate
                    {
                        GasSysManager.Instance.ChangeState(GasSysState.Init);
                        test1.gameObject.SetActive(false);
                    });
                    ShowObject(null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

        }

        public void ShowObject(GameObject obj)
        {
            try
            {
                selectionValveObj.gameObject.SetActive(selectionValveObj.gameObject.Equals(obj));
                storageCylinderObj.gameObject.SetActive(storageCylinderObj.gameObject.Equals(obj));
                controlPanelObj.ShowPanel(controlPanelObj.gameObject.Equals(obj));
                activationCylinderBoxObj.gameObject.SetActive(activationCylinderBoxObj.gameObject.Equals(obj));
                //
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        */
    }
}
