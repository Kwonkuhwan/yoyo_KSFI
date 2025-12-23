using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public enum RTypeRBtnType
{
    예비전원시험,
    자동복구,
    축적소등_비축적점등,
    수신기복구,
    방화문정지,
    주음향정지,
    지구음향정지,
    사이렌정지,
    비상방송정지,
    부저정지,
    메인펌프오토,
    메인펌프수동,
    메인펌프정지,
    충압펌프오토,
    충압펌프수동,
    충압펌프정지,
    sp펌프오토,
    sp펌프수동,
    sp펌프정지,
    sp충압펌프오토,
    sp충압펌프수동,
    sp충압펌프정지,
}

public enum RTypeRPumpBtnType
{
    메인펌프오토,
    메인펌프수동,
    메인펌프정지,
    충압펌프오토,
    충압펌프수동,
    충압펌프정지,
    sp펌프오토,
    sp펌프수동,
    sp펌프정지,
    sp충압펌프오토,
    sp충압펌프수동,
    sp충압펌프정지,
}

[Serializable]
public class RTypeRPanelButtonCheck
{
    public Dictionary<RTypeRBtnType, bool> checkDic;// = new Dictionary<RTypeRBtnType, bool>();
    public ControlMode mainPump;
    public ControlMode jockeyPump;
    public ControlMode spMainPump;
    public ControlMode spJockeyPump;


    public RTypeRPanelButtonCheck(ControlMode _mainPump = ControlMode.Auto, ControlMode _jockeyPump = ControlMode.Auto,
        ControlMode _spMainPump = ControlMode.Auto, ControlMode _spJockeyPump = ControlMode.Auto)
    {
        //((Dictionary<RTypeRBtnType, bool>)null)?.Clear();
        mainPump = _mainPump;
        jockeyPump = _jockeyPump;
        spMainPump = _spMainPump;
        spJockeyPump = _spJockeyPump;
        checkDic = new Dictionary<RTypeRBtnType, bool>
        {
            {RTypeRBtnType.예비전원시험, false},
            {RTypeRBtnType.자동복구, false},
            {RTypeRBtnType.축적소등_비축적점등, false},
            {RTypeRBtnType.수신기복구, false},
            {RTypeRBtnType.방화문정지, false},
            {RTypeRBtnType.주음향정지, false},
            {RTypeRBtnType.지구음향정지, false},
            {RTypeRBtnType.사이렌정지, false},
            {RTypeRBtnType.비상방송정지, false},
            {RTypeRBtnType.부저정지, false},
            {RTypeRBtnType.메인펌프오토, ControlMode.Auto == mainPump},
            {RTypeRBtnType.메인펌프수동, ControlMode.Manual == mainPump},
            {RTypeRBtnType.메인펌프정지, ControlMode.Stop == mainPump},
            {RTypeRBtnType.충압펌프오토, ControlMode.Auto == jockeyPump},
            {RTypeRBtnType.충압펌프수동, ControlMode.Manual == jockeyPump},
            {RTypeRBtnType.충압펌프정지, ControlMode.Stop == jockeyPump},
            {RTypeRBtnType.sp펌프오토, ControlMode.Auto == spMainPump},
            {RTypeRBtnType.sp펌프수동, ControlMode.Manual == spMainPump},
            {RTypeRBtnType.sp펌프정지, ControlMode.Stop == spMainPump},
            {RTypeRBtnType.sp충압펌프오토, ControlMode.Auto == spJockeyPump},
            {RTypeRBtnType.sp충압펌프수동, ControlMode.Manual == spJockeyPump},
            {RTypeRBtnType.sp충압펌프정지, ControlMode.Stop == spJockeyPump}
        };
        /*
        checkDic.Add(RTypeRBtnType.예비전원시험, false);
        checkDic.Add(RTypeRBtnType.자동복구, false);
        checkDic.Add(RTypeRBtnType.축적소등_비축적점등, false);
        checkDic.Add(RTypeRBtnType.수신기복구, false);
        checkDic.Add(RTypeRBtnType.방화문정지, false);
        checkDic.Add(RTypeRBtnType.주음향정지, false);
        checkDic.Add(RTypeRBtnType.지구음향정지, false);
        checkDic.Add(RTypeRBtnType.사이렌정지, false);
        checkDic.Add(RTypeRBtnType.비상방송정지, false);
        checkDic.Add(RTypeRBtnType.부저정지, false);
        checkDic.Add(RTypeRBtnType.메인펌프오토, ControlMode.Auto == mainPump);
        checkDic.Add(RTypeRBtnType.메인펌프수동, ControlMode.Manual == mainPump);
        checkDic.Add(RTypeRBtnType.메인펌프정지, ControlMode.Stop == mainPump);
        checkDic.Add(RTypeRBtnType.충압펌프오토, ControlMode.Auto == jockeyPump);
        checkDic.Add(RTypeRBtnType.충압펌프수동, ControlMode.Manual == jockeyPump);
        checkDic.Add(RTypeRBtnType.충압펌프정지, ControlMode.Stop == jockeyPump);
        checkDic.Add(RTypeRBtnType.sp펌프오토, ControlMode.Auto == spMainPump);
        checkDic.Add(RTypeRBtnType.sp펌프수동, ControlMode.Manual == spMainPump);
        checkDic.Add(RTypeRBtnType.sp펌프정지, ControlMode.Stop == spMainPump);
        checkDic.Add(RTypeRBtnType.sp충압펌프오토, ControlMode.Auto == spJockeyPump);
        checkDic.Add(RTypeRBtnType.sp충압펌프수동, ControlMode.Manual == spJockeyPump);
        checkDic.Add(RTypeRBtnType.sp충압펌프정지, ControlMode.Stop == spJockeyPump);
        */

    }

