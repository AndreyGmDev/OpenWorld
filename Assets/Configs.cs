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

    private SaveConfigs saveConfigs;

    private void Start()
    {
        saveConfigs = SaveConfigs.instance;

        if (save != null)
        {
            save.onClick.AddListener(saveConfigs.Save);
            save.onClick.AddListener(() => gameObject.SetActive(false));
        }

        if (back != null)
        {
            back.onClick.AddListener(() => gameObject.SetActive(false));
        }
    }

    private void Update()
    {
        // Todo - valor dos Sliders alterarem os volumes(volume, sfx, music) no SoundMixerManager que ainda será criado.
    }

}
