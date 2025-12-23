using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMW.Sprinkler
{
    public class ExitButton : MonoBehaviour
    {
        [SerializeField] GameObject Button_Exit;

        private void OnEnable()
        {
            if(ScenarioManager.Instance.IsMode == true)
            {
                Button_Exit.SetActive(true);
            }
            else
            {
                Button_Exit.SetActive(false);
            }
        }
    }
}
