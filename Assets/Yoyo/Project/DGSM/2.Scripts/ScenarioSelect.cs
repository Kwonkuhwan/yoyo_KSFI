using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DGSM
{

    public class ScenarioSelect : MonoBehaviour
    {
        public Button carSelectBtn;
        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnEnable()
        {
            carSelectBtn.onClick.RemoveAllListeners();
            carSelectBtn.onClick.AddListener(delegate
            {
                DGSM_Manager.Instance.GetPracticeModePanel().ChangeState(PracticeModeState.CarScenario);
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}