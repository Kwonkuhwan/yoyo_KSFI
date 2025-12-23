using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class Question_Select : MonoBehaviour
    {
        [SerializeField] List<Toggle> list_toggle = new List<Toggle>();

        private void Awake()
        {
            for(int i = 0; i < list_toggle.Count; i++)
            {
                int index = i;
                list_toggle[index].onValueChanged.AddListener(ChanageValue);
            }
        }

        void ChanageValue(bool isOn)
        {

        }
    }
}
