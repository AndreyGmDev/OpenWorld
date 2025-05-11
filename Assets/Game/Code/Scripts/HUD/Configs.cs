using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    [SerializeField] Dropdown ddpResolution; // Valor da resolução selecionada, não necessáriamente a que está aplicada no momento.
    private int ddpResolutionRealValue; // Valor da resolução real. Só é alterado quando o botão Apply/Save for pressionado.
    [SerializeField] Dropdown ddpQuality; // Valor da qualidade selecionada.
    [SerializeField] Toggle vsync; // bool para o vsync ativo.

    private List<string> resolutions = new List<string>(); // Lista com todas as resoluções possiveis a serem selecionadas.
    private List<string> quality = new List<string>(); // Lista com todas as qualidades possiveis a serem selecionadas.

    [Header("Controls Settings")]
    [SerializeField] Slider normalSensitivity; // Sensibilidade do player sem mirar.
    [SerializeField] Slider aimSensitivity; // Sensibilidade do player mirarando.

    private SaveConfigs saveConfigs; // Manager do SaveConfigs.
    private MixerManager mixerManager; // Manager do Mixer.

    public TextMeshProUGUI fps;

    private void Awake()
    {
        // Carrega os managers.
        saveConfigs = SaveConfigs.Instance;
        mixerManager = MixerManager.Instance;

        // Configs resolution.
        Resolution[] allResolutions = Screen.resolutions; // Cria um array com todas as resoluções.
        allResolutions = allResolutions.OrderByDescending(x => x.width).ToArray(); // Inverte a ordem da lista.

        // Formata e adiciona todas as resoluções na lista.
        foreach (var resolution in allResolutions)
        {
            resolutions.Add(string.Format("{0} X {1}", resolution.width, resolution.height));
        }

        // Adiciona toas as opções possiveis na interface.
        ddpResolution.AddOptions(resolutions);

        // Qualidade.
        quality = QualitySettings.names.ToList();
        quality = quality.OrderByDescending(x => x).ToList();
        ddpQuality.AddOptions(quality);
        ddpQuality.value = QualitySettings.GetQualityLevel();

        // Carrega o save.
        Load();

        ChangeVideoSettings();
    }

    private void Start()
    {
        if (save != null)
        {
            save.onClick.AddListener(ChangeVideoSettings);
            save.onClick.AddListener(() => Save(0));
            save.onClick.AddListener(() => gameObject.SetActive(false));
        }

        if (back != null)
        {
            back.onClick.AddListener(() => gameObject.SetActive(false));
        }

        // Audio.
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(mixerManager.SetMasterVolume);
            volumeSlider.onValueChanged.AddListener(Save);
        }
        
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(mixerManager.SetSFXVolume);
            sfxSlider.onValueChanged.AddListener(Save);
        }
        
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(mixerManager.SetMusicVolume);
            musicSlider.onValueChanged.AddListener(Save);
        }

        if (normalSensitivity != null)
        {
            normalSensitivity.onValueChanged.AddListener(CameraController.SetNormalSensitivity);
            normalSensitivity.onValueChanged.AddListener(Save);
        }

        if (aimSensitivity != null)
        {
            aimSensitivity.onValueChanged.AddListener(CameraController.SetAimSensitivity);
            aimSensitivity.onValueChanged.AddListener(Save);
        }

        //InvokeRepeating(nameof(ShowFps), 0f, 0.2f);
    }

    private void ShowFps()
    {
        //fps.text = Mathf.Floor(1 / Time.deltaTime).ToString() + " FPS";
    }

    private void ChangeVideoSettings()
    {
        // Resolution.
        string[] currentResolution = resolutions[ddpResolution.value].Split("X");
        int w = Convert.ToInt32(currentResolution[0].Trim());
        int h = Convert.ToInt32(currentResolution[0].Trim());
        Screen.SetResolution(w, h, true);

        ddpResolutionRealValue = ddpResolution.value; // Seta o value da resolução quando o botão Apply/Save é pressionado.

        // Vsync
        QualitySettings.vSyncCount = vsync.isOn ? 1 : 0;
        
        // FPS
        //Application.targetFrameRate = 9;
    }

    
    // Chama o código de Save no SaveConfigs e passa as variáveis.
    private void Save(float doNothing)
    {
        saveConfigs.Save(new SaveConfigsInfos
        {
            // Audio.
            Volume = volumeSlider.value,
            Sfx = sfxSlider.value,
            Music = musicSlider.value,

            // Video.
            Resolution = ddpResolutionRealValue,
            Quality = ddpQuality.value,
            Vsync = vsync.isOn,

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
        vsync.isOn = configsData.vsync;

        // Controls.
        normalSensitivity.value = configsData.normalSensitivity;
        aimSensitivity.value = configsData.aimSensitivity;
    }
}
