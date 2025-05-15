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
    [SerializeField] Dropdown ddpResolution; // Valor da resolu��o selecionada, n�o necess�riamente a que est� aplicada no momento.
    private int realResolutionValue; // Valor da resolu��o real. S� � alterado quando o bot�o Apply/Save for pressionado.
    [SerializeField] Dropdown ddpQuality; // Valor da qualidade selecionada.
    [SerializeField] Toggle vsync; // bool para o vsync ativo.
    [SerializeField] Toggle showFPS; // bool para o Show FPS ativo.
    [SerializeField] TextMeshProUGUI fps; // Onde mostra o FPS.

    private List<string> resolutions = new List<string>(); // Lista com todas as resolu��es possiveis a serem selecionadas.
    private List<string> quality = new List<string>(); // Lista com todas as qualidades possiveis a serem selecionadas.

    [Header("Controls Settings")]
    [SerializeField] Slider normalSensitivity; // Sensibilidade do player sem mirar.
    [SerializeField] Slider aimSensitivity; // Sensibilidade do player mirarando.

    // Managers
    private SaveConfigs saveConfigs; // Manager do SaveConfigs.
    private MixerManager mixerManager; // Manager do Mixer.


    private void Awake()
    {
        // Carrega os managers.
        saveConfigs = SaveConfigs.Instance;
        mixerManager = MixerManager.Instance;

        // Configura a resolu��o.
        Resolution[] allResolutions = Screen.resolutions; // Cria um array com todas as resolu��es.
        allResolutions = allResolutions.OrderByDescending(x => x.width).ToArray(); // Inverte a ordem da lista.

        // Formata e adiciona todas as resolu��es na lista.
        foreach (var resolution in allResolutions)
        {
            resolutions.Add(string.Format("{0} X {1}", resolution.width, resolution.height));
        }

        ddpResolution.AddOptions(resolutions); // Adiciona toas as op��es possiveis na interface.

        // Configura a Qualidade.
        quality = QualitySettings.names.ToList();
        quality = quality.OrderByDescending(x => x).ToList();
        ddpQuality.AddOptions(quality);
        ddpQuality.value = QualitySettings.GetQualityLevel();

        // Carrega o save.
        Load();

        // Carrega o video settings.
        ChangeVideoSettings();

        if (fps != null)
        {
            fps.enabled = showFPS.isOn;
        }
    }

    private void OnDisable()
    {
        ddpResolution.value = realResolutionValue;
    }

    private void Start()
    {
        if (save != null)
        {
            save.onClick.AddListener(ChangeVideoSettings);
            save.onClick.AddListener(Save);
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
            volumeSlider.onValueChanged.AddListener((_) => Save());
        }
        
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(mixerManager.SetSFXVolume);
            sfxSlider.onValueChanged.AddListener((_) => Save());
        }
        
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(mixerManager.SetMusicVolume);
            musicSlider.onValueChanged.AddListener((_) => Save());
        }

        // Video.
        if (vsync != null)
        {
            vsync.onValueChanged.AddListener((_) => Save());
        }

        if (showFPS != null)
        {
            if (fps != null)
            {
                showFPS.onValueChanged.AddListener((_) => fps.enabled = showFPS.isOn);
            }
            showFPS.onValueChanged.AddListener((_) => Save());
        }

        // Controls.
        if (normalSensitivity != null)
        {
            normalSensitivity.onValueChanged.AddListener(CameraController.SetNormalSensitivity);
            normalSensitivity.onValueChanged.AddListener((_) => Save());
        }

        if (aimSensitivity != null)
        {
            aimSensitivity.onValueChanged.AddListener(CameraController.SetAimSensitivity);
            aimSensitivity.onValueChanged.AddListener((_) => Save());
        } 
    }

    private void ChangeVideoSettings()
    {
        // Altera a resolu��o verdadeira para a resolu��o selecionada apenas quando o bot�o Apply/Save � pressionado.
        realResolutionValue = ddpResolution.value; 

        // Altera a resolu��o do jogo para a selecionada.
        string[] currentResolution = resolutions[realResolutionValue].Split("X");
        int w = Convert.ToInt32(currentResolution[0].Trim());
        int h = Convert.ToInt32(currentResolution[0].Trim());
        Screen.SetResolution(w, h, true);

        // Vsync
        QualitySettings.vSyncCount = vsync.isOn ? 1 : 0;
       
        //Application.targetFrameRate = 9;
    }

    
    // Chama o c�digo de Save no SaveConfigs e passa as vari�veis.
    private void Save()
    {
        saveConfigs.Save(new SaveConfigsInfos
        {
            // Audio.
            Volume = volumeSlider.value,
            Sfx = sfxSlider.value,
            Music = musicSlider.value,

            // Video.
            Resolution = realResolutionValue,
            Quality = ddpQuality.value,
            Vsync = vsync.isOn,
            ShowFPS = showFPS.isOn,

            // Controls.
            NormalSensitivity = normalSensitivity.value,
            AimSensitivity = aimSensitivity.value,
        });
    }

    // Carrega o Load.
    private void Load()
    {
        SaveConfigsInfos configsInfos = saveConfigs.Load();

        // Audio.
        volumeSlider.value = configsInfos.Volume;
        sfxSlider.value = configsInfos.Sfx;
        musicSlider.value = configsInfos.Music;

        // Video.
        ddpResolution.value = configsInfos.Resolution;
        ddpQuality.value = configsInfos.Quality;
        vsync.isOn = configsInfos.Vsync;
        showFPS.isOn = configsInfos.ShowFPS;

        // Controls.
        normalSensitivity.value = configsInfos.NormalSensitivity;
        aimSensitivity.value = configsInfos.AimSensitivity;
    }
}
