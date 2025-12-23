using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
[Serializable]
public class FireSoundCheck
{
    public bool buzzer;
    public bool siren;
    public bool broadCast;
    public bool alarm;
    public bool alarm2;
}

[Serializable]
public class HintTextAndAudio
{
    public string title;
    [Multiline]
    public string text;
    public AudioClip audioClip;
}

[Serializable]
public class HintTextAndAudio1
{
    [Multiline]
    public string[] hintText;
    public AudioClip[] hintAudio;
}

[Serializable]
public enum GasSysState
{
    None,
    PracticeMode, //실습 모드
    EvaluationMode, //평가 모드
}

[Serializable]
public enum GasSysMainSection
{
    Init,
    SafetyCheck,
    SolenoidValveTest,
    DischargeLightTest,
    RecoveryCheck,
}

[Serializable]
public enum GasSysSolenoidValveTestSection
{
    ManualOperation,
    ManualControlBox,
    CrossCircuitDetector,
    ControlPanelSwitch,
}

[Serializable]
public enum GasSysSafetyCheckPSection
{
    선택밸브조작동관선택,
    선택밸브조작동관분리전,
    선택밸브조작동관분리,
    저장용기조작동관선택,
    저장용기조작동관분리전,
    저장용기조작동관분리,
    감시제어반선택,
    음향활성,
    /*
    주경종,
    지구경종,
    사이렌,
    비상방송,
    부저,
    */
    축적비축적,
    솔레노이드밸브메인정지,
    기동용기함선택,
    솔레노이드밸브1,
    솔레노이드밸브2,
    솔레노이드밸브3,
    교육종료
}

[Serializable]
public enum GasSysSafetyCheckESection
{
    Init,
    E1,
    E2,
    E3,
    E4,
    평가종료,
    //평가종료
}

[Serializable]
public class GasSysControlPanelState
{
    public bool 기동밸브;
    public bool 저장밸브;
    public bool 주경종;
    public bool 지구경종;
    public bool 사이렌;
    public bool 비상방송;
    public bool 부저;
    public bool 축적;
    public bool 복구;
    [FormerlySerializedAs("솔레노이드연동정지")]
    public bool 솔레노이드연동;
    public bool 격발준비;
    public bool 격발;
    public bool 연기감지;
    public bool 열감지;
    public bool 구역1연동;
    public bool 구역2연동;
    public bool 구역1기동;
    public bool 스위치업;
    public bool 스위치다운;
    public bool 안전핀제거;
    public bool 솔레노이드결착;
    public bool 솔레노이드복구;
    
    public GasSysControlPanelState()
    {
        Reset();
    }
    public void Reset()
    {
        기동밸브 = false;
        저장밸브 = false;
        주경종 = false;
        지구경종 = false;
        사이렌 = false;
        비상방송 = false;
        부저 = false;
        축적 = false;
        복구 = false;
        솔레노이드연동 = false;
        격발준비 = false;
        격발 = false;
        연기감지 = false;
        열감지 = false;
        구역1연동 = false;
        구역2연동 = false;
        구역1기동 = false;
        스위치업 = false;
        스위치다운 = false;
        안전핀제거 = false;
        솔레노이드결착 = false;
        솔레노이드복구 = false;
    }
}

[Serializable]
public enum GasSysManualOperationPSection
{
    감시제어반선택,
    제어반음향및솔레노이드밸브연동,
    //축적비축적유지,
    보관함에서솔레노이드,
    안전클립제거및격발,
    //솔정상확인,
    음향정지작동확인,
    제어반화재표시,
    방호구역1,
    //방호구역1음향연동,
    음향정지,
    방호구역2,
    솔레노이드작동여부,
    //수동조작버튼,
    //격발테스트,
    교육종료
}

[Serializable]
public enum GasSysManualOperationESection
{
    Init,
    E1,
    E2,
    E3,
    E4,
    평가종료
}

[Serializable]
public enum GasSysManualControlBoxPSection
{
    감시제어반선택,
    제어반음향및솔레노이드밸브연동,
    //축적비축적유지,
    방호구역1번복도,
    수동조작함선택,
    수동조작함개방및기동스위치,
    수동조작함개방및기동스위치1,
    수동조작함개방및기동스위치2,
    솔정상확인,
    음향정지작동확인,
    제어반화재표시,
    //방호구역1,
    방호구역1음향연동,
    음향정지,
    방호구역2,
    //지연장치,
    //솔레노이드작동여부,
    교육종료
}


