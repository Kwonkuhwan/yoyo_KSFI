using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeDetector : MonoBehaviour
{
    [SerializeField] private GameObject onObj;
    // Start is called before the first frame update

#region 교차회로 감지기 동작

    public void InitCrossCircuitDetector()
    {
        SetSmokeDetector(false);
    }

    public void SetSmokeDetector(bool isOn)
    {
        onObj.SetActive(isOn);
    }

#endregion
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
