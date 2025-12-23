using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;
using RBtnType = RTypeRBtnType;
/// <summary>
/// 화재 경보
/// </summary>
public partial class RTypeRSection
{
    
    [Foldout("화재경보")]
    private RTypeRFireAlarmSystemPSection _curRTypeRFireAlarmSystemPSection;
    private RTypeRFireAlarmSystemESection _curRTypeRFireAlarmSystemESection;
    [FormerlySerializedAs("rTypeRCircuitBreakerHintPObj")]
    public HintScriptableObj rTypeRFireAlarmSystemHintPObj;
    [FormerlySerializedAs("rTypeRCircuitBreakerHintEObj")]
    public HintScriptableObj rTypeRFireAlarmSystemHintEObj;
    public Button F2CorrectBtn;
    public Button F2WrongBtn;
    public GameObject dragHandAniObj;
    public GameObject testerCheckImg;
    public GameObject smokeCheckImg;
    [EndFoldout]

    
    public void InitFireAlarmSystem()
    {
        
         Init();
         curMainSection = RTypeRMainSection.FireAlarmSystem;
         InitEMode();
        _maxSection = rTypeRState switch
        {
            RTypeRState.PracticeMode => Enum.GetValues(typeof(RTypeRFireAlarmSystemPSection)).Length,
            RTypeRState.EvaluationMode => Enum.GetValues(typeof(RTypeRFireAlarmSystemESection)).Length,
            _ => _maxSection
        };
        _soundManager.StopAllFireSound();
        SetSectionRange(0, _maxSection, _maxSection);
        ShowRoom(RTypeRRoomType.RTypeR);
        _rPanel.ShowScreen(_rPanel.mainScreenImg);
    }
    private void SetFireAlarmSystemSection(int index)
    {
        if (rTypeRState.Equals(RTypeRState.PracticeMode))
        {
            prevBtn.gameObject.SetActive(true);
            SetHint(GetHintTextAndAudio(rTypeRFireAlarmSystemHintPObj, (int)RTypeRFireAlarmSystemPSection.음향정지 + index));
            ChangeStateFireAlarmSystemP(RTypeRFireAlarmSystemPSection.음향정지 + index);
        }
        if (!rTypeRState.Equals(RTypeRState.EvaluationMode))
            return;
        prevBtn.gameObject.SetActive(false);
        SetHint(GetHintTextAndAudio(rTypeRFireAlarmSystemHintEObj, (int)RTypeRFireAlarmSystemESection.음향정지 + index));
        ChangeStateFireAlarmSystemE(RTypeRFireAlarmSystemESection.음향정지 + index);
    }

    public void ChangeStateFireAlarmSystemE(RTypeRFireAlarmSystemESection state)
    {

        _curRTypeRFireAlarmSystemESection = state;
        OnStateChangedFireAlarmSystemE(_curRTypeRFireAlarmSystemESection);
    }

    public void FAButtonEventResetE()
    {
        RTypeRPanel.RTypeRPanelButtonAction.RemoveAllListeners();
        areaManagerObj.GetArea2_1Btn().onClick.RemoveAllListeners();
        uIDragAndCollisionHandler.ResetEvent();
        uIDragAndCollisionHandler.StopDragging();
        areaManagerObj.GetRTypeRBtn().onClick.RemoveAllListeners();
        GetArea2_1SmokeDetector().onClick.RemoveAllListeners();
        area2_1SmokeDetectorPopup2.GetSmokeDetectorBtn().onClick.RemoveAllListeners();
        smokeTestPopup.GetPowerBtn().onClick.RemoveAllListeners();
        F2CorrectBtn.onClick.RemoveAllListeners();
        F2WrongBtn.onClick.RemoveAllListeners();
        F2CorrectBtn.gameObject.SetActive(false);
        F2WrongBtn.gameObject.SetActive(false);
        areaManagerObj.GetRTypeRBtn().onClick.RemoveAllListeners();
        _rPanel.GetFireAlarmBtn().onClick.RemoveAllListeners();
        _rPanel.GetOperationLogBtn().onClick.RemoveAllListeners();
        _rPanel.GetOperationLogAllBtn().onClick.RemoveAllListeners();
        _rPanel.GetSearchBtn().onClick.RemoveAllListeners();
        RTypeRPanel.OnLogResultCheck.RemoveAllListeners();
        _rPanel.GetUpBtn().onClick.RemoveAllListeners();
        _rPanel.GetDownBtn().onClick.RemoveAllListeners();
        _rPanel.GetInitScreenBtn().onClick.RemoveAllListeners();
    }
    private void OnStateChangedFireAlarmSystemE(RTypeRFireAlarmSystemESection state)
    {
        _btnManager.RemoveAllHighlights();
        FAButtonEventResetE();
        switch(state)
        {
            case RTypeRFireAlarmSystemESection.음향정지:
                {
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                    _rPanel.ClearLog();
                    smokeTestPopup.gameObject.SetActive(false);
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetFireAlarmEventNum(0);
                    _rPanel.SetEquipmentOperationEventNum(0);
                    _rPanel.SetSwitchButton(false);
                    // _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                    //     RBtnType.지구음향정지, RBtnType.비상방송정지);
                    _soundManager.PlayAlarm(false);
                    _soundManager.PlayAlarm2(false);
                    _soundManager.PlayBroadcast(false);
                    _soundManager.SetBuzzerVolume(0);
                    _soundManager.SetSirenVolume(0);
                    SetArea2_1SmokeDetector(false);
                    _rPanel.SoundCheck();
                    _panelBtnCheck.SetBtn(false);
                    // RTypeRPanel.RTypeRPanelButtonAction.AddListener((btn) =>
                    // {
                    //     _gCanvas.totalScore.화재경보.평가1 = btn.checkDic[RBtnType.주음향정지] && btn.checkDic[RBtnType.지구음향정지] && btn.checkDic[RBtnType.비상방송정지];
                    // });
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    NextEnable(false);
                    _btnManager.SetEvaluationButtons();

                }
                break;
            case RTypeRFireAlarmSystemESection.비축적:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지, RBtnType.비상방송정지);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                    NextEnable(false);
                }
                
