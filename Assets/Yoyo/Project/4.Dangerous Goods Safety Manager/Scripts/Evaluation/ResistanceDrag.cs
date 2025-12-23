using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResistanceDrag : MonoBehaviour
{
    [SerializeField] private GameObject numberImage;
    [SerializeField] private GameObject checkListImage;
    [SerializeField] private RectTransform redPinRect;
    [SerializeField] private RectTransform blackPinRect;
    [Space(10)]
    [SerializeField] private RectTransform minusRect;
    [SerializeField] private RectTransform plusRect;
    [Space(10)]
    [SerializeField] private RectTransform redPinSetPlaceRect;
    [SerializeField] private RectTransform blackPinSetPlaceRect;
    [Space(10)]
    [SerializeField] private RectTransform redPlugRect;
    [SerializeField] private RectTransform blackPlugRect;
    [Space(10)]
    [SerializeField] private RectTransform redLineRect;
    [SerializeField] private RectTransform blackLineRect;
    [Space(10)]
    [SerializeField] private Button eAR2WButton;
    [SerializeField] private Button testButton;
    [Space(10)]
    [SerializeField] private UIDragAndCollisionHandler dragHandler;
    [SerializeField] private Quiz quiz;
    [Space(10)]
    [SerializeField] private TextMeshProUGUI docsTMP;
    [SerializeField] private string[] docsTexts;
    [SerializeField] private TextMeshProUGUI popupTMP;
    [SerializeField] private string[] popupTexts;
    
    private RectTransform currentRedPinRect;
    private RectTransform currentBlackPinRect;
    private RectTransform currentRedPlugRect;
    private RectTransform currentBlackPlugRect;
    private bool eAR2WisOn = false;

    private void Awake()
    {
        eAR2WButton.onClick.AddListener(ClickEAR2WButton);
        testButton.onClick.AddListener(ClickTestButton);
        quiz.resetAction += QuizReset;
        currentRedPinRect = redPinRect;
        currentBlackPinRect = blackPinRect;
        currentRedPlugRect = redPlugRect;
        currentBlackPlugRect = blackPlugRect;
    }

    private void OnEnable()
    {
        dragHandler.OnCollisionDetected += PinDragEvent;
        EvaluationManager.Instance.ReturnEvent += AllComplete;
    }

    private void Update()
    {
        DrawLineBetweenImages(currentRedPinRect, currentRedPlugRect, redLineRect);
        DrawLineBetweenImages(currentBlackPinRect, currentBlackPlugRect, blackLineRect);
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

    private void PinDragEvent(GameObject d, GameObject t)
    {
        if (d == redPinRect.parent.gameObject)
        {
            d.SetActive(false);
            currentRedPinRect = redPinSetPlaceRect;
            redPinSetPlaceRect.parent.gameObject.SetActive(true);
        }
        else if (d == blackPinRect.parent.gameObject)
        {
            d.SetActive(false);
            currentBlackPinRect = blackPinSetPlaceRect;
            blackPinSetPlaceRect.parent.gameObject.SetActive(true);
        }
        else if (d == redPlugRect.parent.gameObject)
        {
            d.SetActive(false);
            currentRedPlugRect = plusRect;
            redPlugRect.parent.gameObject.SetActive(false);
        }
        else if (d == blackPlugRect.parent.gameObject)
        {
            d.SetActive(false);
            currentBlackPlugRect = minusRect;
            blackPlugRect.parent.gameObject.SetActive(false);
        }
    }

    private void ClickEAR2WButton()
    {
        Debug.Log("클릭");
        eAR2WisOn = true;
    }

    private void ClickTestButton()
    {
        Debug.Log("클릭");
        if (!eAR2WisOn)
            return;
        if (currentRedPinRect != redPinSetPlaceRect)
            return;
        if (currentBlackPinRect != blackPinSetPlaceRect)
            return;
        if (currentBlackPlugRect != minusRect)
            return;
        if (currentRedPlugRect != plusRect)
            return;
        Debug.Log("성공");
        numberImage.SetActive(true);
    }

    private bool AllComplete()
    {
        if (numberImage.activeSelf && !checkListImage.activeSelf)
        {
            checkListImage.SetActive(true);
            return false;
        }
        else
            return true;
    }

    private void QuizReset()
    {
        redPinRect.parent.gameObject.SetActive(true);
        blackPinRect.parent.gameObject.SetActive(true);
        redPinSetPlaceRect.parent.gameObject.SetActive(false);
        blackPinSetPlaceRect.parent.gameObject.SetActive(false);
        redPlugRect.parent.gameObject.SetActive(true);
        blackPlugRect.parent.gameObject.SetActive(true);
        numberImage.SetActive(false);
        checkListImage.SetActive(false);
        currentRedPinRect = redPinRect;
        currentBlackPinRect = blackPinRect;
        currentRedPlugRect = redPlugRect;
        currentBlackPlugRect = blackPlugRect;
        dragHandler.OnCollisionDetected -= PinDragEvent;
        EvaluationManager.Instance.ReturnEvent -= AllComplete;
    }
}
