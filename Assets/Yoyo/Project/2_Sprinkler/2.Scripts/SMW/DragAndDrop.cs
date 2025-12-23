using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class DragAndDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public enum 이름
        {
            열감지기,
            연기감지기
        }

        [SerializeField] Camera cam;
        [SerializeField] GameObject Inventory;
        [SerializeField] GameObject DetectObject;
        [SerializeField] GameObject Drag_On;
        [SerializeField] FireScene FireScene;
        [SerializeField] WorldCanvasZoom WorldCanvasZoom;

        public Vector3 DefaultPos;

        RectTransform rt;
        Vector2 vt;

        public 이름 name;

        public bool IsDetect
        {
            get { return isDetect; }
        }
        bool isDetect;
        bool isDrag = false;

        private void Awake()
        {
            rt = transform.parent.GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            HighLight.Instance.OnHighLight += OnHighLight;
        }

        private void OnDisable()
        {
            HighLight.Instance.OnHighLight -= OnHighLight;
        }

        void OnHighLight()
        {
            if (!isDetect)
            {
                HighLight.Instance.On(transform.GetComponent<Button>());
            }
        }
        
        public void SetInventoryItem()
        {
            Reset();
            Inventory.SetActive(true);
            Inventory.GetComponent<Mask>().enabled = true;
        }

        public void Reset()
        {
            Interactive(false);
            isDetect = false;
            //DetectObject.transform.GetChild(0).gameObject.SetActive(false);
            transform.localPosition = DefaultPos;
            Inventory.SetActive(false);
            if (Drag_On != null)
            {
                Drag_On.SetActive(false);
            }
        }

        public void Interactive(bool isActive)
        {
            transform.GetComponent<Button>().interactable = isActive;
            transform.GetComponent<Image>().raycastTarget = isActive;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(Drag_On != null)
            {
                Drag_On.SetActive(true);
            }
            Inventory.GetComponent<Mask>().enabled = false;
            WorldCanvasZoom.SetItemDragBoolean(true);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (isDetect) return;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, cam, out vt))
            {
                transform.localPosition = vt;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isDetect == false)
            {
                Inventory.GetComponent<Mask>().enabled = true;
                transform.localPosition = DefaultPos;
                if (Drag_On != null)
                {
                    Drag_On.SetActive(false);
                }
            }
            WorldCanvasZoom.SetItemDragBoolean(false);
        }

        void Detect()
        {
            isDetect = true;
            transform.localPosition = DefaultPos;
            Inventory.GetComponent<Mask>().enabled = true;
            if (Drag_On != null)
            {
                Drag_On.SetActive(false);
            }
            Interactive(false);

            switch (name)
            {
                case 이름.열감지기:
                    FireScene.MultiOpen(OBJECT.열감지기);
                    break;
                case 이름.연기감지기:
                    FireScene.MultiOpen(OBJECT.연기감지기);
                    break;
            }
            ScenarioManager.Instance.CheckScenarioStep();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.gameObject == DetectObject)
            {
                Detect();
            }
        }
    }
}
