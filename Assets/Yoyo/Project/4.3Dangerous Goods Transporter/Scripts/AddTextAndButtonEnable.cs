using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddTextAndButtonEnable : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private string text;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private GameObject setEnableButton;

    private void Awake()
    {
        button.onClick.AddListener(AddTextInTMP);
    }

    private void AddTextInTMP()
    {
        textMeshPro.text += text;
        setEnableButton.SetActive(true);
    }
}
