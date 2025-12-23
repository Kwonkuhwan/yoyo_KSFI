using UnityEngine;

public class ObjectReset : MonoBehaviour
{
    [SerializeField] private GameObject[] setOnObjs;
    [SerializeField] private GameObject[] setOffObjs;
    private void OnDisable()
    {
        foreach (GameObject obj in setOnObjs)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in setOffObjs) 
        {
            obj.SetActive(false); 
        }
    }
}
