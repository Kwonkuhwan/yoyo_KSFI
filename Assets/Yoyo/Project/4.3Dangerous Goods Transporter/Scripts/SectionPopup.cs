using UnityEngine;

namespace RJH.Transporter
{
    public class SectionPopup : MonoBehaviour
    {
        [SerializeField] private int popupNumber;
        private void OnEnable()
        {
            PopupManager.Instance.NormalSetPopup(popupNumber);
        }

        private void OnDisable()
        {
            PopupManager.Instance.PopupClose();
        }
    }
}