    public void SetBtn(bool isOn)
    {
        checkDic = checkDic.ToDictionary(entry => entry.Key, entry => isOn);
    }

    public void SetBtns(bool isOn, params RTypeRBtnType[] switchBtnTypes)
    {
        if (switchBtnTypes == null || !switchBtnTypes.Any())
            return;

        checkDic = checkDic.ToDictionary(
            entry => entry.Key,
            entry => switchBtnTypes.Contains(entry.Key) ? isOn : entry.Value
        );
    }
    
    

}

[Serializable]
public enum RTypeRPanelButtonType
{
    화재경보,
    회로차단,
    설비동작,
}

[Serializable]
public enum RTypeRState
{
    None,
    PracticeMode, //실습 모드
    EvaluationMode, //평가 모드
}

[Serializable]
public enum RTypeRMainSection
{
    Init,
    EquipmentOperation,
    FireAlarmSystem,
    CircuitBreaker,
}

[Serializable]
public enum RTypeREquipmentOperationPSection
{
    수신기선택,
    화재발생,
    화재발생지점확인,
    작동설비확인,
    //화재여부확인및현장이동,
    현장1,
    수신기이동1,
    음향정지1,
    현장2,
    감지기먼지제거1,
    감지기먼지제거2,
    감지기먼지제거3,
    수신기이동, //2025-02-10 추가
    수신기복구, //2025-02-10 추가
    방화문복구1,
    방화문복구2,
    초기화면,
    초기화면이동,
    교육완료,
    //수신기이동2,
    // 수신기복구2,
    // 음향복구1,
    // 초기화면2,

}

[Serializable]
public enum RTypeREquipmentOperationESection
{
    Init,
    화재발생,
    화재여부확인및현장이동,
    현장1,
    수신기이동1,
    음향정지1,
    현장2,
    감지기먼지제거1,
    감지기먼지제거2,
    방화문복구1,
    방화문복구2,
    수신기이동2,
    수신기복구,
    //음향복구1,
    평가종료,
}

[Serializable]
public enum RTypeRFireAlarmSystemPSection
{
    음향정지,
    비축적,
    현장1,
    스프레이연기감지1,
    스프레이연기감지2,
    미작동,
    R수신기1,
    현장2,
    감지기탈거,
    전압측정연결1,
    전압측정연결2,
    전압측정전원온1,
    전압정상확인,
    감지기교체,
    감지기선로분리,
    감지기제거및교체,
    감지기선로연결,
    감지기결합,
    스프레이연기감지3,
    R수신기2,
    //R수신기3,
    주경종정지,
    비상방송음향확인,
    비상방송음향정지,
    화재경보클릭,
    수신기복구,
    음향복구,
    축적상태전환,
    운영기록,
    전체,
    조회,
    이력확인,
    초기화면,
    교육완료,
    //현장4,
    //교육완료

}


