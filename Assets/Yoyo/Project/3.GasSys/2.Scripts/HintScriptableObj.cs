using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HintPopupInfo", menuName = "KFSI/HintPopupInfo")]
public class HintScriptableObj : ScriptableObject
{
    public List<HintTextAndAudio> hintData;
}