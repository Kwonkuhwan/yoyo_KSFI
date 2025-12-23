using System;
using System.Collections;
using System.Collections.Generic;
using LJS;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public class RTypeRSmokeDetectorPopup : MonoBehaviour
{
    public GameObject emptyObj;
    public GameObject defaultObj;
    public GameObject lineOnObj;
    public GameObject lineOnDustObj;
    public Animation removeDustAni;
    public GameObject lineOffObj;
    public GameObject onObj;
    [FormerlySerializedAs("openAniObj")]
    public GameObject openAniObj;
    public GameObject closeAniObj;
    public Animation openAni;
    
    public Button smokeDetectorBtn;
    public Button closeBtn;


    private float _interval = 0.5f;
    [SerializeField] private GameObject[] openAniObjs;
    [SerializeField] private GameObject[] closeAniObjs;
    private int _curIndex = 0;

    private CompositeDisposable _disposable = new CompositeDisposable();
    // Start is called before the first frame update

    public void Init(RTypeRSmokeDetectorPopupType type = RTypeRSmokeDetectorPopupType.Default, Action smokeAction = null, Action closeAction = null)
    {
        ShowSmokeDetector(type);
        GetCloseBtn(closeAction).gameObject.SetActive(false);
        GetSmokeDetectorBtn(smokeAction);
        gameObject.SetActive(true);
    }
    
    public Button GetCloseBtn(Action action = null)
    {
        if(null == action)
            return closeBtn;
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(action.Invoke);
        return closeBtn;
    }

    public Button GetSmokeDetectorBtn(Action action = null)
    {
        if(null == action)
            return smokeDetectorBtn;
        smokeDetectorBtn.onClick.RemoveAllListeners();
        smokeDetectorBtn.onClick.AddListener(action.Invoke);
        return smokeDetectorBtn;
    }

    public void ShowSmokeDetector(RTypeRSmokeDetectorPopupType type)
    {
        switch (type)
        {

            case RTypeRSmokeDetectorPopupType.Default:
                ShowSmokeDetectorObj(defaultObj);
                break;
            case RTypeRSmokeDetectorPopupType.OpenAni:
                ShowSmokeDetectorObj(openAniObj);
                //OpenAni();
                break;
            case RTypeRSmokeDetectorPopupType.CloseAni:
                ShowSmokeDetectorObj(closeAniObj);
                break;
            case RTypeRSmokeDetectorPopupType.Empty:
                ShowSmokeDetectorObj(emptyObj);
                break;
            case RTypeRSmokeDetectorPopupType.LineOn:
                ShowSmokeDetectorObj(lineOnObj);
                break;
            case RTypeRSmokeDetectorPopupType.LineOnDust:
                ShowSmokeDetectorObj(lineOnDustObj);
                removeDustAni.Play();
                break;
            case RTypeRSmokeDetectorPopupType.LineOff:
                ShowSmokeDetectorObj(lineOffObj);
                break;
            case RTypeRSmokeDetectorPopupType.On:
                ShowSmokeDetectorObj(onObj);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void OpenAni(Action openAction)
    {
        _curIndex = 0;
        foreach(var obj in openAniObjs)
        {
            obj.SetActive(false);
        }
        if (openAniObjs.Length > 0)
        {
            openAniObjs[_curIndex].gameObject.SetActive(true);
        }
        
        var t = Observable.Interval(TimeSpan.FromSeconds(_interval))
            .Take(openAniObjs.Length-1) // UIImage 개수만큼 반복
            .Subscribe(index =>
                {
                    SwitchOpenAni();
                },
                () =>
                {
                    openAction.Invoke();
                    Debug.Log("모든 이미지가 활성화되었습니다."); // 완료 메시지
                })
            .AddTo(this); // GameObject와 생명주기 연동
        _disposable.Add(t);
    }

    private void SwitchOpenAni()
    {
        openAniObjs[_curIndex].SetActive(false);
        /*
        Debug.Log($"첫번째 {_curIndex}\n" +
                  $"{openAniObjs[_curIndex].activeSelf}");
                  */
        _curIndex = (_curIndex + 1) % openAniObjs.Length;
        openAniObjs[_curIndex].SetActive(true);
        /*
        Debug.Log($"두번째 {_curIndex}\n" +
                  $"{openAniObjs[_curIndex].activeSelf}");
                  */

        //Debug.Log(_curIndex);
    }

    public void CloseAni(Action closeAction)
    {
        _curIndex = 0;
        foreach(var obj in closeAniObjs)
        {
            obj.SetActive(false);
        }
        if (closeAniObjs.Length > 0)
        {
            closeAniObjs[_curIndex].gameObject.SetActive(true);
        }
        
        var t = Observable.Interval(TimeSpan.FromSeconds(_interval))
            .Take(closeAniObjs.Length-1) // UIImage 개수만큼 반복
            .Subscribe(index =>
                {
                    SwitchCloseAni();
                },
                () =>
                {
                    closeAction.Invoke();
                    Util.Log("모든 이미지가 활성화되었습니다."); // 완료 메시지
                })
            .AddTo(this); // GameObject와 생명주기 연동
        _disposable.Add(t);
    }
    
    private void SwitchCloseAni()
    {
        closeAniObjs[_curIndex].gameObject.SetActive(false);
        _curIndex = (_curIndex + 1) % closeAniObjs.Length;
        closeAniObjs[_curIndex].gameObject.SetActive(true);
        //Debug.Log(_curIndex);
    }

    public void ShowSmokeDetectorObj(GameObject obj)
    {
        emptyObj.SetActive(obj.Equals(emptyObj));
        defaultObj.SetActive(obj.Equals(defaultObj));
        lineOnObj.SetActive(obj.Equals(lineOnObj));
        lineOffObj.SetActive(obj.Equals(lineOffObj));
        onObj.SetActive(obj.Equals(onObj));
        if(openAniObj)
            openAniObj.SetActive(obj.Equals(openAniObj));
        if(closeAniObj)
            closeAniObj.SetActive(obj.Equals(closeAniObj));
        if(lineOnDustObj)
            lineOnDustObj.SetActive(obj.Equals(lineOnDustObj));
    }
}
