using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Configurações de Efeitos Sonoros")]
    public AudioSource sfxObject;
    private AudioSource currentSFX = null;

    [Header("Configurações de Música")]
    [SerializeField] AudioClip daytimeMusic;
    [SerializeField] AudioClip nighttimeMusic;
    [SerializeField] AudioClip pauseMusic;
    [SerializeField, Tooltip("Time in Seconds")] float minTimeBetweenMusic = 300f; // 5 minutos
    [SerializeField, Tooltip("Time in Seconds")] float maxTimeBetweenMusic = 900f; // 15 minutos
    [SerializeField] float fadeOutDuration = 5f;
    [SerializeField] float musicVolume = 0.6f;
    [SerializeField] AudioMixerGroup musicMixerGroup;

    [Header("Configurações de Som Ambiente (Vento)")]
    [SerializeField] AudioClip windSound;
    [SerializeField] float windVolume = 0.3f;
    [SerializeField] float windFadeDuration = 2f;
    [SerializeField] float windPlayDuration = 180f; // 3 minutos
    [SerializeField] AudioMixerGroup windMixerGroup;

    private float nextMusicTime;
    private AudioSource musicSource;
    private AudioSource pauseMusicSource;
    private AudioSource windSource;
    private DaylightCycle daylightCycle;
    private Coroutine windCoroutine;

    private bool isDay;
    private bool isPaused = false;

    private static AudioManager audioManager;

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

        // Setup Music Source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = false;
        musicSource.spatialBlend = 0f; // som 2D
        musicSource.volume = musicVolume;
        if (musicMixerGroup != null)
        {
            musicSource.outputAudioMixerGroup = musicMixerGroup;
        }

        // Setup Wind Source
        windSource = gameObject.AddComponent<AudioSource>();
        windSource.playOnAwake = false;
        windSource.loop = true;
        windSource.spatialBlend = 0f; // som 2D
        windSource.volume = 0f; // Começa com volume 0 para fade in
        if (windMixerGroup != null)
        {
            windSource.outputAudioMixerGroup = windMixerGroup;
        }
        if (windSound != null)
        {
            windSource.clip = windSound;
        }

        // Setup Pause Music Source
        pauseMusicSource = gameObject.AddComponent<AudioSource>();
        pauseMusicSource.playOnAwake = false;
        pauseMusicSource.loop = true;
        pauseMusicSource.spatialBlend = 0f; // som 2D
        pauseMusicSource.volume = musicVolume;
        if (musicMixerGroup != null)
        {
            pauseMusicSource.outputAudioMixerGroup = musicMixerGroup;
        }
        if (pauseMusic != null)
        {
            pauseMusicSource.clip = pauseMusic;
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
        if (daylightCycle != null && !isPaused)
        {
            DailyMusic();
            HandleWindSound();
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
        
        // Usa tempo sem escala para destruir o som mesmo quando o jogo está pausado
        StartCoroutine(DestroySFXAfterTime(currentSFX.gameObject, clipLength));
    }

    /// <summary>
    /// Toca um efeito sonoro que funciona corretamente mesmo quando o jogo está pausado
    /// </summary>
    public void PlaySoundFXClipUnscaled(AudioClip audioClip, Transform spawnTransform, float volume, bool is3D)
    {
        AudioSource sfx = Instantiate(sfxObject, spawnTransform.position, Quaternion.identity);
        sfx.clip = audioClip;
        sfx.volume = volume;
        sfx.spatialBlend = is3D ? 1f : 0f;
        sfx.Play();

        float clipLength = sfx.clip.length;
        StartCoroutine(DestroySFXAfterTimeUnscaled(sfx.gameObject, clipLength));
    }

    private IEnumerator DestroySFXAfterTime(GameObject sfxObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (sfxObject != null)
        {
            Destroy(sfxObject);
        }
    }

    private IEnumerator DestroySFXAfterTimeUnscaled(GameObject sfxObject, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (sfxObject != null)
        {
            Destroy(sfxObject);
        }
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

    private void HandleWindSound()
    {
        // Se não há música tocando e o vento não está tocando, inicia o vento
        if (!musicSource.isPlaying && !windSource.isPlaying && windSound != null && windCoroutine == null)
        {
            windCoroutine = StartCoroutine(PlayWindSound());
        }
        // Se a música começou a tocar e o vento está tocando, para o vento
        else if (musicSource.isPlaying && windSource.isPlaying)
        {
            if (windCoroutine != null)
            {
                StopCoroutine(windCoroutine);
                windCoroutine = null;
            }
            StartCoroutine(FadeOutWind());
        }
    }

    private IEnumerator PlayWindSound()
    {
        if (windSound == null) yield break;

        windSource.Play();
        
        // Fade in
        float timer = 0;
        while (timer < windFadeDuration)
        {
            timer += Time.deltaTime;
            windSource.volume = Mathf.Lerp(0, windVolume, timer / windFadeDuration);
            yield return null;
        }
        windSource.volume = windVolume;

        // Toca por 3 minutos
        yield return new WaitForSeconds(windPlayDuration);

        // Fade out
        yield return StartCoroutine(FadeOutWind());
        
        windCoroutine = null;
    }

    private IEnumerator FadeOutWind()
    {
        if (!windSource.isPlaying) yield break;

        float startVolume = windSource.volume;
        float timer = 0;

        while (timer < windFadeDuration)
        {
            timer += Time.deltaTime;
            windSource.volume = Mathf.Lerp(startVolume, 0, timer / windFadeDuration);
            yield return null;
        }

        windSource.volume = 0;
        windSource.Stop();
    }

    /// <summary>
    /// Interrompe o som do vento imediatamente
    /// </summary>
    public void InterruptWind()
    {
        if (windCoroutine != null)
        {
            StopCoroutine(windCoroutine);
            windCoroutine = null;
        }
        
        if (windSource.isPlaying)
        {
            windSource.Stop();
            windSource.volume = 0;
        }
    }

    /// <summary>
    /// Controla o estado de pausa do AudioManager
    /// </summary>
    /// <param name="paused">True para pausar, false para despausar</param>
    public void SetPauseState(bool paused)
    {
        isPaused = paused;
        
        if (paused)
        {
            // Interrompe o vento quando pausar
            InterruptWind();
            
            // Toca música de pausa se disponível
            if (pauseMusic != null && pauseMusicSource != null)
            {
                pauseMusicSource.Play();
            }
        }
        else
        {
            // Para a música de pausa quando despausar
            if (pauseMusicSource != null && pauseMusicSource.isPlaying)
            {
                pauseMusicSource.Stop();
            }
        }
    }
} 