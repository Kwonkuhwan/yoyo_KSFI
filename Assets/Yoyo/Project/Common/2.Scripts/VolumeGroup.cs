using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeGroup : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeText;

    private void Init()
    {
        volumeSlider.onValueChanged.RemoveAllListeners();
        volumeSlider.onValueChanged.AddListener((value)=>
        {
            volumeText.text = $"{SoundManager.ConvertVolumeToUser(value)}";
        });
    }

    public void SetVolume(float volume)
    {
        volumeSlider.value = volume;
    }

    public float GetVolume()
    {
        return volumeSlider.value;
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
