using System.Collections;
using System.Collections.Generic;
using GASSYS;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SolenoidValvePopup : MonoBehaviour
{
    [SerializeField] public GameObject aActivationObj;
    [SerializeField] public GameObject mActivationObj;
    [SerializeField] public GameObject aActivationPinObj;
    [SerializeField] public GameObject triggerObj;
    [SerializeField] public GameObject clipObj;
    [SerializeField] public GameObject triggerOnObj;
    [SerializeField] public GameObject recoveryPinObj;
    [SerializeField] public GameObject wallObj;
    [SerializeField] public Animation recoveryAni;

    [SerializeField] public Button closeBtn;
    
    [SerializeField] public Inventory inventory;

    [SerializeField] public Button pinDetachBtn;
    [SerializeField] public Button clipDetachBtn;
    [SerializeField] public Button mActivateBtn;

    [SerializeField] public Transform defaultParent;
    [SerializeField] public Transform area1CorridorPopupParent;
    [SerializeField] public Transform area1PopupParent;
    
    [SerializeField] public UIDragAndCollisionHandler dragAndCollisionHandler;

    public void Init()
    {
        //aActivationObj.SetActive(false);
        //mActivationObj.SetActive(false);
        aActivationPinObj.SetActive(false);
        wallObj.SetActive(false);
        //triggerObj.SetActive(false);
        clipObj.SetActive(false);
        recoveryPinObj.SetActive(false);
        triggerOnObj.SetActive(false);
        closeBtn.onClick.RemoveAllListeners();
        pinDetachBtn.onClick.RemoveAllListeners();
        clipDetachBtn.onClick.RemoveAllListeners();
        mActivateBtn.onClick.RemoveAllListeners();

        closeBtn.gameObject.SetActive(false);
        pinDetachBtn.gameObject.SetActive(false);
        clipDetachBtn.gameObject.SetActive(false);
        mActivateBtn.gameObject.SetActive(false);
        mActivationObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(188, -51);
        aActivationObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(177, 90);
        if (!transform.parent.Equals(defaultParent))
        {
            transform.SetParent(defaultParent);
        }
        closeBtn.onClick.AddListener(delegate
        {
            this.gameObject.SetActive(false);
            
            //교육 완료.
        });
    }

    public void DefaultActivation()
    {
        mActivationObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(188, -51);
        aActivationObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(177, 90);
    }
    
    
    public void InitSafetyCheck()
    {
        Init();
        aActivationPinObj.SetActive(true);
        clipObj.SetActive(true);
        pinDetachBtn.gameObject.SetActive(true);
        
        closeBtn.gameObject.SetActive(false);

        closeBtn.gameObject.SetActive(true);
        pinDetachBtn.onClick.AddListener(delegate
        {
            aActivationPinObj.SetActive(false);
            inventory.ShowSafetyPin(true);
            
            pinDetachBtn.gameObject.SetActive(false);
            //GlobalCanvas.Instance.ShowHint(false);
        });
    }

    /*
    public void InitManualOperationController()
    {
        Init();
        clipObj.SetActive(true);
        clipDetachBtn.gameObject.SetActive(true);
        closeBtn.gameObject.SetActive(false);
        
        closeBtn.onClick.AddListener(delegate
        {
            this.gameObject.SetActive(false);
            //교육 완료.
        });
        
        clipDetachBtn.onClick.AddListener(delegate
        {
            inventory.ShowSafetyClip(true);
            mActivateBtn.gameObject.SetActive(true);
            clipDetachBtn.gameObject.SetActive(false);
            clipObj.SetActive(false);
        });
        
        mActivateBtn.onClick.AddListener(delegate
        {
            mActivationObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(158, -51);
            triggerOnObj.SetActive(true);
            ControlPanel.Instance.ShowFire(true);
            ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, true);
            
            Observable.Timer(System.TimeSpan.FromSeconds(5))
                .Subscribe(_ =>
                {
                    SoundManager.Instance.StopAllFireSound();
                    GasSysManager.Instance.ChangeState(GasSysState.Init);
                })
                .AddTo(this); // 이 스크립트가 파괴될 때 구독을 자동으로 해제
                
        });
    }

    public void InitManualControlBox()
    {
        Init();
        clipObj.SetActive(true);
        transform.SetParent(area1CorridorPopupParent);
        //this.gameObject.SetActive(true);
    }
    
    public void InitCrossCircuitDetector()
    {
        Init();
        clipObj.SetActive(true);
        transform.SetParent(area1PopupParent);
    }
    */
    public void SetStorageRoomPopupParent()
    {
        if (!transform.parent.Equals(defaultParent))
            transform.SetParent(defaultParent);
    }

    public void SetArea1CorridorPopupParent()
    {
        if (!transform.parent.Equals(area1CorridorPopupParent))
            transform.SetParent(area1CorridorPopupParent);
    }

    public void SetArea1PopupParent()
    {
        if (!transform.parent.Equals(area1PopupParent))
            transform.SetParent(area1PopupParent);
    }

    public void InitControlPanelSwitch()
    {
        Init();
        clipObj.SetActive(true);
    }

    public void SolenoidValveAActivation(bool playSound = true)
    {
        //mActivationObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(510, -317);
        triggerOnObj.SetActive(true);
        aActivationObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(153, 90);
        if (!playSound)
            return;
        SoundManager.Instance.PlayBang();
    }

    public void InitRecoveryCheck1(UnityAction closeAction)
    {
        Init();
        triggerOnObj.SetActive(true);
        wallObj.SetActive(true);
        clipObj.SetActive(true);
        dragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
        {
            if (!recoveryAni.isPlaying)
            {
                recoveryAni.clip = recoveryAni.GetClip("NewRecoveryAni");
                recoveryAni.Play();
            }
            inventory?.ShowSafetyPin(false);
        };
        closeBtn.onClick.AddListener(delegate
        {
            closeAction?.Invoke();
            inventory.ShowRecoverySolenoidValve2(true);
            this.gameObject.SetActive(false);
        });
        
    }
    
    public static UnityEvent PinAttach = new UnityEvent(); 
    public void EndRecoveryAni()
    {
        Debug.Log("Ani End");
        //dragAndCollisionHandler.ResetEvent();
        // ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.ActivateSolenoidValve, false);
        // bool isAttach = false;
        // dragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
        // {
        //     if (!recoveryPinObj.Equals(draggedObject))
        //         return;
        //     if (isAttach)
        //         return;
        //     recoveryPinObj.SetActive(false);
        //     aActivationPinObj.SetActive(true);
        //     closeBtn.gameObject.SetActive(true);
        //     Debug.Log("ㅐㅏ");
        //     isAttach = true;
        // };
        PinAttach?.Invoke();

    }

    public void SolenoidValveMActivation(bool playSound = true)
    {
        triggerOnObj.SetActive(true);
        aActivationObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(153, 90);
        mActivationObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(158, -51);
        if (!playSound)
            return;
        SoundManager.Instance.PlayBang();
    }


    // Start is called before the first frame update
    void Start()
    {
        //InitRecoveryCheck1(null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
