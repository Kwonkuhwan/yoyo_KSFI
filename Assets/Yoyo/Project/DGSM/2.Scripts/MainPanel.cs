using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DGSM
{
    public class MainPanel : MonoBehaviour
    {
        //실습모드
        [SerializeField] private Button practiceModeBtn;
        //평가모드
        [SerializeField] private Button assessmentModeBtn;
        // Start is called before the first frame update
        private void Start()
        {

        }

        public void Init(UnityAction practiceMode, UnityAction assessmentMode)
        {
            SetButton(practiceMode, assessmentMode);
        }


        public void SetButton(UnityAction practiceMode, UnityAction assessmentMode)
        {
            practiceModeBtn.onClick.RemoveAllListeners();
            assessmentModeBtn.onClick.RemoveAllListeners();

            practiceModeBtn.onClick.AddListener(practiceMode);
            assessmentModeBtn.onClick.AddListener(assessmentMode);
        }


    }
}