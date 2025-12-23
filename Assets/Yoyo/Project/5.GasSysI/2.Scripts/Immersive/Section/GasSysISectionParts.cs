using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using VInspector;

public partial class GasSysISection
{
    [Foldout("구성요소")]
    private GasSysIPartsSection _curParts;
    public HintScriptableObj partsHintObj;
    public CinemachineVirtualCamera[] partsCamPosObjs;
    public GasSysIPartList partListObj;
    [FormerlySerializedAs("touchControlObj")]
    public ObjectControl objectControl;
    //public MouseObjectControl mouseControlObj;

    public GameObject[] parts;
    public GameObject partObj;
    public GameObject 저장용기Obj;
    public GameObject 기동용기가스용기Obj;
    
    public Dictionary<GameObject, TransformData> defaultPartsDict = new Dictionary<GameObject, TransformData>();
    
    [EndFoldout]
    public void InitParts()
    {
        Init();
        IsAuto(false);
        ObjectControl.Instance.EnableMoveMode();
        _camList.AddRange(partsCamPosObjs);
        //_camList[0].gameObject.SetActive(true);
        //SetCamPos(_camList[0].gameObject);
        curGasSysIState = GasSysIState.수동조작함작동수동;
        partListObj.ShowPanel(true);
        _hintPanel.Init(partsHintObj, NextAni, PrevAni);
        int maxSection = Enum.GetValues(typeof(GasSysIPartsSection)).Length;
        _hintPanel.SetSectionRange(0, maxSection, maxSection);
        _hintPanel.ShowHint(true);
        _controlPanel.ShowPanel(false);
        _hintPanel.nextBtn.gameObject.SetActive(false);
        _hintPanel.prevBtn.gameObject.SetActive(false);
        partListObj.SelectPart();
        partObj.SetActive(true);
        partListObj.SetBtn("저장용기", delegate
        {
            ShowOperationParts((int)GasSysIPartsSection.저장용기);
        });
        partListObj.SetBtn("기동용가스용기", delegate
        {
            ShowOperationParts((int)GasSysIPartsSection.기동용가스용기);
        });
        partListObj.SetBtn("솔레노이드밸브", delegate
        {
            ShowOperationParts((int)GasSysIPartsSection.솔레노이드밸브);
        });
        partListObj.SetBtn("선택밸브", delegate
        {
            ShowOperationParts((int)GasSysIPartsSection.선택밸브);
        });
        partListObj.SetBtn("압력스위치", delegate
        {
            ShowOperationParts((int)GasSysIPartsSection.압력스위치);
        });
        partListObj.SetBtn("방출표시등", delegate
        {
            ShowOperationParts((int)GasSysIPartsSection.방출표시등);
        });
        partListObj.SetBtn("수동조작함", delegate
        {
            ShowOperationParts((int)GasSysIPartsSection.수동조작함);
        });
        partListObj.SetBtn("감시제어반", delegate
        {
            ShowOperationParts((int)GasSysIPartsSection.감시제어반);
        });
        partListObj.SetBtn("오브젝트재정렬", delegate
        {
            ResetPart();
        });
        defaultPartsDict.Clear();
        foreach (var part in parts)
        {
            //part.SetActive(false);
            var data = new TransformData(part.transform.position, part.transform.rotation, part.transform.localScale);
            defaultPartsDict.Add(part, data);
        }
        //autoObj.SetActive(false);
        //manualObj.SetActive(true);


    }
    
    private void ShowOperationParts(int index)
    {
        // if (_.Equals(GasSysState.PracticeMode))
        // {
        //     prevBtn.gameObject.SetActive(true);
        //     GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(manualOperationHintPObj, (int)GasSysManualOperationPSection.감시제어반선택 + index));
        //     ChangeStateManualOperationP(GasSysManualOperationPSection.감시제어반선택 + index);
        // }
        // if (!_gasSysState.Equals(GasSysState.EvaluationMode))
        //     return;
        // prevBtn.gameObject.SetActive(false);
        // GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(manualOperationHintEObj, (int)GasSysSafetyCheckESection.Init + index));
        // ChangeStateManualOperationE(GasSysManualOperationESection.Init + index);
        //GasSysGlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(manualOperationHintPObj, (int)GasSysManualOperationPSection.감시제어반선택 + index));
        ChangeParts(GasSysIPartsSection.전체 + index);
    }

    private void ChangeParts(GasSysIPartsSection state)
    {
        _curParts = state;
        OnStateChangeParts(_curParts);
        
    }

