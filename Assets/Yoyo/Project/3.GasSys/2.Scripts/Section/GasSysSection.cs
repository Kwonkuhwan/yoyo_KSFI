using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;
using Random = UnityEngine.Random;

public partial class GasSysSection : MonoBehaviour
{
    public Button[] buttons;
    public GameObject[] showObject;

    [Foldout("공용")]
    public AudioClip hintClip;
    public Inventory inventoryObj;
    public GasSysAreaManager areaManagerObj;



    public UIDragAndCollisionHandler uiDragAndCollisionHandler;
    public GasSysStorageRoom storageRoomObj;
    public GasSysArea1 area1Obj;
    public GasSysArea1Corridor area1CorridorObj;

    [SerializeField] public Button nextBtn;
    [SerializeField] public Image nextImg;
    [SerializeField] public TextMeshProUGUI nextText;

    [SerializeField] public Button prevBtn;
    [SerializeField] public Image prevImg;
    [SerializeField] public TextMeshProUGUI prevText;

    public int _curSection = 0;
    [HideInInspector] public int _minSection = 0;
    [HideInInspector] public int _maxSection = 0;
    [HideInInspector] public CompositeDisposable _disposable = new CompositeDisposable();

    [HideInInspector] public GasSysMainSection curMainSection;
    [HideInInspector] public GasSysSolenoidValveTestSection curSolenoidValveTestSection;

    private GasSysState _gasSysState = GasSysState.None;

    private List<SwitchButtonCheck> _switchButtons = new List<SwitchButtonCheck>();
    private SoundCheck _soundCheck = new SoundCheck();

    private GasSysControlPanelState _gasSysEvaluation = new GasSysControlPanelState();

    private ControlPanelButtonCheck _panelBtnCheck = new ControlPanelButtonCheck();

    [FormerlySerializedAs("_completePopup")]
    [SerializeField] private CompletePopup completePopup;
#region 저장실 오브젝트

    [Foldout("저장실")]
    [SerializeField] public SelectionValve selectionValve01;
    [SerializeField] public SelectionValve selectionValve02;
    [SerializeField] public StorageCylinder storageCylinder;

    [SerializeField] public SelectionValvePopup selectionValvePopup;
    [SerializeField] public StorageCylinderPopup storageCylinderPopup;
    [SerializeField] public ControlPanel controlPanelPopup;
    [SerializeField] public ActivationCylinderBoxPopup activationCylinderBoxPopup;
    [SerializeField] public SolenoidValvePopup solenoidValvePopup;

    [SerializeField] public Button selectionValveBtn;
    [SerializeField] public Button controlBoxBtn;
    [SerializeField] public Button activationBoxBtn;
    [SerializeField] public Button storageCylinderBtn;
    [SerializeField] public GameObject activationBoxOpenObj;
    [SerializeField] public GameObject activationBoxOpenObj2;

    [HideInInspector] public ControlMode _curControlMode;
    [HideInInspector] public ControlMode _curArea1Mode = ControlMode.Auto;
    //[EndFoldout]

#endregion

#region 1번구역 복도

    [Foldout("1번 방호구역 복도")]
    [SerializeField] public GameObject dischargeOnLight;
    [SerializeField] public GameObject boxDischargeOn;
    [SerializeField] public Button manualControlBoxBtn;

    [SerializeField] public ManualControlBoxPopup manualControlBoxPopup;

#endregion

#region 1번 구역

    [Foldout("1번 방호구역")]
    [SerializeField] public GameObject smokeDetectorObj;
    [SerializeField] public GameObject heatDetectorObj;

    [SerializeField] public GameObject smokeDetectorOn;
    [SerializeField] public GameObject heatDetectorOn;

    [SerializeField] public SmokeDetectorPopup smokeDetectorPopup;
    [SerializeField] public HeatDetectorPopup heatDetectorPopup;

#endregion


