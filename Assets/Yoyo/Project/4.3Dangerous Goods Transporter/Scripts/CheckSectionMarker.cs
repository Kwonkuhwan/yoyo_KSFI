using UnityEngine;


namespace RJH.Transporter
{
    public class CheckSectionMarker : MonoBehaviour
    {
        [SerializeField] private GameObject[] markers;
        [SerializeField] private int nextSectionIndex; // 다음 섹션의 번호
        [SerializeField] private GameObject nextSection; // 다름 섹션
        private void OnEnable()
        {
            foreach (var marker in markers)
            {
                if (marker.activeSelf == false)
                {
                    return;
                }
            }
            Debug.Log("실행");
            //this.gameObject.SetActive(false);
            //SectionAndBackGroundManager.Instance.sectionAction = null;
            //SectionAndBackGroundManager.Instance.MoveNextSection(nextSection, nextSectionIndex, false, true);
        }
        public bool ReturnBool()
        {
            foreach (var marker in markers)
            {
                if (marker.activeSelf == false)
                {
                    return false;
                }
            }
            //ResetMarker();

            this.gameObject.SetActive(false);
            SectionAndBackGroundManager.Instance.sectionAction = null;
            SectionAndBackGroundManager.Instance.MoveNextSection(nextSection, nextSectionIndex, false, true);
            return true;
        }
        public void ResetMarker()
        {
            foreach (var marker in markers)
            {
                marker.SetActive(false);
            }
        }
    }
}
