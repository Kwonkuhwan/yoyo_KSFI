                  using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

namespace KKH
{
    public class EvaluationMenuInfo : MonoBehaviour
    {
        [Foldout("메뉴 공통")]
        public int titleIdx = 0;
        public int infoIdx = 0;

        public Button btn_Prev;
        public Button btn_Next;
        public EvaluationInfoDataScriptableObject infoData;

        public TMP_Text text_Title;
        public TMP_Text text_Info;

        public bool isEnd = false;

        private static EvaluationMenuInfo instance;
        public static EvaluationMenuInfo Inst;

        [SerializeField] private GameObject[] go_Actions;

        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected bool isMute = false;
        [SerializeField] protected Button btn_Mute;
        [SerializeField] protected Sprite[] sprites_Mute;

        protected virtual void Awake()
        {
            if (btn_Prev != null)
            {
                btn_Prev.onClick.AddListener(() => PrevBtnClick());
            }

            if (btn_Next != null)
            {
                btn_Next.onClick.AddListener(() => NextBtnClick());
            }

            if(btn_Mute != null)
            {
                btn_Mute.onClick.AddListener(()=> MuteImageChange());
            }

            audioSource = GetComponent<AudioSource>();
        }

        protected virtual void Start()
        {
            infoIdx = 0;
            //if (btn_Prev != null)
            //{
            //    btn_Prev.gameObject.SetActive(false);
            //}

            if (infoData != null)
            {
                text_Title.text = infoData.str_Title[infoIdx];
                text_Info.text = infoData.str_infodatas[infoIdx];
            }

            PageScroll();
        }

        public virtual void PrevBtnClick()
        {
            infoIdx--;

            if (infoIdx < 0)
            {
                PrevPage();

                infoIdx = 0;
            }

            text_Info.text = infoData.str_infodatas[infoIdx];
            text_Title.text = infoData.str_Title[infoIdx];
            PageScroll();
        }

        protected virtual void PrevPage()
        {
            gameObject.transform.parent.gameObject.SetActive(false);
        }

        public virtual void NextIndex()
        {
            if (!btn_Prev.gameObject.activeInHierarchy)
            {
                btn_Prev.gameObject.SetActive(true);
            }

            if (isEnd && infoIdx >= go_Actions.Length - 1)
            {
                CanvasControl_Evaluation.Inst.panel_EvalEndpopup.SetActive(true);
                CanvasControl_Evaluation.Inst.panel_Evaluation.SetActive(false);
                return;
            }

            if (infoData != null)
            {
                if (infoIdx < infoData.str_infodatas.Length - 1)
                {
                    infoIdx++;
                    text_Info.text = infoData.str_infodatas[infoIdx];
                    text_Title.text = infoData.str_Title[infoIdx];
                }
            }
        }
        public virtual void NextBtnClick()
        {
            NextIndex();
            PageScroll();
        }

        public void PageScroll()
        {
            if (audioSource != null)
            {
                audioSource.clip = infoData.audioClip[infoIdx];
                audioSource.Play();
            }
            if (infoIdx == 0)
            {
                btn_Prev.gameObject.SetActive(false);
            }

            AllHidePopup();
            go_Actions[infoIdx].SetActive(true);
        }

        public void AllHidePopup()
        {
            foreach (var obj in go_Actions)
            {
                obj.SetActive(false);
            }
        }

        private void MuteImageChange()
        {
            if (isMute)
            {
                isMute = false;
                btn_Mute.GetComponent<Image>().sprite = sprites_Mute[1];
            }
            else
            {
                isMute = true;
                btn_Mute.GetComponent<Image>().sprite = sprites_Mute[0];
            }

            if (audioSource != null)
            {
                audioSource.mute = isMute;
            }
        }
    }
}