using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class CheckListPopup : MonoBehaviour
    {
        [SerializeField] private ToggleGroup[] toggleGroup;
        [SerializeField] private int[] checkNum;

        [SerializeField] private Button btn_Done;
        [SerializeField] private Sprite sprite_Disable;
        [SerializeField] private Sprite sprite_Enable;

        [SerializeField] private GameObject go_Error;

        [SerializeField] private InfoMenu infoMenu;

        [SerializeField] private bool isEnd;
        [SerializeField] private RegularInspectionType rit;

        [SerializeField] private GameObject go_비고;

        private void Awake()
        {
            btn_Done.onClick.AddListener(() => DoneBtnClick());
        }

        private void OnEnable()
        {
            //btn_Done.gameObject.SetActive(false);
            btn_Done.interactable = false;
            btn_Done.GetComponent<Image>().sprite = sprite_Disable;
            if (go_비고 != null)
            {
                go_비고?.SetActive(false);
            }

            foreach (var tGroup in toggleGroup)
            {
                foreach (var toggle in tGroup.GetComponentsInChildren<Toggle>())
                {
                    toggle.isOn = false;
                }
            }
        }

        private void Update()
        {
            int check = 0;
            for (int idx = 0; idx < toggleGroup.Length; idx++)
            {
                Toggle[] toggles = toggleGroup[idx].GetComponentsInChildren<Toggle>();
                for (int i = 0; i < toggles.Length; i++)
                {
                    if (toggles[i].isOn)
                    {
                        check++;
                        if (i == 1)
                        {
                            if (go_비고 != null)
                            {
                                go_비고?.SetActive(true);
                            }
                        }
                        else
                        {
                            if (go_비고 != null)
                            {
                                go_비고?.SetActive(false);
                            }
                        }
                    }

                    continue;
                }

                if (check == toggleGroup.Length)
                {
                    btn_Done.interactable = true;
                    btn_Done.GetComponent<ButtonManager_KKH>().isEnable = true;
                    btn_Done.GetComponent<Image>().sprite = sprite_Enable;
                }
            }
        }

        private void DoneBtnClick()
        {
            for (int idx = 0; idx < toggleGroup.Length; idx++)
            {
                int num = checkNum[idx];
                Toggle[] toggles = toggleGroup[idx].GetComponentsInChildren<Toggle>();
                if (!toggles[num].isOn)
                {
                    go_Error.SetActive(true);
                    return;
                }
            }

            if (isEnd)
            {
                RegularInspection.Inst.SetBtnImage(rit);
            }

            infoMenu.NextIndex();
        }

    }
}