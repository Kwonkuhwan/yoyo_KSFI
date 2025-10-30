using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class MoveToGasStationPopup : MonoBehaviour
    {
        [SerializeField] private InfoMenu infoMenu;
        [SerializeField] private Button btn_Done;

        private void Awake()
        {
            btn_Done.onClick.AddListener(delegate
            {
                infoMenu.NextIndex();
            });
        }
    }
}