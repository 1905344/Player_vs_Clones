using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    void Start()
    {
        SoundManager.instance.ChangeMasterVolume(_slider.value);
        SoundManager.instance.ChangeMusicVolume(_musicSlider.value);
        SoundManager.instance.ChangeSFXVolume(_sfxSlider.value);

        _slider.onValueChanged.AddListener(val => SoundManager.instance.ChangeMasterVolume(val));
        _musicSlider.onValueChanged.AddListener(val => SoundManager.instance.ChangeMusicVolume(val));
        _sfxSlider.onValueChanged.AddListener(val => SoundManager.instance.ChangeSFXVolume(val));
    }

}
