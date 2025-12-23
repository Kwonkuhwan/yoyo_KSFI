using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public class GasSysIMenu : MonoBehaviour
{
#region singleton

    private static GasSysIMenu instance;
    public static GasSysIMenu Instance { get { return instance; } }

#endregion
    [FormerlySerializedAs("modeParent")]
    [FormerlySerializedAs("modParent")]
    [Foldout("가스계 실감 모드 메뉴")]
    [SerializeField] public GameObject menuParent;
    [SerializeField] private Button 주요구성요소Btn;
    [SerializeField] private Button 작동순서Btn;
    [SerializeField] private Button 점검Btn;

    [FormerlySerializedAs("menu1Parent")]
    [FormerlySerializedAs("menuParent")]
    [Foldout("작동순서 메뉴 선택")]
    [SerializeField] public GameObject modeParent;
    [SerializeField] private Button autoBtn;
    [SerializeField] private Button manualBtn;


    [Foldout("공용")]
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button prevBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private ExitPopup exitPopup;
    // [Foldout("점검")]
    // [SerializeField] public GameObject checkParent;
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
    //
    [SerializeField] private GasSysISection section;
    // Start is called before the first frame update

    private GasSysIState _gasSysIState;

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        gameObject.SetActive(true);
        ShowObject(menuParent);
        GasSysIPartList.Instance.ShowPanel(false);
        GasSysIHintPanel.Instance.ShowHint(false);
        주요구성요소Btn.onClick.RemoveAllListeners();
        작동순서Btn.onClick.RemoveAllListeners();
        점검Btn.onClick.RemoveAllListeners();
        autoBtn.onClick.RemoveAllListeners();
        manualBtn.onClick.RemoveAllListeners();
        homeBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();
        // safetyCheckBtn.onClick.RemoveAllListeners();
        // solTestBtn.onClick.RemoveAllListeners();
        // dischargeBtn.onClick.RemoveAllListeners();
        // recoverBtn.onClick.RemoveAllListeners();
        // manualOperationBtn.onClick.RemoveAllListeners();
        // manualControlBoxBtn.onClick.RemoveAllListeners();
        // crossCircuitDetectorBtn.onClick.RemoveAllListeners();
        // controlPanelSwitchBtn.onClick.RemoveAllListeners();

        주요구성요소Btn.onClick.AddListener(delegate
        {
            _gasSysIState = GasSysIState.주요구성요소;
            HideObject();
            SoundManager.Instance.StopAllFireSound();
            section.InitParts();
            //ShowMenuPanel(false);
        });
        작동순서Btn.onClick.AddListener(delegate
        {
            ShowObject(modeParent);
            _gasSysIState = GasSysIState.감시기작동오토;
        });
        점검Btn.onClick.AddListener(delegate
        {
            HideObject();
            SoundManager.Instance.StopAllFireSound();
            section.InitCheck();
        });
        autoBtn.onClick.AddListener(delegate
        {
            HideObject();
            SoundManager.Instance.StopAllFireSound();
            section.InitOperationAuto();

        });
        manualBtn.onClick.AddListener(delegate
        {
            HideObject();
            SoundManager.Instance.StopAllFireSound();
            section.InitOperationManual();
        });
        
        homeBtn.onClick.AddListener(delegate
        {
            SoundManager.Instance.StopAllFireSound();
            //GasSysManager.Instance.Init();
            SceneManager.LoadSceneAsync("EntryScene");
            //menuObj.Init();
            //ShowObject(false);
        });
        prevBtn.gameObject.SetActive(false);
        prevBtn.onClick.AddListener(delegate
        {
            ShowObject(menuParent);
        });

        exitPopup.Init(OnExitYesButton, OnExitNoButton);
        exitBtn.onClick.AddListener(ToggleExitPopup);

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

    // Update is called once per frame
    public void ShowObject(GameObject obj)
    {
        menuParent.SetActive(menuParent.Equals(obj));
        modeParent.SetActive(modeParent.Equals(obj));
        // checkParent.SetActive(checkParent.Equals(obj));
        // solenoidValveTestParent.SetActive(solenoidValveTestParent.Equals(obj));
        prevBtn.gameObject.SetActive(!menuParent.activeSelf);
        if (prevBtn.gameObject.activeSelf)
        {
            homeBtn.transform.localPosition = new Vector3(43f, -330f);
            exitBtn.transform.localPosition = new Vector3(253f, -330f);
        }
        else
        {
            homeBtn.transform.localPosition = new Vector3(270f, -330f);
            exitBtn.transform.localPosition = new Vector3(480f, -330f);
        }
    }

    public void HideObject()
    {
        gameObject.SetActive(false);
        ShowObject(null);
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

