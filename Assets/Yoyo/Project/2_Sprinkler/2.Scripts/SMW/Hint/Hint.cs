using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class Hint : MonoBehaviour
    {
        class Data
        {
            [Serializable]
            public struct Case
            {
                public string title;
                public string[] Message;
            }
            public Case[] Type;
        }
        Data data;

        [Serializable]
        class Audio
        {
            public AudioClip[] clip;
        }

        // 시나리오 타입
        int type = 0;
        // 시나리오 타입의 메시지 배열 번호
        int index = 0;

        [Header("점검 전 조치 데이터")]
        [SerializeField] TextAsset Hint_Data;

        [Header("텍스트")]
        [SerializeField] TMP_Text Text_Title;
        [SerializeField] TMP_Text Text_Message;
        [SerializeField] TMP_Text Text_Next;

        [Header("버튼")]
        [SerializeField] Button Button_Pre;
        [SerializeField] Button Button_Next;
        [SerializeField] Button Button_Audio;

        [Header("오디오")]
        [SerializeField] AudioSource AI_Audio;
        [SerializeField] Audio[] audios;

        [SerializeField] Sprite Audio_Off;
        [SerializeField] Sprite Audio_ON;
        bool isAudio = true;

        void Awake()
        {
            SettingHintData();
            SettingButton();

            HighLight.Instance.OnHighLight += OnHighLight;
        }

        void OnHighLight()
        {
            HighLight.Instance.On(Button_Next);
        }

        // 초기화
        public void Reset()
        {
            index = 0;
        }

        void SettingButton()
        {
            Button_Pre.onClick.AddListener(OnClickPreButton);
            Button_Next.onClick.AddListener(OnClickNextButton);
            Button_Audio.onClick.AddListener(() =>
            {
                if(AI_Audio.mute == true)
                {
                    AI_Audio.mute = false;
                    Button_Audio.GetComponent<Image>().sprite = Audio_ON;
                    isAudio = true;
                }
                else
                {
                    AI_Audio.mute = true;
                    Button_Audio.GetComponent<Image>().sprite = Audio_Off;
                    isAudio = false;
                }
            });
        }

        void SettingHintData()
        {
            data = new Data();
            data = JsonUtility.FromJson<Data>(Hint_Data.ToString());
        }

        // 이전버튼
        void OnClickPreButton()
        {
            ScenarioManager.Instance.PrevScenario();
        }

        // 다음버튼
        public void OnClickNextButton()
        {
            index++;
            SetHint(index);

            ScenarioManager.Instance.NextScenario();
        }

        /// <summary>
        /// 다음버튼 선택가능 여부
        /// </summary>
        public void ActiveNextButton(bool isActive, bool isMode = false)
        {
            Button_Next.interactable = isActive;

            if(isActive)
            {
                if(isMode == false)
                {
                    //Text_Message.text = data.Type[type].Message[index] + "(완료)";
                    Text_Message.text = InsertTextBeforeNewline(data.Type[type].Message[index], "(완료)");
                }
                else
                {
                    ActivePreButton(false);
                }
                Text_Next.alpha = 1f;
            }
            else
            {
                Text_Message.text = data.Type[type].Message[index];
                Text_Next.alpha = 0.5f;
            }
        }

        string InsertTextBeforeNewline(string original, string insert)
        {
            // "\n"의 위치를 찾음
            int newlineIndex = original.IndexOf("\n");

            // "\n"이 없다면 원본 문자열 반환
            if (newlineIndex == -1)
            {
                return original + " " + insert;
            }

            // "\n" 앞에 텍스트 삽입
            return original.Insert(newlineIndex, " " + insert);
        }

        void ActivePreButton(bool isActive)
        {
            Button_Pre.gameObject.SetActive(isActive);
        }

        /// <summary>
        /// 힌트창 오픈
        /// </summary>
        /// <param name="type"> 시나리오 타입 </param>
        /// <param name="_index"> 오픈할 힌트 텍스트 배열번호 </param>
        public void OpenHint(SCENARIO type, int _index = 0)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            this.type = (int)type;

            index = _index;
            SetHint(_index);
        }

        /// <summary>
        /// 점검 전 조치 완료
        /// </summary>
        public void CompeleteInspection(SCENARIO type)
        {
            this.type = (int)type;
            index = -1;
        }

        // 힌트 설정
        void SetHint(int index)
        {
            // 텍스트 설정
            Text_Title.text = data.Type[type].title;
            Text_Message.text = data.Type[type].Message[index];
            AI_Audio.clip = audios[type].clip[index];

            // 버튼 설정
            if (index == 0)
            {
                Button_Pre.gameObject.SetActive(false);
            }
            else
            {
                Button_Pre.gameObject.SetActive(true);
            }

            if (index == data.Type[type].Message.Length - 1)
            {
                Button_Next.gameObject.SetActive(false);
            }
            else
            {
                Button_Next.gameObject.SetActive(true);
            }

            AI_Audio.Play();
        }

        public void MuteHint(bool isMute)
        {
            if(isAudio == false)
            {
                if(isMute == false)
                {
                    AI_Audio.mute = true;
                }
                else
                {
                    AI_Audio.mute = isMute;
                }
            }
            else
            {
                AI_Audio.mute = isMute;
            }
        }
    }
}
