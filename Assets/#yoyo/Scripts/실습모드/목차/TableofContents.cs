using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TableofContents : MonoBehaviour
{
    [SerializeField] private Button btn_TableofContents;
    [SerializeField] private GameObject panel_TableofContents;

    private void Awake()
    {
        btn_TableofContents.onClick.AddListener(() => ShowPanel());
    }

    public void ShowPanel()
    {
#if KFSI_ALL
        panel_TableofContents.SetActive(true);
#else
#if !KFSI_TEST
            panel_TableofContents.SetActive(true);

#else
            SceneManager.LoadSceneAsync("TitleScene");
#endif
#endif
    }
}
