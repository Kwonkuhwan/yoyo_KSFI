using System;
using System.Collections.Generic;
using UnityEngine;

namespace GASSYS
{
    public enum GasSysPracticeModeState
    {
        Init,
        MenuSelect, //메뉴 선택
        SafetyCheck, //점검 전 안전조치(공통)
        // FireDetect, //교차 화로 감지 확인 1 (열 감지기)
        // AlarmCheck, //감시 제어반 음향 확인
        // SmokeDetect, //교차 화로 감지 확인 2 (연기 감지기)
        // SirenCheck, //감시 제어반 사이렌 음향 확인
        SolenoidCheck, //솔레노이드 격발 확인
        DischargeCheck, //방출표시등 점검 확인(공통)
        RecoveryCheck, //점검 후 복구(공통)
        Complete, //실습 모드 완료
    }
    public class GasSysPracticeMode : MonoBehaviour
    {
        [SerializeField] private UIDragAndCollisionHandler dragAndCollisionHandler;
        [SerializeField] private NameListScriptableObj menuBtnScript;
        
        [SerializeField] private GasSysMenuSelect menuSelectObj;
        [SerializeField] private List<MenuButtonObj> menuBtns;
        [SerializeField] public GasSysSafetyCheck safetyCheckObj;
        [SerializeField] public GasSysSolenoidValveTestController solenoidValveTestControllerObj;
        [SerializeField] public GasSysDischargeIndicatorLightTest dischargeIndicatorLightTestObj;
        [SerializeField] public GasSysRecoveryCheck recoveryCheckObj;
        [SerializeField] public GasSysAreaManager areaManagerObj;
        
        [SerializeField] private GasSysStorageRoom storageRoomObj;
        [SerializeField] private GasSysArea1 area1Obj;
        [SerializeField] private GasSysArea1Corridor area1CorridorObj;

        [SerializeField] private Inventory inventoryObj;
        [SerializeField] private GasSysSection sectionObj;
        
        private ControlMode _curControlMode;
        private GasSysMainSection _curState = GasSysMainSection.Init; 
        public void Init()
        {
            //menuSelectObj?.Init(menuBtnScript);
          
         
            int menuIndex = 0;
            foreach (var menuBtn in menuBtns)
            {
                int index = menuIndex;
                menuBtn.SetButton(delegate
                {
                    ChangeState(GasSysMainSection.SafetyCheck+index);
                });
                ++menuIndex;
            }
            /*
            foreach (string menuName in menuBtnScript.korNames)
            {
               
                menuSelectObj?.SetButton(menuName, delegate
                {
                    ChangeState(GasSysPracticeModeState.SafetyCheck+index);
                });
               
            }
            */
            ControlPanel.Instance.ResetTimer();
            //ChangeState(GasSysMainSection.MenuSelect);
            areaManagerObj.Init(delegate
            {
                ShowRoom(area1Obj.gameObject);
            }, delegate
            {
                ShowRoom(area1CorridorObj.gameObject);
            }, delegate
            {
                ShowRoom(storageRoomObj.gameObject);
            }, null);
            inventoryObj.ShowPanel(false);
            ShowRoom(null);
        }
        public void ChangeState(GasSysMainSection state)
        {
            _curState = state;
            OnStateChanged(_curState);
        }

