using System;
using System.Collections.Generic;
using GASSYS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : MonoBehaviour
{
    [SerializeField] private GameObject resultObj;
    [SerializeField] private Transform parent;
    [SerializeField] private Button mainBtn;
    [SerializeField] private Button modeBtn;
    [SerializeField] private TextMeshProUGUI resultText;
    
    List<ResultBoxObj> resultBoxObjs = new List<ResultBoxObj>();


    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        mainBtn.onClick.RemoveAllListeners();
        modeBtn.onClick.RemoveAllListeners();
        mainBtn.onClick.AddListener(delegate
        {
            //최상단으로 이동
            
            //GasSysManager.Instance.Init();
            gameObject.SetActive(false);
        });
        modeBtn.onClick.AddListener(delegate
        {
            //GasSysGlobalCanvas.Instance.ShowMenuPopup(true);
            gameObject.SetActive(false);
        });
    }

    public void SetResult(List<ResultObject> results, string title = "")
    {
        resultText.text = title; 
        foreach (var obj in resultBoxObjs)
        {
            obj.gameObject.SetActive(false);
        }
        

        for (int i = 0; i < results.Count; ++i)
        {
            if (resultBoxObjs.Count - 1 < i)
            {
                var obj = Instantiate(resultObj, parent).GetComponent<ResultBoxObj>();
                obj.Init(results[i]);
                resultBoxObjs.Add(obj);
            }
            else
            {
                resultBoxObjs[i].Init(results[i]);
            }
        }
        this.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        SoundManager.Instance.StopAllFireSound();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
