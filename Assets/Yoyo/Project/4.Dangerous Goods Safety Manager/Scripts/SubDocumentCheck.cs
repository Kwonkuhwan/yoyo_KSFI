using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubDocumentCheck : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image exampleImage;
    [SerializeField] private Image exampleImage2;
    [SerializeField] private Button confirmButton;


    public void Awake()
    {
        confirmButton.onClick.AddListener(CloseSubDocument);
    }

    public void CloseSubDocument()
    {
        gameObject.SetActive(false);
    }

    public void SetOnSubDocument(string title, Sprite sprite, Sprite sprite2)
    {
        gameObject.SetActive(true); 
        titleText.text = title; 
        exampleImage.sprite = sprite;   
        exampleImage2.sprite = sprite2;
    }
}
