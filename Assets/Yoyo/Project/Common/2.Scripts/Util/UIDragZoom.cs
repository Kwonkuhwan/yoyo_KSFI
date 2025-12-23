
#region Ver5
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragZoom : MonoBehaviour
{
    public RectTransform[] targetUIs; // 여러 개의 UI 요소
    public Canvas canvas;  // UI가 속한 Canvas
    public float zoomSpeed = 0.1f; // 확대/축소 속도
    public float minScale = 0.5f;  // 최소 스케일
    public float maxScale = 2.0f;  // 최대 스케일
    private Vector3[] originalScales; // 원래 스케일 저장
    private Vector3[] originalPositions; // 원래 위치 저장

    private bool isDragging = false;
    private Vector2 lastInputPosition;

    void Start()
    {
        // UI의 원래 위치와 스케일을 저장
        originalScales = new Vector3[targetUIs.Length];
        originalPositions = new Vector3[targetUIs.Length];

        for (int i = 0; i < targetUIs.Length; i++)
        {
            originalScales[i] = targetUIs[i].localScale;
            originalPositions[i] = targetUIs[i].localPosition;
        }
    }

    void Update()
    {
        ZoomWithScrollWheel();
        DragWithMouseOrTouch();
    }

    // 휠로 확대/축소
    void ZoomWithScrollWheel()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            foreach (RectTransform targetUI in targetUIs)
            {
                Vector3 currentScale = targetUI.localScale;
                float newScale = currentScale.x + scroll * zoomSpeed;
                newScale = Mathf.Clamp(newScale, minScale, maxScale);
                targetUI.localScale = new Vector3(newScale, newScale, 1);
            }
        }
    }

    // 마우스 또는 터치로 드래그하여 이동 (UI 상호작용 요소를 제외)
    void DragWithMouseOrTouch()
    {
        // 터치가 있으면 터치로 드래그 처리
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (IsPointerOverUIElement(touch.fingerId)) return;

            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                lastInputPosition = GetTouchCanvasPosition(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 currentTouchPosition = GetTouchCanvasPosition(touch.position);
                Vector2 delta = currentTouchPosition - lastInputPosition;

                foreach (RectTransform targetUI in targetUIs)
                {
                    Vector3 newPos = targetUI.localPosition + new Vector3(delta.x, delta.y, 0);
                    targetUI.localPosition = ClampPositionToCanvas(targetUI, newPos);
                }

                lastInputPosition = currentTouchPosition;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
        // 마우스로 드래그 처리
        else if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUIElement()) return; // 마우스가 UI 요소 위에 있으면 드래그 하지 않음

            isDragging = true;
            lastInputPosition = GetMouseCanvasPosition();
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 currentMousePosition = GetMouseCanvasPosition();
            Vector2 delta = currentMousePosition - lastInputPosition;

            foreach (RectTransform targetUI in targetUIs)
            {
                Vector3 newPos = targetUI.localPosition + new Vector3(delta.x, delta.y, 0);
                targetUI.localPosition = ClampPositionToCanvas(targetUI, newPos);
            }

            lastInputPosition = currentMousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    // UI 상호작용 요소 위에서 입력이 발생했는지 확인 (터치)
    bool IsPointerOverUIElement(int fingerId)
    {
        return EventSystem.current.IsPointerOverGameObject(fingerId);
    }

    // UI 상호작용 요소 위에서 입력이 발생했는지 확인 (마우스)
    bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    // 터치 좌표를 캔버스 좌표로 변환
    Vector2 GetTouchCanvasPosition(Vector2 touchPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, touchPosition, canvas.worldCamera, out Vector2 localPoint);
        return localPoint;
    }

    // 마우스 좌표를 캔버스 좌표로 변환
    Vector2 GetMouseCanvasPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out Vector2 localPoint);
        return localPoint;
    }

    // UI 요소를 캔버스 안에 제한
    Vector3 ClampPositionToCanvas(RectTransform targetUI, Vector3 newPosition)
    {
        Vector3 minPosition = canvas.transform.TransformPoint(canvas.pixelRect.min);
        Vector3 maxPosition = canvas.transform.TransformPoint(canvas.pixelRect.max);

        Vector3 clampedPosition = newPosition;

        // UI의 현재 크기에 따른 경계를 계산하여 제한
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Canvas 크기 안에서 UI 요소가 벗어나지 않도록 제한
        Vector3 uiMin = targetUI.TransformPoint(targetUI.rect.min);
        Vector3 uiMax = targetUI.TransformPoint(targetUI.rect.max);

        // 좌측 경계 제한
        if (uiMin.x < minPosition.x)
            clampedPosition.x += minPosition.x - uiMin.x;
        // 우측 경계 제한
        if (uiMax.x > maxPosition.x)
            clampedPosition.x -= uiMax.x - maxPosition.x;
        // 아래 경계 제한
        if (uiMin.y < minPosition.y)
            clampedPosition.y += minPosition.y - uiMin.y;
        // 위 경계 제한
        if (uiMax.y > maxPosition.y)
            clampedPosition.y -= uiMax.y - maxPosition.y;

        return clampedPosition;
    }

    // 리셋 기능 (원래 위치와 크기로)
    public void ResetUI()
    {
        for (int i = 0; i < targetUIs.Length; i++)
        {
            targetUIs[i].localScale = originalScales[i];
            targetUIs[i].localPosition = originalPositions[i];
        }
    }
}

