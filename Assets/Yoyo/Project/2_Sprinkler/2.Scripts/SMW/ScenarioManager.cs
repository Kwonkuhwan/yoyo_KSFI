using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public enum SCENARIO
    {
        점검,
        감지기,
        수동조작함,
        수동기동밸브,
        수동기동스위치,
        동작시험,
        복구,
        평가점검,
        평가감지기,
        평가수동조작함,
        평가수동기동밸브,
        평가수동기동스위치,
        평가동작시험,
        평가복구
    }

    public class ScenarioManager : MonoBehaviour
    {
        private static ScenarioManager instance;
        public static ScenarioManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new ScenarioManager();
                }
                return instance;
            }
        }
        [Header("프리액션밸브")]
        [SerializeField] PreactionValve PreactionValve;

        [Header("제어반")]
        [SerializeField] GameObject Obj_ControlPanel;
        ControlPanel ControlPanel;

        [Header("힌트")]
        [SerializeField] Hint Hint;

        [Header("공간이동")]
        [SerializeField] Teleport Teleport;

        [Header("주차장")]
        [SerializeField] FireScene FireRoom;

        [Header("문제")]
        [SerializeField] Question Question;

        // 진행중인 시나리오 이름
        SCENARIO scenario;
        public SCENARIO GetScenarioName
        {
            get { return scenario; }
        }

        // 시나리오 단계
        int scenario_index = 0;

        // 시나리오 단계 체크 시 첫 체크판별용도
        bool isFirstLoad = true;

        // 0 = 실습모드, 1 = 평가모드
        public bool IsMode
        {
            get { return isMode; }
        }
        bool isMode;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            SettingEnviroment();
        }

        /// <summary>
        /// 시나리오 단계 체크
        /// </summary>
        public void CheckScenarioStep()
        {
            switch (scenario)
            {
                case SCENARIO.점검:
                    CheckInspectionLevel();
                    break;
                case SCENARIO.감지기:
                    CheckSensorLevel();
                    break;
                case SCENARIO.수동조작함:
                    CheckManualBoxStep();
                    break;
                case SCENARIO.수동기동밸브:
                    CheckOperatingVavle();
                    break;
                case SCENARIO.수동기동스위치:
                    CheckManualSwitch();
                    break;
                case SCENARIO.동작시험:
                    CheckOperationTest();
                    break;
                case SCENARIO.복구:
                    CheckRecoverStep();
                    break;
                case SCENARIO.평가점검:
                    EvaluationInspectionLevel();
                    break;
                case SCENARIO.평가감지기:
                    EvaluationSensorLevel();
                    break;
                case SCENARIO.평가수동조작함:
                    EvaluationManualBoxLevel();
                    break;
                case SCENARIO.평가수동기동밸브:
                    EvaluationOperatingVavleLevel();
                    break;
                case SCENARIO.평가수동기동스위치:
                    EvaluationManualSwitchLevel();
                    break;
                case SCENARIO.평가동작시험:
                    EvaluationOperationTestLevel();
                    break;
                case SCENARIO.평가복구:
                    EvaluationRecoverLevel();
                    break;
            }
        }

        /// <summary>
        /// 환경설정
        /// </summary>
        void SettingEnviroment()
        {
            ControlPanel = Obj_ControlPanel.GetComponent<ControlPanel>();
            ControlPanel.Setting();
            PreactionValve.Setting();
            FireRoom.Setting();
        }

        #region 공통사항
        public void Reset()
        {
            scenario_index = 0;
            isFirstLoad = true;
            Hint.OpenHint(scenario);
            Teleport.Open();

            // 모든 기기들 리셋
            PreactionValve.Reset();
            PreactionValve.CloseAllPopup();

            // 기기들 리셋
            HeatTester.Reset();
            SmokeTester.Reset();
            ManualBox.Reset();
            HeatDectector.Reset();
            SmokeDetector.Reset();

            // 동력제어반 리셋
            PopupManager.Instance.ControlPowerReset();

            // 팝업창 닫기
            PopupManager.Instance.CloseAll();
            FireRoom.CloseAllPopup();

            // 솔레노이드 밸브 리셋
            SolenoidValve.Reset();
            SolenoideValve_Mini.Reset();

            // 다이얼 리셋
            ControlPanel.ResetDial();

            // 감시제어반 리셋
            ControlPanel.Reset();

            // 문제 리셋
            Question.Reset();

            ToggleGroup.gameObject.SetActive(false);
        }

        public void NextScenario()
        {
            scenario_index++;
            isFirstLoad = true;
            CheckScenarioStep();
        }

        public void PrevScenario()
        {
            scenario_index--;
            Hint.OpenHint(scenario, scenario_index);

            isFirstLoad = true;
            CheckScenarioStep();
        }

        void DisableAllButtons()
        {
            Teleport.Interactive(false);

            ControlPanel.InteractiveDial(false);
            ControlPanel.InteractiveSwitch(false);
            ControlPanel.InteractiveTool(false);

            PreactionValve.Interactive(프리액션밸브.닫기);
            PreactionValve.InteractiveSolenoidValve(false);

            PopupManager.Instance.InteractivePowerControl(false);
            PopupManager.Instance.ControlPowerStopButtonInteractive(false);
            PopupManager.Instance.ControlFillingPumpStateInteractive(false);

            SolenoidValve.Interactive(false);

            FireRoom.InteractiveManulBoxOpen(false);
        }

        void OnenableAllButtons()
        {
            Teleport.Interactive(false);

            ControlPanel.InteractiveDial(false);
            ControlPanel.InteractiveTool(true);
            ControlPanel.InteractiveSwitch(true);

            PreactionValve.Interactive(프리액션밸브.열기);
            PreactionValve.InteractiveSolenoidValve(true);

            PopupManager.Instance.InteractivePowerControl(true);
            PopupManager.Instance.ControlPowerStopButtonInteractive(false);
            PopupManager.Instance.ControlFillingPumpStateInteractive(false);

            SolenoidValve.Interactive(true);

            FireRoom.InteractiveManulBoxOpen(true);
            Question.Reset();
        }

        /// <summary>
        /// 시나리오 단계 시작 후 첫 준비단계
        /// </summary>
        /// <param name="StepForReady"></param>
        void FirstLoad(Action StepForReady = null)
        {
            // 실습모드는 버튼들 비활성화처리
            if(!isMode && isFirstLoad)
            {
                isFirstLoad = false;
                DisableAllButtons();
                if (StepForReady != null) StepForReady();
            }
            else if(isMode && isFirstLoad)
            {
                isFirstLoad = false;
                OnenableAllButtons();
                if (StepForReady != null) StepForReady();
            }
        }
        #endregion

        #region 시나리오선택
        /// <summary>
        /// 시나리오 선택
        /// </summary>
        /// <param name="isMode"></param>
        public void SelectScenario(SCENARIO _scenario, bool _isMode)
        {
            this.scenario = _scenario;
            this.isMode = _isMode;
            HighLight.Instance.isHighlight = !_isMode;
            Hint.MuteHint(false);
            Reset();
            CheckScenarioStep();
        }

        public void NextSelectScenario()
        {
            scenario++;
            SelectScenario(scenario, isMode);
            EvaluationManager.Instance.OpenNext(false);
        }

        public void PreSelectScenario()
        {
            scenario--;
            if(scenario < 0) scenario = 0;
            SelectScenario(scenario, isMode);
            EvaluationManager.Instance.OpenNext(false);
        }

        // 재평가
        public void ResetSelectScenario()
        {
            SelectScenario(scenario, isMode);
            EvaluationManager.Instance.OpenNext(false);
        }
        #endregion

        #region 점검 전 조치
        public bool IsSeconValve
        {
            get { return PreactionValve.CheckValve(프리액션밸브.이차템퍼스위치, true); }
        }
        /// <summary>
        /// 점검 전 조치 단계별 완료 여부 파악
        /// </summary>
        void CheckInspectionLevel()
        {
            bool isActive = false;
            switch (scenario_index)
            {
                case 0:
                    // 감시제어반의 음향경보를 정지로 전환하고, 축적을 비축적상태로 전환하세요.
                    {
                        FirstLoad(() =>
                        {
                            PreactionValve.ResetValve(프리액션밸브.이차템퍼스위치);
                            ControlPanel.Reset();
                            Teleport.MoveArea(AREA.감시제어반);
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4, 5 });
                            LogManager.Instance.Reset();
                        });
                        isActive = ControlPanel.CheckSwitchOn(new int[] { 0, 1, 2, 3, 4, 5 });
                    }
                    break;
                case 1:
                    // 2차측 개폐밸브를 잠그고, 감시제어반으로 이동하세요.
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.유수검지장치실);
                            PreactionValve.ResetValve(프리액션밸브.이차템퍼스위치);
                            PreactionValve.Interactive(프리액션밸브.이차템퍼스위치);

                            PopupManager.Instance.FirstGaugeUp();
                            PopupManager.Instance.ResetPopup(POPUP.이차가압계);
                        });
                        ControlPanel.DisplayLightOn(9, IsSeconValve);
                        Teleport.Interactive(IsSeconValve, (int)AREA.감시제어반);
                        if (Teleport.CompareArea(AREA.감시제어반))
                        {
                            Hint.OnClickNextButton();
                        }
                    }
                    break;
                case 2:
                    // 프리액션밸브 2차측 템퍼스위치 표시등 점등상태를 확인하세요.
                    {
                        FirstLoad(()=>
                        {
                            ControlPanel.DisplayLightOn(9, true);
                            ControlPanel.SwitchButtonOnOff((int)SWITCH.부저, true);
                        });
                        isActive = ControlPanel.CheckDisplayOn(new int[] { 9 });
                    }
                    break;
                case 3:
                    // 부저스위치를 복구하여 음향 경보를 확인하세요.
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);

                            ControlPanel.SwitchButtonOnOff((int)SWITCH.부저, true);
                            ControlPanel.InteractiveSwitch(true, new int[] {0,1,2,3,4});
                        });
                        ControlPanel.CheckSwitchOn(new int[] { 0, 1, 2, 3 });
                        ControlPanel.CheckSwitchOff(new int[] { 4 });
                        isActive = ControlPanel.CheckSwitchOn(new int[] { 0,1,2,3,5 }) && ControlPanel.CheckSwitchOff(new int[] { 4 });
                    }
                    break;
                case 4:
                    // "부저 음향장치를 정지상태로 전환하고 유수검지장치실로 이동하세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);

                            PreactionValve.LockValve(프리액션밸브.배수밸브);
                            ControlPanel.SwitchButtonOnOff((int)SWITCH.부저, false);
                            ControlPanel.InteractiveSwitch(true, (int)SWITCH.부저);
                        });
                        Teleport.Interactive(ControlPanel.CheckSwitchOn(SWITCH.부저), (int)AREA.유수검지장치실);
                        if(Teleport.CompareArea(AREA.유수검지장치실))
                        {
                            Hint.OnClickNextButton();
                        }
                    }
                    break;
                case 5:
                    // 배수밸브를 개방하고 현장으로 이동하세요.
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.유수검지장치실);

                            PreactionValve.LockValve(프리액션밸브.배수밸브);
                            PreactionValve.Interactive(프리액션밸브.배수밸브);
                        });
                        Teleport.Interactive(PreactionValve.CheckValve(프리액션밸브.배수밸브, false), (int)AREA.주차장);
                        if (Teleport.CompareArea(AREA.주차장))
                        {
                            Hint.OnClickNextButton();
                        }
                    }
                    break;
                case 6:
                    // 점검 전 조치를 완료하였습니다. 현장으로 이동하여 프리액션밸브 유수검지장치 작동시험을 실시하세요.
                    {
                        FirstLoad(()=>
                        {
                            PopupManager.Instance.Open(POPUP.완료);
                        });
                    }
                    break;
            }

            Hint.ActiveNextButton(isActive);
        }

        // 평가 - 점검 전 준비
        void EvaluationInspectionLevel()
        {
            bool isCorrect = false;
            switch(scenario_index)
            {
                case 0:
                    // "점검 전 제어반 안전 조치를 실시하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            LogManager.Instance.Reset();
                        });
                        isCorrect = ControlPanel.CheckSwitchOn(new int[] { 0, 1, 2, 3, 4, 5 }) && ControlPanel.CheckTools(new int[] { 0, 0, 0 });
                        EvaluationManager.Instance.Box[0].Success[0] = isCorrect;
                    }
                    break;
                case 1:
                    // "프리액션밸브 2차측 개폐밸브를 폐쇄하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.유수검지장치실);

                            PopupManager.Instance.FirstGaugeUp();
                            PopupManager.Instance.ResetPopup(POPUP.이차가압계);
                            PreactionValve.LockValve(프리액션밸브.배수밸브);
                        });
                        isCorrect = PreactionValve.CheckValve(프리액션밸브.이차템퍼스위치, true, true) && PreactionValve.CheckValve(프리액션밸브.일차템퍼스위치, false) &&
                            PreactionValve.CheckValve(프리액션밸브.배수밸브, true) && PreactionValve.CheckValve(프리액션밸브.세팅밸브, false) &&
                            IsOpenSolValve == false;
                        EvaluationManager.Instance.Box[0].Success[1] = isCorrect;
                    }
                    break;
                case 2:
                    // "배수밸브(드레인밸브)를 개방하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PreactionValve.CloseAllPopup();

                            PreactionValve.Reset();
                            PreactionValve.LockValve(프리액션밸브.이차템퍼스위치);
                            PreactionValve.LockValve(프리액션밸브.배수밸브);

                            PopupManager.Instance.CloseAll();
                        });
                        isCorrect = PreactionValve.CheckValve(프리액션밸브.이차템퍼스위치, true) && PreactionValve.CheckValve(프리액션밸브.일차템퍼스위치, false) &&
                            PreactionValve.CheckValve(프리액션밸브.배수밸브, false, true) && PreactionValve.CheckValve(프리액션밸브.세팅밸브, false) &&
                            IsOpenSolValve == false;
                        EvaluationManager.Instance.Box[0].Success[2] = isCorrect;
                    }
                    break;
                case 3:
                    // "정상작동여부를 확인하고 불량 부분에 모두 체크한 후 다음 버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            Question.SetQuestion(new 문제[] { 문제.템퍼스위치, 문제.화재표시등, 문제.밸브개방표시등 });

                            ControlPanel.Reset();
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.축적비축적, SWITCH.부저 }, true);
                            ControlPanel.PowerOnOff(0, true);
                            DisableAllButtons();
                        });
                        isCorrect = Question.CheckAnswer(new 문제[] { 문제.화재표시등, 문제.템퍼스위치 });
                        EvaluationManager.Instance.Box[0].Success[3] = isCorrect;
                    }
                    break;
                case 4:
                    EvaluationManager.Instance.OpenNext(true);
                    break;
            }
            Hint.ActiveNextButton(true, isMode);
        }

        void SetInspection()
        {
            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.축적비축적, SWITCH.부저 }, true);
            ControlPanel.DisplayLightOn(9, true);
        }
        #endregion

        #region 감지기

        [Header("열감지기")]
        [SerializeField] DragAndDrop HeatTester;
        [SerializeField] HeatDectector HeatDectector;
        public bool isHeatDetect
        {
            get { return !HeatDectector.IsAnimation; }
        }

        [Header("연기감지기")]
        [SerializeField] DragAndDrop SmokeTester;
        [SerializeField] SmokeDetector SmokeDetector;
        public bool isSmokeDetect
        {
            get { return !SmokeDetector.IsAnimation; }
        }

        [Header("솔레노이드밸브")]
        [SerializeField] SV_Popup SolenoidValve;
        [SerializeField] SV_Popup SolenoideValve_Mini;
        public bool IsOpenSolValve
        {
            get { return SolenoidValve.IsOpenSolValve || SolenoideValve_Mini.IsOpenSolValve; }
        }

        // 감지기
        void CheckSensorLevel()
        {
            bool isActive = false;

            switch (scenario_index)
            {
                case 0:
                    // 열감지기 테스터기로 열감지기를 동작시키세요.
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.주차장);
                            SetInspection();

                            FireRoom.CloseAllPopup();
                            HeatTester.SetInventoryItem();
                            HeatTester.Interactive(true);
                            SmokeTester.SetInventoryItem();
                            HeatDectector.Reset();

                            LogManager.Instance.Reset();
                        });

                        ControlPanel.PowerOnOff(0, isHeatDetect);
                        ControlPanel.DisplayLightOn(0, isHeatDetect);
                        ControlPanel.SwitchButtonOnOff(SWITCH.주경종, !isHeatDetect);
                        isActive = isHeatDetect;
                    }
                    break;
                case 1:
                    // "감시제어반의 점등상태를 확인하고 경보스위치를 복구하여 음향장치의 정상 작동 여부를 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            FireRoom.CloseAllPopup();
                            Teleport.MoveArea(AREA.감시제어반);

                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4 });
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, true);
                            ControlPanel.SwitchButtonOnOff(SWITCH.주경종, false);
                        });
                        isActive = ControlPanel.CheckSwitchOff(new int[] { 0, 1, 2, 3, 4 });
                    }
                    break;
                case 2:
                    // "음향장치를 다시 정지하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, false);
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4 });
                        });
                        isActive = ControlPanel.CheckSwitchOn(new int[] { 0, 1, 2, 3, 4, 5 });
                    }
                    break;
                case 3:
                    // 주차장으로 이동하여 감지기 B를 추가 동작하세요.
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            Teleport.Interactive(true, (int)AREA.주차장);

                            ControlPanel.DisplayLightOn(1, false);
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, true);

                            SmokeTester.Interactive(false);
                            SmokeDetector.Reset();
                        });
                        if(Teleport.CompareArea(AREA.주차장))
                        {
                            Hint.OnClickNextButton();
                        }
                    }
                    break;
                case 4:
                    // 연기감지기 테스터기를 이용하여 연기감지기를 동작시키세요.
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.주차장);

                            FireRoom.CloseAllPopup();
                            SmokeTester.SetInventoryItem();
                            SmokeTester.Interactive(true);
                            SmokeDetector.Reset();

                            SolenoideValve_Mini.Reset();
                            PopupManager.Instance.CloseAll();
                        });

                        ControlPanel.DisplayLightOn(1, isSmokeDetect);
                        ControlPanel.SwitchButtonOnOff(SWITCH.주경종, !isSmokeDetect);
                        isActive = isSmokeDetect;
                    }
                    break;
                case 5:
                    // 감시제어반의 정상점등 여부를 확인하고 솔레노이드밸브의 정상동작 여부를 확인하세요.
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);

                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, true);
                            ControlPanel.SwitchButtonOnOff(SWITCH.주경종, false);

                            SolenoideValve_Mini.StartOpenValve();
                        });
                        ControlPanel.DisplayLightOn(3, IsOpenSolValve);
                        isActive = ControlPanel.CheckDisplayOn(new int[] { 0, 1, 3, 9 }) && IsOpenSolValve;
                    }
                    break;
                case 6:
                    // "음향장치를 복구하여 정상 작동 여부를 확인하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();

                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, true);
                            ControlPanel.SwitchButtonOnOff(SWITCH.주경종, false);
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4 });
                        });
                        isActive = ControlPanel.CheckSwitchOff(new int[] { 0, 1, 2, 3, 4 });
                    }
                    break;
                case 7:
                    {
                        FirstLoad(() =>
                        {
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, false);
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4 });

                            StopCoroutine("FillingPumpOn");
                            FillingPumpOff();
                        });
                        isActive = ControlPanel.CheckSwitchOn(new int[] { 0, 1, 2, 3, 4, 5 });
                    }
                    break;
                case 8:
                    // "충압펌프 압력스위치와 기동표시등 점등 여부를 확인하고 다음 버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            FillingPumpOff();
                            StartCoroutine("FillingPumpOn");
                            ControlPanel.DisplayLightOn(10, false);

                            StopCoroutine("MainPumpOn");
                            MainPumpOff();
                        });
                        PopupManager.Instance.ControlPower(PUMP.충압펌프, true);
                        isActive = ControlPanel.CheckDisplayOn(new int[] {0,1,3,9,11});
                    }
                    break;
                case 9:
                    // "주펌프 압력스위치와 기동표시등 점등 여부를 확인하고 다음 버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);

                            MainPumpOff();
                            StartCoroutine("MainPumpOn");
                        });
                        PopupManager.Instance.ControlPower(PUMP.주펌프, true);
                        isActive = ControlPanel.CheckDisplayOn(new int[] {0,1,3,9,10});
                    }
                    break;
                case 10:
                    // 펌프실로 이동하여 펌프의 정상 작동여부를 확인하세요.
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            Teleport.Interactive(true, (int)AREA.펌프실);
                            PopupManager.Instance.CloseAll();
                        });
                        if(Teleport.CompareArea(AREA.펌프실))
                        {
                            Hint.OnClickNextButton();
                        }
                    }
                    break;
                case 11:
                    // 펌프 기동 및 동력제어반 기동표시등을 확인하세요.
                    {
                        FirstLoad(() => 
                        {
                            PopupManager.Instance.CloseAll();
                            PopupManager.Instance.InteractivePowerControl(true);
                        });
                        isActive = PopupManager.Instance.CurrentOpenPopup == POPUP.동력제어반;
                    }
                    break;
                case 12:
                    // 펌프의 정상기동으로 소화수가 개방된 배수밸브를 통해 물이 배수되는 것을 확인하여 수동기동밸브 테스트를 완료하였습니다.
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.MultyOpen(new POPUP[] { POPUP.배수밸브, POPUP.동력제어반 });
                        });
                        isActive = true;
                    }
                    break;
                case 13:
                    {
                        PopupManager.Instance.Open(POPUP.완료);
                    }
                    break;
                default:
                    Debug.LogError("시나리오를 벗어남");
                    break;
            }

            Hint.ActiveNextButton(isActive);
        }

        void EvaluationSensorLevel()
        {
            bool isCorrect = false;
            switch (scenario_index)
            {
                case 0:
                    // "교차회로 감지기 동작시험을 실시하겠습니다. 열감지기를 동작시키고 다음버튼을 누르세요",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.주차장);
                            SetInspection();

                            SmokeTester.SetInventoryItem();
                            SmokeTester.Interactive(true);
                            HeatTester.SetInventoryItem();
                            HeatTester.Interactive(true);

                            LogManager.Instance.Reset();
                        });
                        isCorrect = isHeatDetect && isSmokeDetect == false && IsSVP == false;
                        EvaluationManager.Instance.Box[1].Success[0] = isCorrect;
                    }
                    break;
                case 1:
                    // "정상작동여부를 확인하고 불량 부분에 모두 체크한 후 다음 버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);

                            HeatDectector.Detect();
                            SmokeDetector.Reset();
                            SmokeTester.SetInventoryItem();
                            ManualBox.Reset();

                            ControlPanel.DisplayLightOn(0, true);
                            ControlPanel.SwitchButtonOnOff(0, false);
                            ControlPanel.PowerOnOff(0, false);

                            Question.SetALL(new 문제[] { 문제.음향장치, 문제.수동조작함 });
                            DisableAllButtons();
                            SoundManager.Instance.MuteAll(true);
                        });

                        isCorrect = Question.CheckAnswer(new 문제[] { 문제.화재표시등 });
                        EvaluationManager.Instance.Box[1].Success[1] = isCorrect;
                    }
                    break;
                case 2:
                    // "연기감지기를 동작시키고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.주차장);
                            FireRoom.CloseAllPopup();

                            SmokeTester.Interactive(true);
                            HeatTester.SetInventoryItem();
                            HeatTester.Interactive(true);
                        });
                        isCorrect = isHeatDetect && isSmokeDetect && IsSVP == false;
                        EvaluationManager.Instance.Box[1].Success[2] = isCorrect;
                    }
                    break;
                case 3:
                    // "감시제어반의 전기적 신호가 솔레노이드밸브를 작동시키는 것을 확인하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);

                            ControlPanel.Reset();
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저, SWITCH.축적비축적 }, true);
                            ControlPanel.DisplayLightOn(0, true);
                            ControlPanel.DisplayLightOn(1, true);
                            ControlPanel.DisplayLightOn(9, true);

                            SmokeDetector.Detect();
                            ManualBox.Reset();

                            SolenoideValve_Mini.StartOpenValve();
                        });
                    }
                    break;
                case 4:
                    // "정상작동여부를 확인하고 불량 부분에 모두 체크한 후 다음 버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();

                            ControlPanel.Reset();
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저, SWITCH.축적비축적 }, true);
                            ControlPanel.DisplayLightOn(0, true);
                            ControlPanel.DisplayLightOn(1, true);
                            ControlPanel.DisplayLightOn(9, true);
                            ControlPanel.DisplayLightOn(10, true);
                            ControlPanel.PowerOnOff(0, true);

                            PopupManager.Instance.ControlPower(PUMP.주펌프, true);

                            Question.SetALL(new 문제[] { 문제.수동조작함, 문제.템퍼스위치, 문제.음향장치 });
                            DisableAllButtons();
                            SoundManager.Instance.MuteAll(true);
                        });
                        isCorrect = Question.CheckAnswer(new 문제[] { 문제.밸브개방표시등, 문제.펌프기동표시등 });
                        EvaluationManager.Instance.Box[1].Success[3] = isCorrect;
                    }
                    break;
                case 5:
                    // "펌프 기동 및 동력제어반 기동표시등을 확인하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.펌프실);
                        });
                    }
                    break;
                case 6:
                    // "펌프의 정상기동으로 소화수가 개방된 배수밸브를 통해 물이 배수되는 것을 확인하여 교차회로 감지기 테스트를 완료하였습니다. 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.MultyOpen(new POPUP[] { POPUP.배수밸브, POPUP.동력제어반 });
                        });
                    }
                    break;
                case 7:
                    EvaluationManager.Instance.OpenNext(true);
                    break;
            }
            Hint.ActiveNextButton(true, isMode);
        }

        /// <summary>
        /// 충압펌프
        /// </summary>
        IEnumerator FillingPumpOn()
        {
            ControlPanel.DisplayLightOn(11, true);
            yield return new WaitForSeconds(1);
            ControlPanel.ToolsDisplayOn(1, true);
            CheckScenarioStep();
        }

        void FillingPumpOff()
        {
            ControlPanel.ToolsDisplayOn(1, false);
            ControlPanel.DisplayLightOn(11, false);
        }

        /// <summary>
        /// 메인펌프
        /// </summary>
        IEnumerator MainPumpOn()
        {
            StopCoroutine("FillingPumpOn");
            ControlPanel.DisplayLightOn(10, true);
            ControlPanel.DisplayLightOn(11, true);
            ControlPanel.ToolsDisplayOn(1, true);
            yield return new WaitForSeconds(1);
            ControlPanel.ToolsDisplayOn(0, true);
            FillingPumpOff();
            PopupManager.Instance.ControlPower(PUMP.충압펌프, false);
            CheckScenarioStep();
        }

        void MainPumpOff()
        {
            ControlPanel.ToolsDisplayOn(0, false);
            ControlPanel.DisplayLightOn(10, false);
        }
        #endregion

        [SerializeField] ManualBox ManualBox;
        public bool IsSVP
        {
            get { return ManualBox.IsSVP; }
        }

        #region 수동조작함
        // 수동조작함
        void CheckManualBoxStep()
        {
            bool isActive = false;
            switch(scenario_index)
            {
                case 0:
                    // "해당 방호구역 수동조작함(SVP)의 수동조작스위치를 작동시키세요.",
                    {
                        FirstLoad(() => 
                        {
                            SetInspection();
                            Teleport.MoveArea(AREA.주차장);
                            PopupManager.Instance.CloseAll();

                            FireRoom.CloseAllPopup();
                            FireRoom.InteractiveManulBoxOpen(true);
                            ManualBox.Reset();

                            LogManager.Instance.Reset();
                        });
                        if(IsSVP)
                        {
                            Hint.OnClickNextButton();
                        }
                    }
                    break;
                case 1:
                    // "수동조작함(SVP)의 밸브개방 표시등 점등과 솔레노이드 밸브 개방을 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.주차장);
                            PopupManager.Instance.Open(POPUP.솔레노이드밸브);

                            SolenoidValve.StartOpenValve();

                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, true);
                            ControlPanel.SwitchButtonOnOff(SWITCH.주경종, false);
                            ControlPanel.SwitchButtonOnOff(0, false);
                        });
                        ManualBox.IsValveOpen(IsOpenSolValve);
                        ControlPanel.SwitchButtonOnOff(0, !IsOpenSolValve);
                        ControlPanel.PowerOnOff(0, IsOpenSolValve);
                        ControlPanel.DisplayLightOn(2, IsOpenSolValve);
                        ControlPanel.DisplayLightOn(3, IsOpenSolValve);
                        isActive = IsOpenSolValve;
                    }
                    break;
                case 2:
                    // "감시제어반의 점등상태를 확인하고 경보스위치를 복구하여 음향장치의 정상 작동 여부를 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            PopupManager.Instance.CloseAll();

                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, true);
                            ControlPanel.SwitchButtonOnOff(SWITCH.주경종, false);
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4 });
                        });
                        isActive = ControlPanel.CheckSwitchOff(new int[] { 0, 1, 2, 3, 4 });
                    }
                    break;
                case 3:
                    // "음향장치를 다시 정지하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, false);
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4 });

                            StopCoroutine("FillingPumpOn");
                            FillingPumpOff();
                        });
                        isActive = ControlPanel.CheckSwitchOn(new int[] { 0, 1, 2, 3, 4, 5 });
                    }
                    break;
                case 4:
                    // "감시제어반의 충압펌프 압력스위치와 펌프기동 표시등 점등여부를 확인하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            FillingPumpOff();
                            StartCoroutine("FillingPumpOn");

                            StopCoroutine("MainPumpOn");
                            MainPumpOff();
                        });
                        PopupManager.Instance.ControlPower(PUMP.충압펌프, true);
                        isActive = ControlPanel.CheckDisplayOn(new int[] { 2, 3, 9, 11 });
                    }
                    break;
                case 5:
                    // "감시제어반의 주펌프 압력스위치와 주펌프 기동 표시등 점등여부를 확인하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);

                            MainPumpOff();
                            StartCoroutine("MainPumpOn");
                        });
                        PopupManager.Instance.ControlPower(PUMP.주펌프, true);
                        isActive = ControlPanel.CheckDisplayOn(new int[] { 2, 3, 9, 10 });
                    }
                    break;
                case 6:
                    // "펌프실로 이동하여 펌프의 정상 작동여부를 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            Teleport.Interactive(true, (int)AREA.펌프실);
                            PopupManager.Instance.CloseAll();
                        });
                        if (Teleport.CompareArea(AREA.펌프실))
                        {
                            Hint.OnClickNextButton();
                        }
                    }
                    break;
                case 7:
                    // "펌프 기동 및 동력제어반 기동표시등을 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();
                            PopupManager.Instance.InteractivePowerControl(true);
                        });
                        isActive = PopupManager.Instance.CurrentOpenPopup == POPUP.동력제어반;
                    }
                    break;
                case 8:
                    // "펌프의 정상기동으로 소화수가 개방된 배수밸브를 통해 물이 배수되는 것을 확인하여 수동조작함(SVP) 테스트를 완료하였습니다.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.MultyOpen(new POPUP[] { POPUP.동력제어반, POPUP.배수밸브 });
                        });
                        isActive = true;
                    }
                    break;
                case 9:
                    {
                        PopupManager.Instance.Open(POPUP.완료);
                    }
                    break;
            }
            Hint.ActiveNextButton(isActive);
        }

        void EvaluationManualBoxLevel()
        {
            bool isCorrect = false;
            switch (scenario_index)
            {
                case 0:
                    // "수동조작함 동작 시험을 실시하고 다음버튼을 누르세요",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.주차장);
                            SetInspection();
                            LogManager.Instance.Reset();
                        });
                        if(IsSVP == true && IsOpenSolValve == false)
                        {
                            SolenoidValve.StartOpenValve();
                        }
                        ManualBox.IsValveOpen(IsOpenSolValve);
                        isCorrect = IsSVP && IsOpenSolValve;
                        EvaluationManager.Instance.Box[2].Success[0] = isCorrect;
                    }
                    break;
                case 1:
                    // "정상작동여부를 확인하고 불량 부분에 모두 체크한 후 다음 버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            PopupManager.Instance.CloseAll();

                            SolenoidValve.OpenReset();

                            ControlPanel.Reset();
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저, SWITCH.축적비축적 }, true);
                            ControlPanel.DisplayLightOn(0, true);
                            ControlPanel.DisplayLightOn(1, true);
                            ControlPanel.DisplayLightOn(3, true);
                            ControlPanel.DisplayLightOn(9, true);
                            ControlPanel.DisplayLightOn(10, true);
                            ControlPanel.ToolsDisplayOn(0, true);

                            PopupManager.Instance.ControlPower(PUMP.주펌프, true);

                            Question.SetALL(new 문제[] { 문제.템퍼스위치, 문제.음향장치 });
                            SoundManager.Instance.MuteAll(true);
                            DisableAllButtons();
                        });
                        isCorrect = Question.CheckAnswer(new 문제[] { 문제.감지기동작표시등, 문제.수동조작함, 문제.화재표시등 });
                        EvaluationManager.Instance.Box[2].Success[1] = isCorrect;
                    }
                    break;
                case 2:
                    // "동력제어반의 펌프기동 표시등을 확인하고 다음버튼을 누르세요",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.펌프실);
                            PopupManager.Instance.CloseAll();
                            PopupManager.Instance.InteractivePowerControl(true);
                        });
                    }
                    break;
                case 3:
                    // "펌프의 정상기동으로 소화수가 개방된 배수밸브를 통해 물이 배수되는 것을 확인하여 수동조작함 테스트를 완료하였습니다. 다음버튼을 누르세요",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.MultyOpen(new POPUP[] { POPUP.배수밸브, POPUP.동력제어반 });
                        });
                    }
                    break;
                case 4:
                    {
                        EvaluationManager.Instance.OpenNext(true);
                    }
                    break;
            }
            Hint.ActiveNextButton(true, isMode);
        }
        #endregion

        #region 수동기동밸브
        void CheckOperatingVavle()
        {
            bool isActive = false;
            switch(scenario_index)
            {
                case 0:
                    // "솔레노이드밸브를 수동 개방합니다.",
                    {
                        FirstLoad(() =>
                        {
                            ControlPanel.Reset();
                            SetInspection();
                            Teleport.MoveArea(AREA.유수검지장치실);
                            PopupManager.Instance.CloseAll();

                            PreactionValve.InteractiveSolenoidValve(true);
                            PreactionValve.LockValve(프리액션밸브.이차템퍼스위치);

                            SolenoidValve.Reset();
                            SolenoidValve.StartMaunal();

                            PopupManager.Instance.FirstGaugeUp();
                            PopupManager.Instance.ResetPopup(POPUP.이차가압계);

                            LogManager.Instance.Reset();
                        });
                        ControlPanel.DisplayLightOn(3, SolenoidValve.IsOpenSolValve);
                        isActive = SolenoidValve.CheckVavle(true);
                    }
                    break;
                case 1:
                    // "배수밸브를 통해 소화수가 배수되는 것을 확인하고 감시제어반으로 이동하세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.유수검지장치실);
                            Teleport.Interactive(true, (int)AREA.감시제어반);
                            SolenoidValve.Interactive(false);
                            PopupManager.Instance.MultyOpen(new POPUP[] { POPUP.솔레노이드밸브, POPUP.배수밸브 });
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, true);
                        });
                        if(Teleport.CompareArea(AREA.감시제어반))
                        {
                            Hint.OnClickNextButton();
                        }
                    }
                    break;
                case 2:
                    // "감시제어반의 점등상태를 확인하고 경보스위치를 복구하여 음향장치의 정상 작동 여부를 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();

                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, true);
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4 });
                        });
                        isActive = ControlPanel.CheckSwitchOff(new int[] { 0, 1, 2, 3, 4 });
                    }
                    break;
                case 3:
                    // "음향장치를 다시 정지하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, false);
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4 });

                            StopCoroutine("FillingPumpOn");
                            FillingPumpOff();
                        });
                        isActive = ControlPanel.CheckSwitchOn(new int[] { 0, 1, 2, 3, 4, 5 });
                    }
                    break;
                case 4:
                    // "감시제어반의 충압펌프 압력스위치와 충압펌프 기동표시등 점등여부를 확인하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            FillingPumpOff();
                            StartCoroutine("FillingPumpOn");

                            StopCoroutine("MainPumpOn");
                            MainPumpOff();
                        });
                        PopupManager.Instance.ControlPower(PUMP.충압펌프, true);
                        isActive = ControlPanel.CheckDisplayOn(new int[] { 3, 9, 11 });
                    }
                    break;
                case 5:
                    // "감시제어반의 주펌프 압력스위치와 주펌프 기동표시등 점등여부를 확인하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);

                            MainPumpOff();
                            StartCoroutine("MainPumpOn");
                        });

                        PopupManager.Instance.ControlPower(PUMP.주펌프, true);
                        isActive = ControlPanel.CheckDisplayOn(new int[] { 3, 9, 10 });
                    }
                    break;
                case 6:
                    // "펌프실로 이동하여 펌프의 정상 작동여부를 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            Teleport.Interactive(true, (int)AREA.펌프실);
                            PopupManager.Instance.CloseAll();
                        });
                        if (Teleport.CompareArea(AREA.펌프실))
                        {
                            Hint.OnClickNextButton();
                        }
                    }
                    break;
                case 7:
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();
                            PopupManager.Instance.InteractivePowerControl(true);
                        });
                        isActive = PopupManager.Instance.CurrentOpenPopup == POPUP.동력제어반;
                    }
                    break;
                case 8:
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.MultyOpen(new POPUP[] { POPUP.동력제어반, POPUP.배수밸브 });
                        });
                        isActive = true;
                    }
                    break;
                case 9:
                    {
                        PopupManager.Instance.Open(POPUP.완료);
                    }
                    break;
            }
            Hint.ActiveNextButton(isActive);
        }

        void EvaluationOperatingVavleLevel()
        {
            bool isCorrect = false;
            switch (scenario_index)
            {
                case 0:
                    // "수동기동밸브 개방 시험을 실시하고 다음 버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            SetInspection();

                            Teleport.MoveArea(AREA.유수검지장치실);
                            PreactionValve.LockValve(프리액션밸브.이차템퍼스위치);

                            PopupManager.Instance.FirstGaugeUp();
                            PopupManager.Instance.ResetPopup(POPUP.이차가압계);

                            LogManager.Instance.Reset();
                        });

                        isCorrect = IsOpenSolValve == true && PreactionValve.CheckValve(프리액션밸브.이차템퍼스위치, true) &&
                            PreactionValve.CheckValve(프리액션밸브.배수밸브, false) && PreactionValve.CheckValve(프리액션밸브.일차템퍼스위치, false) &&
                            PreactionValve.CheckValve(프리액션밸브.세팅밸브, false);
                        EvaluationManager.Instance.Box[3].Success[0] = isCorrect;
                    }
                    break;
                case 1:
                    // "배수밸브를 통해 소화수가 배수되는 것을 확인하고 다음버튼을 누르세요",
                    {
                        FirstLoad(() =>
                        {
                            SolenoidValve.OpenReset();
                            FireRoom.CloseAllPopup();
                            PopupManager.Instance.MultyOpen(new POPUP[] { POPUP.솔레노이드밸브, POPUP.배수밸브 });
                        });
                    }
                    break;
                case 2:
                    // "정상작동여부를 확인하고 불량 부분에 모두 체크한 후 다음 버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            PopupManager.Instance.CloseAll();

                            ControlPanel.Reset();
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저, SWITCH.축적비축적 }, true);
                            ControlPanel.DisplayLightOn(2, true);
                            ControlPanel.DisplayLightOn(9, true);
                            ControlPanel.DisplayLightOn(10, true);

                            Question.SetALL(new 문제[] { 문제.템퍼스위치, 문제.음향장치 });
                            DisableAllButtons();
                            SoundManager.Instance.MuteAll(true);
                        });
                        isCorrect = Question.CheckAnswer(new 문제[] { 문제.수동조작함, 문제.밸브개방표시등, 문제.펌프기동표시등 });
                        EvaluationManager.Instance.Box[3].Success[1] = isCorrect;
                    }
                    break;
                case 3:
                    // "동력제어반의 펌프기동 표시등을 확인하고 다음버튼을 누르세요",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.펌프실);
                            PopupManager.Instance.CloseAll();
                            PopupManager.Instance.InteractivePowerControl(true);
                            PopupManager.Instance.ControlPower(PUMP.주펌프, true);
                        });
                    }
                    break;
                case 4:
                    // "펌프의 정상기동으로 소화수가 개방된 배수밸브를 통해 물이 배수되는 것을 확인하여 수동기동밸브 테스트를 완료하였습니다. 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.MultyOpen(new POPUP[] { POPUP.배수밸브, POPUP.동력제어반 });
                        });
                    }
                    break;
                case 5:
                    EvaluationManager.Instance.OpenNext(true);
                    break;
            }
            Hint.ActiveNextButton(true, isMode);
        }
        #endregion

        #region 수동기동스위치
        void CheckManualSwitch()
        {
            bool isActive = false;
            switch (scenario_index)
            {
                case 0:
                    // "감시제어반의 솔레노이드밸브 선택스위치를 눌러 수동위치로 전환하세요.",
                    {
                        FirstLoad(() =>
                        {
                            SetInspection();
                            Teleport.MoveArea(AREA.감시제어반);

                            ControlPanel.InteractiveTool(true, 조작툴.솔레노이드밸브, false);
                            ControlPanel.ToolsChangeState(2, 작동.자동);

                            SolenoideValve_Mini.Reset();
                            LogManager.Instance.Reset();
                        });
                        isActive = ControlPanel.CheckTools(new int[] { 0, 0, 2 });
                    }
                    break;
                case 1:
                    // "감시제어반의 솔레노이드밸브 기동스위치를 눌러 밸브를 개방하세요.",
                    {
                        FirstLoad(() =>
                        {
                            SolenoideValve_Mini.Reset();
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, true);
                        });

                        ControlPanel.InteractiveTool(!IsOpenSolValve, 조작툴.솔레노이드밸브, true);
                        ControlPanel.DisplayLightOn(3, IsOpenSolValve);
                        isActive = IsOpenSolValve;
                    }
                    break;
                case 2:
                    // "감시제어반의 점등상태를 확인하고 경보스위치를 복구하여 음향장치의 정상 작동 여부를 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4 });
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, true);
                        });
                        isActive = ControlPanel.CheckSwitchOff(new int[] { 0, 1, 2, 3, 4 });
                    }
                    break;
                case 3:
                    // "음향장치를 다시 정지하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, false);
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4 });

                            StopCoroutine("FillingPumpOn");
                            FillingPumpOff();
                        });
                        isActive = ControlPanel.CheckSwitchOn(new int[] { 0, 1, 2, 3, 4, 5 });
                    }
                    break;
                case 4:
                    // "감시제어반의 충압펌프 압력스위치와 펌프기동 표시등 점등여부를 확인하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            FillingPumpOff();
                            StartCoroutine("FillingPumpOn");

                            StopCoroutine("MainPumpOn");
                            MainPumpOff();
                        });

                        PopupManager.Instance.ControlPower(PUMP.충압펌프, true);
                        isActive = ControlPanel.CheckDisplayOn(new int[] { 3, 9, 11 });
                    }
                    break;
                case 5:
                    // "감시제어반의 주펌프 압력스위치와 주펌프 기동표시등 점등여부를 확인하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);

                            MainPumpOff();
                            StartCoroutine("MainPumpOn");
                        });

                        PopupManager.Instance.ControlPower(PUMP.주펌프, true);
                        isActive = ControlPanel.CheckDisplayOn(new int[] { 3, 9, 10 });
                    }
                    break;
                case 6:
                    // "펌프실로 이동하여 펌프의 정상 작동여부를 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            Teleport.Interactive(true, (int)AREA.펌프실);
                            PopupManager.Instance.CloseAll();
                        });
                        if (Teleport.CompareArea(AREA.펌프실))
                        {
                            Hint.OnClickNextButton();
                        }
                    }
                    break;
                case 7:
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();
                            PopupManager.Instance.InteractivePowerControl(true);
                        });
                        isActive = PopupManager.Instance.CurrentOpenPopup == POPUP.동력제어반;
                    }
                    break;
                case 8:
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.MultyOpen(new POPUP[] { POPUP.동력제어반, POPUP.배수밸브 });
                        });
                        isActive = true;
                    }
                    break;
                case 9:
                    {
                        PopupManager.Instance.Open(POPUP.완료);
                    }
                    break;
            }
            Hint.ActiveNextButton(isActive);
        }

        void EvaluationManualSwitchLevel()
        {
            bool isCorrect = false;
            switch (scenario_index)
            {
                case 0:
                    // "수동기동스위치 작동 시험을 실시하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            SetInspection();
                            LogManager.Instance.Reset();
                        });
                        ControlPanel.DisplayLightOn(3, IsOpenSolValve);
                        isCorrect = IsOpenSolValve == true;
                        EvaluationManager.Instance.Box[4].Success[0] = isCorrect;
                    }
                    break;
                case 1:
                    // "정상작동여부를 확인하고 불량 부분에 모두 체크한 후 다음 버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();
                            ControlPanel.Reset();
                            SetInspection();
                            ControlPanel.DisplayLightOn(2, true);
                            ControlPanel.DisplayLightOn(3, true);
                            ControlPanel.DisplayLightOn(10, true);
                            ControlPanel.ToolsChangeState(2, 작동.수동);

                            Question.SetALL(new 문제[] { 문제.템퍼스위치, 문제.음향장치 });
                            DisableAllButtons();
                            SoundManager.Instance.MuteAll(true);
                        });
                        isCorrect = Question.CheckAnswer(new 문제[] { 문제.수동조작함, 문제.펌프기동표시등 });
                        EvaluationManager.Instance.Box[4].Success[1] = isCorrect;
                    }
                    break;
                case 2:
                    // "동력제어반의 펌프기동 표시등을 확인하고 다음버튼을 누르세요",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.펌프실);
                            PopupManager.Instance.CloseAll();
                            PopupManager.Instance.InteractivePowerControl(true);

                            PopupManager.Instance.ControlPower(PUMP.주펌프, true);
                        });
                    }
                    break;
                case 3:
                    // "펌프의 정상기동으로 소화수가 개방된 배수밸브를 통해 물이 배수되는 것을 확인하여 수동기동스위치 테스트를 완료하였습니다. 다음버튼을 누르세요",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.MultyOpen(new POPUP[] { POPUP.배수밸브, POPUP.동력제어반 });
                        });
                    }
                    break;
                case 4:
                    {
                        EvaluationManager.Instance.OpenNext(true);
                    }
                    break;
            }
            Hint.ActiveNextButton(true, isMode);
        }

        /// <summary>
        /// 솔레노이드밸브 기동
        /// </summary>
        public void OnClickSolVavleTool()
        {
            SolenoideValve_Mini.StartOpenValve();
        }
        #endregion

        #region 동작시험
        public bool isOperationTest
        {
            get { return ControlPanel.DialNumber == 1; }
        }
        public bool isOperationTest2
        {
            get { return ControlPanel.DialNumber == 2; }
        }
        // 동작시험
        void CheckOperationTest()
        {
            bool isActive = false;
            switch (scenario_index)
            {
                case 0:
                    // "감시제어반의 동작시험 스위치를 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            SetInspection();
                            Teleport.MoveArea(AREA.감시제어반);

                            ControlPanel.SwitchButtonOnOff(SWITCH.동작시험, false);
                            ControlPanel.InteractiveSwitch(true, SWITCH.동작시험);

                            ControlPanel.ResetDial();
                            LogManager.Instance.Reset();
                        });
                        isActive = ControlPanel.CheckSwitchOn(SWITCH.동작시험);
                    }
                    break;
                case 1:
                    // "회로선택스위치를 1번으로 돌려 감지기 A회로를 동작시키세요.",
                    {
                        FirstLoad(() =>
                        {
                            ControlPanel.ResetDial();
                            ControlPanel.InteractiveDial(true);
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, true);
                        });
                        ControlPanel.DisplayLightOn(0, ControlPanel.CheckDial(1));      // 감지기A
                        ControlPanel.PowerOnOff(0, ControlPanel.CheckDial(1));          // 화재
                        ControlPanel.SwitchButtonOnOff(SWITCH.주경종, !ControlPanel.CheckDial(1));
                        isActive = ControlPanel.CheckDial(1);
                    }
                    break;
                case 2:
                    // "감시제어반의 점등상태를 확인하고 경보스위치를 복구하여 음향장치의 정상 작동 여부를 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4 });
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, true);
                            ControlPanel.SwitchButtonOnOff(SWITCH.주경종, false);
                        });
                        isActive = ControlPanel.CheckSwitchOff(new int[] { 0, 1, 2, 3, 4 });
                    }
                    break;
                case 3:
                    // "음향장치를 다시 정지하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, false);
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4 });
                            ControlPanel.DisplayLightOn(1, false);

                            ControlPanel.MoveDial(1);
                        });
                        isActive = ControlPanel.CheckSwitchOn(new int[] { 0, 1, 2, 3, 4, 5, 9 });
                    }
                    break;
                case 4:
                    // "회로선택스위치를 2번으로 돌려 감지기 B회로를 동작시키세요.",
                    {
                        FirstLoad(() =>
                        {
                            ControlPanel.InteractiveDial(true);
                            ControlPanel.MoveDial(1);

                            SolenoideValve_Mini.Reset();
                            PopupManager.Instance.CloseAll();
                        });
                        ControlPanel.DisplayLightOn(1, ControlPanel.CheckDial(2));
                        ControlPanel.SwitchButtonOnOff(0, !ControlPanel.CheckDial(2));
                        isActive = ControlPanel.CheckDial(2);
                    }
                    break;
                case 5:
                    // "감시제어반의 점등상태와 솔레노이드밸브의 정상 작동 여부를 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            SolenoideValve_Mini.Reset();
                            SolenoideValve_Mini.StartOpenValve();

                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, true);
                            ControlPanel.SwitchButtonOnOff(SWITCH.주경종, false);
                        });
                        ControlPanel.DisplayLightOn(3, IsOpenSolValve);
                        isActive = IsOpenSolValve;
                    }
                    break;
                case 6:
                    // "감시제어반의 음향장치를 복구하여 정상 작동 여부를 확인하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();

                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, true);
                            ControlPanel.SwitchButtonOnOff(SWITCH.주경종, false);
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4 });
                        });
                        isActive = ControlPanel.CheckSwitchOff(new int[] { 0, 1, 2, 3, 4 });
                    }
                    break;
                case 7:
                    // "음향장치를 다시 정지하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4 });
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저 }, false);

                            StopCoroutine("FillingPumpOn");
                            FillingPumpOff();
                        });
                        isActive = ControlPanel.CheckSwitchOn(new int[] { 0, 1, 2, 3, 4, 5, 9 });
                    }
                    break;
                case 8:
                    // "감시제어반의 충압펌프 압력스위치와 펌프기동 표시등 점등여부를 확인하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            FillingPumpOff();
                            StartCoroutine("FillingPumpOn");
                            ControlPanel.DisplayLightOn(10, false);

                            StopCoroutine("MainPumpOn");
                            MainPumpOff();
                        });

                        PopupManager.Instance.ControlPower(PUMP.충압펌프, true);
                        isActive = ControlPanel.CheckDisplayOn(new int[] { 0, 1, 3, 9, 11 });
                    }
                    break;
                case 9:
                    // "감시제어반의 주펌프 압력스위치와 주펌프 기동표시등 점등여부를 확인하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);

                            MainPumpOff();
                            StartCoroutine("MainPumpOn");
                        });

                        PopupManager.Instance.ControlPower(PUMP.주펌프, true);
                        isActive = ControlPanel.CheckDisplayOn(new int[] { 0, 1, 3, 9, 10 });
                    }
                    break;
                case 10:
                    // "펌프실로 이동하여 펌프의 정상 작동여부를 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            Teleport.Interactive(true, (int)AREA.펌프실);
                            PopupManager.Instance.CloseAll();
                        });
                        if (Teleport.CompareArea(AREA.펌프실))
                        {
                            Hint.OnClickNextButton();
                        }
                    }
                    break;
                case 11:
                    // "펌프 기동 및 동력제어반 기동표시등을 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();
                            PopupManager.Instance.InteractivePowerControl(true);
                        });
                        isActive = PopupManager.Instance.CurrentOpenPopup == POPUP.동력제어반;
                    }
                    break;
                case 12:
                    // "펌프의 정상기동으로 소화수가 개방된 배수밸브를 통해 물이 배수되는 것을 확인하여 동작시험 테스트를 완료하였습니다.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.MultyOpen(new POPUP[] { POPUP.동력제어반, POPUP.배수밸브 });
                        });
                        isActive = true;
                    }
                    break;
                case 13:
                    {
                        PopupManager.Instance.Open(POPUP.완료);
                    }
                    break;
            }
            Hint.ActiveNextButton(isActive);
        }

        void EvaluationOperationTestLevel()
        {
            bool isCorrect = false;
            switch (scenario_index)
            {
                case 0:
                    // "동작시험을 실시하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            SetInspection();
                            LogManager.Instance.Reset();
                        });

                        if(isOperationTest)
                        {
                            ControlPanel.DisplayLightOn(0, true);
                            ControlPanel.PowerOnOff(0, true);
                            ControlPanel.SwitchButtonOnOff(0, false);
                        }

                        if(isOperationTest2)
                        {
                            ControlPanel.DisplayLightOn(1, true);
                            ControlPanel.DisplayLightOn(3, true);
                            ControlPanel.SwitchButtonOnOff(0, false);
                        }

                        if(isOperationTest == false && isOperationTest2 == false)
                        {
                            ControlPanel.DisplayLightOn(0, false);
                            ControlPanel.PowerOnOff(0, false);
                            ControlPanel.DisplayLightOn(1, false);
                            ControlPanel.DisplayLightOn(3, false);
                            ControlPanel.SwitchButtonOnOff(0, true);
                        }

                        ControlPanel.InteractiveDial(ControlPanel.CheckSwitchOn(SWITCH.동작시험));

                        isCorrect = ControlPanel.CheckDisplayOn(new int[] { 0, 1, 3, 9 });
                        EvaluationManager.Instance.Box[5].Success[0] = isCorrect;
                    }
                    break;
                case 1:
                    // "정상작동여부를 확인하고 불량 부분에 모두 체크한 후 다음 버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();

                            SolenoidValve.OpenReset();

                            ControlPanel.Reset();
                            ControlPanel.MoveDial(2);
                            ControlPanel.SwitchButtonOnOff(SWITCH.주경종, false);
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저, SWITCH.축적비축적, SWITCH.동작시험 }, true);
                            ControlPanel.DisplayLightOn(0, true);
                            ControlPanel.DisplayLightOn(1, true);
                            ControlPanel.DisplayLightOn(3, true);
                            ControlPanel.DisplayLightOn(9, true);
                            ControlPanel.DisplayLightOn(10, true);

                            Question.SetALL(new 문제[] { 문제.템퍼스위치, 문제.음향장치 });
                            SoundManager.Instance.MuteAll(true);
                            DisableAllButtons();
                        });
                        isCorrect = Question.CheckAnswer(new 문제[] { 문제.화재표시등, 문제.펌프기동표시등 });
                        EvaluationManager.Instance.Box[5].Success[1] = isCorrect;
                    }
                    break;
                case 2:
                    // "동력제어반의 펌프 기동표시등을 확인하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.펌프실);
                            PopupManager.Instance.CloseAll();
                            PopupManager.Instance.InteractivePowerControl(true);
                            PopupManager.Instance.ControlPower(PUMP.주펌프, true);
                        });
                    }
                    break;
                case 3:
                    // "펌프의 정상기동으로 소화수가 개방된 배수밸브를 통해 물이 배수되는 것을 확인하여 동작시험 테스트를 완료하였습니다. 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.MultyOpen(new POPUP[] { POPUP.배수밸브, POPUP.동력제어반 });
                        });
                    }
                    break;
                case 4:
                    {
                        EvaluationManager.Instance.OpenNext(true);
                    }
                    break;
            }
            Hint.ActiveNextButton(true, isMode);
        }
        #endregion

        #region 복구
        /// <summary>
        /// 복구시나리오 단계
        /// </summary>
        void CheckRecoverStep()
        {
            bool isActive = false;
            switch(scenario_index)
            {
                case 0:
                    // "프리액션밸브 1차측 개폐밸브를 폐쇄하고 다음 버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            SetInspection();
                            Teleport.MoveArea(AREA.유수검지장치실);

                            PreactionValve.ResetValve(프리액션밸브.일차템퍼스위치);
                            PreactionValve.Interactive(프리액션밸브.일차템퍼스위치);
                            PreactionValve.LockValve(프리액션밸브.이차템퍼스위치);

                            PopupManager.Instance.CloseAll();
                            PopupManager.Instance.ControlPowerReset();
                            LogManager.Instance.Reset();
                        });
                        if(PreactionValve.CheckValve(프리액션밸브.일차템퍼스위치, true))
                        {
                            PopupManager.Instance.ResetPopup(POPUP.이차가압계);
                            PopupManager.Instance.ResetPopup(POPUP.일차가압계);
                            PreactionValve.SetWater(false);
                        }
                        else
                        {
                            PopupManager.Instance.FirstGaugeUp();
                            PopupManager.Instance.SecondGaugeUp();
                            PreactionValve.SetWater(true);
                        }
                        isActive = PreactionValve.CheckValve(프리액션밸브.일차템퍼스위치, true);
                    }
                    break;
                case 1:
                    // "충압펌프가 정지된 것을 확인하고 충압펌프 선택스위치를 수동으로 전환하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.펌프실);
                            PopupManager.Instance.Open(POPUP.동력제어반);
                            PopupManager.Instance.ControlPower(PUMP.주펌프, true);
                            PopupManager.Instance.ControlChangeMainPumpState(상태.자동);
                            PopupManager.Instance.ControlChangeFillingPumpState(상태.자동);
                            PopupManager.Instance.ControlFillingPumpStateInteractive(true);
                        });
                        isActive = PopupManager.Instance.ControlFillingPumpStateCheck(상태.수동);
                    }
                    break;
                case 2:
                    // "주펌프를 수동으로 정지하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.펌프실);
                            PopupManager.Instance.Open(POPUP.동력제어반);
                            PopupManager.Instance.ControlPower(PUMP.주펌프, true);
                            PopupManager.Instance.ControlChangeMainPumpState(상태.자동);
                            PopupManager.Instance.ControlPowerStopButtonInteractive(true);
                        });
                        SoundManager.Instance.PlayPump(!PopupManager.Instance.ControlPowerCheck(PUMP.주펌프, 점등.정지));
                        PopupManager.Instance.ControlMainPumpStateCheck(상태.수동);
                        isActive = PopupManager.Instance.ControlPowerCheck(PUMP.주펌프, 점등.정지);
                    }
                    break;
                case 3:
                    // "감시제어반의 점등상태를 확인하고 복구스위치를 눌러 화재를 복구해주세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();
                            Teleport.MoveArea(AREA.감시제어반);

                            ControlPanel.ResetSwitchButtonReset();
                            ControlPanel.InteractiveSwitch(true, (int)SWITCH.복구);
                            ControlPanel.DisplayLightOn(0, true);
                            ControlPanel.DisplayLightOn(1, true);
                            ControlPanel.DisplayLightOn(8, true);
                            ControlPanel.DisplayLightOn(3, true);
                            ControlPanel.PowerOnOff(0, true);
                        });
                        isActive = ControlPanel.CheckSwitchResetButtonPress();
                    }
                    break;
                case 4:
                    // "솔레노이드밸브를 복구하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();
                            Teleport.MoveArea(AREA.유수검지장치실);

                            SolenoidValve.OpenReset();
                            SolenoidValve.StartMaunal();
                            PreactionValve.InteractiveSolenoidValve(true);
                        });
                        ControlPanel.DisplayLightOn(3, IsOpenSolValve);
                        isActive = SolenoidValve.CheckVavle(false);
                    }
                    break;
                case 5:
                    // "프리액션밸브의 배수밸브를 폐쇄하세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.유수검지장치실);
                            ControlPanel.DisplayLightOn(3, false);
                            PreactionValve.CloseAllPopup();
                            PreactionValve.ResetValve(프리액션밸브.배수밸브);
                            PreactionValve.Interactive(프리액션밸브.배수밸브);
                            PreactionValve.ResetValve(프리액션밸브.세팅밸브);
                            PopupManager.Instance.ResetPopup(POPUP.일차가압계);
                            PopupManager.Instance.CloseAll();
                        });
                        isActive = PreactionValve.CheckValve(프리액션밸브.배수밸브, true);
                    }
                    break;
                case 6:
                    // "프리액션밸브의 세팅밸브를 개방하여 중간챔버의 가압수를 공급하세요.",
                    {
                        FirstLoad(() =>
                        {
                            PreactionValve.CloseAllPopup();
                            PreactionValve.Interactive(프리액션밸브.세팅밸브);
                            PreactionValve.ResetValve(프리액션밸브.세팅밸브);

                            PopupManager.Instance.ResetPopup(POPUP.일차가압계);
                            PopupManager.Instance.Open(POPUP.일차가압계);
                        });
                        if(PreactionValve.CheckValve(프리액션밸브.세팅밸브, true))
                        {
                            PopupManager.Instance.FirstGaugeUp();
                        }
                        else
                        {
                            PopupManager.Instance.ResetPopup(POPUP.일차가압계);
                        }
                        isActive = PreactionValve.CheckValve(프리액션밸브.세팅밸브, true);
                    }
                    break;
                case 7:
                    // "1차측 개폐밸브를 서서히 개방하면서 2차측 압력계의 상승여부를 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.Open(POPUP.이차가압계);

                            PreactionValve.CloseAllPopup();
                            PreactionValve.LockValve(프리액션밸브.일차템퍼스위치);
                            PreactionValve.Interactive(프리액션밸브.일차템퍼스위치);
                            PreactionValve.LockValve(프리액션밸브.세팅밸브);
                        });
                        isActive = PreactionValve.CheckValve(프리액션밸브.일차템퍼스위치, false);
                    }
                    break;
                case 8:
                    // "세팅밸브를 폐쇄하세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();
                            PreactionValve.CloseAllPopup();

                            PreactionValve.LockValve(프리액션밸브.세팅밸브);
                            PreactionValve.Interactive(프리액션밸브.세팅밸브);
                        });
                        isActive = PreactionValve.CheckValve(프리액션밸브.세팅밸브, false);
                    }
                    break;
                case 9:
                    // "프리액션밸브의 2차측 개폐밸브를 서서히 개방하세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.유수검지장치실);
                            PreactionValve.CloseAllPopup();

                            PreactionValve.LockValve(프리액션밸브.이차템퍼스위치);
                            PreactionValve.Interactive(프리액션밸브.이차템퍼스위치);

                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저, SWITCH.축적비축적 }, true);
                        });
                        isActive = PreactionValve.CheckValve(프리액션밸브.이차템퍼스위치, false);
                    }
                    break;
                case 10:
                    // "프리액션밸브의 1,2차측 탬퍼스위치 소등상태를 확인하고 감시제어반을 모두 정상으로 복구하세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);

                            ControlPanel.DisplayLightOn(8, false);
                            ControlPanel.DisplayLightOn(9, false);
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저, SWITCH.축적비축적 }, true);
                            ControlPanel.InteractiveSwitch(true, new int[] { 0, 1, 2, 3, 4, 5 });
                        });
                        isActive = ControlPanel.CheckSwitchOff(new int[] { 0, 1, 2, 3, 4, 5 });
                    }
                    break;
                case 11:
                    // "감시제어반의 정상상태를 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            PopupManager.Instance.CloseAll();
                        });
                        isActive = true;
                    }
                    break;
                case 12:
                    // "충압펌프와 주펌프 선택스위치를 자동으로 전환하고 펌프가 기동되지 않은 것을 확인하세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.펌프실);
                            PopupManager.Instance.Open(POPUP.동력제어반);
                            PopupManager.Instance.ControlChangeFillingPumpState(상태.수동);
                            PopupManager.Instance.ControlChangeMainPumpState(상태.수동);
                            PopupManager.Instance.ControlFillingPumpStateInteractive(true);
                            SoundManager.Instance.PlayPump(false);
                        });
                        PopupManager.Instance.ControlPowerStopButtonInteractive(PopupManager.Instance.ControlFillingPumpStateCheck(상태.자동));
                        isActive = PopupManager.Instance.ControlMainPumpStateCheck(상태.자동) && PopupManager.Instance.ControlFillingPumpStateCheck(상태.자동);
                    }
                    break;
                case 13:
                    {
                        PopupManager.Instance.Open(POPUP.완료);
                    }
                    break;
            }
            Hint.ActiveNextButton(isActive);
        }

        [Header("정상/복구")]
        [SerializeField] ToggleGroup ToggleGroup;

        void EvaluationRecoverLevel()
        {
            bool isCorrect = false;
            switch (scenario_index)
            {
                case 0:
                    // "프리액션밸브의 1차측 개폐밸브를 폐쇄하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            SetInspection();
                            Teleport.MoveArea(AREA.유수검지장치실);

                            PreactionValve.Reset();
                            PreactionValve.LockValve(프리액션밸브.이차템퍼스위치);

                            SolenoidValve.OpenReset();
                            SolenoidValve.Interactive(true);
                            PreactionValve.InteractiveSolenoidValve(true);

                            LogManager.Instance.Reset();
                        });
                        if(PreactionValve.CheckValve(프리액션밸브.일차템퍼스위치, true))
                        {
                            PopupManager.Instance.ResetPopup(POPUP.이차가압계);
                            PopupManager.Instance.ResetPopup(POPUP.일차가압계);
                            PreactionValve.SetWater(false);
                        }
                        else
                        {
                            PopupManager.Instance.SecondGaugeUp();
                            PopupManager.Instance.FirstGaugeUp();
                            PreactionValve.SetWater(true);
                        }

                        isCorrect = PreactionValve.CheckValve(프리액션밸브.이차템퍼스위치, true) && PreactionValve.CheckValve(프리액션밸브.배수밸브, false) &&
                            PreactionValve.CheckValve(프리액션밸브.일차템퍼스위치, true, true) && PreactionValve.CheckValve(프리액션밸브.세팅밸브, false);
                        EvaluationManager.Instance.Box[6].Success[0] = isCorrect;
                    }
                    break;
                case 1:
                    // "충압펌프가 정지된 것을 확인하고 충압펌프 선택스위치를 수동으로 전환하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.펌프실);
                            PopupManager.Instance.CloseAll();
                            PopupManager.Instance.ResetPopup(POPUP.이차가압계);
                            PopupManager.Instance.ResetPopup(POPUP.일차가압계);
                            PopupManager.Instance.Open(POPUP.동력제어반);
                            PopupManager.Instance.ControlPower(PUMP.주펌프, true);
                            PopupManager.Instance.ControlChangeFillingPumpState(상태.자동);
                            PopupManager.Instance.ControlFillingPumpStateInteractive(true);
                        });
                    }
                    break;
                case 2:
                    // "주펌프를 수동으로 정지하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.ControlChangeFillingPumpState(상태.수동);
                            PopupManager.Instance.ControlPower(PUMP.충압펌프, false);
                            PopupManager.Instance.ControlPower(PUMP.주펌프, true);
                            PopupManager.Instance.ControlPowerStopButtonInteractive(true);
                        });
                        SoundManager.Instance.PlayPump(!PopupManager.Instance.ControlPowerCheck(PUMP.주펌프, 점등.정지));
                        isCorrect = PopupManager.Instance.ControlPowerCheck(PUMP.주펌프, 점등.정지);
                        EvaluationManager.Instance.Box[6].Success[1] = isCorrect;
                    }
                    break;
                case 3:
                    // "감시제어반의 점등상태를 확인하고 복구스위치를 눌러 화재를 복구해주세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            PopupManager.Instance.CloseAll();
                            ControlPanel.Reset();
                            SetInspection();

                            ControlPanel.DisplayLightOn(0, true);
                            ControlPanel.DisplayLightOn(1, true);
                            ControlPanel.DisplayLightOn(8, true);
                            ControlPanel.DisplayLightOn(3, true);
                        });

                        isCorrect = ControlPanel.CheckDisplayOn(8) && ControlPanel.CheckDisplayOn(9) &&
                            ControlPanel.CheckSwitchOn(new int[] { 0, 1, 2, 3, 4, 5 }) && ControlPanel.CheckTools(new int[] { 0, 0, 0 }) &&
                            !ControlPanel.CheckDisplayOn(0) && !ControlPanel.CheckDisplayOn(1);
                        EvaluationManager.Instance.Box[6].Success[2] = isCorrect;
                    }
                    break;
                case 4:
                    // "솔레노이드밸브를 복구하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.유수검지장치실);
                            PopupManager.Instance.CloseAll();

                            PreactionValve.Reset();
                            PreactionValve.LockValve(프리액션밸브.이차템퍼스위치);
                            PreactionValve.LockValve(프리액션밸브.일차템퍼스위치);

                            SolenoidValve.OpenReset();
                            PreactionValve.InteractiveSolenoidValve(true);
                            SolenoidValve.Interactive(true);
                        });
                        isCorrect = IsOpenSolValve == false && PreactionValve.CheckValve(프리액션밸브.이차템퍼스위치, true) &&
                            PreactionValve.CheckValve(프리액션밸브.배수밸브, false) && PreactionValve.CheckValve(프리액션밸브.일차템퍼스위치, true) &&
                            PreactionValve.CheckValve(프리액션밸브.세팅밸브, false);
                        EvaluationManager.Instance.Box[6].Success[3] = isCorrect;
                    }
                    break;
                case 5:
                    // "정상작동여부를 확인하고 불량 부분에 모두 체크한 후 다음 버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.감시제어반);
                            PopupManager.Instance.CloseAll();
                            ControlPanel.Reset();
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저, SWITCH.축적비축적 }, true);
                            ControlPanel.DisplayLightOn(3, true);

                            Question.SetQuestion(new 문제[] { 문제.감지기동작표시등, 문제.밸브개방표시등, 문제.화재표시등, 문제.템퍼스위치 });
                            SoundManager.Instance.MuteAll(true);
                            DisableAllButtons();
                        });
                        SoundManager.Instance.PlayAlarm2(!ControlPanel.CheckSwitchOn(SWITCH.지구경종));
                        SoundManager.Instance.PlayBroadcast(!ControlPanel.CheckSwitchOn(SWITCH.비상방송));
                        isCorrect = Question.CheckAnswer(new 문제[] { 문제.밸브개방표시등, 문제.템퍼스위치 });
                        EvaluationManager.Instance.Box[6].Success[4] = isCorrect;
                    }
                    break;
                case 6:
                    // "배수밸브를 폐쇄하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.유수검지장치실);

                            PreactionValve.Reset();
                            PreactionValve.LockValve(프리액션밸브.이차템퍼스위치);
                            PreactionValve.LockValve(프리액션밸브.일차템퍼스위치);

                            SolenoidValve.Reset();
                            SolenoidValve.Interactive(true);
                            PreactionValve.InteractiveSolenoidValve(true);
                        });
                        isCorrect = IsOpenSolValve == false && PreactionValve.CheckValve(프리액션밸브.이차템퍼스위치, true) &&
                            PreactionValve.CheckValve(프리액션밸브.배수밸브, true, true) && PreactionValve.CheckValve(프리액션밸브.일차템퍼스위치, true) &&
                            PreactionValve.CheckValve(프리액션밸브.세팅밸브, false);
                        EvaluationManager.Instance.Box[6].Success[5] = isCorrect;
                    }
                    break;
                case 7:
                    // "세팅밸브를 개방하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();
                            PreactionValve.CloseAllPopup();

                            PreactionValve.Reset();
                            PreactionValve.LockValve(프리액션밸브.이차템퍼스위치);
                            PreactionValve.LockValve(프리액션밸브.일차템퍼스위치);
                            PreactionValve.LockValve(프리액션밸브.배수밸브);

                            PopupManager.Instance.MultyOpen(new POPUP[] { POPUP.일차가압계, POPUP.이차가압계 });

                            SolenoidValve.Reset();
                            SolenoidValve.Interactive(true);
                            PreactionValve.InteractiveSolenoidValve(true);
                        });
                        if (PreactionValve.CheckValve(프리액션밸브.세팅밸브, true))
                        {
                            PopupManager.Instance.FirstGaugeUp();
                        }
                        else
                        {
                            PopupManager.Instance.ResetPopup(POPUP.일차가압계);
                        }

                        isCorrect = IsOpenSolValve == false && PreactionValve.CheckValve(프리액션밸브.이차템퍼스위치, true) &&
                            PreactionValve.CheckValve(프리액션밸브.배수밸브, true) && PreactionValve.CheckValve(프리액션밸브.일차템퍼스위치, true) &&
                            PreactionValve.CheckValve(프리액션밸브.세팅밸브, true);
                        EvaluationManager.Instance.Box[6].Success[6] = isCorrect;
                    }
                    break;
                case 8:
                    // "1차측 개폐밸브를 개방하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();
                            PreactionValve.CloseAllPopup();

                            PreactionValve.Reset();
                            PreactionValve.LockValve(프리액션밸브.이차템퍼스위치);
                            PreactionValve.LockValve(프리액션밸브.일차템퍼스위치);
                            PreactionValve.LockValve(프리액션밸브.배수밸브);
                            PreactionValve.LockValve(프리액션밸브.세팅밸브);
                            PopupManager.Instance.FirstGaugeUp();

                            SolenoidValve.Reset();
                            SolenoidValve.Interactive(true);
                            PreactionValve.InteractiveSolenoidValve(true);

                            PopupManager.Instance.MultyOpen(new POPUP[] { POPUP.일차가압계, POPUP.이차가압계 });
                        });
                        PreactionValve.CheckValve(프리액션밸브.일차템퍼스위치, false, true);
                    }
                    break;
                case 9:
                    // "정상 복구 여부를 선택하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PreactionValve.CloseAllPopup();
                            DisableAllButtons();
                            PreactionValve.ResetValve(프리액션밸브.일차템퍼스위치);
                            ToggleGroup.gameObject.SetActive(true);
                            if (ToggleGroup.AnyTogglesOn())
                            {
                                ToggleGroup.SetAllTogglesOff();
                            }
                        });
                        isCorrect = isNormalToggle;
                        EvaluationManager.Instance.Box[6].Success[7] = isCorrect;
                    }
                    break;
                case 10:
                    // "세팅밸브를 폐쇄하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();
                            ToggleGroup.gameObject.SetActive(false);
                            PreactionValve.CloseAllPopup();

                            PreactionValve.Reset();
                            PreactionValve.LockValve(프리액션밸브.이차템퍼스위치);
                            PreactionValve.LockValve(프리액션밸브.배수밸브);
                            PreactionValve.LockValve(프리액션밸브.세팅밸브);

                            SolenoidValve.Reset();
                            SolenoidValve.Interactive(true);
                            PreactionValve.InteractiveSolenoidValve(true);
                        });
                        isCorrect = IsOpenSolValve == false && PreactionValve.CheckValve(프리액션밸브.이차템퍼스위치, true) &&
                            PreactionValve.CheckValve(프리액션밸브.배수밸브, true) && PreactionValve.CheckValve(프리액션밸브.일차템퍼스위치, false) &&
                            PreactionValve.CheckValve(프리액션밸브.세팅밸브, false);
                        EvaluationManager.Instance.Box[6].Success[8] = isCorrect;
                    }
                    break;
                case 11:
                    // "프리액션밸브의 2차측 개폐밸브를 서서히 개방하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();
                            PreactionValve.CloseAllPopup();

                            PreactionValve.Reset();
                            PreactionValve.LockValve(프리액션밸브.이차템퍼스위치);
                            PreactionValve.LockValve(프리액션밸브.배수밸브);

                            SolenoidValve.Interactive(true);
                            SolenoidValve.Reset();
                            PreactionValve.InteractiveSolenoidValve(true);
                        });
                        isCorrect = IsOpenSolValve == false && PreactionValve.CheckValve(프리액션밸브.이차템퍼스위치, false, true) &&
                            PreactionValve.CheckValve(프리액션밸브.배수밸브, true) && PreactionValve.CheckValve(프리액션밸브.일차템퍼스위치, false) &&
                            PreactionValve.CheckValve(프리액션밸브.세팅밸브, false);
                        EvaluationManager.Instance.Box[6].Success[9] = isCorrect;
                    }
                    break;
                case 12:
                    // "제어반을 모두 정상상태로 전환하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            PopupManager.Instance.CloseAll();

                            Teleport.MoveArea(AREA.감시제어반);
                            ControlPanel.Reset();
                            ControlPanel.SwitchButtonOnOff(new SWITCH[] { SWITCH.주경종, SWITCH.지구경종, SWITCH.사이렌, SWITCH.비상방송, SWITCH.부저, SWITCH.축적비축적 }, true);
                        });
                        isCorrect = ControlPanel.CheckSwitchOff(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }) &&
                            ControlPanel.CheckTools(new int[] { 0, 0, 0 });
                        EvaluationManager.Instance.Box[6].Success[10] = isCorrect;
                    }
                    break;
                case 13:
                    // "충압펌프와 주펌프를 자동으로 전환하고 다음버튼을 누르세요.",
                    {
                        FirstLoad(() =>
                        {
                            Teleport.MoveArea(AREA.펌프실);
                            PopupManager.Instance.Open(POPUP.동력제어반);

                            PopupManager.Instance.ControlChangeFillingPumpState(상태.수동);
                            PopupManager.Instance.ControlChangeMainPumpState(상태.수동);

                            PopupManager.Instance.ControlPower(PUMP.주펌프, false);
                            PopupManager.Instance.ControlPower(PUMP.충압펌프, false);

                            PopupManager.Instance.ControlFillingPumpStateInteractive(true);
                            PopupManager.Instance.ControlPowerStopButtonInteractive(true);

                            SoundManager.Instance.PlayPump(false);
                        });
                    }
                    break;
                case 14:
                    // "프리액션밸브 복구를 완료하였습니다.",
                    {
                        FirstLoad(() =>
                        {
                            EvaluationManager.Instance.OpenResult(true);
                        });
                    }
                    break;
            }
            Hint.ActiveNextButton(true, isMode);
        }

        bool isNormalToggle = false;
        public void OnClickNormalToggle(bool isOn)
        {
            isNormalToggle = isOn;
            CheckScenarioStep();
        }

        public void OnClickResetButton()
        {
            ControlPanel.DisplayLightOn(0, false);
            ControlPanel.DisplayLightOn(1, false);
            if(IsMode == false)
            {
                ControlPanel.DisplayLightOn(3, false);
            }
            ControlPanel.PowerOnOff(0, false);
            CheckScenarioStep();
        }
        #endregion
    }
}