    // public void Init()
    // {
    //     gameObject.SetActive(true);
    //     foreach (var obj in showObject)
    //     {
    //         obj.SetActive(true);
    //     }
    //     ButtonManager.Instance.DisableAllButtons();
    //     ButtonManager.Instance.EnableSpecificButton(buttons);
    //     GlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio());
    // }

    // private void OnDisable()
    // {
    //     foreach (var obj in showObject)
    //     {
    //         obj.SetActive(false);
    //     }
    // }
    //
    // private void OnEnable()
    // {
    //
    // }

    private void Start()
    {
        
        //completePopup.gameObject.SetActive(false);
    }

    public void InitEMode()
    {
        점검전안전조치평가1.SetActive(false);
        점검전안전조치토글1.isOn = false;
        점검전안전조치토글2.isOn = false;
        점검전안전조치토글3.isOn = false;
        점검전안전조치토글4.isOn = false;
        점검전안전조치토글5.isOn = false;

        즉시격발평가1.SetActive(false);
        즉시격발토글1.isOn = false;
        즉시격발토글2.isOn = false;
        즉시격발토글3.isOn = false;
        즉시격발토글4.isOn = false;
        즉시격발토글5.isOn = false;
        즉시격발토글6.isOn = false;

        수동조작함작동평가1.SetActive(false);
        수동조작함작동토글1.isOn = false;
        수동조작함작동토글2.isOn = false;
        수동조작함작동토글3.isOn = false;
        수동조작함작동토글4.isOn = false;
        수동조작함작동토글5.isOn = false;
        수동조작함작동토글6.isOn = false;

        교차회로감지기평가1.SetActive(false);
        교차회로감지기토글1.isOn = false;
        교차회로감지기토글2.isOn = false;
        교차회로감지기토글3.isOn = false;
        교차회로감지기토글4.isOn = false;
        교차회로감지기토글5.isOn = false;
        교차회로감지기토글6.isOn = false;
        
        제어반수동조작평가1.SetActive(false);
        제어반수동조작토글1.isOn = false;
        제어반수동조작토글2.isOn = false;
        제어반수동조작토글3.isOn = false;
        제어반수동조작토글4.isOn = false;
        제어반수동조작토글5.isOn = false;
        제어반수동조작토글6.isOn = false;
        
        방출표시등평가1.SetActive(false);
        방출표시등토글1.isOn = false;
        방출표시등토글2.isOn = false;
        방출표시등토글3.isOn = false;
        
        ControlPanel.Instance.ClearReceiverLog();
    }

    private void InitStorageRoom()
    {
        //inventoryObj?.Init();
        selectionValveBtn.gameObject.SetActive(false);
        controlBoxBtn.gameObject.SetActive(false);
        activationBoxBtn.gameObject.SetActive(false);
        storageCylinderBtn.gameObject.SetActive(false);
        ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
        selectionValvePopup.gameObject.SetActive(false);
        storageCylinderPopup.gameObject.SetActive(false);
        activationCylinderBoxPopup.gameObject.SetActive(false);
        ControlPanel.Instance.ShowPanel(false);
        solenoidValvePopup.gameObject.SetActive(false);

        selectionValveBtn.onClick.RemoveAllListeners();
        controlBoxBtn.onClick.RemoveAllListeners();
        activationBoxBtn.onClick.RemoveAllListeners();
        storageCylinderBtn.onClick.RemoveAllListeners();
        solenoidValvePopup.Init();
        activationCylinderBoxPopup.Init();
    }


    private void InitArea1Corridor()
    {
        dischargeOnLight.SetActive(false);
        boxDischargeOn.SetActive(false);
        manualControlBoxPopup.Init();
        manualControlBoxBtn.onClick.RemoveAllListeners();
        manualControlBoxBtn.gameObject.SetActive(false);
        manualControlBoxPopup.gameObject.SetActive(false);

    }

