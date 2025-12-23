using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GasSysArea1Corridor : MonoBehaviour
{
    [SerializeField] private GameObject dischargeOnLight;
    [SerializeField] private GameObject boxDischargeOn;
    [SerializeField] private Button manualControlBoxBtn;
    
    [SerializeField] private ManualControlBoxPopup manualControlBoxPopup;
    
#region 기동용기 솔레노이드밸브 격발시험 -> 수동조작함 작동

    [SerializeField] private HintScriptableObj _manualControlBoxControllerHint;
    [SerializeField] private RectTransform[] _manualControlBoxControllerHintRects;
    
#endregion //기동용기 솔레노이드밸브 격발시험 -> 수동조작함 작동
    
    private void Init()
    {
        dischargeOnLight.SetActive(false);
        boxDischargeOn.SetActive(false);
        manualControlBoxBtn.onClick.RemoveAllListeners();
        manualControlBoxBtn.gameObject.SetActive(false);
        manualControlBoxPopup.gameObject.SetActive(false);
    }

    public void InitManualControlBox(UnityAction openAction, UnityAction dischargeAction)
    {
        Init();
        manualControlBoxBtn.gameObject.SetActive(true);
        manualControlBoxBtn.onClick.AddListener(delegate
        {
            //GlobalCanvas.Instance.SetHintPopup(5,5, _manualControlBoxControllerHint, _manualControlBoxControllerHintRects[0]);
            //GlobalCanvas.Instance.ShowHint(true);
            manualControlBoxPopup.InitManualControlBox(openAction, dischargeAction);
            manualControlBoxPopup.gameObject.SetActive(true);
            manualControlBoxBtn.gameObject.SetActive(false);
        });
    }

    public void InitDischargeCheck()
    {
        Init();
        manualControlBoxBtn.gameObject.SetActive(true);
        manualControlBoxBtn.onClick.AddListener(delegate
        {
            manualControlBoxPopup.gameObject.SetActive(true);
        });
        manualControlBoxPopup.InitDischargeCheck(delegate
        {
            manualControlBoxBtn.gameObject.SetActive(false);
        });
        //dischargeOnLight.SetActive(true);
    }

    public void SetDischarge(bool value)
    {
        dischargeOnLight.SetActive(value);
        boxDischargeOn.SetActive(value);
        manualControlBoxPopup.SetDischarge(value);
    }

    public bool GetDischarge()
    {
        return dischargeOnLight.activeSelf;
    }

}
