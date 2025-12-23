using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedTextMeshPro : MonoBehaviour
{
    public string key;  // 번역 데이터의 키
    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
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
        if (LocalizationManager.Instance != null)
        {
            string localizedText = LocalizationManager.Instance.GetLocalizedValue(key);
            textMeshPro.text = localizedText;
        }
    }
}
