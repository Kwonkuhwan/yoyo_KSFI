using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    // 열감지기
    public class HeatDectector : MonoBehaviour
    {
        public bool IsAnimation
        {
            get { return isAnimation; }
        }
        bool isAnimation = true;

        [SerializeField] GameObject DetectOn;
        [SerializeField] GameObject TesterOn;
        [SerializeField] GameObject HeatDectector_on;

        [SerializeField] Vector2 StopPoint;
        [SerializeField] Transform Tester;

        public Vector2 DefaultPos;
        float speed = 3.0f;

        private void OnEnable()
        {
            StartCoroutine(Animation());
        }

        public void Reset()
        {
            StopAllCoroutines();

            isAnimation = true;
            Tester.localPosition = DefaultPos;

            DetectOn.SetActive(false);
            TesterOn.SetActive(false);
            HeatDectector_on.SetActive(false);
        }

        public void Detect()
        {
            isAnimation = false;
            DetectOn.SetActive(true);
            TesterOn.SetActive(true);
            HeatDectector_on.SetActive(true);
        }

        IEnumerator Animation()
        {
            while(isAnimation)
            {
                if(Vector2.Distance(Tester.localPosition, StopPoint) < 5)
                {
                    isAnimation = false;
                }
                else
                {
                    Tester.localPosition = Vector3.Lerp(Tester.localPosition, StopPoint, Time.deltaTime * speed);
                }
                yield return null;
            }
            DetectOn.SetActive(true);
            TesterOn.SetActive(true);
            HeatDectector_on.SetActive(true);
            ScenarioManager.Instance.CheckScenarioStep();
        }
    }
}