[Serializable]
public enum RTypeRFireAlarmSystemESection
{
    음향정지,
    비축적,
    현장1,
    스프레이연기감지1,
    스프레이연기감지2,
    미작동,
    R수신기1,
    현장2,
    감지기탈거,
    전압측정연결1,
    전압측정연결2,
    // 전압측정전원온1,
    // 전압정상확인,
    // 감지기교체,
    // 감지기선로분리,
    // 감지기제거및교체,
    // 감지기선로연결,
    // 감지기결합,
    // 스프레이연기감지3,
    //R수신기2,
    //R수신기3,
    주경종정지,
    // 비상방송음향확인,
    // 비상방송음향정지,
    // 화재경보클릭,
    수신기복구,
    //음향복구,
    //축적상태전환,
    운영기록,
    전체,
    // 조회,
    // 이력확인,
    초기화면,
    현장3,
    //현장4,
    평가종료
}

[Serializable]
public enum RTypeRCircuitBreakerPSection
{
    회로단선,
    부저음정지,
    회로차단,
    중계기제어선택,
    이상신호중계기선택,
    중계기통신재접속,
    이상신호확인,
    현장1,
    //소화전확인,
    소화전오픈,
    보관함테스터기,
    //중계기전원테스트1,
    중계기전원테스트2,
    중계기통신테스트1,
    //중계기통신테스트2,
    중계기통신테스트3,
    감지선로테스트1,
    //감지선로테스트2,
    감지선로테스트3,
    수신기이동,
    부저음복구,
    교육완료,
    
}


[Serializable]
public enum RTypeRCircuitBreakerESection
{
    Init,
    부저음정지,
    회로차단,
    중계기제어선택,
    이상신호중계기선택,
    중계기통신재접속,
    이상신호확인,
    현장1,
    //소화전확인,
    소화전오픈,
    보관함테스터기,
    //중계기전원테스트1,
    중계기전원테스트2,
    중계기통신테스트1,
    //중계기통신테스트2,
    중계기통신테스트3,
    감지선로테스트1,
    //감지선로테스트2,
    감지선로테스트3,
    수신기이동,
    부저음복구,
    평가완료,
}


[Serializable]
public enum RTypeRSmokeDetectorPopupType
{
    Default,
    OpenAni,
    CloseAni,
    Empty,
    LineOn,
    LineOnDust,
    LineOff,
    On,
}

[Serializable]
public enum RTypeRFirePlugType
{
    Default,
    Inner,
}

[Serializable]
public enum RTypeRTesterPenType
{
    None,
    Default,
    SmokePopup,
    중계기전원,
    중계기통신,
    감지기선로
}

[Serializable]
public enum RTypeRTesterPowerType
{
    Off,
    On,
}

[Serializable]
public enum RTypeRTesterNumType
{
    Off,
    Num2059,
    Num2390,
    Num2710,
    Num0,
}


[Serializable]
public enum RTypeRRoomType
{
    RTypeR,
    FireDoor,
    Area2_1,
    Area3_2,
}

[Serializable]
public class RTypeREquipmentOperationScore
{
    public bool 평가1;
    public bool 평가2;
    public bool 평가3;
    public bool 평가4;

    public void ResetData()
    {
        평가1 = false;
        평가2 = false;
        평가3 = false;
        평가4 = false;
    }
}

[Serializable]
public class RTypeRFireAlarmSystemScore
{
    [FormerlySerializedAs("평가2")]
    public bool 평가1;
    [FormerlySerializedAs("평가3")]
    public bool 평가2;

    public void ResetData()
    {
        평가1 = false;
        평가2 = false;
    }

}


[Serializable]
public class RTypeRCircuitBreakerScore
{
    public bool 평가1;
    public bool 평가2;
    public bool 평가3;

    public void ResetData()
    {
        평가1 = false;
        평가2 = false;
        평가3 = false;
    }

}

[Serializable]
public class RTypeRTotalScore
{
    public RTypeREquipmentOperationScore 설비동작 = new RTypeREquipmentOperationScore();
    public List<ResultObject> 설비동작List = new List<ResultObject>();
    public RTypeRFireAlarmSystemScore 화재경보 = new RTypeRFireAlarmSystemScore();
    public List<ResultObject> 화재경보List = new List<ResultObject>();
    public RTypeRCircuitBreakerScore 회로차단 = new RTypeRCircuitBreakerScore();
    public List<ResultObject> 회로차단List = new List<ResultObject>();
    public void ResetData()
    {
        설비동작.ResetData();
        설비동작List.Clear();
        화재경보.ResetData();
        화재경보List.Clear();
        회로차단.ResetData();
        회로차단List.Clear();
    }
}