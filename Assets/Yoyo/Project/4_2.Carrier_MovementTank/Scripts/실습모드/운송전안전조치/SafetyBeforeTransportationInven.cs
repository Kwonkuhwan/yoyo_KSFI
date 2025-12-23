using UnityEngine;

namespace KKH
{
    public class SafetyBeforeTransportationInven : MonoBehaviour
    {
        [SerializeField] private SafetyBeforeTransportationType sbttype;

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
            if(go_Target != null)
            {
                go_Target.SetActive(true);
            }

            foreach (GameObject go in go_Buttons)
            {
                go.SetActive(true);
            }

            isCom = false;
        }

        public bool SetImage(SafetyBeforeTransportationType type)
        {
            if (sbttype == type)
            {
                isCom = true;
                if (go_Image != null)
                {
                    go_Image?.SetActive(true);
                }

                go_Target.SetActive(false);

                //if (sbttype == SafetyBeforeTransportationType.접지도선)
                //{
                //    SafetyBeforeTransportationInfoMenu.Inst.NextIndex();
                //}

                return true;
            }
            else
            {
                return false;
            }

        }
    }
}