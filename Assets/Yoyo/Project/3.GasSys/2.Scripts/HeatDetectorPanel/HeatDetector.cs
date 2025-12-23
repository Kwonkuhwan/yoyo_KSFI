using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatDetector : MonoBehaviour
{
    [SerializeField] private GameObject onObj;
  
#region 교차회로 감지기 동작

    public void InitCrossCircuitDetector()
    {
        SetHeatDetector(false);
    }

    public void SetHeatDetector(bool isOn)
    {
        onObj.SetActive(isOn);
    }

#endregion

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