[Serializable]
public enum GasSysManualControlBoxESection
{
    Init,
    E1,
    E2,
    E3,
    평가종료
}

[Serializable]
public enum GasSysCrossCircuitDetectorPSection
{
    감시제어반선택,
    제어반음향및솔레노이드밸브연동,
    방호구역1번,
    연기감지기,
    연기감지기2,
    열감지기,
    열감지기2,
    솔정상확인,
    음향정지작동확인,
    제어반화재표시,
    //방호구역1,
    방호구역1음향연동,
    음향정지,
    방호구역2,
    //지연장치,
    //솔레노이드작동여부,
    교육종료
}

[Serializable]
public enum GasSysCrossCircuitDetectorESection
{
    Init,
    E1,
    E2,
    E3,
    E4,
    평가종료
}

[Serializable]
public enum GasSysControlPanelSwitchPSection
{
    감시제어반선택,
    제어반음향,
    솔레노이드밸브수동,
    방호구역1번수동,
    방호구역1번기동,
    방호구역1번기동2,
    솔정상확인,
    음향정지작동확인,
    제어반화재표시,
    //방호구역1,
    방호구역1음향연동,
    음향정지,
    방호구역2,
    //지연장치,
    //솔레노이드작동여부,
    교육종료
}

[Serializable]
public enum GasSysControlPanelSwitchESection
{
    Init,
    E1,
    E2,
    //E3,
    평가종료
}

[Serializable]
public enum GasSysDischargeLightTestPSection
{
    방호구역1출입문소등,
    수동조작함소등,
    약제저장실,
    기동용기함선택,
    압력스위치업전,
    압력스위치업,
    방호구역1출입문,
    방호구역1출입문점등,
    수동조작함선택,
    수동조작함방출등확인,
    저장용기실,
    기동용기함선택2,
    압력스위치다운전,
    압력스위치다운,
    교육종료
}

[Serializable]
public enum GasSysDischargeLightTestESection
{
    Init,
    E1,
    E2,
    E3,
    평가종료
}


[Serializable]
public enum GasSysRecoveryCheckPSection
{
    감시제어반선택,
    복구전,
    트리거복구,
    안전핀결착,
    솔레노이드자동,
    감시제어반복구,
    솔레노이드밸브복구,
    안전핀복구,
    저장용기조작동관선택,
    저장용기조작동관연결전,
    저장용기조작동관연결,
    선택밸브조작동관선택,
    선택밸브조작동관연결전,
    선택밸브조작동관연결,
    교육종료
}

[Serializable]
public enum GasSysRecoveryCheckESection
{
    Init,
    E1,
    E2,
    E3,
    E4,
    E5,
    E6,
    //E7,
    //E8,
    //E9,
    //E10,
    평가종료
}

[Serializable]
public class ControlPanelButtonCheck
{
    public bool 주경종;
    public bool 지구경종;
    public bool 사이렌;
    public bool 비상방송;
    public bool 부저;
    public bool 축적;
    public bool 예비전원;
    public bool 유도등;
    public bool 도통시험;
    public bool 자동복구;
    public bool 복구;
    public ControlMode 솔레노이드밸브;
    public ControlMode 구역1;
    public ControlMode 구역2;
    public bool 솔레노이드메인기동;
    public bool 구역1기동;
    public bool 구역2기동;

    public ControlPanelButtonCheck(ControlMode sol = ControlMode.Auto, ControlMode area1 = ControlMode.Auto, ControlMode area2 = ControlMode.Auto)
    {
        주경종 = false;
        지구경종 = false;
        사이렌 = false;
        비상방송 = false;
        부저 = false;
        축적 = false;
        예비전원 = false;
        유도등 = false;
        도통시험 = false;
        자동복구 = false;
        복구 = false;
        솔레노이드밸브 = sol;
        구역1 = area1;
        구역2 = area2;
        솔레노이드메인기동 = false;
        구역1기동 = false;
        구역2기동 = false;
    }