#endregion //Ver5
#region Ver4
/*
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragZoom : MonoBehaviour
{
    public RectTransform[] targetUIs; // 여러 개의 UI 요소
    public Canvas canvas;  // UI가 속한 Canvas
    public float zoomSpeed = 0.1f; // 확대/축소 속도
    public float minScale = 0.5f;  // 최소 스케일
    public float maxScale = 2.0f;  // 최대 스케일
    private Vector3[] originalScales; // 원래 스케일 저장
    private Vector3[] originalPositions; // 원래 위치 저장

    private bool isDragging = false;
    private Vector2 lastInputPosition;

    void Start()
    {
        // UI의 원래 위치와 스케일을 저장
        originalScales = new Vector3[targetUIs.Length];
        originalPositions = new Vector3[targetUIs.Length];

        for (int i = 0; i < targetUIs.Length; i++)
        {
            originalScales[i] = targetUIs[i].localScale;
            originalPositions[i] = targetUIs[i].localPosition;
        }
    }

    void Update()
    {
        ZoomWithScrollWheel();
        DragWithMouseOrTouch();
    }

    // 휠로 확대/축소
    void ZoomWithScrollWheel()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            foreach (RectTransform targetUI in targetUIs)
            {
                Vector3 currentScale = targetUI.localScale;
                float newScale = currentScale.x + scroll * zoomSpeed;
                newScale = Mathf.Clamp(newScale, minScale, maxScale);
                targetUI.localScale = new Vector3(newScale, newScale, 1);
            }
        }
    }

    // 마우스 또는 터치로 드래그하여 이동 (UI 상호작용 요소를 제외)
    void DragWithMouseOrTouch()
    {
        if (IsPointerOverUIElement())
        {
            return; // UI 상호작용 요소 위에서는 드래그 비활성화
        }

        if (Input.touchCount > 0)  // 터치 입력이 있을 경우
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) // 터치 시작
            {
                isDragging = true;
                lastInputPosition = GetTouchCanvasPosition(touch.position); // 터치 좌표
            }
            else if (touch.phase == TouchPhase.Moved) // 터치 이동 중
            {
                Vector2 currentTouchPosition = GetTouchCanvasPosition(touch.position);
                Vector2 delta = currentTouchPosition - lastInputPosition;

                foreach (RectTransform targetUI in targetUIs)
                {
                    Vector3 newPos = targetUI.localPosition + new Vector3(delta.x, delta.y, 0);
                    targetUI.localPosition = ClampPositionToCanvas(targetUI, newPos);  // 캔버스 내에서 위치 제한
                }

                lastInputPosition = currentTouchPosition;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) // 터치 끝
            {
                isDragging = false;
            }
        }
        else if (Input.GetMouseButtonDown(0)) // 마우스 클릭 시작
        {
            isDragging = true;
            lastInputPosition = GetMouseCanvasPosition(); // 마우스 좌표
        }
        else if (Input.GetMouseButton(0) && isDragging) // 마우스 드래그 중
        {
            Vector2 currentMousePosition = GetMouseCanvasPosition();
            Vector2 delta = currentMousePosition - lastInputPosition;

            foreach (RectTransform targetUI in targetUIs)
            {
                Vector3 newPos = targetUI.localPosition + new Vector3(delta.x, delta.y, 0);
                targetUI.localPosition = ClampPositionToCanvas(targetUI, newPos);  // 캔버스 내에서 위치 제한
            }

            lastInputPosition = currentMousePosition;
        }
        else if (Input.GetMouseButtonUp(0)) // 마우스 클릭 끝
        {
            isDragging = false;
        }
    }

    // UI 상호작용 요소 위에서 입력이 발생했는지 확인
    bool IsPointerOverUIElement()
    {
        // 마우스 입력 확인
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true; // UI 상호작용 요소 위에서 클릭 발생
        }

        // 터치 입력 확인
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return true; // 터치 상호작용 요소 위에서 발생
            }
        }

        return false;
    }

    // 터치 좌표를 캔버스 좌표로 변환
    Vector2 GetTouchCanvasPosition(Vector2 touchPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, touchPosition, canvas.worldCamera, out Vector2 localPoint);
        return localPoint;
    }

    // 마우스 좌표를 캔버스 좌표로 변환
    Vector2 GetMouseCanvasPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out Vector2 localPoint);
        return localPoint;
    }

    // UI 요소를 캔버스 안에 제한
    Vector3 ClampPositionToCanvas(RectTransform targetUI, Vector3 newPosition)
    {
        Vector3 minPosition = canvas.transform.TransformPoint(canvas.pixelRect.min);
        Vector3 maxPosition = canvas.transform.TransformPoint(canvas.pixelRect.max);

        Vector3 clampedPosition = newPosition;

        // UI의 현재 크기에 따른 경계를 계산하여 제한
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Canvas 크기 안에서 UI 요소가 벗어나지 않도록 제한
        Vector3 uiMin = targetUI.TransformPoint(targetUI.rect.min);
        Vector3 uiMax = targetUI.TransformPoint(targetUI.rect.max);

        // 좌측 경계 제한
        if (uiMin.x < minPosition.x)
            clampedPosition.x += minPosition.x - uiMin.x;
        // 우측 경계 제한
        if (uiMax.x > maxPosition.x)
            clampedPosition.x -= uiMax.x - maxPosition.x;
        // 아래 경계 제한
        if (uiMin.y < minPosition.y)
            clampedPosition.y += minPosition.y - uiMin.y;
        // 위 경계 제한
        if (uiMax.y > maxPosition.y)
            clampedPosition.y -= uiMax.y - maxPosition.y;

        return clampedPosition;
    }

    // 리셋 기능 (원래 위치와 크기로)
    public void ResetUI()
    {
        for (int i = 0; i < targetUIs.Length; i++)
        {
            targetUIs[i].localScale = originalScales[i];
            targetUIs[i].localPosition = originalPositions[i];
        }
    }
}
*/
#endregion //Ver4

