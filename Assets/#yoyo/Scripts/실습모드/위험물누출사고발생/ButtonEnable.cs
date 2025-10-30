using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public class ButtonEnable : MonoBehaviour
    {
        [SerializeField] private GameObject[] gos;

        private void OnEnable()
        {
            foreach(var go in gos)
            {
                go.SetActive(true);
            }
        }

        public void EnableButton(int num)
        {
            gos[num].SetActive(true);
        }
    }
}
