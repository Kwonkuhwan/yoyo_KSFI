using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActivationCylinderBox : MonoBehaviour
{
    [SerializeField] private Button openBox1Btn;
    [SerializeField] private Button openBox2Btn;

    [SerializeField] private GameObject insidePopupObj;
    [SerializeField] private InsideBox insideBox1Obj;
    [SerializeField] private InsideBox insideBox2Obj;

    [SerializeField] private GameObject popupObj;
    [SerializeField] private GameObject solenoidValvePopupObj;
    [SerializeField] private SolenoidValve solenoidValveObj;

    private void Init()
    {
        insidePopupObj.SetActive(false);
        popupObj.SetActive(false);
        solenoidValvePopupObj.SetActive(false);
    }
#region 점검 전 안전조치

    public void InitSafetyCheck(UnityAction action)
    {
        Init();
        popupObj.SetActive(true);
        insideBox1Obj.gameObject.SetActive(false);
        insideBox2Obj.gameObject.SetActive(false);
        openBox1Btn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                popupObj.SetActive(false);
                insidePopupObj.SetActive(true);
                insideBox1Obj.InitSafetyCheck();
                insideBox1Obj.SetDetachBtn(() =>
                {
                    action?.Invoke();
                    //ShowSolenoidValvePopup(action);
                });
            }).AddTo(this);
    }

    public void ShowSafetyCheckSolenoidValvePopup(UnityAction action)
    {
        solenoidValvePopupObj.SetActive(true);
        solenoidValveObj.Init(SolenoidValve.InitState.SafetyCheck);
        solenoidValveObj.SetReleaseActivationCylinderFromSolenoidValveState();
        solenoidValveObj.SetDetachSafetyPinBtn(() =>
        {
            action?.Invoke();
            //종료
        });
    }

#endregion //점검 전 안전조치

#region 기동용기 솔레노이드 밸브 격발시험-> 수동조작버튼작동[즉시격발]

    public void InitManualOperationController(UnityAction action)
    {
        Init();
        gameObject.SetActive(true);
        solenoidValvePopupObj.SetActive(true);
        solenoidValveObj.Init(SolenoidValve.InitState.ManualOperationController);
        solenoidValveObj.SetActivationBtn(() =>
        {
            action?.Invoke();
            solenoidValveObj.SetActivationImg(true);
        });
    }

#endregion //기동용기 솔레노이드 밸브 격발시험-> 수동조작버튼작동[즉시격발]

#region 기동용기 솔레노이드밸브 격발시험 -> 수동조작함 작동

    public void InitManualControlBox()
    {
        Init();
        gameObject.SetActive(true);
        solenoidValvePopupObj.SetActive(true);
        solenoidValveObj.Init(SolenoidValve.InitState.ManualControlBoxController);
        // solenoidValveObj.SetActivationBtn(() =>
        // {
        //     action?.Invoke();
        //     solenoidValveObj.SetActivationImg(true);
        // });
    }

#endregion //기동용기 솔레노이드밸브 격발시험 -> 수동조작함 작동

#region 기동용기 솔레노이드밸브 격발시험 -> 교차회로 감지기 동작

    public void InitCrossCircuitDetector()
    {
        Init();
        gameObject.SetActive(true);
        solenoidValvePopupObj.SetActive(true);
        solenoidValveObj.Init(SolenoidValve.InitState.CrossCircuitDetector);
        // solenoidValveObj.SetActivationBtn(() =>
        // {
        //     action?.Invoke();
        //     solenoidValveObj.SetActivationImg(true);
        // });
    }

#endregion //기동용기 솔레노이드밸브 격발시험 -> 교차회로 감지기 동작

#region 기동용기 솔레노이드밸브 격발시험 -> 제어반 수동조작스위치 동작

    public void InitControlPanelSwitch()
    {
        Init();
        gameObject.SetActive(true);
        insideBox1Obj.gameObject.SetActive(true);
        insideBox2Obj.gameObject.SetActive(false);
        solenoidValvePopupObj.SetActive(true);
        solenoidValveObj.Init(SolenoidValve.InitState.ControlPanelSwitchController);
        // solenoidValveObj.SetActivationBtn(() =>
        // {
        //     action?.Invoke();
        //     solenoidValveObj.SetActivationImg(true);
        // });
    }

#endregion //기동용기 솔레노이드밸브 격발시험 -> 제어반 수동조작스위치 동작

#region 방출표시등 작동시험

    public void InitDischargeIndicatorLightTest(UnityAction upAction, UnityAction downAction)
    {
        Init();
        gameObject.SetActive(true);
        insidePopupObj.SetActive(true);
        insideBox1Obj.gameObject.SetActive(true);
        insideBox2Obj.gameObject.SetActive(false);
        insideBox1Obj.InitDischargeIndicatorLightTest();
        insideBox1Obj.SetPressureUpBtn(upAction);
        insideBox1Obj.SetPressureDownBtn(downAction);
    }

#endregion //방출표시등 작동시험

#region 점검 후 복구

    public void InitRecoveryCheck()
    {
        Init();
        gameObject.SetActive(true);
        insidePopupObj.SetActive(true);
        insideBox1Obj.InitRecoveryCheck();
        insideBox1Obj.SetAttachBtn(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void InitRecoveryCheck2()
    {
        Init();
        gameObject.SetActive(true);
        insidePopupObj.SetActive(true);
        insideBox1Obj.InitRecoveryCheck2();
        insideBox1Obj.SetSafetyPinBtn(() =>
        {
            gameObject.SetActive(false);
        });
    }

#endregion //점검 후 복구
    public void SetSolenoidValveActivationImg(bool isOn)
    {
        solenoidValveObj.SetActivationImg(isOn);
    }
}
