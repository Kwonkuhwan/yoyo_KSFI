using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KKH
{
    public enum CarryOnInspection
    {
        물질안전보건자료 = 0,
        방독마스크,
        메가폰,
        흡착포,
        삼각대,
        적색기
    }
    public class CarryOnInspectionMoveImage : UnitItem
    {
        public CarryOnInspection coi;

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            transform.position = oldPoint;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            try
            {
                CarryOnInspectionInven inven = collision.gameObject.GetComponent<CarryOnInspectionInven>();
                if (inven != null)
                {
                    if (coi == CarryOnInspection.물질안전보건자료)
                    {
                        inven.isMSDS = true;
                        gameObject.SetActive(false);
                    }
                    else if (coi == CarryOnInspection.메가폰)
                    {
                        inven.isMegaphone = true;
                        gameObject.SetActive(false);
                    }
                    else if (coi == CarryOnInspection.흡착포)
                    {
                        inven.isAdsorptionCloth = true;
                        gameObject.SetActive(false);
                    }
                    else if (coi == CarryOnInspection.적색기)
                    {
                        inven.isRedFlag = true;
                        gameObject.SetActive(false);
                    }
                    else if(coi == CarryOnInspection.삼각대)
                    {
                        inven.isTripod = true;
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        inven.IncreaseErrorCount();
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
            //        CarryOnInspectionInven inven = collider.GetComponent<CarryOnInspectionInven>();
            //        if(inven != null)
            //        {
            //            if(coi == CarryOnInspection.물질안전보건자료)
            //            {
            //                inven.isMSDS = true;
            //                gameObject.SetActive(false);
            //            }
            //            else if(coi == CarryOnInspection.메가폰)
            //            {
            //                inven.isMegaphone = true;
            //                gameObject.SetActive(false);
            //            }
            //            else if(coi == CarryOnInspection.흡착포)
            //            {
            //                inven.isAdsorptionCloth = true;
            //                gameObject.SetActive(false);
            //            }
            //            else if(coi == CarryOnInspection.적색기)
            //            {
            //                inven.isRedFlag = true;
            //                gameObject.SetActive(false);
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