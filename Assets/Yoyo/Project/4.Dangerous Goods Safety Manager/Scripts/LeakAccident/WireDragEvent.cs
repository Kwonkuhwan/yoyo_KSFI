using UnityEngine;
using RJH.DangerousGoods;
using UnityEngine.UI;
public class WireDragEvent : MonoBehaviour
{
    [SerializeField] private UIDragAndCollisionHandler dragHandler;
    [SerializeField] private RectTransform wireRect;
    [SerializeField] private Image setWire;
    [SerializeField] private RectTransform setWireRect;
    [Space(10)]
    [SerializeField] private RectTransform wireedgeRect;
    [SerializeField] private RectTransform setwireedgeRect;
    [SerializeField] private RectTransform truckRect;
    [SerializeField] private RectTransform lineRect;
    [Space]
    [SerializeField] private string titleText;
    [SerializeField] private string descriptionText;
    [SerializeField] private AudioClip audioClip;
    private RectTransform currentwireRect;
    private bool audioplaying;
    private void OnEnable()
    { 
        WireEvent();
        currentwireRect = wireedgeRect;
        audioplaying = false;
    }

    private void Update()
    {
        DrawLineBetweenImages(currentwireRect, truckRect, lineRect);
    }

    public void WireEvent()
    {
        dragHandler.ResetEvent();
        dragHandler.OnCollisionDetected += (GameObject d, GameObject t) =>
        {
            d.SetActive(false);
            currentwireRect = setwireedgeRect;
            setWire.enabled = true;
            t.GetComponent<DocsButton>().OnClick();
            SectionAndBackGroundManager.Instance.SetDocument_text(titleText, descriptionText);
            if (audioplaying == false)
            {
                audioplaying = true;
                AudioManager.Instance.PlayDocs(audioClip);
            }
        };
    }

    private void OnDisable()
    {
        setWire.enabled = false;
        dragHandler.ResetEvent();
    }

    public void DrawLineBetweenImages(RectTransform image1, RectTransform image2, RectTransform lineRect, float lineWidth = 5f)
    {
        // 이미지의 실제 위치 계산 (pivot을 반영)
        Vector2 startPos = image1.position;
        Vector2 endPos = image2.position + new Vector3(0, (-image2.rect.height * image2.pivot.y) + 6f, 0); // pivot(0.5, 0) 보정

        // 라인의 위치 및 크기 설정
        Vector2 direction = (endPos - startPos).normalized;
        float distance = Vector2.Distance(startPos, endPos);

        lineRect.sizeDelta = new Vector2(distance, lineWidth);
        lineRect.position = startPos + (endPos - startPos) / 2;
        lineRect.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        // 생성된 라인을 Dictionary에 저장
        //existingLines.Add(lineObject);
    }
}
