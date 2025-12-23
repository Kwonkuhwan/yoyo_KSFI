using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

public class UIDragAndCollision : MonoBehaviour
{
    // UI 충돌 감지를 위한 GraphicRaycaster 및 EventSystem
      // UI 충돌 감지를 위한 GraphicRaycaster 및 EventSystem
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    public Camera uiCamera;

    // 드래그할 UI 오브젝트 (할로겐열시험기)
    public GameObject halogenHeatTester;
    private RectTransform halogenRectTransform;

    // 충돌 대상 UI 오브젝트 (열감지기)
    public GameObject heatDetector;

    private bool isDragging = false;

    void Start()
    {
        // 할로겐열시험기의 RectTransform 가져오기
        halogenRectTransform = halogenHeatTester.GetComponent<RectTransform>();

        // 마우스 클릭 이벤트를 옵저버블로 변환하고 처리
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0)) // 마우스 버튼이 눌릴 때
            .Subscribe(_ => TryPickUIObject(Input.mousePosition));

        // 드래그 중 마우스 위치에 따라 오브젝트 이동
        this.UpdateAsObservable()
            .Where(_ => isDragging) // 드래그 중일 때
            .Subscribe(_ =>
            {
                DragUIObject(Input.mousePosition);

                if (IsOverlappingWithHeatDetector())
                {
                    HandleCollisionWithHeatDetector();
                }
            });

        // 마우스 버튼을 놓으면 드래그 중지
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonUp(0) && isDragging)
            .Subscribe(_ => isDragging = false);
    }

    // UI 오브젝트 선택 (피킹)
    private void TryPickUIObject(Vector2 screenPosition)
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        if (results.Count > 0)
        {
            foreach (RaycastResult result in results)
            {
                // 클릭된 오브젝트가 할로겐열시험기일 때만 드래그 시작
                if (result.gameObject == halogenHeatTester)
                {
                    isDragging = true;
                    break;
                }
            }
        }
    }

    // UI 오브젝트 이동
    private void DragUIObject(Vector2 screenPosition)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            halogenRectTransform.parent as RectTransform,
            screenPosition,
            uiCamera,
            out localPoint
        );

        halogenRectTransform.localPosition = localPoint;
    }

    // 충돌 감지: 할로겐열시험기와 열감지기의 충돌 여부 확인
    private bool IsOverlappingWithHeatDetector()
    {
        Rect halogenRect = halogenRectTransform.rect;
        halogenRect.position = halogenRectTransform.localPosition;

        Rect heatDetectorRect = heatDetector.GetComponent<RectTransform>().rect;
        heatDetectorRect.position = heatDetector.GetComponent<RectTransform>().localPosition;

        return halogenRect.Overlaps(heatDetectorRect);
    }

    // 충돌 이벤트 처리: 열감지기와 충돌할 때 발생하는 이벤트
    private void HandleCollisionWithHeatDetector()
    {
        Debug.Log("Collision Detected: Halogen Heat Tester with Heat Detector");
        // 이벤트 처리 로직 추가 (예: 애니메이션, 소리, 상태 변화 등)
    }
}
