using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMW.Sprinkler
{
    public class DisplayNaming : MonoBehaviour
    {
        public List<Display> list_display = new List<Display>();

        [SerializeField] TextAsset Display_Data;

        /// <summary>
        /// 디스플레이의 초기세팅 (인덱스, 장소, 이름)
        /// </summary>
        public void Setting()
        {
            foreach (Transform trs in transform)
            {
                list_display.Add(trs.GetComponent<Display>());
            }

            Data d = new Data();
            d = JsonUtility.FromJson<Data>(Display_Data.ToString());

            for (int i = 0; i < list_display.Count; i++)
            {
                list_display[i].Set(d.index[i], d.area[i], d.name[i]);
            }
        }

        class Data
        {
            public string[] index;
            public string[] area;
            public string[] name;
        }
    }
}
