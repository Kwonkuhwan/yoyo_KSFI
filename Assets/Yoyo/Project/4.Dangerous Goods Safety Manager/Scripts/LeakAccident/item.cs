using UnityEngine;
using UnityEngine.EventSystems;

public class item : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform currentTransform;

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localDelta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            currentTransform.parent as RectTransform, // 부모 RectTransform 기준
            eventData.position, // 현재 터치 위치
            eventData.pressEventCamera, // 이벤트 카메라
            out Vector2 currentPosition
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            currentTransform.parent as RectTransform,
            eventData.position - eventData.delta, // 이전 터치 위치
            eventData.pressEventCamera,
            out Vector2 previousPosition
        );

        currentTransform.anchoredPosition = currentPosition - previousPosition;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
