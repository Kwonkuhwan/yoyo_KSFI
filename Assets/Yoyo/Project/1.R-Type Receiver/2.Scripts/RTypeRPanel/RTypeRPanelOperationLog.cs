using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;
/// <summary>
/// 운영 기록
/// </summary>
public partial class RTypeRPanel
{
    [Foldout("운영기록 검색")]
    [SerializeField] public Button searchBtn;
    [SerializeField] public Button[] numBtns;
    [SerializeField] public Button acBtn;
    [SerializeField] public Button deleteBtn;
    [SerializeField] public Button closeBtn;
    [SerializeField] public Button allBtn;
    [SerializeField] public TextMeshProUGUI searchText;
    [SerializeField] public string dateText;
    public string inputString = "________";
    public string defaultString = "20250909";
    
    [Foldout("운영기록 결과")]
    [SerializeField] public Button upBtn;
    [SerializeField] public Button downBtn;
    [SerializeField] public ScrollRect scrollRect;
    [SerializeField] private Transform logTextParent;
    [SerializeField] public GameObject logTextObj;



    private static readonly StringBuilder ReceiverLogList = new StringBuilder();
    private readonly List<GameObject> _logList = new List<GameObject>();
    public float lineHeight = 67.5f;
    static public readonly UnityEvent<bool> OnCheckDate = new UnityEvent<bool>();
    static public readonly UnityEvent OnLogResultCheck = new UnityEvent();
    [EndFoldout]
    
    private List<string> _numList = new List<string>();
    private char[] dateArray = "00000000".ToCharArray();
    private StringBuilder _sb = new StringBuilder();
    public void InitOperationLog()
    {
        upBtn.onClick.RemoveAllListeners();
        downBtn.onClick.RemoveAllListeners();

        int index = 0;
        foreach (var btn in numBtns)
        {
            btn.onClick.RemoveAllListeners();
            //btn.onClick.AddListener(delegate { OnNumberButtonPressed(index.ToString()); });
            SetNum(btn, index);
            index++;
        }
        
        deleteBtn.onClick.RemoveAllListeners();
        deleteBtn.onClick.AddListener(OnDeleteButtonPressed);
        searchBtn.onClick.RemoveAllListeners();
        searchBtn.onClick.AddListener(SearchOperation);
        
        acBtn.onClick.RemoveAllListeners();
        acBtn.onClick.AddListener(OnClearButtonPressed);
        upBtn.onClick.AddListener(ScrollUp);
        downBtn.onClick.AddListener(ScrollDown);
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(delegate
        {
            ShowScreen(mainScreenImg);
        });
        scrollRect.verticalNormalizedPosition = 1f;
        OnClearButtonPressed();
        ClearLog();
    }

    public Button GetOperationLogAllBtn(Action action = null)
    {
        if (null == action)
            return allBtn;
        allBtn.onClick.RemoveAllListeners();
        allBtn.onClick.AddListener(action.Invoke);
        return allBtn;
    }

    public Button GetSearchBtn(Action action = null)
    {
        if(null == action)
            return searchBtn;
        searchBtn.onClick.RemoveAllListeners();
        searchBtn.onClick.AddListener(action.Invoke);
        return searchBtn;
    }

    public Button GetUpBtn(Action action = null)
    {
        if(null == action)
            return upBtn;
        upBtn.onClick.RemoveAllListeners();
        upBtn.onClick.AddListener(action.Invoke);
        return upBtn;
    }

    public Button GetDownBtn(Action action = null)
    {
        if(null == action)
            return downBtn;
        downBtn.onClick.RemoveAllListeners();
        downBtn.onClick.AddListener(action.Invoke);
        return downBtn;
    }

    public void SetNum(Button btn, int index)
    {
        btn.onClick.AddListener(delegate
        {
            OnNumberButtonPressed(index.ToString());
        });
    }
    
    public void OnNumberButtonPressed(string number)
    {
        if (!IsValidInput(number))
            return;
        if (inputString.Length > 8)
            return;
        _numList.Add(number);
        var index = _numList.Count-1;
        _sb[index] = number[0];
        inputString = _sb.ToString();
        //int.Parse(currentInput.Substring(0, Mathf.Min(4, currentInput.Length)));
        //inputString[index] = _numList[^1];
        UpdateDisplay();
    }
    
