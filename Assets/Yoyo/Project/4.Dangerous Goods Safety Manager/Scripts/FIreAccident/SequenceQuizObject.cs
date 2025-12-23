using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

[Serializable]
public class SequenceQuizObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    [SerializeField] private Image image;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private RectTransform currentImageRectTransform;
    [SerializeField] private SequenceQuiz sequence;
    [SerializeField] private RectTransform boundaryRect;

    private Vector2 imaginationRectTransform;
    private Transform currentTrans;
    private void Awake()
    {
        transform.localPosition = startPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        imaginationRectTransform = currentImageRectTransform.anchoredPosition;
        //sequence.RemoveAnswer(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       // //Vector3 vector = sequence.CheckTransform(image.transform);
       // //if(vector == Vector3.zero)
       //// {
       //     image.transform.localPosition = startPosition;
       //     //sequence.RemoveAnswer(this);
       // }
       // else
       // {
       //     //image.transform.localPosition = vector;
       // }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localDelta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            currentImageRectTransform.parent as RectTransform, // 부모 RectTransform 기준
            eventData.position, // 현재 터치 위치
            eventData.pressEventCamera, // 이벤트 카메라
            out Vector2 currentPosition
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            currentImageRectTransform.parent as RectTransform,
            eventData.position - eventData.delta, // 이전 터치 위치
            eventData.pressEventCamera,
            out Vector2 previousPosition
        );

        // 이동량 = 현재 위치 - 이전 위치
        localDelta = currentPosition - previousPosition;
        // imaginationPosition을 이동
        imaginationRectTransform += localDelta;
        ClampToBoundary();

        // 실제 RectTransform에 imaginationPosition을 반영
        //currentImageRectTransform.anchoredPosition = imaginationRectTransform;
    }

    private void ClampToBoundary()
    {
        if (boundaryRect == null || currentImageRectTransform == null) return;

        // boundaryRect의 월드 좌표를 가져옵니다
        Vector3[] boundaries = new Vector3[4];
        boundaryRect.GetWorldCorners(boundaries);

        // 이미지 크기와 크기의 반영된 영역을 계산
        Vector2 imageSize = image.rectTransform.sizeDelta;

        // 현재 imaginationPosition의 월드 좌표를 계산합니다
        Vector3 position = currentImageRectTransform.parent.TransformPoint(imaginationRectTransform);

        position.x = Mathf.Clamp(position.x, boundaries[0].x+10, boundaries[2].x-10);
        position.y = Mathf.Clamp(position.y, boundaries[0].y+10, boundaries[1].y-10);


        // imaginationPosition에 반영
        currentImageRectTransform.anchoredPosition = currentImageRectTransform.parent.InverseTransformPoint(position);
        Debug.Log(position);
    }

    public void ReSetPosition()
    {
        transform.localPosition = startPosition;
    }
}
