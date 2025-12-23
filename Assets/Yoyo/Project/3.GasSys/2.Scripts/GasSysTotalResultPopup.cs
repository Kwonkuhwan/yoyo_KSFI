using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using GASSYS;
public class GasSysTotalResultPopup : MonoBehaviour
{
    [SerializeField] private GameObject resultObj;
    [SerializeField] private Transform parent;
    [SerializeField] private Button mainBtn;
    [SerializeField] private Button modeBtn;
    [SerializeField] private ResultBoxObj[] resultBoxList;
    
    private GasSysTotalScore _totalScore;
    public void Init()
    {
        SoundManager.Instance.StopHint();
        _totalScore = GasSysGlobalCanvas.Instance.totalScore;
        gameObject.SetActive(true);
        mainBtn.onClick.RemoveAllListeners();
        modeBtn.onClick.RemoveAllListeners();
        mainBtn.onClick.AddListener(delegate
        {
            //최상단으로 이동
            
            GasSysManager.Instance.Init();
            gameObject.SetActive(false);
            GasSysGlobalCanvas.Instance.totalScore.ResetData();
        });
        modeBtn.onClick.AddListener(delegate
        {
            GasSysGlobalCanvas.Instance.ShowMenuPopup(true);
            GasSysGlobalCanvas.Instance.totalScore.ResetData();
        });

        int count = _totalScore.점검전안전조치List.Count(obj => obj.IsSuccess);
        ResultType resultType;
        if (count == _totalScore.점검전안전조치List.Count)
            resultType = ResultType.성공;
        else if (0 == count)
            resultType = ResultType.실패;
        else
            resultType = ResultType.보류;
        resultBoxList[0].Init(new ResultObject()
        {
            IsSuccess = 3 <= count,
            resultType = resultType,
            Title = "점검 전 안전조치"
        }, () =>
        {
            GasSysGlobalCanvas.Instance.SetResultPopup(_totalScore.점검전안전조치List, "점검 전 안전조치 결과");
        });
        
        count = _totalScore.즉시격발List.Count(obj => obj.IsSuccess);
        if (count == _totalScore.즉시격발List.Count)
            resultType = ResultType.성공;
        else if (0 == count)
            resultType = ResultType.실패;
        else
            resultType = ResultType.보류;
        resultBoxList[1].Init(new ResultObject()
        {
            IsSuccess = 3 <= count,
            resultType = resultType,
            Title = "수동조작버튼 작동[즉시격발] 격발시험"
        }, () =>
        {
            GasSysGlobalCanvas.Instance.SetResultPopup(_totalScore.즉시격발List, "수동조작버튼 작동[즉시격발] 시험 결과");
        });
        
        count = _totalScore.수동조작함작동List.Count(obj => obj.IsSuccess);
        if (count == _totalScore.수동조작함작동List.Count)
            resultType = ResultType.성공;
        else if (0 == count)
            resultType = ResultType.실패;
        else
            resultType = ResultType.보류;
        resultBoxList[2].Init(new ResultObject()
        {
            IsSuccess = 3 <= count,
            resultType = resultType,
            Title = "수동조작함 작동 격발시험"
        }, () =>
        {
            GasSysGlobalCanvas.Instance.SetResultPopup(_totalScore.수동조작함작동List, "수동조작함 작동 격발시험 결과");
        });
        
        count = _totalScore.교차회로List.Count(obj => obj.IsSuccess);
        if (count == _totalScore.교차회로List.Count)
            resultType = ResultType.성공;
        else if (0 == count)
            resultType = ResultType.실패;
        else
            resultType = ResultType.보류;
        resultBoxList[3].Init(new ResultObject()
        {
            IsSuccess = 4 <= count,
            resultType = resultType,
            Title = "교차회로 감지기 작동 격발시험"
        }, () =>
        {
            GasSysGlobalCanvas.Instance.SetResultPopup(_totalScore.교차회로List, "교차회로 감지기 작동 격발시험 결과");
        });
        
        count = _totalScore.스위치동작List.Count(obj => obj.IsSuccess);
        if (count == _totalScore.스위치동작List.Count)
            resultType = ResultType.성공;
        else if (0 == count)
            resultType = ResultType.실패;
        else
            resultType = ResultType.보류;
        resultBoxList[4].Init(new ResultObject()
        {
            IsSuccess = 3 <= count,
            resultType = resultType,
            Title = "제어반 수동조작 스위치 작동 격발시험"
        }, () =>
        {
            GasSysGlobalCanvas.Instance.SetResultPopup(_totalScore.스위치동작List, "제어반 수동조작 스위치 작동 격발시험 결과");
        });
        
        count = _totalScore.방출표시등List.Count(obj => obj.IsSuccess);
        if (count == _totalScore.방출표시등List.Count)
            resultType = ResultType.성공;
        else if (0 == count)
            resultType = ResultType.실패;
        else
            resultType = ResultType.보류;
        resultBoxList[5].Init(new ResultObject()
        {
            IsSuccess = 3 <= count,
            resultType = resultType,
            Title = "방출표시등 작동시험"
        }, () =>
        {
            GasSysGlobalCanvas.Instance.SetResultPopup(_totalScore.방출표시등List,"방출표시등 작동시험 결과");
        });
        
        count = _totalScore.점검완료후복구List.Count(obj => obj.IsSuccess);
        if (count == _totalScore.점검완료후복구List.Count)
            resultType = ResultType.성공;
        else if (0 == count)
            resultType = ResultType.실패;
        else
            resultType = ResultType.보류;
        resultBoxList[6].Init(new ResultObject()
        {
            IsSuccess = 4 <= count,
            resultType = resultType,
            Title = "점검 후 복구"
        }, () =>
        {
            GasSysGlobalCanvas.Instance.SetResultPopup(_totalScore.점검완료후복구List, "점검 후 복구 결과");
        });
    }
}