#region Ver3
/*
using UnityEngine;
using UnityEngine.UI;

public class UIDragZoom : MonoBehaviour
{
    public RectTransform[] targetUIs; // 여러 개의 UI 요소
    public Canvas canvas;  // UI가 속한 Canvas
    public float zoomSpeed = 0.1f; // 확대/축소 속도
    public float minScale = 0.5f;  // 최소 스케일
    public float maxScale = 2.0f;  // 최대 스케일
    private Vector3[] originalScales; // 원래 스케일 저장
    private Vector3[] originalPositions; // 원래 위치 저장

    private bool isDragging = false;
    private Vector2 lastInputPosition;

    void Start()
    {
        // UI의 원래 위치와 스케일을 저장
        originalScales = new Vector3[targetUIs.Length];
        originalPositions = new Vector3[targetUIs.Length];

        for (int i = 0; i < targetUIs.Length; i++)
        {
            originalScales[i] = targetUIs[i].localScale;
            originalPositions[i] = targetUIs[i].localPosition;
        }
    }

    void Update()
    {
        ZoomWithScrollWheel();
        DragWithMouseOrTouch();
    }

    // 휠로 확대/축소
    void ZoomWithScrollWheel()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            foreach (RectTransform targetUI in targetUIs)
            {
                Vector3 currentScale = targetUI.localScale;
                float newScale = currentScale.x + scroll * zoomSpeed;
                newScale = Mathf.Clamp(newScale, minScale, maxScale);
                targetUI.localScale = new Vector3(newScale, newScale, 1);
            }
        }
    }

    // 마우스 또는 터치로 드래그하여 이동
    void DragWithMouseOrTouch()
    {
        if (Input.touchCount > 0)  // 터치 입력이 있을 경우
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) // 터치 시작
            {
                isDragging = true;
                lastInputPosition = GetTouchCanvasPosition(touch.position); // 터치 좌표
            }
            else if (touch.phase == TouchPhase.Moved) // 터치 이동 중
            {
                Vector2 currentTouchPosition = GetTouchCanvasPosition(touch.position);
                Vector2 delta = currentTouchPosition - lastInputPosition;

                foreach (RectTransform targetUI in targetUIs)
                {
                    Vector3 newPos = targetUI.localPosition + new Vector3(delta.x, delta.y, 0);
                    targetUI.localPosition = ClampPositionToCanvas(targetUI, newPos);  // 캔버스 내에서 위치 제한
                }

                lastInputPosition = currentTouchPosition;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) // 터치 끝
            {
                isDragging = false;
            }
        }
        else if (Input.GetMouseButtonDown(0)) // 마우스 클릭 시작
        {
            isDragging = true;
            lastInputPosition = GetMouseCanvasPosition(); // 마우스 좌표
        }
        else if (Input.GetMouseButton(0) && isDragging) // 마우스 드래그 중
        {
            Vector2 currentMousePosition = GetMouseCanvasPosition();
            Vector2 delta = currentMousePosition - lastInputPosition;

            foreach (RectTransform targetUI in targetUIs)
            {
                Vector3 newPos = targetUI.localPosition + new Vector3(delta.x, delta.y, 0);
                targetUI.localPosition = ClampPositionToCanvas(targetUI, newPos);  // 캔버스 내에서 위치 제한
            }

            lastInputPosition = currentMousePosition;
        }
        else if (Input.GetMouseButtonUp(0)) // 마우스 클릭 끝
        {
            isDragging = false;
        }
    }

    // 터치 좌표를 캔버스 좌표로 변환
    Vector2 GetTouchCanvasPosition(Vector2 touchPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, touchPosition, canvas.worldCamera, out Vector2 localPoint);
        return localPoint;
    }

    // 마우스 좌표를 캔버스 좌표로 변환
    Vector2 GetMouseCanvasPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out Vector2 localPoint);
        return localPoint;
    }

    // UI 요소를 캔버스 안에 제한
    Vector3 ClampPositionToCanvas(RectTransform targetUI, Vector3 newPosition)
    {
        Vector3 minPosition = canvas.transform.TransformPoint(canvas.pixelRect.min);
        Vector3 maxPosition = canvas.transform.TransformPoint(canvas.pixelRect.max);

        Vector3 clampedPosition = newPosition;

        // UI의 현재 크기에 따른 경계를 계산하여 제한
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Canvas 크기 안에서 UI 요소가 벗어나지 않도록 제한
        Vector3 uiMin = targetUI.TransformPoint(targetUI.rect.min);
        Vector3 uiMax = targetUI.TransformPoint(targetUI.rect.max);

        // 좌측 경계 제한
        if (uiMin.x < minPosition.x)
            clampedPosition.x += minPosition.x - uiMin.x;
        // 우측 경계 제한
        if (uiMax.x > maxPosition.x)
            clampedPosition.x -= uiMax.x - maxPosition.x;
        // 아래 경계 제한
        if (uiMin.y < minPosition.y)
            clampedPosition.y += minPosition.y - uiMin.y;
        // 위 경계 제한
        if (uiMax.y > maxPosition.y)
            clampedPosition.y -= uiMax.y - maxPosition.y;

        return clampedPosition;
    }

    // 리셋 기능 (원래 위치와 크기로)
    public void ResetUI()
    {
        for (int i = 0; i < targetUIs.Length; i++)
        {
            targetUIs[i].localScale = originalScales[i];
            targetUIs[i].localPosition = originalPositions[i];
        }
    }
}
*/

