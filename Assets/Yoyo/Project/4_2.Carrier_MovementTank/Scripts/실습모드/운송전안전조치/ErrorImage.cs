using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public class ErrorImage : MonoBehaviour
    {
        public float cooltime = 1.0f;

        void OnEnable()
        {
            StartCoroutine(Hide());
        }

        IEnumerator Hide()
        {
            yield return new WaitForSeconds(cooltime);

            gameObject.SetActive(false);
        }
    }
}