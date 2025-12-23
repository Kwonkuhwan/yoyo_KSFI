using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KKH
{
    public class DangerousTransportationUnit_Evaluation : UnitItem
    {
        [SerializeField] private bool isRigth = false;
        [SerializeField] private Image image;
        public EDangerousTransportation dDtype;
        public bool isEnable = false;

        public Sprite sprite_Empty;
        public Sprite[] sprites;

        protected override void Awake()
        {
            base.Awake();
            image = GetComponent<Image>();
        }

        public void SetUnit(EDangerousTransportation dDtype)
        {
            this.dDtype = dDtype;
            isEnable = true;
            image.sprite = sprites[(int)dDtype];
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
                        if (!inven.SetData(dDtype)) break;
                        if (transform.parent.GetComponent<DangerousTransportationInven_Evaluation>().dttList.Remove(dDtype))
                        {
                            isEnable = false;
                        }
                        transform.SetParent(tr_Panel);
                        gameObject.SetActive(false);
                    }

                    DangerousTransportationSelectInven_Evaluation selectInven = collider.GetComponent<DangerousTransportationSelectInven_Evaluation>();
                    if (selectInven != null)
                    {
                        if (!selectInven.Rollback(dDtype, isRigth)) break;

                        dDtype = EDangerousTransportation.None;
                        isEnable = false;
                        image.sprite = sprite_Empty;
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