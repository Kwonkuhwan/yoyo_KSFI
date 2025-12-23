using UnityEditor;
#if Import
namespace Yoyo.Project.Common._2.Scripts.Editor
{
    public class CustomAutoSetTextureType : AssetPostprocessor
    {
        private const string TargetFolder = "Assets/Yoyo";

        private void OnPreprocessTexture()
        {
            // Asset 경로 확인
            if (!assetPath.StartsWith(TargetFolder))
                return;
        
            // Texture Importer 가져오기
            var textureImporter = (TextureImporter)assetImporter;

            // Texture Type을 Sprite로 설정
            textureImporter.textureType = TextureImporterType.Sprite;

            // 추가 설정 (필요에 따라 수정)
            textureImporter.spritePixelsPerUnit = 100; // 픽셀 단위
            textureImporter.mipmapEnabled = false; // MipMap 비활성화
            //textureImporter.filterMode = FilterMode.Bilinear; // 필터 모드 설정
        }
    }
}
#endif