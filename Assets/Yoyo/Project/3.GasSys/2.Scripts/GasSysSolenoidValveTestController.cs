using System;
using System.Collections.Generic;
using LJS;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
namespace GASSYS
{
    public enum SolenoidValveTestState
    {
        Init,
        MenuSelect, //메뉴 선택
        ManualOperationController, //수동조작버튼작동[즉시격발]
        ManualControlBoxController, //수동조작함 작동
        CrossCircuitDetectorController, //교차회로 감지기 동작
        ControlPanelSwitchController, //제어반 수동조작 스위치 동작
        Complete, //완료
    }
    public class GasSysSolenoidValveTestController : MonoBehaviour
    {
        [SerializeField] private NameListScriptableObj menuBtnScript;

        [SerializeField] private GasSysMenuSelect menuSelectObj;
        [SerializeField] private List<MenuButtonObj> menuBtns;
        [SerializeField] private GasSysManualOperationController gasSysManualOperationController;
        [SerializeField] private GasSysManualControlBoxController gasSysManualControlBoxController;
        [SerializeField] private GasSysCrossCircuitDetectorController gasSysCrossCircuitDetectorController;
        [SerializeField] private GasSysControlPanelSwitchController gasSysControlPanelSwitchController;
        [FormerlySerializedAs("inventory")]
        [SerializeField] private Inventory inventoryObj;

        [SerializeField] private GasSysAreaManager areaManagerObj;

        [SerializeField] private GasSysStorageRoom storageRoomObj;
        [SerializeField] private GasSysArea1 area1Obj;
        [SerializeField] private GasSysArea1Corridor area1CorridorObj;
        [SerializeField] private GasSysSection sectionObj;


        private SolenoidValveTestState _curState = SolenoidValveTestState.Init;
        public void Init()
        {
            //menuSelectObj.Init(menuBtnScript);
            int menuIndex = 0;
            foreach (var menuBtn in menuBtns)
            {
                int index = menuIndex;
                menuBtn.SetButton(delegate
                {
                    ChangeState(SolenoidValveTestState.ManualOperationController + index);
                });
                ++menuIndex;
            }
            // foreach (string menuName in menuBtnScript.korNames)
            // {
            //     int index = menuIndex;
            //     string regexName = Util.RemoveWhitespaceUsingRegex(menuName);
            //     menuSelectObj?.SetButton(regexName, delegate
            //     {
            //         ChangeState(SolenoidValveTestState.ManualOperationController + index);
            //     });
            //     ++menuIndex;
            // }

            ChangeState(SolenoidValveTestState.MenuSelect);
            inventoryObj?.ShowPanel(false);
        }

        public void ChangeState(SolenoidValveTestState state)
        {
            _curState = state;
            OnStateChanged(_curState);
        }

