using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GasSysAreaManager : MonoBehaviour
{
    public enum StartAreaType
    {
        Area1, // 1번 구역
        Area1Corridor, // 1번 구역 복도
        StorageRoom, // 약제 저장소
        Area2, // 2번 구역
        Area2Corridor,
    }
    [SerializeField] public Button area1Btn;
    [SerializeField] public Button area1CorridorBtn;
    [SerializeField] public Button storageRoomBtn;
    [SerializeField] public Button area2Btn;
    [SerializeField] public Button area2CorridorBtn;

    [SerializeField] public GameObject area1EnableObj;
    [SerializeField] public GameObject area1CorridorEnableObj;
    [SerializeField] public GameObject storageRoomEnableObj;
    [SerializeField] public GameObject area2EnableObj;
    [SerializeField] public GameObject area2CorridorEnableObj;

    [SerializeField] private GameObject panelObj;
    //[SerializeField] private GameObject 

    public void Init(UnityAction area1Action,
        UnityAction area1CorridorAction, UnityAction storageRoomAction, UnityAction area2Action)
    {
        area1Btn.onClick.RemoveAllListeners();
        area1CorridorBtn.onClick.RemoveAllListeners();
        storageRoomBtn.onClick.RemoveAllListeners();
        area2Btn.onClick.RemoveAllListeners();

        area1Btn.onClick.AddListener(delegate
        {
            area1Action?.Invoke();
            ShowObj(area1EnableObj);
        });
        area1CorridorBtn.onClick.AddListener(delegate
        {
            area1CorridorAction?.Invoke();
            ShowObj(area1CorridorEnableObj);
        });
        storageRoomBtn.onClick.AddListener(delegate
        {
            storageRoomAction?.Invoke();
            ShowObj(storageRoomEnableObj);
        });
        area2Btn.onClick.AddListener(delegate
        {
            area2Action?.Invoke();
            ShowObj(area2EnableObj);
        });


    }

    public void Init()
    {
        area1Btn.onClick.RemoveAllListeners();
        area1CorridorBtn.onClick.RemoveAllListeners();
        storageRoomBtn.onClick.RemoveAllListeners();
        area2Btn.onClick.RemoveAllListeners();
        area2CorridorBtn.onClick.RemoveAllListeners();
    }

    public void StartArea(StartAreaType type)
    {
        switch (type)
        {

            case StartAreaType.Area1:
                ShowObj(area1EnableObj);
                break;
            case StartAreaType.Area1Corridor:
                //GlobalCanvas.Instance.ShowHint(false);
                ShowObj(area1CorridorEnableObj);
                break;
            case StartAreaType.StorageRoom:
                ShowObj(storageRoomEnableObj);
                break;
            case StartAreaType.Area2:
                ShowObj(area2EnableObj);
                break;
            case StartAreaType.Area2Corridor:
                ShowObj(area2CorridorEnableObj);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void ShowArea(StartAreaType type)
    {
        switch (type)
        {

            case StartAreaType.Area1:
                ShowObj(area1EnableObj);
                break;
            case StartAreaType.Area1Corridor:
                //GlobalCanvas.Instance.ShowHint(false);
                ShowObj(area1CorridorEnableObj);
                break;
            case StartAreaType.StorageRoom:
                ShowObj(storageRoomEnableObj);
                break;
            case StartAreaType.Area2:
                ShowObj(area2EnableObj);
                break;
            case StartAreaType.Area2Corridor:
                ShowObj(area2CorridorEnableObj);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void ShowObj(GameObject obj)
    {
        area1EnableObj.SetActive(obj.Equals(area1EnableObj));
        //area1Btn.interactable = !obj.Equals(area1EnableObj);

        area1CorridorEnableObj.SetActive(obj.Equals(area1CorridorEnableObj));
        //area1CorridorBtn.interactable = !obj.Equals(area1CorridorEnableObj);

        storageRoomEnableObj.SetActive(obj.Equals(storageRoomEnableObj));
        //storageRoomBtn.interactable = !obj.Equals(storageRoomEnableObj);

        area2EnableObj.SetActive(obj.Equals(area2EnableObj));
        //area2Btn.interactable = !obj.Equals(area2EnableObj);
        
        area2CorridorEnableObj.SetActive(obj.Equals(area2CorridorEnableObj));
        
    }



    public void ShowPanel(bool show)
    {
        panelObj.gameObject.SetActive(show);
    }
}
