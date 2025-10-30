using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SMW.Sprinkler.EvaluationManager;

namespace KKH
{
    public class DangerousAccidentPopup : MonoBehaviour
    {
        public bool isRightAnser = false;
        public int count = 0;

        [SerializeField] private GameObject panel_SelectAnswer;
        [SerializeField] private GameObject panel_RightAnswer;
        [SerializeField] private GameObject panel_ErrorAnswer;

        [SerializeField] private DangerousAccidentMoveImage[] dangerousAccidentMoveImage;
        [SerializeField] private DangerousAccidentInven[] rigthReportInvens;
        [SerializeField] private DangerousAccidentInven[] erroranserInvens;

        [SerializeField] private GameObject go_Error;

        private void Awake()
        {
            dangerousAccidentMoveImage = panel_SelectAnswer.GetComponentsInChildren<DangerousAccidentMoveImage>();

            rigthReportInvens = panel_RightAnswer.GetComponentsInChildren<DangerousAccidentInven>();
            erroranserInvens = panel_ErrorAnswer.GetComponentsInChildren<DangerousAccidentInven>();
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
                if (item.ar == DangerousAccident.흡착포 || item.ar == DangerousAccident.소방대 || item.ar == DangerousAccident.바람)
                {
                    cnt++;
                }
            }

            if (cnt != 3)
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
                if (item.ar == DangerousAccident.빗자루 || item.ar == DangerousAccident.소방번호 || item.ar == DangerousAccident.차량뒷편)
                {
                    cnt++;
                }
            }

            if (cnt != 3)
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
            foreach (var item in rigthReportInvens)
            {
                item.ShowImage(DangerousAccident.None);
            }

            foreach (var item in erroranserInvens)
            {
                item.ShowImage(DangerousAccident.None);
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
                        item.ShowImage(DangerousAccident.흡착포);
                    }
                    else if (cnt == 1)
                    {
                        item.ShowImage(DangerousAccident.소방대);
                    }
                    else if (cnt == 2)
                    {
                        item.ShowImage(DangerousAccident.바람);
                    }
                    cnt++;
                    if (cnt == 3) break;
                }
                cnt = 0;
                foreach (var item in erroranserInvens)
                {
                    if (cnt == 0)
                    {
                        item.ShowImage(DangerousAccident.빗자루);
                    }
                    else if (cnt == 1)
                    {
                        item.ShowImage(DangerousAccident.소방번호);
                    }
                    else if (cnt == 2)
                    {
                        item.ShowImage(DangerousAccident.차량뒷편);
                    }
                    cnt++;
                    if (cnt == 3) break;
                }

                cnt = 0;
                foreach (var item in dangerousAccidentMoveImage)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        public void ResetMoveImage(DangerousAccident inar)
        {
            foreach (var item in dangerousAccidentMoveImage)
            {
                if (item.da == inar)
                {
                    item.gameObject.SetActive(true);
                }
            }
        }
    }
}