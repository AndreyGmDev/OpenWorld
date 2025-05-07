using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Configs : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button save;
    [SerializeField] Button back;

    [Header("Volume Settings")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider musicSlider;

    [Header("Video Settings")]
    [SerializeField] Dropdown ddpResolution;
    [SerializeField] Dropdown ddpQuality;
    [SerializeField] bool vsync;

    private List<string> resolutions = new List<string>();
    private List<string> quality = new List<string>();

    [Header("Controls Settings")]
    [SerializeField] Slider normalSensitivity;
    [SerializeField] Slider aimSensitivity;

    private SaveConfigs saveConfigs;
    private MixerManager mixerManager;

    private void Start()
    {
        // Carrega os managers.
        saveConfigs = SaveConfigs.Instance;
        mixerManager = MixerManager.Instance;

        if (save != null)
        {
            save.onClick.AddListener(Save);
            save.onClick.AddListener(() => gameObject.SetActive(false));
        }

        if (back != null)
        {
            back.onClick.AddListener(() => gameObject.SetActive(false));
        }

        // Audio.
        volumeSlider.onValueChanged.AddListener(mixerManager.SetMasterVolume);
        sfxSlider.onValueChanged.AddListener(mixerManager.SetSFXVolume);
        musicSlider.onValueChanged.AddListener(mixerManager.SetMusicVolume);

        // Configs resolution.
        Resolution[] allResolutions = Screen.resolutions; // Cria um array com todas as resoluções.
        allResolutions = allResolutions.OrderByDescending(x  => x.width).ToArray(); // Inverte a ordem da lista.

        // Formata e adiciona todas as resoluções na lista.
        foreach (var resolution in allResolutions)
        {
            resolutions.Add(string.Format("{0} X {1}", resolution.width, resolution.height));
        }

        // Adiciona toas as opções possiveis na interface.
        ddpResolution.AddOptions(resolutions);

        // Qualidade.
        /*quality = QualitySettings.names.ToList();
        quality = quality.OrderByDescending(x => x).ToList();
        ddpQuality.AddOptions(quality);
        ddpQuality.value = QualitySettings.GetQualityLevel();*/

        // Carrega o save.
        Load();
    }

    private void ChangeResolution()
    {
        string[] currentResolution = resolutions[ddpResolution.value].Split("X");
        int w = Convert.ToInt32(currentResolution[0].Trim());
        int h = Convert.ToInt32(currentResolution[0].Trim());
        Screen.SetResolution(w, h, true);
    }
    private void Update()
    {
    }
    // Chama o código de Save no SaveConfigs e passa as variáveis.
    private void Save()
    {
        saveConfigs.Save(new SaveConfigsInfos
        {
            // Audio.
            Volume = volumeSlider.value,
            Sfx = sfxSlider.value,
            Music = musicSlider.value,

            // Video.
            Resolution = ddpResolution.value,
            Quality = ddpQuality.value,
            Vsync = vsync,

            // Controls.
            NormalSensitivity = normalSensitivity.value,
            AimSensitivity = aimSensitivity.value,
        });
    }

    // Carrega o Load.
    private void Load()
    {
        ConfigsData configsData = new ConfigsData();
        configsData = saveConfigs.Load();

        // Audio.
        volumeSlider.value = configsData.volume;
        sfxSlider.value = configsData.sfx;
        musicSlider.value = configsData.music;

        // Video.
        ddpResolution.value = configsData.ddpResolution;
        ddpQuality.value = configsData.ddpQuality;
        vsync = configsData.vsync;

        // Controls.
        normalSensitivity.value = configsData.normalSensitivity;
        aimSensitivity.value = configsData.aimSensitivity;
    }
}
