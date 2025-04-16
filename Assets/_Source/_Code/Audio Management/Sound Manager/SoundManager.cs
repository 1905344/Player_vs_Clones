using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioSource musicSource, effectsSource;
    [SerializeField] private AudioClip sfxClip;

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
    }

    public void ChangeMusicVolume(float value)
    {
        musicSource.volume = value;
    }
    public void ChangeSFXVolume(float value)
    {
        effectsSource.volume = value;
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
}


