using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

public class UIDragAndCollisionHandler : MonoBehaviour
{
    // GraphicRaycaster 및 EventSystem 참조
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    public Camera uiCamera;

    [System.Serializable]
    public class DraggablePair
    {
        public GameObject draggableObject; // 드래그할 오브젝트
        public GameObject collisionTarget; // 해당하는 충돌 대상
        public List<GameObject> collisionTargets = new List<GameObject>();
    }
    public List<DraggablePair> draggablePairs = new List<DraggablePair>();
    // 드래그 가능한 UI 오브젝트 리스트
    public List<GameObject> draggableObjects = new List<GameObject>();

    // 충돌 감지 대상 UI 오브젝트 리스트
    public List<GameObject> collisionTargets = new List<GameObject>();

    private RectTransform selectedRectTransform;
    private GameObject selectedObject = null;
    private GameObject targetObject = null; // 선택된 오브젝트의 대응 충돌 대상
    private List<GameObject> targetObjects = new List<GameObject>(); 
    private bool isDragging = false;

    private bool _isObjectSelected = true;
    // 이벤트 처리용 델리게이트
    public delegate void PickEventHandler(GameObject pickObject);
    public event PickEventHandler OnPicked;
    public delegate void CollisionEventHandler(GameObject draggedObject, GameObject targetObject);
    public event CollisionEventHandler OnCollisionDetected;

    private void Start()
    {
        //GasSysSoundManager.Init();
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

                // 드래그 중 충돌 감지
                //if (CheckCollision() || CheckCollision2())
                if (CheckCollision())
                {
                    OnCollisionDetected?.Invoke(selectedObject, targetObject);
                }
            });

        // 마우스 버튼을 놓으면 드래그 중지
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonUp(0) && isDragging)
            .Subscribe(_ =>
            {
                EnableMaskSprite(selectedObject);
                isDragging = false;
                selectedObject = null;
                targetObject = null;
                targetObjects.Clear();
            });
    }

    public void StopDragging()
    {
        _isObjectSelected = false;
    }

    public void StartDragging()
    {
        _isObjectSelected = true;
    }

    public void ResetEvent()
    {
        OnPicked = null;
        OnCollisionDetected = null;
    }

    public bool IsDragging()
    {
        return isDragging;
    }

    // UI 오브젝트 선택 (피킹)
    private void TryPickUIObject(Vector2 screenPosition)
    {
        if (!_isObjectSelected)
            return;
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        if (results.Count > 0)
        {
            foreach (RaycastResult result in results)
            {
                // 드래그 가능한 오브젝트 리스트에 있는지 확인
                if (draggableObjects.Contains(result.gameObject))
                {
                    selectedObject = result.gameObject;
                    selectedRectTransform = selectedObject.GetComponent<RectTransform>();
                    isDragging = true;
                    break;
                }

                foreach (var pair in draggablePairs)
                {
                    if (pair.draggableObject == result.gameObject)
                    {
                        selectedObject = pair.draggableObject;
                        targetObject = pair.collisionTarget;
                        targetObjects.AddRange(pair.collisionTargets);
                        OnPicked?.Invoke(selectedObject);
                        DisableMaskSprite(selectedObject);
                        selectedRectTransform = selectedObject.GetComponent<RectTransform>();
                        isDragging = true;
                        return;
                    }
                }
            }
        }
    }

    private Vector2 draggableObjectVector2;
    private void DisableMaskSprite(GameObject draggableObject)
    {
        draggableObjectVector2 = draggableObject.GetComponent<RectTransform>().anchoredPosition;
        if (null == draggableObject.GetComponent<InventoryItem>())
            return;
        draggableObject.GetComponent<Image>().enabled = false;
        var image = draggableObject.transform.GetChild(0).GetComponent<Image>();
        image.enabled = false;

    }

    private void EnableMaskSprite(GameObject draggableObject)
    {
        draggableObject.GetComponent<RectTransform>().anchoredPosition = draggableObjectVector2;
        if (null == draggableObject.GetComponent<InventoryItem>())
            return;
        draggableObject.GetComponent<Image>().enabled = true;
        var image = draggableObject.transform.GetChild(0).GetComponent<Image>();
        image.enabled = true;
    }

    // UI 오브젝트 이동
    private void DragUIObject(Vector2 screenPosition)
    {
        if (selectedRectTransform != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                selectedRectTransform.parent as RectTransform,
                screenPosition,
                uiCamera,
                out localPoint
            );

            selectedRectTransform.localPosition = localPoint;
        }
    }

    // 충돌 감지: 드래그 중인 오브젝트가 충돌 대상과 겹치는지 확인
    private bool CheckCollision()
    {
        if (selectedObject == null)
            return false;

        if (null == targetObjects && null == targetObject)
            return false;
        
        // if (selectedObject == null ||  targetObject == null)
        //     return false;

        var draggedRect = selectedRectTransform.rect;
        draggedRect.position = selectedRectTransform.position;



        foreach (var target in collisionTargets)
        {
            var targetRect = target.GetComponent<RectTransform>().rect;
            targetRect.position = target.GetComponent<RectTransform>().position;

            if (!draggedRect.Overlaps(targetRect))
                continue;
            // 충돌이 감지되면 등록된 이벤트 처리
            OnCollisionDetected?.Invoke(selectedObject, target);
            Debug.Log($"충돌0, 타겟 :{targetObject.name}");
            break;
        }
        
        foreach (var target in targetObjects)
        {
            var targetRect = target.GetComponent<RectTransform>().rect;
            targetRect.position = target.GetComponent<RectTransform>().position;
        
            if (!draggedRect.Overlaps(targetRect))
                continue;
            // 충돌이 감지되면 등록된 이벤트 처리
            if(null == target)
                Debug.Log("Target is null");
            OnCollisionDetected?.Invoke(selectedObject, target);
            Debug.Log($"충돌1, 타겟 :{targetObject.name}");
            break;
        }

        if (null == targetObject)
            return false;
        var targetRect1 = targetObject.GetComponent<RectTransform>().rect;
        targetRect1.position = targetObject.GetComponent<RectTransform>().position;
        if (!draggedRect.Overlaps(targetRect1))
            return false;
        // 충돌이 감지되면 등록된 이벤트 처리
        //OnCollisionDetected?.Invoke(selectedObject, targetObject);
        Debug.Log($"충돌2, 드래그 : {selectedRectTransform.name} 타겟 :{targetObject.name}");
        return true;
    }
