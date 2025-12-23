using System;
using TMPro;
using UnityEngine;

public class LogTextObj : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI logText;

    public void SetText(string text)
    {
        logText.text = $"<size=9>{DateTime.Now:yy-MM-dd HH:mm:ss}</size>: {text}";
    }
}
