using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public class AccidentReportPopup : MonoBehaviour
    {
        public bool isRightAnser = false;

        public int count = 0;

        [SerializeField] private GameObject panel_SelectAnswer;
        [SerializeField] private GameObject panel_RightAnswer;
        [SerializeField] private GameObject panel_ErrorAnswer;

        [SerializeField] private AccidentReportMoveImage[] accidentReportMoveImage;
        [SerializeField] private AccidentReportInven[] rigthReportInvens;
        [SerializeField] private AccidentReportInven[] erroranserInvens;

        [SerializeField] private GameObject go_Error;

        private void Awake()
        {
            accidentReportMoveImage = panel_SelectAnswer.GetComponentsInChildren<AccidentReportMoveImage>();

            rigthReportInvens = panel_RightAnswer.GetComponentsInChildren<AccidentReportInven>();
            erroranserInvens = panel_ErrorAnswer.GetComponentsInChildren<AccidentReportInven>();
        }

        private void OnEnable()
        {
            if (go_Error != null && go_Error.activeInHierarchy)
            {
                go_Error.SetActive(false);
            }

            count = 0;
            isRightAnser = false;
        }

        private void Update()
        {
            int cnt = 0;
            foreach (var item in rigthReportInvens)
            {
                if (item.ar == AccidentReport.사고종류 || item.ar == AccidentReport.흡착포 || item.ar == AccidentReport.발생장소 || item.ar == AccidentReport.유의사항)
                {
                    cnt++;
                }
            }

            if(cnt != 4)
            {
                isRightAnser = false;
                return;
            }
            else
            {
                isRightAnser = true;
            }

            cnt = 0;
            foreach (var item in erroranserInvens)
            {

                if (item.ar == AccidentReport.전화 || item.ar == AccidentReport.자격증)
                {
                    cnt++;
                }
            }

            if(cnt != 2)
            {
                isRightAnser = false;
                return;
            }
            else
            {
                isRightAnser = true;
            }
        }

        private void AllItemClear()
        {
            foreach(var item in rigthReportInvens)
            {
                item.ShowImage(AccidentReport.None);
            }

            foreach (var item in erroranserInvens)
            {
                item.ShowImage(AccidentReport.None);
            }
        }

        public void IncreaseErrorCount()
        {
            go_Error.SetActive(true);

            count++;

            if (count >= 3)
            {
                AllItemClear();

                int cnt = 0;
                foreach (var item in rigthReportInvens)
                {
                    if (cnt == 0)
                    {
                        item.ShowImage(AccidentReport.사고종류);
                    }
                    else if (cnt == 1)
                    {
                        item.ShowImage(AccidentReport.흡착포);
                    }
                    else if (cnt == 2)
                    {
                        item.ShowImage(AccidentReport.발생장소);
                    }
                    else if (cnt == 3)
                    {
                        item.ShowImage(AccidentReport.유의사항);
                    }
                    cnt++;
                    if (cnt == 4) break;
                }
                cnt = 0;
                foreach (var item in erroranserInvens)
                {
                    if (cnt == 0)
                    {
                        item.ShowImage(AccidentReport.자격증);
                    }
                    else if (cnt == 1)
                    {
                        item.ShowImage(AccidentReport.전화);
                    }
                    cnt++;
                    if (cnt == 2) break;
                }

                cnt = 0;
                foreach (var item in accidentReportMoveImage)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        public void ResetMoveImage(AccidentReport inar)
        {
            foreach (var item in accidentReportMoveImage)
            {
                if (item.ar == inar)
                {
                    item.gameObject.SetActive(true);
                }
            }
        }
    }
}