    public void UpdateDisplay()
    {
        searchText.text = FormatDate(inputString); // 현재 입력된 값을 표시
        OnCheckDate?.Invoke(inputString.Equals(defaultString));
    }

#region 검색

    public void ShowOperationLogSearch(bool isShow)
    {
        operationSearchObj.SetActive(isShow);
    }

    public void SearchOperation()
    {
        if (!IsValidDate(FormatDate(inputString)))
            inputString = defaultString;

        foreach (var logItem in _logList)
        {
            logItem.GetComponent<OperationLogItem>().SetDate(FormatDate(inputString));
        }
        ShowScreen(operationResultObj);
        
    }

    public void OnClearButtonPressed()
    {
        _numList.Clear();
        inputString = "________";
        _sb.Clear();
        for(int i=0; i<8; ++i)
        {
            _sb.Append("_");
        }
        UpdateDisplay();
    }
    
    public void OnDeleteButtonPressed()
    {
        if (inputString.Length <= 0)
            return;
        if (0 == _numList.Count)
            return;
        // 마지막 문자 제거
        var index = _numList.Count - 1;
        _sb[index] = '_';
        _numList.RemoveAt(_numList.Count - 1);
        inputString = _sb.ToString();
        UpdateDisplay();
    }

    private string FormatDate(string date)
    {
        if (string.IsNullOrEmpty(date))
            date = "________";
        //date = Convert.ToInt32(date).ToString("D8");
        string year = date.Substring(0, 4);
        string month = date.Substring(4, 2);
        string day = date.Substring(6, 2);
        
        return  $"{year}-{month}-{day}";
    }
    
    private bool IsValidDate(string date)
    {
        // 유효성 검사할 형식 지정
        string format = "yyyy-MM-dd";
        
        // TryParseExact로 검증
        return DateTime.TryParseExact(
            date,                 // 입력된 문자열
            format,               // 요구되는 형식
            null,                 // 문화권 정보 (기본값 사용)
            System.Globalization.DateTimeStyles.None, // 스타일 옵션
            out _                 // 성공 시 변환된 DateTime (사용하지 않음)
        );
    }
    
    private bool IsValidInput(string input)
    {
        return int.TryParse(input, out _); // 입력값이 숫자인지 확인
    }
#endregion


#region 결과

    public void ShowOperationLogResult(bool isShow)
    {
        operationResultObj.SetActive(isShow);
    }

    public void SetLog(string log)
    {
        // 형식 지정하여 문자열로 변환
        string formattedTime = DateTime.Now.ToString("HH:mm:ss");
        ReceiverLogList.AppendLine($"<color=white>{formattedTime}  {log}</color>");
        var obj = Instantiate(logTextObj, logTextParent);
        obj.GetComponent<OperationLogItem>().SetInfo(formattedTime, log);
        _logList.Add(obj);
    }

    public void ClearLog()
    {
        foreach (var obj in _logList)
        {
            Destroy(obj);
        }
        ReceiverLogList.Clear();
        _logList.Clear();
    }
    public void ScrollUp()
    {
        // 스크롤 위치를 25픽셀 위로 이동
        float scrollableHeight = (logTextParent.childCount * lineHeight) - scrollRect.viewport.rect.height;
        float newYPosition = scrollRect.verticalNormalizedPosition + (lineHeight / scrollableHeight);
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newYPosition);
    }

    public void ScrollDown()
    {
        // 스크롤 위치를 25픽셀 아래로 이동
        float scrollableHeight = (logTextParent.childCount * lineHeight) - scrollRect.viewport.rect.height;
        float newYPosition = scrollRect.verticalNormalizedPosition - (lineHeight / scrollableHeight);
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newYPosition);
        if (0 >= (int)(scrollRect.verticalNormalizedPosition*10))
        {
            OnLogResultCheck?.Invoke();
        }
    }

#endregion


}
