using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class ChoiceAnser : MonoBehaviour
    {
        [SerializeField] private int questionNum = 0;
        [SerializeField] private Button[] buttons;
        [SerializeField] protected GameObject[] outlines;
        [SerializeField] protected MultipleChoice_Evaluation evaluation;

        private int anser;
        public int Anser => anser;

        private void Awake()
        {
            for (int idx = 0; idx < buttons.Length; idx++)
            {
                int i = idx;
                buttons[i].onClick.AddListener(() => BtnClick(i));
            }

            foreach (var obj in outlines)
            {
                obj.SetActive(false);
            }
        }

        private void BtnClick(int idx)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i == idx)
                {
                    outlines[i].SetActive(true);
                }
                else
                {
                    outlines[i].SetActive(false);
                }
            }

            evaluation.ansers[questionNum] = idx + 1;
        }
    }
}
