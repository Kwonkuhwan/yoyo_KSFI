using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class PopUpControl : MonoBehaviour
    {
        [SerializeField] private ToggleControl toggleControl;
        [SerializeField] protected Button btn_Done;
        [SerializeField] private Button btn_Close;
        [SerializeField] private GameObject go_parent;
        [SerializeField] protected InfoMenu infoMenu;

        protected virtual void Start()
        {
            if (btn_Done != null)
            {
                btn_Done.onClick.AddListener(() => DoneBtnClick());
            }

            if (btn_Close != null)
            {
                btn_Close.onClick.AddListener(() => CloseBtnClick());
            }
        }

        protected virtual void DoneBtnClick()
        {
            if (go_parent != null)
            {
                go_parent.gameObject.SetActive(false);
            }

            if (infoMenu != null)
            {
                infoMenu.NextIndex();
            }
            gameObject.SetActive(false);
        }

        protected virtual void CloseBtnClick()
        {
            gameObject.SetActive(false);
        }        
    }
}