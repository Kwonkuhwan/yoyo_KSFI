using RJH.Transporter;
using UnityEngine;

public class EmergancyDragObjectEvent : MonoBehaviour
{
    public UIDragAndCollisionHandler handler;

    public GameObject[] targetObjects;
    public GameObject[] targetImageObjects;

    public GameObject megaphoneObject;
    public GameObject ledbatonObject;

    public GameObject megaphoneHanded;
    public GameObject ledbatonHanded;
    public GameObject allHanded;

    private void OnEnable()
    {
        handler.OnCollisionDetected += DragObjectEvent;
    }

    private void OnDisable()
    {
        handler.OnCollisionDetected -= DragObjectEvent;
    }

    private void DragObjectEvent(GameObject d, GameObject t)
    {
        d.SetActive(false);
        t.SetActive(false);
        d.GetComponent<DocsButton>().OnClick();
        t.GetComponent<DocsButton>().OnClick();
        
        for (int i = 0; i < targetObjects.Length; i++)
        {
            if (targetObjects[i] == t)
            {
                targetImageObjects[i].SetActive(true);
                break;
            }
        }

        if(d == megaphoneObject)
        {
            //if(ledbatonHanded.activeSelf)
            //{
            //    ledbatonHanded.SetActive(false);
            //    allHanded.SetActive(true);
            //}
            //else
            //{
            //    megaphoneHanded.SetActive(true);
            //}
            ledbatonHanded.SetActive(false);
            megaphoneHanded.SetActive(true);
        }
        if(d == ledbatonObject)
        {
            //if (megaphoneHanded.activeSelf)
            //{
            //    megaphoneHanded.SetActive(false);
            //    allHanded.SetActive(true);
            //}
            //else
            //{
            //    ledbatonHanded.SetActive(true);
            //}
            ledbatonHanded.SetActive(true);
        }
        if(d != megaphoneObject)
            SectionAndBackGroundManager.Instance.OnNextDocument();
    }
}
