using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class HighLight : MonoBehaviour, IPointerDownHandler
    {
        private static HighLight instance;
        public static HighLight Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HighLight();
                }
                return instance;
            }
        }

        public bool isHighlight = true;
        public Action OnHighLight;

        public float timer = 0;
        float stay_timer = 0;
        bool isTouch = true;

        List<GameObject> pool = new List<GameObject>();
        List<GameObject> list_highlight = new List<GameObject>();

        [SerializeField] GameObject obj_highlight;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        // 하이라이트 ON
        public void On<T>(List<T> t) where T : Button
        {
            for (int i = 0; i < t.Count; i++)
            {
                On(t[i]);
            }
        }

        public void On(Transform t)
        {
            if(t.gameObject.activeInHierarchy)
            {
                SetHighlight(t);
            }
        }

        public void On(Button b)
        {
            if(b.interactable && b.gameObject.activeInHierarchy)
            {
                SetHighlight(b.transform);
            }
        }

        // 하이라이트 생성
        void SetHighlight(Transform t)
        {
            GameObject obj;
            if (pool.Count > 0)
            {
                obj = pool[0];
                pool.RemoveAt(0);
            }
            else
            {
                obj = Instantiate(obj_highlight);
            }
            list_highlight.Add(obj);
            obj.transform.SetParent(t, false);
            obj.SetActive(true);
            obj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            obj.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        }

        // 하이라이트 OFF , 오브젝트 풀 반납
        void OffAll()
        {
            for(int i = 0; i < list_highlight.Count; i++)
            {
                list_highlight[i].gameObject.SetActive(false);
                pool.Add(list_highlight[i]);
            }
            list_highlight.Clear();
            isTouch = true;
        }

        // 마우스 클릭
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isHighlight) return;
            if (!isTouch) return;

            isTouch = false;
            stay_timer = 0;
            timer = 0;
            OnHighLight?.Invoke();
        }

        // 하이라이트 나타내는 시간 계산, 클릭 속도 조절
        private void Update()
        {
            if (!isHighlight) return;

            if (stay_timer > 5)
            {
                stay_timer = 0;
                isTouch = false;
                timer = 0;
                OnHighLight?.Invoke();
            }
            else
            {
                //if(stay_timer > 1)
                //{
                //    OffAll();
                //}
                stay_timer += Time.deltaTime;
            }

            if (isTouch) return;
            if (timer > 1)
            {
                timer = 0;
                OffAll();
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }
}
