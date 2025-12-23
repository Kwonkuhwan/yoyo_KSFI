using RJH.DangerousGoods;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RJH
{
    public class ButtonController : MonoBehaviour
    {
        [SerializeField] private GameObject checkObjSetactive;
        [SerializeField] private DocsButton docsButton;
        [SerializeField] private Button button;
        [SerializeField] private Image buttonImage;
        [SerializeField] private TextMeshProUGUI buttonText;
        [Space(20)]
        [SerializeField] private ButtonSprite buttonSprite;
        [SerializeField] private ButtonTextColor buttonTextColor;

        [SerializeField] private bool isButtonInteractable;
        private void Awake()
        {
            buttonImage.sprite = buttonSprite.inactiveSprite;
            buttonText.color = buttonTextColor.inactiveSprite;
            button = GetComponent<Button>();
            button.interactable = isButtonInteractable;
        }

        private void Update()
        {
            if(docsButton.isChecked == false)
            {
                SetInteractable(true);
            }
            else
            {
                SetInteractable(false);
            }
        }

        public void SetInteractable(bool isInteractable)
        {
            //button.interactable = isInteractable;
            
            if (isInteractable)
            {
                buttonImage.sprite = buttonSprite.defaultSprite;
                buttonText.color = buttonTextColor.defaultColor;
            }
            else
            {
                buttonImage.sprite = buttonSprite.inactiveSprite;
                buttonText.color = buttonTextColor.inactiveSprite;
            }
        }

        private void OnDisable()
        {
            SetInteractable(false);
        }
    }

    [Serializable]
    public class ButtonSprite
    {
        public Sprite defaultSprite;
        public Sprite inactiveSprite;

    }

    [Serializable]
    public class ButtonTextColor
    {
        public Color defaultColor;
        public Color inactiveSprite;
    }


}


