using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMW.Sprinkler
{
    public class OperatingTool : MonoBehaviour
    {
        public List<Tool> List_Tool = new List<Tool>();

        public void Setting()
        {
            foreach (Transform t in transform)
            {
                List_Tool.Add(t.GetComponent<Tool>());
                t.GetComponent<Tool>().Setting();
            }

            for(int i = 0; i < List_Tool.Count; i++)
            {
                int index = i;
                List_Tool[i].SetName(index);
            }
        }
    }
}
