using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UIDragAndCollisionHandler;

public class Transporter_DraggingObjectQuiz : Quiz
{
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    public Camera uiCamera;
    [Space(10)]
    public List<DraggablePair> draggablePairs = new List<DraggablePair>();
    public RectTransform[] draggableObjects;
    public GameObject[] targetObjects;
    public TextMeshProUGUI[] targetTexts;
    public bool isDuplicateInput = false;
    public bool selectObkectsDisable = false;
    [Space(10)]
    public Transform imageList;
    public Transform selectImage;

    private GameObject[] selectObjects;
    private Vector2[] startPostionVector2;
    private RectTransform selectedRectTransform;
    private GameObject selectedObject = null;
    private GameObject targetObject = null; // 선택된 오브젝트의 대응 충돌 대상
    private Vector2 draggableObjectVector2;
    private bool isDragging = false;
    private bool _isObjectSelected = true;
    public delegate void CollisionEventHandler(GameObject draggedObject, GameObject targetObject);
    public event CollisionEventHandler OnCollisionDetected;
    private readonly string[] ANSWERTEXT = { "제 1석유류", "제 2석유류", "I", "II", "20", "30", "화기주의", "화기엄금" };
    // Start is called before the first frame update
    void Start()
    {
        Init();
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0)) // 마우스 버튼이 눌릴 때
            .Subscribe(_ => TryPickUIObject(Input.mousePosition));

        // 드래그 중 마우스 위치에 따라 오브젝트 이동
        this.UpdateAsObservable()
            .Where(_ => isDragging) // 드래그 중일 때
            .Subscribe(_ =>
            {
                DragUIObject(Input.mousePosition);

            });

        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonUp(0) && isDragging)
            .Subscribe(_ =>
            {
                if (CheckCollision() || CheckDistance())
                {
                    //OnCollisionDetected?.Invoke(selectedObject, targetObject);
                }
                else
                {
                    EnableMaskSprite(selectedObject);
                }
                isDragging = false;
                selectedObject.transform.SetParent(imageList);
                selectedObject = null;

            });
        OnCollisionDetected += DraggingQuizEvent;

    }

    private void Init()
    {
        selectObjects = new GameObject[answer.Length];
        startPostionVector2 = new Vector2[draggableObjects.Length];
        for (int i = 0; i < startPostionVector2.Length; i++)
        {
            startPostionVector2[i] = draggableObjects[i].anchoredPosition;
        }
    }

    private void TryPickUIObject(Vector2 screenPosition)
    {
        if (!_isObjectSelected)
            return;
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        if (results.Count > 0)
        {
            foreach (RaycastResult result in results)
            {
                foreach (var pair in draggablePairs)
                {
                    if (pair.draggableObject == result.gameObject)
                    {
                        selectedObject = pair.draggableObject;
                        targetObject = pair.collisionTarget;
                        selectedObject.transform.SetParent(selectImage);
                        selectedRectTransform = selectedObject.GetComponent<RectTransform>();
                        isDragging = true;
                        return;
                    }
                }
            }
        }
    }

    private void DragUIObject(Vector2 screenPosition)
    {
        if (selectedRectTransform != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                selectedRectTransform.parent as RectTransform,
                screenPosition,
                uiCamera,
                out localPoint
            );

            selectedRectTransform.localPosition = localPoint;
        }
    }

    private void EnableMaskSprite(GameObject draggableObject)
    {
        draggableObject.GetComponent<RectTransform>().anchoredPosition = startPostionVector2[Array.IndexOf(draggableObjects, draggableObject.GetComponent<RectTransform>())];
        if (Array.IndexOf(selectObjects, draggableObject) != -1)
        {
            if(targetTexts.Length != 0)
                targetTexts[Array.IndexOf(selectObjects, draggableObject)].text = "";
            selectObjects[Array.IndexOf(selectObjects, draggableObject)] = null;
        }
    }

    // 충돌 감지: 드래그 중인 오브젝트가 충돌 대상과 겹치는지 확인
    private bool CheckCollision()
    {
        if (selectedObject == null || targetObject == null)
            return false;

        var draggedRect = selectedRectTransform.rect;
        draggedRect.position = selectedRectTransform.position;

        if (isDuplicateInput)
        {
            foreach (var target in targetObjects)
            {
                Debug.Log(target.name);
                var targetRect = target.GetComponent<RectTransform>().rect;
                targetRect.position = target.GetComponent<RectTransform>().position;

                if (!draggedRect.Overlaps(targetRect))
                    continue;
                // 충돌이 감지되면 등록된 이벤트 처리
                OnCollisionDetected?.Invoke(selectedObject, target);
                Debug.Log("충돌");
                return true;
            }
        }

        var targetRect1 = targetObject.GetComponent<RectTransform>().rect;
        targetRect1.position = targetObject.GetComponent<RectTransform>().position;
        if (!draggedRect.Overlaps(targetRect1))
            return false;
        // 충돌이 감지되면 등록된 이벤트 처리
        OnCollisionDetected?.Invoke(selectedObject, targetObject);
        Debug.Log("충돌");
        return true;
    }

    private bool CheckDistance()
    {
        float distance = Vector2.Distance(selectedObject.transform.localPosition, targetObject.transform.localPosition);
        Debug.Log(distance);
        if (distance < 10)
        {
            Debug.Log("가까운 거리");
            return true;
        }
        else
        {
            Debug.Log("먼 거리");
            return false;
        }
    }


    private void DraggingQuizEvent(GameObject d, GameObject t)
    {
        // 이미 타겟 오브젝트 위에 다른 오브젝트가 있는 경우
        if (selectObjects[Array.IndexOf(targetObjects, t)] != null)
        {
            int index = Array.IndexOf(targetObjects, t);
            GameObject resetObject = selectObjects[index];
            int index2 = Array.IndexOf(draggableObjects, resetObject.GetComponent<RectTransform>());

            if (selectObkectsDisable)
                selectObjects[index].SetActive(true);

            selectObjects[index] = null;
            if (targetTexts.Length != 0)
                targetTexts[index].text = "";
            resetObject.GetComponent<RectTransform>().anchoredPosition = startPostionVector2[index2];
        }

        if (Array.IndexOf(selectObjects, d) != -1)
        {
            Debug.Log("중복");
            if (targetTexts.Length != 0)
                targetTexts[Array.IndexOf(selectObjects, d)].text = "";
            selectObjects[Array.IndexOf(selectObjects, d)] = null;
        }

        d.GetComponent<RectTransform>().position = t.GetComponent<RectTransform>().position;
        selectObjects[Array.IndexOf(targetObjects, t)] = d;
        if(targetTexts.Length != 0)
            targetTexts[Array.IndexOf(targetObjects, t)].text = ANSWERTEXT[Array.IndexOf(draggableObjects, d.GetComponent<RectTransform>())];
        
        if(selectObkectsDisable)
            d.SetActive(false);
    }

    public override bool CheckQuestionAnswer()
    {
        for (int i = 0; i < selectObjects.Length; i++)
        {
            if (selectObjects[i] == null)
                return false;

            int selectAnswer = Array.IndexOf(draggableObjects, selectObjects[i].GetComponent<RectTransform>());

            if (answer[i] != selectAnswer)
                return false;
        }
        return true;
    }

    public override void QuizReset()
    {
        for (int i = 0; i < draggableObjects.Length; i++)
        {
            draggableObjects[i].anchoredPosition = startPostionVector2[i];
        }
        selectObjects = new GameObject[answer.Length];
    }
}
