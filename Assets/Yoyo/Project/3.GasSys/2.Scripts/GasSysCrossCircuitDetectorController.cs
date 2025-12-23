using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GasSysCrossCircuitDetectorController : MonoBehaviour
{
    [SerializeField] private UIDragAndCollisionHandler dragAndCollisionHandler;
    [SerializeField] private Button area1Btn;
    [SerializeField] private Button area2Btn;
    [SerializeField] private Button chemicalStorageRoomBtn;
    [SerializeField] private SmokeDetectorPanel smokeDetectorPanel;
    [SerializeField] private HeatDetectorPanel heatDetectorPanel;
    [SerializeField] private ActivationCylinderBox activationCylinderBox;
    private ControlMode _curMode = ControlMode.Stop;
    private CompositeDisposable _disposable = new CompositeDisposable();
    
    public void Init()
    {
        //dragAndCollisionHandler.OnCollisionDetected -= HandleCollision;
        dragAndCollisionHandler.ResetEvent();
        dragAndCollisionHandler.OnCollisionDetected += HandleCollision;
        
        _curMode = ControlMode.Stop;
        ControlPanel.Instance.InitCrossCircuitDetector();
        ControlPanel.Instance.ShowPanel(false);
        _disposable?.Clear();
        
        var area1Disposable = area1Btn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ControlPanel.Instance.ShowPanel(true);
                
            }).AddTo(this);
        _disposable?.Add(area1Disposable);
        
        var area2Disposable = area2Btn.OnClickAsObservable()
            .Subscribe(_ =>
            {

            }).AddTo(this);
        _disposable?.Add(area2Disposable);
        
        var chemicalStorageRoomDisposable = chemicalStorageRoomBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {

            }).AddTo(this);
        _disposable?.Add(chemicalStorageRoomDisposable);
        
        var disposable = ControlPanel.Instance.onSwitchBtnValueChangeEvent.AsObservable()
            .Subscribe(data =>
            {
                Debug.Log($"{data.Item1}, {data.Item2}");
                switch (data.Item1)
                {
                    case "주경종":
                        {
                            string isOn = !data.Item2 ? "On" : "Off";
                            ControlPanel.Instance.SetReceiverLog($"{data.Item1} 음향 {isOn}");
                            break;
                        }
                    case "지구경종":
                        {
                            string isOn = !data.Item2 ? "On" : "Off";
                            ControlPanel.Instance.SetReceiverLog($"{data.Item1} 음향 {isOn}");
                            break;
                        }
                    case "사이렌":
                        {
                            string isOn = !data.Item2 ? "On" : "Off";
                            if (_curMode.Equals(ControlMode.Auto))
                            {
                                ControlPanel.Instance.SetReceiverLog($"{data.Item1} 음향 {isOn}");
                                if(data.Item2)
                                    activationCylinderBox.InitCrossCircuitDetector();
                            }
                            break;
                        }
                    case "비상방송":
                        {
                            string isOn = !data.Item2 ? "On" : "Off";
                            ControlPanel.Instance.SetReceiverLog($"{data.Item1} 음향 {isOn}");
                            if(data.Item2)
                                heatDetectorPanel.gameObject.SetActive(true);
                            break;
                        }
                }
            }).AddTo(this);
        _disposable?.Add(disposable);
        
        var timeEnd = ControlPanel.OnTimerEnd.AsObservable().Subscribe(_ =>
        {
            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
            activationCylinderBox.SetSolenoidValveActivationImg(true);
        }).AddTo(this);
        _disposable?.Add(timeEnd);
        ControlPanel.Instance.SetSolenoidValveModeAndActivateBtn(mode =>
        {
            _curMode = mode;
            if (!_curMode.Equals(ControlMode.Auto))
                return;
            //ControlPanel.Instance.StartTimer(30f);
            smokeDetectorPanel.gameObject.SetActive(true);

        }, null);
    }

    public void OnDisable()
    {
        dragAndCollisionHandler.OnCollisionDetected -= HandleCollision;
        _disposable?.Clear();

    }

    private void HandleCollision(GameObject draggedObject, GameObject targetObject)
    {
        Debug.Log($"Collision Detected: {draggedObject.name} with {targetObject.name}");
        switch (targetObject.name)
        {
            case "연기감지기":
                smokeDetectorPanel.SetActivate(true);
                smokeDetectorPanel.gameObject.SetActive(false);
                ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                ControlPanel.Instance.ShowFire(true);
                ControlPanel.Instance.SetTimeNum(30f);
                heatDetectorPanel.gameObject.SetActive(true);
                break;
            case "열감지기":
                heatDetectorPanel.SetActivate(true);
                heatDetectorPanel.gameObject.SetActive(false);
                ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, true);
                ControlPanel.Instance.ShowFire(true);
                ControlPanel.Instance.SetTimeNum(30f);
                ControlPanel.Instance.StartTimer(30f);
                activationCylinderBox.InitCrossCircuitDetector();
                break;
        }
        // if (draggedObject.name.Equals("할로겐열시험기") && targetObject.name.Equals("열감지기"))
        // {
        //     Debug.Log($"Collision Detected: {draggedObject.name} with {targetObject.name}");
        // }
        // if (draggedObject.name.Equals("연기감지기 점검용 스프레이") && targetObject.name.Equals("연기감지기"))
        // {
        //     Debug.Log($"Collision Detected: {draggedObject.name} with {targetObject.name}");
        // }

        // 추가적인 이벤트 처리 로직 (예: 애니메이션, 소리, 상태 변화 등)
    }

}
