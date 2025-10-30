using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

namespace KKH
{
    public class InfoMenu : MonoBehaviour
    {
        [Foldout("메뉴 공통")]
        public int titleIdx = 0;
        public int infoIdx = 0;

        public Button btn_Prev;
        public Button btn_Next;
        public InfoDataScriptableObject infoData;

        public TMP_Text text_Title;
        public TMP_Text text_Info;

        public bool isPrevGameObject = false;
        public bool isPreveGameObjectParent = false;
        public bool isNextGameObject = false;

        public GameObject go_PrevGameObjectParent;
        public GameObject go_PrevGameObject;
        public GameObject go_NextGameObjectParent;
        public GameObject go_NextGameObject;

        [SerializeField] private bool isNextInfoClear = false;
        [SerializeField] private InfoMenu NextInfoMenu;
        public GameObject[] obj_BackGrounds;

        public bool isEnd = false;
        public GameObject panel_parent;

        [Foldout("하이라이트")]
        [SerializeField] private ButtonManager_KKH[] gos_Button;
        private float highCoolTime = 0;
        [SerializeField] private float maxHighCoolTime = 5.0f;

        [Foldout("오디오")]
        [SerializeField] protected AudioSource audioSource;


        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }

            if (infoData.audioClip.Length > 0 && infoData.audioClip[infoIdx] != null)
            {
                audioSource.clip = infoData.audioClip[infoIdx];
            }


            if (btn_Prev != null)
            {
                btn_Prev.onClick.AddListener(delegate
                {
                    PrevBtnClick();
                });
            }

            if (btn_Next != null)
            {
                btn_Next.onClick.AddListener(delegate
                {
                    NextBtnClick();
                });
            }
        }

        protected virtual void Start()
        {
            infoIdx = 0;
            if (btn_Prev != null)
            {
                if (!isPrevGameObject)
                {
                    btn_Prev.gameObject.SetActive(false);
                }
                else
                {
                    btn_Prev.gameObject.SetActive(true);
                }
            }
        }

        private void Update()
        {
            highCoolTime += Time.deltaTime;

            if (Input.GetMouseButtonDown(0) || maxHighCoolTime <= highCoolTime)
            {
                highCoolTime = 0.0f;
                StartCoroutine(CoroutineCheckHighLight());
            }
        }

        private IEnumerator CoroutineCheckHighLight()
        {
            yield return new WaitForSeconds(0.1f);
            CheckHighLight();
        }

        protected virtual void OnEnable()
        {
            if (infoData != null)
            {
                text_Title.text = infoData.str_Title;
                text_Info.text = infoData.str_infodatas[infoIdx];
                audioSource.clip = infoData.audioClip[infoIdx];
            }

            if (audioSource != null)
            {
                audioSource.Play();
            }
        }

        public virtual void PrevBtnClick()
        {
            infoIdx--;

            if (infoIdx < 0)
            {
                if (!isPrevGameObject)
                {
                    btn_Prev.gameObject.SetActive(false);
                }
                else
                {
                    btn_Prev.gameObject.SetActive(true);
                }

                if (isPrevGameObject && (go_PrevGameObject != null || go_PrevGameObjectParent != null))
                {
                    PrevPage();
                }

                infoIdx = 0;
            }

            try
            {
                if (infoData != null)
                {
                    text_Info.text = infoData.str_infodatas[infoIdx];
                }
            }
            catch { }
            AudioStart();
        }

        protected virtual void PrevPage()
        {
            if (go_PrevGameObjectParent != null && CanvasControl.Inst.isSelectMode && infoIdx <= 0)
            {
                CanvasControl.Inst.panel_Selectmode.SetActive(true);
                panel_parent.SetActive(false);
                return;
            }

            if (isPreveGameObjectParent)
            {
                go_PrevGameObjectParent.SetActive(true);
                panel_parent.SetActive(false);
            }
            else
            {
                go_PrevGameObject.SetActive(true);
                transform.parent.gameObject.SetActive(false);
            }
        }

        public virtual void NextIndex()
        {
            if (!btn_Prev.gameObject.activeInHierarchy)
            {
                btn_Prev.gameObject.SetActive(true);
            }

            if (infoData != null)
            {
                if (infoIdx >= obj_BackGrounds.Length - 1 && isEnd)
                {
                    //CanvasControl.Inst.panel_Selectmode.SetActive(true);
                    //go_PrevGameObjectParent.SetActive(true);
                    if (go_NextGameObjectParent)
                    {
                        go_NextGameObjectParent.SetActive(true);
                    }
                    else
                    {
                        go_NextGameObject.SetActive(true);
                    }
                    panel_parent.SetActive(false);
                    CanvasControl.Inst.isSelectMode = false;
                }

                if (infoIdx < infoData.str_infodatas.Length - 1)
                {
                    infoIdx++;
                    text_Info.text = infoData.str_infodatas[infoIdx];

                    AudioStart();
                }
                else if (isNextGameObject && go_NextGameObject != null)
                {
                    NextPage();
                }
            }
        }

        public bool CheckHighLight()
        {
            try
            {
                bool ishigh = false;
                gos_Button = transform.parent.GetComponentsInChildren<ButtonManager_KKH>();
                foreach (ButtonManager_KKH btnMange in gos_Button)
                {
                    //ButtonManager_KKH btnMange = btn.GetComponent<ButtonManager_KKH>();
                    if (btnMange != null && !btnMange.isCompelet && btnMange.isEnable)
                    {
                        btnMange.ShowAlrt();
                        if (!btnMange.isAutoSkip)
                        {
                            ishigh = true;
                        }
                    }
                }
                return ishigh;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return false;
            }
        }

        public virtual void NextBtnClick()
        {
            if (CheckHighLight()) return;

            NextIndex();
        }

        protected virtual void NextPage()
        {
            if (NextInfoMenu != null && (NextInfoMenu.obj_BackGrounds != null && NextInfoMenu.obj_BackGrounds.Length > 0) || isNextInfoClear)
            {
                NextInfoMenu.infoIdx = 0;
            }

            go_NextGameObject.SetActive(true);
            transform.parent.gameObject.SetActive(false);
        }

        protected virtual void AudioStart()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                try
                {
                    if (infoData != null)
                    {
                        if (infoData.audioClip[infoIdx] == null) return;
                        audioSource.clip = infoData.audioClip[infoIdx];
                        audioSource.Play();
                    }
                }
                catch
                {
                    return;
                }
            }
        }
    }
}