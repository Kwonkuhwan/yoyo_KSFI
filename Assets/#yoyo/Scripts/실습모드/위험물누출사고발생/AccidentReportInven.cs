using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KKH
{
    public class AccidentReportInven : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private bool isDragging = false;
        private Vector3 oldPoint;

        public AccidentReport ar = AccidentReport.None;

        [SerializeField] private bool isTrueBox = false;
        [SerializeField] private Image image;
        [SerializeField] private Sprite sprite_default;
        [SerializeField] private Sprite[] sprites;

        [SerializeField] private AccidentReportPopup accidentReportPopup;

        [SerializeField] private Transform tr_Panel;
        [SerializeField] private Transform tr_DragParent;

        private void Awake()
        {
            image = GetComponent<Image>();
            oldPoint = transform.position;
        }

        private void OnEnable()
        {
            ar = AccidentReport.None;
            image.sprite = sprite_default;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true; // 마우스 클릭 시작
            transform.SetParent(tr_DragParent);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false; // 마우스 클릭 종료
            transform.SetParent(tr_Panel);
            transform.position = oldPoint;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging)
            {
                // 마우스의 위치를 RectTransform으로 변환
                RectTransform rectTransform = GetComponent<RectTransform>();
                Vector2 localPoint;
                // 현재 마우스 위치를 RectTransform으로 변환
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out localPoint);
                rectTransform.anchoredPosition = localPoint; // 위치 업데이트
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            try
            {
                // 다른 이미지와의 충돌 처리
                Collider2D[] colliders = Physics2D.OverlapBoxAll(GetComponentInChildren<Collider2D>().bounds.center, GetComponentInChildren<Collider2D>().bounds.size, 0);
                foreach (var collider in colliders)
                {
                    if (collider.gameObject != gameObject)
                    {
                        ButtonEnable inven = collider.GetComponent<ButtonEnable>();
                        if (inven != null)
                        {
                            inven.EnableButton((int)ar);
                            image.sprite = sprite_default;
                            transform.SetParent(tr_Panel);
                            transform.position = oldPoint;
                            continue;
                        }

                        AccidentReportInven ari = collider.GetComponent<AccidentReportInven>();
                        if (ari != null)
                        {
                            AccidentReport temp = ari.ar;
                            if (ari.ShowImage(ar, true))
                            {
                                ShowImage(temp, true);
                                transform.SetParent(tr_Panel);
                                transform.position = oldPoint;
                            }
                            break;
                        }
                    }
                }
            }
            catch
            {
                transform.SetParent(tr_Panel);
                transform.position = oldPoint;
            }
        }

        public bool ShowImage(AccidentReport inar, bool isInvenToInven = false)
        {
            if (ar != AccidentReport.None && !isInvenToInven)
            {
                accidentReportPopup.ResetMoveImage(ar);
            }

            if (inar != AccidentReport.None)
            {
                image.sprite = sprites[(int)inar];
            }
            else
            {
                image.sprite = sprite_default;
            }

            ar = inar;

            return true;
        }
    }
}
