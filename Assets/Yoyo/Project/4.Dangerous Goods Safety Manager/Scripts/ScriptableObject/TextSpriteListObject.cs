using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TextSpriteList", menuName = "ScriptableObjects/TextSpriteListObject\r\n", order = 3)]
public class TextSpriteListObject : ScriptableObject
{
    
    public TextSpriteList[] textSpriteLists;
}

[Serializable]
public class TextSpriteList
{
    public string title;
    public TextSprite[] textSprites;
}
