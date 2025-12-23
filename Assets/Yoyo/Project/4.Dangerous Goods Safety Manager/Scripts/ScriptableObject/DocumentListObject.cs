using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DocumentList", menuName = "ScriptableObjects/DocumentListObject\r\n", order = 5)]
public class DocumentListObject : ScriptableObject
{
    public Document[] documentList;
   

    private void OnValidate()
    {
        
        for (int i = 0; i < documentList.Length; i++)
        {
            documentList[i].id = i;
            
        }
    }
}

[Serializable]
public class Document
{
    public string title;
    [TextArea]
    public string document;
    public AudioClip audioClip;
    public bool checkDuplication = false;
    public int id;
    public string[] buttonNames;
    public RemoveButton removeButton;
    public bool isCheckListOn = false;
}

public enum RemoveButton
{
    None,
    Right,
    Left,
    Both
}



