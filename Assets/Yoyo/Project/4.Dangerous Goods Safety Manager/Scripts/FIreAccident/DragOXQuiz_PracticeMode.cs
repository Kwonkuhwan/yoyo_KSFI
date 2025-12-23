using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UIDragAndCollisionHandler;

public class DragOXQuiz_PracticeMode : MonoBehaviour
{
    public int[] answers;
    [Space(10)]
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    public Camera uiCamera;
    public GameObject wrongPopup;
    [Space(10)]
    public List<DraggablePair> draggablePairs = new List<DraggablePair>();
    public RectTransform[] draggableObjects;
    public GameObject[] targetObjects;
    public bool isDuplicateInput = false;
    [Space(10)]
    public Transform imageList;
    public Transform selectImage;
    public GameObject[] selectObjects;
    private Vector2[] startPostionVector2;
    private RectTransform selectedRectTransform;
    private GameObject selectedObject = null;
    private GameObject targetObject = null; // 선택된 오브젝트의 대응 충돌 대상
    private Vector2 draggableObjectVector2;
    private bool isDragging = false;
    private bool _isObjectSelected = true;
    public delegate void CollisionEventHandler(GameObject draggedObject, GameObject targetObject);
    public event CollisionEventHandler OnCollisionDetected;
    [Space(10)]
    public RectTransform[] correctRects;
    public RectTransform[] wrongRects;
    private int wrongCount = 0;

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
                if (CheckCollision())
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
        selectObjects = new GameObject[targetObjects.Length];
        startPostionVector2 = new Vector2[draggableObjects.Length];
        for (int i = 0; i < startPostionVector2.Length; i++)
        {
            startPostionVector2[i] = draggableObjects[i].anchoredPosition;
        }
    }

    private void OpenPopup()
    {
        //SectionAndBackGroundManager.Instance.SetDocument_text(titleText, descriptionText);
        //AudioManager.Instance.PlayDocs(audioClip);
    }

    // 두 Rect의 겹치는 영역을 Rect로 반환
    private Rect GetOverlapRect(Rect rect1, Rect rect2)
    {
        float xMin = Mathf.Max(rect1.xMin, rect2.xMin);
        float yMin = Mathf.Max(rect1.yMin, rect2.yMin);
        float xMax = Mathf.Min(rect1.xMax, rect2.xMax);
        float yMax = Mathf.Min(rect1.yMax, rect2.yMax);

        if (xMin < xMax && yMin < yMax)
        {
            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }
        else
        {
            return Rect.zero; // 겹치지 않는 경우
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
                Rect overlapRect = GetOverlapRect(draggedRect, targetRect);
                float overlapArea = overlapRect.width * overlapRect.height;

                float targetArea = targetRect.width * targetRect.height;

                float percentage = (overlapArea / targetArea) * 100f;
                Debug.Log(percentage + "%");
                if (percentage < 20)
                    continue;

                // 충돌이 감지되면 등록된 이벤트 처리
                OnCollisionDetected?.Invoke(selectedObject, target);
                Debug.Log("충돌");
                return true;
            }

            //foreach (var target in targetObjects)
            //{
            //    Debug.Log(target.name);
            //    var targetRect = target.GetComponent<RectTransform>().rect;
            //    targetRect.position = target.GetComponent<RectTransform>().position;

            //    if (!draggedRect.Overlaps(targetRect))
            //        continue;

            //    // 충돌이 감지되면 등록된 이벤트 처리
            //    OnCollisionDetected?.Invoke(selectedObject, target);
            //    Debug.Log("충돌");
            //    return true;
            //}
        }
        else
        {
            var targetRect1 = targetObject.GetComponent<RectTransform>().rect;
            targetRect1.position = targetObject.GetComponent<RectTransform>().position;
            if (!draggedRect.Overlaps(targetRect1))
                return false;
            // 충돌이 감지되면 등록된 이벤트 처리
            OnCollisionDetected?.Invoke(selectedObject, targetObject);
            Debug.Log("충돌");
            return true;
        }
        return false;
        //var targetRect1 = targetObject.GetComponent<RectTransform>().rect;
        //targetRect1.position = targetObject.GetComponent<RectTransform>().position;
        //if (!draggedRect.Overlaps(targetRect1))
        //    return false;
        //// 충돌이 감지되면 등록된 이벤트 처리
        //OnCollisionDetected?.Invoke(selectedObject, targetObject);
        //Debug.Log("충돌");
        //return true;
    }

    private bool CheckDistance()
    {
        float distance = Vector2.Distance(selectedObject.transform.localPosition, targetObject.transform.localPosition);
        Debug.Log(distance);
        if (distance < 5)
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
            Debug.Log(index);
            Debug.Log(index2);
            if(Array.IndexOf(selectObjects, d) == -1)
            {
                selectObjects[index] = null;
                resetObject.GetComponent<RectTransform>().anchoredPosition = startPostionVector2[index2];
            }
            else
            {
                int index3 = Array.IndexOf(selectObjects, d);
                selectObjects[index] = null;
                resetObject.GetComponent<RectTransform>().position = targetObjects[index3].GetComponent<RectTransform>().position;
                selectObjects[index3] = resetObject;
            }
            
        }

        if (Array.IndexOf(selectObjects, d) != -1)
        {
            Debug.Log("중복");
            selectObjects[Array.IndexOf(selectObjects, d)] = null;
        }

        d.GetComponent<RectTransform>().position = t.GetComponent<RectTransform>().position;
        selectObjects[Array.IndexOf(targetObjects, t)] = d;

    }

    public bool CheckQuestionAnswer()
    {
        for (int i = 0; i < draggableObjects.Length; i++)
        {
            if (Array.IndexOf(selectObjects, draggableObjects[i].gameObject) == -1)
            {
                if (wrongPopup != null)
                    StartCoroutine(PopupUpDown());
                wrongCount++;
                if(wrongCount >= 3)
                    AutoSolve();
                return false;
            }
            int number = Array.IndexOf(selectObjects, draggableObjects[i].gameObject);
            int value = number < 4 ? 1 : 0;

            if (answers[i] != value)
            {
                if (wrongPopup != null)
                    StartCoroutine(PopupUpDown());
                wrongCount++;
                if (wrongCount >= 3)
                    AutoSolve();
                return false;
            }
        }
        //gameObject.SetActive(false);
        return true;
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);

        if (RJH.DangerousGoods.SectionAndBackGroundManager.Instance != null)
        {
            RJH.DangerousGoods.SectionAndBackGroundManager.Instance.ReturnEvent -= CheckQuestionAnswer;
        }
        else if(RJH.Transporter.SectionAndBackGroundManager.Instance != null)
        {
            RJH.Transporter.SectionAndBackGroundManager.Instance.ReturnEvent -= CheckQuestionAnswer;
        }
        wrongPopup.SetActive(false);
        QuizReset();
        wrongCount = 0;
    }

    public void QuizReset()
    {
        //wrongCount = 0;
        for (int i = 0; i < draggableObjects.Length; i++)
        {
            draggableObjects[i].anchoredPosition = startPostionVector2[i];
        }
        selectObjects = new GameObject[targetObjects.Length];
    }

    private void OnEnable()
    {
        OpenPopup();
        if (RJH.DangerousGoods.SectionAndBackGroundManager.Instance != null)
        {
            RJH.DangerousGoods.SectionAndBackGroundManager.Instance.ReturnEvent += CheckQuestionAnswer;
        }
        else if (RJH.Transporter.SectionAndBackGroundManager.Instance != null)
        {
            RJH.Transporter.SectionAndBackGroundManager.Instance.ReturnEvent += CheckQuestionAnswer;
        }
    }

    private void AutoSolve()
    {
        QuizReset();
        int correctindex = 0;
        int wrongindex = 0; 
        for (int i = 0; i < answers.Length; i++)
        {
            if (answers[i] == 1)
            {
                draggableObjects[i].position = correctRects[correctindex].position;
                selectObjects[correctindex] = draggableObjects[i].gameObject;
                correctindex++;
            }
            else if (answers[i] == 0)
            {
                draggableObjects[i].position = wrongRects[wrongindex].position;
                selectObjects[wrongindex+4] = draggableObjects[i].gameObject;
                wrongindex++;
            }
        }
    }

    private IEnumerator PopupUpDown()
    {
        wrongPopup.SetActive(true);

        yield return new WaitForSeconds(1);

        wrongPopup.SetActive(false);
    }
}
