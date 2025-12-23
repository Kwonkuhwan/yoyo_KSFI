using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace KKH
{
    public class DeceleratingPopup : MonoBehaviour
    {
        [SerializeField] private Button[] btns;
        [SerializeField] private GameObject[] outlines;
        public int SelectNum = 0;
        public int count = 0;
        [SerializeField] private GameObject go_Error;

        private void Awake()
        {
            for (int i = 0; i < btns.Length; i++)
            {
                int idx = i;

                btns[idx].onClick.AddListener(delegate
                {
                    AllHideOutline();
                    outlines[idx].SetActive(true);
                    SelectNum = idx + 1;
                });
            }
        }

        private void OnEnable()
        {
            if (go_Error != null && go_Error.activeInHierarchy)
            {
                go_Error.SetActive(false);
            }
            count = 0;
            AllHideOutline();
            SelectNum = 0;
        }

        private void AllHideOutline()
        {
            foreach (var obj in outlines)
            {
                obj.SetActive(false);
            }
        }

        public bool IncreaseErrorCount()
        {
            if (count < 3)
            {
                count++;
                go_Error.SetActive(true);
            }

            if (count == 3)
            {
                AllHideOutline();
                SelectNum = 1;
                outlines[0].SetActive(true);
                return true;
            }
            else
            {
                return true;
            }
        }
    }
}
