using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RTypeRModePanel : MonoBehaviour
{
    [SerializeField] private Button manualBtn;
    [SerializeField] private Button stopBtn;
    [SerializeField] private Button autoBtn;

    [SerializeField] private GameObject manualObj;
    [SerializeField] private GameObject stopObj;
    [SerializeField] private GameObject autoObj;

    private UnityAction<ControlMode> _controlAction;
    private ControlMode _curMode;

    private void Awake()
    {
        //Init(ControlMode.Auto);
    }

    public void Init(ControlMode mode, UnityAction<ControlMode> action = null)
    {
        manualBtn.onClick.RemoveAllListeners();
        stopBtn.onClick.RemoveAllListeners();
        autoBtn.onClick.RemoveAllListeners();
        manualBtn.onClick.AddListener(() =>
        {
            SetMode(ControlMode.Manual);
        });
        stopBtn.onClick.AddListener(() =>
        {
            SetMode(ControlMode.Stop);
        });
        autoBtn.onClick.AddListener(() =>
        {
            SetMode(ControlMode.Auto);
        });
        
        SetMode(mode);
        SetModeChangeAction(action);
    }

    public void SetModeChangeAction(UnityAction<ControlMode> action)
    {
        _controlAction = action;
    }

    public void SetMode(ControlMode mode)
    {
        _curMode = mode;
        switch (_curMode)
        {
            case ControlMode.Auto:
                UpdateModeImage(autoObj);
                break;
            case ControlMode.Stop:
                UpdateModeImage(stopObj);
                break;
            case ControlMode.Manual:
                UpdateModeImage(manualObj);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _controlAction?.Invoke(_curMode);
    }

    private void UpdateModeImage(GameObject obj)
    {
        manualObj.SetActive(manualObj.Equals(obj));
        stopObj.SetActive(stopObj.Equals(obj));
        autoObj.SetActive(autoObj.Equals(obj));
    }

    public Button GetManualBtn()
    {
        return manualBtn;
    }

    public Button GetStopBtn()
    {
        return stopBtn;
    }

    public Button GetAutoBtn()
    {
        return autoBtn;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
