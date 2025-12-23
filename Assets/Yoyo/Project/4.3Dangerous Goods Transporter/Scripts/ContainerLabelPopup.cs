using RJH.Transporter;
using UnityEngine;
using UnityEngine.UI;

public class ContainerLabelPopup : MonoBehaviour
{
    [SerializeField] private Button popupConfirmButton;
    [SerializeField] private Button[] containerLabelPopupButtons;
    [SerializeField] private GameObject containerLabelPopup;

    private void Awake()
    {
        foreach (var button in containerLabelPopupButtons)
        {
            button.onClick.AddListener(CloseContainerLabelPopup);
        }
    }

    private void OnEnable()
    {
        SectionAndBackGroundManager.Instance.ReturnEventPrev += CheckContainerLabelPopup;
        popupConfirmButton.onClick.AddListener(OpenContainerLabelPopup);
    }

    private void OnDisable()
    {
        containerLabelPopup.SetActive(true);
        SectionAndBackGroundManager.Instance.ReturnEventPrev -= CheckContainerLabelPopup;
        popupConfirmButton.onClick.RemoveListener(OpenContainerLabelPopup);
    }

    private void CloseContainerLabelPopup()
    {
        containerLabelPopup.SetActive(false);
        // 만약 이전버튼 비활성화 필요하면 이전버튼 인스펙터에서 할당받고 이곳에서 비활성화
    }

    private void OpenContainerLabelPopup()
    {
        containerLabelPopup.SetActive(true);
        // 만약 이전버튼 비활성화 필요하면 이전버튼 인스펙터에서 할당받고 이곳에서 활성화
    }

    private bool CheckContainerLabelPopup()
    {
        if(containerLabelPopup.activeSelf)
        {
            return true;
        }
        else
        {
            containerLabelPopup.SetActive(true);
            RJH.Transporter.PopupManager.Instance.PopupClose();
            return false;
        }
    }
}
