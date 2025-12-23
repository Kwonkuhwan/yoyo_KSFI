using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GASSYS
{
    public class GasSysSelectMode : MonoBehaviour
    {
        [FormerlySerializedAs("autoModeBtn")]
        [SerializeField] private Button practiceModeBtn;
        [FormerlySerializedAs("manualModeBtn")]
        [SerializeField] private Button evaluationBtn;
        [SerializeField] private Button compCheckBtn;

        /*
        public void Init()
        {
            practiceModeBtn?.onClick.RemoveAllListeners();
            evaluationBtn?.onClick.RemoveAllListeners();
            compCheckBtn?.onClick.RemoveAllListeners();

            practiceModeBtn?.onClick.AddListener(delegate
            {
                GasSysManager.Instance.ChangeState(GasSysState.PracticeMode);
            });
            evaluationBtn?.onClick.AddListener(delegate
            {
                GasSysManager.Instance.ChangeState(GasSysState.EvaluationMode);
            });
            // compCheckBtn?.onClick.AddListener(delegate
            // {
            //     GasSysManager.Instance.ChangeState(GasSysState.CompCheck);
            // });
        }

        public void OnEnable()
        {
            ButtonManager.Instance.EnableSpecificButton(practiceModeBtn,evaluationBtn);
        }
        */
    }
}
