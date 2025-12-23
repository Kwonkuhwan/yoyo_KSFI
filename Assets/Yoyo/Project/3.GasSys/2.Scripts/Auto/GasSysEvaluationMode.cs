using System.Collections;
using System.Collections.Generic;
using GASSYS;
using UnityEngine;

namespace MyNamespace
{
    
}
public class GasSysEvaluationMode : MonoBehaviour
{
    public enum GasSysEvaluationModeState
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
    
    [SerializeField] public GasSysSolenoidValveTestController solenoidValveTestControllerObj;
    [SerializeField] private GasSysSection sectionObj;
    [SerializeField] private List<MenuButtonObj> menuBtns;
    private ControlMode _curControlMode;
    private GasSysEvaluationModeState _curState = GasSysEvaluationModeState.Init; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
