using RJH.DangerousGoods;
using System.Collections;
using UnityEngine;
public class ConditionCheck : MonoBehaviour
{
    [SerializeField] private GameObject[] checkList;
    [SerializeField] private GameObject activateObj;
    [SerializeField] private GameObject activateObj2;
    [SerializeField] private int sectionNumber;
    [SerializeField] private int docsNumber;
    [SerializeField] private Mode mode;
    private bool conditionMet = false;

    private void OnEnable()
    {
        
        foreach (GameObject obj in checkList)
        {
            if (!obj.activeSelf)
                return;
        }
        conditionMet = true;
        switch (mode)
        {
            case Mode.JustActivateObject:
                
                break;
            case Mode.MoveSection:
                SectionAndBackGroundManager.Instance.SetTitleAndBackGround(sectionNumber);
                break;
            case Mode.ChangeDocs:
                SectionAndBackGroundManager.Instance.SetDocument(docsNumber);
                break;
            case Mode.MoveSectionAndChangeDocs:
                SectionAndBackGroundManager.Instance.SetTitleAndBackGround(sectionNumber);
                SectionAndBackGroundManager.Instance.SetDocument(docsNumber);
                break;
        }
        SectionAndBackGroundManager.Instance.SetCheckListDone();
        activateObj.SetActive(true);
        if(activateObj2 != null)
        {
            activateObj2.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (conditionMet == false)
            return;

        activateObj.SetActive(false);
        if (activateObj2 != null)
        {
            activateObj2.SetActive(false);
        }

        foreach (GameObject obj in checkList)
        {
            obj.SetActive(false);
        }
        conditionMet = false;
    }

    public void CloseCheckList()
    {
        foreach (GameObject obj in checkList)
        {
            obj.SetActive(false);
        }
    }
}

public enum Mode
{
    JustActivateObject,
    MoveSection,
    ChangeDocs,
    MoveSectionAndChangeDocs
}
