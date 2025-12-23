using UnityEngine;

public class WorldCanvasZoom : MonoBehaviour
{
    public float minZoom = 270f; // 최소 줌 크기
    public float maxZoom = 540f; // 최대 줌 크기
    public float zoomSpeed = 50f; // 줌 속도
    public float moveSpeed = 0.5f;

    private Camera cam;
    private Vector3 dragOrigin;
    private float cameraWidth, cameraHeight;
    private float initialTouchDistance;
    private float initialCameraSize;

    [SerializeField] private UIDragAndCollisionHandler dragAndCollisionHandler;

    public Vector2 canvasSize = new Vector2(1920f, 1080f);

    // SMW
    private bool isItemDrag = false;

    private void Start()
    {
#if UNITY_EDITOR 
        return;
#endif
        cam = Camera.main;
        if (cam != null)
        {
            cam.orthographicSize = maxZoom; // 카메라의 기본 줌 설정
            cam.aspect = 1.77777779f;
        }
        UpdateCameraBounds();

    }

    private void Update()
    {
#if UNITY_EDITOR
        return;
#endif
        if (dragAndCollisionHandler && dragAndCollisionHandler.IsDragging())
            return;
        if (isItemDrag) return;         // SMW
        // Zoom in/out with mouse scroll
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
            UpdateCameraBounds(); // Update camera bounds when zoom changes
        }

        // Pan with mouse
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPosition = cam.transform.position + difference;
            cam.transform.position = ClampCamera(newPosition);
        }

        // Touch controls for panning and zooming
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                dragOrigin = cam.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(touch.position);
                Vector3 newPosition = cam.transform.position + difference;
                cam.transform.position = ClampCamera(newPosition);
            }
        }
        else if (Input.touchCount == 2)
        {
            // Pinch to zoom
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // Calculate the distance between the two touches in the current and previous frame
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

            float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
            float touchDeltaMag = (touch1.position - touch2.position).magnitude;

            // Calculate the zoom factor
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + deltaMagnitudeDiff * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
            UpdateCameraBounds(); // Update camera bounds when zoom changes
        }
    }

    private void UpdateCameraBounds()
    {
        cameraHeight = cam.orthographicSize * 2;
        cameraWidth = cameraHeight * cam.aspect;
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float minX = -canvasSize.x / 2 + cameraWidth / 2;
        float maxX = canvasSize.x / 2 - cameraWidth / 2;
        float minY = -canvasSize.y / 2 + cameraHeight / 2;
        float maxY = canvasSize.y / 2 - cameraHeight / 2;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        return targetPosition;
    }

    // SMW
    public void SetItemDragBoolean(bool isDrag)
    {
        isItemDrag = isDrag;
    }
}
