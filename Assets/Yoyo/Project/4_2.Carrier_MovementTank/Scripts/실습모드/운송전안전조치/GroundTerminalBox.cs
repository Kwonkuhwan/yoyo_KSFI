using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class GroundTerminalBox : MonoBehaviour
    {
        [SerializeField] private Button btn_Point;
        [SerializeField] private InfoMenu infoMenu;

        private void Awake()
        {
            if (btn_Point == null) return;
            if (infoMenu == null) return;
            btn_Point.onClick.AddListener(() => infoMenu.NextIndex());
        }

        private void OnEnable()
        {
            if (btn_Point == null) return;
            if (!btn_Point.gameObject.activeInHierarchy)
            {
                btn_Point.gameObject.SetActive(true);
            }
        }
    }
}
