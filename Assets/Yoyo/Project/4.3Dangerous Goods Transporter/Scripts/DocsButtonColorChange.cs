using RJH.Transporter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DocsButtonColorChange : MonoBehaviour
{
    [SerializeField] private DocsButton docsButton;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI text;
    [Header("버튼")]
    [SerializeField] private Sprite activeButtonColor;
    [SerializeField] private Sprite unactiveButtonColor;
    [Header("텍스트")]
    [SerializeField] private Color activeTextColor;
    [SerializeField] private Color unactiveTextColor;

    private void Update()
    {
        if (!docsButton.isChecked == false)
        {
            buttonImage.sprite = unactiveButtonColor;
            text.color = unactiveTextColor;
            
        }
        else
        {
            buttonImage.sprite = activeButtonColor;
            text.color = activeTextColor;
        }

    }
}
