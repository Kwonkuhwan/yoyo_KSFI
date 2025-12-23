using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW.Sprinkler
{
    public class SV_Popup : Popup
    {
        //[SerializeField] Image handle;
        [SerializeField] RectTransform Handle_rect;
        //[SerializeField] Sprite handle_1;
        //[SerializeField] Sprite handle_2;

        [SerializeField] Button Button_Handle;
        [SerializeField] Button Button_Sol;

        [SerializeField] GameObject on;

        Vector3 open_angle = new Vector3(0, 0, 0);
        Vector3 close_angle = new Vector3(0, 0, 90);

        // 기본값 : 자동
        bool isAuto = true;
        bool isAnimation = false;

        List<Button> list_highlight = new List<Button>();

        protected override void Setting()
        {
            Button_Handle.onClick.AddListener(() =>
            {
                if (isAnimation) return;
                if(isOpenSolVavle)
                {
                    StopAllCoroutines();
                    StartCoroutine(CloseValve());
                }
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(OpenVavle());
                }
                ScenarioManager.Instance.CheckScenarioStep();
            });

            Button_Sol.onClick.AddListener(() =>
            {
                isAuto = !isAuto;
                if(isAuto)
                {
                    Button_Handle.interactable = false;
                    on.SetActive(false);
                }
                else
                {
                    Button_Handle.interactable = true;
                    on.SetActive(true);
                }

                ScenarioManager.Instance.CheckScenarioStep();
            });

            HighLight.Instance.OnHighLight += OnHighLight;
        }

        void OnHighLight()
        {
            HighLight.Instance.On(list_highlight);
        }

        /// <summary>
        /// 밸브 수동으로 돌릴 수 있게 만듬
        /// </summary>
        public void StartMaunal()
        {
            Button_Sol.interactable = true;
        }

        /// <summary>
        /// 버튼 인터렉티브 활성화/비활성화
        /// </summary>
        /// <param name="isActive"></param>
        public void Interactive(bool isActive)
        {
            Button_Sol.interactable = isActive;
            Button_Handle.interactable = isActive;
        }

        protected override void RESET()
        {
            isOpenSolVavle = false;
            isAnimation = false;
            isAuto = true;

            StopAllCoroutines();

            Button_Handle.interactable = false;
            Button_Sol.interactable = false;

            Handle_rect.localEulerAngles = close_angle;
            
            on.SetActive(false);
        }

        public void OpenReset()
        {
            isOpenSolVavle = true;
            isAnimation = false;
            isAuto = true;

            Handle_rect.localEulerAngles = open_angle;

            Button_Handle.interactable = false;
            Button_Sol.interactable = false;

            
            on.SetActive(false);
        }

        /// <summary>
        /// 밸브 개방/폐쇄 체크
        /// </summary>
        /// <param name="isValve"> true = 개방, false = 폐쇄 </param>
        /// <returns></returns>
        public bool CheckVavle(bool isValve)
        {
            if(isOpenSolVavle == isValve)
            {
                if(list_highlight.Contains(Button_Sol))
                {
                    list_highlight.Remove(Button_Sol);
                }
                if(list_highlight.Contains(Button_Handle))
                {
                    list_highlight.Remove(Button_Handle);
                }
                return true;
            }
            else
            {
                if(isAuto == false)
                {
                    if(!list_highlight.Contains(Button_Handle))
                    {
                        list_highlight.Add(Button_Handle);
                    }
                    if (list_highlight.Contains(Button_Sol))
                    {
                        list_highlight.Remove(Button_Sol);
                    }
                }
                else
                {
                    if(!list_highlight.Contains(Button_Sol))
                    {
                        list_highlight.Add(Button_Sol);
                    }
                }
                return false;
            }
        }

        public void StartOpenValve()
        {
            SET(true);
            Reset();
            StartCoroutine(OpenVavle());
        }

        IEnumerator OpenVavle()
        {
            isAnimation = true;
            while (isAnimation)
            {
                if (Handle_rect.localEulerAngles.z <= 1)
                {
                    Handle_rect.localEulerAngles = open_angle;
                    isOpenSolVavle = true;
                    isAnimation = false;
                    ScenarioManager.Instance.CheckScenarioStep();
                    yield break;
                }
                Handle_rect.localEulerAngles = Vector3.Lerp(Handle_rect.localEulerAngles, open_angle, Time.deltaTime * 2);
                yield return null;
            }
        }

        IEnumerator CloseValve()
        {
            isAnimation = true;
            while (isAnimation)
            {
                if (Handle_rect.localEulerAngles.z >= 89)
                {
                    Handle_rect.localEulerAngles = close_angle;
                    isOpenSolVavle = false;
                    isAnimation = false;
                    ScenarioManager.Instance.CheckScenarioStep();
                    yield break;
                }

                Handle_rect.localEulerAngles = Vector3.Lerp(Handle_rect.localEulerAngles, close_angle, Time.deltaTime * 2);
                yield return null;
            }
        }
    }
}
