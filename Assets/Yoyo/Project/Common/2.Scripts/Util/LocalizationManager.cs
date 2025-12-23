using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    private Dictionary<string, string> _localizedText;

    public delegate void OnLanguageChanged();
    public event OnLanguageChanged LanguageChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLocalizedText(CurrentLanguage);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLocalizedText(string language)
    {
        _localizedText = new Dictionary<string, string>();
        CurrentLanguage = language;

        TextAsset data = Resources.Load<TextAsset>("LocalizationData");
        if (data == null)
        {
            Debug.LogError("LocalizationData.json not found in Resources folder.");
            return;
        }

        var jsonData = JsonUtility.FromJson<LocalizationDataWrapper>(data.text);

        if (jsonData.Languages.ContainsKey(language))
        {
            foreach (var kvp in jsonData.Languages[language])
            {
                _localizedText.Add(kvp.Key, kvp.Value);
            }
        }
        else
        {
            Debug.LogError($"Language '{language}' not found in LocalizationData.");
        }

        // 언어 변경 이벤트 호출
        LanguageChanged?.Invoke();
    }

    public string GetLocalizedValue(string key)
    {
        if (_localizedText.ContainsKey(key))
        {
            return _localizedText[key];
        }
        else
        {
            Debug.LogWarning($"Localization key '{key}' not found.");
            return key;
        }
    }

    public string CurrentLanguage
    {
        get;
        private set;
    } = "en";
}

[System.Serializable]
public class LocalizationDataWrapper
{
    public Dictionary<string, Dictionary<string, string>> Languages;
    public LocalizationDataWrapper(Dictionary<string, Dictionary<string, string>> languages)
    {
        this.Languages = languages;
    }
}
