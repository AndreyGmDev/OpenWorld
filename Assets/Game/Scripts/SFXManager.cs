using UnityEngine;

public class SFXManager : MonoBehaviour
{
   public static SFXManager instance;
   public AudioSource sfxObject;
   private AudioSource current = null;
    private void Awake()
    {
        instance = this;
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
