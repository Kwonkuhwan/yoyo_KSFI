using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RJH.DangerousGoods;

public class ShowObjectButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private GameObject checkObj;

    private void Awake()
    {
        //button = GetComponent<Button>();
        //button.onClick.AddListener(ShowObject);
        GetComponent<DocsButton>().docsAction += ShowObject;
    }
     
    public void SetText(string title, string subText)
    {
        buttonText.text = title;
        
        checkObj.GetComponent<TextMeshProUGUI>().text = subText;
    }

    public void ShowObject()
    {
        checkObj.SetActive(true);
    }

    public void HideObject()
    {
        checkObj.SetActive(false);
    }

    public bool CheckActiveObject()
    {
        return checkObj.activeSelf;
    }
}
