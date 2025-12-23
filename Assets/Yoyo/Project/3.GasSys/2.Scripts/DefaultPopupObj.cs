using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DefaultPopupObj : CustomPopupMonoBehaviour
{
#region private Fields

    [Header("GUI Object 등록")]
    [SerializeField] private Button agreeBtn;
    [SerializeField] private TextMeshProUGUI agreeBtnText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI titleText;


    [Header("정보 입력")] [SerializeField] private string sTitle;
    [Multiline] [SerializeField] private string sInfo;
    [SerializeField] private TextAsset infoTextAsset;
    [SerializeField] private string sAgreeBtnText;
    [SerializeField] private string sCloseBtnText;

    [Header("체크 옵션")] [SerializeField] private bool checkOption;
    [SerializeField] private Toggle toggle;

    [SerializeField] private SpriteAutoSize spriteAutoSize;
    private Image spriteImage;

#endregion


    /*
    private void Awake()
    {
        spriteAutoSize?.AutoResizeSprite();
        spriteImage = spriteAutoSize?.GetComponent<Image>();
        toggle?.gameObject.SetActive(checkOption);
        toggle?.onValueChanged.RemoveAllListeners();
        if (checkOption)
        {
            toggle?.onValueChanged.AddListener(OnToggleValueChanged);
            if (toggle != null)
            {
                toggle.isOn = false;
            }
            if (toggle != null)
                agreeBtn.interactable = toggle.isOn;
        }

        if (null == closeBtn)
        {
            Util.LogWarning($"Popup CloseBtn null");
            return;
        }
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(delegate
        {
            this.gameObject.SetActive(false);
        });
        SetButtonText();
    }
    */

    private void OnToggleValueChanged(bool isOn)
    {
        agreeBtn.interactable = isOn;
    }

    public void SetImage(Sprite sprite)
    {
        spriteImage.sprite = sprite;
        spriteAutoSize?.AutoResizeSprite();
    }

    public void Init(UnityAction agreeAction)
    {
        if (null == spriteImage)
            spriteImage = spriteAutoSize?.GetComponent<Image>();
        //this.gameObject.SetActive(true);
        closeBtn?.gameObject.SetActive(false);
        agreeBtn.onClick.RemoveAllListeners();
        agreeBtn.onClick.AddListener(agreeAction);
        if (!string.IsNullOrEmpty(sTitle))
            titleText.text = sTitle;
        if (null != infoTextAsset)
            infoText.text = infoTextAsset.text;
        if (!string.IsNullOrEmpty(sInfo))
            infoText.text = sInfo;

        SetButtonText(sAgreeBtnText, sCloseBtnText);
    }

    /// <summary>
    /// 팝업 초기화
    /// 인스팩터에서 내용추가
    /// </summary>
    /// <param name="agreeAction">확인 버튼 액션</param>
    /// <param name="closeAction">닫기 버튼 액션</param>
    public void Init(UnityAction agreeAction, UnityAction closeAction = null)
    {
        this.gameObject.SetActive(true);
        SetCloseBtn(closeAction);
        SetAgreeBtn(agreeAction);
        
        if (!string.IsNullOrEmpty(sTitle))
            titleText.text = sTitle;
        if (null != infoTextAsset)
            infoText.text = infoTextAsset.text;
        if (!string.IsNullOrEmpty(sInfo))
            infoText.text = sInfo;

        SetButtonText(sAgreeBtnText, sCloseBtnText);
    }

    /// <summary>
    /// 초기화
    /// </summary>
    /// <param name="title">팝업 제목</param>
    /// <param name="textAsset">내용</param>
    /// <param name="agreeAction">확인 버튼 액션</param>
    /// <param name="closeAction">닫기 버튼 액션</param>
    public void Init(string title, TextAsset textAsset, UnityAction agreeAction, UnityAction closeAction)
    {
        this.gameObject.SetActive(true);
        SetCloseBtn(closeAction);
        SetAgreeBtn(agreeAction);
        titleText.text = title;
        infoText.text = textAsset.text;
    }

    /// <summary>
    /// 초기화
    /// </summary>
    /// <param name="title">제목</param>
    /// <param name="info">내용</param>
    /// <param name="agreeAction">확인 버튼 액션</param>
    /// <param name="closeAction">닫기 버튼 액션</param>
    public void Init(string title, string info, UnityAction agreeAction, UnityAction closeAction)
    {
        this.gameObject.SetActive(true);
        SetCloseBtn(closeAction);
        SetAgreeBtn(agreeAction);
        titleText.text = title;
        infoText.text = info;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="title">제목</param>
    /// <param name="agreeAction">확인 버튼 액션</param>
    public void Init(string title, UnityAction agreeAction)
    {
        if (null == spriteImage)
            spriteImage = spriteAutoSize?.GetComponent<Image>();
        //this.gameObject.SetActive(true);
        SetAgreeBtn(agreeAction);
        titleText.text = title;
        //infoText.text = infoTextAsset.text;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title">팝업 제목</param>
    /// <param name="textAsset">내용</param>
    /// <param name="agreeAction">확인 버튼 액션</param>
    public void Init(string title, TextAsset textAsset, UnityAction agreeAction)
    {
        this.gameObject.SetActive(true);
        SetAgreeBtn(agreeAction);
        titleText.text = title;
        infoText.text = textAsset.text;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="title">팝업 제목</param>
    /// <param name="info">내용</param>
    /// <param name="agreeAction">확인 버튼 액션</param>
    public void Init(string title, string info, UnityAction agreeAction)
    {
        this.gameObject.SetActive(true);
        SetAgreeBtn(agreeAction);
        titleText.text = title;
        infoText.text = info;
    }

    /// <summary>
    /// 버튼명 설정
    /// 설정 하지 않으면 확인 및 취소로 자동 설정된다.
    /// </summary>
    /// <param name="agree"></param>
    /// <param name="close"></param>
    public void SetButtonText(string agree = "확인", string close = "취소")
    {
        if (agreeBtnText != null)
        {
            agreeBtnText.text = agree;
        }
        if (closeBtnText != null)
        {
            closeBtnText.text = close;
        }
    }

    public void SetAgreeBtn(UnityAction agreeAction)
    {
        agreeBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                agreeAction?.Invoke();
            }).AddTo(this);
    }

    public void SetCloseBtn(UnityAction closeAction)
    {
        closeBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                closeAction?.Invoke();
            }).AddTo(this);
      
    }
}
