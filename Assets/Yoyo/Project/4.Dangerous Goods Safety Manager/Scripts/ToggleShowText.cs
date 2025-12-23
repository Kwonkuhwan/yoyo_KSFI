using UnityEngine;
using UnityEngine.UI;

public class ToggleShowText : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private GameObject textObj;
    
    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(ShowText);
    }

    private void ShowText(bool isOn)
    {
        textObj.SetActive(isOn);
    }
}
