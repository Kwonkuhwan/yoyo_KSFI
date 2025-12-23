using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class Exit : MonoBehaviour
    {
        [SerializeField] GameObject popup;
        [SerializeField] Button Button_Exit;

        private void Awake()
        {
            Button_Exit.onClick.AddListener(OnClickExitButton);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!popup.gameObject.activeSelf)
                {
                    popup.gameObject.SetActive(true);
                }
            }
        }

        void OnClickExitButton()
        {
            Application.Quit();
        }
    }
}