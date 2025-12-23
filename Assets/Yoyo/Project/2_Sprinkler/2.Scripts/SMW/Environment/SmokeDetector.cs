using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class SmokeDetector : MonoBehaviour
    {
        public bool IsAnimation
        {
            get { return isAnimation; }
        }
        bool isAnimation = true;

        [SerializeField] Animator animator_spray;
        [SerializeField] GameObject DetectOn;
        [SerializeField] GameObject SmokeDetector_on;

        public void Reset()
        {
            isAnimation = true;
            DetectOn.SetActive(false);
            SmokeDetector_on.SetActive(false);

            if (animator_spray.gameObject.activeInHierarchy)
            {
                animator_spray.SetBool("isOn", false);
            }
        }

        public void Detect()
        {
            isAnimation = false;
            DetectOn.SetActive(true);
            SmokeDetector_on.SetActive(true);
        }

        private void OnEnable()
        {
            StartCoroutine(Animation());
        }

        IEnumerator Animation()
        {
            animator_spray.SetBool("isOn", true);
            while (isAnimation)
            {
                yield return new WaitForSeconds(3);
                isAnimation = false;
            }
            DetectOn.SetActive(true);
            SmokeDetector_on.SetActive(true);
            ScenarioManager.Instance.CheckScenarioStep();
        }
    }
}
