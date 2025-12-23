using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace DGSM
{
    [Flags]
    public enum DocumentState
    {
        None = 0,
        Init = 1 << 0,
        CheckHandlingLog = 1 << 1,
        CheckEmergencyContact = 1 << 2,
        CheckGeneralInspectionReport = 1 << 3,
        CheckSafetyManagerCertification = 1 << 4,
        CheckDeputyTraining = 1 << 5,
        CheckComplete = Init | CheckHandlingLog | CheckEmergencyContact | CheckGeneralInspectionReport | CheckSafetyManagerCertification |CheckDeputyTraining,
    }

    public class DocumentInspection : MonoBehaviour
    {
        [SerializeField] private Toggle checkHandlingLogToggle;
        [SerializeField] private Toggle checkEmergencyContactToggle;
        [SerializeField] private Toggle checkGeneralInspectionReportToggle;
        [SerializeField] private Toggle checkSafetyManagerCertificationToggle;
        [SerializeField] private Toggle checkDeputyTrainingToggle;
        [SerializeField] private Button checkCompleteButton;

        [Header("체크 리스트 팝업")]
        [SerializeField] private DefaultPopupObj checkHandlingLogPopup;
        [SerializeField] private DefaultPopupObj checkEmergencyContactPopup;
        [SerializeField] private DefaultPopupObj checkGeneralInspectionReportPopup;
        [SerializeField] private DefaultPopupObj checkSafetyManagerCertificationPopup;
        [SerializeField] private DefaultPopupObj checkDeputyTrainingPopup;
        [SerializeField] private DefaultPopupObj checkCompletePopup;

        //private readonly ReactiveProperty<DocumentState> _currentState = new ReactiveProperty<DocumentState>(DocumentState.Init);
        private DocumentState _currentState = DocumentState.Init;
        private DocumentState _stateProgress;
        // Start is called before the first frame update
        private void Start()
        {
            Debug.Log($"{DocumentState.CheckHandlingLog}");
            Debug.Log($"{DocumentState.CheckHandlingLog | DocumentState.CheckEmergencyContact}");
            //var t = DocumentState.CheckHandlingLog | DocumentState.CheckEmergencyContact;
            //ChangeState(DocumentState.Init);
        }

        private void OnEnable()
        {
            ChangeState(DocumentState.Init);
            Debug.Log("OnEnable");
        }

        private void OnDisable()
        {
            Debug.Log("OnDisable");
        }

        public void ChangeState(DocumentState newState)
        {
            _currentState = newState;
            OnStateChanged(_currentState);
        }

        private void OnStateChanged(DocumentState newState)
        {
            switch (newState)
            {
                case DocumentState.Init:
                    Init();
                    _stateProgress |= _currentState;
                    if (IsComplete())
                    {
                        ChangeState(DocumentState.CheckComplete);
                    }
                    break;
                case DocumentState.CheckHandlingLog:
                    checkHandlingLogPopup.Init(delegate
                    {
                        checkHandlingLogPopup.gameObject.SetActive(false);
                        checkHandlingLogToggle.interactable = false;
                    }, null);
                    _stateProgress |= _currentState;
                    if (IsComplete())
                    {
                        ChangeState(DocumentState.CheckComplete);
                    }
                    break;
                case DocumentState.CheckEmergencyContact:
                    checkEmergencyContactPopup.Init(delegate
                    {
                        checkEmergencyContactPopup.gameObject.SetActive(false);
                        checkEmergencyContactToggle.interactable = false;
                    }, null);
                    _stateProgress |= _currentState;
                    if (IsComplete())
                    {
                        ChangeState(DocumentState.CheckComplete);
                    }
                    break;
                case DocumentState.CheckGeneralInspectionReport:
                    checkGeneralInspectionReportPopup.Init(delegate
                    {
                        checkGeneralInspectionReportPopup.gameObject.SetActive(false);
                        checkGeneralInspectionReportToggle.interactable = false;
                    }, null);
                    _stateProgress |= _currentState;
                    if (IsComplete())
                    {
                        ChangeState(DocumentState.CheckComplete);
                    }
                    break;
                case DocumentState.CheckSafetyManagerCertification:
                    checkSafetyManagerCertificationPopup.Init(delegate
                    {
                        checkSafetyManagerCertificationPopup.gameObject.SetActive(false);
                        checkSafetyManagerCertificationToggle.interactable = false;
                    }, null);
                    _stateProgress |= _currentState;
                    if (IsComplete())
                    {
                        ChangeState(DocumentState.CheckComplete);
                    }
                    break;
                case DocumentState.CheckDeputyTraining:
                    checkDeputyTrainingPopup.Init(delegate
                    {
                        checkDeputyTrainingPopup.gameObject.SetActive(false);
                        checkDeputyTrainingToggle.interactable = false;
                    }, null);
                    _stateProgress |= _currentState;
                    if (IsComplete())
                    {
                        ChangeState(DocumentState.CheckComplete);
                    }
                    break;
                case DocumentState.CheckComplete:
                    if (checkCompleteButton != null)
                    {
                        checkCompleteButton.interactable = true;
                    }
                    break;
                case DocumentState.None:
                    break;
                default:
                    break;
            }
     
        }

        private void Init()
        {
            //버튼 초기화
            if (checkHandlingLogToggle != null)
            {
                checkHandlingLogToggle.isOn = false;
                checkHandlingLogToggle.interactable = true;

            }
            if (checkEmergencyContactToggle != null)
            {
                checkEmergencyContactToggle.interactable = true;
                checkEmergencyContactToggle.isOn = false;
            }
            if (checkGeneralInspectionReportToggle != null)
            {
                checkGeneralInspectionReportToggle.interactable = true;
                checkGeneralInspectionReportToggle.isOn = false;
            }
            if (checkSafetyManagerCertificationToggle != null)
            {
                checkSafetyManagerCertificationToggle.interactable = true;
                checkSafetyManagerCertificationToggle.isOn = false;
            }
            if (checkDeputyTrainingToggle != null)
            {
                checkDeputyTrainingToggle.interactable = true;
                checkDeputyTrainingToggle.isOn = false;
            }
            checkHandlingLogToggle?.onValueChanged.RemoveAllListeners();
            checkEmergencyContactToggle?.onValueChanged.RemoveAllListeners();
            checkGeneralInspectionReportToggle?.onValueChanged.RemoveAllListeners();
            checkSafetyManagerCertificationToggle?.onValueChanged.RemoveAllListeners();
            checkDeputyTrainingToggle?.onValueChanged.RemoveAllListeners();
            checkCompleteButton?.onClick.RemoveAllListeners();
            
            
            checkHandlingLogPopup.gameObject.SetActive(false);
            checkEmergencyContactPopup.gameObject.SetActive(false);
            checkGeneralInspectionReportPopup.gameObject.SetActive(false);
            checkSafetyManagerCertificationPopup.gameObject.SetActive(false);
            checkDeputyTrainingPopup.gameObject.SetActive(false);
            _stateProgress = DocumentState.Init;

            checkHandlingLogToggle?.onValueChanged.AddListener(delegate(bool isOn)
            {
                if (isOn)
                {
                    ChangeState(DocumentState.CheckHandlingLog);
                }
            });
            checkEmergencyContactToggle?.onValueChanged.AddListener(delegate(bool isOn)
            {
                if (isOn)
                {
                    ChangeState(DocumentState.CheckEmergencyContact);
                }
            });
            checkGeneralInspectionReportToggle?.onValueChanged.AddListener(delegate(bool isOn)
            {
                if (isOn)
                {
                    ChangeState(DocumentState.CheckGeneralInspectionReport);
                }
            });
            checkSafetyManagerCertificationToggle?.onValueChanged.AddListener(delegate(bool isOn)
            {
                if (isOn)
                {
                    ChangeState(DocumentState.CheckSafetyManagerCertification);
                }
            });
            checkDeputyTrainingToggle?.onValueChanged.AddListener(delegate(bool isOn)
            {
                if (isOn)
                {
                    ChangeState(DocumentState.CheckDeputyTraining);
                }
            });
            if (checkCompleteButton != null)
            {
                checkCompleteButton.interactable = false;
            }
            checkCompleteButton?.onClick.AddListener(delegate
            {
                DGSM_Manager.Instance.GetPracticeModePanel().ChangeState(PracticeModeState.ScenarioSelect);
                //gameObject.SetActive(false);
            });
        }
        
        private bool IsComplete()
        {
            return DocumentState.CheckComplete.Equals(_stateProgress);
        }
        
    }
    

}
