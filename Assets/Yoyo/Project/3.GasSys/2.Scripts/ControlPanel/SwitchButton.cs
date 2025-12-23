using System;
using LJS;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField, Multiline] private string strName;
    [SerializeField] private Image checkMarkImage;
    [SerializeField] private CanvasGroup canvasGroup;

    private bool _checked = false;
    [SerializeField] private float fadeDuration = 0.5f; // 페이드 지속 시간
    [SerializeField] private float delayBetweenFades = 0.1f; // 페이드 간 딜레이
    private IDisposable _fadeLoopDisposable;
    public void Init(UnityAction<string, bool> action, bool isChecked)
    {
      
        if (string.IsNullOrEmpty(strName))
        {
            if (!string.IsNullOrEmpty(Util.GetObjectName(gameObject.name)))
            {
                nameText.text = Util.GetObjectName(gameObject.name);
            }
        }
        else
        {
            nameText.text = strName;
        }
        
        btn.onClick.RemoveAllListeners();
        if (null != action)
        {
            btn.onClick.AddListener(delegate
            {
                if(!nameText.text.Equals("복구"))
                    OnCheck(!_checked);
                else
                {
                    OnCheck(false);
                }

                SetLog(nameText.text, _checked);
                action?.Invoke(nameText.text, _checked);
            });
        }
        if (SceneManager.GetActiveScene().name.Equals("1.R-TypeReceiver"))
        {
            //HandleButtonClick();
            ToggleFadeLoop();
        }


        OnCheck(isChecked);
    }

    public void SetMode(bool isMode)
    {
        btn.interactable = isMode;
    }
    
    public void OnCheck(bool isChecked)
    {
        if (_checked.Equals(isChecked))
            return;
        _checked = isChecked;
        checkMarkImage.gameObject.SetActive(_checked);
        //onValueChangeEvent?.Invoke(panelName, _checked);
        if (SceneManager.GetActiveScene().name.Equals("GasSysScene"))
        {
            string isOn = !isChecked ? "On" : "Off";
            string panelName = Util.RemoveWhitespaceUsingRegex(Util.GetObjectName(nameText.text));
            if (panelName.Equals("축적/비축적"))
            {
                isOn = !isChecked ? "축적" : "비축적";
                panelName = "";
            }
            //ControlPanel.Instance.SetReceiverLog($"{panelName} {isOn}");
        }
        else if (SceneManager.GetActiveScene().name.Equals("1.R-TypeReceiver"))
        {
            
            string isOn = !isChecked ? "출력" : "정지";
            string panelName = Util.RemoveWhitespaceUsingRegex(Util.GetObjectName(nameText.text));
            if (panelName.Equals("축적소등/비축적점등"))
            {
                isOn = !isChecked ? "축적" : "비축적";
                panelName = "";
            }
            if (panelName.Equals("수신기복구"))
            {
                RTypeRPanel.Instance.SetFireAlarmEventNum(0);
                RTypeRPanel.Instance.SetEquipmentOperationEventNum(0);
            }
            //RTypeRPanel.Instance.SetLog($"{panelName} {isOn}");
        }
    }

    private void SetLog(string text, bool isChecked)
    {
        if (SceneManager.GetActiveScene().name.Equals("GasSysScene"))
        {
            string isOn = !isChecked ? "On" : "Off";
            string panelName = Util.RemoveWhitespaceUsingRegex(Util.GetObjectName(nameText.text));
            if (panelName.Equals("축적/비축적"))
            {
                isOn = !isChecked ? "축적" : "비축적";
                panelName = "";
            }
            ControlPanel.Instance.SetReceiverLog($"{panelName} {isOn}");
        }
        else if (SceneManager.GetActiveScene().name.Equals("1.R-TypeReceiver"))
        {
            
            string isOn = !isChecked ? "출력" : "정지";
            string panelName = Util.RemoveWhitespaceUsingRegex(Util.GetObjectName(nameText.text));
            if (panelName.Equals("축적소등/비축적점등"))
            {
                isOn = !isChecked ? "축적" : "비축적";
                panelName = "";
            }
            RTypeRPanel.Instance.SetLog($"{panelName} {isOn}");
        }
    }

    public bool IsChecked()
    {
        return _checked;
    }

    public Button GetButton()
    {
        return btn;
    }
    
    private void ToggleFadeLoop()
    {
        if (_fadeLoopDisposable == null)
        {
            // FadeIn/FadeOut 반복 시작
            _fadeLoopDisposable = StartFadeLoop()
                .Subscribe()
                .AddTo(this);
        }
        else
        {
            // FadeIn/FadeOut 반복 중지
            _fadeLoopDisposable.Dispose();
            _fadeLoopDisposable = null;
        }
    }

    private IObservable<Unit> StartFadeLoop()
    {
        return Observable.Defer(() =>
                Fade(1, 0) // FadeOut
                    .Concat(UniRx.Observable.Timer(TimeSpan.FromSeconds(delayBetweenFades)).AsUnitObservable()) // 딜레이
                    .Concat(Fade(0, 1)) // FadeIn
                    .Concat(UniRx.Observable.Timer(TimeSpan.FromSeconds(delayBetweenFades)).AsUnitObservable()) // 딜레이
        ).Repeat(); // 무한 반복
    }

    private IObservable<Unit> Fade(float startAlpha, float endAlpha)
    {
        return Observable.EveryUpdate()
            .Select(_ => Time.deltaTime)
            .Scan(0f, (elapsed, delta) => elapsed + delta)
            .TakeWhile(elapsed => elapsed < fadeDuration)
            .Do(elapsed =>
            {
                // Alpha 값 변경
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            })
            .AsUnitObservable()
            .DoOnCompleted(() =>
            {
                // 최종 Alpha 값 설정
                canvasGroup.alpha = endAlpha;
            });
    }
    // private void HandleButtonClick()
    // {
    //     // 버튼 클릭 시 FadeOut -> Delay -> FadeIn 실행
    //     Fade(1, 0) // Fade Out
    //         .Concat(UniRx.Observable.Timer(TimeSpan.FromSeconds(delayBetweenFades)).AsUnitObservable()) // 딜레이
    //         .Concat(Fade(0, 1)) // Fade In
    //         .Subscribe()
    //         .AddTo(this);
    // }
    //
    // private IObservable<Unit> Fade(float startAlpha, float endAlpha)
    // {
    //     return Observable.EveryUpdate()
    //         .Select(_ => Time.deltaTime)
    //         .Scan(0f, (elapsed, delta) => elapsed + delta)
    //         .TakeWhile(elapsed => elapsed < fadeDuration)
    //         .Do(elapsed =>
    //         {
    //             // Alpha 값 변경
    //             canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
    //         })
    //         .AsUnitObservable()
    //         .DoOnCompleted(() =>
    //         {
    //             // 최종 Alpha 값 설정
    //             canvasGroup.alpha = endAlpha;
    //         });
    // }
}
