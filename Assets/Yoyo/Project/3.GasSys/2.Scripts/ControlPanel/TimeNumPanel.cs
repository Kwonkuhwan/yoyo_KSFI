using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TimeNumPanel : MonoBehaviour
{
    [FormerlySerializedAs("tensText")]
    [SerializeField] private Image tensImg;
    [FormerlySerializedAs("onesText")]
    [SerializeField] private Image onesImg;
    [FormerlySerializedAs("fractionText")]
    [SerializeField] private Image fractionImg;
    [SerializeField] private Sprite[] timeSprites;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private GameObject dotOn;
    // Start is called before the first frame update

    private float _curTime = 0f;
    private bool _isRunning = false;
    // private void Start()
    // {
    //     UpdateDisplay(_curTime);
    //     StartStopwatch();
    // }
    public void Init()
    {
        //SetTime(time);
        tensImg.sprite = offSprite;
        onesImg.sprite = offSprite;
        fractionImg.sprite = offSprite;
        _isRunning = false;
        dotOn.SetActive(false);
        //StartStopwatch();
    }

    public void SetTime(float time)
    {
        _curTime = time;
        dotOn.SetActive(true);
        UpdateDisplay(_curTime);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_isRunning)
            return;
        // 시간이 0이상일 때만 업데이트
        if (_curTime > 0)
        {
            // 시간 감소
            _curTime -= Time.deltaTime * 3.75f;
            UpdateDisplay(_curTime);
        }
        else
        {
            _isRunning = false; // 스톱워치 정지
            _curTime = 0; // 시간이 0이 되면 멈춤
            UpdateDisplay(_curTime);
            ControlPanel.OnTimerEnd?.Invoke();
        }
        ControlPanel.OnTimerCheck?.Invoke(_curTime);
    }
    
    public void StartStopwatch()
    {
        _isRunning = true;
        ControlPanel.OnTimerStart?.Invoke();
    }

    public void ResetStopwatch()
    {
        _isRunning = false;
        UpdateDisplay(_curTime);
    }

    public bool IsRunning()
    {
        return _isRunning;
    }

    
    private void UpdateDisplay(float time)
    {
        // 초 부분 계산 (정수 부분)
        int tens = Mathf.FloorToInt(time) / 10;
        int ones = Mathf.FloorToInt(time) % 10;

        // 소수점 뒤 첫째 자리 계산
        int fraction = Mathf.FloorToInt((time * 10) % 10);

        if (0 > tens)
        {
            tens = 0;
        }
        if (0 > ones)
        {
            ones = 0;
        }
        if (0 > fraction)
        {
            fraction = 0;
        }
        // Text UI 업데이트
        tensImg.sprite = timeSprites[tens];
        onesImg.sprite = timeSprites[ones];
        fractionImg.sprite = timeSprites[fraction];
    }
}
