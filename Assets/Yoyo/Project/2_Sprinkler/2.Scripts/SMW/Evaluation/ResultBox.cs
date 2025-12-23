using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SMW.Sprinkler
{
    public class ResultBox : MonoBehaviour
    {
        [SerializeField] TMP_Text Text;
        [SerializeField] GameObject Success;

        public void Set(string text, bool isSuccess)
        {
            gameObject.SetActive(true);
            Text.text = text;
            Success.SetActive(isSuccess);
        }
    }
}
