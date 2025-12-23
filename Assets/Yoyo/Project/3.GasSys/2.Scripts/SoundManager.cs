using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
#region singleton

    public static SoundManager Instance
    {
        get;
        private set;
    }

#endregion
    
    [Header("Audio Sources")]
    public AudioSource buzzerSource;
    public AudioSource sirenSource;
    public AudioSource broadcastSource;
    public AudioSource uiClickSource;
    public AudioSource alarmSource;
    public AudioSource alarm2Source;
    public AudioSource hintSource;
    public AudioSource bangSource;
    public AudioSource pumpSource;

    [Header("Audio Clips")]
    public AudioClip buzzerClip;
    public AudioClip sirenClip;
    public AudioClip broadcastClip;
    public AudioClip uiClickClip;
    public AudioClip alarmClip;
    public AudioClip alarm2Clip;
    public AudioClip bangClip;
    public AudioClip pumpClip;

    [HideInInspector] public float buzzerVolume;
    [HideInInspector] public float sirenVolume;
    [HideInInspector] public float broadcastVolume;
    [HideInInspector] public float uiClickVolume;
    [HideInInspector] public float alarmVolume;
    [HideInInspector] public float alarm2Volume;
    [HideInInspector] public float hintVolume;
    [HideInInspector] public float bangVolume;
    [HideInInspector] public float pumpVolume;

    
    bool _buzzerMute = false;
    bool _sirenMute = false;
    bool _broadcastMute = false;
    bool _uiClickMute = false;
    bool _alarmMute = false;
    bool _alarm2Mute = false;
    bool _hintMute = false;
    bool _bangMute = false;
    
    private void Awake()
    {
        Instance = this;
        //LoadVolumeSettings();
        SetDefaultVolume();
    }
    public void Init()
    {
        Instance = this;
    }

    public void PlayBang()
    {
        if(bangSource.isPlaying)
            bangSource.Stop();
        
        bangSource.clip = bangClip;
        bangSource.loop = false;
        bangSource.Play();
    }
    
    // 부저 소리 재생
    public void PlayBuzzer(bool isPlay, bool loop = true)
    {
        if (buzzerSource.isPlaying && isPlay)
            return;
        buzzerSource.clip = buzzerClip;
        buzzerSource.loop = loop;
        buzzerSource.time = 2.1f;
        if(isPlay)
            buzzerSource.Play();
        else
        {
            buzzerSource.Stop();
        }
    }



    // 사이렌 소리 재생
    public void PlaySiren(bool isPlay, bool loop = true)
    {
        
        //isPlay = isPlay && ControlPanel.Instance.SirenCheck();
        
        if (sirenSource.isPlaying && isPlay)
            return;
        sirenSource.clip = sirenClip;
        sirenSource.loop = loop;
        sirenSource.time = 0f;
        if(isPlay)
            sirenSource.Play();
        else
        {
            sirenSource.Stop();
        }
    }



    // 방송 소리 재생
    public void PlayBroadcast(bool isPlay, bool loop = true)
    {
        if (broadcastSource.isPlaying  && isPlay)
            return;
        broadcastSource.clip = broadcastClip;
        broadcastSource.loop = loop;
        broadcastSource.time = 0f;
        if(isPlay)
            broadcastSource.Play();
        else
        {
            broadcastSource.Stop();
        }
        
    }
    


    // UI 클릭 소리 재생
    public void PlayUIClick()
    {
        uiClickSource.clip = uiClickClip;
        uiClickSource.Play();
    }

    public void PlayAlarm(bool isPlay, bool loop = true)
    {
        if (alarmSource.isPlaying && isPlay)
            return;
        alarmSource.clip = alarmClip;
        alarmSource.loop = loop;
        alarmSource.time = 0.2f;
        if(isPlay)
            alarmSource.Play();
        else
        {
            alarmSource.Stop();
        }
    }
    

    public void PlayAlarm2(bool isPlay, bool loop = true)
    {
        if (alarm2Source.isPlaying && isPlay)
            return;
        alarm2Source.clip = alarm2Clip;
        alarm2Source.loop = loop;
        alarm2Source.time = 0.2f;
        if(isPlay)
            alarm2Source.Play();
        else
        {
            alarm2Source.Stop();
        }
    }
    


    #region 펌프음
    // 2024-12-12 SMW 생성
    public void PlayPump(bool isPlay, bool loop = true)
    {
        if (pumpSource == null || pumpClip == null) return;
        if (pumpSource.isPlaying && isPlay)
            return;
        pumpSource.clip = pumpClip;
        pumpSource.loop = loop;
        pumpSource.time = 0.2f;
        if (isPlay)
            pumpSource.Play();
        else
        {
            pumpSource.Stop();
        }
    }

    public void MutePump(bool isMute)
    {
        if (pumpSource == null || pumpClip == null) return;
        pumpSource.mute = isMute;
    }
    #endregion

    public void MuteAll(bool isMute)
    {
        MuteAlarm(isMute);
        MuteAlarm2(isMute);
        MuteBroadcast(isMute);
        MuteBuzzer(isMute);
        MuteSiren(isMute);
        //MuteHint(isMute);
    }
    
    public void MuteAlarm(bool isMute)
    {
        _alarmMute = isMute;
        //alarmSource.volume = _alarmMute ? 0f : PlayerPrefs.GetFloat("AlarmVolume", 1f);
        alarmSource.mute = isMute;
    }
    
    public void MuteAlarm2(bool isMute)
    {
        _alarm2Mute = isMute;
        //alarm2Source.volume = _alarm2Mute ? 0f : PlayerPrefs.GetFloat("Alarm2Volume", 1f);
        alarm2Source.mute = _alarm2Mute;
    }
    
    public void MuteBroadcast(bool isMute)
    {
        _broadcastMute = isMute;
        //broadcastSource.volume = _broadcastMute ? 0f : PlayerPrefs.GetFloat("BroadcastVolume", 1f);
        broadcastSource.mute = _broadcastMute;
    }
    
    public void MuteBuzzer(bool isMute)
    {
        _buzzerMute = isMute;
        //buzzerSource.volume = _buzzerMute ? 0f : PlayerPrefs.GetFloat("BuzzerVolume", 1f);
        buzzerSource.mute = _buzzerMute;
    }
    
    public void MuteSiren(bool isMute)
    {
        if (SceneManager.GetActiveScene().name.Equals("GasSysScene"))
        {
            sirenSource.mute = isMute || GasSysGlobalCanvas.Instance.CheckArea2Open();
        }
        else
        {
            _sirenMute = isMute;
            //sirenSource.volume = _sirenMute ? 0f : PlayerPrefs.GetFloat("SirenVolume", 1f);
            sirenSource.mute = _sirenMute;
        }
    }
    
    public void MuteHint(bool isMute)
    {
        _hintMute = isMute;
        hintSource.mute = _hintMute;
        //hintSource.volume = _hintMute ? 0f : PlayerPrefs.GetFloat("HintVolume", 1f);
    }


    public void PlayHint(AudioClip clip)
    {
        if (null == clip)
            return;
        if (hintSource.isPlaying)
            hintSource.Stop();
        hintSource.clip = clip;
        hintSource.time = 0;
        hintSource.Play();
    }



    public void StopHint()
    {
        hintSource.Stop();
    }

    public void StopAllFireSound()
    {
        buzzerSource.Stop();
        sirenSource.Stop();
        broadcastSource.Stop();
        alarmSource.Stop();
        alarm2Source.Stop();
    }

    public void StopAllFireSound(ref SoundCheck soundCheck)
    {
        soundCheck.buzzer = buzzerSource.isPlaying;
        soundCheck.siren = sirenSource.isPlaying;
        soundCheck.alarm = alarmSource.isPlaying;
        soundCheck.alarm2 = alarm2Source.isPlaying;
        soundCheck.broadcast = broadcastSource.isPlaying;
        buzzerSource.Stop();
        sirenSource.Stop();
        broadcastSource.Stop();
        alarmSource.Stop();
        alarm2Source.Stop();
        StopHint();
    }

    public void ZeroVolume()
    {
        ZeroSaveVolume();
        SetBuzzerVolume(0f);
        SetSirenVolume(0f);
        SetBroadcastVolume(0f);
        //SetUIClickVolume(0f);
        SetAlarmVolume(0f);
        SetAlarm2Volume(0f);
        //SetHintVolume(0f);
        SetBangVolume(0f);
    }

    public void ZeroSaveVolume()
    {
        buzzerVolume = buzzerSource.volume;
        sirenVolume = sirenSource.volume;
        broadcastVolume = broadcastSource.volume;
        //uiClickVolume = uiClickSource.volume;
        alarmVolume = alarmSource.volume;
        alarm2Volume = alarm2Source.volume;
        //hintVolume = hintSource.volume;
        bangVolume = bangSource.volume;
    }

    public void RecoveryVolume()
    {
        buzzerSource.volume = buzzerVolume;
        sirenSource.volume = sirenVolume;
        broadcastSource.volume = broadcastVolume;
        //uiClickSource.volume = uiClickVolume;
        alarmSource.volume = alarmVolume;
        alarm2Source.volume = alarm2Volume;
        //hintSource.volume = hintVolume;
        bangSource.volume = bangVolume;
    }
    
    public void SetDefaultVolume()
    {
        SetBuzzerVolume();
        SetSirenVolume();
        SetBroadcastVolume();
        SetUIClickVolume();
        SetAlarmVolume();
        SetAlarm2Volume();
        SetHintVolume();
        SetBangVolume();
    }

    public void SetBuzzerVolume(float volume = 0.1f)
    {
        buzzerSource.volume = volume;
    }

    public void SetSirenVolume(float volume = 0.5f)
    {
        sirenSource.volume = volume;
    }

    public void SetBangVolume(float volume = 1f)
    {
        bangSource.volume = volume;
    }

    public void SetBroadcastVolume(float volume = 0.5f)
    {
        broadcastSource.volume = volume;
    }

    public void SetUIClickVolume(float volume = 0.5f)
    {
        uiClickSource.volume = volume;
    }

    public void SetAlarmVolume(float volume = 0.5f)
    {
        alarmSource.volume = volume;
    }

    public void SetAlarm2Volume(float volume = 0.2f)
    {
        alarm2Source.volume = volume;
    }

    public void SetHintVolume(float volume = 0.5f)
    {
        hintSource.volume = volume;
    }

    public void PlayAllFireSound(SoundCheck soundCheck)
    {
        PlayAlarm(soundCheck.alarm);
        PlayAlarm2(soundCheck.alarm2);
        PlayBroadcast(soundCheck.broadcast);
        PlaySiren(soundCheck.siren);
        PlayBuzzer(soundCheck.buzzer);
    }
    
    public static float ConvertVolumeToUnity(float volume)
    {
        return Mathf.Clamp(volume / 100f, 0f, 1f);
    }

    // 0~1 범위를 0~100으로 변환하는 함수 (필요하다면 추가)
    public static float ConvertVolumeToUser(float volume)
    {
        return Mathf.Clamp(volume * 100f, 0f, 100f);
    }
    
    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("BuzzerVolume", buzzerSource.volume);
        PlayerPrefs.SetFloat("SirenVolume", sirenSource.volume);
        PlayerPrefs.SetFloat("BroadcastVolume", broadcastSource.volume);
        PlayerPrefs.SetFloat("UIClickVolume", uiClickSource.volume);
        PlayerPrefs.SetFloat("AlarmVolume", alarmSource.volume);
        PlayerPrefs.SetFloat("Alarm2Volume", alarm2Source.volume);
        PlayerPrefs.SetFloat("HintVolume", hintSource.volume);
        PlayerPrefs.SetFloat("BangVolume", bangSource.volume);

        PlayerPrefs.Save(); // 저장
    }
    
    public void LoadVolumeSettings()
    {
        buzzerSource.volume = PlayerPrefs.GetFloat("BuzzerVolume", 1f);
        sirenSource.volume = PlayerPrefs.GetFloat("SirenVolume", 1f);
        broadcastSource.volume = PlayerPrefs.GetFloat("BroadcastVolume", 1f);
        uiClickSource.volume = PlayerPrefs.GetFloat("UIClickVolume", 1f);
        alarmSource.volume = PlayerPrefs.GetFloat("AlarmVolume", 1f);
        alarm2Source.volume = PlayerPrefs.GetFloat("Alarm2Volume", 1f);
        hintSource.volume = PlayerPrefs.GetFloat("HintVolume", 1f);
        bangSource.volume = PlayerPrefs.GetFloat("BangVolume", 1f);
    }
}