    private void OnStateChangeParts(GasSysIPartsSection state)
    {
        OnBlendComplete?.RemoveAllListeners();
        OnAnimationComplete?.RemoveAllListeners();
        _disposable.Clear();
        _hintPanel.SetHintPopup(partsHintObj.hintData[(int)state]);
        mainCamera.cullingMask = LayerMask.GetMask("Default", "저장용기", "기동용가스용기", "솔레노이드밸브", "selectValve", "압력스위치", "방출표시등",
            "수동조작함", "감시제어반");
        ResetPart();
        switch (state)
        {
            case GasSysIPartsSection.전체:
                {
                    CheckCamIsBlending(_camList[(int)GasSysIPartsSection.전체].gameObject);
                }
                break;
            case GasSysIPartsSection.저장용기:
                {
                    CheckCamIsBlending(_camList[(int)GasSysIPartsSection.저장용기].gameObject, delegate
                    {
                        ShowPart(parts[(int)GasSysIPartsSection.저장용기]);
                        mainCamera.cullingMask = LayerMask.GetMask("저장용기");
                        //mouseControlObj.targetTag = "저장용기";
                        objectControl.targetTag = "저장용기";
                        
                    });
                }
                break;
            case GasSysIPartsSection.기동용가스용기:
                {
                    CheckCamIsBlending(_camList[(int)GasSysIPartsSection.기동용가스용기].gameObject, delegate
                    {
                        ShowPart(parts[(int)GasSysIPartsSection.기동용가스용기]);
                        mainCamera.cullingMask = LayerMask.GetMask("기동용가스용기");
                        objectControl.targetTag = "기동용가스용기";
                        
                    });
                }
                break;
            case GasSysIPartsSection.솔레노이드밸브:
                {
                    CheckCamIsBlending(_camList[(int)GasSysIPartsSection.솔레노이드밸브].gameObject, delegate
                    {
                        ShowPart(parts[(int)GasSysIPartsSection.솔레노이드밸브]);
                        mainCamera.cullingMask = LayerMask.GetMask("솔레노이드밸브");
                        objectControl.targetTag = "솔레노이드밸브";
                    });
                }
                break;
            case GasSysIPartsSection.선택밸브:
                {
                    CheckCamIsBlending(_camList[(int)GasSysIPartsSection.선택밸브].gameObject, delegate
                    {
                        ShowPart(parts[(int)GasSysIPartsSection.선택밸브]);
                        mainCamera.cullingMask = LayerMask.GetMask("selectValve");
                        objectControl.targetTag = "선택밸브";
                    });
                }
                break;
            case GasSysIPartsSection.압력스위치:
                {
                    CheckCamIsBlending(_camList[(int)GasSysIPartsSection.압력스위치].gameObject, delegate
                    {
                        ShowPart(parts[(int)GasSysIPartsSection.압력스위치]);
                        mainCamera.cullingMask = LayerMask.GetMask("압력스위치");
                        objectControl.targetTag = "압력스위치";
                    });
                }
                break;
            case GasSysIPartsSection.방출표시등:
                {
                    CheckCamIsBlending(_camList[(int)GasSysIPartsSection.방출표시등].gameObject, delegate
                    {
                        ShowPart(parts[(int)GasSysIPartsSection.방출표시등]);
                        mainCamera.cullingMask = LayerMask.GetMask("방출표시등");
                        objectControl.targetTag = "방출표시등";
                    });
                }
                break;
            case GasSysIPartsSection.수동조작함:
                {
                    CheckCamIsBlending(_camList[(int)GasSysIPartsSection.수동조작함].gameObject, delegate
                    {
                        ShowPart(parts[(int)GasSysIPartsSection.수동조작함]);
                        mainCamera.cullingMask = LayerMask.GetMask("수동조작함");
                        objectControl.targetTag = "수동조작함";
                    });
                }
                break;
            case GasSysIPartsSection.감시제어반:
                {
                    CheckCamIsBlending(_camList[(int)GasSysIPartsSection.감시제어반].gameObject, delegate
                    {
                        ShowPart(parts[(int)GasSysIPartsSection.감시제어반]);
                        mainCamera.cullingMask = LayerMask.GetMask("감시제어반");
                        objectControl.targetTag = "감시제어반";
                    });
                }
                break;
            case GasSysIPartsSection.방출헤드:
                {
                    CheckCamIsBlending(_camList[(int)GasSysIPartsSection.방출헤드].gameObject);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void SetPartsCamPos(GameObject obj)
    {
        foreach (var camPos in partsCamPosObjs)
        {
            camPos.gameObject.SetActive(camPos.gameObject.Equals(obj));
        }
        _curCamPos = obj;
    }

    public void ResetPart()
    {
        foreach (var part in parts)
        {
            part.transform.position = defaultPartsDict[part].position;
            part.transform.rotation =  defaultPartsDict[part].rotation;
            part.transform.localScale = defaultPartsDict[part].scale;
        }
    }

    public void ShowPart(GameObject obj)
    {
        return;
        foreach (var part in parts)
        {
            part.SetActive(false);
        }
        obj.SetActive(true);
    }
    
    [System.Serializable]
    public class TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
}
