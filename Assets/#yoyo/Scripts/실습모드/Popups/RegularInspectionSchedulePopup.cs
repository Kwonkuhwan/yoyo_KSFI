using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

namespace KKH
{
    public class RegularInspectionSchedulePopup : PopUpControl
    {
        [SerializeField] private bool isDoneBtnHide = false;

        [SerializeField] private Button btn_DangerousGoodsSafetyCard;
        [SerializeField] private GameObject popup_DangerousGoodsSafetyCard;

        [Foldout("실무 교육 이수 여부")]
        [SerializeField] private Button btn_NumberofInspections;
        [SerializeField] private Button btn_LifeofPreservation;
        [SerializeField] private Button btn_DeadlineforSubmission;
        [SerializeField] private Button btn_PlaceofSubmission;

        [SerializeField] private TMP_Text text_NumberofInspections;
        [SerializeField] private TMP_Text text_LifeofPreservation;
        [SerializeField] private TMP_Text text_DeadlineforSubmission;
        [SerializeField] private TMP_Text text_PlaceofSubmission;

        [Foldout("활성화 버튼")]
        [SerializeField] private Button button;

        [Foldout("서류점검 완료 버튼")]
        [SerializeField] private Button btn_Compelet;

        private void OnEnable()
        {
            //if (isDoneBtnHide)
            //{
            //    btn_Done.GetComponent<ButtonControl_KKH>().interactable = false;
            //    btn_Done.gameObject.SetActive(false);
            //}

            //if (text_NumberofInspections != null)
            //{
            //    text_NumberofInspections.gameObject.SetActive(false);
            //}

            //if (text_LifeofPreservation != null)
            //{
            //    text_LifeofPreservation.gameObject.SetActive(false);
            //}

            //if (text_DeadlineforSubmission != null)
            //{
            //    text_DeadlineforSubmission.gameObject.SetActive(false);
            //}

            //if (text_PlaceofSubmission != null)
            //{
            //    text_PlaceofSubmission.gameObject.SetActive(false);
            //}
        }

        protected override void Start()
        {
            base.Start();

            if (btn_NumberofInspections != null)
            {
                btn_NumberofInspections.onClick.AddListener(() => NumberofInspectionsBtnClick());
            }

            if (btn_LifeofPreservation != null)
            {
                btn_LifeofPreservation.onClick.AddListener(() => LifeofPreservationBtnClick());
            }

            if (btn_DeadlineforSubmission != null)
            {
                btn_DeadlineforSubmission.onClick.AddListener(() => DeadlineforSubmissionBtnClick());
            }

            if (btn_PlaceofSubmission != null)
            {
                btn_PlaceofSubmission.onClick.AddListener(() => PlaceofSubmissionBtnClick());
            }

            if (btn_DangerousGoodsSafetyCard != null)
            {
                btn_DangerousGoodsSafetyCard.onClick.AddListener(() => ShowPopup());
            }
        }

        private void Update()
        {
            if (btn_Done != null)
            {
                ButtonControl_KKH buttonControl = btn_Done.GetComponent<ButtonControl_KKH>();
                if (buttonControl != null)
                {
                    if (text_PlaceofSubmission.gameObject.activeInHierarchy)
                    {
                        btn_Done.GetComponent<ButtonControl_KKH>().interactable = true;
                    }
                    else
                    {
                        btn_Done.GetComponent<ButtonControl_KKH>().interactable = false;
                    }
                }
            }
        }

        private void NumberofInspectionsBtnClick()
        {
            if (text_NumberofInspections != null)
            {
                text_NumberofInspections.gameObject.SetActive(true);
                //infoMenu.NextIndex();
                //btn_LifeofPreservation.interactable = true;
                //btn_LifeofPreservation.GetComponent<ButtonManager_KKH>().isEnable = true;
            }
        }

        private void LifeofPreservationBtnClick()
        {
            if (text_LifeofPreservation != null)
            {
                text_LifeofPreservation.gameObject.SetActive(true);
                //infoMenu.NextIndex();
                //btn_DeadlineforSubmission.interactable = true;
                //btn_DeadlineforSubmission.GetComponent<ButtonManager_KKH>().isEnable = true;
            }
        }

        private void DeadlineforSubmissionBtnClick()
        {
            if (text_DeadlineforSubmission != null)
            {
                text_DeadlineforSubmission.gameObject.SetActive(true);
                //infoMenu.NextIndex();
                //btn_PlaceofSubmission.interactable = true;
                //btn_PlaceofSubmission.GetComponent<ButtonManager_KKH>().isEnable = true;
            }
        }

        private void PlaceofSubmissionBtnClick()
        {
            if (text_PlaceofSubmission != null)
            {
                text_PlaceofSubmission.gameObject.SetActive(true);
                //infoMenu.NextIndex();
                //btn_Done.GetComponent<ButtonControl_KKH>().interactable = true;
                //btn_Done.gameObject.SetActive(true);
            }
        }

        protected override void CloseBtnClick()
        {
            base.CloseBtnClick();
        }

        private void ShowPopup()
        {
            infoMenu.NextIndex();
            if (btn_DangerousGoodsSafetyCard != null)
            {
                popup_DangerousGoodsSafetyCard.SetActive(true);
            }
        }

        protected override void DoneBtnClick()
        {
            if (button)
            {
                button.GetComponent<ButtonManager_KKH>().isEnable = true;
            }

            if (btn_Compelet)
            {
                btn_Compelet.GetComponent<ButtonControl_KKH>().interactable = true;
            }

            base.DoneBtnClick();
        }
    }
}