    public void SetBtn(bool isOn)
    {
        주경종 = isOn;
        지구경종 = isOn;
        사이렌 = isOn;
        비상방송 = isOn;
        부저 = isOn;
        축적 = isOn;
        예비전원 = isOn;
        유도등 = isOn;
        도통시험 = isOn;
        자동복구 = isOn;
        복구 = isOn;
    }
}

[Serializable]
public class SwitchButtonCheck
{
    public Button btn;
    public bool select;
}

[Serializable]
public enum ControlMode { Auto, Stop, Manual }

[Serializable]
public enum ResultType
{
    성공,
    실패,
    보류,
}

[Serializable]
public class ResultObject
{
    public bool IsSuccess;
    public ResultType resultType = ResultType.실패;
    public string Title;
}

[Serializable]
public class SoundCheck
{
    public bool buzzer;
    public bool siren;
    public bool broadcast;
    public bool alarm;
    public bool alarm2;
}

[Serializable]
public class GasSysSafetyScore
{
    public bool 조작동관분리 = false;
    public bool 감시제어반조치 = false;
    public bool 솔분리및격발준비 = false;
    public bool 안전조치상태확인 = false;
    public void ResetData()
    {
        조작동관분리 = false;
        감시제어반조치 = false;
        솔분리및격발준비 = false;
        안전조치상태확인 = false;
    }
}

[Serializable]
public class GasSysSol1
{
    public bool 감시제어반조치 = false;
    public bool 격발 = false;
    public bool 동작확인 = false;
    public void ResetData()
    {
        감시제어반조치 = false;
        격발 = false;
        동작확인 = false;
    }
}

[Serializable]
public class GasSysSol2
{
    public bool 감시제어반조치 = false;
    public bool 격발 = false;
    public bool 동작확인 = false;
    public void ResetData()
    {
        감시제어반조치 = false;
        격발 = false;
        동작확인 = false;
    }
}

[Serializable]
public class GasSysSol3
{
    public bool 감시제어반조치 = false;
    public bool 화재표시동작확인 = false;
    public bool 연기감지기 = false;
    public bool 열감지기 = false;
    public bool 동작확인 = false;
    
    public void ResetData()
    {
        감시제어반조치 = false;
        화재표시동작확인 = false;
        연기감지기 = false;
        열감지기 = false;
        동작확인 = false;
    }
    
}

[Serializable]
public class GasSysSol4
{
    public bool 감시제어반조치 = false;
    public bool 격발시험 = false;
    public bool 솔수동 = false;
    public bool 수동기동 = false;
    public bool 동작확인 = false;
    public void ResetData()
    {
        감시제어반조치 = false;
        격발시험 = false;
        솔수동 = false;
        수동기동 = false;
        동작확인 = false;
    }
}

[Serializable]
public class GasSysLightTest
{
    public bool 압력스위치작동 = false;
    public bool 동작확인 = false;
    public bool 압력스위치복구 = false;

    public void ResetData()
    {
        압력스위치작동 = false;
        동작확인 = false;
        압력스위치복구 = false;
    }
}

[Serializable]
public class GasSysRecovery
{
    public bool 화재복구 = false;
    public bool 솔복구 = false;
    public bool 솔결합 = false;
    public bool 제어반복구 = false;
    public bool 솔안전핀분리 = false;
    public bool 조작동관복구 = false;

    public void ResetData()
    {
        화재복구 = false;
        솔복구 = false;
        솔결합 = false;
        제어반복구 = false;
        솔안전핀분리 = false;
        조작동관복구 = false;
    }
}



[Serializable]
public class GasSysTotalScore
{
    public GasSysSafetyScore 점검전안전조치 = new GasSysSafetyScore();
    public List<ResultObject> 점검전안전조치List = new List<ResultObject>(); 
    public GasSysSol1 즉시격발 = new GasSysSol1();
    public List<ResultObject> 즉시격발List = new List<ResultObject>(); 
    public GasSysSol2 수동조작함작동 = new GasSysSol2();
    public List<ResultObject> 수동조작함작동List = new List<ResultObject>(); 
    public GasSysSol3 교차회로 = new GasSysSol3();
    public List<ResultObject> 교차회로List= new List<ResultObject>(); 
    public GasSysSol4 스위치동작 = new GasSysSol4();
    public List<ResultObject> 스위치동작List = new List<ResultObject>(); 
    public GasSysLightTest 방출표시등 = new GasSysLightTest();
    public List<ResultObject> 방출표시등List = new List<ResultObject>(); 
    public GasSysRecovery 점검완료후복구 = new GasSysRecovery();
    public List<ResultObject> 점검완료후복구List = new List<ResultObject>();

