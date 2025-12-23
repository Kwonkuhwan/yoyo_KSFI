#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
/// <summary>
/// 이재성
/// Sprite Size즈로 Image Size 수정
/// </summary>
[CustomEditor(typeof(SpriteAutoSize))]
public class SpriteAutoSizeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 그리기
        DrawDefaultInspector();

        // SpriteAutoSize 스크립트에 대한 참조 가져오기
        var spriteAutoSize = (SpriteAutoSize)target;

        // 버튼 추가
        if (GUILayout.Button("자동 크기 조정"))
        {
            // 버튼 클릭 시 AutoResizeSprite 메서드 호출
            spriteAutoSize.AutoResizeSprite();
        }
    }
}
#endif