        private void OnStateChanged(GasSysMainSection state)
        {
            switch (state)
            {

                case GasSysMainSection.Init:
                    Init();
                    // GlobalCanvas.Instance.SetBackBtn(delegate
                    // {
                    //     GasSysManager.Instance.ChangeState(GasSysState.Init);
                    // });
                    ShowObject(menuSelectObj.gameObject);
                    GasSysGlobalCanvas.Instance.SetTitle("모드 선택");
                    inventoryObj.ShowPanel(false);
                    dragAndCollisionHandler.ResetEvent();

                    break;
                // case GasSysPracticeModeState.MenuSelect:
                //     ShowObject(menuSelectObj.gameObject);
                //     GlobalCanvas.Instance.SetTitle("모드 선택");
                //     inventoryObj.ShowPanel(false);
                //     break;
                case GasSysMainSection.SafetyCheck:
                    //safetyCheckObj?.Init();
                    //safetyCheckSectionObj.Init();
                    //sectionObj.InitSafetyCheck();
                    /*
                    SectionManager.Instance.InitSafety();
                    areaManagerObj?.StartArea(GasSysAreaManager.StarAreaType.StorageRoom);
                    areaManagerObj?.ShowPanel(true);
                    ShowRoom(storageRoomObj?.gameObject);
                    storageRoomObj?.InitSafetyCheck();
                    inventoryObj?.InitSafetyCheck();
                    inventoryObj?.ShowPanel(true);
                    */
                    GasSysGlobalCanvas.Instance.SetTitle("점검 전 안전 조치");
                    
                    //GlobalCanvas.Instance.SetTitleAndBack("점검 전 안전 조치","약제 저장실", null);
                    //ShowObject(safetyCheckObj?.gameObject);
                    break;
                case GasSysMainSection.SolenoidValveTest:
                    GasSysGlobalCanvas.Instance.SetTitle("기동용기 솔레노이드 밸브 격발시험");
                    solenoidValveTestControllerObj?.Init();
                    ShowObject(solenoidValveTestControllerObj?.gameObject);
                    break;
                case GasSysMainSection.DischargeLightTest:
                    GasSysGlobalCanvas.Instance.SetTitle("방출표시등 작동시험");
                    sectionObj.InitDischargeLightTest();
                    // bool downSwitch = false;
                    // areaManagerObj?.Init(delegate
                    // {
                    //     ShowRoom(area1Obj.gameObject);
                    // }, delegate
                    // {
                    //     ShowRoom(area1CorridorObj.gameObject);
                    //     //GlobalCanvas.Instance.ShowHint(false);
                    // }, delegate
                    // {
                    //     ShowRoom(storageRoomObj.gameObject);
                    //     if (downSwitch)
                    //         return;
                    //     if (false == area1CorridorObj?.GetDischarge())
                    //         return;
                    //     downSwitch = true;
                    //     storageRoomObj?.InitDischargeDownCheck(delegate
                    //     {
                    //         area1CorridorObj.SetDischarge(false);
                    //     });
                    // }, null);
                    // areaManagerObj?.StartArea(GasSysAreaManager.StarAreaType.StorageRoom);
                    // areaManagerObj?.ShowPanel(true);
                    //
                    // area1CorridorObj?.InitDischargeCheck();
                    // storageRoomObj?.InitDischargeUpCheck(delegate
                    // {
                    //     area1CorridorObj.SetDischarge(true);
                    // });
                    // //dischargeIndicatorLightTestObj?.Init();
                    // //ShowObject(dischargeIndicatorLightTestObj?.gameObject);
                    //ShowRoom(storageRoomObj?.gameObject);
           
                    //inventoryObj.InitDischargeCheck();
                    //inventoryObj.ShowPanel(true);
                    break;
                case GasSysMainSection.RecoveryCheck:
                    GasSysGlobalCanvas.Instance.SetTitle("점검완료 후 복구");
                    sectionObj.InitRecoveryCheck();
                    // areaManagerObj?.StartArea(GasSysAreaManager.StarAreaType.StorageRoom);
                    // areaManagerObj?.ShowPanel(true);
                    // ShowRoom(storageRoomObj?.gameObject);
                    // storageRoomObj?.InitRecoveryCheck();
                    //
                    // //recoveryCheckObj?.Init();
                    // //ShowObject(recoveryCheckObj?.gameObject);
                    // inventoryObj.InitRecoveryCheck();
                    // inventoryObj.ShowPanel(true);
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
                safetyCheckObj.gameObject.SetActive(safetyCheckObj.gameObject.Equals(obj));
                solenoidValveTestControllerObj.gameObject.SetActive(solenoidValveTestControllerObj.gameObject.Equals(obj));
                dischargeIndicatorLightTestObj.gameObject.SetActive(dischargeIndicatorLightTestObj.gameObject.Equals(obj));
                recoveryCheckObj.gameObject.SetActive(recoveryCheckObj.gameObject.Equals(obj));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void ShowRoom(GameObject obj)
        {
            storageRoomObj.gameObject.SetActive(storageRoomObj.gameObject.Equals(obj));
            area1Obj.gameObject.SetActive(area1Obj.gameObject.Equals(obj));
            area1CorridorObj.gameObject.SetActive(area1CorridorObj.gameObject.Equals(obj));
        }

        public void HideObject()
        {
            //safetyCheckObj.ShowObject(null);
            ShowObject(null);
        }
    }
}