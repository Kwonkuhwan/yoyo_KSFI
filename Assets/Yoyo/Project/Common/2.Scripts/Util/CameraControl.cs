using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Camera uiCamera;           // UI를 위한 카메라
    public RectTransform canvasRect;  // Canvas의 RectTransform
    public float zoomSpeed = 10f;     // 줌 속도
    public float moveSpeed = 10f;     // 이동 속도
    public float minZoom = 5f;        // 최소 줌 크기
    public float maxZoom = 20f;       // 최대 줌 크기

    private Vector2 canvasMin;        // Canvas 최소 좌표
    private Vector2 canvasMax;        // Canvas 최대 좌표

    void Start()
    {
        // 캔버스 크기 및 좌표 계산
        canvasMin = canvasRect.TransformPoint(canvasRect.rect.min);
        canvasMax = canvasRect.TransformPoint(canvasRect.rect.max);
    }

    void Update()
    {
        HandleZoom();
        HandleMovement();
        ClampCameraPosition();
    }

    // 확대/축소 처리
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            uiCamera.orthographicSize = Mathf.Clamp(uiCamera.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
        }
    }

    // 카메라 이동 처리 (WASD 또는 마우스 드래그)
    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // 카메라를 WASD 키로 이동
        uiCamera.transform.Translate(new Vector3(moveX, moveY, 0f), Space.World);

        // 마우스 드래그로 이동 (마우스 오른쪽 버튼 클릭 시)
        if (Input.GetMouseButton(1))
        {
            float mouseX = -Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
            float mouseY = -Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;
            uiCamera.transform.Translate(new Vector3(mouseX, mouseY, 0f), Space.World);
        }
    }

    // 카메라가 Canvas 영역을 벗어나지 않도록 제한
    void ClampCameraPosition()
    {
        // 카메라의 크기 및 화면 비율 계산
        float cameraHeight = uiCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * uiCamera.aspect;

        // 카메라의 위치 제한
        float camX = Mathf.Clamp(uiCamera.transform.position.x, canvasMin.x + cameraWidth / 2f, canvasMax.x - cameraWidth / 2f);
        float camY = Mathf.Clamp(uiCamera.transform.position.y, canvasMin.y + cameraHeight / 2f, canvasMax.y - cameraHeight / 2f);

        // 카메라 위치 업데이트
        uiCamera.transform.position = new Vector3(camX, camY, uiCamera.transform.position.z);
    }
}
