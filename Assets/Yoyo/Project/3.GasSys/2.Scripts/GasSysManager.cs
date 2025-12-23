using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GASSYS
{
    // public enum GasSysState
    // {
    //     Init, //초기화
    //     SelectMode, //모드 선택
    //     PracticeMode, //실습 모드
    //     EvaluationMode, //평가 모드
    //     // CompCheck, //구성장치 확인
    //     // SafetyCheck, //점검 전 안전조치
    //     // DischargeCheck, //방출표시등 점검 확인
    //     // RecoveryCheck, // 점검 후 복구
    // }
    public class GasSysManager : MonoBehaviour
    {
        public static GasSysManager Instance;

        [SerializeField] public GasSysSelectMode selectMode;
        [SerializeField] public GasSysPracticeMode practiceMode;
        [SerializeField] public GasSysManualMode evaluationMode;
        [SerializeField] public Inventory inventory;
        [SerializeField] public GasSysAreaManager areaManager;
        
        [SerializeField] public GasSysMenu gasSysMenu;
        // [SerializeField] private GasSysCompCheck compCheck;
        //
        // [SerializeField] private GasSysSafetyCheck safetyCheck;
        // [SerializeField] private GasSysDischargeCheck dischargeCheck;
        // [SerializeField] private GasSysRecoveryCheck recoveryCheck;
        [SerializeField] private Button goToMenuBtn;

        private CompositeDisposable _disposable = new CompositeDisposable();

        [SerializeField] private GameObject[] roomObjs;
        //private GasSysState _curState = GasSysState.Init;
        
        private void Start()
        {
            
            //Screen.SetResolution(1920, 1080, true);
            Initialization();
            //GasSysSoundManager.Instance.UpdateResolution();
            Init();
            
            //ChangeState(GasSysState.Init);
        }

        private void Initialization()
        {
            if (null == Instance)
                Instance = this;

            _disposable?.Clear();
            // var disposable = goToMenuBtn.OnClickAsObservable().Subscribe(_ =>
            // {
            //     if (GasSysState.SelectMode.Equals(_curState))
            //     {
            //         Application.Quit(0);
            //     }
            //     else
            //     {
            //         ChangeState(GasSysState.Init);
            //         practiceMode.HideObject();
            //     }
            //
            // }).AddTo(this);
            // _disposable?.Add(disposable);
        }

        // public void ChangeState(GasSysState state)
        // {
        //     _curState = state;
        //     OnStateChanged(_curState);
        // }
        //
        // private void OnStateChanged(GasSysState state)
        // {
        //     switch (state)
        //     {
        //
        //         case GasSysState.Init:
        //             Init();
        //             ControlPanel.Instance.ShowPanel(false);
        //             GlobalCanvas.Instance.ShowCompletePopup(false);
        //             SoundManager.Instance.StopAllFireSound();
        //             //GlobalCanvas.Instance.ShowHint(false);
        //             GlobalCanvas.Instance.SetTitle("가스계 소화설비", "");
        //             GlobalCanvas.Instance.SetBackBtn(delegate
        //             {
        //                 if (GasSysState.SelectMode.Equals(_curState))
        //                 {
        //                     Application.Quit(0);
        //                 }
        //                 else
        //                 {
        //                     ChangeState(GasSysState.Init);
        //                     practiceMode.HideObject();
        //                 }
        //             });
        //             ChangeState(GasSysState.SelectMode);
        //             break;
        //         case GasSysState.SelectMode:
        //             //selectMode.Init();
        //             gasSysMenu.Init();
        //             ShowObject(gasSysMenu.gameObject);
        //             break;
        //         case GasSysState.PracticeMode:
        //             gasSysMenu.Init();
        //             //practiceMode.ChangeState(GasSysMainSection.Init);
        //             //ShowObject(practiceMode.gameObject);
        //             break;
        //         case GasSysState.EvaluationMode:
        //             gasSysMenu.Init();
        //             //evaluationMode.ChangeState(GasSysMainSection.Init);
        //             //ShowObject(evaluationMode.gameObject);
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException(nameof(state), state, null);
        //     }
        // }

        public void Init()
        {
            inventory.ShowPanel(false);
            areaManager.ShowPanel(false);
            GasSysGlobalCanvas.Instance.ShowHint(false);
            gasSysMenu.Init();
            ShowObject(gasSysMenu.gameObject);
            CloseRoomObj();
            //ShowObject(gasSysMenu.gameObject);
        }

        private void CloseRoomObj()
        {
            foreach (var obj in roomObjs)
            {
                obj.SetActive(false);
            }
        }

        private void OnEnable()
        {
            Initialization();
        }

        public void ShowObject(GameObject obj)
        {
            try
            {
                gasSysMenu.gameObject.SetActive(gasSysMenu.gameObject.Equals(obj));
                practiceMode.gameObject.SetActive(practiceMode.gameObject.Equals(obj));
                evaluationMode.gameObject.SetActive(evaluationMode.gameObject.Equals(obj));
                // compCheck.gameObject.SetActive(compCheck.gameObject.Equals(obj));
                // safetyCheck.gameObject.SetActive(safetyCheck.gameObject.Equals(obj));
                // dischargeCheck.gameObject.SetActive(dischargeCheck.gameObject.Equals(obj));
                // recoveryCheck.gameObject.SetActive(recoveryCheck.gameObject.Equals(obj));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // public GasSysState GetGasSysState()
        // {
        //     return _curState;
        // }
    }
}
