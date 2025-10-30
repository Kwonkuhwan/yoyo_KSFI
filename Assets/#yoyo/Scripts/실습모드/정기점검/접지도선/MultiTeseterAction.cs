using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class MultiTeseterAction : MonoBehaviour
    {
        public GroundPoint redpoint;

        public Button btn_Done;

        [SerializeField] private GameObject go_GroundObj;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private bool isSound = false;
        [SerializeField] private GameObject go_groundImage;

        private void Awake()
        {
            if (btn_Done != null)
            {
                btn_Done.onClick.AddListener(() => MultiTeseterInfoMenu.Inst.NextBtnClick());
            }
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            go_GroundObj.SetActive(true);
            go_groundImage.SetActive(false);
            isSound = false;
        }

        private void Update()
        {
            if (isSound) return;
            if (redpoint.isEnable)
            {
                go_groundImage.SetActive(true);

                PlayBuzzer(true);
                //audioSource.Play();
                isSound = true;
            }
        }

        public void PlayBuzzer(bool isPlay, bool loop = true)
        {
            if (audioSource.isPlaying && isPlay)
                return;
            audioSource.loop = loop;
            audioSource.time = 2.1f;
            if (isPlay)
                audioSource.Play();
            else
            {
                audioSource.Stop();
            }
        }
    }
}