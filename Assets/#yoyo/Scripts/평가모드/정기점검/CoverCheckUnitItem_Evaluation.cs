using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KKH 
{
    public class CoverCheckUnitItem_Evaluation : UnitItem
    {
        public CoverCheckType ccType;
        protected override void Awake()
        {
            oldPoint = transform.localPosition;
            tr_Panel = transform.parent;
        }

        protected override void OnEnable()
        {
            transform.localPosition = oldPoint;
        }


        public override void OnPointerClick(PointerEventData eventData)
        {
            try
            {
                // 다른 이미지와의 충돌 처리
                Collider2D[] colliders = Physics2D.OverlapBoxAll(GetComponentInChildren<Collider2D>().bounds.center, GetComponentInChildren<Collider2D>().bounds.size, 0);
                foreach (var collider in colliders)
                {
                    CoverCheckInven_Evaluation inven = collider.GetComponent<CoverCheckInven_Evaluation>();
                    if (inven != null)
                    {
                        inven.SetData(ccType);
                        transform.SetParent(tr_Panel);
                        gameObject.SetActive(false);
                    }
                }

                if (gameObject.activeInHierarchy)
                {
                    transform.localPosition = oldPoint;
                }
            }
            catch
            {
                transform.localPosition = oldPoint;
            }
        }
    }
}