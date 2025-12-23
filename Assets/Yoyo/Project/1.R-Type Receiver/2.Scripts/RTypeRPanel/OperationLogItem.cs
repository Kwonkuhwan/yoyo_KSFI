using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OperationLogItem : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI dateText;
    [SerializeField] public TextMeshProUGUI timeText;
    [SerializeField] public TextMeshProUGUI infoText;

    public void Init(string time, string info)
    {
        SetInfo(time, info);
     
    }
    
    public void SetInfo(string time, string info)
    {
        timeText.text = time;
        infoText.text = info;
    }

    public void SetDate(string date)
    {
        dateText.text = date;
    }
    
}
