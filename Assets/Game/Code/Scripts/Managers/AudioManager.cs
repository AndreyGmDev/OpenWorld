using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System.ComponentModel;

public class AudioManager : MonoBehaviour
{
    [Header("Configurações de Efeitos Sonoros")]
    public AudioSource sfxObject;
    private AudioSource currentSFX = null;

    [Header("Configurações de Música")]
    [SerializeField] AudioClip daytimeMusic;
    [SerializeField] AudioClip nighttimeMusic;
    [SerializeField, Tooltip("Time in Seconds")] float minTimeBetweenMusic = 300f; // 5 minutos
    [SerializeField, Tooltip("Time in Seconds")] float maxTimeBetweenMusic = 900f; // 15 minutos
    [SerializeField] float fadeOutDuration = 5f;
    [SerializeField] float musicVolume = 0.6f;
    [SerializeField] AudioMixerGroup musicMixerGroup;

    private float nextMusicTime;
    private AudioSource musicSource;
    private DaylightCycle daylightCycle;

    private static AudioManager audioManager;

    bool isDay;
    

    public static AudioManager Instance
    {
        get
        {
            if (audioManager == null)
            {
                audioManager = FindFirstObjectByType<AudioManager>();

                if (audioManager == null)
                {
                    if (GameObject.Find("GameManager"))
                    {
                        GameObject obj = GameObject.Find("GameManager");
                        obj.AddComponent<AudioManager>();
                        Debug.Log("Adicionado AudioManager ao GameManager");
                    }
                    else
                    {
                        GameObject obj = new GameObject("GameManager");
                        obj.AddComponent<AudioManager>();
                        Debug.Log("Criado GameManager e adicionado AudioManager");
                    }
                }
            }
            return audioManager;
        }
    }

    private void Awake()
    {
        if (audioManager == null)
        {
            audioManager = this;
        }
        else if (audioManager != this)
        {
            Debug.Log("Encontradas múltiplas instâncias do AudioManager. Destruindo a duplicata em: " + gameObject.name);
            Destroy(gameObject);
        }

        // Setup
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = false;
        musicSource.spatialBlend = 0f; // som 2D
        musicSource.volume = musicVolume;
        if (musicMixerGroup != null)
        {
            musicSource.outputAudioMixerGroup = musicMixerGroup;
        }

        // Procura o DaylightCycle na cena
        daylightCycle = FindFirstObjectByType<DaylightCycle>();
        if (daylightCycle != null)
        {
            ScheduleNextMusic();
        }
    }

    private void Start()
    {
        if (daylightCycle != null)
        {
            isDay = daylightCycle.IsDaytime();
        }
    }


    private void Update()
    {
        if (daylightCycle != null)
        {
            DailyMusic();
        }
    }

    private void DailyMusic()
    {
        if (daylightCycle.IsDaytime())
        {
            if (Time.time >= nextMusicTime && !musicSource.isPlaying)
            {
                PlayDaytimeMusic();
                ScheduleNextMusic();
            }
        }

        if (musicSource.isPlaying)
        {
            if (daylightCycle != null)
            {
                if (daylightCycle.IsDaytime() != isDay)
                {
                    StartCoroutine(FadeOutMusic());
                }

                isDay = daylightCycle.IsDaytime();
            }
        }

        else if (!daylightCycle.IsDaytime())
        {
            if (Time.time >= nextMusicTime && !musicSource.isPlaying)
            {
                PlayNighttimeMusic();
                ScheduleNextMusic();
            }
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume, bool is3D)
    {
        currentSFX = Instantiate(sfxObject, spawnTransform.position, Quaternion.identity);
        currentSFX.clip = audioClip;
        currentSFX.volume = volume;
        currentSFX.spatialBlend = is3D ? 1f : 0f;
        currentSFX.Play();

        float clipLength = currentSFX.clip.length;
        Destroy(currentSFX.gameObject, clipLength);
    }

    public void InterruptSFX()
    {
        if (currentSFX == null) return;
        currentSFX.Stop();
        Destroy(currentSFX.gameObject);
        currentSFX = null;
    }

    private void ScheduleNextMusic()
    {
        nextMusicTime = Time.time + Random.Range(minTimeBetweenMusic, maxTimeBetweenMusic);
    }

    private void PlayDaytimeMusic()
    {
        if (daytimeMusic != null)
        {
            musicSource.clip = daytimeMusic;
            musicSource.Play();
        }
    }

    private void PlayNighttimeMusic()
    {
        if (nighttimeMusic != null)
        {
            musicSource.clip = nighttimeMusic;
            musicSource.Play();
        }
    }

    private IEnumerator FadeOutMusic()
    {
        if (musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;
            float timer = 0;

            while (timer < fadeOutDuration)
            {
                timer += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0, timer / fadeOutDuration);
                yield return null;
            }

            musicSource.Stop();
            musicSource.volume = musicVolume; //Reseta o volume
        }
    }
} 