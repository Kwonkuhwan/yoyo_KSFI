using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public enum SWITCH
    {
        주경종,
        지구경종,
        사이렌,
        비상방송,
        부저,
        축적비축적,
        예비전원,
        유도등,
        도통시험,
        동작시험,
        자동복구,
        복구
    }
    public class SwichButtonNaming : MonoBehaviour
    {
        public List<SwitchButton> list_switch = new List<SwitchButton>();

        [SerializeField] TextAsset Switch_Data;

        /// <summary>
        /// 스위치 버튼 세팅
        /// </summary>
        public void Setting()
        {
            foreach (Transform trs in transform)
            {
                list_switch.Add(trs.GetComponent<SwitchButton>());
            }

            Data d = new Data();
            d = JsonUtility.FromJson<Data>(Switch_Data.ToString());

            for (int i = 0; i < list_switch.Count; i++)
            {
                int index = i;
                list_switch[i].Set(d.name[i], index);
            }
        }

        class Data
        {
            public string[] name;
        }
    }
}
