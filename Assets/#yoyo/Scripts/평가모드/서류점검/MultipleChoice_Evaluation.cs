using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class MultipleChoice_Evaluation : MonoBehaviour
    {
        [SerializeField] protected int quizNum;
        [SerializeField] protected List<int> rightAnswer;
        public List<int> ansers = new List<int>();

        private void Awake()
        {
            //for (int idx = 0; idx < buttons.Length; idx++)
            //{
            //    int i = idx;
            //    buttons[i].onClick.AddListener(() => BtnClick(i));
            //}

            //foreach (var obj in outlines)
            //{
            //    obj.SetActive(false);
            //}
        }

        protected virtual void Update()
        {
            EvaluationManager.Inst.DocumentPoint[quizNum - 1] = rightAnswer.SequenceEqual(ansers);
        }

        protected virtual void BtnClick(int idx)
        {
            //for (int i = 0; i < buttons.Length; i++)
            //{
            //    if (i == idx)
            //    {
            //        outlines[i].SetActive(true);
            //    }
            //    else
            //    {
            //        outlines[i].SetActive(false);
            //    }

            //    if(idx == rightAnswer - 1)
            //    {
            //        EvaluationManager.Inst.DocumentPoint[quizNum - 1] = true;
            //    }
            //    else
            //    {
            //        EvaluationManager.Inst.DocumentPoint[quizNum - 1] = false;
            //    }
            //}
        }
    }
}