using Autodesk.Fbx;
using TMPro;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Variables

    public static SoundManager instance;

    [Header("Audio References")]
    [SerializeField] private AudioSource musicSource, effectsSource;
    [SerializeField] private AudioClip sfxClip;

    [Space(10)]

    [Header("Slider Text")]
    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private TMP_Text sfxVolumeText;

    private bool updateMasterVolumeText = false;
    private bool updateMusicVolumeText = false;
    private bool updateSFXVolumeText = false;

    #endregion

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        masterVolumeText.maxVisibleCharacters = 4;
        musicVolumeText.maxVisibleCharacters = 4;
        sfxVolumeText.maxVisibleCharacters = 4;

        SetVolumeSliders();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            Vector3 cameraPos = Camera.main.transform.position;
            effectsSource.PlayOneShot(clip);
        }
    }

    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
        updateMasterVolumeText = true;
    }

    public void ChangeMusicVolume(float value)
    {
        musicSource.volume = value;
        updateMusicVolumeText = true;
    }
    public void ChangeSFXVolume(float value)
    {
        effectsSource.volume = value;
        updateSFXVolumeText = true;
    }

    public void ToggleEffects()
    {
        effectsSource.mute = !effectsSource.mute;
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void TestSFX()
    {
        PlaySFX(sfxClip); 
    }

    public void SetVolumeSliders()
    {
        masterVolumeText.text = AudioListener.volume.ToString();
        musicVolumeText.text = musicSource.volume.ToString();
        sfxVolumeText.text = effectsSource.volume.ToString();
    }

    private void Update()
    {
        if (updateMasterVolumeText)
        {
            masterVolumeText.text = AudioListener.volume.ToString();
            updateMasterVolumeText = false;
        }

        if (updateMusicVolumeText)
        {
            musicVolumeText.text = musicSource.volume.ToString();
            updateMusicVolumeText = false;
        }

        if (updateSFXVolumeText)
        {
            sfxVolumeText.text = effectsSource.volume.ToString();
            updateSFXVolumeText = false;
        }
    }
}