    private void InitArea1()
    {
        smokeDetectorOn.SetActive(false);
        heatDetectorOn.SetActive(false);
        smokeDetectorPopup.gameObject.SetActive(false);
        heatDetectorPopup.gameObject.SetActive(false);
    }

    private HintTextAndAudio GetHintTextAndAudio(HintScriptableObj hintObj, int index)
    {
        return new HintTextAndAudio()
        {
            title = hintObj.hintData[index].title,
            text = hintObj.hintData[index].text,
            audioClip = hintObj.hintData[index].audioClip
        };
    }


    public void NextDisable()
    {
        //ButtonManager.Instance.NextDisable();
        nextBtn.interactable = false;
        ColorBlock colors = nextBtn.colors;
        //colors.disabledColor = colors.normalColor;
        nextImg.color = colors.disabledColor;
        nextText.color = colors.disabledColor;
    }

    public void NextEnable(bool isCompleteHint = true)
    {
        //ButtonManager.Instance.NextEnable();
        nextBtn.interactable = true;
        ColorBlock colors = nextBtn.colors;
        //colors.disabledColor = colors.normalColor;
        nextImg.color = colors.normalColor;
        nextText.color = colors.normalColor;
        if (isCompleteHint)
            GasSysGlobalCanvas.Instance.SetCompleteHint();
        ButtonManager.Instance.HighlightButton(nextBtn);
    }


    private void ShowRoom(GameObject obj)
    {
        storageRoomObj.gameObject.SetActive(storageRoomObj.gameObject.Equals(obj));
        area1Obj.gameObject.SetActive(area1Obj.gameObject.Equals(obj));
        area1CorridorObj.gameObject.SetActive(area1CorridorObj.gameObject.Equals(obj));


        if (storageRoomObj.gameObject.activeSelf)
        {
            areaManagerObj.StartArea(GasSysAreaManager.StartAreaType.StorageRoom);
        }
        if (area1Obj.gameObject.activeSelf)
        {
            areaManagerObj.StartArea(GasSysAreaManager.StartAreaType.Area1);
        }
        if (area1CorridorObj.gameObject.activeSelf)
        {
            areaManagerObj.StartArea(GasSysAreaManager.StartAreaType.Area1Corridor);
        }
    }

    public void Prev()
    {
        if (_curSection <= 0)
            return;
        _curSection--;
        ShowSection(_curSection);
        UpdateBtn();
    }

    public void Next()
    {
        if (_curSection >= _maxSection)
            return;
        _curSection++;
        ShowSection(_curSection);
        UpdateBtn();
    }

