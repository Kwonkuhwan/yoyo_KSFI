using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SMW.Sprinkler
{
    public class LogText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI logText;

        public void SetText(string text)
        {
            logText.text = $"<size=9>{DateTime.Now:yy-MM-dd HH:mm:ss}</size>: {text}";
        }
    }
}
