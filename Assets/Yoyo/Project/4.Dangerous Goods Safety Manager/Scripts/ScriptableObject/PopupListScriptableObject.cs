
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PopupListData", menuName = "ScriptableObjects/PopupListScriptableObject", order = 7)]
public class PopupListScriptableObject : ScriptableObject
{
    public PopupList[] popupLists;

    private void OnValidate()
    {
        for (int i = 0; i < popupLists.Length; i++)
        {
            popupLists[i].listIndex = i;
            for(int j = 0; j < popupLists[i].popups.Length; j++)
            {
                popupLists[i].popups[j].index = j;
            }
        }
    }
}

[Serializable]
public class PopupList
{
    public int listIndex;
    public Popup[] popups;
    
}
