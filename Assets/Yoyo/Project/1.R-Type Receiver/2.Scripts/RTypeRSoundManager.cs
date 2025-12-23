using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class RTypeRSoundManager : MonoBehaviour
{
#region singleton

    public static RTypeRSoundManager Instance
    {
        get;
        private set;
    }

#endregion

    [Foldout("오디오 소스")]
    public AudioSource buzzerSource;
    public AudioSource sirenSource;
    public AudioSource broadcastSource;
    public AudioSource uiClickSource;
    public AudioSource alarmSource;
    public AudioSource alarm2Source;
    public AudioSource hintSource;

    [Foldout("오디오 클립")]
    public AudioClip buzzerClip;
    public AudioClip sirenClip;
    public AudioClip broadcastClip;
    public AudioClip uiClickClip;
    public AudioClip alarmClip;
    public AudioClip alarm2Clip;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    public static void Init()
    {

    }

    // 부저 소리 재생
    public void PlayBuzzer(bool isPlay, bool loop = true)
    {
        if (buzzerSource.isPlaying && isPlay)
            return;
        buzzerSource.clip = buzzerClip;
        buzzerSource.loop = loop;
        buzzerSource.time = 2.1f;
        if (isPlay)
            buzzerSource.Play();
        else
        {
            buzzerSource.Stop();
        }
    }

    public void MuteBuzzer(bool isMute)
    {
        buzzerSource.mute = isMute;
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
        if (isPlay)
            sirenSource.Play();
        else
        {
            sirenSource.Stop();
        }
    }

    public void MuteSiren(bool isMute)
    {


        sirenSource.mute = isMute || GasSysGlobalCanvas.Instance.CheckArea2Open();
    }

    // 방송 소리 재생
    public void PlayBroadcast(bool isPlay, bool loop = true)
    {
        if (broadcastSource.isPlaying && isPlay)
            return;
        broadcastSource.clip = broadcastClip;
        broadcastSource.loop = loop;
        broadcastSource.time = 0f;
        if (isPlay)
            broadcastSource.Play();
        else
        {
            broadcastSource.Stop();
        }

    }

    public void MuteBroadcast(bool isMute)
    {
        broadcastSource.mute = isMute;
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
        if (isPlay)
            alarmSource.Play();
        else
        {
            alarmSource.Stop();
        }
    }

    public void MuteAlarm(bool isMute)
    {
        alarmSource.mute = isMute;
    }

    public void PlayAlarm2(bool isPlay, bool loop = true)
    {
        if (alarm2Source.isPlaying && isPlay)
            return;
        alarm2Source.clip = alarm2Clip;
        alarm2Source.loop = loop;
        alarm2Source.time = 0.2f;
        if (isPlay)
            alarm2Source.Play();
        else
        {
            alarm2Source.Stop();
        }
    }

    public void MuteAlarm2(bool isMute)
    {
        alarm2Source.mute = isMute;
    }

    public void MuteAll(bool isMute)
    {
        MuteAlarm(isMute);
        MuteAlarm2(isMute);
        MuteBroadcast(isMute);
        MuteBuzzer(isMute);
        MuteSiren(isMute);
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

    public void MuteHint(bool isMute)
    {
        hintSource.mute = isMute;
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
        StopHint();
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
        // buzzerSource.volume = 0;
        // sirenSource.volume = 0;
        // broadcastSource.volume = 0;
        // uiClickSource.volume = 0;
        // alarmSource.volume = 0;
        // alarm2Source.volume = 0;
        // hintSource.volume = 0;
        // bangSource.volume = 0;
        SetBuzzerVolume(0f);
        SetSirenVolume(0f);
        SetBroadcastVolume(0f);
        SetUIClickVolume(0f);
        SetAlarmVolume(0f);
        SetAlarm2Volume(0f);
        SetHintVolume(0f);
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
    }

    public void SetBuzzerVolume(float volume = 0.047f)
    {
        buzzerSource.volume = volume;
    }

    public void SetSirenVolume(float volume = 0.3f)
    {
        sirenSource.volume = volume;
    }

    // public void SetBangVolume(float volume = 0.3f)
    // {
    //     bangSource.volume = volume;
    // }

    public void SetBroadcastVolume(float volume = 0.3f)
    {
        broadcastSource.volume = volume;
    }

    public void SetUIClickVolume(float volume = 0.3f)
    {
        uiClickSource.volume = volume;
    }

    public void SetAlarmVolume(float volume = 0.3f)
    {
        alarmSource.volume = volume;
    }

    public void SetAlarm2Volume(float volume = 0.3f)
    {
        alarm2Source.volume = volume;
    }

    public void SetHintVolume(float volume = 0.3f)
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
}
