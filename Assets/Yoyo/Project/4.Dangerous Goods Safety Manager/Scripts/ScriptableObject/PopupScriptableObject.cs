using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PopupData", menuName = "ScriptableObjects/PopupScriptableObject", order = 1)]
public class PopupScriptableObject : ScriptableObject
{
    public Popup[] popups;

    private void OnValidate()
    {
        for (int i = 0; i < popups.Length; i++)
        {
            popups[i].index = i;
        }
    }
}

[Serializable]
public class Popup
{
    public string titile;
    [TextArea]
    public string description;
    public bool isSingleExample;
    public bool isbackGroundChange = false;
    public int backGroundNumber;
    public Sprite exampleOne;
    public Sprite exampleTwo;
    public Sprite exampleThree;
    public int popupBgnumber = 0;
    public int index;
}
