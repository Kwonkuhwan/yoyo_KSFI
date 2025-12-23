using TMPro;
using UnityEngine;
using UniRx;

public class ObjectControl : MonoBehaviour
{
#region singleton
    private static ObjectControl instance;
    public static ObjectControl Instance { get { return instance; } }
#endregion
    public string targetTag = "Rotatable"; // 회전 및 스케일 가능한 오브젝트의 태그

    public TextMeshProUGUI debugText;
    [SerializeField] private Camera mainCamera;
    private Transform selectedObject;

    private Vector3 offset;
    private bool isMoveMode = false; // 이동 모드 활성화 여부
    private bool isRotateMode = false; // 회전 모드 활성화 여부

    private void Awake()
    {
        instance = this;
        //EditorPrefs.SetInt("kAutoRefresh", 1);
    }
    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag(targetTag))
            {
                selectedObject = hit.transform;
            }
        }
        // 마우스 입력 처리
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0)|| Input.GetMouseButtonDown(1)|| Input.GetMouseButtonDown(2))
            .Subscribe(_ => OnMouseDown())
            .AddTo(this);

        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButton(0) && selectedObject != null)
            .Subscribe(_ => HandleMouseInput())
            .AddTo(this);

        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonUp(0))
            .Subscribe(_ => OnMouseUp())
            .AddTo(this);

        // 터치 입력 처리
        Observable.EveryUpdate()
            .Where(_ => Input.touchCount > 0)
            .Subscribe(_ => HandleTouch())
            .AddTo(this);
        GasSysIPartList.Instance.GetMoveBtn(EnableMoveMode);
        GasSysIPartList.Instance.GetRotateBtn(EnableRotateMode);
        EnableMoveMode();
    }

    private void OnMouseDown()
    {
        debugText.text += "마우스 다운 이벤트\n";
        if (null == mainCamera)
        {
            debugText.text += "카메라 Null\n";
        }
     
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        //Ray ray = mainCamera.ViewportPointToRay(Input.mousePosition);
        debugText.text += Input.mousePosition + "\n";
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            debugText.text += hit.collider.name + "\n";
            if (hit.collider.CompareTag(targetTag))
            {
                selectedObject = hit.transform;
                debugText.text += "오브젝트 선택됨\n";
                offset = selectedObject.position - hit.point;
            }
        }
    }

    private void OnMouseUp()
    {
        debugText.text += "마우스 UP\n";
        selectedObject = null;
    }

    private void HandleMouseInput()
    {
        if (isMoveMode)
        {
            MoveObjectWithMouse();
        }
        else if (isRotateMode)
        {
            RotateObjectWithMouse();
        }
        ScaleObjectWithMouse();
    }

    /// <summary>
    /// Moves the selected object based on mouse input.
    /// Calculates the new position using a raycast to determine the point in the scene
    /// where the mouse is pointing and applies an offset to maintain consistent movement.
    /// </summary>
    private void MoveObjectWithMouse2()
    {
        if (selectedObject == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            selectedObject.position = hit.point + offset;
        }
    }
    private void MoveObjectWithMouse()
    {
        if (selectedObject == null)
        {
            debugText.text += "마우스 이동 선택 오브젝트 없음\n";
            return;
        }

        // float moveSpeed = 0.2f; // Adjust movement sensitivity
        // float moveX = Input.GetAxis("Mouse X") * moveSpeed;
        // float moveY = Input.GetAxis("Mouse Y") * moveSpeed;
        //
        // // Calculate movement direction based on camera orientation
        // Vector3 right = mainCamera.transform.right; // Camera's right direction
        // Vector3 up = mainCamera.transform.up; // Camera's up direction
        //
        // // Apply movement to the selected object
        // selectedObject.position += right * moveX + up * moveY;
        //Input.mousePosition;
        var touchPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.WorldToScreenPoint(selectedObject.position).z);
        var worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        selectedObject.position = worldPosition;
    }

    /// <summary>
    /// Rotates the selected object based on mouse movement.
    /// Uses the camera's orientation to determine the axes for rotation and applies the
    /// rotation incrementally based on mouse movement.
    /// </summary>
    private void RotateObjectWithMouse()
    {
        if (selectedObject == null)
        {
            debugText.text += "마우스 회전 선택 오브젝트 없음\n";
            return;
        }

        float rotationSpeed = 500f;
        Vector3 cameraRight = mainCamera.transform.right;
        Vector3 cameraUp = mainCamera.transform.up;

        float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        selectedObject.Rotate(cameraUp, -rotationX, Space.World);
        selectedObject.Rotate(cameraRight, rotationY, Space.World);
    }

    /// <summary>
    /// Scales the selected object based on mouse scroll input.
    /// Adjusts the scale incrementally, ensuring smooth zooming in and out.
    /// </summary>
    private void ScaleObjectWithMouse()
    {
        if (selectedObject == null)
        {
            debugText.text += "마우스 스케일 선택 오브젝트 없음\n";
            return;
        }

        float scaleSpeed = 0.1f;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            selectedObject.localScale += Vector3.one * scroll * scaleSpeed;
        }
    }

    private void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            HandleSingleTouch();
        }
        else if (Input.touchCount == 2)
        {
            ScaleObjectWithTouch();
        }
    }

    private void HandleSingleTouch()
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = mainCamera.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag(targetTag))
                {
                    selectedObject = hit.transform;
                    offset = selectedObject.position - hit.point;
                }
            }
        }

        if (touch.phase == TouchPhase.Moved && selectedObject != null)
        {
            if (isMoveMode)
            {
                MoveObjectWithTouch(touch);
            }
            else if (isRotateMode)
            {
                RotateObjectWithTouch(touch);
            }
        }

        if (touch.phase == TouchPhase.Ended)
        {
            selectedObject = null;
        }
    }

    /// <summary>
    /// Moves the selected object based on touch input.
    /// Calculates the new position using a raycast to determine the point in the scene
    /// where the touch is pointing and applies an offset to maintain consistent movement.
    /// </summary>
    private void MoveObjectWithTouch2(Touch touch)
    {
        Ray ray = mainCamera.ScreenPointToRay(touch.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            selectedObject.position = hit.point + offset;
        }
    }
    private void MoveObjectWithTouch(Touch touch)
    {
        if (selectedObject == null) return;

        // float moveSpeed = 0.1f; // Adjust movement sensitivity
        // Vector2 deltaPosition = touch.deltaPosition * moveSpeed;
        //
        // // Calculate movement direction based on camera orientation
        // Vector3 right = mainCamera.transform.right; // Camera's right direction
        // Vector3 up = mainCamera.transform.up; // Camera's up direction
        //
        // // Apply movement to the selected object
        // selectedObject.position += right * deltaPosition.x + up * deltaPosition.y;
        var touchPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, mainCamera.WorldToScreenPoint(selectedObject.position).z);
        var worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        selectedObject.position = worldPosition;
    }

    /// <summary>
    /// Rotates the selected object based on touch movement.
    /// Calculates rotation using the delta movement of the touch and applies it
    /// relative to the camera's orientation.
    /// </summary>
    private void RotateObjectWithTouch(Touch touch)
    {
        float rotationSpeed = 0.2f;
        Vector2 deltaPosition = touch.deltaPosition;

        Vector3 cameraRight = mainCamera.transform.right;
        Vector3 cameraUp = mainCamera.transform.up;

        selectedObject.Rotate(cameraUp, -deltaPosition.x * rotationSpeed, Space.World);
        selectedObject.Rotate(cameraRight, deltaPosition.y * rotationSpeed, Space.World);
    }

    /// <summary>
    /// Scales the selected object based on two-finger touch movement.
    /// Calculates the difference in distance between two fingers and adjusts
    /// the object's scale proportionally.
    /// </summary>
    private void ScaleObjectWithTouch()
    {
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
        {
            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

            if (selectedObject != null)
            {
                float scaleSpeed = 0.01f;
                selectedObject.localScale += Vector3.one * deltaMagnitudeDiff * scaleSpeed;
            }
        }
    }

    /// <summary>
    /// Enables move mode and disables rotate mode.
    /// </summary>
    public void EnableMoveMode()
    {
        isMoveMode = true;
        isRotateMode = false;
        GasSysIPartList.Instance.GetMoveBtn().interactable = !isMoveMode;
        GasSysIPartList.Instance.GetRotateBtn().interactable = isMoveMode;
    }

    /// <summary>
    /// Enables rotate mode and disables move mode.
    /// </summary>
    public void EnableRotateMode()
    {
        isMoveMode = false;
        isRotateMode = true;
        GasSysIPartList.Instance.GetMoveBtn().interactable = !isMoveMode;
        GasSysIPartList.Instance.GetRotateBtn().interactable = isMoveMode;
    }
}


