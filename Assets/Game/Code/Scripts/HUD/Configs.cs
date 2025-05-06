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
    [SerializeField] Toggle fullScreen;

    private List<string> resolutions = new List<string>();
    private List<string> quality = new List<string>();

    [Header("Controls Settings")]
    [SerializeField] float normalSensitivity;
    [SerializeField] float aimSensitivity;

    private SaveConfigs saveConfigs;
    private MixerManager mixerManager;

    private void Start()
    {
        saveConfigs = SaveConfigs.Instance;
        mixerManager = MixerManager.Instance;
        
        if (save != null)
        {
            save.onClick.AddListener(saveConfigs.Save);
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
        ddpResolution.value = 0;

        // Qualidade.
        /*quality = QualitySettings.names.ToList();
        quality = quality.OrderByDescending(x => x).ToList();
        ddpQuality.AddOptions(quality);
        ddpQuality.value = QualitySettings.GetQualityLevel();*/

        /*var teste = Screen.resolutions.ToList();*/

    }

    private void Update()
    {
        
    }

    private void ChangeResolution()
    {
        string[] currentResolution = resolutions[ddpResolution.value].Split("X");
        int w = Convert.ToInt32(currentResolution[0].Trim());
        int h = Convert.ToInt32(currentResolution[0].Trim());
        Screen.SetResolution(w, h, true);
    }

}
