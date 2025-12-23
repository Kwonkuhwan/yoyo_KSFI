using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public enum OBJECT
    {
        열감지기 = 0,
        연기감지기,
        수동조작함
    }

    public class FireScene : MonoBehaviour
    {
        [Header("팝업")]
        [SerializeField] Transform Group_Popup;
        List<GameObject> list_popup = new List<GameObject>();

        [Header("수동조작함")]
        [SerializeField] Button OpenManualBox;

        void Awake()
        {
            HighLight.Instance.OnHighLight += OnHighLight;
            OpenManualBox.onClick.AddListener(() =>
            {
                OpenPopup(OBJECT.수동조작함);
            });
        }

        void OnHighLight()
        {
            if(!list_popup[(int)OBJECT.수동조작함].activeSelf)
            {
                HighLight.Instance.On(OpenManualBox);
            }
        }

        public void Setting()
        {
            SettingButtons();
        }

        public void OpenPopup(OBJECT index)
        {
            for(int i = 0; i < list_popup.Count; i++)
            {
                if(i == (int)index)
                {
                    list_popup[i].gameObject.SetActive(true);
                }
                else
                {
                    list_popup[i].gameObject.SetActive(false);
                }
            }
        }

        public void MultiOpen(OBJECT index)
        {
            list_popup[(int)index].gameObject.SetActive(true);
        }

        public void CloseAllPopup()
        {
            for(int i = 0; i < list_popup.Count;i++)
            {
                list_popup[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 수동조작함 인터렉터블 활성화/비활성화
        /// </summary>
        public void InteractiveManulBoxOpen(bool isActive)
        {
            OpenManualBox.interactable = isActive;
        }

        /// <summary>
        /// 버튼 세팅
        /// </summary>
        void SettingButtons()
        {
            foreach(Transform t in Group_Popup)
            {
                list_popup.Add(t.gameObject);
            }
        }
    }
}
