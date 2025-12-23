using UnityEngine;
using UnityEngine.EventSystems;

namespace KKH
{
    public class UnitItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        protected bool isDragging = false;

        [SerializeField] protected GameObject[] go_Points;

        [SerializeField] protected Vector3 oldPoint;

        [SerializeField] protected Transform tr_Panel;
        [SerializeField] protected Transform tr_DragParent;

        [SerializeField] protected GameObject go_Error;

        protected virtual void Awake()
        {
            oldPoint = transform.position;
            tr_Panel = transform.parent;
        }

        protected virtual void OnEnable()
        {
            gameObject.SetActive(true);
            transform.position = oldPoint;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true; // 마우스 클릭 시작
            transform.SetParent(tr_DragParent);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false; // 마우스 클릭 종료
            transform.SetParent(tr_Panel);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging)
            {
                // 마우스의 위치를 RectTransform으로 변환
                RectTransform rectTransform = GetComponent<RectTransform>();
                Vector2 localPoint;
                // 현재 마우스 위치를 RectTransform으로 변환
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out localPoint);
                rectTransform.anchoredPosition = localPoint; // 위치 업데이트
            }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            
        }
    }
}