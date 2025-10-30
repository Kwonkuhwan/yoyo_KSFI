using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KKH
{
    public class CoverCheckUnit_Evaluation : UnitItem
    {
        [SerializeField] private Image image;
        //[SerializeField] private Button button;
        public CoverCheckType cCType;
        public bool isEnable = false;
        [SerializeField] private Sprite sprite_Default;
        [SerializeField] private Sprite sprite_Active;

        protected override void Awake()
        {
            base.Awake();

            image = GetComponent<Image>();
            //button = GetComponent<Button>();
            //button.onClick.AddListener(() => BtnClick());
        }

        public void SetUnit(CoverCheckType dDtype)
        {
            this.cCType = dDtype;
            isEnable = true;
            image.sprite = sprite_Active;

        }

        //private void BtnClick()
        //{
        //    transform.parent.GetComponent<CoverCheckInven_Evaluation>().Rollback(dDtype);
        //    isEnable = false;
        //    //gameObject.SetActive(false);
        //    image.sprite = sprite_Default;
        //}

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
                        if (!inven.SetData(cCType)) break;
                        transform.SetParent(tr_Panel);
                        gameObject.SetActive(false);
                    }

                    CoverCheckSelectInven_Evaluation selectInven = collider.GetComponent<CoverCheckSelectInven_Evaluation>();
                    if (selectInven != null)
                    {
                        if (!selectInven.Rollback(cCType)) break;

                        //cCType = CoverCheckType.None;
                        isEnable = false;
                        image.sprite = sprite_Default;
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