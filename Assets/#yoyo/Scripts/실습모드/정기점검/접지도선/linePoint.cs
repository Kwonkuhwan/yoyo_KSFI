using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class linePoint : MonoBehaviour
    {
        public GameObject linepoint1;
        public GameObject linepoint2;

        Dictionary<string, GameObject> existingLines = new Dictionary<string, GameObject>();

        List<GameObject> lines = new List<GameObject>();

        [SerializeField] private float linewidth = 2.0f;
        [SerializeField] private Color lineColor;


        private void Update()
        {
            foreach (var line in lines)
            {
                Destroy(line.gameObject);
            }

            lines.Clear();
            DrawLineBetweenImages(linepoint1.GetComponent<RectTransform>(), linepoint2.GetComponent<RectTransform>(), transform, lineColor, linewidth);
        }


        public void DrawLineBetweenImages(RectTransform image1, RectTransform image2, Transform parent, Color lineColor, float lineWidth = 2f)
        {
            string lineKey = $"{image1.name}-{image2.name}";

            GameObject lineObject = new GameObject("Line");
            lineObject.transform.SetParent(parent);
            lineObject.layer = parent.gameObject.layer;

            // Line의 RectTransform 설정
            RectTransform lineRect = lineObject.AddComponent<RectTransform>();
            lineRect.anchorMin = Vector2.zero;
            lineRect.anchorMax = Vector2.zero;
            lineRect.pivot = new Vector2(0.5f, 0.5f);

            // Line 이미지 추가
            Image lineImage = lineObject.AddComponent<Image>();
            lineImage.color = lineColor;

            // 이미지의 실제 위치 계산 (pivot을 반영)
            Vector2 startPos = image1.TransformPoint(new Vector3(0, 0, 0));//image1.position;
            Vector2 endPos = image2.TransformPoint(new Vector3(0, (-image2.rect.height * image2.pivot.y) + 6f, 0)); // 이미지 2의 pivot(0.5, 0)을 기준//image2.position + new Vector3(0, (-image2.rect.height * image2.pivot.y)+6f, 0); // pivot(0.5, 0) 보정

            // 라인의 위치 및 크기 설정
            Vector2 direction = (endPos - startPos).normalized;
            float distance = Vector2.Distance(startPos, endPos);

            lineRect.sizeDelta = new Vector2(distance, lineWidth);
            Vector2 vector2 = startPos + (endPos - startPos) / 2;
            Vector3 vector3 = new Vector3(vector2.x, vector2.y, 0);
            lineRect.position = vector3;
            lineRect.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            // 생성된 라인을 Dictionary에 저장
            lines.Add(lineObject);

            //existingLines.Add(lineKey, lineObject);
        }
    }
}
