using UnityEngine;
using UnityEngine.UI;

namespace KKH {
    public class TruckMove : MonoBehaviour
    {
        [SerializeField] private Vector3[] pos_Truck;
        [SerializeField] private Vector3[] scale_Truck;
        [SerializeField] private Sprite[] sprites_Truck;
        [SerializeField] private Image image_Truck;
        [SerializeField] private int truckIdx = 0;
        [SerializeField] private int maxTruckIdx = 3;

        [SerializeField] private float fMaxCoolTime = 0.5f;
        [SerializeField] private float fCoolTime = 0.0f;

        [SerializeField] private Animation anim_Truck;

        [SerializeField] private InfoMenu infoMenu;

        [SerializeField] private bool isAnimation = false;

        private void Awake()
        {
            anim_Truck = GetComponent<Animation>();
        }

        private void OnEnable()
        {
            truckIdx = 0;
            isAnimation = false;
            SetTruckImage();
        }

        private void Update()
        {
            //if (truckIdx < maxTruckIdx)
            //{
            //    fCoolTime += Time.deltaTime;

            //    if (fMaxCoolTime < fCoolTime)
            //    {
            //        fCoolTime = 0;
            //        SetTruckImage();
            //    }
            //}
            //if (!isAnimation)
            //{
            //    SetTruckImage();
            //}
            //else
            //{
            //    if (infoMenu != null)
            //    {
            //        infoMenu.NextBtnClick();
            //    }
            //}
        }

        private void SetTruckImage()
        {
            image_Truck.transform.localPosition = pos_Truck[truckIdx];
            image_Truck.transform.localScale = scale_Truck[truckIdx];

            anim_Truck.Play();

            isAnimation = true;
            //try
            //{
            //    if (sprites_Truck[truckIdx] != null)
            //    {
            //        image_Truck.sprite = sprites_Truck[truckIdx];
            //    }
            //}
            //catch { }
            //image_Truck.gameObject.SetActive(true);

            //truckIdx++;
        }
    }
}