#endregion 


#region Ver2
/*
using UnityEngine;
using UnityEngine.UI;

public class UIDragZoom : MonoBehaviour
{
    public RectTransform[] targetUIs; // 여러 개의 UI 요소
    public Canvas canvas;  // UI가 속한 Canvas
    public float zoomSpeed = 0.1f; // 확대/축소 속도
    public float minScale = 0.5f;  // 최소 스케일
    public float maxScale = 2.0f;  // 최대 스케일
    private Vector3[] originalScales; // 원래 스케일 저장
    private Vector3[] originalPositions; // 원래 위치 저장

    private bool isDragging = false;
    private Vector2 lastMousePosition;

    void Start()
    {
        // UI의 원래 위치와 스케일을 저장
        originalScales = new Vector3[targetUIs.Length];
        originalPositions = new Vector3[targetUIs.Length];

        for (int i = 0; i < targetUIs.Length; i++)
        {
            originalScales[i] = targetUIs[i].localScale;
            originalPositions[i] = targetUIs[i].localPosition;
        }
    }

    void Update()
    {
        ZoomWithScrollWheel();
        DragWithMouseOrTouch();
    }

    // 휠로 확대/축소
    void ZoomWithScrollWheel()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            foreach (RectTransform targetUI in targetUIs)
            {
                Vector3 currentScale = targetUI.localScale;
                float newScale = currentScale.x + scroll * zoomSpeed;
                newScale = Mathf.Clamp(newScale, minScale, maxScale);
                targetUI.localScale = new Vector3(newScale, newScale, 1);
            }
        }
    }

    // 마우스 또는 터치로 드래그하여 이동
    void DragWithMouseOrTouch()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 클릭 또는 터치 시작
        {
            isDragging = true;
            lastMousePosition = GetMouseCanvasPosition(); // 캔버스 기준으로 변환된 마우스 좌표
        }

        if (Input.GetMouseButtonUp(0)) // 마우스 클릭 해제 또는 터치 끝
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 currentMousePosition = GetMouseCanvasPosition(); // 캔버스 기준으로 변환된 마우스 좌표
            Vector2 delta = currentMousePosition - lastMousePosition;

            foreach (RectTransform targetUI in targetUIs)
            {
                Vector3 newPos = targetUI.localPosition + new Vector3(delta.x, delta.y, 0);
                targetUI.localPosition = ClampPositionToCanvas(targetUI, newPos);  // 캔버스 내에서 위치 제한
            }

            lastMousePosition = currentMousePosition;
        }
    }

    // 마우스 좌표를 캔버스 좌표로 변환
    Vector2 GetMouseCanvasPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out Vector2 localPoint);
        return localPoint;
    }

    // UI 요소를 캔버스 안에 제한
    Vector3 ClampPositionToCanvas(RectTransform targetUI, Vector3 newPosition)
    {
        Vector3 minPosition = canvas.transform.TransformPoint(canvas.pixelRect.min);
        Vector3 maxPosition = canvas.transform.TransformPoint(canvas.pixelRect.max);

        Vector3 clampedPosition = newPosition;

        // UI의 현재 크기에 따른 경계를 계산하여 제한
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Canvas 크기 안에서 UI 요소가 벗어나지 않도록 제한
        Vector3 uiMin = targetUI.TransformPoint(targetUI.rect.min);
        Vector3 uiMax = targetUI.TransformPoint(targetUI.rect.max);

        // 좌측 경계 제한
        if (uiMin.x < minPosition.x)
            clampedPosition.x += minPosition.x - uiMin.x;
        // 우측 경계 제한
        if (uiMax.x > maxPosition.x)
            clampedPosition.x -= uiMax.x - maxPosition.x;
        // 아래 경계 제한
        if (uiMin.y < minPosition.y)
            clampedPosition.y += minPosition.y - uiMin.y;
        // 위 경계 제한
        if (uiMax.y > maxPosition.y)
            clampedPosition.y -= uiMax.y - maxPosition.y;

        return clampedPosition;
    }

    // 리셋 기능 (원래 위치와 크기로)
    public void ResetUI()
    {
        for (int i = 0; i < targetUIs.Length; i++)
        {
            targetUIs[i].localScale = originalScales[i];
            targetUIs[i].localPosition = originalPositions[i];
        }
    }
}
*/
#endregion //Ver2


