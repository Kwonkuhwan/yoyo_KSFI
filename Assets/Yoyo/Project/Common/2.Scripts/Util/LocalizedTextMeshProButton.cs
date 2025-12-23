using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LocalizedTextMeshProButton : MonoBehaviour
{
    public string key;  // 번역 데이터의 키
    private TextMeshProUGUI buttonText;

    private void Awake()
    {
        // Button 컴포넌트에서 TextMeshProUGUI 찾기
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText == null)
        {
            Debug.LogError("TextMeshProUGUI component is missing in the Button's child.");
        }
    }

    private void OnEnable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.LanguageChanged += UpdateText;
            UpdateText();
        }
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.LanguageChanged -= UpdateText;
        }
    }

    public void UpdateText()
    {
        if (LocalizationManager.Instance != null && buttonText != null)
        {
            string localizedText = LocalizationManager.Instance.GetLocalizedValue(key);
            buttonText.text = localizedText;
        }
    }
}
