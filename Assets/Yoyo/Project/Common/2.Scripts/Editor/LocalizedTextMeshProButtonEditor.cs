using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedTextMeshProButtonEditor : MonoBehaviour
{
    // 메뉴에 항목 추가
    [MenuItem("GameObject/UI/Localized TextMeshPro Button", false, 10)]
    private static void CreateLocalizedTextMeshProButton(MenuCommand menuCommand)
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

        // 버튼 생성
        GameObject button = new GameObject("Localized Button");
        button.AddComponent<RectTransform>();
        button.AddComponent<Image>();
        button.AddComponent<Button>();
        button.transform.SetParent(canvas.transform, false);

        // TextMeshPro 텍스트 추가
        GameObject buttonText = new GameObject("TextMeshPro Text");
        buttonText.AddComponent<RectTransform>();
        TextMeshProUGUI tmpText = buttonText.AddComponent<TextMeshProUGUI>();
        tmpText.text = "Localized Button";  // 기본 텍스트
        tmpText.color = Color.black;
        buttonText.transform.SetParent(button.transform, false);

        // 버튼에 LocalizedTextMeshProButton 컴포넌트 추가
        button.AddComponent<LocalizedTextMeshProButton>();

        // 새롭게 생성된 오브젝트를 선택
        Selection.activeObject = button;

        // Undo를 위해 등록
        Undo.RegisterCreatedObjectUndo(button, "Create Localized TextMeshPro Button");
    }
}
