using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerPanel : MonoBehaviour
{
    [SerializeField] public TimeNumPanel numPanel;
    [SerializeField] private ConfirmationPanel confirmationPanel;

    public void Init(bool isCheck = false)
    {
        numPanel.Init();
        confirmationPanel.InitTwoLine(isCheck);
    }

    public void OnCheck(bool isOn)
    {
        confirmationPanel.OnCheck(isOn);
    }

    public void SetTimeNum(float timeNum)
    {
        numPanel.SetTime(timeNum);
    }

    public void StartTimer(float time)
    {
        numPanel.SetTime(time);
        numPanel.StartStopwatch();
    }
    
    public bool IsRunning()
    {
        return numPanel.IsRunning();
    }

    public void ResetTimer()
    {
        numPanel.ResetStopwatch();
    }

}
