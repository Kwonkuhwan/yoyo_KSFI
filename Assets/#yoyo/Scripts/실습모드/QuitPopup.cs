using UnityEngine;
using UnityEngine.UI;

public class QuitPopup : MonoBehaviour
{
    [SerializeField] private Button btn_Done;
    [SerializeField] private Button btn_cancle;

    private void Awake()
    {
        btn_Done.onClick.AddListener(() => QuitBtnClick());
        btn_cancle.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void QuitBtnClick()
    {
        Application.Quit();
    }
}