    private void ShowSection(int index)
    {
        switch (curMainSection)
        {

            case GasSysMainSection.Init:
                break;
            case GasSysMainSection.SafetyCheck:
                //ChangeStateSafetyCheck(GasSysSafetyCheckSection.선택밸브조작동관선택 + index);
                //GlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(safetyCheckHintObj, (int)GasSysSafetyCheckSection.선택밸브조작동관선택 + index));
                ShowSafetyCheckSection(index);
                break;
            case GasSysMainSection.SolenoidValveTest:
                ShowSolenoidValveTestSection(index);
                break;
            case GasSysMainSection.DischargeLightTest:
                ShowDischargeLightTestSection(index);
                break;
            case GasSysMainSection.RecoveryCheck:
                ShowRecoverySection(index);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        //safetySections[index].Init();
    }

    private void ShowSolenoidValveTestSection(int index)
    {
        switch (curSolenoidValveTestSection)
        {
            case GasSysSolenoidValveTestSection.ManualOperation:
                ShowManualOperationSection(index);
                break;
            case GasSysSolenoidValveTestSection.ManualControlBox:
                ShowManualControlBoxSection(index);
                break;
            case GasSysSolenoidValveTestSection.CrossCircuitDetector:
                ShowCrossCircuitDetectorSection(index);
                break;
            case GasSysSolenoidValveTestSection.ControlPanelSwitch:
                ShowControlPanelSwitchSection(index);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    public void SetSectionRange(int minIndex, int maxIndex, int value)
    {
        _minSection = Mathf.Clamp(minIndex, 0, value - 1); // 최소 인덱스가 범위를 벗어나지 않도록 제한
        _maxSection = Mathf.Clamp(maxIndex, 0, value - 1); // 최대 인덱스도 제한
        _curSection = _minSection; // 범위 내에서 처음 페이지로 이동
        UpdateBtn();
        ShowSection(_curSection);
    }

    private void UpdateBtn()
    {
        prevBtn.interactable = _curSection > _minSection; // 첫 번째 페이지일 때 이전 버튼 비활성화
        //nextBtn.interactable = _curSection < _maxSection; // 마지막 페이지일 때 다음 버튼 비활성화


        if (!prevBtn.interactable)
        {
            ColorBlock colors = prevBtn.colors;
            //colors.disabledColor = colors.normalColor;
            prevImg.color = colors.disabledColor;
            prevText.color = colors.disabledColor;
        }
        else
        {
            ColorBlock colors = prevBtn.colors;
            //colors.disabledColor = colors.normalColor;
            prevImg.color = colors.normalColor;
            prevText.color = colors.normalColor;
        }

        // if (_minSection.Equals(_maxSection))
        // {
        //     prevBtn.gameObject.SetActive(false);
        //     nextBtn.gameObject.SetActive(false);
        // }
        // else
        // {
        //     prevBtn.gameObject.SetActive(true);
        //     nextBtn.gameObject.SetActive(true);
        // }
    }

    private void ChangeValveState(bool isOn)
    {
        selectionValve01.ChangeValveState(isOn);
        selectionValve02.ChangeValveState(isOn);
        selectionValvePopup.SetOnOff(isOn);
        Debug.Log($"선택밸브 {isOn}");
    }

    private void ChangeStorageValveState(bool isOn)
    {
        storageCylinder.ChangeValveState(isOn);
        storageCylinderPopup.SetOnOff(isOn);
    }

    private bool GetValveState()
    {
        return selectionValvePopup.GetIsOnOff();
    }

    private bool GetStorageState()
    {
        return storageCylinderPopup.GetIsOnOff();
    }

    private void OpenActivationBox(bool isOpen, bool isOpen2 = false)
    {
        activationBoxOpenObj.SetActive(isOpen);
        activationBoxOpenObj2.SetActive(isOpen2);

    }

    private void SetCompletePopup(string text, string nextStr = "", Action action = null)
    {
        completePopup.SetPrevStepBtn(delegate
        {
            //SoundManager.Instance.PlayAllFireSound(_soundCheck);
            SoundManager.Instance.SetDefaultVolume();
            Prev();
            completePopup.ShowCompletePopup(false);
        });
        SoundManager.Instance.ZeroVolume();
        SoundManager.Instance.StopHint();
        completePopup.SetCompleteText(text, nextStr, action);
        completePopup.ShowCompletePopup(true);
        //ButtonManager.Instance.EnableSpecificButton(completePopup.GetButtons());
    }

    public void SetGasSysState(GasSysState state)
    {
        _gasSysState = state;
    }

    public void SetHighlightControlPanel(ControlPanelButtonCheck state, bool live = false)
    {
        _switchButtons.Clear();
        ControlPanel.Instance.CheckSwitchBtn();
        var switchBtn = ControlPanel.Instance.controlPanelButton;
        ControlPanel.Instance.SoundCheck();
        // ControlPanel.Instance.ShowPanel(true);
        // ControlPanel.Instance.GetSwitchButton("주경종").OnCheck(false);
        // ControlPanel.Instance.GetSwitchButton("지구경종").OnCheck(false);
        // ControlPanel.Instance.GetSwitchButton("사이렌").OnCheck(false);
        // ControlPanel.Instance.GetSwitchButton("비상방송").OnCheck(false);
        // ControlPanel.Instance.GetSwitchButton("부저").OnCheck(false);
        // ControlPanel.Instance.GetSwitchButton("축적/비축적").OnCheck(false);
        // _switchButtons.Add(new SwitchButtonCheck()
        // {
        //     btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
        //     select = switchBtn.주경종 == state.주경종
        // });
        // _switchButtons.Add(new SwitchButtonCheck()
        // {
        //     btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
        //     select = !state.지구경종
        // });
        // _switchButtons.Add(new SwitchButtonCheck()
        // {
        //     btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
        //     select = !state.사이렌
        // });
        // _switchButtons.Add(new SwitchButtonCheck()
        // {
        //     btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
        //     select = !state.비상방송
        // });
        // _switchButtons.Add(new SwitchButtonCheck()
        // {
        //     btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
        //     select = !state.부저
        // });
        // _switchButtons.Add(new SwitchButtonCheck()
        // {
        //     btn = ControlPanel.Instance.GetSwitchButton("축적/비축적").GetButton(),
        //     select = !state.축적
        // });
        // _switchButtons.Add(new SwitchButtonCheck()
        // {
        //     btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
        //     select = !state.복구
        // });
        //
        // _switchButtons.Add(new SwitchButtonCheck()
        // {
        //     btn = ControlPanel.Instance.GetSolenoidSelectBtn(),
        //     select = state.솔레노이드밸브.Equals(ControlPanel.Instance.controlPanelButton.솔레노이드밸브)
        // });
        //
        // _switchButtons.Add(new SwitchButtonCheck()
        // {
        //     btn = ControlPanel.Instance.GetArea1SelectBtn(),
        //     select = state.구역1.Equals(ControlPanel.Instance.controlPanelButton.구역1)
        // });
        //
        // _switchButtons.Add(new SwitchButtonCheck()
        // {
        //     btn = ControlPanel.Instance.GetArea2SelectBtn(),
        //     select = state.구역2.Equals(ControlPanel.Instance.controlPanelButton.구역2)
        // });
        // _switchButtons.Add(new SwitchButtonCheck()
        // {
        //     btn = ControlPanel.Instance.GetArea1ActivateBtn(),
        //     select = !state.구역1기동
        // });
        //
        // _switchButtons.Add(new SwitchButtonCheck()
        // {
        //     btn = GasSysGlobalCanvas.Instance.GetCheckAgreeBtn(),
        //     select = false
        // });
        if (live)
        {
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
                select = switchBtn.주경종 == state.주경종
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
                select = state.지구경종 == switchBtn.지구경종
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
                select = state.사이렌 == switchBtn.사이렌
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
                select = state.비상방송 == switchBtn.비상방송
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
                select = state.부저 == switchBtn.부저
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("축적/비축적").GetButton(),
                select = state.축적 == switchBtn.축적
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
                select = state.복구 == switchBtn.복구
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSolenoidSelectBtn(),
                select = switchBtn.솔레노이드밸브.Equals(state.솔레노이드밸브)
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetArea1SelectBtn(),
                select = switchBtn.구역1.Equals(state.구역1)
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetArea2SelectBtn(),
                select = switchBtn.구역2.Equals(state.구역2)
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetArea1ActivateBtn(),
                select = !switchBtn.구역1.Equals(ControlMode.Manual) || switchBtn.구역1기동 == state.구역1기동
            });
        }
        else
        {
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
                select = !state.주경종
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
                select = !state.지구경종
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
                select = !state.사이렌
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
                select = !state.비상방송
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
                select = !state.부저
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("축적/비축적").GetButton(),
                select = !state.축적
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
                select = !state.복구
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSolenoidSelectBtn(),
                select = state.솔레노이드밸브.Equals(ControlPanel.Instance.controlPanelButton.솔레노이드밸브)
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetArea1SelectBtn(),
                select = state.구역1.Equals(ControlPanel.Instance.controlPanelButton.구역1)
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetArea2SelectBtn(),
                select = state.구역2.Equals(ControlPanel.Instance.controlPanelButton.구역2)
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetArea1ActivateBtn(),
                select = !state.구역1기동
            });

        }
        _switchButtons.Add(new SwitchButtonCheck()
        {
            btn = GasSysGlobalCanvas.Instance.GetCheckAgreeBtn(),
            select = false
        });
        if (state.주경종 == switchBtn.주경종 && state.지구경종 == switchBtn.지구경종 &&
            state.사이렌 == switchBtn.사이렌 && state.비상방송 == switchBtn.비상방송 &&
            state.부저 == switchBtn.부저 && state.축적 == switchBtn.축적 && state.복구 == switchBtn.복구 &&
            switchBtn.구역1.Equals(state.구역1) && switchBtn.솔레노이드밸브.Equals(state.솔레노이드밸브) &&
            switchBtn.구역2.Equals(state.구역2))
        {
            NextEnable(false);
        }
        else
        {
            NextDisable();
        }
        ButtonManager.Instance.EnableSpecificButton(_switchButtons);
        //NextDisable();

    }

    public void SetHighlightControlPanelCheck(ControlPanelButtonCheck state, bool isCheck = true)
    {
        ControlPanel.ControlPanelButtonAction.RemoveAllListeners();
        ControlPanel.ControlPanelButtonAction.AddListener(switchBtn =>
        {
            _switchButtons.Clear();
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("주경종").GetButton(),
                select = state.주경종 == switchBtn.주경종
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("지구경종").GetButton(),
                select = state.지구경종 == switchBtn.지구경종
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("사이렌").GetButton(),
                select = state.사이렌 == switchBtn.사이렌
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("비상방송").GetButton(),
                select = state.비상방송 == switchBtn.비상방송
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("부저").GetButton(),
                select = state.부저 == switchBtn.부저
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("축적/비축적").GetButton(),
                select = state.축적 == switchBtn.축적
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSwitchButton("복구").GetButton(),
                select = state.복구 == switchBtn.복구
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetSolenoidSelectBtn(),
                select = switchBtn.솔레노이드밸브.Equals(state.솔레노이드밸브)
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetArea1SelectBtn(),
                select = switchBtn.구역1.Equals(state.구역1)
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetArea2SelectBtn(),
                select = switchBtn.구역2.Equals(state.구역2)
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = ControlPanel.Instance.GetArea1ActivateBtn(),
                select = !switchBtn.구역1.Equals(ControlMode.Manual) || switchBtn.구역1기동 == state.구역1기동
            });
            if (!isCheck)
            {
                _switchButtons.Add(new SwitchButtonCheck()
                {
                    btn = GasSysGlobalCanvas.Instance.GetCheckAgreeBtn(),
                    select = false
                });
            }
            ButtonManager.Instance.EnableSpecificButton(_switchButtons);

            if (state.주경종 == switchBtn.주경종 && state.지구경종 == switchBtn.지구경종 &&
                state.사이렌 == switchBtn.사이렌 && state.비상방송 == switchBtn.비상방송 &&
                state.부저 == switchBtn.부저 && state.축적 == switchBtn.축적 && state.복구 == switchBtn.복구 &&
                switchBtn.구역1.Equals(state.구역1) && switchBtn.솔레노이드밸브.Equals(state.솔레노이드밸브) &&
                switchBtn.구역2.Equals(state.구역2))
            {
                if (isCheck)
                {
                    NextEnable();
                }
                else
                {
                    NextEnable(false);
                }
            }
            else
            {
                if (isCheck)
                {
                    NextDisable();
                }
            }
        });
    }

    public bool CheckDetector1()
    {
        return ControlPanel.Instance.CheckDetector1();
    }

    public bool CheckDetector2()
    {
        return ControlPanel.Instance.CheckDetector2();
    }

    public bool CheckVerifyDischarge()
    {
        return ControlPanel.Instance.CheckVerifyDischarge();
    }

}
