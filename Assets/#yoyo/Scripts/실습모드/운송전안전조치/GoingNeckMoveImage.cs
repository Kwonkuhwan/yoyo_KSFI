using UnityEngine;
using UnityEngine.EventSystems;

public class GoingNeckMoveImage : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private bool isDragging = false;
    [SerializeField] private GameObject[] go_Points;


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

    public void OnPointerClick(PointerEventData eventData)
    {
        // 다른 이미지와의 충돌 처리
        Collider2D[] colliders = Physics2D.OverlapBoxAll(GetComponentInChildren<Collider2D>().bounds.center, GetComponentInChildren<Collider2D>().bounds.size, 0);
        foreach (var collider in colliders)
        {
            foreach (var obj in go_Points)
            {
                if (collider.gameObject == obj)
                {
                    gameObject.transform.position = obj.transform.position;
                    break;
                }
            }
        }
    }
}