/*
    private bool CheckCollision2()
    {
        if (selectedObject == null || targetObject == null)
            return false;

        // 드래그된 오브젝트의 월드 좌표에서의 바운드 가져오기
        Bounds draggedBounds = GetWorldBounds(selectedRectTransform);

        // 충돌 대상 오브젝트의 월드 좌표에서의 바운드 가져오기
        RectTransform targetRectTransform = targetObject.GetComponent<RectTransform>();
        Bounds targetBounds = GetWorldBounds(targetRectTransform);

        // 2D 충돌 감지 (X, Y 축에서 겹침 확인)
        bool is2DCollision = draggedBounds.Intersects(targetBounds);

        // 2D에서 겹침이 있을 때만 충돌로 처리
        if (!is2DCollision)
            return false;
        // 충돌이 감지되면 등록된 이벤트 처리
        //OnCollisionDetected?.Invoke(selectedObject, targetObject);
        Debug.Log("충돌2");
        return true;
    }

    private bool CheckCollision3()
    {
        if (selectedObject == null || targetObject == null)
            return false;

        // 드래그된 오브젝트의 월드 좌표에서의 바운드 가져오기
        //var draggedBounds = GetScreenRect(selectedRectTransform);
        var dragCanvas = GetParentCanvas(selectedRectTransform);

        // 충돌 대상 오브젝트의 월드 좌표에서의 바운드 가져오기
        RectTransform targetRectTransform = targetObject.GetComponent<RectTransform>();
        //var targetBounds = GetScreenRect(targetRectTransform);
        Canvas targetCanvas = GetParentCanvas(targetRectTransform);

        // if (!draggedBounds.Overlaps(targetBounds))
        //     return false;

        // 충돌이 감지되면 등록된 이벤트 처리
        //OnCollisionDetected?.Invoke(selectedObject, targetObject);
        if (!IsUICollision(dragCanvas, targetCanvas, selectedRectTransform, targetRectTransform))
            return false;
        Debug.Log("충돌3");
        return true;
    }
    
    private bool IsUICollision(Canvas dragCanvas, Canvas targetCanvas, RectTransform dragRectTransform, RectTransform targetRectTransform)
    {
        if (dragCanvas == null || targetCanvas == null) return false;

        Rect rectA = GetScreenRect(dragRectTransform, dragCanvas);
        Rect rectB = GetScreenRect(targetRectTransform, targetCanvas);

        return rectA.Overlaps(rectB);
    }

// RectTransform의 월드 좌표에서 바운드를 가져오는 함수
    private Bounds GetWorldBounds(RectTransform rectTransform)
    {
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        // 좌하단과 우상단 코너를 통해 Bounds 계산
        Vector3 min = worldCorners[0]; // 좌하단
        Vector3 max = worldCorners[2]; // 우상단

        return new Bounds((min + max) * 0.5f, max - min); // 바운드 중앙과 크기 계산
    }

    // private Rect GetScreenRect(RectTransform rectTransform)
    // {
    //     Vector3[] worldCorners = new Vector3[4];
    //     rectTransform.GetWorldCorners(worldCorners);
    //
    //     float xMin = worldCorners[0].x;
    //     float xMax = worldCorners[2].x;
    //     float yMin = worldCorners[0].y;
    //     float yMax = worldCorners[2].y;
    //
    //     return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    // }

    private Rect GetScreenRect(RectTransform rectTransform, Canvas canvas)
    {
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        Vector2 screenPointMin = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldCorners[0]);
        Vector2 screenPointMax = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldCorners[2]);

        return new Rect(screenPointMin.x, screenPointMin.y, screenPointMax.x - screenPointMin.x, screenPointMax.y - screenPointMin.y);
    }
    
    private Canvas GetParentCanvas(RectTransform uiObject)
    {
        var parentTransform = uiObject.transform;
        while (parentTransform != null)
        {
            Canvas canvas = parentTransform.GetComponent<Canvas>();
            if (canvas != null)
            {
                return canvas;
            }
            parentTransform = parentTransform.parent;
        }
        return null;
    }

// 특정 UI 오브젝트의 ScreenPoint를 월드 좌표로 변환
    private Vector3 GetWorldPositionFromScreenPoint(RectTransform rectTransform, Vector2 screenPoint)
    {
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, uiCamera, out worldPoint);
        return worldPoint;
    }
    */
}
