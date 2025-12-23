using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    [SerializeField] private List<Button> buttons;
    [SerializeField] private List<Button> testBtn;
    // Start is called before the first frame update
    void Start()
    {
        ButtonManager.Instance.InitializeSection();
        //ButtonManager.Instance.DisableAllButtons(buttons);
        ButtonManager.Instance.EnableSpecificButton(testBtn.ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
