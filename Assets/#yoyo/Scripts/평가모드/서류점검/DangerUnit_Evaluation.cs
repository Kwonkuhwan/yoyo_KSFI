using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KKH
{
    public class DangerUnit_Evaluation : UnitItem
    {
        //[SerializeField] private Button button;
        [SerializeField] private Image image;
        public DangerDocumentType dDtype;
        public bool isEnable = false;

        public Sprite sprite_Empty;
        public Sprite[] sprites;

        protected override void Awake()
        {
            base.Awake();
            image = GetComponent<Image>();
            //button = GetComponent<Button>();
            //button.onClick.AddListener(() => BtnClick());
        }

        public void SetUnit(DangerDocumentType dDtype)
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
                    DangerInven_Evaluation inven = collider.GetComponent<DangerInven_Evaluation>();
                    if (inven != null)
                    {
                        if (!inven.SetData(dDtype)) break;
                        transform.SetParent(tr_Panel);
                        gameObject.SetActive(false);
                    }

                    DangerSelectInven_Evaluation selectInven = collider.GetComponent<DangerSelectInven_Evaluation>();
                    if (selectInven != null)
                    {
                        if (!selectInven.Rollback(dDtype)) break;

                        dDtype = DangerDocumentType.None;
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
