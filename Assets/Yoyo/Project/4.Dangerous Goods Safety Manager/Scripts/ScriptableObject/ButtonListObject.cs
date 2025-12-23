using System;
using UnityEngine;


[CreateAssetMenu(fileName = "ButtonList", menuName = "ScriptableObjects/ButtonListObject\r\n", order = 6)]
public class ButtonListObject : ScriptableObject
{
   public ButtonList[] buttonLists;

    private void OnValidate()
    {
        for (int i = 0; i < buttonLists.Length; i++)
        {
            buttonLists[i].index = i;
        }
    }
}
[Serializable]
public class ButtonList
{
    public int index;
    public string[] buttonNames;
}
