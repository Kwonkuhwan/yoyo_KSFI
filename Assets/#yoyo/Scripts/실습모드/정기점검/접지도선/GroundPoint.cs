using UnityEngine;

namespace KKH {
    public class GroundPoint : MonoBehaviour
    {
        public bool isEvaluation = false;
        [SerializeField] private float fEvaluation_CoolTime = 1.0f;
        [SerializeField] private float fEvaluation_time = 0.0f;

        public bool isEnable = false;
        [SerializeField] private ButtonManager_KKH buttonManager_KKH;

        private void OnEnable()
        {
            isEnable = false;
        }

        private void Update()
        {
            if (isEvaluation)
            {
                fEvaluation_time += Time.deltaTime;

                if (fEvaluation_CoolTime > fEvaluation_time)
                {
                    isEnable = false;
                    fEvaluation_time = 0.0f;
                }
            }
        }

        public void SetEnable()
        {
            if (isEvaluation)
            {
                isEnable = true;
                if (buttonManager_KKH != null)
                {
                    buttonManager_KKH.isCompelet = true;
                }
            }
        }
    }
}