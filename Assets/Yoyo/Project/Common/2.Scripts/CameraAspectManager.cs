using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAspectManager : MonoBehaviour
{
    public Camera[] cameras;
    public GameObject m_objBackScissor;

    private void Awake()
    {
        UpdateResolution();
    }

    public void UpdateResolution()
    {
        m_objBackScissor = Resources.Load("BackScissor") as GameObject;
        var objCameras = Camera.allCameras;

        //width 2, height 3
        float a = Screen.width / 16f;
        float b = Screen.height / 9f;
        if (Mathf.Approximately(a, b))
            return;
        float fResolutionX = Screen.width / 16.0f;
        float fResolutionY = Screen.height / 9.0f;
        if (fResolutionX > fResolutionY)
        {
            float fValue = (fResolutionX - fResolutionY) * 0.5f;
            fValue = fValue / fResolutionX;
            //fResolutionX fix, left & right Scissor (Viewport Re Setting)
            foreach (var obj in objCameras)
            {
                obj.rect = new Rect(Screen.width * fValue / Screen.width + obj.rect.x * (1.0f - 2.0f * fValue),
                    obj.rect.y
                    , obj.rect.width * (1.0f - 2.0f * fValue), obj.rect.height);
            }

            GameObject objLeftScissor = (GameObject)Instantiate(m_objBackScissor);
            objLeftScissor.GetComponent<Camera>().rect = new Rect(0, 0, Screen.width * fValue / Screen.width, 1.0f);
            objLeftScissor.GetComponent<back_scissor_canvas>().setleftPanel(objLeftScissor.GetComponent<Camera>().rect);
            GameObject objRightScissor = (GameObject)Instantiate(m_objBackScissor);
            objRightScissor.GetComponent<Camera>().rect = new Rect(
                (Screen.width - Screen.width * fValue) / Screen.width, 0
                , Screen.width * fValue / Screen.width, 1.0f);
            objRightScissor.GetComponent<back_scissor_canvas>()
                .setrightPanel(objRightScissor.GetComponent<Camera>().rect);
        }
        else if (fResolutionX < fResolutionY)
        {
            float fValue = (fResolutionY - fResolutionX) * 0.5f;
            fValue = fValue / fResolutionY;
            //fResolutionY fix, Top & Bottom Scissor (Viewport Re Setting)
            foreach (var obj in objCameras)
            {
                obj.rect = new Rect(obj.rect.x,
                    Screen.height * fValue / Screen.height + obj.rect.y * (1.0f - 2.0f * fValue)
                    , obj.rect.width, obj.rect.height * (1.0f - 2.0f * fValue));
                //obj.rect = new Rect( obj.rect.x , obj.rect.y + obj.rect.y * fValue, obj.rect.width, obj.rect.height - obj.rect.height * fValue );
            }

            GameObject objTopScissor = (GameObject)Instantiate(m_objBackScissor);
            objTopScissor.GetComponent<Camera>().rect = new Rect(0, 0, 1.0f, Screen.height * fValue / Screen.height);
            objTopScissor.GetComponent<back_scissor_canvas>().settopPanel(objTopScissor.GetComponent<Camera>().rect);
            GameObject objBottomScissor = (GameObject)Instantiate(m_objBackScissor);
            objBottomScissor.GetComponent<Camera>().rect = new Rect(0,
                (Screen.height - Screen.height * fValue) / Screen.height
                , 1.0f, Screen.height * fValue / Screen.height);
            objBottomScissor.GetComponent<back_scissor_canvas>()
                .setbottomPanel(objBottomScissor.GetComponent<Camera>().rect);
        }
        else
        {
            // Do Not Setting Camera
        }

        SetDefaultCamera();
    }

    private void SetDefaultCamera()
    {
        foreach (var obj in cameras)
        {
            obj.rect = new Rect(0, 0, 1, 1);
        }
    }
}