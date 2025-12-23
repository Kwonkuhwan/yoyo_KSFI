using UnityEngine;
using UnityEngine.UI;

public class UIWheelZoom : MonoBehaviour
{
    public RectTransform targetUI;  // 확대/축소할 UI 요소
    public float zoomSpeed = 0.1f;  // 확대/축소 속도
    public float minScale = 0.5f;   // 최소 스케일
    public float maxScale = 2.0f;   // 최대 스케일

    void Update()
    {
        ZoomWithScrollWheel();
    }

    void ZoomWithScrollWheel()
    {
        // 마우스 휠의 스크롤 값을 가져옴
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            // 현재 UI의 로컬 스케일을 가져옴
            Vector3 currentScale = targetUI.localScale;

            // 스크롤 값에 따라 확대/축소 계산
            float newScale = currentScale.x + scroll * zoomSpeed;

            // 스케일 값을 최소/최대로 제한
            newScale = Mathf.Clamp(newScale, minScale, maxScale);

            // UI 요소의 스케일 적용
            targetUI.localScale = new Vector3(newScale, newScale, 1);
        }
    }
}
