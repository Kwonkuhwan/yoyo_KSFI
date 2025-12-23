using UnityEngine;
using UnityEngine.EventSystems;

namespace KKH
{
    public class GroundWireMoveImage : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private Vector3 oldposition;
        private bool isDragging = false;
        [SerializeField] private GameObject go_GroundPoint;
        [SerializeField] private ButtonManager_KKH buttonManager_KKH;
        [SerializeField] private GameObject[] go_linePoints;

        private void Awake()
        {
            buttonManager_KKH = GetComponent<ButtonManager_KKH>();
            oldposition = transform.localPosition;
        }

        private void OnEnable()
        {
            go_GroundPoint.SetActive(true);
            foreach (var point in go_linePoints)
            {
                point.SetActive(true);
            }

            transform.localPosition = oldposition;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true; // 마우스 클릭 시작
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false; // 마우스 클릭 종료
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

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject == go_GroundPoint)
            {
                // 충돌 시 추가할 행동을 여기에 작성
                if (collision.gameObject.GetComponent<GroundPoint>() != null)
                {
                    collision.gameObject.GetComponent<GroundPoint>().isEnable = true;
                    collision.gameObject.SetActive(false);
                    if (buttonManager_KKH != null)
                    {
                        buttonManager_KKH.isCompelet = true;
                    }
                    foreach (var point in go_linePoints)
                    {
                        point.SetActive(false);
                    }
                    gameObject.SetActive(false);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // 다른 이미지와의 충돌 처리
            Collider2D[] colliders = Physics2D.OverlapBoxAll(GetComponentInChildren<Collider2D>().bounds.center, GetComponentInChildren<Collider2D>().bounds.size, 0);
            foreach (var collider in colliders)
            {

            }
        }
    }
}