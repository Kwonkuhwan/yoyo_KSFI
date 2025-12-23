using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace KKH
{
    public class EmergencyResponsDrawLine_Evaluation : MonoBehaviour
    {
        public int quizNum = 0;
        public int[] answer;

        public GraphicRaycaster raycaster;
        public EventSystem eventSystem;
        public Camera uiCamera;
        [Space(10)]
        public GameObject[] startPoint;
        public GameObject[] endPoint;
        [Space(10)]
        public GameObject lineParent;
        public GameObject lineObject;

        public int[] connected;
        public GameObject[] connectedLine;
        private GameObject drawline;
        private bool isStartPoint = false;
        private Vector2 startPosition;
        private Vector2 endPosition;
        private int startIndex = 0;
        private int endIndex = 0;
        private RectTransform drawlineRect;
        private bool isDragging = false;

        private void Start()
        {
            connected = new int[startPoint.Length];
            connectedLine = new GameObject[startPoint.Length];

            for (int i = 0; i < connectedLine.Length; i++)
            {
                connected[i] = -1;
            }

            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonDown(0)) // 마우스 버튼이 눌릴 때
                .Subscribe(_ => TrySelectStartPoint(Input.mousePosition));

            this.UpdateAsObservable()
                .Where(_ => isDragging) // 드래그 중일 때
                .Subscribe(_ =>
                {
                    DrawLineObject(Input.mousePosition);

                });

            this.UpdateAsObservable()
               .Where(_ => Input.GetMouseButtonUp(0) && isDragging)
               .Subscribe(_ =>
               {
                   TryFinishDawLine(Input.mousePosition);

               });
        }

        private void Update()
        {
            if (CheckQuestionAnswer())
            {
                EvaluationManager.Inst.EmergencyResponsPoint[quizNum - 1] = true;
            }
            else
            {
                EvaluationManager.Inst.EmergencyResponsPoint[quizNum - 1] = false;
            }
        }

        private void TrySelectStartPoint(Vector2 screenPosition)
        {
            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = screenPosition;
            //Debug.Log(screenPosition);
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            if (results.Count > 0)
            {
                foreach (RaycastResult result in results)
                {
                    int i = 0;
                    foreach (var start in startPoint)
                    {
                        if (start == result.gameObject)
                        {
                            // TODO 시작 지점에 이미 연결중인 선이 있는가?
                            if (connected[i] != -1)
                            {
                                Destroy(connectedLine[i]);
                                connected[i] = -1;
                            }
                            drawline = Instantiate(lineObject, lineParent.transform);
                            drawlineRect = drawline.GetComponent<RectTransform>();
                            drawlineRect.anchoredPosition = start.GetComponent<RectTransform>().anchoredPosition;
                            startIndex = i;
                            startPosition = drawlineRect.anchoredPosition;

                            //Debug.Log("Start" + startPosition);
                            isDragging = true;
                            isStartPoint = true;
                            return;
                        }
                        ++i;
                    }
                    i = 0;
                    foreach (var end in endPoint)
                    {
                        if (end == result.gameObject)
                        {
                            // TODO 끝 지점에 이미 연결중인 선이 있는가?
                            if (connected.Contains(i))
                            {
                                Destroy(connectedLine[Array.IndexOf(connected, i)]);
                                connected[Array.IndexOf(connected, i)] = -1;
                            }
                            drawline = Instantiate(lineObject, lineParent.transform);
                            drawlineRect = drawline.GetComponent<RectTransform>();
                            drawlineRect.anchoredPosition = end.GetComponent<RectTransform>().anchoredPosition;
                            endIndex = i;
                            startPosition = drawlineRect.anchoredPosition;

                            //Debug.Log("Start" + startPosition);
                            isDragging = true;
                            isStartPoint = false;
                            return;
                        }
                        ++i;
                    }
                }
            }
        }

        private void DrawLineObject(Vector2 screenPosition)
        {
            if (drawline == null)
                return;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    drawlineRect.parent as RectTransform,
                    screenPosition,
                    uiCamera,
                    out localPoint
                );

            Vector2 direction = (localPoint - startPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            float distance = Vector2.Distance(startPosition, localPoint);
            Vector2 newSize = drawlineRect.sizeDelta;
            newSize.x = distance;
            drawlineRect.sizeDelta = newSize;
            drawlineRect.localEulerAngles = new Vector3(0, 0, angle);
        }


        private void TryFinishDawLine(Vector2 screenPosition)
        {
            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = screenPosition;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            if (results.Count > 0)
            {
                foreach (RaycastResult result in results)
                {
                    if (isStartPoint)
                    {
                        int i = 0;
                        foreach (var end in endPoint)
                        {
                            if (end == result.gameObject)
                            {
                                if (connected.Contains(i))
                                {
                                    int index = Array.IndexOf(connected, i);
                                    Destroy(connectedLine[index]);
                                    connected[index] = -1;
                                }

                                RectTransform endRect = end.GetComponent<RectTransform>();
                                Vector2 direction = (endRect.anchoredPosition - startPosition).normalized;
                                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                                float distance = Vector2.Distance(startPosition, endRect.anchoredPosition);
                                Vector2 newSize = drawlineRect.sizeDelta;
                                newSize.x = distance;
                                drawlineRect.sizeDelta = newSize;
                                drawlineRect.localEulerAngles = new Vector3(0, 0, angle);

                                endIndex = i;
                                connected[startIndex] = endIndex;
                                connectedLine[startIndex] = drawline;
                                drawline = null;
                                drawlineRect = null;
                                startPosition = new Vector2();
                                isDragging = false;
                                return;
                            }
                            ++i;
                        }
                    }
                    else
                    {
                        int i = 0;
                        foreach (var start in startPoint)
                        {
                            if (start == result.gameObject)
                            {
                                if (connected[i] != -1)
                                {
                                    Destroy(connectedLine[i]);
                                    connected[i] = -1;
                                }

                                RectTransform startRect = start.GetComponent<RectTransform>();
                                Vector2 direction = (startRect.anchoredPosition - startPosition).normalized;
                                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                                float distance = Vector2.Distance(startPosition, startRect.anchoredPosition);
                                Vector2 newSize = drawlineRect.sizeDelta;
                                newSize.x = distance;
                                drawlineRect.sizeDelta = newSize;
                                drawlineRect.localEulerAngles = new Vector3(0, 0, angle);

                                startIndex = i;
                                connected[startIndex] = endIndex;
                                connectedLine[startIndex] = drawline;
                                drawline = null;
                                drawlineRect = null;
                                startPosition = new Vector2();
                                isDragging = false;
                                return;
                            }
                            ++i;
                        }
                    }

                    Destroy(drawline);
                    drawline = null;
                    drawlineRect = null;
                    startPosition = new Vector2();
                    isDragging = false;
                }
            }
        }

        //public void QuizReset()
        //{
        //    for (int i = 0; i < connected.Length; i++)
        //    {
        //        connected[i] = -1;
        //        if (connectedLine[i] != null)
        //            Destroy(connectedLine[i]);
        //    }
        //}

        public bool CheckQuestionAnswer()
        {
            for (int i = 0; i < connected.Length; i++)
            {
                if (connected[i] != answer[i])
                    return false;
            }
            return true;
        }
    }
}
