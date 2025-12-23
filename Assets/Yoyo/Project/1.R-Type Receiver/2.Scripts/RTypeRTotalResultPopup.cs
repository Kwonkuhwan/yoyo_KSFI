using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RTypeRTotalResultPopup : MonoBehaviour
{
    [SerializeField] private GameObject resultObj;
    [SerializeField] private Transform parent;
    [SerializeField] private Button mainBtn;
    [SerializeField] private Button modeBtn;
    [SerializeField] private ResultBoxObj[] resultBoxList;
    //[SerializeField] private RTypeRSection secion;
    private RTypeRTotalScore _totalScore;
    public void Init()
    {
        _totalScore = RTypeRGlobalCanvas.Instance.totalScore;
        gameObject.SetActive(true);
        mainBtn.onClick.RemoveAllListeners();
        modeBtn.onClick.RemoveAllListeners();
        mainBtn.onClick.AddListener(delegate
        {
            //최상단으로 이동
            RTypeRMenu.Instance.Init();
            gameObject.SetActive(false);
            RTypeRGlobalCanvas.Instance.totalScore.ResetData();
        });
        modeBtn.onClick.AddListener(delegate
        {
            RTypeRGlobalCanvas.Instance.ShowMenuPopup(true);
            RTypeRGlobalCanvas.Instance.totalScore.ResetData();
        });


        int count = _totalScore.화재경보List.Count(obj => obj.IsSuccess);

        ResultType resultType;
        if (count == _totalScore.화재경보List.Count)
            resultType = ResultType.성공;
        else if (0 == count)
            resultType = ResultType.실패;
        else
            resultType = ResultType.보류;
        
        
        resultBoxList[0].Init(new ResultObject()
        {
            resultType = resultType,
            IsSuccess = 2 <= count,
            Title = "화재경보"
        }, () =>
        {
            RTypeRGlobalCanvas.Instance.SetResultPopup(_totalScore.화재경보List, "화재경보");
        });
  

        count = _totalScore.회로차단List.Count(obj => obj.IsSuccess);
        if (count == _totalScore.회로차단List.Count)
            resultType = ResultType.성공;
        else if (0 == count)
            resultType = ResultType.실패;
        else
            resultType = ResultType.보류;
        resultBoxList[1].Init(new ResultObject()
        {
            IsSuccess = 2 <= count,
            resultType = resultType,
            Title = "회로단선"
        }, () =>
        {
            RTypeRGlobalCanvas.Instance.SetResultPopup(_totalScore.회로차단List, "회로단선");
        });
        
        count = _totalScore.설비동작List.Count(obj => obj.IsSuccess);
        if (count == _totalScore.설비동작List.Count)
            resultType = ResultType.성공;
        else if (0 == count)
            resultType = ResultType.실패;
        else
            resultType = ResultType.보류;
        resultBoxList[2].Init(new ResultObject()
        {
            IsSuccess = 2 <= count,
            resultType = resultType,
            Title = "설비작동"
        }, () =>
        {
            RTypeRGlobalCanvas.Instance.SetResultPopup(_totalScore.설비동작List, "설비작동");
        });
    }
}
