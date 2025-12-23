using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 이재성
/// Sprite Size즈로 Image Size 수정
/// </summary>
[RequireComponent(typeof(Image))]
public class SpriteAutoSize : MonoBehaviour
{
    [SerializeField] private Image image; // UI Image용 변수
    //public SpriteRenderer spriteRenderer; // 2D SpriteRenderer용 변수

    // 이미지 크기를 자동으로 조정하는 메서드
    public Vector2 maxSize = new Vector2(500, 500); // 최대 크기
    public Vector2 minSize = new Vector2(100, 100); // 최소 크기

    private Vector2 _min = Vector2.zero;
    private Vector2 _max = Vector2.zero;
     // 이미지 크기를 비율에 맞춰 자동으로 조정하는 메서드
    public void AutoResizeSprite()
    {
        if(null == image )
            image = GetComponent<Image>();
        
        if (image != null && image.sprite != null)
        {
            ResizeImage(image.sprite.rect.width, image.sprite.rect.height);
        }
        // else if (spriteRenderer != null && spriteRenderer.sprite != null)
        // {
        //     ResizeImage(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
        // }
    }

    // 실제 크기 조정 로직
    private void ResizeImage(float spriteWidth, float spriteHeight)
    {
        float aspectRatio = spriteWidth / spriteHeight;

        // 가로 세로 비율에 맞춰 최대 크기와 최소 크기 적용
        var newSize = new Vector2(spriteWidth, spriteHeight);

        // 최대 크기 적용
        if (newSize.x > maxSize.x || newSize.y > maxSize.y)
        {
            if (newSize.x > maxSize.x)
            {
                newSize.x = maxSize.x;
                newSize.y = maxSize.x / aspectRatio;
            }

            if (newSize.y > maxSize.y)
            {
                newSize.y = maxSize.y;
                newSize.x = maxSize.y * aspectRatio;
            }
        }

        // 최소 크기 적용
        if (newSize.x < minSize.x || newSize.y < minSize.y)
        {
            if (newSize.x < minSize.x)
            {
                newSize.x = minSize.x;
                newSize.y = minSize.x / aspectRatio;
            }

            if (newSize.y < minSize.y)
            {
                newSize.y = minSize.y;
                newSize.x = minSize.y * aspectRatio;
            }
        }

        // UI Image일 경우 RectTransform 크기 조정
        if (image == null)
            return;
        var rt = image.GetComponent<RectTransform>();
        rt.sizeDelta = newSize;
        // // 2D SpriteRenderer일 경우 localScale 조정
        // else if (spriteRenderer != null)
        // {
        //     transform.localScale = new Vector3(newSize.x, newSize.y, 1);
        // }
    }
}
