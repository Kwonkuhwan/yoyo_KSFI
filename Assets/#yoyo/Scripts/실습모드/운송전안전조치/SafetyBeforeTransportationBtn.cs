using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class SafetyBeforeTransportationBtn : MonoBehaviour
    {
        [SerializeField] private GameObject go_SetImage;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => SetImage());
        }

        public bool SetImage()
        {
            if (go_SetImage != null)
            {
                go_SetImage.SetActive(true);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}