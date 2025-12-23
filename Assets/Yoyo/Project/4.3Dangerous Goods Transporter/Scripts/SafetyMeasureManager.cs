using RJH.Transporter;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class SafetyMeasureManager : MonoBehaviour
{
    [Foldout("점검표")]
    [SerializeField] private CheckListManager checkList;

    [Foldout("차량이동")]
    [SerializeField] private DocsButton moveToButton;
    [SerializeField] private GameObject truckObject;

    [Space(10)]
    [Foldout("안전장비 설치")]
    [SerializeField] private DocsButton[] chockButtons;
    [SerializeField] private DocsButton[] trafficConeButtons;
    [SerializeField] private DocsButton[] fireExtinguishButtons;
    [SerializeField] private GameObject chockObject;
    [SerializeField] private GameObject trafficConeObject;
    [SerializeField] private GameObject fireExtinguishObject;
    [SerializeField] private UIDragAndCollisionHandler dragAndDropHandler;

    [Space(10)]
    [Foldout("운반용기 이동")]
    [SerializeField] private DocsButton moveContainerButton;

    [Space(10)]
    [Foldout("운반용기 적재")]
    [SerializeField] private DocsButton carryingContainerButton;

    [Space(10)]
    [Foldout("안전점검")]
    [SerializeField] private GameObject safetyMeasureObject;
    [SerializeField] private Button hazardLabelButton;
    [SerializeField] private Button containerButton;
    [SerializeField] private Button loadingAndSecuringButton;
    [SerializeField] private Button fireExtinguishButton;

    [Space(10)]
    [Foldout("위험성 표시")]
    [SerializeField] private GameObject hazardLabelPage;
    [SerializeField] private DocsButton hazardLabelPopupButton;
    [SerializeField] private DocsButton pictogramPopupButton;
    [SerializeField] private DocsButton uNnumberPopupButton;

    [Space(10)]
    [Foldout("운반용기")]
    [SerializeField] private GameObject containerPage;
    [SerializeField] private DocsButton containerPopupButton;
    [SerializeField] private GameObject containerLabelPopup;
    [SerializeField] private DocsButton[] containerLabelPopupButtons;
    [SerializeField] private DocsButton containerReceptaclePopupButton;

    [Space(10)]
    [Foldout("적재 및 고정,결박")]
    [SerializeField] private GameObject loadingAndSecuringPage;
    [SerializeField] private GameObject quizPopup;

    [Space(10)]
    [Foldout("소화기")]
    [SerializeField] private GameObject fireExtinguishPage;
    [SerializeField] private DocsButton fireExtinguishPopupButton;


    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        //moveToButton.docsAction += TruckMoveToStorage;
        dragAndDropHandler.OnCollisionDetected += SettingEvent;
        checkList.nextAction += CheckListNextAction;

    }

    #region 차량이동
    private void TruckMoveToStorage()
    {

    }
    #endregion

    #region 안전장비 설치
    private void SettingEvent(GameObject d, GameObject t)
    {

        if (chockButtons.Contains(t.GetComponentInChildren<DocsButton>()))
        {
            d.GetComponent<DocsButton>().OnRevert();
            d.SetActive(false);
            ChockSet();
        }
        else if (trafficConeButtons.Contains(t.GetComponentInChildren<DocsButton>()))
        {
            d.GetComponent<DocsButton>().OnRevert();
            d.SetActive(false);
            TrafficConeSetting();
        }
        else if (fireExtinguishButtons.Contains(t.GetComponentInChildren<DocsButton>()))
        {
            d.GetComponent<DocsButton>().OnRevert();
            d.SetActive(false);
            FireExtinguishSetting();
        }
    }

    private void ChockSet()
    {
        foreach (var chock in chockButtons)
        {
            chock.OnRevert();
        }
        chockObject.SetActive(true);
    }

    private void TrafficConeSetting()
    {
        foreach (var trafficCone in trafficConeButtons)
        {
            trafficCone.OnRevert();
        }
        trafficConeObject.SetActive(true);
    }

    private void FireExtinguishSetting()
    {
        foreach (var fireExtinguish in fireExtinguishButtons)
        {
            fireExtinguish.OnRevert();
        }
        fireExtinguishObject.SetActive(true);
    }
    #endregion

    #region 운반용기

    private void CheckAllContainerLabelPopupButtons()
    {
        foreach (var button in containerLabelPopupButtons)
        {
            if (button.isChecked == false)
            {
                SectionAndBackGroundManager.Instance.OnNextDocument();
                return;
            }
        }
        containerLabelPopup.SetActive(false);
        SectionAndBackGroundManager.Instance.SetDocument(35);
    }
    #endregion

    // 점검표 점검 완료후 실행할 내용 등록
    private void CheckListNextAction(int index)
    {
        SectionAndBackGroundManager.Instance.OnNextDocument();
    }
}
