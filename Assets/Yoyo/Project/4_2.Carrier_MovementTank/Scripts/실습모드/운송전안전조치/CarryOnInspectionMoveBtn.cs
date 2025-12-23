using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class CarryOnInspectionMoveBtn : MonoBehaviour
    {
        [SerializeField] private Image image_Carry;
        [SerializeField] private CarryOnInspection COI;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => ShowCarryOnInspection());
        }

        public void ShowCarryOnInspection()
        {
            //image_Carry.GetComponent<CarryOnInspectionMoveImage>().COI = COI;
            image_Carry.gameObject.SetActive(true);
        }
    }
}
