using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GasSysIHintPanel : MonoBehaviour
{
#region singleton
    private static GasSysIHintPanel instance;
    public static GasSysIHintPanel Instance { get { return instance; } }
#endregion
    
    [SerializeField] public GameObject hintObj;
    [SerializeField] public TextMeshProUGUI titleText;
    [SerializeField] public TextMeshProUGUI nextText;
    [SerializeField] public TextMeshProUGUI prevText;
    [SerializeField] public TextMeshProUGUI hintText;
    [SerializeField] public Button nextBtn;
    [SerializeField] public Image nextImg;

    [SerializeField] public Button prevBtn;
    [SerializeField] public Image prevImg;

    public int _curSection = 0;
    [HideInInspector] public int _minSection = 0;
    [HideInInspector] public int _maxSection = 0;
    
    public static UnityEvent<int> onChangeSection = new UnityEvent<int>();
    
    public Animator animator;

    public HintScriptableObj curHintObj;
    
    [SerializeField] private Sprite[] hintSoundOnOff;
    [SerializeField] private Image hintSoundBtnImage;
    [SerializeField] private Button hintSoundBtn;

    //[SerializeField] private Button prevBtn;
    //[SrializeField] private Button nextBtn;

    //[SerializeField] private RectTransform rect;

    //private HintScriptableObj _hintScriptable;

    //private List<string> _hits = new List<string>();
    //private List<HintTextAndAudio> _hintData = new List<HintTextAndAudio>();
    //private int _curPage = 0;
    //private int _minPage = 0;
    //private int _maxPage = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (!hintSoundBtn)
            return;
        hintSoundBtn.onClick.RemoveAllListeners();
        hintSoundBtnImage.sprite = SoundManager.Instance.hintSource.mute ? hintSoundOnOff[1] : hintSoundOnOff[0];
        hintSoundBtn.onClick.AddListener(delegate
        {
            if (SoundManager.Instance.hintSource.mute)
            {
                SoundManager.Instance.hintSource.mute = false;
                hintSoundBtnImage.sprite = hintSoundOnOff[0];
            }
            else
            {
                SoundManager.Instance.hintSource.mute = true;
                hintSoundBtnImage.sprite = hintSoundOnOff[1];
            }
        });
    }

    public void Init(HintScriptableObj obj, Action nextAction = null, UnityAction prevAction = null)
    {
        if (null == instance)
            instance = this;
        curHintObj = obj;
        nextBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.AddListener(delegate
        {
            nextAction?.Invoke();
            Next();
        });
        prevBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.AddListener(delegate
        {
            prevAction?.Invoke();
            Prev();
        });
    }

    public void SetHintTextAndAudio(HintScriptableObj obj)
    {
        curHintObj = obj;
    }

    
    public void SetHintPopup(HintTextAndAudio obj)
    {
        if(hintSoundBtn)
            hintSoundBtnImage.sprite = SoundManager.Instance.hintSource.mute ? hintSoundOnOff[1] : hintSoundOnOff[0];
        //_hits.Clear();
        // prevBtn.onClick.RemoveAllListeners();
        // nextBtn.onClick.RemoveAllListeners();
        //
        // prevBtn.onClick.AddListener(Prev);
        // nextBtn.onClick.AddListener(Next);

        titleText.text= obj.title;
        hintText.text = obj.text;
        
        // 2024-11-18 이재성
        // 힌트 사운드 음소거
        SoundManager.Instance.PlayHint(obj.audioClip);
        
        //UpdateBtn();
    }

    public void SetCompleteHint()
    {
        if (string.IsNullOrEmpty(hintText.text))
            return;
        if(!hintText.text.Contains("(완료)"))
        {
            hintText.text = $"{hintText.text} <color=black><b>(완료)</b></color>";
        }
    }
    
    public void ShowHint(bool isShow)
    {
        hintObj.SetActive(isShow);
    }

    private HintTextAndAudio GetHintTextAndAudio(HintScriptableObj hintObj, int index)
    {
        return new HintTextAndAudio()
        {
            title = hintObj.hintData[index].title,
            text = hintObj.hintData[index].text,
            audioClip = hintObj.hintData[index].audioClip
        };
    }


    public void NextDisable()
    {
        //ButtonManager.Instance.NextDisable();
        nextBtn.interactable = false;
        ColorBlock colors = nextBtn.colors;
        //colors.disabledColor = colors.normalColor;
        nextImg.color = colors.disabledColor;
        nextText.color = colors.disabledColor;
    }

    public void NextEnable(bool isHighlight = true, bool isCompleteHint = true)
    {
        //ButtonManager.Instance.NextEnable();
        nextBtn.interactable = true;
        ColorBlock colors = nextBtn.colors;
        //colors.disabledColor = colors.normalColor;
        nextImg.color = colors.normalColor;
        nextText.color = colors.normalColor;
        if (isCompleteHint)
            RTypeRGlobalCanvas.Instance.SetCompleteHint();
        if (isHighlight)
        {
            ButtonManager.Instance.HighlightButton(nextBtn);
        }
    }

    public void SetSectionRange(int minIndex, int maxIndex, int value)
    {
        _minSection = Mathf.Clamp(minIndex, 0, value - 1); // 최소 인덱스가 범위를 벗어나지 않도록 제한
        _maxSection = Mathf.Clamp(maxIndex, 0, value - 1); // 최대 인덱스도 제한
        _curSection = _minSection; // 범위 내에서 처음 페이지로 이동
        UpdateBtn();
        SetSection(_curSection);
        onChangeSection.Invoke(_curSection);
        UpdateHint();
    }



    public void Prev()
    {
        if (_curSection <= 0)
            return;
        _curSection--;
        onChangeSection.Invoke(_curSection);
        SetSection(_curSection);
        UpdateHint();
        UpdateBtn();
    }

    public void Next()
    {
        if (_curSection >= _maxSection)
            return;
        _curSection++;
        onChangeSection.Invoke(_curSection);
        SetSection(_curSection);
        UpdateHint();
        UpdateBtn();
        
    }

    public void UpdateHint()
    {
        titleText.text = curHintObj.hintData[_curSection].title;
        hintText.text = curHintObj.hintData[_curSection].text;
  
        //SoundManager.Instance.PlayHint(curHintObj.hintData[_curSection].audioClip);
        // 2024-11-18 이재성
        // 힌트 사운드 음소거
        SoundManager.Instance.PlayHint(curHintObj.hintData[_curSection].audioClip);
        //GasSysSoundManager.Instance.PlayHint(obj.audioClip);
    }

    private void UpdateBtn()
    {
        prevBtn.interactable = _curSection > _minSection; // 첫 번째 페이지일 때 이전 버튼 비활성화
        //nextBtn.interactable = _curSection < _maxSection; // 마지막 페이지일 때 다음 버튼 비활성화


        if (!prevBtn.interactable)
        {
            ColorBlock colors = prevBtn.colors;
            //colors.disabledColor = colors.normalColor;
            prevImg.color = colors.disabledColor;
            prevText.color = colors.disabledColor;
        }
        else
        {
            ColorBlock colors = prevBtn.colors;
            //colors.disabledColor = colors.normalColor;
            prevImg.color = colors.normalColor;
            prevText.color = colors.normalColor;
        }

        // if (_minSection.Equals(_maxSection))
        // {
        //     prevBtn.gameObject.SetActive(false);
        //     nextBtn.gameObject.SetActive(false);
        // }
        // else
        // {
        //     prevBtn.gameObject.SetActive(true);
        //     nextBtn.gameObject.SetActive(true);
        // }
    }

    private void SetSection(int index)
    {

        // switch (curMainSection)
        // {
        //
        //     case RTypeRMainSection.Init:
        //         break;
        //     case RTypeRMainSection.EquipmentOperation:
        //         //ChangeStateSafetyCheck(GasSysSafetyCheckSection.선택밸브조작동관선택 + index);
        //         //GlobalCanvas.Instance.SetHintPopup(GetHintTextAndAudio(safetyCheckHintObj, (int)GasSysSafetyCheckSection.선택밸브조작동관선택 + index));
        //         SetEquipmentOperationSection(index);
        //         break;
        //     case RTypeRMainSection.FireAlarmSystem:
        //         //ShowSolenoidValveTestSection(index);
        //         SetFireAlarmSystemSection(index);
        //         break;
        //     case RTypeRMainSection.CircuitBreaker:
        //         //ShowDischargeLightTestSection(index);
        //         SetCircuitBreakerSection(index);
        //         break;
        //     default:
        //         throw new ArgumentOutOfRangeException();
        // }
        // //safetySections[index].Init();
    }


    /*
    public void SetHintPopup(HintScriptableObj obj, RectTransform pos)
    {
        _hintData.Clear();
        //_hits.Clear();
        _hintScriptable = obj;
        prevBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.RemoveAllListeners();

        prevBtn.onClick.AddListener(Prev);
        nextBtn.onClick.AddListener(Next);

        rect.position = pos.position;
        titleText.text = _hintScriptable.hintTitle[0];
        _hintData.AddRange(_hintScriptable.hintData);
        //_hits.AddRange(_hintScriptable.hintTexts);
        SetPageRange(0, _hintData.Count);
        ShowPageAndAudio(_curPage);
        UpdateBtn();
    }

    public void SetHintPopup(int min, int max, HintScriptableObj obj, RectTransform pos)
    {
        _hintData.Clear();
        _hintScriptable = obj;
        prevBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.RemoveAllListeners();

        prevBtn.onClick.AddListener(Prev);
        nextBtn.onClick.AddListener(Next);

        rect.position = pos.position;
        titleText.text = _hintScriptable.hintTitle[0];
        _hintData.AddRange(_hintScriptable.hintData); 
        //_hits.AddRange(_hintScriptable.hintTexts);
        SetPageRange(min, max);
        ShowPageAndAudio(_curPage);
        UpdateBtn();
    }

    public void SetPageRange(int minIndex, int maxIndex)
    {
        _minPage = Mathf.Clamp(minIndex, 0, _hintData.Count - 1); // 최소 인덱스가 범위를 벗어나지 않도록 제한
        _maxPage = Mathf.Clamp(maxIndex, 0, _hintData.Count - 1); // 최대 인덱스도 제한
        _curPage = _minPage; // 범위 내에서 처음 페이지로 이동
        ShowPageAndAudio(_curPage);
        UpdateBtn();
    }

    private void Prev()
    {
        if (_curPage <= 0)
            return;
        _curPage--;
        ShowPageAndAudio(_curPage);
        UpdateBtn();
    }

    private void Next()
    {
        if (_curPage >= _hintData.Count - 1)
            return;
        _curPage++;
        ShowPageAndAudio(_curPage);
        UpdateBtn();
    }

    private void ShowPage(int index)
    {
        hintText.text = _hits[index];
    }

    private void ShowPageAndAudio(int index)
    {
        hintText.text = _hintData[index].text;
        SoundManager.Instance.PlayHint(_hintData[index].audioClip);
    }

    private void UpdateBtn()
    {
        prevBtn.interactable = _curPage > _minPage; // 첫 번째 페이지일 때 이전 버튼 비활성화
        nextBtn.interactable = _curPage < _maxPage; // 마지막 페이지일 때 다음 버튼 비활성화
        if (_minPage.Equals(_maxPage))
        {
            prevBtn.gameObject.SetActive(false);
            nextBtn.gameObject.SetActive(false);
        }
        else
        {
            prevBtn.gameObject.SetActive(true);
            nextBtn.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        //prevBtn.onClick.RemoveAllListeners();
        //nextBtn.onClick.RemoveAllListeners();
    }

    
    */

}
