using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ControlModePanel : MonoBehaviour
{
    
    [SerializeField] private Button selectBtn;
    [SerializeField] private Button activateBtn;

    [SerializeField] private GameObject autoImg;
    [SerializeField] private GameObject stopImg;
    [SerializeField] private GameObject manualImg;

    [SerializeField] private GameObject autoObj;
    [SerializeField] private GameObject stopObj;
    [SerializeField] private GameObject manualObj;

    [SerializeField] private TextMeshProUGUI titleText;

    [SerializeField, Multiline] private string strTitle;

    [SerializeField] private bool isStopMode = true;
    [SerializeField] private bool isActivateButton = true;

    //private CompositeDisposable _btnDisposable = new CompositeDisposable(); 


    private UnityAction<ControlMode> _controlAction;
    private ControlMode _curMode;

    public void Init(ControlMode mode = ControlMode.Auto, UnityAction<ControlMode> action = null, UnityAction activateAction=null)
    {
        SetModeChangeAction(action);
        stopObj.SetActive(isStopMode);

        
        /*
        _btnDisposable?.Clear();

        var selectDisposable = selectBtn.OnClickAsObservable().Subscribe(_ =>
        {
            SwitchMode();
        }).AddTo(this);
        
        var activateDisposable = activateBtn.OnClickAsObservable().Subscribe(_ =>
        {
            ControlPanel.Instance.SetReceiverLog($"{strTitle} 기동");
            activateAction?.Invoke();
        }).AddTo(this);
        
        _btnDisposable.Add(selectDisposable);
        _btnDisposable.Add(activateDisposable);
        */
        
        
        selectBtn.onClick.RemoveAllListeners();
        selectBtn.onClick.AddListener(SwitchMode);

        SetActivateBtn(activateAction);
        

        activateBtn.gameObject.SetActive(isActivateButton);
        activateBtn.interactable = false;
        SetMode(mode);

        //UpdateModeImage(autoImg);
    }

    public void SetMode(ControlMode mode)
    {
        _curMode = mode;
        switch (_curMode)
        {

            case ControlMode.Auto:
                //ControlPanel.Instance.SetReceiverLog($"{titleText.text} 자동");
                UpdateModeImage(autoImg);
                activateBtn.interactable = false;
                break;
            case ControlMode.Stop:
                //ControlPanel.Instance.SetReceiverLog($"{titleText.text} 정지");
                UpdateModeImage(stopImg);
                activateBtn.interactable = false;
                break;
            case ControlMode.Manual:
                //ControlPanel.Instance.SetReceiverLog($"{titleText.text} 수동");
                UpdateModeImage(manualImg);
                activateBtn.interactable = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetModeChangeAction(UnityAction<ControlMode> action)
    {
        _controlAction = action;
    }

    public void SetActivateBtn(UnityAction action)
    {
        activateBtn.onClick.RemoveAllListeners();
        activateBtn.onClick.AddListener(delegate
        {
            ControlPanel.Instance.SetReceiverLog($"{strTitle} 기동");
            action?.Invoke();
        });
    }

    private void SwitchMode()
    {
        if (isStopMode)
        {
            switch (_curMode)
            {
                // 현재 모드를 확인하고 순차적으로 변경
                case ControlMode.Auto:
                    _curMode = ControlMode.Stop;
                    ControlPanel.Instance.SetReceiverLog($"{titleText.text} 정지");
                    UpdateModeImage(stopImg);
                    activateBtn.interactable = false;
                    break;
                case ControlMode.Stop:
                    _curMode = ControlMode.Manual;
                    ControlPanel.Instance.SetReceiverLog($"{titleText.text} 수동");
                    UpdateModeImage(manualImg);
                    //activateBtn.interactable = true;
                    break;
                case ControlMode.Manual:
                default:
                    _curMode = ControlMode.Auto;
                    ControlPanel.Instance.SetReceiverLog($"{titleText.text} 자동");
                    UpdateModeImage(autoImg);
                    //activateBtn.interactable = false;
                    break;
            }
        }
        else
        {
            switch (_curMode)
            {
                // 현재 모드를 확인하고 순차적으로 변경
                case ControlMode.Auto:
                    _curMode = ControlMode.Manual;
                    ControlPanel.Instance.SetReceiverLog($"{titleText.text} 수동");
                    UpdateModeImage(manualImg);
                    //activateBtn.interactable = true;
                    break;
                case ControlMode.Stop:
                case ControlMode.Manual:
                default:
                    _curMode = ControlMode.Auto;
                    ControlPanel.Instance.SetReceiverLog($"{titleText.text} 자동");
                    UpdateModeImage(autoImg);
                    //activateBtn.interactable = false;
                    break;
            }
        }
        _controlAction?.Invoke(_curMode);
        // 변경된 모드를 표시

    }

    public ControlMode GetMode()
    {
        return _curMode;
    }

    private void UpdateModeImage(GameObject obj)
    {
        autoImg.SetActive(autoImg.Equals(obj));
        stopImg.SetActive(stopImg.Equals(obj));
        manualImg.SetActive(manualImg.Equals(obj));
    }

    public Button GetSelectBtn()
    {
        return selectBtn;
    }

    public Button GetActivateBtn()
    {
        return activateBtn;
    }
    

}
