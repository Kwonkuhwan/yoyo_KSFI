using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    private static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
        docsSource.volume = 0.5f;
        muteButton.onClick.AddListener(MuteSetting);
    }
    #endregion

    public AudioSource docsSource;
    public AudioSource sfxSource;
    public AudioClip currentDocsClip;
    public AudioClip currentsfxClip;
    public Button muteButton;
    public Image muteButtonImage;
    public Sprite[] volumeButtonSprites;

    public void PlayDocs(AudioClip clip, bool checkDuplication = false)
    {
        if (clip != null)
        {
            //docsSource.Stop();
            //if (currentDocsClip == clip)
            //    return;
            if (checkDuplication && currentDocsClip == clip)
                return;

            currentDocsClip = clip;
            docsSource.clip = currentDocsClip;
            docsSource.Play();
        }
        else
        {
            Debug.LogWarning("null audioClip");
        }
    }

    public void StopDocs()
    {
        currentDocsClip = null;
        docsSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            if (currentsfxClip == clip)
                return;

            currentsfxClip = clip;
            sfxSource.clip = currentsfxClip;
            sfxSource.Play();
        }
        else
        {
            Debug.LogWarning("null audioClip");
        }
    }

    public void StopSFX()
    {
        if (sfxSource.isPlaying)
        {
            currentsfxClip = null;
            sfxSource.Stop();
        }
    }

    private void MuteSetting()
    {
        if (docsSource.volume == 0.5f)
        {
            docsSource.volume = 0f;
            muteButtonImage.sprite = volumeButtonSprites[0];

        }
        else
        {
            docsSource.volume = 0.5f;
            muteButtonImage.sprite = volumeButtonSprites[1];

        }
    }
}
