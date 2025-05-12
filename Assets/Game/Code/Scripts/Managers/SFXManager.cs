using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioSource sfxObject;
    private AudioSource current = null;


    private static SFXManager sfxManager;
    public static SFXManager Instance
    {
        get
        {
            if (sfxManager == null)
            {
                sfxManager = FindFirstObjectByType<SFXManager>();

                if (sfxManager == null)
                {
                    if (GameObject.Find("GameManager"))
                    {
                        GameObject obj = GameObject.Find("GameManager");
                        obj.AddComponent<SFXManager>();
                        print("Adicione o Script SFXManager no GameManager");
                    }
                    else
                    {
                        GameObject obj = new GameObject("GameManager");
                        obj.AddComponent<SFXManager>();
                        print("Crie um GameManager e adicione o Script SFXManager no GameManager");
                    }
                }
            }

            return sfxManager;
        }
    }

    private void Awake()
    {
        // Deleta o objeto se já houver um SaveGame em cena
        if (sfxManager == null)
        {
            sfxManager = this;
        }
        else if (sfxManager != this)
        {
            print("Procure esses objetos e retire o script SFXManager até sobrar apenas um: " + gameObject.name + ", " + sfxManager.name);
            Destroy(gameObject);
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        current = Instantiate(sfxObject, spawnTransform.position, Quaternion.identity);

        current.clip = audioClip;

        current.volume = volume;

        current.Play();

        float clipLength = current.clip.length;

        Destroy(current.gameObject, clipLength);
    }
    public void Interrupt()
    {
        if (current == null) return;
        current.Stop();
        Destroy(current.gameObject);
        current = null;
    }
}
