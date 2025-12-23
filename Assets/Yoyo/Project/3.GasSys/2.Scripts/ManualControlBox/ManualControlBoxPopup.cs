using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ManualControlBoxPopup : MonoBehaviour
{
    [SerializeField] public GameObject closeObj;
    [SerializeField] public GameObject openObj;

    [SerializeField] public GameObject dischargeOnObj; 
    
    
    [SerializeField] public Button selectOpenBtn;
    [SerializeField] public Button selectActivateDischargeBtn;
    [SerializeField] public Button closeBtn;
    
    public void Init()
    {
        closeObj.SetActive(false);
        openObj.SetActive(true);
        dischargeOnObj.SetActive(false);
        selectOpenBtn.gameObject.SetActive(false);
        selectOpenBtn.onClick.RemoveAllListeners();
        
        selectActivateDischargeBtn.gameObject.SetActive(false);
        selectActivateDischargeBtn.onClick.RemoveAllListeners();
        
        closeBtn.gameObject.SetActive(false);
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(delegate
        {
            gameObject.SetActive(false);
        });
    }

    public void InitManualControlBox(UnityAction openAction, UnityAction dischargeAction = null, bool isClose = false)
    {
        Init();
        closeObj.SetActive(isClose);
        
        selectOpenBtn.gameObject.SetActive(true);
        selectOpenBtn.onClick.AddListener(delegate
        {
            openAction?.Invoke(); // 소리 출력
        });

        if (null != dischargeAction)
        {
            selectActivateDischargeBtn.onClick.AddListener(delegate
            {
                dischargeAction?.Invoke();
                //selectActivateDischargeBtn.gameObject.SetActive(false);
                //this.gameObject.SetActive(false);
            });
        }

    }

    public void InitDischargeCheck(UnityAction closeAction)
    {
        Init();
        closeObj.SetActive(true);
        //SetDischarge(true);
        closeBtn.gameObject.SetActive(true);
        closeBtn.onClick.AddListener(delegate
        {
            closeAction?.Invoke();
            this.gameObject.SetActive(false);
        });
    }

    public void SetDischarge(bool value)
    {
        dischargeOnObj.SetActive(value);
    }
    
    
    //[SerializeField] private GameObject 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
