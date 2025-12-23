using LJS;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuButtonObj : MonoBehaviour
{
    [SerializeField] private Button agreeBtn;
    [SerializeField] private TextMeshProUGUI agreeText;
    private CompositeDisposable _btnDisposable = new CompositeDisposable();

    public void Init(string btnName, Sprite bgSprite = null)
    {
        if(null != bgSprite)
            agreeBtn.GetComponent<Image>().sprite = bgSprite;
        agreeText.text = btnName;
        string regexName = Util.RemoveWhitespaceUsingRegex(btnName);
        this.gameObject.name = $"MenuButton({regexName})";
    }

    public void SetButton(UnityAction action)
    {
        _btnDisposable?.Clear();
        var disposable = agreeBtn?.OnClickAsObservable().Subscribe(_ =>
        {
            Debug.Log(agreeText.text);
            action?.Invoke();
            
        }).AddTo(this);
        _btnDisposable?.Add(disposable);
    }

    public Button GetButton()
    {
        return agreeBtn;
    }
}
