using UnityEngine;
using UnityEngine.UI;


namespace RJH.DangerousGoods
{
    public class BackGroundSpriteChange : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private string title;
        [SerializeField] private string docs;
        [SerializeField] private AudioClip audioClip;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(SpriteChange);
        }

        private void SpriteChange()
        {
            SectionAndBackGroundManager.Instance.BackGroundChange();
            if(title != null && title != "")
            {
                SectionAndBackGroundManager.Instance.SetDocument_text(title, docs);
                AudioManager.Instance.PlayDocs(audioClip);
            }
        }
    }
}