// using UnityEngine;
// using UniRx;
//
// public class ObjectControl : MonoBehaviour
// {
//     public string targetTag = "Rotatable"; // 회전 및 스케일 가능한 오브젝트의 태그
//
//     [SerializeField] private Camera mainCamera;
//     private Transform selectedObject;
//
//     private void Start()
//     {
//         // 마우스 입력 처리
//         Observable.EveryUpdate()
//             .Where(_ => Input.GetMouseButtonDown(0))
//             .Subscribe(_ => OnMouseDown())
//             .AddTo(this);
//
//         Observable.EveryUpdate()
//             .Where(_ => Input.GetMouseButton(0) && selectedObject != null)
//             .Subscribe(_ => OnMouseDrag())
//             .AddTo(this);
//
//         Observable.EveryUpdate()
//             .Where(_ => Input.GetMouseButtonUp(0))
//             .Subscribe(_ => OnMouseUp())
//             .AddTo(this);
//
//         // 터치 입력 처리
//         Observable.EveryUpdate()
//             .Where(_ => Input.touchCount > 0)
//             .Subscribe(_ => HandleTouch())
//             .AddTo(this);
//     }
//
//     private void OnMouseDown()
//     {
//         Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
//         if (Physics.Raycast(ray, out RaycastHit hit))
//         {
//             if (hit.collider.CompareTag(targetTag))
//             {
//                 selectedObject = hit.transform;
//             }
//         }
//     }
//
//     private void OnMouseDrag()
//     {
//         if (selectedObject == null) return;
//
//         float rotationSpeed = 500f;
//         float scaleSpeed = 0.1f;
//
//         // 마우스 이동에 따른 회전 (카메라 방향 기준)
//         Vector3 cameraRight = mainCamera.transform.right;
//         Vector3 cameraUp = mainCamera.transform.up;
//
//         float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
//         float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
//
//         selectedObject.Rotate(cameraUp, -rotationX, Space.World);
//         selectedObject.Rotate(cameraRight, rotationY, Space.World);
//
//         // 마우스 휠에 따른 스케일 조정
//         float scroll = Input.GetAxis("Mouse ScrollWheel");
//         if (Mathf.Abs(scroll) > 0.01f)
//         {
//             selectedObject.localScale += Vector3.one * scroll * scaleSpeed;
//         }
//     }
//
//     private void OnMouseUp()
//     {
//         selectedObject = null;
//     }
//
//     private void HandleTouch()
//     {
//         if (Input.touchCount == 1) // 한 손가락 터치: 회전
//         {
//             Touch touch = Input.GetTouch(0);
//
//             if (touch.phase == TouchPhase.Began)
//             {
//                 Ray ray = mainCamera.ScreenPointToRay(touch.position);
//                 if (Physics.Raycast(ray, out RaycastHit hit))
//                 {
//                     if (hit.collider.CompareTag(targetTag))
//                     {
//                         selectedObject = hit.transform;
//                     }
//                 }
//             }
//
//             if (touch.phase == TouchPhase.Moved && selectedObject != null)
//             {
//                 float rotationSpeed = 0.2f;
//                 Vector2 deltaPosition = touch.deltaPosition;
//
//                 Vector3 cameraRight = mainCamera.transform.right;
//                 Vector3 cameraUp = mainCamera.transform.up;
//
//                 selectedObject.Rotate(cameraUp, -deltaPosition.x * rotationSpeed, Space.World);
//                 selectedObject.Rotate(cameraRight, deltaPosition.y * rotationSpeed, Space.World);
//             }
//
//             if (touch.phase == TouchPhase.Ended)
//             {
//                 selectedObject = null;
//             }
//         }
//         else if (Input.touchCount == 2) // 두 손가락 터치: 스케일 조정
//         {
//             Touch touch0 = Input.GetTouch(0);
//             Touch touch1 = Input.GetTouch(1);
//
//             if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
//             {
//                 Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
//                 Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
//
//                 float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
//                 float touchDeltaMag = (touch0.position - touch1.position).magnitude;
//
//                 float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;
//
//                 if (selectedObject != null)
//                 {
//                     float scaleSpeed = 0.01f;
//                     selectedObject.localScale += Vector3.one * deltaMagnitudeDiff * scaleSpeed;
//                 }
//             }
//         }
//     }
// }
