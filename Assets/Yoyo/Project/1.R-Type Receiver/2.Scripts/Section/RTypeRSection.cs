using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public partial class RTypeRSection : MonoBehaviour
{

    //[SerializeField] private HintPanel hintPanel;
    [Foldout("공용")]
    [HideInInspector] public RTypeRMainSection curMainSection;
    [FormerlySerializedAs("_rTypeRState")]
    public RTypeRState rTypeRState = RTypeRState.None;
    public Inventory inventoryObj;

    public RTypeRAreaManager areaManagerObj;
    public UIDragAndCollisionHandler uIDragAndCollisionHandler;
    public GameObject fireDoorRoomObj;
    public GameObject rTypeRoomObj;
    public GameObject area2_1RoomObj;
    public GameObject area3_2RoomObj;
    public GameObject menuPanel;

    [Foldout("방화문")]
    [SerializeField] public GameObject fireDoorObj;
    [SerializeField] public GameObject smokeDetector;
    [SerializeField] public GameObject smokeDetectorOn;
    [SerializeField] public Button fireDoorBtn;
    [SerializeField] public RTypeRSmokeDetectorPopup smokeDetectorPopup;

    [FormerlySerializedAs("smokeDetectorPopup2")]
    [Foldout("구역2_1")]
    [SerializeField] public SmokeDetectorPopup area2_1SmokeDetectorPopup1;
    [FormerlySerializedAs("area2_1SmokeDetectorPopup")]
    [SerializeField] public RTypeRSmokeDetectorPopup area2_1SmokeDetectorPopup2;
    [SerializeField] public GameObject area2_1SmokeDetector;
    [SerializeField] public GameObject area2_1SmokeDetectorOn;
    [SerializeField] public RTypeRSmokeDetectorTestPopup smokeTestPopup;

    [Foldout("구역3_2")]
    [SerializeField] public Button firePlugBtn;
    [SerializeField] public RTypeRFirePlugTestPopup firePlugTestPopup;

    [Foldout("힌트")]
    [SerializeField] public GameObject hintObj;
    [SerializeField] public TextMeshProUGUI titleText;
    [SerializeField] public TextMeshProUGUI nextText;
    [SerializeField] public TextMeshProUGUI prevText;
    [SerializeField] public TextMeshProUGUI hintText;
    [SerializeField] public Button nextBtn;
    [SerializeField] public Image nextImg;
    [SerializeField] private Sprite[] hintSoundOnOff;
    [SerializeField] private Image hintSoundBtnImage;
    [SerializeField] private Button hintSoundBtn;

    [SerializeField] public Button prevBtn;
    [SerializeField] public Image prevImg;

    public int _curSection = 0;
    [HideInInspector] public int _minSection = 0;
    [HideInInspector] public int _maxSection = 0;

    private RTypeRPanelButtonCheck _panelBtnCheck = new RTypeRPanelButtonCheck();

    private List<SwitchButtonCheck> _switchButtons = new List<SwitchButtonCheck>();

    private RTypeRPanel _rPanel;
    private ButtonManager _btnManager;
    private SoundManager _soundManager;
    private RTypeRGlobalCanvas _gCanvas;

    private void Start()
    {
        menuPanel.SetActive(true);
    }
    private void Init()
    {
        if (!hintSoundBtn)
            return;
        hintSoundBtn.onClick.RemoveAllListeners();
        hintSoundBtnImage.sprite = SoundManager.Instance.hintSource.mute ? hintSoundOnOff[1] : hintSoundOnOff[0];
        hintSoundBtn.onClick.AddListener(delegate
        {
            if (SoundManager.Instance.hintSource.mute)
            {
                SoundManager.Instance.hintSource.mute = false;
                hintSoundBtnImage.sprite = hintSoundOnOff[0];
            }
            else
            {
                SoundManager.Instance.hintSource.mute = true;
                hintSoundBtnImage.sprite = hintSoundOnOff[1];
            }
        });
        _rPanel = RTypeRPanel.Instance;
        _btnManager = ButtonManager.Instance;
        _soundManager = SoundManager.Instance;
        _gCanvas = RTypeRGlobalCanvas.Instance;
        area2_1SmokeDetectorPopup1.gameObject.SetActive(false);
        area2_1SmokeDetectorPopup2.gameObject.SetActive(false);
        //area2_1SmokeDetector.gameObject.SetActive(false);
        area2_1SmokeDetectorOn.SetActive(false);
        smokeTestPopup.gameObject.SetActive(false);
        firePlugTestPopup.gameObject.SetActive(false);
        
        nextBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.AddListener(Next);
        prevBtn.onClick.AddListener(Prev);
        inventoryObj?.Init();
        inventoryObj?.ShowPanel(true);
        ShowHint(true);
        _gCanvas.ShowCompletePopup(false);
        _gCanvas.HideCheckObj();
        uIDragAndCollisionHandler.ResetEvent();
        _soundManager.SetDefaultVolume();
        RTypeRPanel.RTypeRPanelButtonAction.RemoveAllListeners();
        _rPanel.GetSwitchButton("수신기복구").GetButton().onClick.RemoveListener(DefaultFireMap);
        RTypeRPanel.OnCheckDate.RemoveAllListeners();
    }

    // Start is called before the first frame update

    private void InitRTypeRRoom()
    {

    }


    private void InitFireDoor()
    {

    }

    private void InitArea()
    {

    }

    private void ShowRoom(RTypeRRoomType type)
    {
        switch (type)
        {

            case RTypeRRoomType.RTypeR:
                ShowRoom(rTypeRoomObj);
                break;
            case RTypeRRoomType.FireDoor:
                ShowRoom(fireDoorRoomObj);
                break;
            case RTypeRRoomType.Area2_1:
                ShowRoom(area2_1RoomObj);
                break;
            case RTypeRRoomType.Area3_2:
                ShowRoom(area3_2RoomObj);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private void ShowRoom(GameObject obj)
    {
        rTypeRoomObj.SetActive(rTypeRoomObj.gameObject.Equals(obj));
        fireDoorRoomObj.gameObject.SetActive(fireDoorRoomObj.gameObject.Equals(obj));
        area2_1RoomObj.gameObject.SetActive(area2_1RoomObj.gameObject.Equals(obj));
        area3_2RoomObj.gameObject.SetActive(area3_2RoomObj.gameObject.Equals(obj));


        if (rTypeRoomObj.gameObject.activeSelf)
        {
            areaManagerObj.StartArea(RTypeRRoomType.RTypeR);
        }
        if (fireDoorRoomObj.gameObject.activeSelf)
        {
            areaManagerObj.StartArea(RTypeRRoomType.FireDoor);
        }
        if (area2_1RoomObj.gameObject.activeSelf)
        {
            areaManagerObj.StartArea(RTypeRRoomType.Area2_1);
        }
        if (area3_2RoomObj.gameObject.activeSelf)
        {
            areaManagerObj.StartArea(RTypeRRoomType.Area3_2);
        }
    }

    public void SetRTypeRState(RTypeRState state)
    {
        rTypeRState = state;
    }

    private void SetSection(int index)
    {

        switch (curMainSection)
        {

            case RTypeRMainSection.Init:
                break;
            case RTypeRMainSection.EquipmentOperation:
                //ChangeStateSafetyCheck(GasSysSafetyCheckSection.선택밸브조작동관선택 + index);
                //GlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(safetyCheckHintObj, (int)GasSysSafetyCheckSection.선택밸브조작동관선택 + index));
                SetEquipmentOperationSection(index);
                break;
            case RTypeRMainSection.FireAlarmSystem:
                //ShowSolenoidValveTestSection(index);
                SetFireAlarmSystemSection(index);
                break;
            case RTypeRMainSection.CircuitBreaker:
                //ShowDischargeLightTestSection(index);
                SetCircuitBreakerSection(index);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        //safetySections[index].Init();
    }



#region 힌트

    public void ShowHint(bool isShow)
    {
        hintObj.SetActive(isShow);
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

    public void NextEnable(bool isHighlight = true, bool isCompleteHint = true)
    {
        //ButtonManager.Instance.NextEnable();
        nextBtn.interactable = true;
        ColorBlock colors = nextBtn.colors;
        //colors.disabledColor = colors.normalColor;
        nextImg.color = colors.normalColor;
        nextText.color = colors.normalColor;
        if (isCompleteHint)
            RTypeRGlobalCanvas.Instance.SetCompleteHint();
        if (isHighlight)
        {
            ButtonManager.Instance.HighlightButton(nextBtn);
        }
    }

    public void SetSectionRange(int minIndex, int maxIndex, int value)
    {
        _minSection = Mathf.Clamp(minIndex, 0, value - 1); // 최소 인덱스가 범위를 벗어나지 않도록 제한
        _maxSection = Mathf.Clamp(maxIndex, 0, value - 1); // 최대 인덱스도 제한
        _curSection = _minSection; // 범위 내에서 처음 페이지로 이동
        UpdateBtn();
        SetSection(_curSection);
    }



    public void Prev()
    {
        if (_curSection <= 0)
            return;
        _curSection--;
        SetSection(_curSection);
        UpdateBtn();
    }

    public void Next()
    {
        if (_curSection >= _maxSection)
            return;
        _curSection++;
        SetSection(_curSection);
        UpdateBtn();
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

    public void SetHint(HintTextAndAudio obj)
    {
        //_hits.Clear();
        // prevBtn.onClick.RemoveAllListeners();
        // nextBtn.onClick.RemoveAllListeners();
        //
        // prevBtn.onClick.AddListener(Prev);
        // nextBtn.onClick.AddListener(Next);

        titleText.text = obj.title;
        hintText.text = obj.text;

        // 2024-11-18 이재성
        // 힌트 사운드 음소거
        SoundManager.Instance.PlayHint(obj.audioClip);

        //UpdateBtn();
    }

    public void SetCompleteHint()
    {
        if (string.IsNullOrEmpty(hintText.text))
            return;
        if (!hintText.text.Contains("(완료)"))
        {
            hintText.text = $"{hintText.text} <color=black><b>(완료)</b></color>";
        }
    }

#endregion //힌트

    private void SetCompletePopup(string text, string nextStr = "", Action action = null)
    {
        _gCanvas.SetStepBtn(delegate
        {
            //SoundManager.Instance.PlayAllFireSound(_soundCheck);
            _soundManager.SetDefaultVolume();
            Prev();
            _gCanvas.ShowCompletePopup(false);
        });
        _soundManager.ZeroVolume();
        _gCanvas.SetCompleteText(text, nextStr, action);
        _gCanvas.ShowCompletePopup(true);
        //_btnManager.EnableSpecificButton(_gCanvas.GetCompletePopupButtons());
    }


    public void OpenFireDoor(bool isOpen)
    {
        fireDoorObj.SetActive(isOpen);
    }

    public GameObject SetFDSmokeDetector(bool isOn)
    {
        smokeDetectorOn.SetActive(isOn);
        return smokeDetectorOn;
    }

    public Button GetFireDoorBtn(Action action = null)
    {
        if (null == action)
            return fireDoorBtn;
        fireDoorBtn.onClick.RemoveAllListeners();
        fireDoorBtn.onClick.AddListener(action.Invoke);
        return fireDoorBtn;
    }

    public Button GetFDSmokeDetector(Action action = null)
    {
        var btn = smokeDetector.GetComponent<Button>();
        if (null == action)
            return btn;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action.Invoke);
        return btn;
    }

    public GameObject SetArea2_1SmokeDetector(bool isOn)
    {
        area2_1SmokeDetectorOn.SetActive(isOn);
        return area2_1SmokeDetectorOn;
    }

    public Button GetArea2_1SmokeDetector(Action action = null)
    {
        var btn = area2_1SmokeDetector.GetComponent<Button>();
        if (null == action)
            return btn;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action.Invoke);
        return btn;
    }

    public Button GetFirePlugBtn(Action action = null)
    {
        if (null == action)
            return firePlugBtn;
        firePlugBtn.onClick.RemoveAllListeners();
        firePlugBtn.onClick.AddListener(action.Invoke);
        return firePlugBtn;
    }

#region 버튼 하이라이트 로직

    public void SetHighlightControlPanel(RTypeRPanelButtonCheck state, bool live = false)
    {
        _switchButtons.Clear();
        RTypeRPanel.Instance.CheckSwitchBtn();
        var switchBtn = RTypeRPanel.Instance.rTypeRPanelButton;
        //RTypeRPanel.Instance.SoundCheck();
        if (live)
        {
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("예비전원시험").GetButton(),
                select = switchBtn.checkDic[RTypeRBtnType.예비전원시험] == state.checkDic[RTypeRBtnType.예비전원시험]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("자동복구").GetButton(),
                select = state.checkDic[RTypeRBtnType.자동복구] == switchBtn.checkDic[RTypeRBtnType.자동복구]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("축적소등_비축적점등").GetButton(),
                select = state.checkDic[RTypeRBtnType.축적소등_비축적점등] == switchBtn.checkDic[RTypeRBtnType.축적소등_비축적점등]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("수신기복구").GetButton(),
                select = state.checkDic[RTypeRBtnType.수신기복구] == switchBtn.checkDic[RTypeRBtnType.수신기복구]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("방화문정지").GetButton(),
                select = state.checkDic[RTypeRBtnType.방화문정지] == switchBtn.checkDic[RTypeRBtnType.방화문정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("주음향정지").GetButton(),
                select = state.checkDic[RTypeRBtnType.주음향정지] == switchBtn.checkDic[RTypeRBtnType.주음향정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("지구음향정지").GetButton(),
                select = state.checkDic[RTypeRBtnType.지구음향정지] == switchBtn.checkDic[RTypeRBtnType.지구음향정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("사이렌정지").GetButton(),
                select = state.checkDic[RTypeRBtnType.사이렌정지] == switchBtn.checkDic[RTypeRBtnType.사이렌정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("비상방송정지").GetButton(),
                select = state.checkDic[RTypeRBtnType.비상방송정지] == switchBtn.checkDic[RTypeRBtnType.비상방송정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("부저정지").GetButton(),
                select = state.checkDic[RTypeRBtnType.부저정지] == switchBtn.checkDic[RTypeRBtnType.부저정지]
            });

            switch (state.mainPump)
            {

                case ControlMode.Auto:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpAutoBtn(),
                            select = state.checkDic[RTypeRBtnType.메인펌프오토] == switchBtn.checkDic[RTypeRBtnType.메인펌프오토]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpStopBtn(),
                            select = true
                        });
                    }
                    break;
                case ControlMode.Stop:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpStopBtn(),
                            select = state.checkDic[RTypeRBtnType.메인펌프정지] == switchBtn.checkDic[RTypeRBtnType.메인펌프정지]
                        });
                    }
                    break;
                case ControlMode.Manual:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpManualBtn(),
                            select = state.checkDic[RTypeRBtnType.메인펌프수동] == switchBtn.checkDic[RTypeRBtnType.메인펌프수동]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpStopBtn(),
                            select = true
                        });
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }



            switch (state.jockeyPump)
            {

                case ControlMode.Auto:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpAutoBtn(),
                            select = state.checkDic[RTypeRBtnType.충압펌프오토] == switchBtn.checkDic[RTypeRBtnType.충압펌프오토]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpStopBtn(),
                            select = true
                        });
                    }
                    break;
                case ControlMode.Stop:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpStopBtn(),
                            select = state.checkDic[RTypeRBtnType.충압펌프정지] == switchBtn.checkDic[RTypeRBtnType.충압펌프정지]
                        });
                    }
                    break;
                case ControlMode.Manual:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpManualBtn(),
                            select = state.checkDic[RTypeRBtnType.충압펌프수동] == switchBtn.checkDic[RTypeRBtnType.충압펌프수동]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpStopBtn(),
                            select = true
                        });
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            switch (state.spMainPump)
            {

                case ControlMode.Auto:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpAutoBtn(),
                            select = state.checkDic[RTypeRBtnType.sp펌프오토] == switchBtn.checkDic[RTypeRBtnType.sp펌프오토]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpStopBtn(),
                            select = true
                        });

                    }
                    break;
                case ControlMode.Stop:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpStopBtn(),
                            select = state.checkDic[RTypeRBtnType.sp펌프정지] == switchBtn.checkDic[RTypeRBtnType.sp펌프정지]
                        });

                    }
                    break;
                case ControlMode.Manual:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpManualBtn(),
                            select = state.checkDic[RTypeRBtnType.sp펌프수동] == switchBtn.checkDic[RTypeRBtnType.sp펌프수동]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpStopBtn(),
                            select = true
                        });

                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (state.spJockeyPump)
            {

                case ControlMode.Auto:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpAutoBtn(),
                            select = state.checkDic[RTypeRBtnType.sp충압펌프오토] == switchBtn.checkDic[RTypeRBtnType.sp충압펌프오토]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpStopBtn(),
                            select = true
                        });

                    }
                    break;
                case ControlMode.Stop:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpStopBtn(),
                            select = state.checkDic[RTypeRBtnType.sp충압펌프정지] == switchBtn.checkDic[RTypeRBtnType.sp충압펌프정지]
                        });

                    }
                    break;
                case ControlMode.Manual:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpManualBtn(),
                            select = state.checkDic[RTypeRBtnType.sp충압펌프수동] == switchBtn.checkDic[RTypeRBtnType.sp충압펌프수동]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpStopBtn(),
                            select = true
                        });

                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        else
        {
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("예비전원시험").GetButton(),
                select = !state.checkDic[RTypeRBtnType.예비전원시험]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("자동복구").GetButton(),
                select = !state.checkDic[RTypeRBtnType.자동복구]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("축적소등_비축적점등").GetButton(),
                select = !state.checkDic[RTypeRBtnType.축적소등_비축적점등]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("수신기복구").GetButton(),
                select = !state.checkDic[RTypeRBtnType.수신기복구]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("방화문정지").GetButton(),
                select = !state.checkDic[RTypeRBtnType.방화문정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("주음향정지").GetButton(),
                select = !state.checkDic[RTypeRBtnType.주음향정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("지구음향정지").GetButton(),
                select = !state.checkDic[RTypeRBtnType.지구음향정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("사이렌정지").GetButton(),
                select = !state.checkDic[RTypeRBtnType.사이렌정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("비상방송정지").GetButton(),
                select = !state.checkDic[RTypeRBtnType.비상방송정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("부저정지").GetButton(),
                select = !state.checkDic[RTypeRBtnType.부저정지]
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetMainPumpAutoBtn(),
                select = !state.checkDic[RTypeRBtnType.메인펌프오토]
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetMainPumpManualBtn(),
                select = !state.checkDic[RTypeRBtnType.메인펌프수동]
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetMainPumpStopBtn(),
                select = !state.checkDic[RTypeRBtnType.메인펌프정지]
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetJockeyPumpAutoBtn(),
                select = !state.checkDic[RTypeRBtnType.충압펌프오토]
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetJockeyPumpManualBtn(),
                select = !state.checkDic[RTypeRBtnType.충압펌프수동]
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetJockeyPumpStopBtn(),
                select = !state.checkDic[RTypeRBtnType.충압펌프정지]
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSpPumpAutoBtn(),
                select = !state.checkDic[RTypeRBtnType.sp펌프오토]
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSpPumpManualBtn(),
                select = !state.checkDic[RTypeRBtnType.sp펌프수동]
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSpPumpStopBtn(),
                select = !state.checkDic[RTypeRBtnType.sp펌프정지]
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSpJockeyPumpAutoBtn(),
                select = !state.checkDic[RTypeRBtnType.sp충압펌프오토]
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSpJockeyPumpManualBtn(),
                select = !state.checkDic[RTypeRBtnType.sp충압펌프수동]
            });

            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSpJockeyPumpStopBtn(),
                select = !state.checkDic[RTypeRBtnType.sp충압펌프정지]
            });

        }
        _switchButtons.Add(new SwitchButtonCheck()
        {
            btn = RTypeRGlobalCanvas.Instance.GetCheckAgreeBtn(),
            select = false
        });
        if (state.checkDic[RTypeRBtnType.예비전원시험] == switchBtn.checkDic[RTypeRBtnType.예비전원시험] && state.checkDic[RTypeRBtnType.자동복구] == switchBtn.checkDic[RTypeRBtnType.자동복구] &&
            state.checkDic[RTypeRBtnType.축적소등_비축적점등] == switchBtn.checkDic[RTypeRBtnType.축적소등_비축적점등] && state.checkDic[RTypeRBtnType.수신기복구] == switchBtn.checkDic[RTypeRBtnType.수신기복구] &&
            state.checkDic[RTypeRBtnType.방화문정지] == switchBtn.checkDic[RTypeRBtnType.방화문정지] && state.checkDic[RTypeRBtnType.주음향정지] == switchBtn.checkDic[RTypeRBtnType.주음향정지] && state.checkDic[RTypeRBtnType.지구음향정지] == switchBtn.checkDic[RTypeRBtnType.지구음향정지] &&
            state.checkDic[RTypeRBtnType.사이렌정지] == switchBtn.checkDic[RTypeRBtnType.사이렌정지] && state.checkDic[RTypeRBtnType.비상방송정지] == switchBtn.checkDic[RTypeRBtnType.비상방송정지] && state.checkDic[RTypeRBtnType.부저정지] == switchBtn.checkDic[RTypeRBtnType.부저정지] &&
            state.checkDic[RTypeRBtnType.메인펌프오토] == switchBtn.checkDic[RTypeRBtnType.메인펌프오토] && state.checkDic[RTypeRBtnType.메인펌프수동] == switchBtn.checkDic[RTypeRBtnType.메인펌프수동] && state.checkDic[RTypeRBtnType.메인펌프정지] == switchBtn.checkDic[RTypeRBtnType.메인펌프정지] &&
            state.checkDic[RTypeRBtnType.충압펌프오토] == switchBtn.checkDic[RTypeRBtnType.충압펌프오토] && state.checkDic[RTypeRBtnType.충압펌프수동] == switchBtn.checkDic[RTypeRBtnType.충압펌프수동] && state.checkDic[RTypeRBtnType.충압펌프정지] == switchBtn.checkDic[RTypeRBtnType.충압펌프정지] &&
            state.checkDic[RTypeRBtnType.sp펌프오토] == switchBtn.checkDic[RTypeRBtnType.sp펌프오토] && state.checkDic[RTypeRBtnType.sp펌프수동] == switchBtn.checkDic[RTypeRBtnType.sp펌프수동] && state.checkDic[RTypeRBtnType.sp펌프정지] == switchBtn.checkDic[RTypeRBtnType.sp펌프정지] &&
            state.checkDic[RTypeRBtnType.sp충압펌프오토] == switchBtn.checkDic[RTypeRBtnType.sp충압펌프오토] && state.checkDic[RTypeRBtnType.sp충압펌프수동] == switchBtn.checkDic[RTypeRBtnType.sp충압펌프수동] && state.checkDic[RTypeRBtnType.sp충압펌프정지] == switchBtn.checkDic[RTypeRBtnType.sp충압펌프정지])
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

    public void SetHighlightControlPanelCheck(RTypeRPanelButtonCheck state, bool isCheck = true)
    {
        RTypeRPanel.RTypeRPanelButtonAction.RemoveAllListeners();
        RTypeRPanel.RTypeRPanelButtonAction.AddListener(switchBtn =>
        {
            _switchButtons.Clear();
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("예비전원시험").GetButton(),
                select = switchBtn.checkDic[RTypeRBtnType.예비전원시험] == state.checkDic[RTypeRBtnType.예비전원시험]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("자동복구").GetButton(),
                select = state.checkDic[RTypeRBtnType.자동복구] == switchBtn.checkDic[RTypeRBtnType.자동복구]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("축적소등_비축적점등").GetButton(),
                select = state.checkDic[RTypeRBtnType.축적소등_비축적점등] == switchBtn.checkDic[RTypeRBtnType.축적소등_비축적점등]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("수신기복구").GetButton(),
                select = state.checkDic[RTypeRBtnType.수신기복구] == switchBtn.checkDic[RTypeRBtnType.수신기복구]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("방화문정지").GetButton(),
                select = state.checkDic[RTypeRBtnType.방화문정지] == switchBtn.checkDic[RTypeRBtnType.방화문정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("주음향정지").GetButton(),
                select = state.checkDic[RTypeRBtnType.주음향정지] == switchBtn.checkDic[RTypeRBtnType.주음향정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("지구음향정지").GetButton(),
                select = state.checkDic[RTypeRBtnType.지구음향정지] == switchBtn.checkDic[RTypeRBtnType.지구음향정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("사이렌정지").GetButton(),
                select = state.checkDic[RTypeRBtnType.사이렌정지] == switchBtn.checkDic[RTypeRBtnType.사이렌정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("비상방송정지").GetButton(),
                select = state.checkDic[RTypeRBtnType.비상방송정지] == switchBtn.checkDic[RTypeRBtnType.비상방송정지]
            });
            _switchButtons.Add(new SwitchButtonCheck()
            {
                btn = RTypeRPanel.Instance.GetSwitchButton("부저정지").GetButton(),
                select = state.checkDic[RTypeRBtnType.부저정지] == switchBtn.checkDic[RTypeRBtnType.부저정지]
            });

            switch (state.mainPump)
            {

                case ControlMode.Auto:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpAutoBtn(),
                            select = state.checkDic[RTypeRBtnType.메인펌프오토] == switchBtn.checkDic[RTypeRBtnType.메인펌프오토]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpStopBtn(),
                            select = true
                        });
                    }
                    break;
                case ControlMode.Stop:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpStopBtn(),
                            select = state.checkDic[RTypeRBtnType.메인펌프정지] == switchBtn.checkDic[RTypeRBtnType.메인펌프정지]
                        });
                    }
                    break;
                case ControlMode.Manual:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpManualBtn(),
                            select = state.checkDic[RTypeRBtnType.메인펌프수동] == switchBtn.checkDic[RTypeRBtnType.메인펌프수동]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetMainPumpStopBtn(),
                            select = true
                        });
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }



            switch (state.jockeyPump)
            {

                case ControlMode.Auto:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpAutoBtn(),
                            select = state.checkDic[RTypeRBtnType.충압펌프오토] == switchBtn.checkDic[RTypeRBtnType.충압펌프오토]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpStopBtn(),
                            select = true
                        });
                    }
                    break;
                case ControlMode.Stop:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpStopBtn(),
                            select = state.checkDic[RTypeRBtnType.충압펌프정지] == switchBtn.checkDic[RTypeRBtnType.충압펌프정지]
                        });
                    }
                    break;
                case ControlMode.Manual:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpManualBtn(),
                            select = state.checkDic[RTypeRBtnType.충압펌프수동] == switchBtn.checkDic[RTypeRBtnType.충압펌프수동]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetJockeyPumpStopBtn(),
                            select = true
                        });
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            switch (state.spMainPump)
            {

                case ControlMode.Auto:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpAutoBtn(),
                            select = state.checkDic[RTypeRBtnType.sp펌프오토] == switchBtn.checkDic[RTypeRBtnType.sp펌프오토]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpStopBtn(),
                            select = true
                        });

                    }
                    break;
                case ControlMode.Stop:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpStopBtn(),
                            select = state.checkDic[RTypeRBtnType.sp펌프정지] == switchBtn.checkDic[RTypeRBtnType.sp펌프정지]
                        });

                    }
                    break;
                case ControlMode.Manual:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpManualBtn(),
                            select = state.checkDic[RTypeRBtnType.sp펌프수동] == switchBtn.checkDic[RTypeRBtnType.sp펌프수동]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpPumpStopBtn(),
                            select = true
                        });

                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (state.spJockeyPump)
            {

                case ControlMode.Auto:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpAutoBtn(),
                            select = state.checkDic[RTypeRBtnType.sp충압펌프오토] == switchBtn.checkDic[RTypeRBtnType.sp충압펌프오토]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpStopBtn(),
                            select = true
                        });

                    }
                    break;
                case ControlMode.Stop:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpManualBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpStopBtn(),
                            select = state.checkDic[RTypeRBtnType.sp충압펌프정지] == switchBtn.checkDic[RTypeRBtnType.sp충압펌프정지]
                        });

                    }
                    break;
                case ControlMode.Manual:
                    {
                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpAutoBtn(),
                            select = true
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpManualBtn(),
                            select = state.checkDic[RTypeRBtnType.sp충압펌프수동] == switchBtn.checkDic[RTypeRBtnType.sp충압펌프수동]
                        });

                        _switchButtons.Add(new SwitchButtonCheck()
                        {
                            btn = RTypeRPanel.Instance.GetSpJockeyPumpStopBtn(),
                            select = true
                        });

                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ButtonManager.Instance.EnableSpecificButton(_switchButtons);

            if (state.checkDic[RTypeRBtnType.예비전원시험] == switchBtn.checkDic[RTypeRBtnType.예비전원시험] && state.checkDic[RTypeRBtnType.자동복구] == switchBtn.checkDic[RTypeRBtnType.자동복구] &&
                state.checkDic[RTypeRBtnType.축적소등_비축적점등] == switchBtn.checkDic[RTypeRBtnType.축적소등_비축적점등] && state.checkDic[RTypeRBtnType.수신기복구] == switchBtn.checkDic[RTypeRBtnType.수신기복구] &&
                state.checkDic[RTypeRBtnType.방화문정지] == switchBtn.checkDic[RTypeRBtnType.방화문정지] && state.checkDic[RTypeRBtnType.주음향정지] == switchBtn.checkDic[RTypeRBtnType.주음향정지] && state.checkDic[RTypeRBtnType.지구음향정지] == switchBtn.checkDic[RTypeRBtnType.지구음향정지] &&
                state.checkDic[RTypeRBtnType.사이렌정지] == switchBtn.checkDic[RTypeRBtnType.사이렌정지] && state.checkDic[RTypeRBtnType.비상방송정지] == switchBtn.checkDic[RTypeRBtnType.비상방송정지] && state.checkDic[RTypeRBtnType.부저정지] == switchBtn.checkDic[RTypeRBtnType.부저정지] &&
                state.checkDic[RTypeRBtnType.메인펌프오토] == switchBtn.checkDic[RTypeRBtnType.메인펌프오토] && state.checkDic[RTypeRBtnType.메인펌프수동] == switchBtn.checkDic[RTypeRBtnType.메인펌프수동] && state.checkDic[RTypeRBtnType.메인펌프정지] == switchBtn.checkDic[RTypeRBtnType.메인펌프정지] &&
                state.checkDic[RTypeRBtnType.충압펌프오토] == switchBtn.checkDic[RTypeRBtnType.충압펌프오토] && state.checkDic[RTypeRBtnType.충압펌프수동] == switchBtn.checkDic[RTypeRBtnType.충압펌프수동] && state.checkDic[RTypeRBtnType.충압펌프정지] == switchBtn.checkDic[RTypeRBtnType.충압펌프정지] &&
                state.checkDic[RTypeRBtnType.sp펌프오토] == switchBtn.checkDic[RTypeRBtnType.sp펌프오토] && state.checkDic[RTypeRBtnType.sp펌프수동] == switchBtn.checkDic[RTypeRBtnType.sp펌프수동] && state.checkDic[RTypeRBtnType.sp펌프정지] == switchBtn.checkDic[RTypeRBtnType.sp펌프정지] &&
                state.checkDic[RTypeRBtnType.sp충압펌프오토] == switchBtn.checkDic[RTypeRBtnType.sp충압펌프오토] && state.checkDic[RTypeRBtnType.sp충압펌프수동] == switchBtn.checkDic[RTypeRBtnType.sp충압펌프수동] && state.checkDic[RTypeRBtnType.sp충압펌프정지] == switchBtn.checkDic[RTypeRBtnType.sp충압펌프정지])
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

#endregion //버튼 하이라이트 로직

    public void InitEMode()
    {
        F2CorrectBtn.gameObject.SetActive(false);
        F2WrongBtn.gameObject.SetActive(false);
        C2CorrectBtn.gameObject.SetActive(false);
        C2WrongBtn.gameObject.SetActive(false);
        C3CorrectBtn.gameObject.SetActive(false);
        C3WrongBtn.gameObject.SetActive(false);
        C4CorrectBtn.gameObject.SetActive(false);
        C4WrongBtn.gameObject.SetActive(false);
        설비동작평가1.SetActive(false);
        설비동작토글1.isOn = false;
        설비동작토글2.isOn = false;
    }
}
