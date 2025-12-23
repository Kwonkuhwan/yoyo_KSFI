using UnityEngine;
using UnityEngine.UI;

public class UIWheelZoomAndMove : MonoBehaviour
{
    public RectTransform[] targetUIs; // 여러 개의 UI 요소
    public float zoomSpeed = 0.1f; // 확대/축소 속도
    public float moveSpeed = 10f;  // 상하좌우 이동 속도
    public float minScale = 0.5f;  // 최소 스케일
    public float maxScale = 2.0f;  // 최대 스케일
    private Vector3[] originalScales; // 원래 스케일 저장
    private Vector3[] originalPositions; // 원래 위치 저장

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
        MoveWithKeyboard();
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

    // 키보드로 상하좌우 이동 (WASD 또는 방향키 사용)
    void MoveWithKeyboard()
    {
        Vector3 moveDirection = new Vector3(
            Input.GetAxis("Horizontal"), // 좌우 이동 (A, D 또는 ←, →)
            Input.GetAxis("Vertical"),   // 상하 이동 (W, S 또는 ↑, ↓)
            0
        );

        if (moveDirection != Vector3.zero)
        {
            foreach (RectTransform targetUI in targetUIs)
            {
                targetUI.localPosition += moveDirection * moveSpeed * Time.deltaTime;
            }
        }
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