    public void ResetData()
    {
        점검전안전조치.ResetData();
        점검전안전조치List.Clear();
        즉시격발.ResetData();
        즉시격발List.Clear();
        수동조작함작동.ResetData();
        수동조작함작동List.Clear();
        교차회로.ResetData();
        교차회로List.Clear();
        스위치동작.ResetData();
        스위치동작List.Clear();
        방출표시등.ResetData();
        방출표시등List.Clear();
        점검완료후복구.ResetData();
        점검완료후복구List.Clear();
    }
}

[Serializable]
public enum GasSysIState
{
    None,
    주요구성요소,
    감시기작동오토,
    수동조작함작동수동,
    점검,
    즉시격발,
    수동조작함작동,
    교차회로,
    스위치동작,
    방출표시등,
    점검완료후복구
}

// [Serializable]
// public enum GasSysIOperationFlowState
// {
//     None,
//     Auto,
//     Manual,
// }
//

[Serializable]
public enum GasSysIOperationAutoSection
{
    전체,
    화재발생,
    감지기작동,
    감시제어반출력,
    //기동용기함개방,
    //기동용기함개방완료,
    솔밸브동작,
    제어반표시등점등,
    // 기동용기에가스동관이동,
    // 가스선택밸브로이동,
    선택밸브개방,
    선택밸브에서약제저장실이동,
    저장용기개방,
    집합관으로이동,
    //집합관선택밸브,
    압력스위치온,
    방출표시등점등,
    수동조작함가스방출표시등,
    제어반,
    가스방출화재진압,
    실습완료,
}

[Serializable]
public enum GasSysIOperationManualSection
{
    전체,
    화재발생,
    화재발생확인,
    //수동조작함개방,
    수동조작함방출,
    감시제어반출력,
    //기동용기함개방,
    //기동용기함개방완료,
    솔밸브동작,
    제어반표시등점등,
    // 기동용기에가스동관이동,
    // 가스선택밸브로이동,
    선택밸브개방,
    선택밸브에서약제저장실이동,
    저장용기개방,
    집합관으로이동,
    //집합관선택밸브,
    압력스위치온,
    방출표시등점등,
    수동조작함가스방출표시등,
    제어반,
    가스방출화재진압,
    실습완료,
}

[Serializable]
public enum GasSysICheckSection
{
    전체,
    선택밸브동관분리,
    저장용기동관분리,
    음향정지전,
    솔정지,
    //기동용기함개방,
    안전핀및격발준비,
    제어반솔자동,
    제어반정상,
    //수동조작함개방,
    수동조작함방출_부저,
    방호구역1이동_사이렌_경종_방송_부저,
    제어반_화재_수동조작함기동,
    수동조작함지연,
    감시제어반타이머정지,
    수동조작함지연정지,
    감시제어반타이머완료및솔작동확인,
    수동조작함방출복구,
    감시제어반수동조작함기동비활성,
    감시제어반복구,
    솔복구,
    감시제어반정상복구확인,
    축적,
    연기감지기,
    감시제어반_화재_감지기A,
    열감지기,
    감시제어반_화재_감지기AB,
    감시제어반방출지연,
    감시제어반방출,
    //솔밸브기동,
    복구2,
    비축적,
    솔복구2,
    감시제어반정상확인,
    방출표시등작동시험,
    방출표시등확인,
    감시제어반방출표시등확인,
    압력스위치작동,
    방출표시등확인2,
    감시제어반방출표시등확인2,
    압력스위치복구,
    방출표시등확인3,
    감시제어반방출표시등확인3,
    제어반정상복구,
    점검후복구,
    //연동정지,
    솔장착,
    연동자동,
    안전핀분리,
    저장용기동관결합,
    선택밸브동관결합,
    실습완료,
}

[Serializable]
public enum GasSysIPartsSection
{
    전체,
    저장용기,
    기동용가스용기,
    솔레노이드밸브,
    선택밸브,
    압력스위치,
    방출표시등,
    수동조작함,
    감시제어반,
    방출헤드,
}