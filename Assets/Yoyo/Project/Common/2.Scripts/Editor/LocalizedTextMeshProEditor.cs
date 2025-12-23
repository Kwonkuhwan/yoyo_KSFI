using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocalizedTextMeshProEditor : MonoBehaviour
{
    // 메뉴에 항목 추가
    [MenuItem("GameObject/UI/Localized TextMeshPro", false, 10)]
    private static void CreateLocalizedTextMeshPro(MenuCommand menuCommand)
    {
        // Canvas가 없으면 새로 생성
        GameObject canvas = GameObject.FindObjectOfType<Canvas>()?.gameObject;
        if (canvas == null)
        {
            canvas = new GameObject("Canvas");
            canvas.AddComponent<Canvas>();
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            Undo.RegisterCreatedObjectUndo(canvas, "Create Canvas");
        }

        // TextMeshPro 텍스트 생성
        GameObject localizedText = new GameObject("LocalizedTextMeshPro");
        localizedText.AddComponent<RectTransform>();
        TextMeshProUGUI tmpText = localizedText.AddComponent<TextMeshProUGUI>();
        tmpText.text = "Localized Text";  // 기본 텍스트
        tmpText.fontSize = 36;  // 기본 폰트 크기 설정
        tmpText.color = Color.white;
        localizedText.transform.SetParent(canvas.transform, false);

        // LocalizedTextMeshPro 컴포넌트 추가
        localizedText.AddComponent<LocalizedTextMeshPro>();

        // 새롭게 생성된 오브젝트를 선택
        Selection.activeObject = localizedText;

        // Undo를 위해 등록
        Undo.RegisterCreatedObjectUndo(localizedText, "Create Localized TextMeshPro");
    }
}
