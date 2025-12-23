using System;
using System.Collections;
using System.Collections.Generic;
using LJS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GasSysMenuSelect : MonoBehaviour
{
    [SerializeField] private GameObject menuBtn;
    [SerializeField] private Transform parent;
    
    public Dictionary<string, MenuButtonObj> menuButtons = new Dictionary<string, MenuButtonObj>();
    [SerializeField] private Sprite[] sprites;

    [SerializeField] private Button[] btns;
    public void Init(NameListScriptableObj scriptableObj)
    {
        //menuButtons.Clear();
        foreach(string menuName in scriptableObj.korNames)
        {
            string regexName = Util.RemoveWhitespaceUsingRegex(menuName);
            if(menuButtons.ContainsKey(regexName))
                continue;
            var obj = Instantiate(menuBtn, parent);
            var menuBtnObj = obj.GetComponent<MenuButtonObj>();
            if (0 >= sprites.Length)
            {
                menuBtnObj.Init(menuName);
            }
            else
            {
                menuBtnObj.Init(regexName, sprites[menuButtons.Count]);
            }
            menuButtons.Add(regexName, menuBtnObj);
        }
    }

    public void SetButton(string menuName, UnityAction action)
    {
        string regexName = Util.RemoveWhitespaceUsingRegex(menuName);
        menuButtons[regexName].SetButton(action);
    }

    private void OnEnable()
    {
        ButtonManager.Instance.EnableSpecificButton(btns);
    }
}
