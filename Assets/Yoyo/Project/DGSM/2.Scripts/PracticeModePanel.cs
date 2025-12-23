using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace DGSM
{
    public enum PracticeModeState
    {
        None,
        Init,
        DocumentInspection,
        RegularInspection,
        InspectionReportSubmit,
        ScenarioSelect,
        CarScenario,
        TankScenario,
        End,
    }
    public class PracticeModePanel : MonoBehaviour
    {
        public static PracticeModePanel instance;
        [Header("배경 이미지")]
        [SerializeField] private GameObject officeBg;

        [Header("인터렉티브 버튼")]
        [SerializeField] private Button documentInspectionBtn;
        [SerializeField] private Button regularInspectionBtn;

        [Header("MainPopups")]
        [SerializeField] private GameObject documentInspectionPopup;
        private PracticeModeState _currentState = PracticeModeState.Init;

        [SerializeField] private GameObject documentInspectionObj;
        [SerializeField] private GameObject regularInspectionObj;
        [SerializeField] private GameObject inspectionReportSubmitObj;
        [SerializeField] private GameObject scenarioSelectObj;
        [SerializeField] private GameObject carScenarioObj;
        
        
        private void Start()
        {
            if (null == instance)
                instance = this;
            ChangeState(PracticeModeState.Init);
        }
        public void ChangeState(PracticeModeState newState)
        {
            _currentState = newState;
            OnStateChanged(_currentState);
        }

        private void OnStateChanged(PracticeModeState newState)
        {
            switch (newState)
            {
                case PracticeModeState.Init:
                    //Initialize();
                    Init();
                    break;
                case PracticeModeState.DocumentInspection:
                    ShowMode(documentInspectionObj);
                    Debug.Log("Check Handling Log");
                    break;
                case PracticeModeState.RegularInspection:
                    Debug.Log("Check Emergency Contact");
                    break;
                case PracticeModeState.InspectionReportSubmit:
                    break;
                case PracticeModeState.ScenarioSelect:
                    ShowMode(scenarioSelectObj);
                    Debug.Log("Check General Inspection Report");
                    break;
                case PracticeModeState.CarScenario:
                    ShowMode(carScenarioObj);
                    Debug.Log("Check Safety Manager Certification");
                    break;
                case PracticeModeState.TankScenario:
                    Debug.Log("Check Deputy Training");
                    break;
                case PracticeModeState.End:
                    Debug.Log("Check Complete");
                    break;
                default:
                    break;
            }
        }

        private void Init()
        {
            ShowMode(documentInspectionObj);
            documentInspectionBtn?.onClick.RemoveAllListeners();
            regularInspectionBtn?.onClick.RemoveAllListeners();
            ShowPopup(null);
            documentInspectionBtn?.onClick.AddListener(delegate
            {
                ShowPopup(documentInspectionPopup);
            });
            regularInspectionBtn?.onClick.AddListener(delegate
            {
                //ShowPopup(documentInspectionPopup);
            });
        }

        private void ShowMode(GameObject obj)
        {
            documentInspectionObj.SetActive(documentInspectionObj.Equals(obj));
            regularInspectionObj.SetActive(regularInspectionObj.Equals(obj));
            inspectionReportSubmitObj.SetActive(inspectionReportSubmitObj.Equals(obj));
            scenarioSelectObj.SetActive(scenarioSelectObj.Equals(obj));
            carScenarioObj.SetActive(carScenarioObj.Equals(obj));
        }

        private void ShowPopup(GameObject obj)
        {
            documentInspectionPopup.gameObject.SetActive(documentInspectionPopup.gameObject.Equals(obj));
            //practiceModePanel.gameObject.SetActive(practiceModePanel.gameObject.Equals(obj));
        }
    }

}
