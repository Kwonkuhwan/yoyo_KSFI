using UnityEngine;
using UnityEngine.UI;


namespace RJH.Transporter
{
    public class BackGroundSpriteChange : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private string title;
        [SerializeField] private string docs;


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
            }
        }
    }
}


