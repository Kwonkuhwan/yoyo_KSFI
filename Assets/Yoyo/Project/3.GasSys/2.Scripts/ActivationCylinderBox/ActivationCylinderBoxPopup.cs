using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ActivationCylinderBoxPopup : MonoBehaviour
{
    [SerializeField] public Inventory inventory;
    [SerializeField] public UIDragAndCollisionHandler dragAndCollisionHandler;
    [FormerlySerializedAs("aActivationObj")]
    [SerializeField] public GameObject aActivationPinObj;
    [SerializeField] public GameObject solenoidValveObj;
    [SerializeField] public GameObject pressureSwitch;

    public GameObject aActivationObj;
    
    [SerializeField] public GameObject selectPinObj;
    [SerializeField] public GameObject selectAttachPinObj;

    [SerializeField] public GameObject pinObj;
    
    [SerializeField] public Button closeBtn;

    [SerializeField] public Button detachSolenoidBtn;

    [SerializeField] public Button selectSwitchUpBtn;
    [SerializeField] public Button selectSwitchDownBtn;
    [SerializeField] public GameObject recoveryPinObj;

    [SerializeField] public RectTransform backGround;
    [SerializeField] public Vector2 defaultPos;
    [SerializeField] public Vector2 lightTestPos;
    

    [SerializeField] public GameObject bodyObj;
    public void Init()
    {
        detachSolenoidBtn.gameObject.SetActive(false);
        closeBtn.gameObject.SetActive(false);
        selectPinObj.SetActive(false);
        selectAttachPinObj.SetActive(false);
        detachSolenoidBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
        selectSwitchUpBtn.onClick.RemoveAllListeners();
        selectSwitchDownBtn.onClick.RemoveAllListeners();
        solenoidValveObj.SetActive(false);
        aActivationPinObj.SetActive(false);
        selectSwitchUpBtn.gameObject.SetActive(false);
        selectSwitchDownBtn.gameObject.SetActive(false);
        pinObj.SetActive(false);
        pinObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(207, 135);
        SetDefaultPos();
        pressureSwitch.GetComponent<RectTransform>().anchoredPosition = new Vector2(162, -76);
        
        //dragAndCollisionHandler.ResetEvent();
    }

    public void SetDefaultPos()
    {
        backGround.localPosition = defaultPos;
    }

    public void SetLightTestPos()
    {
        backGround.localPosition = lightTestPos;
    }
    
    public void InitSafetyCheck(UnityAction unityAction = null)
    {
        Init();
        dragAndCollisionHandler.ResetEvent();
        //dragAndCollisionHandler.OnPicked -= DragObjectPicked;
        dragAndCollisionHandler.OnPicked += (obj) =>
        {
            selectAttachPinObj.SetActive(true);
        };
        bool isDetached = false;
        //dragAndCollisionHandler.OnCollisionDetected -= HandleCollision;
        dragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObjectHandleCollision) =>
        {
            if (!isDetached)
                isDetached = true;
            selectPinObj.SetActive(false);
            draggedObject.SetActive(false);
            aActivationPinObj.SetActive(true);
            selectAttachPinObj.SetActive(false);
            detachSolenoidBtn.gameObject.SetActive(true);
            
        };
        
        closeBtn.gameObject.SetActive(true);

        pinObj.SetActive(true);
        solenoidValveObj.SetActive(true);
        
        detachSolenoidBtn.onClick.AddListener(delegate
        {
            detachSolenoidBtn.gameObject.SetActive(false);
            solenoidValveObj.SetActive(false);
            unityAction?.Invoke();
        });
        
        closeBtn.onClick.AddListener(delegate
        {
            this.gameObject.SetActive(false);
        });
        
        selectPinObj.SetActive(true);
        gameObject.SetActive(true);
    }
    
    public void InitDischargeUpCheck(UnityAction closeAction, UnityAction upAction)
    {
        Init();
        solenoidValveObj.SetActive(true);
        aActivationPinObj.SetActive(true);
        closeBtn.onClick.AddListener(delegate
        {
            closeAction?.Invoke();
            closeBtn.gameObject.SetActive(false);
            // selectSwitchDownBtn.gameObject.SetActive(true);
            // selectSwitchDownBtn.onClick.AddListener(delegate
            // {
            //     PressureSwitchDown();
            //     downAction?.Invoke();
            //     ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);
            //     selectSwitchUpBtn.gameObject.SetActive(false);
            //     closeBtn.gameObject.SetActive(true);
            // });
            this.gameObject.SetActive(false);
        });
        
        selectSwitchUpBtn.gameObject.SetActive(true);
        selectSwitchUpBtn.onClick.AddListener(delegate
        {
            PressureSwitchUp();
            upAction?.Invoke();
            selectSwitchUpBtn.gameObject.SetActive(false);
            closeBtn.gameObject.SetActive(true);
        });
    }
    
    public void InitDischargeDownCheck(UnityAction closeAction, UnityAction downAction)
    {
        Init();
        solenoidValveObj.SetActive(true);
        aActivationPinObj.SetActive(true);
        closeBtn.onClick.AddListener(delegate
        {
            closeAction?.Invoke();
            closeBtn.gameObject.SetActive(false);
            // selectSwitchDownBtn.gameObject.SetActive(true);
            // selectSwitchDownBtn.onClick.AddListener(delegate
            // {
            //     PressureSwitchDown();
            //     downAction?.Invoke();
            //     ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);
            //     selectSwitchUpBtn.gameObject.SetActive(false);
            //     closeBtn.gameObject.SetActive(true);
            // });
            this.gameObject.SetActive(false);
        });

        PressureSwitchUp();
        
        selectSwitchDownBtn.gameObject.SetActive(true);
        selectSwitchDownBtn.onClick.AddListener(delegate
        {
            PressureSwitchDown();
            downAction?.Invoke();
            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);
            selectSwitchUpBtn.gameObject.SetActive(false);
            closeBtn.gameObject.SetActive(true);
        });
        
        // selectSwitchUpBtn.gameObject.SetActive(true);
        // selectSwitchUpBtn.onClick.AddListener(delegate
        // {
        //     PressureSwitchUp();
        //     upAction?.Invoke();
        //     selectSwitchUpBtn.gameObject.SetActive(false);
        //     closeBtn.gameObject.SetActive(true);
        // });
    }

    public void InitRecoveryCheck(UnityAction closeAction, UnityAction attachBody)
    {
        Init();
        bool isRecovery = false;
        dragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
        {
            if (targetObject.name.Equals("Body"))
            {
                attachBody.Invoke();
                draggedObject.SetActive(false);
                solenoidValveObj.SetActive(true);
                aActivationPinObj.SetActive(true);
                inventory.ShowRecoverySolenoidValve2(false);
            }
            else
            {
                if (isRecovery)
                    return;
                draggedObject.SetActive(false);
                targetObject.SetActive(true);
                isRecovery = true;
                closeBtn.gameObject.SetActive(true);
            }
        
        };
        
        closeBtn.onClick.AddListener(delegate
        {
            closeAction?.Invoke();
            this.gameObject.SetActive(false);
        });
    }

    public void PressureSwitchUp(bool isLog = false)
    {
        if (isLog)
        {
            ControlPanel.Instance.SetReceiverLog("방출확인 on");
        }
        pressureSwitch.GetComponent<RectTransform>().anchoredPosition = new Vector2(162, -58);
        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, true);
        
        

    }

    public void PressureSwitchDown(bool isLog = false)
    {
        if (isLog)
        {
            ControlPanel.Instance.SetReceiverLog("방출확인 off");
        }
        pressureSwitch.GetComponent<RectTransform>().anchoredPosition = new Vector2(162, -76);
        ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.VerifyDischarge, false);
    }
    // Start is called before the first frame update
    void Start()
    {
        //InitSafetyCheck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        //dragAndCollisionHandler.OnPicked -= DragObjectPicked;
        //dragAndCollisionHandler.OnCollisionDetected -= HandleCollision;
    }

    private void DragObjectPicked(GameObject obj)
    {
        //selectPinObj.SetActive(false);
        selectAttachPinObj.SetActive(true);
    }

    private void HandleCollision(GameObject draggedObject, GameObject targetObject)
    {
        selectPinObj.SetActive(false);
        draggedObject.SetActive(false);
        aActivationPinObj.SetActive(true);
        selectAttachPinObj.SetActive(false);
        detachSolenoidBtn.gameObject.SetActive(true);
        //targetObject.SetActive(false);
    }
}
