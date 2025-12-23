using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RJH.Transporter
{
    public class DocsButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] public Image buttonImage;
        [SerializeField] private DocsButton nextButton;
        [SerializeField] private EmphasisEffect emphasisEffect;
        [SerializeField] private ClickEffect clickEffect;
        [SerializeField] private GameObject marker;

        public bool isChecked = true;
        public bool isSkippable = false;
        public bool noCheckMode = false;
        public float transitionDuration = 1.0f;
        public UnityAction docsAction;

        private bool isClicked = true;

        public void SetButton()
        {
            if (button != null)
            {
                button.onClick.AddListener(OnClick);
                button.enabled = false;
            }
            else
            {
                buttonImage.raycastTarget = false;
            }
            isChecked = true;
        }

        public void SetClickAble()
        {
            isChecked = false;
            if (button != null)
                button.enabled = true;
            else
            {
                buttonImage.raycastTarget = true;
            }

            if (emphasisEffect == EmphasisEffect.Enable)
            {
                gameObject.SetActive(true);
            }

            if (!this.gameObject.activeInHierarchy)
                return;
        }

        public void OnEnable()
        {
            if (isChecked == true)
                return;
        }

        public virtual void OnClick()
        {
            if (isChecked == true)
                return;

            isChecked = true;

            if (button != null)
                button.enabled = false;

            if (emphasisEffect == EmphasisEffect.Enable)
                gameObject.SetActive(false);

            switch (clickEffect)
            {
                case ClickEffect.MarkerImage:
                    marker.SetActive(true);
                    break;
                case ClickEffect.Disable:
                    gameObject.SetActive(false);
                    break;
                case ClickEffect.MarkerDisable:
                    marker.SetActive(false);
                    break;
                case ClickEffect.MarkerImageAndDisable:
                    marker.SetActive(true);
                    gameObject.SetActive(false);
                    break;
            }

            if (nextButton != null)
            {
                nextButton.SetClickAble();
            }
            docsAction?.Invoke();

            if (isClicked && noCheckMode == false)
            {
                SectionAndBackGroundManager.Instance.OnNextDocument2();
            }
            else
                isClicked = true;
        }

        public void OnClickedImage()
        {

        }

        public virtual void OnRevert()
        {
            isChecked = true;

            if (button != null)
                button.enabled = false;
            else
                buttonImage.raycastTarget = false;

            if (emphasisEffect == EmphasisEffect.Enable)
                gameObject.SetActive(false);
        }

        private IEnumerator LoopColorTransition()
        {
            Color whiteColor = Color.white;
            Color yellowColor = Color.yellow;

            while (true)
            {
                // 흰색에서 노란색으로 전환
                yield return StartCoroutine(ChangeColor(whiteColor, yellowColor));
                // 노란색에서 흰색으로 전환
                yield return StartCoroutine(ChangeColor(yellowColor, whiteColor));
            }
        }

        private IEnumerator ChangeColor(Color fromColor, Color toColor)
        {
            float elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                buttonImage.color = Color.Lerp(fromColor, toColor, elapsed / transitionDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            buttonImage.color = toColor;
        }

        private IEnumerator LoopAlphaTransition()
        {
            Color AlphaZeroColor = buttonImage.color;
            Color AlphaOneColor = buttonImage.color;

            AlphaOneColor.a = 0.7f;
            AlphaZeroColor.a = 0f;

            while (true)
            {
                // 흰색에서 노란색으로 전환
                yield return StartCoroutine(ChangeColor(AlphaOneColor, AlphaZeroColor));
                // 노란색에서 흰색으로 전환
                yield return StartCoroutine(ChangeColor(AlphaZeroColor, AlphaOneColor));
            }
        }

        public void OnClicked()
        {
            isClicked = false;

            button?.onClick.Invoke();
        }

        private void OnValidate()
        {
            button = GetComponent<Button>();
        }

        public bool MarkerImageActivate()
        {
            return marker.activeSelf;
        }

        public void SetMarkerImageOff()
        {
            marker.SetActive(false);
        }
    }
}


