using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace DGSM
{
    public enum DGSMState
    {
        None,
        Init,
        Main,
        PracticeMode,
        AssessmentMode,
    }
    public class DGSM_Manager : MonoBehaviour
    {
        public static DGSM_Manager Instance;
        [SerializeField] private MainPanel mainPanel;
        [SerializeField] private PracticeModePanel practiceModePanel;

        private DGSMState _currentState;
        //private readonly ReactiveProperty<DGSMState> _currentState = new ReactiveProperty<DGSMState>(DGSMState.Init);
        
        private void Start()
        {
            if (null == Instance)
                Instance = this;
            
            //_currentState.Subscribe(OnStateChanged).AddTo(this);
            ChangeState(DGSMState.Init);
            
        }

        public PracticeModePanel GetPracticeModePanel()
        {
            return practiceModePanel;
        }

        public void ChangeState(DGSMState newState)
        {
            _currentState = newState;
            OnStateChanged(_currentState);
        }

        private void OnStateChanged(DGSMState newState)
        {
            switch (newState)
            {
                case DGSMState.Init:
                    //Initialize();
                    Init();
                    break;
                case DGSMState.Main:
                    MainMenu();
                    break;
                case DGSMState.PracticeMode:
                    PracticeMode();
                    break;
                case DGSMState.AssessmentMode:
                    AssessmentMode();
                    break;
                default:
                    break;
            }
        }

        private void Init()
        {
            Debug.Log("Init");
            ShowPanels(null);
            ChangeState(DGSMState.Main);
        }

        private void MainMenu()
        {
            ShowPanels(mainPanel.gameObject);
            mainPanel.Init(delegate
            {
                ChangeState(DGSMState.PracticeMode);
            }, delegate
            {
                ChangeState(DGSMState.AssessmentMode);
            });
        }

        private void PracticeMode()
        {
            ShowPanels(practiceModePanel.gameObject);
        }

        private void AssessmentMode()
        {

        }

        private void ShowPanels(GameObject obj)
        {
            mainPanel.gameObject.SetActive(mainPanel.gameObject.Equals(obj));
            practiceModePanel.gameObject.SetActive(practiceModePanel.gameObject.Equals(obj));
        }

    }

}