        private void OnStateChanged(SolenoidValveTestState state)
        {
            switch (state)
            {
                case SolenoidValveTestState.Init:
                    Init();
                    break;
                case SolenoidValveTestState.MenuSelect:
                    ShowObject(menuSelectObj.gameObject);
                    break;
                case SolenoidValveTestState.ManualOperationController:
                    //gasSysManualOperationController?.Init();
                    ShowObject(gasSysManualOperationController?.gameObject);
                    GasSysGlobalCanvas.Instance.SetSubTitle("수동조작버튼작동[즉시격발]");
                    sectionObj.InitManualOperation();
                    // areaManagerObj?.StartArea(GasSysAreaManager.StarAreaType.StorageRoom);
                    // areaManagerObj?.ShowPanel(true);
                    // storageRoomObj?.InitManualOperationController();
                    // ShowRoom(storageRoomObj?.gameObject);
                    // inventoryObj?.InitManualOperationController();
                    // inventoryObj?.ShowPanel(true);
                    
                    break;
                case SolenoidValveTestState.ManualControlBoxController:
                    //gasSysManualControlBoxController?.Init();
                    ShowObject(gasSysManualControlBoxController?.gameObject);
                    GasSysGlobalCanvas.Instance.SetSubTitle("수동조작함 작동");
                    sectionObj.InitManualControlBox();
                    
                    // areaManagerObj?.StartArea(GasSysAreaManager.StarAreaType.StorageRoom);
                    // areaManagerObj?.ShowPanel(true);
                    // storageRoomObj?.InitManualControlBox();
                    // area1CorridorObj?.InitManualControlBox(delegate
                    // {
                    //     ControlPanel.Instance.SetArea1CorridorPopupParent();
                    //     ControlPanel.Instance.ShowPanel(true);
                    // }, delegate
                    // {
                    //     //GlobalCanvas.Instance.ShowHint(false);
                    //     storageRoomObj?.ShowSolenoidValvePopup(true);
                    //     ControlPanel.Instance.ShowFire(true);
                    //     ControlPanel.Instance.SetTimeNum(30f);
                    //     ControlPanel.Instance.StartTimer(30);
                    //     ControlPanel.OnTimerEnd.AddListener(delegate
                    //     {
                    //         storageRoomObj?.ManualActivateToSolenoidValve();
                    //         ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                    //         Observable.Timer(System.TimeSpan.FromSeconds(3))
                    //             .Subscribe(_ =>
                    //             {
                    //                 GasSysManager.Instance.ChangeState(GasSysState.Init);
                    //             })
                    //             .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
                    //     });
                    //     ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);
                    // });
                    // ShowRoom(storageRoomObj?.gameObject);
                    // inventoryObj?.InitManualControlBox();
                    // inventoryObj?.ShowPanel(true);
                    break;
                case SolenoidValveTestState.CrossCircuitDetectorController:
                    GasSysGlobalCanvas.Instance.SetSubTitle("교차회로 감지기 작동");
                    ShowObject(gasSysCrossCircuitDetectorController?.gameObject);
                    sectionObj.InitCrossCircuitDetector();
                    // areaManagerObj?.StartArea(GasSysAreaManager.StarAreaType.StorageRoom);
                    // areaManagerObj?.ShowPanel(true);
                    // storageRoomObj?.InitCrossCircuitDetector();
                    // area1Obj?.InitCrossCircuitDetector(delegate
                    // {
                    //     storageRoomObj?.ShowSolenoidValvePopup(true);
                    //     ControlPanel.Instance.ShowFire(true);
                    //     ControlPanel.Instance.SetTimeNum(30f);
                    //     ControlPanel.Instance.StartTimer(30);
                    //     ControlPanel.OnTimerEnd.RemoveAllListeners();
                    //     ControlPanel.OnTimerEnd.AddListener(delegate
                    //     {
                    //         storageRoomObj?.ManualActivateToSolenoidValve();
                    //         ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                    //         Observable.Timer(System.TimeSpan.FromSeconds(3))
                    //             .Subscribe(_ =>
                    //             {
                    //                 GasSysManager.Instance.ChangeState(GasSysState.Init);
                    //             })
                    //             .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
                    //     });
                    //     //ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateManualControlBox, true);
                    // });
                    // ShowRoom(storageRoomObj?.gameObject);
                    // inventoryObj?.InitCrossCircuitDetector();
                    // inventoryObj?.ShowPanel(true);
                    // gasSysCrossCircuitDetectorController?.Init();
                    // ShowObject(gasSysCrossCircuitDetectorController?.gameObject);
                    break;
                case SolenoidValveTestState.ControlPanelSwitchController:
                    GasSysGlobalCanvas.Instance.SetSubTitle("제어반 수동조작 스위치 작동");
                    ShowObject(null);
                    // areaManagerObj?.StartArea(GasSysAreaManager.StarAreaType.StorageRoom);
                    // areaManagerObj?.ShowPanel(true);
                    // storageRoomObj?.InitControlPanelSwitch();
                    // ShowRoom(storageRoomObj?.gameObject);
                    // inventoryObj?.ControlPanelSwitch();
                    // inventoryObj?.ShowPanel(true);
                    sectionObj.InitControlPanelSwitch();
                  
                    //GlobalCanvas.Instance.SetTitleAndBack("수동조작버튼작동[즉시격발] | 약제 저장실", null);
                    // gasSysControlPanelSwitchController?.Init();
                    // ShowObject(gasSysControlPanelSwitchController?.gameObject);
                    break;
                case SolenoidValveTestState.Complete:
                    // 완료 됬을시 작업
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void ShowRoom(GameObject obj)
        {
            storageRoomObj.gameObject.SetActive(storageRoomObj.gameObject.Equals(obj));
            area1Obj.gameObject.SetActive(area1Obj.gameObject.Equals(obj));
            area1CorridorObj.gameObject.SetActive(area1CorridorObj.gameObject.Equals(obj));
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
