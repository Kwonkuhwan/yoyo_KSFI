using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(HintScriptableObj))]
public class HintScriptableObjEditor : Editor
{
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        // 대상 객체 가져오기
        HintScriptableObj hintScriptableObj = (HintScriptableObj)target;

        // ReorderableList 초기화
        reorderableList = new ReorderableList(
            serializedObject,
            serializedObject.FindProperty("hintData"),
            true, // 드래그로 순서 변경 가능
            true, // 헤더 표시
            true, // 추가 버튼 활성화
            true  // 삭제 버튼 활성화
        );

        // 헤더 설정
        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Hint Data");
        };

        // 요소 높이 설정
        reorderableList.elementHeightCallback = (int index) =>
        {
            return EditorGUIUtility.singleLineHeight * 5 + 20; // 요소 높이를 충분히 크게 설정
        };

        // 리스트 요소 그리기
        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            // 인덱스 표시
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 30, EditorGUIUtility.singleLineHeight), $"[{index}]");

            // Title 필드
            EditorGUI.LabelField(new Rect(rect.x + 35, rect.y, 50, EditorGUIUtility.singleLineHeight), "Title");
            EditorGUI.PropertyField(new Rect(rect.x + 85, rect.y, rect.width - 85, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("title"), GUIContent.none);

            // TextArea 필드
            EditorGUI.LabelField(new Rect(rect.x + 35, rect.y + 20, 50, EditorGUIUtility.singleLineHeight), "Text");
            EditorGUI.PropertyField(new Rect(rect.x + 85, rect.y + 20, rect.width - 85, EditorGUIUtility.singleLineHeight * 2), element.FindPropertyRelative("text"), GUIContent.none);

            // AudioClip 필드
            EditorGUI.PropertyField(new Rect(rect.x + 35, rect.y + 60, rect.width - 35, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("audioClip"), new GUIContent("Audio Clip"));
        };

        // 항목 추가
        reorderableList.onAddCallback = (ReorderableList list) =>
        {
            reorderableList.serializedProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();
        };

        // 항목 제거
        reorderableList.onRemoveCallback = (ReorderableList list) =>
        {
            if (EditorUtility.DisplayDialog("확인", "선택한 항목을 삭제하시겠습니까?", "예", "아니오"))
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                serializedObject.ApplyModifiedProperties();
            }
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ReorderableList 표시
        reorderableList.DoLayoutList();

        // 변경 사항 저장
        serializedObject.ApplyModifiedProperties();
    }
}

/*
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(HintScriptableObj))]
public class HintScriptableObjEditor : Editor
{
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        HintScriptableObj hintScriptableObj = (HintScriptableObj)target;

        // ReorderableList 초기화
        reorderableList = new ReorderableList(hintScriptableObj.hintData, typeof(HintTextAndAudio), true, true, true, true);

        // 헤더 설정
        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Hint Data");
        };

        // 요소 높이 설정
        reorderableList.elementHeightCallback = (int index) =>
        {
            return EditorGUIUtility.singleLineHeight * 5 + 20; // 요소 높이를 충분히 크게 설정
        };

        // 리스트 요소 그리기
        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = hintScriptableObj.hintData[index];
            rect.y += 2;

            // Title 필드
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 50, EditorGUIUtility.singleLineHeight), "Title");
            element.title = EditorGUI.TextField(new Rect(rect.x + 50, rect.y, rect.width - 50, EditorGUIUtility.singleLineHeight), element.title);

            // TextArea 필드
            EditorGUI.LabelField(new Rect(rect.x, rect.y + 20, 50, EditorGUIUtility.singleLineHeight), "Text");
            element.text = EditorGUI.TextArea(new Rect(rect.x + 50, rect.y + 20, rect.width - 50, 40), element.text);

            // AudioClip 필드
            element.audioClip = (AudioClip)EditorGUI.ObjectField(new Rect(rect.x, rect.y + 70, rect.width, EditorGUIUtility.singleLineHeight), "Audio Clip", element.audioClip, typeof(AudioClip), false);
        };

        // 항목 추가 및 제거 정의
        reorderableList.onAddCallback = (ReorderableList list) =>
        {
            hintScriptableObj.hintData.Add(new HintTextAndAudio());
        };
        reorderableList.onRemoveCallback = (ReorderableList list) =>
        {
            hintScriptableObj.hintData.RemoveAt(list.index);
        };
    }

    public override void OnInspectorGUI()
    {
        HintScriptableObj hintScriptableObj = (HintScriptableObj)target;
        
        // ReorderableList 표시
        reorderableList.DoLayoutList();

        // 변경 사항 저장
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
    
    
}
*/