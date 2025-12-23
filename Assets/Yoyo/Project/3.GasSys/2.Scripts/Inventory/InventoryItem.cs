using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI itemName;

    public void Init(string itemNameText, Sprite icon)
    {
        itemName.text = itemNameText;
        iconImg.sprite = icon;
    }
}

