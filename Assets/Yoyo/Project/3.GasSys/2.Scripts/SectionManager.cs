using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SectionManager : MonoBehaviour
{
#region singleton
    public static SectionManager Instance {get; private set;}
#endregion
    [SerializeField] private GasSysSection[] safetySections;

    [SerializeField] private Button nextBtn;
    [SerializeField] private Button prevBtn;
    
    [SerializeField] private Image nextImg;
    [SerializeField] private TextMeshProUGUI nextText;
    
    [SerializeField] private Image prevImg;
    [SerializeField] private TextMeshProUGUI prevText;
    private int _curSection = 0;
    private int _minSection = 0;
    private int _maxSection = 0;
    private void Awake()
    {
        Initialized();
    }
    public void Initialized()
    {
        if (null == Instance)
            Instance = this;
    }

    public void Init()
    {
        nextBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.AddListener(Next);
        prevBtn.onClick.AddListener(Prev);
        foreach (var section in safetySections)
        {
            section.gameObject.SetActive(false);
        }
        GasSysGlobalCanvas.Instance.ShowHint(false);
    }
    // Start is called before the first frame update
    public void InitSafety()
    {
        Init();
        _curSection = 0;
        SetSectionRange(0, safetySections.Length, safetySections.Length);
        ShowSection(_curSection);
        UpdateBtn();
    }

    public void SetNextBtn(UnityAction action)
    {
        nextBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.AddListener(delegate
        {
            Next();
            action?.Invoke();
        });
    }
    
    public void NextEnable()
    {
        //ButtonManager.Instance.NextEnable();
        ColorBlock colors = nextBtn.colors;
        //colors.disabledColor = colors.normalColor;
        nextImg.color = colors.normalColor;
        nextText.color =colors.normalColor;
    }

    public void NextDisable()
    {
        //ButtonManager.Instance.NextDisable();
        ColorBlock colors = nextBtn.colors;
        //colors.disabledColor = colors.normalColor;
        nextImg.color = colors.disabledColor;
        nextText.color =colors.disabledColor;
    }

    public void SetPrevBtn(UnityAction action)
    {
        prevBtn.onClick.RemoveAllListeners();
        prevBtn.onClick.AddListener(delegate
        {
            Prev();
            action?.Invoke();
        });
    }
    
    public void Prev()
    {
        if (_curSection <= 0)
            return;
        _curSection--;
        ShowSection(_curSection);
        UpdateBtn();
    }

    public void Next()
    {
        if (_curSection >= safetySections.Length - 1)
            return;
        _curSection++;
        ShowSection(_curSection);
        UpdateBtn();
    }

    private void ShowSection(int index)
    {
        //safetySections[index].Init();
    }
    
    public void SetSectionRange(int minIndex, int maxIndex, int value)
    {
        _minSection = Mathf.Clamp(minIndex, 0, value - 1); // 최소 인덱스가 범위를 벗어나지 않도록 제한
        _maxSection = Mathf.Clamp(maxIndex, 0, value - 1); // 최대 인덱스도 제한
        _curSection = _minSection; // 범위 내에서 처음 페이지로 이동
        ShowSection(_curSection);
        UpdateBtn();
    }
    


    private void UpdateBtn()
    {
        prevBtn.interactable = _curSection > _minSection; // 첫 번째 페이지일 때 이전 버튼 비활성화
        nextBtn.interactable = _curSection < _maxSection; // 마지막 페이지일 때 다음 버튼 비활성화

        
        if (!prevBtn.interactable)
        {   ColorBlock colors = prevBtn.colors;
            //colors.disabledColor = colors.normalColor;
            prevImg.color = colors.disabledColor;
            prevText.color =colors.disabledColor;
        }
        else
        {
            ColorBlock colors = prevBtn.colors;
            //colors.disabledColor = colors.normalColor;
            prevImg.color = colors.normalColor;
            prevText.color =colors.normalColor;
        }
        
        if (_minSection.Equals(_maxSection))
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
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
