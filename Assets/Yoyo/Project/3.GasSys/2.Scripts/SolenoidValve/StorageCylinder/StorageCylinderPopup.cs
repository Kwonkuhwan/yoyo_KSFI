using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StorageCylinderPopup : MonoBehaviour
{
    [SerializeField] private Button onOffbtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private GameObject onObj;
    [SerializeField] private GameObject offObj;
    //private readonly bool[] _isOnOff = new bool[2];
    private bool _isOnOff = true;

    public void Init()
    {
        // SetOnOff(true);
        // _isOnOff = true;
        //_isOnOff[1] = true;
        onOffbtn.gameObject.SetActive(false);
        closeBtn.gameObject.SetActive(false);
        onOffbtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }
    
    public void InitSafetyCheck(bool isOnOff, UnityAction<bool> callback, UnityAction close, int index = 0)
    {
        Init();
        _isOnOff = isOnOff;
        onOffbtn.gameObject.SetActive(true);
        
        onOffbtn.onClick.AddListener(delegate
        {
            _isOnOff = !_isOnOff;
            SetOnOff(_isOnOff);
            onOffbtn.gameObject.SetActive(false);
            if(!_isOnOff)
                closeBtn.gameObject.SetActive(true);
            callback(_isOnOff);
        });
        
        closeBtn.onClick.AddListener(delegate
        {
            close?.Invoke();
            this.gameObject.SetActive(false);
        });
    }
    
    public void InitPopup(bool isOnOff, UnityAction<bool> callback,  bool closeBtnShow=false, UnityAction close = null)
    {
        Init();
        this.gameObject.SetActive(true);
        int index = 0;
        _isOnOff = isOnOff;
        SetOnOff(_isOnOff);
        onOffbtn.gameObject.SetActive(true);
        
        if(closeBtnShow)
            closeBtn.gameObject.SetActive(true);
        onOffbtn.onClick.AddListener(delegate
        {
            _isOnOff = !_isOnOff;
            SetOnOff(_isOnOff);
            //onOffbtn.gameObject.SetActive(false);
           
            callback(_isOnOff);
        });
        
        
        closeBtn.onClick.AddListener(delegate
        {
            close?.Invoke();
            this.gameObject.SetActive(false);
        });
        
    }
    
    public void InitPopupE(UnityAction<bool> callback, bool closeBtnShow=false, UnityAction close = null)
    {
        Init();
        this.gameObject.SetActive(true);
        int index = 0;
        onOffbtn.gameObject.SetActive(true);
        // SetOnOff(isOnOff);
        // _isOnOff = isOnOff;
        if(closeBtnShow)
            closeBtn.gameObject.SetActive(true);
        onOffbtn.onClick.AddListener(delegate
        {
            _isOnOff = !_isOnOff;
            SetOnOff(_isOnOff);
            //onOffbtn.gameObject.SetActive(false);
           
            callback(_isOnOff);
            gameObject.SetActive(false);
        });
        
        
        closeBtn.onClick.AddListener(delegate
        {
            close?.Invoke();
            this.gameObject.SetActive(false);
        });
        
    }
    
    public void InitRecoveryCheck(bool isOnOff, UnityAction<bool> callback, UnityAction close, int index = 0)
    {
        Init();
        _isOnOff = isOnOff;
        onOffbtn.gameObject.SetActive(true);
        SetOnOff(false);
        onOffbtn.onClick.AddListener(delegate
        {
            _isOnOff = !_isOnOff;
            SetOnOff(_isOnOff);
            onOffbtn.gameObject.SetActive(false);
            closeBtn.gameObject.SetActive(true);
            callback(_isOnOff);
        });
        
        closeBtn.onClick.AddListener(delegate
        {
            close?.Invoke();
            this.gameObject.SetActive(false);
        });
    }
    
    public Button[] GetButtons()
    {
        return new Button[] { onOffbtn };
    }

    public void SetOnOff(bool isOnOff)
    {
        _isOnOff = isOnOff;
        onObj.SetActive(isOnOff);
        offObj.SetActive(!isOnOff);
    }

    public bool GetIsOnOff()
    {
        return _isOnOff;
    }
}
