using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Transporter_SelectQuiz : Quiz
{
    [SerializeField] private ToggleGroup[] toggleGroups;
    
    public override bool CheckQuestionAnswer()
    {
        int j = 0;
        //ToDo 체크리스트에서 체크를 안 한 부분이 있는지 검사
        foreach (ToggleGroup group in toggleGroups)
        {
            if (!group.ActiveToggles().Any())
            {
                return false;
            }
            int i = 0;
            foreach (Toggle toggle in group.transform.GetComponentsInChildren<Toggle>())
            {
                if (toggle.isOn == true)
                {
                    if (i != answer[j])
                    {
                        return false;
                    }
                }
                ++i;
            }
            ++j;
        }
        return true;
    }

    public override (bool, bool) CheckDoubleQuestionAnswer()
    {
        bool item1 = CheckAnswerForGroup(0);
        bool item2 = CheckAnswerForGroup(1);

        return (item1, item2);
    }

    // 공통 로직을 함수로 분리
    private bool CheckAnswerForGroup(int index)
    {
        if (toggleGroups[index].ActiveToggles().Any())
        {
            int i = 0;
            foreach (Toggle toggle in toggleGroups[index].transform.GetComponentsInChildren<Toggle>())
            {
                if (toggle.isOn && i == answer[index])
                {
                    return true;
                }
                ++i;
            }
        }

        return false;
    }

    public override void QuizReset()
    {
        foreach (ToggleGroup group in toggleGroups)
        {
            if (group.AnyTogglesOn())
            {
                foreach (var toggle in group.ActiveToggles())
                {
                    toggle.isOn = false;
                }
            }
        }
    }
}
