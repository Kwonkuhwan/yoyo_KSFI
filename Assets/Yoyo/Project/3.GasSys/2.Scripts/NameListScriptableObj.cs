using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NameList", menuName = "KFSI/NameList")]
public class NameListScriptableObj : ScriptableObject
{
    [Multiline]
    public string[] korNames;
    [Multiline]
    public string[] engNames;
}
