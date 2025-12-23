using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class ToggleControl : MonoBehaviour
{
    [Foldout("이미지들")]
    [SerializeField] private Sprite disableImage;
    [SerializeField] private Sprite eableImage;

    [Foldout("토글 관련")]
    public bool isOn = false;
    [SerializeField] private Image toggleImg;


    private void Awake()
    {
        toggleImg = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        if (toggleImg == null) return;
        if (!isOn)
        {
            toggleImg.sprite = disableImage;
        }
        else
        {
            toggleImg.sprite = eableImage;
        }
    }
}
