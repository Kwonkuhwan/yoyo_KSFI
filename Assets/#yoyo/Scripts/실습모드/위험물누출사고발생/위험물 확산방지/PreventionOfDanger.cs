using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class PreventionOfDanger : MonoBehaviour
    {
        [SerializeField] private InfoMenu infoMenu;
        [SerializeField] private Button[] go_Buttons;

        private void Awake()
        {
            if (go_Buttons == null || go_Buttons.Length <= 0) return;
            foreach(var go in go_Buttons)
            {
                go.onClick.AddListener(() => infoMenu.NextBtnClick());
            }
        }
    }
}