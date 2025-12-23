using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

#if RectTransformEditorExtension
[CustomEditor(typeof(RectTransform))]
public class RectTransformEditorExtension : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 RectTransform Inspector 표시
        DrawDefaultInspector();

        
        // 'Paste From Clipboard' 버튼 추가
        if (GUILayout.Button("Paste RectTransform From Clipboard"))
        {
            PasteRectTransformFromClipboard();
        }
        
    }

    void PasteRectTransformFromClipboard()
    {
        // 클립보드에서 텍스트 가져오기
        string clipboardText = GUIUtility.systemCopyBuffer;

        // 첫 번째 형식 (Pos.X, Pos.Y, Width, Height)을 처리하는 정규식
        Regex regex1 = new Regex(@"Pos\.X:(-?\d+\.?\d*)\s+Pos\.Y:(-?\d+\.?\d*)[\s\S]+Width:(-?\d+\.?\d*)\s+Height:(-?\d+\.?\d*)[\s\S]+Min:\s+\[X:(-?\d+\.?\d*)\s+Y:(-?\d+\.?\d*)\][\s\S]+Max:\s+\[X:(-?\d+\.?\d*)\s+Y:(-?\d+\.?\d*)\][\s\S]+Pivot:\s+\[X:(-?\d+\.?\d*)\s+Y:(-?\d+\.?\d*)\]");
        
        // 두 번째 형식 (Left, Right, Height, Pos.Y)을 처리하는 정규식
        Regex regex2 = new Regex(@"Left:(-?\d+\.?\d*)\s+Pos\.Y:(-?\d+\.?\d*)[\s\S]+Right:(-?\d+\.?\d*)\s+Height:(-?\d+\.?\d*)[\s\S]+Min:\s+\[X:(-?\d+\.?\d*)\s+Y:(-?\d+\.?\d*)\][\s\S]+Max:\s+\[X:(-?\d+\.?\d*)\s+Y:(-?\d+\.?\d*)\][\s\S]+Pivot:\s+\[X:(-?\d+\.?\d*)\s+Y:(-?\d+\.?\d*)\]");

        Match match1 = regex1.Match(clipboardText);
        Match match2 = regex2.Match(clipboardText);

        if (target is RectTransform rectTransform)
        {
            if (match1.Success)
            {
                // 첫 번째 형식 값을 RectTransform에 적용
                float posX = float.Parse(match1.Groups[1].Value);
                float posY = float.Parse(match1.Groups[2].Value);
                float width = float.Parse(match1.Groups[3].Value);
                float height = float.Parse(match1.Groups[4].Value);
                float anchorMinX = float.Parse(match1.Groups[5].Value);
                float anchorMinY = float.Parse(match1.Groups[6].Value);
                float anchorMaxX = float.Parse(match1.Groups[7].Value);
                float anchorMaxY = float.Parse(match1.Groups[8].Value);
                float pivotX = float.Parse(match1.Groups[9].Value);
                float pivotY = float.Parse(match1.Groups[10].Value);

                Undo.RecordObject(rectTransform, "Paste RectTransform Values");
                rectTransform.anchoredPosition = new Vector2(posX, posY);
                rectTransform.sizeDelta = new Vector2(width, height);
                rectTransform.anchorMin = new Vector2(anchorMinX, anchorMinY);
                rectTransform.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
                rectTransform.pivot = new Vector2(pivotX, pivotY);

                EditorUtility.SetDirty(rectTransform);
                Debug.Log("RectTransform values pasted from clipboard using Pos.X, Pos.Y, Width, Height format.");
            }
            else if (match2.Success)
            {
                // 두 번째 형식 값을 RectTransform에 적용
                float left = float.Parse(match2.Groups[1].Value);
                float posY = float.Parse(match2.Groups[2].Value);
                float right = float.Parse(match2.Groups[3].Value);
                float height = float.Parse(match2.Groups[4].Value);
                float anchorMinX = float.Parse(match2.Groups[5].Value);
                float anchorMinY = float.Parse(match2.Groups[6].Value);
                float anchorMaxX = float.Parse(match2.Groups[7].Value);
                float anchorMaxY = float.Parse(match2.Groups[8].Value);
                float pivotX = float.Parse(match2.Groups[9].Value);
                float pivotY = float.Parse(match2.Groups[10].Value);

                Undo.RecordObject(rectTransform, "Paste RectTransform Values");
                rectTransform.offsetMin = new Vector2(left, rectTransform.offsetMin.y); // Left 값
                rectTransform.offsetMax = new Vector2(-right, rectTransform.offsetMax.y); // Right 값
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height); // Height 값
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, posY); // Pos.Y 값
                rectTransform.anchorMin = new Vector2(anchorMinX, anchorMinY);
                rectTransform.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
                rectTransform.pivot = new Vector2(pivotX, pivotY);

                EditorUtility.SetDirty(rectTransform);
                Debug.Log("RectTransform values pasted from clipboard using Left, Right, Pos.Y, Height format.");
            }
            else
            {
                Debug.LogWarning("Clipboard text format is invalid.");
            }
        }
    }
}

#endif


