using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class LogManager : MonoBehaviour
    {
        private static LogManager instance;
        public static LogManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LogManager();
                }
                return instance;
            }
        }

        [SerializeField] private Button menuBtn;
        [SerializeField] private Button upBtn;
        [SerializeField] private Button downBtn;
        [SerializeField] private Button selectBtn;
        [SerializeField] private TextMeshProUGUI logText;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Transform logTextParent;
        private static readonly StringBuilder ReceiverLogList = new StringBuilder();

        public GameObject logTextObj;

        List<GameObject> pool = new List<GameObject>();
        List<GameObject> list_log = new List<GameObject>();

        public float lineHeight;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            Init();
        }

        public void Init()
        {
            upBtn.onClick.RemoveAllListeners();
            downBtn.onClick.RemoveAllListeners();
            selectBtn.onClick.RemoveAllListeners();

            upBtn.onClick.AddListener(ScrollUp);
            downBtn.onClick.AddListener(ScrollDown);
            selectBtn.onClick.AddListener(delegate
            {
                //
            });
        }

        public void SetLog(string log)
        {
            ReceiverLogList.AppendLine($"<color=black>{log}</color>");

            GameObject logObject = null;
            if(pool.Count > 0)
            {
                logObject = pool[0];
                logObject.SetActive(true);
                pool.RemoveAt(0);
            }
            else
            {
                if(logTextObj != null)
                {
                    logObject = Instantiate(logTextObj, logTextParent);
                }
            }
            if (logTextObj != null)
            {
                logObject.GetComponent<LogText>().SetText($"<color=black>{log}</color>");
            }
            list_log.Add(logObject);
        }

        public void Reset()
        {
            for (int i = 0; i < list_log.Count; i++)
            {
                list_log[i].SetActive(false);
                pool.Add(list_log[i]);
            }
            list_log.Clear();
        }

        private void ScrollUp()
        {
            float scrollableHeight = (logTextParent.childCount * lineHeight) - scrollRect.viewport.rect.height;
            float newYPosition = scrollRect.verticalNormalizedPosition + (lineHeight / scrollableHeight);
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newYPosition);
        }

        private void ScrollDown()
        {
            float scrollableHeight = (logTextParent.childCount * lineHeight) - scrollRect.viewport.rect.height;
            float newYPosition = scrollRect.verticalNormalizedPosition - (lineHeight / scrollableHeight);
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newYPosition);
        }
    }
}
