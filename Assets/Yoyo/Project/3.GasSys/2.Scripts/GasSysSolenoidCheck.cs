using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GASSYS
{
    public enum GasSysSolenoidCheckState
    {
        Init,
        MenuSelect, //메뉴 선택
        ManualOperationController, //수동조작버튼작동[즉시격발]
        ManualControlBoxController, //수동조작함 작동
        CrossCircuitDetectorController, //교차회로 감지기 동작
        ControlPanelSwitchController, //제어반 수동조작 스위치 동작
        Complete, //완료
    }
    public class GasSysSolenoidCheck : MonoBehaviour
    {
        [SerializeField] private NameListScriptableObj menuBtnScript;

        [SerializeField] private GasSysMenuSelect menuSelectObj;
        [SerializeField] private GasSysManualOperationController gasSysManualOperationController;
        [SerializeField] private GasSysManualControlBoxController gasSysManualControlBoxController;
        [SerializeField] private GasSysCrossCircuitDetectorController gasSysCrossCircuitDetectorController;
        [SerializeField] private GasSysControlPanelSwitchController gasSysControlPanelSwitchController;

        private GasSysSolenoidCheckState _curState = GasSysSolenoidCheckState.Init;
        public void Init()
        {
            menuSelectObj.Init(menuBtnScript);
            int menuIndex = 0;
            foreach (string menuName in menuBtnScript.korNames)
            {
                int index = menuIndex;
                menuSelectObj?.SetButton(menuName, delegate
                {
                    ChangeState(GasSysSolenoidCheckState.ManualOperationController + index);
                });
                ++menuIndex;
            }
            
            ChangeState(GasSysSolenoidCheckState.MenuSelect);
        }
        
        public void ChangeState(GasSysSolenoidCheckState state)
        {
            _curState = state;
            OnStateChanged(_curState);
        }

        private void OnStateChanged(GasSysSolenoidCheckState state)
        {
            switch (state)
            {

                case GasSysSolenoidCheckState.Init:
                    Init();
                    break;
                case GasSysSolenoidCheckState.MenuSelect:
                    ShowObject(menuSelectObj.gameObject);
                    break;
                case GasSysSolenoidCheckState.ManualOperationController:
                    gasSysManualOperationController?.Init();
                    ShowObject(gasSysManualOperationController?.gameObject);
                    break;
                case GasSysSolenoidCheckState.ManualControlBoxController:
                    gasSysManualControlBoxController?.Init();
                    ShowObject(gasSysManualControlBoxController?.gameObject);
                    break;
                case GasSysSolenoidCheckState.CrossCircuitDetectorController:
                    gasSysCrossCircuitDetectorController?.Init();
                    ShowObject(gasSysCrossCircuitDetectorController?.gameObject);
                    break;
                case GasSysSolenoidCheckState.ControlPanelSwitchController:
                    gasSysControlPanelSwitchController?.Init();
                    ShowObject(gasSysControlPanelSwitchController?.gameObject);
                    break;
                case GasSysSolenoidCheckState.Complete:
                    // 완료 됬을시 작업
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        private void ShowObject(GameObject obj)
        {
            try
            {
                menuSelectObj.gameObject.SetActive(menuSelectObj.gameObject.Equals(obj));
                gasSysManualOperationController.gameObject.SetActive(gasSysManualOperationController.gameObject.Equals(obj));
                gasSysManualControlBoxController.gameObject.SetActive(gasSysManualControlBoxController.gameObject.Equals(obj));
                gasSysCrossCircuitDetectorController.gameObject.SetActive(gasSysCrossCircuitDetectorController.gameObject.Equals(obj));
                gasSysControlPanelSwitchController.gameObject.SetActive(gasSysControlPanelSwitchController.gameObject.Equals(obj));
                
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

    }
}
