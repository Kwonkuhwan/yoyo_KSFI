using RJH.DangerousGoods;
using UnityEngine;
using UnityEngine.UI;

public class ResistanceMeasurementPopup : MonoBehaviour
{
    [SerializeField] private GameObject eAR2HandObject;
    [SerializeField] private GameObject powerHandObject;
    [Space(10)]
    [SerializeField] private DocsButton eAR2WButton;
    [SerializeField] private DocsButton powerButton;
    [SerializeField] private GameObject blackPenObject;
    [SerializeField] private RectTransform blackPenEnd;
    [SerializeField] private GameObject redPenObject;
    [SerializeField] private RectTransform redPenEnd;
    [SerializeField] private GameObject numberObject;
    [SerializeField] private GameObject redPenillusion;
    [SerializeField] private GameObject blackPenillusion;
    [SerializeField] private GameObject blackPenPlug;
    [SerializeField] private RectTransform blackPenPlugRect;
    [SerializeField] private GameObject redPenPlug;
    [SerializeField] private RectTransform redPenPlugRect;
    [Space(10)]
    [SerializeField] private GameObject setPlacedBlackPen;
    [SerializeField] private RectTransform setPlacedBlackPenRect;
    [SerializeField] private GameObject setPlacedRedPen;
    [SerializeField] private RectTransform setPlacedRedPenRect;
    [Space(10)]

    [SerializeField] private RectTransform blackLine;
    [SerializeField] private RectTransform redLine;
    [Space(10)]
    [SerializeField] private UIDragAndCollisionHandler collisionHandler;
    [SerializeField] private RectTransform minusPoint;
    [SerializeField] private RectTransform plusPoint;

    private Vector3 blackPanStartPosition;
    private Vector3 redPanStartPosition;
    private RectTransform currentBlack;
    private RectTransform currentRed;
    private RectTransform currentBlackPlug;
    private RectTransform currentRedPlug;
    private bool allPlugIn = false;
    private bool allPenSet = false;

    private void Awake()
    {
        blackPanStartPosition = blackPenObject.transform.position;
        redPanStartPosition = redPenObject.transform.position;
        eAR2WButton.docsAction += EAR2WButtonClick;
        powerButton.docsAction += PowerButtonClick;
        currentBlack = blackPenEnd;
        currentRed = redPenEnd;
        currentBlackPlug = blackPenPlugRect;
        currentRedPlug = redPenPlugRect;

        ObjectStateListener listener = blackPenObject.GetComponent<ObjectStateListener>();
        listener.onEnableEvent.AddListener(() => { currentBlack = blackPenEnd; });
        listener.onDisableEvent.AddListener(() => { currentBlack = setPlacedBlackPenRect; });

        listener = redPenObject.GetComponent<ObjectStateListener>();
        listener.onEnableEvent.AddListener(() => { currentRed = redPenEnd; });
        listener.onDisableEvent.AddListener(() => { currentRed = setPlacedRedPenRect; });

        listener = blackPenPlug.GetComponent<ObjectStateListener>();
        listener.onEnableEvent.AddListener(() => { currentBlackPlug = blackPenPlugRect; });
        listener.onDisableEvent.AddListener(() => { currentBlackPlug = minusPoint; });

        listener = redPenPlug.GetComponent<ObjectStateListener>();
        listener.onEnableEvent.AddListener(() => { currentRedPlug = redPenPlugRect; });
        listener.onDisableEvent.AddListener(() => { currentRedPlug = plusPoint; });
    }

    private void Update()
    {
        DrawLineBetweenImages(currentBlack, currentBlackPlug, blackLine);
        DrawLineBetweenImages(currentRed, currentRedPlug, redLine);
    }

    public void PopupStart()
    {
        
        gameObject.SetActive(true);
    }

    private void EAR2WButtonClick()
    {
        //eAR2HandObject.SetActive(false);
        blackPenillusion.SetActive(true);
        redPenillusion.SetActive(true);
        //blackPenObject.GetComponent<Image>().raycastTarget = true;
        //redPenObject.GetComponent<Image>().raycastTarget = true;
    }

    private void PowerButtonClick()
    {
        //powerHandObject.SetActive(false);
        numberObject.SetActive(true);
    }

    private void PinHandDragEvent(GameObject d, GameObject t)
    {
        if (d.GetComponent<DocsButton>().isChecked == true)
            return;

        d.SetActive(false);


        if (d == blackPenObject)
        {
            setPlacedBlackPen.SetActive(true);
            //currentBlack = setPlacedBlackPenRect;
            blackPenillusion.SetActive(false);
            if (blackPenObject.activeSelf == false && redPenObject.activeSelf == false)
            {
                powerHandObject.SetActive(true);
                //handObject.GetComponent<RectTransform>().anchoredPosition = powerHandPosition;
            }
        }

        if (d == redPenObject)
        {
            setPlacedRedPen.SetActive(true);
            //currentRed = setPlacedRedPenRect;
            redPenillusion.SetActive(false);
            if (blackPenObject.activeSelf == false && redPenObject.activeSelf == false)
            {
                powerHandObject.SetActive(true);
                //handObject.GetComponent<RectTransform>().anchoredPosition = powerHandPosition;
            }
        }

        if (d == redPenPlug)
        {
            //currentRedPlug = plusPoint;
            if (redPenPlug.activeSelf == false && blackPenPlug.activeSelf == false)
            {
                eAR2HandObject.SetActive(true);
                //handObject.GetComponent<RectTransform>().anchoredPosition = eAR2WHandPosition;
            }
        }

        if (d == blackPenPlug)
        {
            //currentBlackPlug = minusPoint;
            if (redPenPlug.activeSelf == false && blackPenPlug.activeSelf == false)
            {
                eAR2HandObject.SetActive(true);
                //handObject.GetComponent<RectTransform>().anchoredPosition = eAR2WHandPosition;
            }
        }
        d.GetComponent<DocsButton>().OnClick();
        t.GetComponent<DocsButton>().OnClick();
    }

    private void ConfirmButtonClick()
    {
        blackPenObject.transform.position = blackPanStartPosition;
        redPenObject.transform.position = redPanStartPosition;
        gameObject.SetActive(false);
        SectionAndBackGroundManager.Instance.SetPopupData(-1);
        SectionAndBackGroundManager.Instance.SetSectionOnOff(true);
    }

    private void OnEnable()
    {
        collisionHandler.OnCollisionDetected += PinHandDragEvent;
    }

    private void OnDisable()
    {
        collisionHandler.OnCollisionDetected -= PinHandDragEvent;
        ResetPopup();
    }

    private void ResetPopup()
    {
        gameObject.SetActive(false);
        eAR2HandObject.SetActive(false);
        powerHandObject.SetActive(false);
        //blackPenObject.GetComponent<Image>().raycastTarget = false;
        //blackPenObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        //redPenObject.GetComponent<Image>().raycastTarget = false;
        //redPenObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        numberObject.SetActive(false);
        setPlacedBlackPen.SetActive(false);
        setPlacedRedPen.SetActive(false);
        redPenillusion.SetActive(false);
        blackPenillusion.SetActive(false);
        blackPenPlug.SetActive(true);
        redPenPlug.SetActive(true);
        blackPenObject.SetActive(true);
        redPenObject.SetActive(true);
        //currentBlack = blackPenEnd;
        //currentRed = redPenEnd;
        //currentBlackPlug = blackPenPlugRect;
        //currentRedPlug = redPenPlugRect;
        collisionHandler.ResetEvent();
    }

    public void DrawLineBetweenImages(RectTransform image1, RectTransform image2, RectTransform lineRect, float lineWidth = 10f)
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
