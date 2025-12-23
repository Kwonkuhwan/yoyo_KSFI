using System;
using GASSYS;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GasSysStorageRoom : MonoBehaviour
{
    /*
#region 점검 전 안전 조치 힌트

    [FormerlySerializedAs("_selectionValveHint")]
    [SerializeField]
    private HintScriptableObj _safetyCheckHint;
    [SerializeField] private RectTransform[] _safetyCheckHintRects;

#endregion //점검 전 안전 조치 힌트

#region 기동용기 솔레노이드밸브 격발시험 -> 즉시격발

    [SerializeField] private HintScriptableObj _manualOperationControllerHint;
    [SerializeField] private RectTransform[] _manualOperationControllerHintRects;

#endregion //기동용기 솔레노이드밸브 격발시험 -> 즉시격발

#region 기동용기 솔레노이드밸브 격발시험 -> 수동조작함 작동

    [SerializeField] private HintScriptableObj _manualControlBoxControllerHint;
    [SerializeField] private RectTransform[] _manualControlBoxControllerHintRects;

#endregion //기동용기 솔레노이드밸브 격발시험 -> 수동조작함 작동

#region 기동용기 솔레노이드밸브 -> 교차회로 감지기 동작

    [SerializeField] private HintScriptableObj _crossCircuitDetectorHint;
    [SerializeField] private RectTransform[] _crossCircuiteDetectorHintRects;

#endregion

#region 기동용기 솔레노이드밸브 -> 제어반 수동조작 스위치 동작

    [SerializeField] private HintScriptableObj _ControlPanelSwitchHint;
    [SerializeField] private RectTransform[] _ControlPanelSwitchHintRects;

#endregion

#region 방출표시등 점검 확인

    [SerializeField] private HintScriptableObj _DischargeCheckHint;
    [SerializeField] private RectTransform[] _DischargeCheckHintRects;

#endregion

#region 점검 후 복구
    [SerializeField] private HintScriptableObj _RecoveryCheckHint;
    [SerializeField] private RectTransform[] _RecoveryCheckHintRects;
#endregion //점검 후 복구

#region 선택밸브

    [SerializeField] private SelectionValve selectionValve01;
    [SerializeField] private SelectionValve selectionValve02;
    [SerializeField] private StorageCylinder storageCylinder;

    [SerializeField] private SelectionValvePopup selectionValvePopup;
    [SerializeField] private StorageCylinderPopup storageCylinderPopup;
    [SerializeField] private ControlPanel controlPanelPopup;
    [SerializeField] private ActivationCylinderBoxPopup activationCylinderBoxPopup;
    [SerializeField] private SolenoidValvePopup solenoidValvePopup;

    [SerializeField] private Button selectionValveBtn;
    [SerializeField] private Button controlBoxBtn;
    [SerializeField] private Button activationBoxBtn;
    [SerializeField] private Button storageCylinderBtn;

    [SerializeField] private UIDragAndCollisionHandler uiDragAndCollisionHandler;

    private ControlMode _curControlMode;

    //private ControlMode _curControlMode = ControlMode.Stop;
    private ControlMode _curArea1Mode = ControlMode.Auto;
    //private SolenoidValveTestState curMode;

#endregion //선택밸브
    // Start is called before the first frame update
    private void Start()
    {
        //InitSafetyCheck();
    }
    private void Init()
    {
        selectionValveBtn.gameObject.SetActive(false);
        controlBoxBtn.gameObject.SetActive(false);
        activationBoxBtn.gameObject.SetActive(false);
        storageCylinderBtn.gameObject.SetActive(false);

        selectionValvePopup.gameObject.SetActive(false);
        storageCylinderPopup.gameObject.SetActive(false);
        controlPanelPopup.ShowPanel(false);
        activationCylinderBoxPopup.gameObject.SetActive(false);
        solenoidValvePopup.gameObject.SetActive(false);

        selectionValveBtn.onClick.RemoveAllListeners();
        controlBoxBtn.onClick.RemoveAllListeners();
        activationBoxBtn.onClick.RemoveAllListeners();
        storageCylinderBtn.onClick.RemoveAllListeners();
        //uiDragAndCollisionHandler.ResetEvent();
    }
#region 점검 전 안전조치

    public void InitSafetyCheck()
    {
        Init();
        selectionValveBtn.gameObject.SetActive(true);
        //SectionManager.Instance.NextDisable();
        //SectionManager.Instance.();
        //GlobalCanvas.Instance.SetTitleAndBack("양제저장실", );
        ////GlobalCanvas.Instance.SetHintPopup(0, 0, _safetyCheckHint, _safetyCheckHintRects[0]);
        //GlobalCanvas.Instance.ShowHint(true);

        selectionValveBtn.onClick.AddListener(delegate
        {
            //SectionManager.Instance.NextEnable();
            //SectionManager.Instance.Next();
            //GlobalCanvas.Instance.SetHintPopup(1, 1, _safetyCheckHint, _safetyCheckHintRects[1]);
            //GlobalCanvas.Instance.ShowHint(true);
            selectionValvePopup.InitSafetyCheck(true, selectionValve01.ChangeValveState, delegate
            {
                //GlobalCanvas.Instance.SetHintPopup(2, 2, _safetyCheckHint, _safetyCheckHintRects[2]);
                selectionValveBtn.gameObject.SetActive(false);
                storageCylinderBtn.gameObject.SetActive(true);
            });
            selectionValvePopup.gameObject.SetActive(true);
        });

        storageCylinderBtn.onClick.AddListener(delegate
        {
            //GlobalCanvas.Instance.SetHintPopup(3, 3, _safetyCheckHint, _safetyCheckHintRects[3]);
            storageCylinderPopup.InitSafetyCheck(true, storageCylinder.ChangeValveState, delegate
            {
                //GlobalCanvas.Instance.SetHintPopup(4, 4, _safetyCheckHint, _safetyCheckHintRects[4]);
                storageCylinderBtn.gameObject.SetActive(false);
                controlBoxBtn.gameObject.SetActive(true);
            });
            storageCylinderPopup.gameObject.SetActive(true);
        });

        controlBoxBtn.onClick.AddListener(delegate
        {
            //GlobalCanvas.Instance.SetHintPopup(5, 6, _safetyCheckHint, _safetyCheckHintRects[5]);
            controlPanelPopup.InitNewSafetCheck(delegate
            {
                //GlobalCanvas.Instance.SetHintPopup(7, 7, _safetyCheckHint, _safetyCheckHintRects[7]);
                controlBoxBtn.gameObject.SetActive(false);
                activationBoxBtn.gameObject.SetActive(true);
            });
            controlPanelPopup.SetTimeNum(0f);
        });

        activationBoxBtn.onClick.AddListener(delegate
        {
            //GlobalCanvas.Instance.SetHintPopup(8, 9, _safetyCheckHint, _safetyCheckHintRects[8]);
            activationCylinderBoxPopup.InitSafetyCheck(delegate
            {
                //GlobalCanvas.Instance.SetHintPopup(10, 10, _safetyCheckHint, _safetyCheckHintRects[9]);
                solenoidValvePopup.InitSafetyCheck();
                solenoidValvePopup.gameObject.SetActive(true);
            });
            activationCylinderBoxPopup.gameObject.SetActive(true);
        });

        selectionValve01.InitNewSafetyCheck();
        selectionValve02.InitNewSafetyCheck();
        storageCylinder.InitNewSafetyCheck();
    }

    private void SafetyCheck(bool isSafety)
    {

    }

#endregion //점검전 안전조치

#region 수동조작버튼작동(즉시격발)

    public void InitManualOperationController()
    {
        Init();
        //uiDragAndCollisionHandler.OnCollisionDetected -= HandleCollision;
        uiDragAndCollisionHandler.OnCollisionDetected += (draggedObj, targetObj) =>
        {
            if (!ControlMode.Auto.Equals(_curControlMode))
                return;
            //GlobalCanvas.Instance.SetHintPopup(4, 4, _manualOperationControllerHint, _manualOperationControllerHintRects[2]);
            solenoidValvePopup.InitManualOperationController();
            solenoidValvePopup.gameObject.SetActive(true);
        };
        //GlobalCanvas.Instance.SetHintPopup(0, 0, _manualOperationControllerHint, _manualOperationControllerHintRects[0]);
        //GlobalCanvas.Instance.ShowHint(true);
        controlBoxBtn.gameObject.SetActive(true);
        _curControlMode = ControlMode.Stop;
        controlBoxBtn.onClick.AddListener(delegate
        {
            //GlobalCanvas.Instance.SetHintPopup(1, 3, _manualOperationControllerHint, _manualOperationControllerHintRects[1]);
            controlPanelPopup.InitNewManualOperationController(delegate
            {

            }, mode =>
            {
                _curControlMode = mode;
            });
        });
    }

    //private void 

#endregion //수동조작버튼작동(즉시격발)

#region 수동조작함 작동

    public void InitManualControlBox()
    {
        Init();
        _curControlMode = ControlMode.Stop;
        controlBoxBtn.gameObject.SetActive(true);
        //GlobalCanvas.Instance.SetHintPopup(0, 0, _manualControlBoxControllerHint, _manualOperationControllerHintRects[0]);
        //GlobalCanvas.Instance.ShowHint(true);
        controlBoxBtn.onClick.AddListener(delegate
        {
            //GlobalCanvas.Instance.SetHintPopup(1, 2, _manualControlBoxControllerHint, _manualControlBoxControllerHintRects[1]);
            controlPanelPopup.InitNewManualControlBox(delegate
            {
                //GlobalCanvas.Instance.SetHintPopup(3, 3, _manualControlBoxControllerHint, _manualControlBoxControllerHintRects[2]);
                controlBoxBtn.gameObject.SetActive(false);
                solenoidValvePopup.InitManualControlBox();

            }, mode =>
            {
                _curControlMode = mode;
            });
        });
    }

    public void ShowSolenoidValvePopup(bool show)
    {
        solenoidValvePopup.gameObject.SetActive(show);
    }
    public void ManualActivateToSolenoidValve()
    {
        solenoidValvePopup.SolenoidValveAActivation();
    }

#endregion //소동조작함 작동

#region 교차회로감지기 작동

    public void InitCrossCircuitDetector()
    {
        Init();
        _curControlMode = ControlMode.Stop;
        controlBoxBtn.gameObject.SetActive(true);
        controlBoxBtn.onClick.AddListener(delegate
        {
            controlPanelPopup.ShowPanel(true);
            //GlobalCanvas.Instance.SetHintPopup(0, 1, _crossCircuitDetectorHint, _crossCircuiteDetectorHintRects[0]);
            //GlobalCanvas.Instance.ShowHint(true);

        });
        controlPanelPopup.InitNewCrossCircuitDetector(delegate
        {
            controlBoxBtn.gameObject.SetActive(false);
            //GlobalCanvas.Instance.SetHintPopup(2, 2, _crossCircuitDetectorHint, _crossCircuiteDetectorHintRects[1]);
            solenoidValvePopup.InitCrossCircuitDetector();

        }, mode =>
        {
            _curControlMode = mode;
        });
    }

#endregion //교차회로감지기 작동

#region 제어반 수동조작스위치 작동

    public void InitControlPanelSwitch()
    {
        Init();
        _curControlMode = ControlMode.Stop;
        controlBoxBtn.gameObject.SetActive(true);

        controlBoxBtn.onClick.AddListener(delegate
        {
            controlPanelPopup.InitNewControlPanelSwitch();
            //GlobalCanvas.Instance.SetHintPopup(0, 0, _ControlPanelSwitchHint, _ControlPanelSwitchHintRects[0]);
            //GlobalCanvas.Instance.ShowHint(true);
            ControlPanel.Instance.SetStorageRoomPopupParent();
            controlPanelPopup.SetTimeNum(30f);


            ControlPanel.Instance.SetSolenoidValveModeAndActivateBtn(mode =>
            {
                _curControlMode = mode;
                //if (_curControlMode.Equals(ControlMode.Manual))
                    //GlobalCanvas.Instance.SetHintPopup(1, 1, _ControlPanelSwitchHint, _ControlPanelSwitchHintRects[1]);
            }, null);
            ControlPanel.Instance.SetArea1ModeAndActivateBtn(mode =>
            {
                _curArea1Mode = mode;
                //if (_curArea1Mode.Equals(ControlMode.Manual))
                    //GlobalCanvas.Instance.SetHintPopup(2, 2, _ControlPanelSwitchHint, _ControlPanelSwitchHintRects[1]);
            }, () =>
            {
                if (_curControlMode == ControlMode.Manual
                    && _curArea1Mode == ControlMode.Manual)
                {
                    //GlobalCanvas.Instance.ShowHint(false);
                    ControlPanel.Instance.ShowFire(true);
                    ControlPanel.Instance.StartTimer(30f);
                    solenoidValvePopup.InitControlPanelSwitch();
                    solenoidValvePopup.gameObject.SetActive(true);
                }
            });
            ControlPanel.OnTimerEnd.RemoveAllListeners();
            ControlPanel.OnTimerEnd.AddListener(delegate
            {
                ManualActivateToSolenoidValve();
                ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
                ControlPanel.Instance.ShowFire(true);
                Observable.Timer(System.TimeSpan.FromSeconds(3))
                    .Subscribe(_ =>
                    {
                        ControlPanel.Instance.ShowPanel(false);
                        GasSysManager.Instance.ChangeState(GasSysState.Init);
                    })
                    .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
            });
        });

    }

#endregion

#region 방출표시등 작동 시험

    public void InitDischargeUpCheck(UnityAction upAction)
    {
        Init();
        activationBoxBtn.gameObject.SetActive(true);
        activationBoxBtn.onClick.AddListener(delegate
        {
            activationCylinderBoxPopup?.gameObject.SetActive(true);
            //GlobalCanvas.Instance.SetHintPopup(0, 0, _DischargeCheckHint, _DischargeCheckHintRects[0]);
            //GlobalCanvas.Instance.ShowHint(true);
        });

        activationCylinderBoxPopup?.InitDischargeUpCheck(delegate
        {
            //GlobalCanvas.Instance.SetHintPopup(1, 1, _DischargeCheckHint, _DischargeCheckHintRects[1]);
            controlBoxBtn.gameObject.SetActive(true);
            activationBoxBtn.gameObject.SetActive(false);
        }, upAction);

        controlBoxBtn.onClick.AddListener(delegate
        {
            ControlPanel.Instance.ShowPanel(true);
        });
        ControlPanel.Instance.InitNewDischargeCheck();
    }

    public void InitDischargeDownCheck(UnityAction downAction)
    {
        Init();
        activationBoxBtn.gameObject.SetActive(true);
        activationBoxBtn.onClick.AddListener(delegate
        {
            activationCylinderBoxPopup?.gameObject.SetActive(true);
            //GlobalCanvas.Instance.SetHintPopup(2, 2, _DischargeCheckHint, _DischargeCheckHintRects[0]);
            //GlobalCanvas.Instance.ShowHint(true);
        });

        activationCylinderBoxPopup?.InitDischargeDownCheck(delegate
        {
            //GlobalCanvas.Instance.ShowHint(false);
            controlBoxBtn.gameObject.SetActive(true);
            activationBoxBtn.gameObject.SetActive(false);
        }, downAction);

        controlBoxBtn.onClick.AddListener(delegate
        {
            ControlPanel.Instance.ShowPanel(true);
            Observable.Timer(System.TimeSpan.FromSeconds(3))
                .Subscribe(_ =>
                {
                    ControlPanel.Instance.ShowPanel(false);
                    GasSysManager.Instance.ChangeState(GasSysState.Init);
                })
                .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
        });
    }

#endregion //방출표시등 작동 시험

#region 복구

    public void InitRecoveryCheck()
    {
        Init();
        controlBoxBtn.gameObject.SetActive(true);

        SolenoidValvePopup.PinAttach.RemoveAllListeners();
        SolenoidValvePopup.PinAttach.AddListener(delegate
        {
            //GlobalCanvas.Instance.SetHintPopup(3,3, _RecoveryCheckHint, _RecoveryCheckHintRects[3]);
        });
        storageCylinder.ChangeValveState(false);
        selectionValve01.ChangeValveState(false);
        ControlPanel.Instance.RecoverySwitchEvent?.RemoveAllListeners();
        ControlPanel.Instance.RecoverySwitchEvent?.AddListener(delegate
        {
            //GlobalCanvas.Instance.SetHintPopup(1,1, _RecoveryCheckHint, _RecoveryCheckHintRects[1]);
        });
        controlBoxBtn.onClick.AddListener(delegate
        {
            //GlobalCanvas.Instance.SetHintPopup(0,0, _RecoveryCheckHint, _RecoveryCheckHintRects[0]);
            //GlobalCanvas.Instance.ShowHint(true);
            ControlPanel.Instance.InitNewRecoveryCheck(delegate
            {
                //GlobalCanvas.Instance.ShowHint(false);
                storageCylinderBtn?.gameObject.SetActive(true);
            });
            ControlPanel.Instance.SetStorageRoomPopupParent();
            ControlPanel.Instance.SetSolenoidValveModeAndActivateBtn(mode =>
            {
                _curControlMode = mode;
                if (mode.Equals(ControlMode.Manual))
                {
                    //GlobalCanvas.Instance.SetHintPopup(2,2, _RecoveryCheckHint, _RecoveryCheckHintRects[2]);
                    solenoidValvePopup?.InitRecoveryCheck1(delegate
                    {
                        //GlobalCanvas.Instance.SetHintPopup(4, 4, _RecoveryCheckHint, _RecoveryCheckHintRects[4]);
                        //activationBoxBtn.gameObject.SetActive(true);
                    });
                    solenoidValvePopup?.gameObject.SetActive(true);
                }

                if (mode.Equals(ControlMode.Auto))
                {
                    //GlobalCanvas.Instance.SetHintPopup(5, 5, _RecoveryCheckHint, _RecoveryCheckHintRects[5]);
                    activationCylinderBoxPopup.InitRecoveryCheck(delegate
                    {
                        //GlobalCanvas.Instance.SetHintPopup(7, 7, _RecoveryCheckHint, _RecoveryCheckHintRects[7]);
                    }, delegate
                    {
                        //GlobalCanvas.Instance.SetHintPopup(6, 6, _RecoveryCheckHint, _RecoveryCheckHintRects[6]);
                    });
                    activationCylinderBoxPopup.gameObject.SetActive(true);
                }
            }, null);
            controlBoxBtn.gameObject.SetActive(false);
        });

        storageCylinderBtn.onClick.AddListener(delegate
        {
            //GlobalCanvas.Instance.SetHintPopup(8, 8, _RecoveryCheckHint, _RecoveryCheckHintRects[8]);
            //GlobalCanvas.Instance.ShowHint(true);
            storageCylinderPopup.gameObject.SetActive(true);
        });

        storageCylinderPopup?.InitRecoveryCheck(false, storageCylinder.ChangeValveState, delegate
        {
            //GlobalCanvas.Instance.ShowHint(false);
            storageCylinderBtn.gameObject.SetActive(false);
            selectionValveBtn.gameObject.SetActive(true);
        });

        selectionValveBtn.onClick.AddListener(delegate
        {
            //GlobalCanvas.Instance.SetHintPopup(9, 9, _RecoveryCheckHint, _RecoveryCheckHintRects[9]);
            //GlobalCanvas.Instance.ShowHint(true);
            selectionValvePopup.gameObject.SetActive(true);
        });

        selectionValvePopup?.InitRecoveryCheck(false, selectionValve01.ChangeValveState, delegate
        {
            //GlobalCanvas.Instance.ShowHint(false);
            selectionValveBtn.gameObject.SetActive(false);
            Observable.Timer(System.TimeSpan.FromSeconds(3))
                .Subscribe(_ =>
                {
                    GasSysManager.Instance.ChangeState(GasSysState.Init);
                })
                .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
        });

    }

#endregion
    private void DragObjectPicked(GameObject obj)
    {
        //selectPinObj.SetActive(false);
        //selectAttachPinObj.SetActive(true);
    }

    private void HandleCollision(GameObject draggedObject, GameObject targetObject)
    {
        if (ControlMode.Auto.Equals(_curControlMode))
        {
            //solenoidValvePopup
        }

        // selectPinObj.SetActive(false);
        // draggedObject.SetActive(false);
        // aActivationObj.SetActive(true);
        // selectAttachPinObj.SetActive(false);
        // detachSolenoidBtn.gameObject.SetActive(true);
        // //targetObject.SetActive(false);
    }
    */
}
