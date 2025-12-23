using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMW.Sprinkler
{
    public enum 파워
    {
        화재,
        교류전원,
        예비전원,
        발신기,
        스위치주의,
        선로단선,
        전압지시
    }

    public class PowerNaming : MonoBehaviour
    {
        public List<Power> list_power = new List<Power>();

        [SerializeField] TextAsset Power_Data;

        /// <summary>
        /// 전원 초기세팅 (이름)
        /// </summary>
        public void Setting()
        {
            foreach (Transform trs in transform)
            {
                list_power.Add(trs.GetComponent<Power>());
            }

            Data d = new Data();
            d = JsonUtility.FromJson<Data>(Power_Data.ToString());

            for (int i = 0; i < list_power.Count; i++)
            {
                list_power[i].Set(d.name[i]);
            }
        }

        class Data
        {
            public string[] name;
        }
    }
}
