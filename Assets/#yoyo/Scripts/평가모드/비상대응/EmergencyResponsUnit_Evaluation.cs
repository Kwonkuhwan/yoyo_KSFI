using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KKH
{
    public class EmergencyResponsUnit_Evaluation : UnitItem
    {
        [SerializeField] private Image image;

        public EEmergencyResponsType eERType;
        public bool isEnable = false;

        public Sprite sprite_Empty;
        public Sprite[] sprites;

        protected override void Awake()
        {
            base.Awake();

            image = GetComponent<Image>();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eERType == EEmergencyResponsType.None) return;
            base.OnPointerDown(eventData);
        }

        public void SetData(EEmergencyResponsType eERType)
        {
            this.eERType = eERType;
            if (EEmergencyResponsType.None == eERType)
            {
                image.sprite = sprite_Empty;

            }
            else
            {
                image.sprite = sprites[(int)eERType];
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            try
            {
                // 다른 이미지와의 충돌 처리
                Collider2D[] colliders = Physics2D.OverlapBoxAll(GetComponentInChildren<Collider2D>().bounds.center, GetComponentInChildren<Collider2D>().bounds.size, 0);
                foreach (var collider in colliders)
                {
                    EmergencyResponsInven_Evaluation inven = collider.GetComponent<EmergencyResponsInven_Evaluation>();
                    if (inven != null)
                    {
                        //if (!inven.SetData(dDtype)) break;
                        transform.SetParent(tr_Panel);
                        //gameObject.SetActive(false);
                        break;
                    }

                    EmergencyResponsUnit_Evaluation unit = collider.GetComponent<EmergencyResponsUnit_Evaluation>();
                    if (unit != null && unit.gameObject != gameObject)
                    {
                        EEmergencyResponsType temp = unit.eERType;
                        unit.SetData(eERType);
                        SetData(temp);
                        break;
                    }

                    EmergencyResponseSelectInven selectInven = collider.GetComponent<EmergencyResponseSelectInven>();
                    if (selectInven != null)
                    {
                        if (!selectInven.Rollback(eERType)) break;

                        SetData(EEmergencyResponsType.None);
                        isEnable = false;
                        //gameObject.SetActive(false);
                        break;
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