using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class back_scissor_canvas : MonoBehaviour {

    public GameObject panel;
    public RectTransform canvas;

    public void setleftPanel(Rect _rect)
    { 
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.offsetMax = new Vector2((canvas.rect.width * _rect.width)- canvas.rect.width, 0);
    }

    public void setrightPanel(Rect _rect)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.offsetMin = new Vector2(canvas.rect.width - (canvas.rect.width * _rect.width), 0);
        //d.offsetMin = new Vector2(canvas.offsetMax.x * _rect.xMin, 0);
    }

    public void settopPanel(Rect _rect)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.offsetMax = new Vector2(0, (canvas.rect.height * _rect.height) - canvas.rect.height);

    }

    public void setbottomPanel(Rect _rect)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.offsetMin = new Vector2(0, canvas.rect.height - (canvas.rect.height * _rect.height));
    }

}
