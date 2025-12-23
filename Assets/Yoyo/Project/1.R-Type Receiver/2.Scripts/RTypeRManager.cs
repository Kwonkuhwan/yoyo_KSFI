using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class RTypeRManager : MonoBehaviour
{
#region singleton

    private static RTypeRManager instance;
    public static RTypeRManager Instance { get { return instance; } }

#endregion
    [Foldout("메뉴")]
    [SerializeField] public GameObject modeObj;
    [SerializeField] public GameObject menuObj;

    [SerializeField] public Button practiceModeBtn;
    [SerializeField] public Button evaluationModeBtn;
    [SerializeField] public Button EquipmentOperationBtn;
    [SerializeField] public Button FireAlarmSystemBtn;
    [SerializeField] public Button CircuitBreakerBtn;
    public RTypeRSection section;
    private void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        section.inventoryObj.ShowPanel(false);
        section.areaManagerObj.ShowPanel(false);
        practiceModeBtn.onClick.RemoveAllListeners();
        evaluationModeBtn.onClick.RemoveAllListeners();
        EquipmentOperationBtn.onClick.RemoveAllListeners();
        FireAlarmSystemBtn.onClick.RemoveAllListeners();
        CircuitBreakerBtn.onClick.RemoveAllListeners();

        practiceModeBtn.onClick.AddListener(delegate
        {
            section.SetRTypeRState(RTypeRState.PracticeMode);
            ShowObj(menuObj);
        });

        evaluationModeBtn.onClick.AddListener(delegate
        {
            section.SetRTypeRState(RTypeRState.EvaluationMode);
            ShowObj(menuObj);
        });

        EquipmentOperationBtn.onClick.AddListener(delegate
        {
            section.InitEquipmentOperation();
            ShowObj(null);
        });
        FireAlarmSystemBtn.onClick.AddListener(delegate
        {
            section.InitFireAlarmSystem();
            ShowObj(null);
        });
        CircuitBreakerBtn.onClick.AddListener(delegate
        {
            section.InitCircuitBreaker();
            ShowObj(null);
        });

        section.ShowHint(false);
        section.rTypeRState = RTypeRState.None;
        ShowObj(modeObj);
    }

    private void ShowObj(GameObject obj)
    {
        modeObj.SetActive(modeObj.Equals(obj));
        menuObj.SetActive(menuObj.Equals(obj));
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
