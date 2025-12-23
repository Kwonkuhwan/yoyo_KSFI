using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VInspector;

public class GasSysIMenuPopup : MonoBehaviour
{
#region singleton

    private static GasSysIMenuPopup instance;
    public static GasSysIMenuPopup Instance { get { return instance; } }

#endregion

    [SerializeField] public Button indexBtn;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button prevBtn;
    [SerializeField] public Button closeBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private GasSysISection sectionObj;
    [SerializeField] private GasSysIMenu menuObj;

    [SerializeField] private TextMeshProUGUI titleText;

    [SerializeField] private GameObject menuPanel;

    [SerializeField] private ExitPopup exitPopup;
    [SerializeField] private CompletePopup completePopup;

    [Foldout("가스계 실감 모드 메뉴")]
    [SerializeField] private GameObject menuParent;
    [SerializeField] private Button 주요구성요소Btn;
    [SerializeField] private Button 작동순서Btn;
    [SerializeField] private Button 점검Btn;

    [Foldout("작동순서 메뉴 선택")]
    [SerializeField] private GameObject modeParent;
    [SerializeField] private Button autoBtn;
    [SerializeField] private Button manualBtn;

    // [Foldout("점검")]
    // [SerializeField] private GameObject checkParent;
    // [SerializeField] private Button safetyCheckBtn;
    // [SerializeField] private Button solTestBtn;
    // [SerializeField] private Button dischargeBtn;
    // [SerializeField] private Button recoverBtn;
    //
    // [Foldout("격발테스트")]
    // [SerializeField] private GameObject solenoidValveTestParent;
    // [SerializeField] private Button manualOperationBtn;
    // [SerializeField] private Button manualControlBoxBtn;
    // [SerializeField] private Button crossCircuitDetectorBtn;
    // [SerializeField] private Button controlPanelSwitchBtn;
    private GasSysIState _gasSysIState = GasSysIState.None;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        indexBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
        homeBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.RemoveAllListeners();
        주요구성요소Btn.onClick.RemoveAllListeners();
        작동순서Btn.onClick.RemoveAllListeners();
        점검Btn.onClick.RemoveAllListeners();
        autoBtn.onClick.RemoveAllListeners();
        manualBtn.onClick.RemoveAllListeners();
        // safetyCheckBtn.onClick.RemoveAllListeners();
        // solTestBtn.onClick.RemoveAllListeners();
        // dischargeBtn.onClick.RemoveAllListeners();
        // recoverBtn.onClick.RemoveAllListeners();
        // manualOperationBtn.onClick.RemoveAllListeners();
        // manualControlBoxBtn.onClick.RemoveAllListeners();
        // crossCircuitDetectorBtn.onClick.RemoveAllListeners();
        // controlPanelSwitchBtn.onClick.RemoveAllListeners();

        _gasSysIState = GasSysIState.None;
        prevBtn.gameObject.SetActive(false);
        ShowObject(menuParent);
        indexBtn.onClick.AddListener(delegate
        {
            ShowMenuPanel(true);
        });
        closeBtn.onClick.AddListener(delegate
        {
            ShowMenuPanel(false, true);
        });
        homeBtn.onClick.AddListener(delegate
        {
            SoundManager.Instance.StopAllFireSound();
            //GasSysManager.Instance.Init();
            SceneManager.LoadSceneAsync("EntryScene");
            //menuObj.Init();
            ShowMenuPanel(false);
        });
        prevBtn.onClick.AddListener(delegate
        {
            ShowObject(menuParent);
        });

        exitPopup.Init(OnExitYesButton, OnExitNoButton);
        exitBtn.onClick.AddListener(ToggleExitPopup);

