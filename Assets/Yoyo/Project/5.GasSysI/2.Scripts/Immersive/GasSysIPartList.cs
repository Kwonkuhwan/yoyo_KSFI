using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GasSysIPartList : MonoBehaviour
{
#region singleton
    private static GasSysIPartList instance;
    public static GasSysIPartList Instance { get { return instance; } }
#endregion
    public GasSysIPartItem[] gasSysIPartItems;
    public GameObject panelObj;
    public GameObject moveRotatePanel;
    public Button moveBtn;
    [FormerlySerializedAs("rotationBtn")]
    public Button rotateBtn;
    public Dictionary<string, GasSysIPartItem> partDic = new Dictionary<string, GasSysIPartItem>();


    private void Awake()
    {
        instance = this;
        //EditorPrefs.SetInt("kAutoRefresh", 1);
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        foreach (var item in gasSysIPartItems)
        {
            partDic.Add(item.name, item);
        }
        SelectPart();
    }

    public Button GetMoveBtn(Action action = null)
    {
        if(null == action)
            return moveBtn;
        moveBtn.onClick.RemoveAllListeners();
        moveBtn.onClick.AddListener(action.Invoke);
        return moveBtn;
    }

    public Button GetRotateBtn(Action action = null)
    {
        if(null == action)
            return rotateBtn;
        rotateBtn.onClick.RemoveAllListeners();
        rotateBtn.onClick.AddListener(action.Invoke);
        return rotateBtn;
    }

    public void SetBtn(string btnName, Action action)
    {
        partDic[btnName].btn.onClick.RemoveAllListeners();
        partDic[btnName].btn.onClick.AddListener(delegate
        {
            SelectPart(btnName);
            action?.Invoke();
        });
    }

    public void SelectPart(string partName = "")
    {
        foreach (var part in partDic.Values)
        {
            part.enableObj.SetActive(false);
        }
        if (!string.IsNullOrEmpty(partName))
        {
            partDic[partName].enableObj.SetActive(true);
            if(!partName.Equals("오브젝트재정렬"))
                ShowMoveRotatePanel(true);
        }
        else
        {
            ShowMoveRotatePanel(false);
        }
     
        
    }

    private void OnEnable()
    {
        ShowMoveRotatePanel(false);
    }

    // private void OnDisable()
    // {
    //     ShowMoveRotatePanel(false);
    // }

    public void ShowMoveRotatePanel(bool isShow)
    {
        moveRotatePanel.SetActive(isShow);
    }
    
    public void ShowPanel(bool isShow)
    {
        panelObj.SetActive(isShow);
        if(!isShow)
            ShowMoveRotatePanel(false);
    }
    
}
