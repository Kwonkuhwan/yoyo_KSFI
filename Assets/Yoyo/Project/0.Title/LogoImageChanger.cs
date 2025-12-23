using UnityEngine;
using UnityEngine.UI;

public class LogoImageChanger : MonoBehaviour
{
    [SerializeField] private Image logoImage;

    [SerializeField] private Image evaluationLogoImage;

    private void Start()
    {
#if KFSI_ALL
        logoImage.gameObject.SetActive(true);
        evaluationLogoImage.gameObject.SetActive(false);
#else
#if KFSI_TEST
        evaluationLogoImage.gameObject.SetActive(true);
        logoImage.gameObject.SetActive(false);
#else
        logoImage.gameObject.SetActive(true);
        evaluationLogoImage.gameObject.SetActive(false);
#endif
#endif
    }
}
