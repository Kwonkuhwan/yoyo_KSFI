using UnityEngine;

namespace RJH.DangerousGoods
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private GameObject[] items;
        [SerializeField] private GameObject[] setItems;
        [SerializeField] private UIDragAndCollisionHandler dragHandler;
        [SerializeField] private bool isShoringTimber = false;

        private void OnEnable()
        {
            if(isShoringTimber)
                SetShoringTimber();
            else
                SetTrafficConAndFireExtinguisher();
        }

        public void SetShoringTimber()
        {

            ShoringTimberEvent();
        }

        public void SetTrafficConAndFireExtinguisher()
        {

            TrafficConAndFireExtinguisherEvent();
        }

        public void ShoringTimberEvent()
        {
            dragHandler.ResetEvent();
            dragHandler.OnCollisionDetected += (GameObject d, GameObject t) =>
            {
                d.SetActive(false);
                d.GetComponent<DocsButton>().OnClick();
                t.SetActive(false);
                t.GetComponent<DocsButton>().OnClick();
                //setItems[0].GetComponent<DocsButton>().OnClick();
                //setItems[0].gameObject.SetActive(true);
            };
        }

        public void TrafficConAndFireExtinguisherEvent()
        {
            dragHandler.ResetEvent();
            dragHandler.OnCollisionDetected += (GameObject d, GameObject t) =>
                {
                    if(!t.activeSelf)
                        return;

                    if (items[0].Equals(d))
                    {
                        d.SetActive(false);
                        d.GetComponent<DocsButton>().OnClick();
                        t.SetActive(false);
                        t.GetComponent<DocsButton>().OnClick();
                    }
                    else
                    {
                        d.SetActive(false);
                        d.GetComponent<DocsButton>().OnClick();
                        t.SetActive(false);
                        t.GetComponent<DocsButton>().OnClick();
                    }
                };
        }

    }
}


