using UnityEngine;
using UnityEngine.Audio;

public class MixerManager : MonoBehaviour
{
    private static MixerManager mixerManager;

    public static MixerManager Instance
    {
        get
        {
            if (mixerManager == null)
            {
                mixerManager = FindFirstObjectByType<MixerManager>();

                if (mixerManager == null)
                {
                    if (GameObject.Find("GameManager"))
                    {
                        GameObject obj = GameObject.Find("GameManager");
                        obj.AddComponent<MixerManager>();
                        print("Adicione o Script MixerManager no GameManager");
                    }
                    else
                    {
                        GameObject obj = new GameObject("GameManager");
                        obj.AddComponent<MixerManager>();
                        print("Crie um GameManager e adicione o Script MixerManager no GameManager");
                    }
                }
            }

            return mixerManager;
        }

    }

    private void Awake()
    {
        // Deleta o objeto se já houver um SaveGame em cena
        if (mixerManager == null)
        {
            mixerManager = this;
        }
        else if (mixerManager != this)
        {
            print("Procure esses objetos e retire o script MixerManager até sobrar apenas um: " + gameObject.name + ", " + mixerManager.name);
            Destroy(gameObject);
        }
    }

    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20);
    }
    public void SetSFXVolume(float level)
    {
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(level) * 20);
    }
    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20);
    }
}
