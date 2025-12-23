using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasControl_Mode : MonoBehaviour
{
    public static CanvasControl_Mode Inst;

    [SerializeField] private Button btn_Practice;
    [SerializeField] private Button btn_Evaluation;

    public GameObject panel_Quit;

    private void Awake()
    {
        Inst = this;

        btn_Practice.onClick.AddListener(delegate
        {
            SceneManager.LoadSceneAsync("Carrier_Movenment");
        });

        btn_Evaluation.onClick.AddListener(delegate
        {
            SceneManager.LoadSceneAsync("Carrier_Movenment_Evaluation");
        });
    }

    private void Update()
    {
        // 2025-03-18 RJH WEBGL ESC 비활성화
#if !UNITY_WEBGL
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!panel_Quit.activeInHierarchy)
            {
                panel_Quit.SetActive(true);
            }
            else
            {
                panel_Quit.SetActive(false);
            }
        }
#endif
    }
}
