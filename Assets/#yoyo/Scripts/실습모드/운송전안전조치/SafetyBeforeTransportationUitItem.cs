using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KKH
{
    public enum SafetyBeforeTransportationType
    {
        고정목 = 0,
        라바콘,
        소화기,
        접지도선,
        주유건
    }

    public class SafetyBeforeTransportationUitItem : UnitItem
    {
        [SerializeField] private SafetyBeforeTransportationType sbtType;
        [SerializeField] private bool isAutoDisable = true;
        [SerializeField] private bool isNextIndex = false;

        protected override void Awake()
        {
            base.Awake();

        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            transform.position = oldPoint;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            try
            {
                // 다른 이미지와의 충돌 처리
                SafetyBeforeTransportationInven inven = collision.gameObject.GetComponent<SafetyBeforeTransportationInven>();
                if (inven != null)
                {
                    if (inven.SetImage(sbtType))
                    {
                        inven.GetComponent<ButtonManager_KKH>().isCompelet = true;

                        if (isNextIndex)
                        {
                            SafetyBeforeTransportationInfoMenu.Inst.NextIndex();
                        }

                        if (isAutoDisable)
                        {
                            gameObject.SetActive(false);
                        }
                        else
                        {
                            Button btn = GetComponent<Button>();
                            if (btn != null)
                            {
                                GetComponent<Button>().interactable = true;
                                this.enabled = false;
                            }
                        }
                    }
                    else
                    {
                        go_Error.SetActive(true);
                    }
                }


                if (gameObject.activeInHierarchy)
                {
                    transform.position = oldPoint;
                }
            }
            catch
            {
                transform.position = oldPoint;
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            //try
            //{
            //    // 다른 이미지와의 충돌 처리
            //    Collider2D[] colliders = Physics2D.OverlapBoxAll(GetComponentInChildren<Collider2D>().bounds.center, GetComponentInChildren<Collider2D>().bounds.size, 0);
            //    foreach (var collider in colliders)
            //    {
            //        SafetyBeforeTransportationInven inven = collider.GetComponent<SafetyBeforeTransportationInven>();
            //        if (inven != null)
            //        {
            //            if (inven.SetImage(sbtType)) 
            //            {
            //                inven.GetComponent<ButtonManager_KKH>().isCompelet = true;

            //                if (isNextIndex)
            //                {
            //                    SafetyBeforeTransportationInfoMenu.Inst.NextIndex();
            //                }

            //                if (isAutoDisable)
            //                {
            //                    gameObject.SetActive(false);
            //                }
            //                else
            //                {
            //                    Button btn = GetComponent<Button>();
            //                    if (btn != null)
            //                    {
            //                        GetComponent<Button>().interactable = true;
            //                        this.enabled = false;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                go_Error.SetActive(true);
            //            }
            //        }
            //    }

            //    if (gameObject.activeInHierarchy)
            //    {
            //        transform.position = oldPoint;
            //    }
            //}
            //catch
            //{
            //    transform.position = oldPoint;
            //}
        }
    }
}