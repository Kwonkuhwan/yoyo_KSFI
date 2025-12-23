using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DGSM
{
    /// <summary>
    /// 시나리오
    /// 1. 차량 진입
    /// 2-1. 주유자 옷차림
    /// 2-2. 작업 환경
    /// 3. 주유기를 열고 주유 실시
    /// 4. 주유기를 꽂고 뒤돌아서 담배를 피우려는 장면
    /// </summary>
    public class Scenario1 : ScenarioObj
    {
        public TextMeshProUGUI titleText;
        private int curScenario = 1;

        private void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            nextBtn.onClick.RemoveAllListeners(); 
            nextBtn.onClick.AddListener(OnNext);

            foreach (var interactableBtn in interactableBtns)
            {
                interactableBtn.interactable = true;
            }
            Intro();   
        }

        private void Intro()
        {
            titleText.text = $"차량이 주유소에 진입하는 장면";
            backgroundImage.sprite = bgSprites[0];
            
        }

        private void OnNext()
        {
            curScenario++;
            switch (curScenario)
            {
                case 1:
                    break;
                case 2:
                    Scenario_2();

                    break;
                case 3:
                    Scenario_3();

                    break;
                
                case 4:
                    Scenario_4();
                    break;
                
                case 5:
                    Scenario_5();
                    break;
                
                case 6:
                    Scenario_6();
                    break;
                case 7:
                    curScenario = 1;
                    DGSM_Manager.Instance.GetPracticeModePanel().ChangeState(PracticeModeState.Init);
                    DGSM_Manager.Instance.ChangeState(DGSMState.Init);
                    break;
                    
            }
        }

        private void Scenario_2()
        {
            nextBtn.interactable = false;
            titleText.text = $"주유자 옷차림 장면";
            eventPopupObj.gameObject.SetActive(false);
            backgroundImage.sprite = bgSprites[1];
            interactableBtns[0].gameObject.SetActive(true);
            interactableBtns[0].onClick.AddListener(delegate
            {
                interactableBtns[0].gameObject.SetActive(false);
                eventPopupObj.Init($"니트류 옷을 입은 모습", delegate
                {
                    eventPopupObj.gameObject.SetActive(false);
                    titleText.text = $"작업 환경";
                    interactableBtns[1].gameObject.SetActive(true);
                    interactableBtns[2].gameObject.SetActive(true);
                    interactableBtns[2].interactable = false;

                });
                eventPopupObj.SetImage(eventPopupSprites[0]);
                eventPopupObj.gameObject.SetActive(true);
            });
            
            interactableBtns[1].onClick.AddListener(delegate
            {
                interactableBtns[1].interactable = false;
                eventPopupObj.Init($"정전기 방지패드", delegate
                {
                    eventPopupObj.gameObject.SetActive(false);
                    interactableBtns[2].interactable = true;
                });
                eventPopupObj.SetImage(eventPopupSprites[1]);
                eventPopupObj.gameObject.SetActive(true);
            });
            interactableBtns[2].onClick.AddListener(delegate
            {  
                interactableBtns[2].interactable = false;
                eventPopupObj.Init($"비닐장갑", delegate
                {
                    eventPopupObj.gameObject.SetActive(false);
                    titleText.text = $"정전기 방지 패드와 비닐장갑 무시";
                    interactableBtns[2].gameObject.SetActive(false);
                    interactableBtns[1].gameObject.SetActive(false);
                    nextBtn.interactable = true;
                });
                eventPopupObj.SetImage(eventPopupSprites[2]);
                eventPopupObj.gameObject.SetActive(true);
            });
           
        }

        private void Scenario_3()
        {            
            nextBtn.interactable = false;
            titleText.text = $"주유기를 열고 주유 실시";
            eventPopupObj.gameObject.SetActive(false);
            backgroundImage.sprite = bgSprites[2];
            interactableBtns[3].gameObject.SetActive(true);
            interactableBtns[3].onClick.AddListener(delegate
            {
                interactableBtns[3].gameObject.SetActive(false);
                eventPopupObj.Init($"주유구 오픈", delegate
                {
                    eventPopupObj.gameObject.SetActive(false);
                    interactableBtns[3].interactable = false;
                    nextBtn.interactable = true;

                });
                eventPopupObj.SetImage(eventPopupSprites[3]);
                eventPopupObj.gameObject.SetActive(true);
            });
           
        }
        
        private void Scenario_4()
        {            
            nextBtn.interactable = false;
            titleText.text = $"주유기를 꽂고 뒤돌아서서 담배를 피우려는 장면";
            eventPopupObj.gameObject.SetActive(false);
            backgroundImage.sprite = bgSprites[3];
            interactableBtns[4].gameObject.SetActive(true);
            interactableBtns[4].onClick.AddListener(delegate
            {
                interactableBtns[4].gameObject.SetActive(false);
                eventPopupObj.Init($"CCTV 담배 확인", delegate
                {
                    eventPopupObj.gameObject.SetActive(false);
                    interactableBtns[4].interactable = false;
                    nextBtn.interactable = true;
                    OnNext();

                });
                eventPopupObj.SetImage(eventPopupSprites[4]);
                eventPopupObj.gameObject.SetActive(true);
            });
           
        }
        
        private void Scenario_5()
        {            
            nextBtn.interactable = false;
            titleText.text = $"안전관리자 감시대에서 CCTV를 보고 있다가 담배피는 모습을 확인하는 장면";
            eventPopupObj.gameObject.SetActive(false);
            backgroundImage.sprite = bgSprites[4];
            nextBtn.interactable = true;
            // interactableBtns[5].gameObject.SetActive(true);
            // interactableBtns[5].onClick.AddListener(delegate
            // {
            //     interactableBtns[5].gameObject.SetActive(false);
            //     eventPopupObj.Init($"주유구 오픈", delegate
            //     {
            //         eventPopupObj.gameObject.SetActive(false);
            //         interactableBtns[3].interactable = false;
            //         nextBtn.interactable = true;
            //
            //     });
            //     eventPopupObj.SetImage(eventPopupSprites[3]);
            //     eventPopupObj.gameObject.SetActive(true);
            // });

        }
        
        private void Scenario_6()
        {            
            nextBtn.interactable = false;
            titleText.text = $"방송장비 마이크 ON 하고 담배를 꺼달라고 요청하는 장면";
            eventPopupObj.gameObject.SetActive(false);
            backgroundImage.sprite = bgSprites[5];
            interactableBtns[5].gameObject.SetActive(true);
            interactableBtns[5].onClick.AddListener(delegate
            {
                interactableBtns[5].gameObject.SetActive(false);
                eventPopupObj.Init($"마이크 ON", delegate
                {
                    eventPopupObj.gameObject.SetActive(false);
                    interactableBtns[5].interactable = false;
                    nextBtn.interactable = true;

                });
                eventPopupObj.SetImage(eventPopupSprites[5]);
                eventPopupObj.gameObject.SetActive(true);
            });
           
        }
    }

}