// using UnityEngine;
// using UnityEditor;
// using System.Text.RegularExpressions;
//
// [CustomEditor(typeof(RectTransform))]
// public class RectTransformEditorExtension : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         // 기본 RectTransform Inspector 표시
//         DrawDefaultInspector();
//
//         // 'Paste From Clipboard' 버튼 추가
//         if (GUILayout.Button("Paste RectTransform From Clipboard"))
//         {
//             PasteRectTransformFromClipboard();
//         }
//     }
//
//     void PasteRectTransformFromClipboard()
//     {
//         // 클립보드에서 텍스트 가져오기
//         string clipboardText = GUIUtility.systemCopyBuffer;
//
//         // 첫 번째 형식 (Pos.X, Pos.Y, Width, Height)을 처리하는 정규식
//         Regex regex1 = new Regex(@"Pos\.X:(-?\d+\.?\d*)\s+Pos\.Y:(-?\d+\.?\d*)[\s\S]+Width:(-?\d+\.?\d*)\s+Height:(-?\d+\.?\d*)[\s\S]+Min:\s+\[X:(-?\d+\.?\d*)\s+Y:(-?\d+\.?\d*)\][\s\S]+Max:\s+\[X:(-?\d+\.?\d*)\s+Y:(-?\d+\.?\d*)\][\s\S]+Pivot:\s+\[X:(-?\d+\.?\d*)\s+Y:(-?\d+\.?\d*)\]");
//
//         // 두 번째 형식 (Left, Right, Height, Pos.Y)을 처리하는 정규식
//         Regex regex2 = new Regex(@"Left:(-?\d+\.?\d*)\s+Pos\.Y:(-?\d+\.?\d*)[\s\S]+Right:(-?\d+\.?\d*)\s+Height:(-?\d+\.?\d*)[\s\S]+Min:\s+\[X:(-?\d+\.?\d*)\s+Y:(-?\d+\.?\d*)\][\s\S]+Max:\s+\[X:(-?\d+\.?\d*)\s+Y:(-?\d+\.?\d*)\][\s\S]+Pivot:\s+\[X:(-?\d+\.?\d*)\s+Y:(-?\d+\.?\d*)\]");
//
//         Match match1 = regex1.Match(clipboardText);
//         Match match2 = regex2.Match(clipboardText);
//
//         if (target is RectTransform rectTransform)
//         {
//             if (match1.Success)
//             {
//                 // 첫 번째 형식 값을 RectTransform에 적용
//                 float posX = float.Parse(match1.Groups[1].Value);
//                 float posY = float.Parse(match1.Groups[2].Value);
//                 float width = float.Parse(match1.Groups[3].Value);
//                 float height = float.Parse(match1.Groups[4].Value);
//                 float anchorMinX = float.Parse(match1.Groups[5].Value);
//                 float anchorMinY = float.Parse(match1.Groups[6].Value);
//                 float anchorMaxX = float.Parse(match1.Groups[7].Value);
//                 float anchorMaxY = float.Parse(match1.Groups[8].Value);
//                 float pivotX = float.Parse(match1.Groups[9].Value);
//                 float pivotY = float.Parse(match1.Groups[10].Value);
//
//                 Undo.RecordObject(rectTransform, "Paste RectTransform Values");
//                 rectTransform.anchoredPosition = new Vector2(posX, posY);
//                 rectTransform.sizeDelta = new Vector2(width, height);
//                 rectTransform.anchorMin = new Vector2(anchorMinX, anchorMinY);
//                 rectTransform.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
//                 rectTransform.pivot = new Vector2(pivotX, pivotY);
//
//                 EditorUtility.SetDirty(rectTransform);
//                 Debug.Log("RectTransform values pasted from clipboard using Pos.X, Pos.Y, Width, Height format.");
//             }
//             else if (match2.Success)
//             {
//                 // 두 번째 형식 값을 RectTransform에 적용
//                 float left = float.Parse(match2.Groups[1].Value);
//                 float posY = float.Parse(match2.Groups[2].Value);
//                 float right = float.Parse(match2.Groups[3].Value);
//                 float height = float.Parse(match2.Groups[4].Value);
//                 float anchorMinX = float.Parse(match2.Groups[5].Value);
//                 float anchorMinY = float.Parse(match2.Groups[6].Value);
//                 float anchorMaxX = float.Parse(match2.Groups[7].Value);
//                 float anchorMaxY = float.Parse(match2.Groups[8].Value);
//                 float pivotX = float.Parse(match2.Groups[9].Value);
//                 float pivotY = float.Parse(match2.Groups[10].Value);
//
//                 Undo.RecordObject(rectTransform, "Paste RectTransform Values");
//                 rectTransform.offsetMin = new Vector2(left, rectTransform.offsetMin.y); // Left 값
//                 rectTransform.offsetMax = new Vector2(-right, rectTransform.offsetMax.y); // Right 값
//                 rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height); // Height 값
//                 rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, posY); // Pos.Y 값
//                 rectTransform.anchorMin = new Vector2(anchorMinX, anchorMinY);
//                 rectTransform.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
//                 rectTransform.pivot = new Vector2(pivotX, pivotY);
//
//                 EditorUtility.SetDirty(rectTransform);
//                 Debug.Log("RectTransform values pasted from clipboard using Left, Right, Pos.Y, Height format.");
//             }
//             else
//             {
//                 Debug.LogWarning("Clipboard text format is invalid.");
//             }
//         }
//     }
// }
