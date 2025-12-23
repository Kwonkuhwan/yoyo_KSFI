using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RTypeRAreaManager : MonoBehaviour
{
    [SerializeField] public Button rTypeRBtn;
    [SerializeField] public Button fireDoorBtn;
    [FormerlySerializedAs("areaBtn")]
    [SerializeField] public Button area2_1Btn;
    [SerializeField] public Button area3_2Btn;
    
    [SerializeField] public GameObject rTypeREnableObj;
    [SerializeField] public GameObject fireDoorEnableObj;
    [FormerlySerializedAs("areaEnableObj")]
    [SerializeField] public GameObject area2_1EnableObj;
    [SerializeField] public GameObject area3_2EnableObj;
    [SerializeField] private GameObject panelObj;
    
    public void Init(UnityAction rTypeRAction,
        UnityAction fireDoorAction, UnityAction area2_1Action, UnityAction area3_2Action)
    {
        rTypeRBtn.onClick.RemoveAllListeners();
        fireDoorBtn.onClick.RemoveAllListeners();
        area2_1Btn.onClick.RemoveAllListeners();
        area3_2Btn.onClick.RemoveAllListeners();
        rTypeRBtn.onClick.AddListener(delegate
        {
            rTypeRAction?.Invoke();
            ShowObj(rTypeREnableObj);
        });
        fireDoorBtn.onClick.AddListener(delegate
        {
            fireDoorAction?.Invoke();
            ShowObj(fireDoorEnableObj);
        });
        area2_1Btn.onClick.AddListener(delegate
        {
            area2_1Action?.Invoke();
            ShowObj(area2_1EnableObj);
        });
        
        area3_2Btn.onClick.AddListener(delegate
        {
            area3_2Action?.Invoke();
            ShowObj(area3_2EnableObj);
        });


    }
    
    public void Init()
    {
        rTypeRBtn.onClick.RemoveAllListeners();
        fireDoorBtn.onClick.RemoveAllListeners();
        area2_1Btn.onClick.RemoveAllListeners();
    }
    
    public void StartArea(RTypeRRoomType type)
    {
        switch (type)
        {

            case RTypeRRoomType.RTypeR:
                ShowObj(rTypeREnableObj);
                break;
            case RTypeRRoomType.FireDoor:
                //GlobalCanvas.Instance.ShowHint(false);
                ShowObj(fireDoorEnableObj);
                break;
            case RTypeRRoomType.Area2_1:
                ShowObj(area2_1EnableObj);
                break;
            case RTypeRRoomType.Area3_2:
                ShowObj(area3_2EnableObj);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public Button GetRTypeRBtn(Action action = null)
    {
        if (null == action)
            return rTypeRBtn;
        rTypeRBtn.onClick.RemoveAllListeners();
        rTypeRBtn.onClick.AddListener(delegate
        {
            ShowObj(rTypeREnableObj);
            action.Invoke();
        });
        return rTypeRBtn;
    }

    public Button GetFireDoorBtn(Action action = null)
    {
        if(null == action)
            return fireDoorBtn;
        fireDoorBtn.onClick.RemoveAllListeners();
        fireDoorBtn.onClick.AddListener(delegate
        {
            ShowObj(fireDoorEnableObj);
            action.Invoke();
        });
        return fireDoorBtn;
    }

    public Button GetArea2_1Btn(Action action = null)
    {
        if(null == action)
            return area2_1Btn;
        area2_1Btn.onClick.RemoveAllListeners();
        area2_1Btn.onClick.AddListener(delegate
        {
            ShowObj(area2_1EnableObj);
            action.Invoke();
        });
        return area2_1Btn;
    }

    public Button GetArea3_2Btn(Action action = null)
    {
        if(null == action)
            return area3_2Btn;
        area3_2Btn.onClick.RemoveAllListeners();
        area3_2Btn.onClick.AddListener(delegate
        {
            ShowObj(area3_2EnableObj);
            action.Invoke();
        });
        return area3_2Btn;
    }
    
    public void ShowObj(GameObject obj)
    {
        rTypeREnableObj.SetActive(obj.Equals(rTypeREnableObj));
        fireDoorEnableObj.SetActive(obj.Equals(fireDoorEnableObj));
        area2_1EnableObj.SetActive(obj.Equals(area2_1EnableObj));
        area3_2EnableObj.SetActive(obj.Equals(area3_2EnableObj));
        
    }

    public void ShowPanel(bool isShow)
    {
        gameObject.SetActive(isShow);
    }
    
}
