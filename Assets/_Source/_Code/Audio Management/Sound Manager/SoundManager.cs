using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundManager : MonoBehaviour
{
    #region Variables

    public static SoundManager instance;

    [Header("Audio References")]
    [SerializeField] private AudioSource musicSource, effectsSource;
    [SerializeField] private AudioClip sfxClip;

    [Space(10)]

    [Header("Starting Volume")]
    [SerializeField, Range(0.01f, 1f)] private float masterVolumeStartingVolume = 0.7f;
    [SerializeField, Range(0.01f, 1f)] private float musicVolumeStartingVolume = 0.7f;
    [SerializeField, Range(0.01f, 1f)] private float sfxVolumeStartingVolume = 0.7f;

    [Space(10)]

    [Header("Slider U.I. Elements")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Space(10)]

    [Header("Slider Text")]
    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private TMP_Text sfxVolumeText;

    private bool updateMasterVolumeText = false;
    private bool updateMusicVolumeText = false;
    private bool updateSFXVolumeText = false;

    [SerializeField] private bool mainMenu = false;

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

        if (!mainMenu)
        {
            masterVolumeText.maxVisibleCharacters = 4;
            musicVolumeText.maxVisibleCharacters = 4;
            sfxVolumeText.maxVisibleCharacters = 4;

            SetVolume();
            SetSlidersMinOnStart();
            SetSlidersMaxOnStart();
            SetVolumeSliders();
        }
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

    private void SetSlidersMinOnStart()
    {
        masterVolumeSlider.minValue = 0f;
        musicVolumeSlider.minValue = 0f;
        sfxVolumeSlider.minValue = 0f;
    }

    private void SetSlidersMaxOnStart()
    {
        masterVolumeSlider.maxValue = 1f;
        musicVolumeSlider.maxValue = 1f;
        sfxVolumeSlider.maxValue = 1f;
    }

    public void SetVolumeSliders()
    {
        masterVolumeSlider.value = AudioListener.volume;
        musicVolumeSlider.value = musicSource.volume;
        sfxVolumeSlider.value = effectsSource.volume;

        masterVolumeText.text = AudioListener.volume.ToString();
        musicVolumeText.text = musicSource.volume.ToString();
        sfxVolumeText.text = effectsSource.volume.ToString();
    }

    private void SetVolume()
    {
        AudioListener.volume = masterVolumeStartingVolume;
        musicSource.volume = masterVolumeStartingVolume;
        effectsSource.volume = sfxVolumeStartingVolume;
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


