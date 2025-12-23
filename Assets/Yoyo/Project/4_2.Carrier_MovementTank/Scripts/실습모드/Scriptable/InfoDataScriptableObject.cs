using UnityEngine;

[CreateAssetMenu(fileName = "InfoData", menuName = "KKH/ScriptableObjects/InfoData", order = 1)]
public class InfoDataScriptableObject : ScriptableObject
{
    public string str_Title;
    public string[] str_infodatas;

    public AudioClip[] audioClip;
}
