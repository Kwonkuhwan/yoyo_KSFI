using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KKH
{
    public enum AccidentReport
    {
        사고종류 = 0,
        흡착포,
        발생장소,
        자격증,
        전화,
        유의사항,
        None,
    }

    public class AccidentReportMoveImage : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private bool isDragging = false;

        [SerializeField] private GameObject[] go_Points;

        public AccidentReport ar;

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
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true; // 마우스 클릭 시작
            transform.SetParent(tr_DragParent);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false; // 마우스 클릭 종료
            transform.SetParent(tr_Panel);
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
                    AccidentReportInven inven = collider.GetComponent<AccidentReportInven>();
                    if (inven != null)
                    {
                        if (inven.ShowImage(ar))
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