                break;
            case RTypeRFireAlarmSystemESection.현장1:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지, RBtnType.비상방송정지, RBtnType.축적소등_비축적점등);
                    //_btnManager.EnableSpecificButton();
                    areaManagerObj.GetArea2_1Btn(Next);
                    NextEnable(false);
                }
                break;
            case RTypeRFireAlarmSystemESection.스프레이연기감지1:
                {
                    //NextDisable();
                    //_btnManager.EnableSpecificImage(inventoryObj.ShowSmokeDetect(true), area2_1SmokeDetector);
                    ShowRoom(RTypeRRoomType.Area2_1);
                    bool isDetected = false;
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                    uIDragAndCollisionHandler.StartDragging();
                    inventoryObj.ShowSmokeDetect(true);
                    uIDragAndCollisionHandler.OnCollisionDetected += ((draggedObject, targetObject) =>
                    {
                        if (isDetected)
                            return;
                        isDetected = true;
                        Next();
                        //smokeDetectorPopup2.gameObject.SetActive(true);
                    });
                }
                break;
            case RTypeRFireAlarmSystemESection.스프레이연기감지2:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.Area2_1);
                    inventoryObj.ShowSmokeDetect(false);
                    area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(true);
                    area2_1SmokeDetectorPopup1.InitFireAlarmOff(isOn =>
                    {
                        //ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, isOn);
                        //ControlPanel.Instance.ShowFire(true);
                        //SoundManager.Instance.MuteSiren(true);
                        //smokeDetectorOn.SetActive(true);
                        //smokeDetector
                        ShowRoom(RTypeRRoomType.Area2_1);
                        area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                        inventoryObj.ShowSmokeDetect(false);
                        //_btnManager.EnableSpecificButton();
                        areaManagerObj.GetRTypeRBtn(Next);
                        //_btnManager.EnableSpecificImage(area2_1SmokeDetector, area2_1SmokeDetectorPopup1.onObj.transform.parent.gameObject);
                        //_btnManager.HighlightObj();
                        
                    });
                    NextEnable(false);
                }
                break;
            case RTypeRFireAlarmSystemESection.미작동:
                {
                    area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지, RBtnType.비상방송정지, RBtnType.축적소등_비축적점등);
                    //_btnManager.EnableSpecificButton(areaManagerObj.GetArea2_1Btn(Next));
                    //_btnManager.EnableSpecificImage(_rPanel.GetFireAlarmBtn().gameObject);
                    areaManagerObj.GetArea2_1Btn(Next);
                    NextEnable(false);
                }
                break;
            case RTypeRFireAlarmSystemESection.R수신기1:
                {
                    ShowRoom(RTypeRRoomType.Area2_1);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지, RBtnType.비상방송정지, RBtnType.축적소등_비축적점등);
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                    smokeTestPopup.gameObject.SetActive(false);
                    //_btnManager.EnableSpecificButton();
                    GetArea2_1SmokeDetector(delegate
                    {
                        area2_1SmokeDetectorPopup2.Init(RTypeRSmokeDetectorPopupType.LineOn);
                        Next();
                    });
                    NextEnable(false);
                }
                break;
            case RTypeRFireAlarmSystemESection.현장2:
                {
                    ShowRoom(RTypeRRoomType.Area2_1);
                    SetArea2_1SmokeDetector(false);
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(true);
                    smokeTestPopup.gameObject.SetActive(false);
                    area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOn);
                    uIDragAndCollisionHandler.StartDragging();
                    inventoryObj.ShowTester(true);
                    //_btnManager.EnableSpecificImage(inventoryObj.ShowTester(true));
                    bool isDetected = false;
                    uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        Debug.Log($"드래그 : {draggedObject.name} 타겟 : {targetObject.name}");
                        if (isDetected)
                            return;
                        isDetected = true;
                        //area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
     
                        Next();
                    };
                    
                }
                break;
            case RTypeRFireAlarmSystemESection.감지기탈거:
                {
                    inventoryObj.ShowTester(false);
                    uIDragAndCollisionHandler.ResetEvent();
                    smokeTestPopup.ShowTesterPen(RTypeRTesterPenType.SmokePopup);
                    smokeTestPopup.ShowTesterNum(RTypeRTesterNumType.Num2059);
                    smokeTestPopup.ShowTesterPen(RTypeRTesterPenType.SmokePopup);
                    smokeTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    smokeTestPopup.gameObject.SetActive(true);
                    F2CorrectBtn.gameObject.SetActive(true);
                    F2WrongBtn.gameObject.SetActive(true);
                    F2CorrectBtn.onClick.AddListener(delegate
                    {
                        _gCanvas.totalScore.화재경보.평가1 = true;
                        Next();
                    });
                    F2WrongBtn.onClick.AddListener(delegate
                    {
                        _gCanvas.totalScore.화재경보.평가1 = false;
                        Next();
                    });
                    NextEnable(false);
                    /*
                    ShowRoom(RTypeRRoomType.Area2_1);
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                    smokeTestPopup.Init(RTypeRSmokeDetectorPopupType.LineOn);
                    smokeTestPopup.ShowTesterNum(RTypeRTesterNumType.Off);
                    smokeTestPopup.ShowTesterPen(RTypeRTesterPenType.Default);
                    smokeTestPopup.ShowTesterPower(RTypeRTesterPowerType.Off);
                    smokeTestPopup.gameObject.SetActive(true);
                    inventoryObj.ShowTester(false);
                    SetArea2_1SmokeDetector(false);
                    //_btnManager.EnableSpecificButton(smokeTestPopup.smokeDetectorBtn);
                    //_btnManager.EnableSpecificImage(smokeTestPopup.testerPenObjs[0].gameObject);
                    uIDragAndCollisionHandler.StartDragging();
                    bool isDetected = false;
                    uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (!smokeTestPopup.testerPenObjs[0].gameObject.Equals(draggedObject))
                            return;
                        if (isDetected)
                            return;
                        isDetected = true;
                        _btnManager.EnableSpecificImage();
                        //area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                        smokeTestPopup.ShowTesterPen(RTypeRTesterPenType.SmokePopup);
                        NextEnable(false);
                    };
                    */
                }
                break;
            case RTypeRFireAlarmSystemESection.전압측정연결1:
                {
                    //NextDisable();
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                    smokeTestPopup.gameObject.SetActive(false);
                    //NextDisable();
                    //_btnManager.EnableSpecificImage(inventoryObj.ShowSmokeDetect(true), area2_1SmokeDetector);
                    inventoryObj.ShowSmokeDetect(true);
                    inventoryObj.ShowSmokeHead(false);
                    ShowRoom(RTypeRRoomType.Area2_1);
                    bool isDetected = false;
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                    uIDragAndCollisionHandler.StartDragging();
                    area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
                    uIDragAndCollisionHandler.OnCollisionDetected += ((draggedObject, targetObject) =>
                    {
                        if (!inventoryObj.smokeDetect.Equals(draggedObject))
                            return;
                        if (isDetected)
                            return;
                        isDetected = true;
                        Next();
                        //smokeDetectorPopup2.gameObject.SetActive(true);
                    });
                    NextEnable(false);
                }
                break;
            case RTypeRFireAlarmSystemESection.전압측정연결2:
                {
                    //NextDisable();
                    ShowRoom(RTypeRRoomType.Area2_1);
                    inventoryObj.ShowSmokeDetect(false);
                    area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(true);
                    area2_1SmokeDetectorOn.SetActive(false);
                    area2_1SmokeDetectorPopup1.onObj.SetActive(false);
                    area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
                    area2_1SmokeDetectorPopup1.InitFireAlarmOn(isOn =>
                    {
                        //ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, isOn);
                        //ControlPanel.Instance.ShowFire(true);
                        //SoundManager.Instance.MuteSiren(true);
                        //smokeDetectorOn.SetActive(true);
                        //smokeDetector
                        //area2_1SmokeDetectorPopup1.smokeCheckObj.gameObject.SetActive(false);
                        area2_1SmokeDetectorOn.SetActive(true);
                        //_btnManager.EnableSpecificImage(area2_1SmokeDetector, area2_1SmokeDetectorPopup1.onObj.transform.parent.gameObject);
                        //_btnManager.HighlightObj();
                       
                    });
                    NextEnable(false);
                }
                break;
            // case RTypeRFireAlarmSystemESection.전압측정전원온1:
            //     {
            //         //NextDisable();
            //         ShowRoom(RTypeRRoomType.Area2_1);
            //         smokeTestPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOn);
            //         smokeTestPopup.ShowTesterNum(RTypeRTesterNumType.Off);
            //         smokeTestPopup.ShowTesterPen(RTypeRTesterPenType.SmokePopup);
            //         smokeTestPopup.ShowTesterPower(RTypeRTesterPowerType.Off);
            //         smokeTestPopup.gameObject.SetActive(true);
            //         //_btnManager.EnableSpecificButton(smokeTestPopup.GetPowerBtn(Next));
            //         smokeTestPopup.GetPowerBtn(Next);
            //     }
            //     break;
            // case RTypeRFireAlarmSystemESection.전압정상확인:
            //     {
            //         //NextDisable();
            //         ShowRoom(RTypeRRoomType.Area2_1);
            //         smokeTestPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOn);
            //         smokeTestPopup.ShowTesterNum(RTypeRTesterNumType.Num2059);
            //         smokeTestPopup.ShowTesterPen(RTypeRTesterPenType.SmokePopup);
            //         smokeTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
            //         area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
            //         smokeTestPopup.gameObject.SetActive(true);
            //         F2CorrectBtn.gameObject.SetActive(true);
            //         F2WrongBtn.gameObject.SetActive(true);
            //         F2CorrectBtn.onClick.AddListener(delegate
            //         {
            //             _gCanvas.totalScore.화재경보.평가2 = true;
            //             Next();
            //         });
            //         F2WrongBtn.onClick.AddListener(delegate
            //         {
            //             _gCanvas.totalScore.화재경보.평가2 = false;
            //             Next();
            //         });
            //         NextEnable(false);
            //     }
            //     break;
            // case RTypeRFireAlarmSystemESection.감지기교체:
            //     {
            //         //NextDisable();
            //         ShowRoom(RTypeRRoomType.Area2_1);
            //         smokeTestPopup.gameObject.SetActive(false);
            //         area2_1SmokeDetectorPopup2.gameObject.SetActive(true);
            //         area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOn);
            //         //_btnManager.EnableSpecificButton( area2_1SmokeDetectorPopup2.GetSmokeDetectorBtn(Next));
            //         area2_1SmokeDetectorPopup2.GetSmokeDetectorBtn(Next);
            //     }
            //     break;
            // case RTypeRFireAlarmSystemESection.감지기선로분리:
            //     {
            //         area2_1SmokeDetectorPopup2.gameObject.SetActive(true);
            //         inventoryObj.ShowSmokeBase(false);
            //         area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.Empty);
            //         NextEnable(false);
            //     }
            //     break;
            // case RTypeRFireAlarmSystemESection.감지기제거및교체:
            //     {
            //         area2_1SmokeDetectorPopup2.gameObject.SetActive(true);
            //         inventoryObj.ShowSmokeHead(false);
            //         area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.Empty);
            //         //_btnManager.EnableSpecificImage(inventoryObj.ShowSmokeBase(true));
            //         inventoryObj.ShowSmokeBase(true);
            //         uIDragAndCollisionHandler.StartDragging();
            //         bool isDetected = false;
            //         uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
            //         {
            //             if (!draggedObject.Equals(inventoryObj.ShowSmokeBase(true)))
            //                 return;
            //             if (isDetected)
            //                 return;
            //             isDetected = true;
            //             inventoryObj.ShowSmokeBase(false);
            //             Next();
            //         };
            //         NextEnable(false);
            //     }
            //     break;
            // case RTypeRFireAlarmSystemESection.감지기선로연결:
            //     {
            //         area2_1SmokeDetectorPopup2.gameObject.SetActive(true);
            //         area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOn);
            //         inventoryObj.ShowSmokeBase(false);
            //         inventoryObj.ShowSmokeDetect(false);
            //         //_btnManager.EnableSpecificImage();
            //         inventoryObj.ShowSmokeHead(true);
            //         uIDragAndCollisionHandler.StartDragging();
            //         bool isDetected = false;
            //         uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
            //         {
            //             if (!draggedObject.Equals(inventoryObj.ShowSmokeHead(true)))
            //                 return;
            //             if (isDetected)
            //                 return;
            //             isDetected = true;
            //             inventoryObj.ShowSmokeHead(false);
            //             Next();
            //         };
            //         NextEnable(false);
            //     }
            //     break;
            // case RTypeRFireAlarmSystemESection.감지기결합:
            //     {
            //         area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
            //         //NextDisable();
            //         //_btnManager.EnableSpecificImage(inventoryObj.ShowSmokeDetect(true), area2_1SmokeDetector);
            //         inventoryObj.ShowSmokeDetect(true);
            //         inventoryObj.ShowSmokeHead(false);
            //         ShowRoom(RTypeRRoomType.Area2_1);
            //         bool isDetected = false;
            //         area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
            //         uIDragAndCollisionHandler.StartDragging();
            //         area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
            //         uIDragAndCollisionHandler.OnCollisionDetected += ((draggedObject, targetObject) =>
            //         {
            //             if (!inventoryObj.smokeDetect.Equals(draggedObject))
            //                 return;
            //             if (isDetected)
            //                 return;
            //             isDetected = true;
            //             Next();
            //             //smokeDetectorPopup2.gameObject.SetActive(true);
            //         });
            //         NextEnable(false);
            //         //area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.Default);
            //         //_btnManager.EnableSpecificImage(inventoryObj.ShowSmokeHead(true));
            //     }
            //     break;
            // case RTypeRFireAlarmSystemESection.스프레이연기감지3:
            //     {
            //         //NextDisable();
            //         ShowRoom(RTypeRRoomType.Area2_1);
            //         inventoryObj.ShowSmokeDetect(false);
            //         area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
            //         area2_1SmokeDetectorPopup1.gameObject.SetActive(true);
            //         area2_1SmokeDetectorOn.SetActive(false);
            //         area2_1SmokeDetectorPopup1.onObj.SetActive(false);
            //         area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
            //         area2_1SmokeDetectorPopup1.InitFireAlarmOn(isOn =>
            //         {
            //             //ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, isOn);
            //             //ControlPanel.Instance.ShowFire(true);
            //             //SoundManager.Instance.MuteSiren(true);
            //             //smokeDetectorOn.SetActive(true);
            //             //smokeDetector
            //             //area2_1SmokeDetectorPopup1.smokeCheckObj.gameObject.SetActive(false);
            //             area2_1SmokeDetectorOn.SetActive(true);
            //             //_btnManager.EnableSpecificImage(area2_1SmokeDetector, area2_1SmokeDetectorPopup1.onObj.transform.parent.gameObject);
            //             //_btnManager.HighlightObj();
            //            
            //         });
            //         NextEnable(false);
            //     }
            //     break;
            // case RTypeRFireAlarmSystemESection.R수신기2:
            //     {
            //         area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
            //         area2_1SmokeDetectorOn.SetActive(true);
            //         ShowRoom(RTypeRRoomType.Area2_1);
            //         _rPanel.SetFireAlarmEventNum(0);
            //         _rPanel.SoundCheck();
            //         _soundManager.SetBuzzerVolume(0);
            //         _soundManager.SetSirenVolume(0);
            //         //_btnManager.EnableSpecificButton(areaManagerObj.GetRTypeRBtn(Next));
            //         areaManagerObj.GetRTypeRBtn(Next);
            //         //_btnManager.EnableSpecificImage(_rPanel.GetFireAlarmBtn().gameObject);
            //         //NextDisable();
            //         _rPanel.ShowScreen(_rPanel.mainScreenImg);
            //         NextEnable(false);
            //     }
            //     break;
            // case RTypeRFireAlarmSystemPSection.R수신기3:
            //     {
            //         
            //     }
            //     break;
            case RTypeRFireAlarmSystemESection.주경종정지:
                {
                    area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                    _rPanel.ShowScreen(_rPanel.fireScreen2Img);
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지, RBtnType.비상방송정지, RBtnType.축적소등_비축적점등);
                    _rPanel.SetFireAlarmEventNum(1);
                    _rPanel.SoundCheck();
                    //_soundManager.SetAlarmVolume(0);
                    _soundManager.SetBuzzerVolume(0);
                    _soundManager.SetSirenVolume(0);
                   //_btnManager.EnableSpecificButton(areaManagerObj.GetArea2_1Btn(Next));
                    //_btnManager.EnableSpecificImage(_rPanel.GetFireAlarmBtn().gameObject);
                    NextEnable(false);
                }
                break;
            // case RTypeRFireAlarmSystemESection.비상방송음향확인:
            //     {
            //         ShowRoom(RTypeRRoomType.RTypeR);
            //         _rPanel.SetSwitchButton(false);
            //         // _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
            //         //     RBtnType.지구음향정지, RBtnType.비상방송정지, RBtnType.축적소등_비축적점등);
            //         _rPanel.SoundCheck();
            //         //_btnManager.EnableSpecificButton(areaManagerObj.GetArea2_1Btn(Next));
            //         //_btnManager.EnableSpecificImage(_rPanel.GetFireAlarmBtn().gameObject);
            //         NextEnable(false);
            //     }
            //     break;
            // case RTypeRFireAlarmSystemESection.비상방송음향정지:
            //     {
            //         _rPanel.ShowScreen(_rPanel.fireScreen2Img);
            //         ShowRoom(RTypeRRoomType.RTypeR);
            //         _rPanel.SetSwitchButton(false);
            //         _rPanel.SetSwitchButtons(true, RBtnType.축적소등_비축적점등);
            //         _rPanel.SetFireAlarmEventNum(1);
            //         _rPanel.SoundCheck();
            //         _soundManager.SetBuzzerVolume(0);
            //         _soundManager.SetSirenVolume(0);
            //         //_btnManager.EnableSpecificButton(areaManagerObj.GetArea2_1Btn(Next));
            //         //_btnManager.EnableSpecificImage(_rPanel.GetFireAlarmBtn().gameObject);
            //         NextEnable(false);
            //     }
            //     break;
            // case RTypeRFireAlarmSystemESection.화재경보클릭:
            //     {
            //         _rPanel.ShowScreen(_rPanel.fireScreen2Img);
            //         ShowRoom(RTypeRRoomType.RTypeR);
            //         _rPanel.SetSwitchButton(false);
            //         _rPanel.SetSwitchButtons(true, RBtnType.축적소등_비축적점등,
            //             RBtnType.주음향정지,
            //             RBtnType.지구음향정지,
            //             RBtnType.비상방송정지);
            //         //_btnManager.EnableSpecificButton();
            //         _rPanel.GetFireAlarmBtn(Next);
            //         NextEnable(false);
            //     }
            //     break;
            case RTypeRFireAlarmSystemESection.수신기복구:
                {
                    _rPanel.ShowScreen(_rPanel.fireScreen2Img);
                    ShowRoom(RTypeRRoomType.RTypeR);
                    // _rPanel.SetSwitchButtons(true, RBtnType.축적소등_비축적점등,
                    //     RBtnType.주음향정지,
                    //     RBtnType.지구음향정지,
                    //     RBtnType.비상방송정지);
                    //_rPanel.SetSwitchButton(false);
                    _rPanel.SetFireAlarmEventNum(1, true);
                    _rPanel.GetSwitchButton("수신기복구").OnCheck(false);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = true;
                    _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.AddListener(DefaultFireMap);
                    // _panelBtnCheck.SetBtn(false);
                    // _panelBtnCheck.SetBtns(true, RBtnType.수신기복구);
                    // SetHighlightControlPanel(_panelBtnCheck);
                    // _panelBtnCheck.SetBtns(true,
                    //     RBtnType.수신기복구,
                    //     RBtnType.주음향정지,
                    //     RBtnType.축적소등_비축적점등,
                    //     RBtnType.지구음향정지,
                    //     RBtnType.비상방송정지,
                    //     RBtnType.메인펌프오토,
                    //     RBtnType.충압펌프오토,
                    //     RBtnType.sp펌프오토,
                    //     RBtnType.sp충압펌프오토);
                    // SetHighlightControlPanelCheck(_panelBtnCheck);
                    NextEnable(false);
                }
                break;
            /*
            case RTypeRFireAlarmSystemESection.음향복구:
                {
                    
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지, RBtnType.비상방송정지, RBtnType.축적소등_비축적점등);
                    _rPanel.SoundCheck();
                    // _panelBtnCheck.SetBtn(false);
                    // _panelBtnCheck.SetBtns(true, RBtnType.주음향정지,
                    //     RBtnType.지구음향정지,
                    //     RBtnType.비상방송정지);
                    // SetHighlightControlPanel(_panelBtnCheck);
                    // _panelBtnCheck.checkDic[RBtnType.주음향정지] = false;
                    // _panelBtnCheck.checkDic[RBtnType.지구음향정지] = false;
                    // _panelBtnCheck.checkDic[RBtnType.비상방송정지] = false;
                    // _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토,
                    //     RBtnType.축적소등_비축적점등,
                    //     RBtnType.충압펌프오토,
                    //     RBtnType.sp펌프오토,
                    //     RBtnType.sp충압펌프오토);
                    // SetHighlightControlPanelCheck(_panelBtnCheck);
                    NextEnable(false);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                }
                break;
                */
            // case RTypeRFireAlarmSystemESection.축적상태전환:
            //     {
            //         ShowRoom(RTypeRRoomType.RTypeR);
            //         _rPanel.SetSwitchButton(false);
            //         _rPanel.SetSwitchButtons(true, RBtnType.축적소등_비축적점등);
            //         _rPanel.SoundCheck();
            //         NextEnable(false);
            //         _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
            //     }
            //     break;
            case RTypeRFireAlarmSystemESection.운영기록:
                {
                    _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.RemoveListener(DefaultFireMap);
                    _rPanel.SetFireAlarmEventNum(0);
                    _rPanel.ShowScreen(_rPanel.fireAlarmMap2Img);
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    //_btnManager.EnableSpecificButton();
                    _rPanel.GetOperationLogBtn(Next);
                    NextEnable(false);
                    
                }
                break;
            case RTypeRFireAlarmSystemESection.전체:
                {
                    //_rPanel.ShowPanel();
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.inputString = "";
                    _rPanel.UpdateDisplay();
                    _rPanel.ShowScreen(_rPanel.operationSearchObj);
                    //_btnManager.EnableSpecificButton();
                    //_rPanel.GetOperationLogAllBtn(Next);
                    // _rPanel.GetSearchBtn(delegate
                    // {
                    //     _rPanel.SearchOperation();
                    //     Next();
                    // });
                    RTypeRPanel.OnCheckDate.RemoveAllListeners();
                    RTypeRPanel.OnCheckDate.AddListener((isCheck) =>
                    {
                        if (!isCheck)
                            return;
                        _rPanel.GetSearchBtn(delegate
                        {
                            _rPanel.SearchOperation();
                            _gCanvas.totalScore.화재경보.평가2 = true;
                            Next();
                        });
                    });
                    
                    //_btnManager.EnableSpecificButton(btnList.ToArray());
                    //NextDisable();
                    RTypeRPanel.OnLogResultCheck.RemoveAllListeners();
                    //NextEnable(false);
                    //NextDisable();
                }
                break;
            // case RTypeRFireAlarmSystemESection.조회:
            //     {
            //         ShowRoom(RTypeRRoomType.RTypeR);
            //         _rPanel.inputString = "";
            //         _rPanel.UpdateDisplay();
            //         _rPanel.ShowScreen(_rPanel.operationSearchObj);
            //         _rPanel.GetSearchBtn(delegate
            //         {
            //             _rPanel.SearchOperation();
            //             Next();
            //         });
            //         
            //         NextEnable(false);
            //         
            //     }
            //     break;
            // case RTypeRFireAlarmSystemESection.이력확인:
            //     {
            //         ShowRoom(RTypeRRoomType.RTypeR);
            //         _rPanel.SearchOperation();
            //         _rPanel.ShowScreen(_rPanel.operationResultObj);
            //         _rPanel.scrollRect.verticalNormalizedPosition = 1f;
            //         _rPanel.GetUpBtn(delegate
            //         {
            //             _rPanel.ScrollUp();
            //         });
            //         _rPanel.GetDownBtn(delegate
            //         {
            //             _rPanel.ScrollDown();
            //         });
            //         // RTypeRPanel.OnLogResultCheck.RemoveAllListeners();
            //         // RTypeRPanel.OnLogResultCheck.AddListener(delegate
            //         // {
            //         //     NextEnable();
            //         // });
            //         NextEnable(false);
            //     }
            //     break;
            case RTypeRFireAlarmSystemESection.초기화면:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    RTypeRPanel.OnLogResultCheck.RemoveAllListeners();
                    _rPanel.ShowScreen(_rPanel.operationResultObj);
                    _rPanel.scrollRect.verticalNormalizedPosition = 0f;
                    //_btnManager.EnableSpecificButton();
                    areaManagerObj.GetArea2_1Btn(Next);
                    NextEnable(false);
                }
                break;
            case RTypeRFireAlarmSystemESection.현장3:
                {
                    area2_1SmokeDetectorOn.SetActive(false);
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                    ShowRoom(RTypeRRoomType.Area2_1);
                    _soundManager.ZeroVolume();
                    NextEnable(false);
                }
                break;
            case RTypeRFireAlarmSystemESection.평가종료:
                {
                    //CompletePopup("화재경보");
                    var results = new List<ResultObject>();

                    results.Add(new ResultObject()
                    {
                        Title = "불량 내용 확인",
                        IsSuccess = _gCanvas.totalScore.화재경보.평가1
                    });
                    results.Add(new ResultObject()
                    {
                        Title = "운영 기록 확인",
                        IsSuccess = _gCanvas.totalScore.화재경보.평가2
                    });
                    _gCanvas.totalScore.화재경보List.Clear();
                    _gCanvas.totalScore.화재경보List.AddRange(results);
                    _gCanvas.SetNextEvaluation(InitCircuitBreaker, InitFireAlarmSystem);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    

    public void ChangeStateFireAlarmSystemP(RTypeRFireAlarmSystemPSection state)
    {

        _curRTypeRFireAlarmSystemPSection = state;
        OnStateFireAlarmSystemP(_curRTypeRFireAlarmSystemPSection);
    }
    
    private void OnStateFireAlarmSystemP(RTypeRFireAlarmSystemPSection state)
    {
        _btnManager.RemoveAllHighlights();
        _btnManager.EnableSpecificButton();
        _btnManager.EnableSpecificImage();
        RTypeRPanel.RTypeRPanelButtonAction.RemoveAllListeners();
        uIDragAndCollisionHandler.StopDragging();
        uIDragAndCollisionHandler.ResetEvent();
        switch(state)
        {

            case RTypeRFireAlarmSystemPSection.음향정지:
                {
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                    _rPanel.ClearLog();
                    NextDisable();
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetFireAlarmEventNum(0);
                    _rPanel.SetEquipmentOperationEventNum(0);
                    _rPanel.SetSwitchButton(false);
                    // _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                    //     RBtnType.지구음향정지, RBtnType.비상방송정지);
                    _soundManager.PlayAlarm(false);
                    _soundManager.PlayAlarm2(false);
                    _soundManager.PlayBroadcast(false);
                    _soundManager.SetBuzzerVolume(0);
                    _soundManager.SetSirenVolume(0);
                    SetArea2_1SmokeDetector(false);
                    _rPanel.SoundCheck();
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.SetBtns(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지,
                        RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanel(_panelBtnCheck, true);
                    _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                }
                break;
            case RTypeRFireAlarmSystemPSection.비축적:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지, RBtnType.비상방송정지);
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.SetBtns(true, RBtnType.축적소등_비축적점등,
                        RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지,
                        RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanel(_panelBtnCheck, true);
                    _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                }
                
                break;
            case RTypeRFireAlarmSystemPSection.현장1:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지, RBtnType.비상방송정지, RBtnType.축적소등_비축적점등);
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.SetBtns(true, RBtnType.축적소등_비축적점등,
                        RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지,
                        RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanel(_panelBtnCheck, true);
                    _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    _btnManager.EnableSpecificButton(areaManagerObj.GetArea2_1Btn(Next));
                    NextDisable();
                }
                break;
            case RTypeRFireAlarmSystemPSection.스프레이연기감지1:
                {
                    NextDisable();
                    _btnManager.EnableSpecificImage(inventoryObj.ShowSmokeDetect(true), area2_1SmokeDetector);
                    ShowRoom(RTypeRRoomType.Area2_1);
                    bool isDetected = false;
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                    uIDragAndCollisionHandler.StartDragging();
                    
                    uIDragAndCollisionHandler.OnCollisionDetected += ((draggedObject, targetObject) =>
                    {
                        if (isDetected)
                            return;
                        isDetected = true;
                        Next();
                        //smokeDetectorPopup2.gameObject.SetActive(true);
                    });
                }
                break;
            case RTypeRFireAlarmSystemPSection.스프레이연기감지2:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.Area2_1);
                    inventoryObj.ShowSmokeDetect(false);
                    area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(true);
                    area2_1SmokeDetectorPopup1.InitFireAlarmOff(isOn =>
                    {
                        //ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, isOn);
                        //ControlPanel.Instance.ShowFire(true);
                        //SoundManager.Instance.MuteSiren(true);
                        //smokeDetectorOn.SetActive(true);
                        //smokeDetector
                        area2_1SmokeDetectorPopup1.smokeCheckObj.gameObject.SetActive(false);
                        _btnManager.EnableSpecificImage(area2_1SmokeDetector, area2_1SmokeDetectorPopup1.onObj.transform.parent.gameObject);
                        _btnManager.HighlightObj();
                        NextEnable();
                        
                    });
                }
                break;
            case RTypeRFireAlarmSystemPSection.미작동:
                {
                    ShowRoom(RTypeRRoomType.Area2_1);
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                    inventoryObj.ShowSmokeDetect(false);
                    _btnManager.EnableSpecificButton(areaManagerObj.GetRTypeRBtn(Next));
                    NextDisable();
                }
                break;
            case RTypeRFireAlarmSystemPSection.R수신기1:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지, RBtnType.비상방송정지, RBtnType.축적소등_비축적점등);
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.SetBtns(true, RBtnType.축적소등_비축적점등,
                        RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지,
                        RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanel(_panelBtnCheck, true);
                    _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    _btnManager.EnableSpecificButton(areaManagerObj.GetArea2_1Btn(Next));
                    _btnManager.EnableSpecificImage(_rPanel.GetFireAlarmBtn().gameObject);
                    NextDisable();
                }
                break;
            case RTypeRFireAlarmSystemPSection.현장2:
                {
                    ShowRoom(RTypeRRoomType.Area2_1);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지, RBtnType.비상방송정지, RBtnType.축적소등_비축적점등);
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.SetBtns(true, RBtnType.축적소등_비축적점등,
                        RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지,
                        RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanel(_panelBtnCheck, true);
                    _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                    smokeTestPopup.gameObject.SetActive(false);
                    _btnManager.EnableSpecificButton(GetArea2_1SmokeDetector(delegate
                    {
                        area2_1SmokeDetectorPopup2.Init(RTypeRSmokeDetectorPopupType.Default);
                        Next();
                    }));
                    NextDisable();
                    
                }
                break;
            case RTypeRFireAlarmSystemPSection.감지기탈거:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.Area2_1);
                    SetArea2_1SmokeDetector(false);
                    inventoryObj.ShowTester(false);
                    area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.Default);
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(true);
                    inventoryObj.ShowSmokeHead(false);
                    inventoryObj.ShowSmokeBase(false);
                    _btnManager.EnableSpecificButton( area2_1SmokeDetectorPopup2.GetSmokeDetectorBtn(delegate
                    {
                        _btnManager.EnableSpecificButton();
                        area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.OpenAni);
                        area2_1SmokeDetectorPopup2.OpenAni(delegate
                        {
                            NextEnable();
                            //inventoryObj.ShowSmokeHead(true);
                            //smokeDetectorPopup.Check();
                        });
                    }));
                }
                break;
            case RTypeRFireAlarmSystemPSection.전압측정연결1:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.Area2_1);
                    SetArea2_1SmokeDetector(false);
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(true);
                    smokeTestPopup.gameObject.SetActive(false);
                    area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOn);
                    inventoryObj.ShowSmokeHead(true);
                    uIDragAndCollisionHandler.StartDragging();
                    _btnManager.EnableSpecificImage(inventoryObj.ShowTester(true), testerCheckImg);
                    bool isDetected = false;
                    uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (isDetected)
                            return;
                        isDetected = true;
                        //area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                        inventoryObj.ShowTester(false);
                        uIDragAndCollisionHandler.ResetEvent();
                        Next();
                    };
                    dragHandAniObj.SetActive(false);
                }
                break;
            case RTypeRFireAlarmSystemPSection.전압측정연결2:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.Area2_1);
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                    smokeTestPopup.Init(RTypeRSmokeDetectorPopupType.LineOn);
                    smokeTestPopup.ShowTesterNum(RTypeRTesterNumType.Off);
                    smokeTestPopup.ShowTesterPen(RTypeRTesterPenType.Default);
                    smokeTestPopup.ShowTesterPower(RTypeRTesterPowerType.Off);
                    smokeTestPopup.gameObject.SetActive(true);
                    inventoryObj.ShowTester(false);
                    SetArea2_1SmokeDetector(false);
  
                    dragHandAniObj.SetActive(true);
                    uIDragAndCollisionHandler.StartDragging();
                    _btnManager.EnableSpecificButton(smokeTestPopup.smokeDetectorBtn);
                    _btnManager.EnableSpecificImage(smokeTestPopup.testerPenObjs[0].gameObject);
                    bool isDetected = false;
                    uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (!smokeTestPopup.testerPenObjs[0].gameObject.Equals(draggedObject))
                            return;
                        if (isDetected)
                            return;
                        isDetected = true;
                        _btnManager.EnableSpecificImage();
                        //area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                        smokeTestPopup.ShowTesterPen(RTypeRTesterPenType.SmokePopup);
                        //NextEnable();
                        Next();
                    };
                    //NextEnable();
                    
                }
                break;
            case RTypeRFireAlarmSystemPSection.전압측정전원온1:
                {
                    dragHandAniObj.SetActive(false);
                    NextDisable();
                    ShowRoom(RTypeRRoomType.Area2_1);
                    smokeTestPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOn);
                    smokeTestPopup.ShowTesterNum(RTypeRTesterNumType.Off);
                    //smokeTestPopup.ShowTesterPen(RTypeRTesterPenType.Default);
                    smokeTestPopup.ShowTesterPen(RTypeRTesterPenType.SmokePopup);
                    smokeTestPopup.ShowTesterPower(RTypeRTesterPowerType.Off);
                    smokeTestPopup.gameObject.SetActive(true);
                    //uIDragAndCollisionHandler.StartDragging();
                    _btnManager.EnableSpecificButton(smokeTestPopup.GetPowerBtn(Next));
                    //_btnManager.EnableSpecificImage(smokeTestPopup.testerPenObjs[0].gameObject);
                    //bool isDetected = false;
                    // uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    // {
                    //     if (!smokeTestPopup.testerPenObjs[0].gameObject.Equals(draggedObject))
                    //         return;
                    //     if (isDetected)
                    //         return;
                    //     isDetected = true;
                    //     _btnManager.EnableSpecificImage();
                    //     //area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                    //     smokeTestPopup.ShowTesterPen(RTypeRTesterPenType.SmokePopup);
                    //     //NextEnable();
                    //     Next();
                    // };
                    //_btnManager.EnableSpecificButton(smokeTestPopup.GetPowerBtn(Next));
                }
                break;
            case RTypeRFireAlarmSystemPSection.전압정상확인:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.Area2_1);
                    smokeTestPopup.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOn);
                    smokeTestPopup.ShowTesterNum(RTypeRTesterNumType.Num2059);
                    smokeTestPopup.ShowTesterPen(RTypeRTesterPenType.SmokePopup);
                    smokeTestPopup.ShowTesterPower(RTypeRTesterPowerType.On);
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                    smokeTestPopup.gameObject.SetActive(true);
                    NextEnable();
                }
                break;
            case RTypeRFireAlarmSystemPSection.감지기교체:
                {
                    NextDisable();
                    ShowRoom(RTypeRRoomType.Area2_1);
                    smokeTestPopup.gameObject.SetActive(false);
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(true);
                    area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOn);
                    inventoryObj.ShowSmokeHead(true);
                    inventoryObj.ShowSmokeBase(false);
                    _btnManager.EnableSpecificButton( area2_1SmokeDetectorPopup2.GetSmokeDetectorBtn(() =>
                    {
                        area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.Empty);
                        inventoryObj.ShowSmokeBase(true);
                        NextEnable();
                    }));
                    
                }
                break;
            case RTypeRFireAlarmSystemPSection.감지기선로분리:
                {
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(true);
                    inventoryObj.ShowSmokeBase(true);
                    inventoryObj.ShowSmokeHead(true);
                    area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.Empty);
                    uIDragAndCollisionHandler.StartDragging();
                    _btnManager.EnableSpecificButton(smokeTestPopup.smokeDetectorBtn);
                    _btnManager.EnableSpecificImage(inventoryObj.smokeDetectBase, smokeCheckImg);
                    bool isDetected = false;
                    
                    uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (!draggedObject.Equals(inventoryObj.ShowSmokeBase(true)))
                            return;
                        if (isDetected)
                            return;
                        isDetected = true;
                        uIDragAndCollisionHandler.ResetEvent();
                        inventoryObj.ShowSmokeBase(false);
                        Next();
                    };
                    NextDisable();
                }
                break;
            case RTypeRFireAlarmSystemPSection.감지기제거및교체:
                {
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(true);
                    inventoryObj.ShowSmokeHead(true);
                    inventoryObj.ShowSmokeDetect(false);
                    area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.LineOn);
                    _btnManager.EnableSpecificImage(inventoryObj.ShowSmokeHead(true), smokeCheckImg);
                    uIDragAndCollisionHandler.StartDragging();
                    bool isDetected = false;
                    uIDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
                    {
                        if (!draggedObject.Equals(inventoryObj.ShowSmokeHead(true)))
                            return;
                        if (isDetected)
                            return;
                        isDetected = true;
                        uIDragAndCollisionHandler.ResetEvent();
                        inventoryObj.ShowSmokeHead(false);
                        area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.CloseAni);
                        area2_1SmokeDetectorPopup2.CloseAni(delegate
                        {
                            _btnManager.EnableSpecificImage();
                            NextEnable();
                            //inventoryObj.ShowSmokeHead(false);
                            //inventoryObj.ShowSmokeHead(true);
                        });
                        //area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                        //NextEnable();
                    };
                    NextDisable();
                 
                }
                break;
            case RTypeRFireAlarmSystemPSection.감지기선로연결:
                {
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                    inventoryObj.ShowSmokeHead(false);
                    // area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.Default);
                    // inventoryObj.ShowSmokeDetect(false);
                    // _btnManager.EnableSpecificImage(inventoryObj.ShowSmokeHead(false));
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                    NextDisable();
                    _btnManager.EnableSpecificImage(inventoryObj.ShowSmokeDetect(true), area2_1SmokeDetector);
                    ShowRoom(RTypeRRoomType.Area2_1);
                    bool isDetected = false;
                    area2_1SmokeDetectorOn.SetActive(false);
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                    uIDragAndCollisionHandler.StartDragging();
                    area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
                    uIDragAndCollisionHandler.OnCollisionDetected += ((draggedObject, targetObject) =>
                    {
                        if (!inventoryObj.smokeDetect.Equals(draggedObject))
                            return;
                        if (isDetected)
                            return;
                        isDetected = true;
                        Next();
                        //smokeDetectorPopup2.gameObject.SetActive(true);
                    });
             
                }
                break;
            case RTypeRFireAlarmSystemPSection.감지기결합:
                {
                    area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
                    NextDisable();
                    _btnManager.EnableSpecificImage(inventoryObj.ShowSmokeDetect(true), area2_1SmokeDetector);
                    ShowRoom(RTypeRRoomType.Area2_1);
                    bool isDetected = false;
                    area2_1SmokeDetectorOn.SetActive(false);
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                    uIDragAndCollisionHandler.StartDragging();
                    area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
                    uIDragAndCollisionHandler.OnCollisionDetected += ((draggedObject, targetObject) =>
                    {
                        if (!inventoryObj.smokeDetect.Equals(draggedObject))
                            return;
                        if (isDetected)
                            return;
                        isDetected = true;
                        //Next();
                        inventoryObj.ShowSmokeDetect(false);
                        area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
                        area2_1SmokeDetectorPopup1.InitFireAlarmOn(isOn =>
                        {
                            // area2_1SmokeDetectorOn.SetActive(true);
                            // _btnManager.EnableSpecificImage(area2_1SmokeDetector, area2_1SmokeDetectorPopup1.onObj.transform.parent.gameObject);
                        
                            area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                            area2_1SmokeDetectorOn.SetActive(true);
                            ShowRoom(RTypeRRoomType.Area2_1);
                            _rPanel.SetFireAlarmEventNum(0);
                            _rPanel.SoundCheck();
                            _soundManager.SetBuzzerVolume(0);
                            _soundManager.SetSirenVolume(0);
                            _btnManager.EnableSpecificButton(areaManagerObj.GetRTypeRBtn(Next));
                            //_btnManager.EnableSpecificImage(_rPanel.GetFireAlarmBtn().gameObject);
                            //NextDisable();
                            _rPanel.ShowScreen(_rPanel.mainScreenImg);
                            _btnManager.HighlightObj();
                            Next();
                        });
                        area2_1SmokeDetectorPopup1.gameObject.SetActive(true);
                        //smokeDetectorPopup2.gameObject.SetActive(true);
                    });
                    //area2_1SmokeDetectorPopup2.ShowSmokeDetector(RTypeRSmokeDetectorPopupType.Default);
                    //_btnManager.EnableSpecificImage(inventoryObj.ShowSmokeHead(true));
                }
                break;
            case RTypeRFireAlarmSystemPSection.스프레이연기감지3:
                {
                    /*
                    NextDisable();
                    ShowRoom(RTypeRRoomType.Area2_1);
                    inventoryObj.ShowSmokeDetect(false);
                    area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(true);
                    area2_1SmokeDetectorOn.SetActive(false);
                    area2_1SmokeDetectorPopup1.onObj.SetActive(false);
                    area2_1SmokeDetectorPopup1.ResetSmokeDetectorTime();
                    area2_1SmokeDetectorPopup1.InitFireAlarmOn(isOn =>
                    {
                        // area2_1SmokeDetectorOn.SetActive(true);
                        // _btnManager.EnableSpecificImage(area2_1SmokeDetector, area2_1SmokeDetectorPopup1.onObj.transform.parent.gameObject);
                        
                        area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                        area2_1SmokeDetectorOn.SetActive(true);
                        ShowRoom(RTypeRRoomType.Area2_1);
                        _rPanel.SetFireAlarmEventNum(0);
                        _rPanel.SoundCheck();
                        _soundManager.SetBuzzerVolume(0);
                        _soundManager.SetSirenVolume(0);
                        _btnManager.EnableSpecificButton(areaManagerObj.GetRTypeRBtn(Next));
                        //_btnManager.EnableSpecificImage(_rPanel.GetFireAlarmBtn().gameObject);
                        NextDisable();
                        _rPanel.ShowScreen(_rPanel.mainScreenImg);
                        _btnManager.HighlightObj();
                    });
                    */
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                    area2_1SmokeDetectorOn.SetActive(true);
                    ShowRoom(RTypeRRoomType.Area2_1);
                    _rPanel.SetFireAlarmEventNum(0);
                    _rPanel.SoundCheck();
                
                    _soundManager.SetBuzzerVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _btnManager.EnableSpecificButton(areaManagerObj.GetRTypeRBtn(Next));
                    //_btnManager.EnableSpecificImage(_rPanel.GetFireAlarmBtn().gameObject);
                    //NextDisable();
                    _rPanel.ShowScreen(_rPanel.mainScreenImg);
                    _btnManager.HighlightObj();
                }
                break;
            case RTypeRFireAlarmSystemPSection.R수신기2:
                {
                    // area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                    // area2_1SmokeDetectorOn.SetActive(true);
                    // ShowRoom(RTypeRRoomType.Area2_1);
                    // _rPanel.SetFireAlarmEventNum(0);
                    // _rPanel.SoundCheck();
                    // _soundManager.SetBuzzerVolume(0);
                    // _soundManager.SetSirenVolume(0);
                    // _btnManager.EnableSpecificButton(areaManagerObj.GetRTypeRBtn(Next));
                    // //_btnManager.EnableSpecificImage(_rPanel.GetFireAlarmBtn().gameObject);
                    // NextDisable();
                    // _rPanel.ShowScreen(_rPanel.mainScreenImg);
                    _rPanel.ShowScreen(_rPanel.fireScreen2Img);
                    ShowRoom(RTypeRRoomType.RTypeR);
                    //_rPanel.SetSwitchButton(false);
                    // _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                    //     RBtnType.지구음향정지, RBtnType.비상방송정지, RBtnType.축적소등_비축적점등);
    
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지, RBtnType.비상방송정지, RBtnType.축적소등_비축적점등);
                    _rPanel.SetFireAlarmEventNum(1);
                    _rPanel.SoundCheck();
                    //_soundManager.SetAlarmVolume(0);
                    _soundManager.SetBuzzerVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.SetBtns(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지);
                    // RBtnType.메인펌프오토,
                    // RBtnType.충압펌프오토,
                    // RBtnType.sp펌프오토,
                    // RBtnType.sp충압펌프오토);
                    SetHighlightControlPanel(_panelBtnCheck);
                    _panelBtnCheck.checkDic[RBtnType.주음향정지] = false;
                    _panelBtnCheck.checkDic[RBtnType.지구음향정지] = false;
                    _panelBtnCheck.checkDic[RBtnType.비상방송정지] = false;
                    //_panelBtnCheck.checkDic[RBtnType.부저정지] = false;
                    _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토,
                        RBtnType.축적소등_비축적점등,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                }
                break;
            // case RTypeRFireAlarmSystemPSection.R수신기3:
            //     {
            //         
            //     }
            //     break;
            case RTypeRFireAlarmSystemPSection.주경종정지:
                {
                    _rPanel.ShowScreen(_rPanel.fireScreen2Img);
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.축적소등_비축적점등);
                    _rPanel.SetFireAlarmEventNum(1);
                    _rPanel.SoundCheck();
                    _soundManager.SetAlarmVolume(0);
                    _soundManager.SetBuzzerVolume(0);
                    _soundManager.SetSirenVolume(0);
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.SetBtns(true, RBtnType.축적소등_비축적점등,
                        RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지,
                        RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanel(_panelBtnCheck, true);
                    _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    //_btnManager.EnableSpecificButton(areaManagerObj.GetArea2_1Btn(Next));
                    //_btnManager.EnableSpecificImage(_rPanel.GetFireAlarmBtn().gameObject);
                    NextDisable();
                }
                break;
            case RTypeRFireAlarmSystemPSection.비상방송음향확인:
                {
                    /*
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지, RBtnType.비상방송정지, RBtnType.축적소등_비축적점등);
                    _rPanel.SoundCheck();
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.SetBtns(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지);
                        // RBtnType.메인펌프오토,
                        // RBtnType.충압펌프오토,
                        // RBtnType.sp펌프오토,
                        // RBtnType.sp충압펌프오토);
                    SetHighlightControlPanel(_panelBtnCheck);
                    _panelBtnCheck.checkDic[RBtnType.주음향정지] = false;
                    _panelBtnCheck.checkDic[RBtnType.지구음향정지] = false;
                    _panelBtnCheck.checkDic[RBtnType.비상방송정지] = false;
                    _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토,
                        RBtnType.축적소등_비축적점등,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    */
                    //_btnManager.EnableSpecificButton(areaManagerObj.GetArea2_1Btn(Next));
                    //_btnManager.EnableSpecificImage(_rPanel.GetFireAlarmBtn().gameObject);
                    _rPanel.ShowScreen(_rPanel.fireScreen2Img);
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.축적소등_비축적점등,
                        RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지);
                    _btnManager.EnableSpecificButton(_rPanel.GetFireAlarmBtn(Next));
                    NextDisable();
                    _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.RemoveListener(DefaultFireMap);
                }
                break;
            case RTypeRFireAlarmSystemPSection.비상방송음향정지:
                {
                    // _rPanel.ShowScreen(_rPanel.fireScreen2Img);
                    // ShowRoom(RTypeRRoomType.RTypeR);
                    // _rPanel.SetSwitchButton(false);
                    // _rPanel.SetSwitchButtons(true, RBtnType.축적소등_비축적점등);
                    // _rPanel.SetFireAlarmEventNum(1);
                    // _rPanel.SoundCheck();
                    // _soundManager.SetBuzzerVolume(0);
                    // _soundManager.SetSirenVolume(0);
                    // _panelBtnCheck.SetBtn(false);
                    // _panelBtnCheck.SetBtns(true, RBtnType.주음향정지,
                    //     RBtnType.지구음향정지,
                    //     RBtnType.비상방송정지);
                    // SetHighlightControlPanel(_panelBtnCheck);
                    // _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토,
                    //     RBtnType.축적소등_비축적점등,
                    //     RBtnType.충압펌프오토,
                    //     RBtnType.sp펌프오토,
                    //     RBtnType.sp충압펌프오토);
                    // SetHighlightControlPanelCheck(_panelBtnCheck);
                    _rPanel.ShowScreen(_rPanel.fireAlarmMapImg);
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetFireAlarmEventNum(1, true);
                    _rPanel.SetSwitchButtons(true, RBtnType.축적소등_비축적점등,
                        RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지);
                    _rPanel.GetSwitchButton("수신기복구").OnCheck(false);
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = true;
                    _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.AddListener(DefaultFireMap);
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.SetBtns(true, RBtnType.수신기복구);
                    SetHighlightControlPanel(_panelBtnCheck);
                    _panelBtnCheck.SetBtns(true,
                        RBtnType.수신기복구,
                        RBtnType.주음향정지,
                        RBtnType.축적소등_비축적점등,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지,
                        RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    NextDisable();
                }
                break;
            case RTypeRFireAlarmSystemPSection.화재경보클릭:
                {
                    _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.RemoveListener(DefaultFireMap);
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지, RBtnType.비상방송정지, RBtnType.축적소등_비축적점등);
                    _rPanel.SoundCheck();
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.SetBtns(true, RBtnType.주음향정지,
                        RBtnType.지구음향정지,
                        RBtnType.비상방송정지);
                    // RBtnType.메인펌프오토,
                    // RBtnType.충압펌프오토,
                    // RBtnType.sp펌프오토,
                    // RBtnType.sp충압펌프오토);
                    SetHighlightControlPanel(_panelBtnCheck);
                    _panelBtnCheck.checkDic[RBtnType.주음향정지] = false;
                    _panelBtnCheck.checkDic[RBtnType.지구음향정지] = false;
                    _panelBtnCheck.checkDic[RBtnType.비상방송정지] = false;
                    _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토,
                        RBtnType.축적소등_비축적점등,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    NextDisable();
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                }
                break;
            case RTypeRFireAlarmSystemPSection.수신기복구:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _rPanel.SetSwitchButtons(true, RBtnType.축적소등_비축적점등);
                    _rPanel.SoundCheck();
                    _panelBtnCheck.SetBtn(false);
                    _panelBtnCheck.SetBtns(true, RBtnType.축적소등_비축적점등);
                    // RBtnType.메인펌프오토,
                    // RBtnType.충압펌프오토,
                    // RBtnType.sp펌프오토,
                    // RBtnType.sp충압펌프오토);
                    SetHighlightControlPanel(_panelBtnCheck);
                    // _panelBtnCheck.checkDic[RBtnType.주음향정지] = false;
                    // _panelBtnCheck.checkDic[RBtnType.지구음향정지] = false;
                    // _panelBtnCheck.checkDic[RBtnType.비상방송정지] = false;
                    _panelBtnCheck.checkDic[RBtnType.축적소등_비축적점등] = false;
                    _panelBtnCheck.SetBtns(true, RBtnType.메인펌프오토,
                        RBtnType.충압펌프오토,
                        RBtnType.sp펌프오토,
                        RBtnType.sp충압펌프오토);
                    SetHighlightControlPanelCheck(_panelBtnCheck);
                    NextDisable();
                    _rPanel.GetSwitchButton("수신기복구").GetButton().interactable = false;
                }
                break;
            case RTypeRFireAlarmSystemPSection.음향복구: //운영기록
                {
                    _rPanel.ShowScreen(_rPanel.fireAlarmMap2Img);
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.SetSwitchButton(false);
                    _btnManager.EnableSpecificButton(_rPanel.GetOperationLogBtn(Next));
                    NextDisable();
                }
                break;
            case RTypeRFireAlarmSystemPSection.축적상태전환:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.inputString = "";
                    _rPanel.UpdateDisplay();
                    _rPanel.ShowScreen(_rPanel.operationSearchObj);
                    _btnManager.EnableSpecificButton(_rPanel.GetOperationLogAllBtn(Next));
                }
                break;
            case RTypeRFireAlarmSystemPSection.운영기록:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.inputString = "";
                    _rPanel.UpdateDisplay();
                    _rPanel.ShowScreen(_rPanel.operationSearchObj);
                    List<Button> btnList = new List<Button>();
                    btnList.AddRange(_rPanel.numBtns);
                    btnList.Add(_rPanel.acBtn);
                    btnList.Add(_rPanel.deleteBtn);
                    // btnList.Add(_rPanel.GetSearchBtn(delegate
                    // {
                    //     _rPanel.SearchOperation();
                    //     Next();
                    // }));
                    RTypeRPanel.OnCheckDate.RemoveAllListeners();
                    RTypeRPanel.OnCheckDate.AddListener((isCheck) =>
                    {
                        if (!isCheck)
                            return;
                        _btnManager.EnableSpecificButton(_rPanel.GetSearchBtn(delegate
                        {
                            _rPanel.SearchOperation();
                            Next();
                        }));
                        _btnManager.HighlightObj();
                    });
                    
                    _btnManager.EnableSpecificButton(btnList.ToArray());
                    NextDisable();
                    RTypeRPanel.OnLogResultCheck.RemoveAllListeners();
                    
                }
                break;
            case RTypeRFireAlarmSystemPSection.전체:
                {
                    RTypeRPanel.OnCheckDate.RemoveAllListeners();
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.ShowScreen(_rPanel.operationResultObj);
                    _rPanel.scrollRect.verticalNormalizedPosition = 1f;
                    _btnManager.EnableSpecificButton(_rPanel.GetUpBtn(delegate
                    {
                        _rPanel.ScrollUp();
                        NextEnable();
                    }), _rPanel.GetDownBtn(delegate
                    {
                        _rPanel.ScrollDown();
                        NextEnable();
                    }));
                    // RTypeRPanel.OnLogResultCheck.RemoveAllListeners();
                    // RTypeRPanel.OnLogResultCheck.AddListener(delegate
                    // {
                    //     NextEnable();
                    // });
                    NextDisable();
                }
                break;
            case RTypeRFireAlarmSystemPSection.조회:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    RTypeRPanel.OnLogResultCheck.RemoveAllListeners();
                    _rPanel.ShowScreen(_rPanel.operationResultObj);
                    _rPanel.scrollRect.verticalNormalizedPosition = 0f;
                    _btnManager.EnableSpecificButton(_rPanel.GetInitScreenBtn(Next));
                    NextDisable();
                }
                break;
            case RTypeRFireAlarmSystemPSection.이력확인:
                {
                    ShowRoom(RTypeRRoomType.RTypeR);
                    _rPanel.ShowScreen(_rPanel.mainScreenImg);
                    _btnManager.EnableSpecificButton(areaManagerObj.GetArea2_1Btn(Next));
                    NextDisable();
                }
                break;
            case RTypeRFireAlarmSystemPSection.초기화면:
                {
                    area2_1SmokeDetectorOn.SetActive(false);
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                    ShowRoom(RTypeRRoomType.Area2_1);
                    _soundManager.ZeroVolume();
                    NextEnable();
                }
                break;
            case RTypeRFireAlarmSystemPSection.교육완료:
                {
                    NextDisable();
                    SetCompletePopup("화재경보 조치를 모두 완료했습니다.", "다음 시험을", InitCircuitBreaker);
                }
                break;
            /*
            case RTypeRFireAlarmSystemPSection.현장4:
                {
                    area2_1SmokeDetectorOn.SetActive(false);
                    area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
                    ShowRoom(RTypeRRoomType.Area2_1);
                    _soundManager.ZeroVolume();
                    NextEnable();
                }
                break;
            case RTypeRFireAlarmSystemPSection.교육완료:
                {
                    CompletePopup("화재경보 조치를", "다음 시험을", InitCircuitBreaker);
                }
                break;
                */
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        //_btnManager.HighlightObj();
        _rPanel.CheckWarringSwitch();
    }

    private void DefaultFireMap()
    {
        _rPanel.SetFireAlarmEventNum(0);
        _rPanel.ShowScreen(_rPanel.fireAlarmMap2Img);
    }
    
}
