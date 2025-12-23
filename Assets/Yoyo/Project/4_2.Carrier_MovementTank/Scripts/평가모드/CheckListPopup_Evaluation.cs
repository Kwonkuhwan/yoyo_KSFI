using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class CheckListPopup_Evaluation : MonoBehaviour
    {
        public enum AnserCheck
        {
            틀림 = -1,
            선택안함 = 0,
            맞춤 = 1,
        }

        public enum CheckListAnser
        {
            적합 = 0,
            부적합 = 1,
            해당없음 = 2,
        }

        [SerializeField] private int quizNum = 0;
        [SerializeField] private ToggleGroup[] toggleGroup;
        [SerializeField] private List<CheckListAnser> checkNum;
        [SerializeField] private List<AnserCheck> checkAnser = new List<AnserCheck>();

        private void Awake()
        {
            for (int i = 0; i < checkNum.Count; i++)
            {
                checkAnser.Add(AnserCheck.선택안함);
            }
        }

        private void Update()
        {
            for (int idx = 0; idx < toggleGroup.Length; idx++)
            {
                Toggle[] toggles = toggleGroup[idx].GetComponentsInChildren<Toggle>();
                for (int i = 0; i < toggles.Length; i++)
                {
                    if (toggles[i].isOn)
                    {
                        if(i == (int)checkNum[idx])
                        {
                            checkAnser[idx] = AnserCheck.맞춤;
                        }
                        else
                        {
                            checkAnser[idx] = AnserCheck.틀림;
                        }
                    }

                    continue;
                }
            }

            int check = 0;
            foreach(var obj in checkAnser)
            {
                if (obj == AnserCheck.맞춤) check++;
            }

            int qnum = quizNum - 1;
            if(check == checkAnser.Count)
            {
                EvaluationManager.Inst.RegularinspectionPoint[qnum] = true;
            }
            else
            {
                EvaluationManager.Inst.RegularinspectionPoint[qnum] = false;
            }
        }
    }
}