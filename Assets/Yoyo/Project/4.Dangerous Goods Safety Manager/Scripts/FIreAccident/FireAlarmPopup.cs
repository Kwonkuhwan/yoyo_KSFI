using RJH.DangerousGoods;
using UnityEngine;
using UnityEngine.UI;

public class FireAlarmPopup : MonoBehaviour
{
    [SerializeField] private GameObject popup;
    [SerializeField] private DocsButton firealarmButton;
    [SerializeField] private AudioClip firealarmSound;
    [SerializeField] private Image alarmImage;
    [SerializeField] private Sprite[] alarmSprites;

    //[SerializeField] private Button nextButton;
    [SerializeField] private Image hand;
    [SerializeField] private DocsButton docsButton;

    private bool alarmCheck = false;

    private void Awake()
    {
        docsButton.docsAction += PopupOpen;
        firealarmButton.docsAction += Alarm;

        
    }
    private void OnEnable()
    {
        //SectionAndBackGroundManager.Instance.ReturnEvent -= PopupClose;
        //SectionAndBackGroundManager.Instance.ReturnEvent += PopupClose;
    }

    private void OnDisable()
    {
        AudioManager.Instance.StopSFX();
        //nextButton.onClick.RemoveListener(PopupClose);
        //nextButton.gameObject.SetActive(false);
        alarmImage.sprite = alarmSprites[0];
        popup.SetActive(false);
        alarmCheck = false;
        hand.enabled = true;
        SectionAndBackGroundManager.Instance.ReturnEvent -= PopupClose;
    }

    private void Alarm()
    {
        alarmImage.sprite = alarmSprites[1];
        AudioManager.Instance.PlaySFX(firealarmSound);
        alarmCheck = true;
        hand.enabled = false;
    }

    private void PopupOpen()
    {
        popup.SetActive(true);
        alarmImage.sprite = alarmSprites[0];
        hand.enabled = true;
    }
    private void Update()
    {
        if(!popup.activeSelf)
        {
            AudioManager.Instance.StopSFX();
            hand.enabled = true;
            alarmImage.sprite = alarmSprites[0];
        }
    }

    private bool PopupClose()
    {
        if(alarmCheck == false)
            return false;
       
        AudioManager.Instance.StopSFX();
        popup.SetActive(false);
        alarmCheck = false;
        return true;
    }
}
