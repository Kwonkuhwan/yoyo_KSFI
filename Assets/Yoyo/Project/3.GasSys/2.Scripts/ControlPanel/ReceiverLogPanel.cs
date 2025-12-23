using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReceiverLogPanel : MonoBehaviour
{
    [SerializeField] private Button menuBtn;
    [SerializeField] private Button upBtn;
    [SerializeField] private Button downBtn;
    [SerializeField] private Button selectBtn;
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform logTextParent;
    private static readonly StringBuilder ReceiverLogList = new StringBuilder();

    public GameObject logTextObj;
    
    private List<GameObject> _logTextObjList = new List<GameObject>(); 

    public float lineHeight = 25f; // 한 줄의 높이 (텍스트 크기에 맞게 설정)

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        
        upBtn.onClick.RemoveAllListeners();
        downBtn.onClick.RemoveAllListeners();
        selectBtn.onClick.RemoveAllListeners();
        
        upBtn.onClick.AddListener( ScrollUp);
        downBtn.onClick.AddListener( ScrollDown);
        // selectBtn.onClick.AddListener(delegate
        // {
        //     SceneManager.LoadScene("TitleScene");
        // });
    }

    public void ClearLogText()
    {
        foreach(GameObject obj in _logTextObjList)
        {
            Destroy(obj);
        }
        _logTextObjList.Clear();
    }
    
    public void SetLog(string log)
    {
        ReceiverLogList.AppendLine($"<color=black>{log}</color>");
        var obj = GameObject.Instantiate(logTextObj, logTextParent);
        obj.GetComponent<LogTextObj>().SetText($"<color=black>{log}</color>");
        _logTextObjList.Add(obj);
    }
    
    private void ScrollUp()
    {
        // 스크롤 위치를 25픽셀 위로 이동
        float scrollableHeight = (logTextParent.childCount * lineHeight) - scrollRect.viewport.rect.height;
        float newYPosition = scrollRect.verticalNormalizedPosition + (lineHeight / scrollableHeight);
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newYPosition);
    }

    private void ScrollDown()
    {
        // 스크롤 위치를 25픽셀 아래로 이동
        float scrollableHeight = (logTextParent.childCount * lineHeight) - scrollRect.viewport.rect.height;
        float newYPosition = scrollRect.verticalNormalizedPosition - (lineHeight / scrollableHeight);
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newYPosition);
    }
}
