using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultBoxObj : MonoBehaviour
{
    public GameObject success;
    public GameObject middle;
    public TextMeshProUGUI titleText;
    public Button resultBtn;

    public void Init(ResultObject resultObject, Action action = null)
    {
        success.SetActive(false);
        middle.SetActive(false);
        switch (resultObject.resultType)
        {
            case ResultType.성공:
                success.SetActive(true);
                break;
            case ResultType.실패:
                success.SetActive(false);
                middle.SetActive(false);
                break;
            case ResultType.보류:
                middle.SetActive(true);
                break;
        }
        success.SetActive(resultObject.IsSuccess);
        titleText.text = resultObject.Title;
        gameObject.SetActive(true);
        resultBtn.onClick.RemoveAllListeners();
        if (action != null)
            resultBtn.onClick.AddListener(action.Invoke);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
