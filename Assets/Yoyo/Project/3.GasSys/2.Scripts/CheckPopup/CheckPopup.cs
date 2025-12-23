using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CheckPopup : MonoBehaviour
{
    [SerializeField] private Button agreeBtn;
    [SerializeField] private GameObject area1Obj;
    [SerializeField] private GameObject area2Obj;
    [SerializeField] private GameObject fireShowObj;
    [SerializeField] private GameObject timerObj;
    [SerializeField] private GameObject sol1Obj;
    [SerializeField] private GameObject sol2Obj;
    [SerializeField] private GameObject sol3Obj;
    [SerializeField] private GameObject area1_2Obj;
    [SerializeField] private GameObject lightTestOnObj;
    [FormerlySerializedAs("lightTextOffObj")]
    [SerializeField] private GameObject lightTestOffObj;
    [SerializeField] private GameObject lightTestCheckObj;
    
    // Start is called before the first frame update


    public void ShowArea1(UnityAction action)
    {
        ShowObj(area1Obj);
        AgreeBtn(action);
    }

    public void ShowArea2(UnityAction action)
    {
        ShowObj(area2Obj);
        AgreeBtn(action);
    }

    public void ShowFire(UnityAction action)
    {
        ShowObj(fireShowObj);
        AgreeBtn(action);
    }

    public void ShowTimer(UnityAction action)
    {
        ShowObj(timerObj);
        AgreeBtn(action);
    }

    public void ShowSol1(UnityAction action)
    {
        ShowObj(sol1Obj);
        AgreeBtn(action);
    }

    public void ShowSol2(UnityAction action)
    {
        ShowObj(sol2Obj);
        AgreeBtn(action);
    }

    public void ShowSol3(UnityAction action)
    {
        ShowObj(sol3Obj);
        AgreeBtn(action);
    }

    public void ShowArea1_2(UnityAction action)
    {
        ShowObj(area1_2Obj);
        AgreeBtn(action);
    }

    public void ShowLightTestOn(UnityAction action)
    {
        ShowObj(lightTestOnObj);
        AgreeBtn(action);
    }

    public void ShowLightTestOff(UnityAction action)
    {
        ShowObj(lightTestOffObj);
        AgreeBtn(action);
    }

    public void ShowLightTestCheck(UnityAction action)
    {
        ShowObj(lightTestCheckObj);
        AgreeBtn(action);
    }
    
    
    public void ShowObj(GameObject obj)
    {
        this.gameObject.SetActive(null != obj);
        area1Obj.SetActive(area1Obj.Equals(obj));
        area2Obj.SetActive(area2Obj.Equals(obj));
        fireShowObj.SetActive(fireShowObj.Equals(obj));
        timerObj.SetActive(timerObj.Equals(obj));
        sol1Obj.SetActive(sol1Obj.Equals(obj));
        sol2Obj.SetActive(sol2Obj.Equals(obj));
        sol3Obj.SetActive(sol3Obj.Equals(obj));
        area1_2Obj.SetActive(area1_2Obj.Equals(obj));
        lightTestOnObj.SetActive(lightTestOnObj.Equals(obj));
        lightTestOffObj.SetActive(lightTestOffObj.Equals(obj));
        if(lightTestCheckObj)
            lightTestCheckObj.SetActive(lightTestCheckObj.Equals(obj));
    }

    private void AgreeBtn(UnityAction action)
    {
        return;
        agreeBtn.onClick.RemoveAllListeners();
        agreeBtn.onClick.AddListener(delegate
        {
            action?.Invoke();
            //this.gameObject.SetActive(false);
        });
        agreeBtn.gameObject.SetActive(true);
    }

    public bool Area2Open()
    {
        return area2Obj.activeSelf;
    }

    public Button GetCheckBtn()
    {
        return agreeBtn;
    }
    
}
