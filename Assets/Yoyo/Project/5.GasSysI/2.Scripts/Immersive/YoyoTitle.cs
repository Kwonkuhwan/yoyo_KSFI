using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class YoyoTitle : MonoBehaviour
{
    [SerializeField] private Button gasSysIBtn;
    // Start is called before the first frame update
    void Start()
    {
        gasSysIBtn.onClick.RemoveAllListeners();
        gasSysIBtn.onClick.AddListener(delegate
        {
            SceneManager.LoadSceneAsync("GasSysIScene");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