#region Ver1

// using UnityEngine;
// using UnityEngine.UI;
//
// public class UIDragZoom : MonoBehaviour
// {
//     public RectTransform[] targetUIs; // 여러 개의 UI 요소
//     public Canvas canvas;  // UI가 속한 Canvas
//     public float zoomSpeed = 0.1f; // 확대/축소 속도
//     public float minScale = 0.5f;  // 최소 스케일
//     public float maxScale = 2.0f;  // 최대 스케일
//     private Vector3[] originalScales; // 원래 스케일 저장
//     private Vector3[] originalPositions; // 원래 위치 저장
//
//     private bool isDragging = false;
//     private Vector2 lastMousePosition;
//
//     void Start()
//     {
//         // UI의 원래 위치와 스케일을 저장
//         originalScales = new Vector3[targetUIs.Length];
//         originalPositions = new Vector3[targetUIs.Length];
//
//         for (int i = 0; i < targetUIs.Length; i++)
//         {
//             originalScales[i] = targetUIs[i].localScale;
//             originalPositions[i] = targetUIs[i].localPosition;
//         }
//     }
//
//     void Update()
//     {
//         ZoomWithScrollWheel();
//         DragWithMouseOrTouch();
//     }
//
//     // 휠로 확대/축소
//     void ZoomWithScrollWheel()
//     {
//         float scroll = Input.GetAxis("Mouse ScrollWheel");
//
//         if (scroll != 0)
//         {
//             foreach (RectTransform targetUI in targetUIs)
//             {
//                 Vector3 currentScale = targetUI.localScale;
//                 float newScale = currentScale.x + scroll * zoomSpeed;
//                 newScale = Mathf.Clamp(newScale, minScale, maxScale);
//                 targetUI.localScale = new Vector3(newScale, newScale, 1);
//             }
//         }
//     }
//
//     // 마우스 또는 터치로 드래그하여 이동
//     void DragWithMouseOrTouch()
//     {
//         if (Input.GetMouseButtonDown(0)) // 마우스 클릭 또는 터치 시작
//         {
//             isDragging = true;
//             lastMousePosition = Input.mousePosition;
//         }
//
//         if (Input.GetMouseButtonUp(0)) // 마우스 클릭 해제 또는 터치 끝
//         {
//             isDragging = false;
//         }
//
//         if (isDragging)
//         {
//             Vector2 currentMousePosition = Input.mousePosition;
//             Vector2 delta = currentMousePosition - lastMousePosition;
//
//             foreach (RectTransform targetUI in targetUIs)
//             {
//                 Vector3 newPos = targetUI.localPosition + new Vector3(delta.x, delta.y, 0);
//                 targetUI.localPosition = ClampPositionToCanvas(targetUI, newPos);  // 캔버스 내에서 위치 제한
//             }
//
//             lastMousePosition = currentMousePosition;
//         }
//     }
//
//     // UI 요소를 캔버스 안에 제한
//     Vector3 ClampPositionToCanvas(RectTransform targetUI, Vector3 newPosition)
//     {
//         Vector3 minPosition = canvas.transform.TransformPoint(canvas.pixelRect.min);
//         Vector3 maxPosition = canvas.transform.TransformPoint(canvas.pixelRect.max);
//
//         Vector3 clampedPosition = newPosition;
//
//         // UI의 현재 크기에 따른 경계를 계산하여 제한
//         RectTransform canvasRect = canvas.GetComponent<RectTransform>();
//
//         // Canvas 크기 안에서 UI 요소가 벗어나지 않도록 제한
//         Vector3 uiMin = targetUI.TransformPoint(targetUI.rect.min);
//         Vector3 uiMax = targetUI.TransformPoint(targetUI.rect.max);
//
//         // 좌측 경계 제한
//         if (uiMin.x < minPosition.x)
//             clampedPosition.x += minPosition.x - uiMin.x;
//         // 우측 경계 제한
//         if (uiMax.x > maxPosition.x)
//             clampedPosition.x -= uiMax.x - maxPosition.x;
//         // 아래 경계 제한
//         if (uiMin.y < minPosition.y)
//             clampedPosition.y += minPosition.y - uiMin.y;
//         // 위 경계 제한
//         if (uiMax.y > maxPosition.y)
//             clampedPosition.y -= uiMax.y - maxPosition.y;
//
//         return clampedPosition;
//     }
//
//     // 리셋 기능 (원래 위치와 크기로)
//     public void ResetUI()
//     {
//         for (int i = 0; i < targetUIs.Length; i++)
//         {
//             targetUIs[i].localScale = originalScales[i];
//             targetUIs[i].localPosition = originalPositions[i];
//         }
//     }
// }


#endregion //Ver1