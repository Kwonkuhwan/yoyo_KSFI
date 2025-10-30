using UnityEngine;

namespace KKH
{
    public class TankBody : MonoBehaviour
    {
        [SerializeField] private GameObject popup_CheckList;
        public GameObject checkList => popup_CheckList;
    }
}
