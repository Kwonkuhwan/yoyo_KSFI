using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KKH
{
    public class DangerUnitItem_Evaluation : UnitItem
    {
        public DangerDocumentType dDtype;
        protected override void Awake()
        {
            oldPoint = transform.position;
            tr_Panel = transform.parent;
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
                    DangerInven_Evaluation inven = collider.GetComponent<DangerInven_Evaluation>();
                    if (inven != null)
                    {
                        if (inven.SetData(dDtype))
                        {
                            gameObject.SetActive(false);
                        }
                        transform.SetParent(tr_Panel);
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
