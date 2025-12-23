using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
#endif

public class TitleManager : MonoBehaviour
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void OpenFullScreen();

    [DllImport("__Internal")]
    private static extern void GoFullscreen();

    [DllImport("__Internal")]
    private static extern void HideAddressBar();
    //     [DllImport("__Internal")]
    // private static extern void QuitGame();
#endif

    [SerializeField] private ExitPopup exitPopup;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button[] btns1;
    [SerializeField] private Button[] btns2;

    [SerializeField] private Button fullBtn;

    // Start is called before the first frame update
    private void Awake()
    {
        Application.targetFrameRate = 120;

        exitPopup.Init(OnExitYesButton, OnExitNoButton);
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(ToggleExitPopup);
        SetButton1();
        SetButton2();

#if UNITY_WEBGL
        exitBtn.gameObject.SetActive(false);
#endif

        // btns[8].onClick.AddListener(delegate
        // {
        //     SceneManager.LoadSceneAsync("TankLorry");
        // });
        // btns[9].onClick.AddListener(delegate
        // {
        //     SceneManager.LoadSceneAsync("TransporterScene");
        // });
#if UNITY_WEBGL && !UNITY_EDITOR
        //fullBtn.gameObject.SetActive(true);
        fullBtn.onClick.AddListener(OnClickFullScreen);
#endif
    }

    public void OnClickFullScreen()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        //OpenFullScreen();
        GoFullscreen();
#else
        Screen.fullScreen = !Screen.fullScreen;
#endif
    }

    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        HideAddressBar();
#endif
    }

    private void SetButton1()
    {
        foreach (var btn in btns1)
        {
            btn.onClick.RemoveAllListeners();
        }

        //P형 수신기
        btns1[0].onClick.AddListener(delegate { Application.OpenURL("https://survey.kfsi.or.kr/"); });
        //R형 수신기
        btns1[1].onClick.AddListener(delegate
        {
            SceneManager.LoadSceneAsync("1.R-TypeReceiver");
            //Application.OpenURL("https://survey.kfsi.or.kr/");
        });
        //제어반
        btns1[2].onClick.AddListener(delegate { Application.OpenURL("https://survey.kfsi.or.kr/"); });
        //습식스프링클러
        btns1[3].onClick.AddListener(delegate { Application.OpenURL("https://survey.kfsi.or.kr/"); });
        //준비작동식스프링클러
        btns1[4].onClick.AddListener(delegate
        {
            //SceneManager.LoadSceneAsync("1.R-TypeReceiver");
            SceneManager.LoadSceneAsync("SprinklerScene");
        });
        //펌프성능시험
        btns1[5].onClick.AddListener(delegate
        {
            //SceneManager.LoadSceneAsync("1.R-TypeReceiver");
            Application.OpenURL("https://survey.kfsi.or.kr/");
        });
        //가스계소화설비
        btns1[6].onClick.AddListener(delegate { SceneManager.LoadSceneAsync("GasSysScene"); });
        //방화셔터
        btns1[7].onClick.AddListener(delegate
        {
            //SceneManager.LoadSceneAsync("SafetyManagerScene");
            Application.OpenURL("https://www.kfsi.or.kr/edu/qrcode/index.html");
        });
    }

    private void SetButton2()
    {
        foreach (var btn in btns2)
        {
            btn.onClick.RemoveAllListeners();
        }

        btns2[0].onClick.AddListener(delegate
        {
#if KFSI_ALL
            SceneManager.LoadSceneAsync("SafetyManagerScene");
#else
#if KFSI_TEST
            SceneManager.LoadSceneAsync("SafetyManagerScene_EvaluationMode");
#else
            SceneManager.LoadSceneAsync("SafetyManagerScene");
#endif
#endif
        });
        btns2[1].onClick.AddListener(delegate
        {
#if KFSI_ALL
            SceneManager.LoadSceneAsync("TankLorry");
#else
#if KFSI_TEST
                SceneManager.LoadSceneAsync("Carrier_Movenment_Evaluation");
#else
            SceneManager.LoadSceneAsync("Carrier_Movenment");
#endif
#endif
        });
        btns2[2].onClick.AddListener(delegate
        {
#if KFSI_ALL
            SceneManager.LoadSceneAsync("TransporterScene");
#else
#if KFSI_TEST
            SceneManager.LoadSceneAsync("Transporter_Evalution");
#else
            SceneManager.LoadSceneAsync("TransporterScene");
#endif
#endif
        });
    }

    private void ToggleExitPopup()
    {
        if (exitPopup is null)
            return;
        bool isActive = exitPopup.gameObject.activeSelf;
        exitPopup.gameObject.SetActive(!isActive);
    }

    private void Update()
    {
        // Windows에서 ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleExitPopup();
        }
    }

    private static void OnExitYesButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit(0);
#endif
    }

    // 종료 팝업에서 "아니오" 버튼 클릭 시 팝업 닫기
    private void OnExitNoButton()
    {
        exitPopup?.gameObject.SetActive(false);
    }
}