using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class ButtonManager_KKH : MonoBehaviour
    {
        public bool isCompelet = false;
        public bool isFixEnable = true;
        public bool isEnable = true;
        public bool isAutoSkip = false;

        [SerializeField] private float MaxCoolTime = 1.0f;
        [SerializeField] private float coolTime = 0.0f;
        [SerializeField] private AlrtBoxControl alrtBoxControl;
        public AlrtBoxControl AlrtBox => alrtBoxControl;
        [SerializeField] private Button button;
        [SerializeField] private bool isAuto = true;
        private Coroutine coroutine;

        [SerializeField] private GameObject go_ShowPopup;

        public virtual void Awake()
        {
            isCompelet = false;
            if (alrtBoxControl == null)
            {
                alrtBoxControl = GetComponentInChildren<AlrtBoxControl>();
            }

            button = GetComponentInChildren<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SetCompelet());
            }
        }

        private void FixedUpdate()
        {
            if (coroutine != null && MaxCoolTime >= coolTime)
            {
                coolTime += Time.fixedDeltaTime;
            }
        }

        private void OnEnable()
        {
            if (alrtBoxControl != null)
            {
                alrtBoxControl.gameObject.SetActive(false);
            }

            if (CanvasControl.Inst.isSelectMode)
            {
                if (alrtBoxControl != null)
                {
                    if (alrtBoxControl.gameObject.activeInHierarchy)
                    {
                        alrtBoxControl.gameObject.SetActive(false);
                    }
                }

                isCompelet = false;
                isEnable = isFixEnable;
            }
        }

        public void SetCompelet()
        {
            if (isAuto)
            {
                isCompelet = true;
            }

            if (go_ShowPopup != null)
            {
                go_ShowPopup.SetActive(true);
            }
        }

        public void ShowAlrt()
        {
            if (!isEnable) return;
            if (alrtBoxControl == null) return;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            coolTime = 0.0f;
            coroutine = StartCoroutine(AlrtShow());
        }

        public IEnumerator AlrtShow()
        {
            if (alrtBoxControl != null)
            {
                while (alrtBoxControl)
                {
                    yield return new WaitForFixedUpdate();

                    if (isCompelet)
                    {
                        alrtBoxControl.gameObject.SetActive(false);
                        break;
                    }

                    if (coolTime <= MaxCoolTime)
                    {
                        if (!alrtBoxControl.gameObject.activeInHierarchy)
                        {
                            alrtBoxControl.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        alrtBoxControl.gameObject.SetActive(false);
                        break;
                    }
                }
            }

        }
    }
}