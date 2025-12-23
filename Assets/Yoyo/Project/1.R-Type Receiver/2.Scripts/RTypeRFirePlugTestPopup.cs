using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public class RTypeRFirePlugTestPopup : MonoBehaviour
{
    [Foldout("공용")]
    public Button closeBtn;
    
    [Foldout("소화전")]
    public Button openFirePlugBtn;
    public GameObject defaultFirePlugObj;
    public GameObject innerFirePlugObj;
    public GameObject[] firePlugObjs;
    
    
    [Foldout("테스터")]
    public Transform lineParent;
    public RectTransform redStartRt;
    public RectTransform blackStartRt;
    public Button powerBtn;
    [FormerlySerializedAs("targetTesterObj")]
    public GameObject testerObj;

    public GameObject testerTargetObj;
    
    public TesterPenObj[] testerPenObjs;
    
    public GameObject[] powerObjs;
    public GameObject[] numObjs;

    public GameObject[] testAreas;
    
    private List<GameObject> existingLines = new List<GameObject>();

    

    public Button GetCloseBtn(Action action = null)
    {
        if (null == action)
            return closeBtn;
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(action.Invoke);
        return closeBtn;
    }

    public Button GetOpenFirePlugBtn(Action action = null)
    {
        if(null == action)
            return openFirePlugBtn;
        openFirePlugBtn.onClick.RemoveAllListeners();
        openFirePlugBtn.onClick.AddListener(action.Invoke);
        return openFirePlugBtn;
    }

    public Button GetPowerBtn(Action action = null)
    {
        if(null == action)
            return powerBtn;
        powerBtn.onClick.RemoveAllListeners();
        powerBtn.onClick.AddListener(action.Invoke);
        return powerBtn;
    }

    public void ShowFirePlug(RTypeRFirePlugType type)
    {
        switch(type)
        {
            case RTypeRFirePlugType.Default:
                {
                    ShowFirePlugObj(firePlugObjs[0]);
                }
                break;
            case RTypeRFirePlugType.Inner:
                {
                    ShowFirePlugObj(firePlugObjs[1]);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void ShowFirePlugObj(GameObject obj)
    {
        foreach (var penObj in firePlugObjs)
        {
            penObj.gameObject.SetActive(obj.Equals(penObj));
        }
    }

    public void ShowTesterPen(RTypeRTesterPenType type)
    {
        switch (type)
        {

            case RTypeRTesterPenType.None:
                {
                    ShowTesterPenObj(null);
                    DestroyLines();
                }
                break;
            case RTypeRTesterPenType.Default:
                {
                    ShowTesterPenObj(testerPenObjs[0]);
                    DestroyLines();
                    DrawLineBetweenImages(redStartRt, testerPenObjs[0].redPen, lineParent, Color.red);
                    DrawLineBetweenImages(blackStartRt, testerPenObjs[0].blackPen, lineParent, Color.black);
                }
                break;
            case RTypeRTesterPenType.SmokePopup:
                {
                    
                }
                break;
            case RTypeRTesterPenType.중계기전원:
                {
                    ShowTesterPenObj(testerPenObjs[2]);
                    DestroyLines();
                    DrawLineBetweenImages(redStartRt, testerPenObjs[2].redPen, lineParent, Color.red);
                    DrawLineBetweenImages(blackStartRt, testerPenObjs[2].blackPen, lineParent, Color.black);
                }
                break;
            case RTypeRTesterPenType.중계기통신:
                {
                    ShowTesterPenObj(testerPenObjs[3]);
                    DestroyLines();
                    DrawLineBetweenImages(redStartRt, testerPenObjs[3].redPen, lineParent, Color.red);
                    DrawLineBetweenImages(blackStartRt, testerPenObjs[3].blackPen, lineParent, Color.black);
                }
                break;
            case RTypeRTesterPenType.감지기선로:
                {
                    ShowTesterPenObj(testerPenObjs[4]);
                    DestroyLines();
                    DrawLineBetweenImages(redStartRt, testerPenObjs[4].redPen, lineParent, Color.red);
                    DrawLineBetweenImages(blackStartRt, testerPenObjs[4].blackPen, lineParent, Color.black);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void ShowTesterPenObj(TesterPenObj obj)
    {
        foreach (var penObj in testerPenObjs)
        {
            penObj.gameObject.SetActive(penObj.Equals(obj));
        }
    }
    
    
    public void ShowTesterPower(RTypeRTesterPowerType type)
    {
        switch(type)
        {
            case RTypeRTesterPowerType.Off:
                {
                    ShowTesterPowerObj(powerObjs[0]);
                }
                break;
            case RTypeRTesterPowerType.On:
                {
                    ShowTesterPowerObj(powerObjs[1]);   
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void ShowTesterPowerObj(GameObject obj)
    {
        foreach (var powerObj in powerObjs)
        {
            powerObj.SetActive(obj.Equals(powerObj));
        }
    }
    
    public GameObject ShowTesterNum(RTypeRTesterNumType type)
    {
        switch(type)
        {
            case RTypeRTesterNumType.Off:
                {
                    ShowTesterNumObj(numObjs[0]);
                    return numObjs[0];
                }
                break;
            case RTypeRTesterNumType.Num2059:
                {
                    ShowTesterNumObj(numObjs[1]);
                    return numObjs[1];
                }
                break;
            case RTypeRTesterNumType.Num2390:
                {
                    ShowTesterNumObj(numObjs[2]);
                    return numObjs[2];
                }
                break;
            case RTypeRTesterNumType.Num2710:
                {
                    ShowTesterNumObj(numObjs[3]);
                    return numObjs[3];
                }
                break;
            case RTypeRTesterNumType.Num0:
                {
                    ShowTesterNumObj(numObjs[4]);
                    return numObjs[4];
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void ShowTesterNumObj(GameObject obj)
    {
        foreach (var numObj in numObjs)
        {
            numObj.SetActive(numObj.Equals(obj));
        }
    }
    
    public void DestroyLines()
    {
        foreach(var line in existingLines)
        {
            Destroy(line);
        }
        existingLines.Clear();
    }
    
    public void DrawLineBetweenImages(RectTransform image1, RectTransform image2, Transform parent, Color lineColor, float lineWidth = 2f)
    {
        string lineKey = $"{image1.name}-{image2.name}";

        // 기존 라인이 있으면 제거
        // if (existingLines.ContainsKey(lineKey))
        // {
        //     Destroy(existingLines[lineKey]);
        //     existingLines.Remove(lineKey);
        // }

        // 라인 오브젝트 생성
        GameObject lineObject = new GameObject("Line");
        lineObject.transform.SetParent(parent);
        lineObject.layer = parent.gameObject.layer;

        // Line의 RectTransform 설정
        RectTransform lineRect = lineObject.AddComponent<RectTransform>();
        lineRect.anchorMin = Vector2.zero;
        lineRect.anchorMax = Vector2.zero;
        lineRect.pivot = new Vector2(0.5f, 0.5f);

        // Line 이미지 추가
        Image lineImage = lineObject.AddComponent<Image>();
        lineImage.color = lineColor;

        // 이미지의 실제 위치 계산 (pivot을 반영)
        Vector2 startPos = image1.TransformPoint(new Vector3(0, 0, 0));//image1.position;
        Vector2 endPos = image2.TransformPoint(new Vector3(0, (-image2.rect.height * image2.pivot.y)+6f, 0)); // 이미지 2의 pivot(0.5, 0)을 기준//image2.position + new Vector3(0, (-image2.rect.height * image2.pivot.y)+6f, 0); // pivot(0.5, 0) 보정

        // 라인의 위치 및 크기 설정
        Vector2 direction = (endPos - startPos).normalized;
        float distance = Vector2.Distance(startPos, endPos);

        lineRect.sizeDelta = new Vector2(distance, lineWidth);
        lineRect.position = startPos + (endPos - startPos) / 2;
        lineRect.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        // 생성된 라인을 Dictionary에 저장
        existingLines.Add(lineObject);
    }
}
