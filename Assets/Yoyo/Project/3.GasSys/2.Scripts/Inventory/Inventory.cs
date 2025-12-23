using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour
{
    [SerializeField] public GameObject smokeDetect;
    [SerializeField] public GameObject heatDetect;
    [SerializeField] public GameObject safetyPin;
    [SerializeField] public GameObject safetyClip;
    [FormerlySerializedAs("solenoidValve")]
    [SerializeField] public GameObject solenoidValve1;
    [FormerlySerializedAs("solenoidValve")]
    [SerializeField] public GameObject solenoidValve2;
    [FormerlySerializedAs("recoverySolenoidValve")]
    [SerializeField] public GameObject recoverySolenoidValve1;
    [SerializeField] public GameObject recoverySolenoidValve2;

    [SerializeField] public GameObject tester;

    [SerializeField] public GameObject smokeDetectBase;
    [SerializeField] public GameObject smokeDetectHead;

    [SerializeField] private GameObject panelObj;
    //[SerializeField] private GameObject selectImg;

    public void Init()
    {
        smokeDetect.SetActive(false);
        heatDetect.SetActive(false);
        safetyPin.SetActive(false);
        safetyClip.SetActive(false);
        solenoidValve1.SetActive(false);
        solenoidValve2.SetActive(false);
        recoverySolenoidValve1.SetActive(false);
        recoverySolenoidValve2.SetActive(false);
        if(tester)
            tester.SetActive(false);
        if(smokeDetectBase)
            smokeDetectBase.SetActive(false);
        if(smokeDetectHead)
            smokeDetectHead.SetActive(false);
    }

#region 점검 전 안전 조치

    public void InitSafetyCheck()
    {
        Init();
    }

#endregion //점검 전 안전 조치

#region 수동조작버튼작동(즉시격발)

    public void InitManualOperationController()
    {
        Init();
        ShowSolenoidValve1(true);
    }

#endregion //수동조작버튼작동(즉시격발)

#region 수동조작함 작동

    public void InitManualControlBox()
    {
        Init();
    }

# endregion

#region 교차회로감지기 작동

    public void InitCrossCircuitDetector()
    {
        Init();
        ShowSmokeDetect(true);
        ShowHeatDetect(true);
    }

#endregion //교차회로감지기 작동

#region 제어반 수동조작스위치 동작

    public void ControlPanelSwitch()
    {
        Init();
    }

#endregion

#region 방출표시등 작동시험

    public void InitDischargeCheck()
    {
        Init();
    }

#endregion //방출표시등 작동시험

#region 복구

    public void InitRecoveryCheck()
    {
        Init();
        ShowSafetyPin(true);
    }

#endregion
    public GameObject ShowSmokeDetect(bool show)
    {
        smokeDetect.SetActive(show);
        return smokeDetect;
    }

    public GameObject ShowHeatDetect(bool show)
    {
        heatDetect.SetActive(show);
        return heatDetect;
    }

    public GameObject ShowSafetyPin(bool show)
    {
        safetyPin.SetActive(show);
        return safetyPin;
    }

    public GameObject ShowSafetyClip(bool show)
    {
        safetyClip.SetActive(show);
        return safetyClip;
    }

    public GameObject ShowSolenoidValve1(bool show)
    {
        solenoidValve1.SetActive(show);
        return solenoidValve1;
    }
    
    public GameObject ShowSolenoidValve2(bool show)
    {
        solenoidValve2.SetActive(show);
        return solenoidValve2;
    }

    public GameObject ShowRecoverySolenoidValve1(bool show)
    {
        recoverySolenoidValve1.SetActive(show);
        return recoverySolenoidValve1;
    }
    public GameObject ShowRecoverySolenoidValve2(bool show)
    {
        recoverySolenoidValve2.SetActive(show);
        return recoverySolenoidValve2;
    }

    public GameObject ShowTester(bool show)
    {
        if (null == tester)
            return null;
        tester.SetActive(show);
        return tester;
    }
    
    public GameObject ShowSmokeBase(bool show)
    {
        if (!smokeDetectBase)
            return null;
        smokeDetectBase.SetActive(show);
        return smokeDetectBase;
    }

    public GameObject ShowSmokeHead(bool show)
    {
        if(!smokeDetectHead)
            return null;
        smokeDetectHead.SetActive(show);
        return smokeDetectHead;
    }

    public void ShowPanel(bool show)
    {
        panelObj.SetActive(show);
    }

}
