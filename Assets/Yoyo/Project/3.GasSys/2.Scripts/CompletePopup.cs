using System;
using GASSYS;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CompletePopup : MonoBehaviour
{
#region singleton

    private static CompletePopup instance;
    public static CompletePopup Instance { get { return instance; } }

#endregion
    [SerializeField] private TextMeshProUGUI completeText;
    [SerializeField] private TextMeshProUGUI nextText;
    [SerializeField] private TextMeshProUGUI defaultText;
    [SerializeField] private TextMeshProUGUI nextBtnText;
    [FormerlySerializedAs("mainBtn")]
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button modeBtn;
    [FormerlySerializedAs("stepBtn")]
    [SerializeField] private Button prevBtn;
    [SerializeField] private GameObject panelObj;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        Init();
        //this.gameObject.SetActive(false);
    }


    public void Init()
    {
        modeBtn.onClick.RemoveAllListeners();
        modeBtn.onClick.AddListener(delegate
        {
            if (SceneManager.GetActiveScene().name.Equals("GasSysScene"))
            {
                GasSysGlobalCanvas.Instance.ShowMenuPopup(true);
            }
            if (SceneManager.GetActiveScene().name.Equals("1.R-TypeReceiver"))
            {
                RTypeRGlobalCanvas.Instance.ShowMenuPopup(true);
            }
            if (SceneManager.GetActiveScene().name.Equals("GasSysIScene"))
            {
                GasSysIMenuPopup.Instance.ShowMenuPanel(true);
                //GasSysIMenuPopup.Instance
            }
        });
        SetNextBtn();

    }

    public void SetNextBtn()
    {
        nextBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.AddListener(delegate
        {
            //최상단으로 이동
            if (SceneManager.GetActiveScene().name.Equals("GasSysScene"))
            {
                GasSysManager.Instance.Init();
                GasSysGlobalCanvas.Instance.HideCheckObj();
            }
            else if (SceneManager.GetActiveScene().name.Equals("1.R-TypeReceiver"))
            {
                RTypeRMenu.Instance.Init();
                RTypeRGlobalCanvas.Instance.HideCheckObj();
            }
            if (SceneManager.GetActiveScene().name.Equals("GasSysIScene"))
            {
                GasSysIMenu.Instance.Init();
                GasSysIHintPanel.Instance.ShowHint(false);
            }
            gameObject.SetActive(false);
        });
    }

    public void SetPrevStepBtn(UnityAction action)
    {
        prevBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.AddListener(action);
    }

    public void SetNextStepBtn(Action action)
    {
        nextBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.AddListener(delegate
        {
            SoundManager.Instance.StopAllFireSound();
            SoundManager.Instance.SetDefaultVolume();
            action?.Invoke();
        });
    }

    public void SetCompleteText(string completeStr, string nextStr = "", Action action = null)
    {
        completeText.text = $"{completeStr}";
        if (null != action)
        {
            if (nextText)
            {
                nextText.gameObject.SetActive(true);
                defaultText.gameObject.SetActive(false);
                nextText.text = string.IsNullOrEmpty(nextStr) ? "" : $"다음 단계 버튼을 눌러 {nextStr} 진행하세요.";
                nextBtnText.text = "다음 단계";
            }
            SetNextStepBtn(action);
        }
        else
        {
            if (nextText)
            {
                nextText.gameObject.SetActive(false);
                nextText.text = string.IsNullOrEmpty(nextStr) ? "" : $"다음 단계 버튼을 눌러 {nextStr} 진행하세요.";
                //nextBtn.interactable = false;
                defaultText.gameObject.SetActive(true);
                nextBtnText.text = "메인화면";
            }
            SetNextStepBtn(SetNextBtn);
        }
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }
    public void ShowCompletePopup(bool isShow)
    {
        panelObj.SetActive(isShow);
        if (isShow)
            SoundManager.Instance.StopHint();
        //Debug.Log($"완료 팝업 : {gameObject.activeSelf}");
    }

    public Button[] GetButtons()
    {
        return new Button[] { nextBtn, modeBtn, prevBtn };
    }

    public bool IsShowCompletePopup()
    {
        return panelObj.activeSelf;
    }

}