        주요구성요소Btn.onClick.AddListener(delegate
        {
            _gasSysIState = GasSysIState.주요구성요소;
            SoundManager.Instance.StopAllFireSound();
            sectionObj.InitParts();
            menuObj.gameObject.SetActive(false);
            ShowMenuPanel(false);

        });
        작동순서Btn.onClick.AddListener(delegate
        {
            _gasSysIState = GasSysIState.감시기작동오토;
            ShowObject(modeParent);
        });
        점검Btn.onClick.AddListener(delegate
        {
            //_gasSysIState = GasSysIState.수동조작함작동수동;
            SoundManager.Instance.StopAllFireSound();
            sectionObj.InitCheck();
            menuObj.gameObject.SetActive(false);
            ShowMenuPanel(false);
        });
        autoBtn.onClick.AddListener(delegate
        {
            SoundManager.Instance.StopAllFireSound();
            sectionObj.InitOperationAuto();
            menuObj.gameObject.SetActive(false);
            ShowMenuPanel(false);
        });
        manualBtn.onClick.AddListener(delegate
        {
            SoundManager.Instance.StopAllFireSound();
            sectionObj.InitOperationManual();
            menuObj.gameObject.SetActive(false);
            ShowMenuPanel(false);
        });

        // safetyCheckBtn.onClick.AddListener(delegate
        // {
        //     //전검전 안전 조치
        // });
        // solTestBtn.onClick.AddListener(delegate
        // {
        //     ShowObject(solenoidValveTestParent);
        // });
        // dischargeBtn.onClick.AddListener(delegate
        // {
        //     //디스차지
        // });
        // recoverBtn.onClick.AddListener(delegate
        // {
        //     //점검후 복구
        // });
        //
        // manualOperationBtn.onClick.AddListener(delegate
        // {
        //     //즉시격발
        // });
        // manualControlBoxBtn.onClick.AddListener(delegate
        // {
        //     //수동함조작
        // });
        // crossCircuitDetectorBtn.onClick.AddListener(delegate
        // {
        //     //교차회로
        // });
        // controlPanelSwitchBtn.onClick.AddListener(delegate
        // {
        //     //컨트롤 스위치
        // });
    }
    
    private void Prev()
    {
        // if (solenoidValveTestParent.activeSelf)
        // {
        //     ShowObject(checkParent);
        //     return;
        // }
        // if (checkParent.activeSelf||modeParent.activeSelf)
        // {
        //     ShowObject(menuParent);
        // }
        return;
    }


    public void ShowMenuPanel(bool isShow, bool recovery = false)
    {
        _gasSysIState = GasSysIState.None;
        ShowObject(menuParent);
        prevBtn.gameObject.SetActive(false);
        menuPanel.SetActive(isShow);
        if(CompletePopup.Instance)
            closeBtn.gameObject.SetActive(!CompletePopup.Instance.IsShowCompletePopup());// && !RTypeRGlobalCanvas.Instance.IsShowResultPopup());
        completePopup.ShowCompletePopup(false);
        //RTypeRGlobalCanvas.Instance.HideCheckObj();
        if (isShow)
        {
            //SoundManager.Instance.StopAllFireSound(ref _soundCheck);
            SoundManager.Instance.ZeroVolume();
        }
        else
        {
            if (recovery)
            {
                SoundManager.Instance.RecoveryVolume();
            }
        }
    }

    private void ShowObject(GameObject obj)
    {
        titleText.text = $"모드선택";
        menuParent.SetActive(menuParent.Equals(obj));
        modeParent.SetActive(modeParent.Equals(obj));
        // checkParent.SetActive(checkParent.Equals(obj));
        // solenoidValveTestParent.SetActive(solenoidValveTestParent.Equals(obj));
        prevBtn.gameObject.SetActive(!menuParent.activeSelf);
    }

    private void Update()
    {
        // Windows에서 ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleExitPopup();
        }

        // Android에서 뒤로 가기 버튼 입력 감지
        // if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
        // {
        //     ToggleExitPopup();
        // }
    }

    public void ToggleExitPopup()
    {

        if (exitPopup == null)
            return;
        bool isActive = exitPopup.gameObject.activeSelf;
        exitPopup.gameObject.SetActive(!isActive);
    }

    public void OnExitYesButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(0);
#endif
    }

    // 종료 팝업에서 "아니오" 버튼 클릭 시 팝업 닫기
    public void OnExitNoButton()
    {
        if (exitPopup != null)
        {
            exitPopup.gameObject.SetActive(false);
        }
    }

}
