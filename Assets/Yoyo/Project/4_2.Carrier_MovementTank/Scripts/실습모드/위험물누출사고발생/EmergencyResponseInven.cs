using UnityEngine;

namespace KKH
{
    public class EmergencyResponseInven : MonoBehaviour
    {
        [SerializeField] private EmergencyResponseType erType;

        public bool isCom = false;

        [SerializeField] private GameObject[] go_Buttons;
        [SerializeField] private GameObject go_Image;
        [SerializeField] private GameObject go_Target;

        private void OnEnable()
        {
            if (go_Image != null)
            {
                go_Image.SetActive(false);
            }
            if (go_Target != null)
            {
                go_Target.SetActive(true);
            }

            foreach (GameObject go in go_Buttons)
            {
                go.SetActive(true);
            }

            isCom = false;
        }

        public bool SetImage(EmergencyResponseType type)
        {
            if (erType == type)
            {
                isCom = true;
                if (go_Image != null)
                {
                    go_Image?.SetActive(true);
                }
                go_Target.SetActive(false);

                return true;
            }
            else
            {
                return false;
            }

        }
    }
}