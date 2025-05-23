using UnityEngine;

public struct SaveConfigsInfos
{
    // Audio.
    public float Volume;
    public float Sfx;
    public float Music;

    // Video.
    public int Resolution;
    public int Quality;
    public bool Vsync;
    public bool ShowFPS;

    // Controls.
    public float NormalSensitivity;
    public float AimSensitivity;
}

public class SaveConfigs : MonoBehaviour
{
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
        // Deleta o objeto se j� houver um SaveGame em cena
        if (saveConfigs == null)
        {
            saveConfigs = this;
        }
        else if (saveConfigs != this)
        {
            print("Procure esses objetos e retire o script SaveConfigs at� sobrar apenas um: " + gameObject.name + ", " + saveConfigs.name);
            Destroy(gameObject);
        }
    }

    public void Save(in SaveConfigsInfos infos)
    {
        // Audio.
        PlayerPrefs.SetFloat("volume", infos.Volume);
        PlayerPrefs.SetFloat("sfx", infos.Sfx);
        PlayerPrefs.SetFloat("music", infos.Music);

        // Video.
        PlayerPrefs.SetInt("resolution", infos.Resolution);
        PlayerPrefs.SetInt("quality", infos.Quality);
        PlayerPrefs.SetInt("vsync", infos.Vsync ? 1 : 0);
        PlayerPrefs.SetInt("showFPS", infos.ShowFPS ? 1 : 0);

        // Controls.
        PlayerPrefs.SetFloat("normalSensitivity", infos.NormalSensitivity);
        PlayerPrefs.SetFloat("aimSensitivity", infos.AimSensitivity);

        PlayerPrefs.Save();
    }

    public SaveConfigsInfos Load()
    {
        SaveConfigsInfos configsInfos = new SaveConfigsInfos();

        // Audio.
        if (PlayerPrefs.HasKey("volume"))
            configsInfos.Volume = PlayerPrefs.GetFloat("volume");

        if (PlayerPrefs.HasKey("sfx"))
            configsInfos.Sfx = PlayerPrefs.GetFloat("sfx");

        if (PlayerPrefs.HasKey("music"))
            configsInfos.Music = PlayerPrefs.GetFloat("music");

        // Video.
        if (PlayerPrefs.HasKey("resolution"))
            configsInfos.Resolution = PlayerPrefs.GetInt("resolution");

        if (PlayerPrefs.HasKey("quality"))
            configsInfos.Quality = PlayerPrefs.GetInt("quality");

        if (PlayerPrefs.HasKey("vsync"))
            configsInfos.Vsync = PlayerPrefs.GetInt("vsync") == 1 ? true : false;

        if (PlayerPrefs.HasKey("showFPS"))
            configsInfos.ShowFPS = PlayerPrefs.GetInt("showFPS") == 1 ? true : false;

        // Controls.
        if (PlayerPrefs.HasKey("normalSensitivity"))
            configsInfos.NormalSensitivity = PlayerPrefs.GetFloat("normalSensitivity");

        if (PlayerPrefs.HasKey("aimSensitivity"))
            configsInfos.AimSensitivity = PlayerPrefs.GetFloat("aimSensitivity");

        return configsInfos;
    }
}
