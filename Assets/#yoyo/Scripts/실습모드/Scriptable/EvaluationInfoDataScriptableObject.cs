using UnityEngine;

[CreateAssetMenu(fileName = "InfoData", menuName = "KKH/ScriptableObjects/EvInfoData", order = 1)]
public class EvaluationInfoDataScriptableObject : ScriptableObject
{
    public string[] str_Title;
    public string[] str_infodatas;

    public AudioClip[] audioClip;
}
