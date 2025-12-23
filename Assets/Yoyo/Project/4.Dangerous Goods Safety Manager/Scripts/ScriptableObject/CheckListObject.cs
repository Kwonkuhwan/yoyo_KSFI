using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckList", menuName = "ScriptableObjects/CheckListObject\r\n", order = 4)]
public class CheckListObject : ScriptableObject
{
    public CheckList[] checkList;
}


[Serializable]
public class CheckList
{
    public string title;
    public Sprite sprite;
    public Sprite subSprite;
    [TextArea]
    public string[] subTitle;
    [TextArea]
    public string[] subText;
    public int index;
}