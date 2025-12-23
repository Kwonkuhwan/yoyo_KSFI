using UnityEngine;
using UnityEngine.EventSystems;

namespace KKH
{
    public class EmergencyResponsUnitItem_Evaluation : UnitItem
    {
        public EEmergencyResponsType eERType;
        protected override void Awake()
        {
            base.Awake();
            //oldPoint = transform.localPosition;
            //tr_Panel = tr_Panel.parent;
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
                    EmergencyResponsUnit_Evaluation unit = collider.GetComponent<EmergencyResponsUnit_Evaluation>();
                    if (unit != null)
                    {
                        unit.SetData(eERType);
                        transform.SetParent(tr_Panel);
                        gameObject.SetActive(false);
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
    }
}