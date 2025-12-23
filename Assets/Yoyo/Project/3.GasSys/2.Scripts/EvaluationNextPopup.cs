using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationNextPopup : MonoBehaviour
{
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button reTestBtn;
    [SerializeField] private TextMeshProUGUI nextBtnText;
    [SerializeField] private TextMeshProUGUI decoText;
    public void SetNextEvaluation(Action action, Action reAction, bool complete = false)
    {
        this.gameObject.SetActive(true);
        nextBtn.onClick.RemoveAllListeners();
        reTestBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.AddListener(delegate
        {
            action?.Invoke();
            this.gameObject.SetActive(false);
        });
        reTestBtn.onClick.AddListener(delegate
        {
            reAction?.Invoke();
            this.gameObject.SetActive(false);
        });
        if (complete)
        {
            nextBtnText.text = "결과확인";
            decoText.text = "하단의 버튼을 통해 결과를 확인하세요.";
        }
        else
        {
            nextBtnText.text = "다음 평가";
            decoText.text = "하단의 버튼을 통해 다음 평가를 진행하세요.";
        }
    }
    

    private void OnEnable()
    {
        SoundManager.Instance.StopAllFireSound();
        SoundManager.Instance.StopHint();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
