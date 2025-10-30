using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class ImageRotate : MonoBehaviour
    {
        [SerializeField] private int cnt = 0;
        [SerializeField] private GameObject go_VoltageImage;
        [SerializeField] private ButtonManager_KKH buttonControl_KKH;

        private void Awake()
        {
            buttonControl_KKH = GetComponent<ButtonManager_KKH>();
            GetComponent<Button>().onClick.AddListener(() => RotateBtnClick());
        }

        private void OnEnable()
        {
            go_VoltageImage.SetActive(false);
            cnt = 0;
            transform.rotation = Quaternion.EulerRotation(Vector3.zero);
        }

        private void RotateBtnClick()
        {
            if (cnt >= 1) return;

            buttonControl_KKH.isCompelet = true;
            go_VoltageImage.SetActive(true);

            // 90도 회전
            transform.Rotate(0, 0, -27 * 4);
            cnt++;
        }
    }
}