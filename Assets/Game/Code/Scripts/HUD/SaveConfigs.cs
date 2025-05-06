using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SaveConfigs : MonoBehaviour
{
    [Range(0,1)] public float volume = 0.5f;
    [Range(0,1)] public float sfx = 0.3f;
    [Range(0,1)] public float music = 0.5f;

    private static SaveConfigs saveConfigs;

    public static SaveConfigs Instance
    {
        get
        {
            if (saveConfigs == null)
            {
                saveConfigs = FindFirstObjectByType<SaveConfigs>();

                if (saveConfigs == null)
                {
                    if (GameObject.Find("GameManager"))
                    {
                        GameObject obj = GameObject.Find("GameManager");
                        obj.AddComponent<SaveConfigs>();
                        print("Adicione o Script SaveConfigs no GameManager");
                    }
                    else
                    {
                        GameObject obj = new GameObject("GameManager");
                        obj.AddComponent<SaveConfigs>();
                        print("Crie um GameManager e adicione o Script SaveConfigs no GameManager");
                    }
                }
            }

            return saveConfigs;
        }
        
    }

    private void Awake()
    {
        // Deleta o objeto se já houver um SaveGame em cena
        if (saveConfigs == null)
        {
            saveConfigs = this;
        }
        else if (saveConfigs != this)
        {
            print("Procure esses objetos e retire o script SaveConfigs até sobrar apenas um: " + gameObject.name + ", " + saveConfigs.name);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Load();
    }

    public void Save()
    {
        // Audio.
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("sfx", sfx);
        PlayerPrefs.SetFloat("music", music);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        // Audio.
        PlayerPrefs.GetFloat("volume");
        PlayerPrefs.GetFloat("sfx");
        PlayerPrefs.GetFloat("music");
    }
}
