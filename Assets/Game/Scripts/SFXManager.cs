using UnityEngine;

public class SFXManager : MonoBehaviour
{
   public static SFXManager Instance;
    [SerializeField] private AudioSource SFXObject;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(SFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

}
