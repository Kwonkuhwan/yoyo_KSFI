using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class CarryOnInspectionInven : MonoBehaviour
    {
        public bool isMSDS = false;
        public bool isMegaphone = false;
        public bool isAdsorptionCloth = false;
        public bool isRedFlag = false;
        public bool isTripod = false;

        [SerializeField] private GameObject[] go_Buttons;
        [SerializeField] private ButtonManager_KKH buttonManager_KKH;
        [SerializeField] private GameObject go_Target;

        [SerializeField] private InfoMenu infoMenu;
        [SerializeField] private Button btn_Done;

        [SerializeField] private int count = 0;
        [SerializeField] private GameObject go_Error;

        private void Awake()
        {
            if (btn_Done != null)
            {
                btn_Done.onClick.AddListener(delegate
                {
                    CanvasControl.Inst.panel_Emergency_Response.SetActive(true);
                    CanvasControl.Inst.panel_Safety_Measures.SetActive(false);
                });
            }
        }

        private void OnEnable()
        {
            count = 0;

            if (go_Error.activeInHierarchy)
            {
                go_Error.SetActive(false);
            }

            foreach (GameObject go in go_Buttons)
            {
                go.SetActive(true);
            }

            isMSDS = false;
            isMegaphone = false;
            isAdsorptionCloth = false;
            isRedFlag = false;
            isTripod = false;

            //text_info.text = strs_info[0];
            go_Target.SetActive(true);

            if (btn_Done != null)
            {
                btn_Done.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (btn_Done != null)
            {
                if (btn_Done.gameObject.activeInHierarchy) { return; }
            }
        }

        public bool AnserCheck()
        {
            if (isMSDS && isMegaphone && isAdsorptionCloth && isRedFlag && isTripod)
            {
                buttonManager_KKH.isCompelet = true;
                go_Target.SetActive(false);
                return true;
            }
            else
            {
                return IncreaseErrorCount();
            }
        }

        public bool IncreaseErrorCount()
        {
            go_Error.SetActive(true);

            count++;

            if (count >= 3)
            {
                buttonManager_KKH.isCompelet = true;
                go_Target.SetActive(false);
                foreach (GameObject go in go_Buttons)
                {
                    CarryOnInspectionMoveImage moveImage = go.GetComponent<CarryOnInspectionMoveImage>();
                    if(moveImage.coi == CarryOnInspection.방독마스크)
                    {
                        
                    }
                    else
                    {
                        go.SetActive(false);
                    }
                }

                return true;
            }

            return false;
        }
    }
}