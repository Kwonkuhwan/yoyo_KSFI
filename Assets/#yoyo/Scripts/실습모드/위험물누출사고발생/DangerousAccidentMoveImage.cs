using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KKH
{
    public enum DangerousAccident
    {
        빗자루 = 0,
        흡착포,
        소방대,
        소방번호,
        바람,
        차량뒷편,
        None,
    }

    public class DangerousAccidentMoveImage : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private bool isDragging = false;

        [SerializeField] private GameObject[] go_Points;

        public DangerousAccident da;

        private Vector3 oldPoint;

        [SerializeField] private Transform tr_Panel;
        [SerializeField] private Transform tr_DragParent;

        private void Awake()
        {
            oldPoint = transform.position;
        }

        private void OnEnable()
        {
            transform.position = oldPoint;
            transform.SetParent(tr_Panel);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true; // 마우스 클릭 시작
            transform.SetParent(tr_DragParent);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false; // 마우스 클릭 종료
            transform.position = oldPoint;
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

        public void OnPointerClick(PointerEventData eventData)
        {
            try
            {
                // 다른 이미지와의 충돌 처리
                Collider2D[] colliders = Physics2D.OverlapBoxAll(GetComponentInChildren<Collider2D>().bounds.center, GetComponentInChildren<Collider2D>().bounds.size, 0);
                foreach (var collider in colliders)
                {
                    DangerousAccidentInven inven = collider.GetComponent<DangerousAccidentInven>();
                    if (inven != null)
                    {
                        if (inven.ShowImage(da))
                        {
                            transform.SetParent(tr_Panel);
                            gameObject.SetActive(false);
                        }
                    }
                }
            }
            catch
            {
                transform.SetParent(tr_Panel);
                transform.position = oldPoint;
            }
        }
    }
}
