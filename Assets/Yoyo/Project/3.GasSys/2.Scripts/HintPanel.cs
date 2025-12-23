using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI nextText;
    [SerializeField] private TextMeshProUGUI hintText;

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

    public void SetHintTextAndAudio(string hintStr, AudioClip audioClip = null)
    {
        hintText.text = hintStr;
        SoundManager.Instance.PlayHint(audioClip);
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
    
    public void ShowHint(bool show)
    {
        gameObject.SetActive(show);
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
