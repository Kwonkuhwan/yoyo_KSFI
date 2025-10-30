using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KKH
{
    public class DangerousTransportationUnitItem_Evaluation : UnitItem
    {
        public EDangerousTransportation ddtType;
        protected override void Awake()
        {
            oldPoint = transform.position;
        }

        protected override void OnEnable()
        {
            transform.position = oldPoint;
        }


        public override void OnPointerClick(PointerEventData eventData)
        {
            try
            {
                // 다른 이미지와의 충돌 처리
                Collider2D[] colliders = Physics2D.OverlapBoxAll(GetComponentInChildren<Collider2D>().bounds.center, GetComponentInChildren<Collider2D>().bounds.size, 0);
                foreach (var collider in colliders)
                {
                    DangerousTransportationInven_Evaluation inven = collider.GetComponent<DangerousTransportationInven_Evaluation>();
                    if (inven != null)
                    {
                        inven.SetData(ddtType);
                        gameObject.SetActive(false);
                    }
                }

                transform.position = oldPoint;
            }
            catch
            {
                transform.position = oldPoint;
            }
        }
    }
}