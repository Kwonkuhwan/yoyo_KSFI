using System.Linq;

namespace KKH
{
    public class DTMultiChoice_Evaluation : MultipleChoice_Evaluation
    {
        protected override void Update()
        {
            EvaluationManager.Inst.SafetyMeasuresPoint[quizNum - 1] = rightAnswer.SequenceEqual(ansers);
        }
    }
}