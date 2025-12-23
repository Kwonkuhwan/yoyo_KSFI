using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TextSprite", menuName = "ScriptableObjects/TextSpriteObject\r\n", order = 2)]
public class TextSpriteObject : ScriptableObject
{

    public TextSprite[] textSprites;
    private void OnValidate()
    {
        for (int i = 0; i < textSprites.Length; i++)
        {
            textSprites[i].index = i;
        }
    }
}

[Serializable]
public class TextSprite
{
    public string title;
    public Sprite sprite;
    public Sprite subSprite;
    public int index;
}
