using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public SettingsData data;

    [SerializeField] private AudioMixer mixer;
    
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle sfxToggle;
    
    [SerializeField] private Toggle enLanguageToggle;
    [SerializeField] private Toggle ruLanguageToggle;
    
    public void Initialize()
    {
        data = SaveLoadSystem<SettingsData>.Load(new ());

        SetSounds();
        SetLanguage();

        Subscribe();
    }

    private void Subscribe()
    {
        musicToggle.onValueChanged.AddListener(v =>
        {
            if(v)
                mixer.SetFloat("MusicVolume", Mathf.Log10(data.MusicValue) * 20);
            else
                mixer.SetFloat("MusicVolume", -80);

            data.IsMusicOn = v;
            SaveLoadSystem<SettingsData>.Save(data);
        });
        musicSlider.onValueChanged.AddListener(value =>
        {
            if(data.IsMusicOn)
                mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
            else
                mixer.SetFloat("MusicVolume", -80);

            data.MusicValue = value;
            SaveLoadSystem<SettingsData>.Save(data);
        });
        sfxToggle.onValueChanged.AddListener(v =>
        {
            if(v)
                mixer.SetFloat("SFXVolume", Mathf.Log10(data.SFXValue) * 20);
            else
                mixer.SetFloat("SFXVolume", -80);
            
            data.IsSFXOn = v;
            SaveLoadSystem<SettingsData>.Save(data);
        });
        sfxSlider.onValueChanged.AddListener(value =>
        {
            if(data.IsSFXOn)
                mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
            else
                mixer.SetFloat("SFXVolume", -80);

            data.SFXValue = value;
            SaveLoadSystem<SettingsData>.Save(data);
        });
        
        ruLanguageToggle.onValueChanged.AddListener(v =>
        {
            if(!v) return;
                
            Locale locale = LocalizationSettings.AvailableLocales.GetLocale("ru-RU");
            LocalizationSettings.SelectedLocale = locale;

            data.Language = "ru-RU";
            SaveLoadSystem<SettingsData>.Save(data);
        });
        
        enLanguageToggle.onValueChanged.AddListener(v =>
        {
            if(!v) return;
                
            Locale locale = LocalizationSettings.AvailableLocales.GetLocale("en");
            LocalizationSettings.SelectedLocale = locale;
            
            data.Language = "en";
            SaveLoadSystem<SettingsData>.Save(data);
        });
    }

    private void SetLanguage()
    {
        if (data.Language == "ru")
            ruLanguageToggle.isOn = true;
        else
            enLanguageToggle.isOn = true;
    }

    private void SetSounds()
    {
        musicSlider.value = data.MusicValue;
        musicToggle.isOn = data.IsMusicOn;
        sfxSlider.value = data.SFXValue;
        sfxToggle.isOn = data.IsSFXOn;
    }

    private void OnDestroy()
    {
        musicToggle.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxToggle.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        
        ruLanguageToggle.onValueChanged.RemoveAllListeners();
        enLanguageToggle.onValueChanged.RemoveAllListeners();
    }
}