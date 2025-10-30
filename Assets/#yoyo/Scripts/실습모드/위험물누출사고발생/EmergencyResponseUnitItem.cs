using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace KKH
{
    public enum EmergencyResponseType
    {
        삼각대 = 0,
        적색봉,
        메가폰,
        모래주머니,
        흡착포,
        None
    }

    public class EmergencyResponseUnitItem : UnitItem
    {
        public EmergencyResponseType erType;
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
                EmergencyResponseInven inven = collision.gameObject.GetComponent<EmergencyResponseInven>();
                if (inven != null)
                {
                    if (inven.SetImage(erType))
                    {
                        inven.GetComponent<ButtonManager_KKH>().isCompelet = true;

                        if (isNextIndex)
                        {
                            if (isAutoDisable)
                            {
                                gameObject.SetActive(false);
                            }

                            SafetyBeforeTransportationInfoMenu.Inst.NextIndex();
                        }
                        else
                        {
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
                    }
                    else
                    {
                        go_Error.SetActive(true);
                    }
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
            //        EmergencyResponseInven inven = collider.GetComponent<EmergencyResponseInven>();
            //        if (inven != null)
            //        {
            //            if (inven.SetImage(erType))
            //            {
            //                inven.GetComponent<ButtonManager_KKH>().isCompelet = true;

            //                if (isNextIndex)
            //                {
            //                    SafetyBeforeTransportationInfoMenu.Inst.NextIndex();
            //                }
            //                else
            //                {
            //                    if (isAutoDisable)
            //                    {
            //                        gameObject.SetActive(false);
            //                    }
            //                    else
            //                    {
            //                        Button btn = GetComponent<Button>();
            //                        if (btn != null)
            //                        {
            //                            GetComponent<Button>().interactable = true;
            //                            this.enabled = false;
            //                        }
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