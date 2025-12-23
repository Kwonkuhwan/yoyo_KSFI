using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GasSysArea1 : MonoBehaviour
{
    [SerializeField] private GameObject smokeDetectorObj;
    [SerializeField] private GameObject heatDetectorObj;

    [SerializeField] private GameObject smokeDetectorOn;
    [SerializeField] private GameObject heatDetectorOn;

    [SerializeField] private SmokeDetectorPopup smokeDetectorPopup;
    [SerializeField] private HeatDetectorPopup heatDetectorPopup;

    [SerializeField] private Inventory inventory;
    [SerializeField] private UIDragAndCollisionHandler uiDragAndCollisionHandler;

#region 기동용기 솔레노이드밸브 -> 교차회로 감지기 동작

    [SerializeField] private HintScriptableObj _crossCircuitDetectorHint;
    [SerializeField] private RectTransform[] _crossCircuiteDetectorHintRects;
    
#endregion

    private bool isInit = false;
    private void Init()
    {
        smokeDetectorOn.SetActive(false);
        heatDetectorOn.SetActive(false);
        smokeDetectorPopup.gameObject.SetActive(false);
        heatDetectorPopup.gameObject.SetActive(false);
        //uiDragAndCollisionHandler.ResetEvent();
    }

    public void InitCrossCircuitDetector(UnityAction crossCircuitDetectorAction)
    {
        Init();
        isInit = true;
        uiDragAndCollisionHandler.OnCollisionDetected += (draggedObject, targetObject) =>
        {
            if (!this.gameObject.activeSelf)
                return;

            if (smokeDetectorObj.Equals(targetObject))
            {
                //GlobalCanvas.Instance.ShowHint(false);
                ControlPanel.Instance.SetArea1PopupParent();
                ControlPanel.Instance.ShowPanel(true);
                inventory.ShowSmokeDetect(false);
                smokeDetectorPopup.InitCrossCircuitDetector(delegate
                {
                    ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector1, true);
                    ControlPanel.Instance.ShowFire(true);
                    smokeDetectorOn.SetActive(true);
                }, delegate
                {
                    //GlobalCanvas.Instance.SetHintPopup(4,4, _crossCircuitDetectorHint, _crossCircuiteDetectorHintRects[0]);
                    //GlobalCanvas.Instance.ShowHint(true);
                    if (smokeDetectorOn.activeSelf && heatDetectorOn.activeSelf)
                    {
                        crossCircuitDetectorAction?.Invoke();
                    }
                });
                smokeDetectorPopup.gameObject.SetActive(true);
            }

            if (!heatDetectorObj.Equals(targetObject))
                return;
            //GlobalCanvas.Instance.ShowHint(false);
            ControlPanel.Instance.SetArea1PopupParent();
            ControlPanel.Instance.ShowPanel(true);
            inventory.ShowHeatDetect(false);
            heatDetectorPopup.InitCrossCircuitDetector(delegate
            {
                heatDetectorOn.SetActive(true);
                ControlPanel.Instance.SetArea1Check(ControlPanel.EAreaName.Detector2, true);
                ControlPanel.Instance.ShowFire(true);
            }, delegate
            {
                if (smokeDetectorOn.activeSelf && heatDetectorOn.activeSelf)
                {
                    crossCircuitDetectorAction?.Invoke();
                }
            });
            heatDetectorPopup.gameObject.SetActive(true);
        };
    }

    private void OnEnable()
    {
        if (isInit)
        {
            //GlobalCanvas.Instance.SetHintPopup(3,3, _crossCircuitDetectorHint, _crossCircuiteDetectorHintRects[0]);
            //GlobalCanvas.Instance.ShowHint(true);
        }

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
