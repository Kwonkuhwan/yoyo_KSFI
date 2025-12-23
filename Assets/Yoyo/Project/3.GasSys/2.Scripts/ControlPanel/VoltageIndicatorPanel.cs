using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoltageIndicatorPanel : MonoBehaviour
{
    public enum VoltageState
    {
        Low,
        Default,
        Height,
    }
    
    [SerializeField] private GameObject lowCheck;
    [SerializeField] private GameObject defaultCheck;
    [SerializeField] private GameObject heightCheck;


    public void Init(VoltageState voltageState)
    {
        ShowCheck(voltageState);
    }

    public void ShowCheck(VoltageState voltageState)
    {
        switch (voltageState)
        {
            case VoltageState.Low:
                ShowCheck(lowCheck);
                break;
            case VoltageState.Default:
                ShowCheck(defaultCheck);
                break;
            case VoltageState.Height:
                ShowCheck(heightCheck);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(voltageState), voltageState, null);
        }
       
    }

    private void ShowCheck(GameObject obj)
    {
        lowCheck.SetActive(lowCheck.Equals(obj));
        defaultCheck.SetActive(defaultCheck.Equals(obj));
        heightCheck.SetActive(heightCheck.Equals(obj));
    }
    
}
