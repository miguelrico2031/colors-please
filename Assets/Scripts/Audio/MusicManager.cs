using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MusicManager : MonoBehaviour, IMusicService
{
    [SerializeField] private AudioClip[] clips;

    [Range(0, 1)] public float volMusic = 1.0f;
    [Range(0, 1)] public float volSounds = 1.0f;

    private readonly Dictionary<string, AudioClip[]> songs = new();
    private AudioSource audioSourceA;
    private AudioSource audioSourceB;
    private AudioSource backgroundSoundSource;
    private bool isPlayingA = true;

    public AudioClip[] currentClips { get; private set; }
    private int currentPhase = 0;
    private const float FadeDuration = 0.4f;
    private float fadeTimer = 0f;
    private bool isTransitioning = false;
    private bool isSongTransitioning = false;
    private bool isBackgroundTransitioning = false;

    private string queuedSongID;
    private int queuedPhase = -1;
    private float queuedStartTime = 0f;

    private readonly List<AudioSource> soundPool = new();
    private const int PoolSize = 10;

    private float lastVolMusic = -1f;
    private float lastVolSounds = -1f;

    private AudioReverbZone reverbZone;

    public static MusicManager Instance;

    protected void Awake()
    {
        /*
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        */

        audioSourceA = CreateAudioSource(true);
        audioSourceB = CreateAudioSource(true);
        backgroundSoundSource = CreateAudioSource(true);


        reverbZone = GetComponent<AudioReverbZone>();
        if (reverbZone == null)
        {
            Debug.LogWarning("No se encontr� una AudioReverbZone en el GameObject.");
        }


        songs["clips"] = clips;

        InitializeSoundPool();
    }


    private void OnValidate()
    {
        volMusic = Mathf.Clamp01(volMusic);
        volSounds = Mathf.Clamp01(volSounds);
        UpdateMusicVolume();
        UpdateSoundVolume();
    }

    private void Update()
    {
        // Detectar cambios en los volumenes
        if (!Mathf.Approximately(volMusic, lastVolMusic))
        {
            lastVolMusic = volMusic;
            UpdateMusicVolume();
        }

        if (!Mathf.Approximately(volSounds, lastVolSounds))
        {
            lastVolSounds = volSounds;
            UpdateSoundVolume();
        }

        // Transiciones de musica
        if (isTransitioning || isSongTransitioning)
        {
            fadeTimer += Time.deltaTime;
            float fadeProgress = Mathf.Clamp01(fadeTimer / FadeDuration);

            AudioSource activeSource = isPlayingA ? audioSourceA : audioSourceB;
            AudioSource newSource = isPlayingA ? audioSourceB : audioSourceA;

            activeSource.volume = Mathf.Lerp(volMusic, 0, fadeProgress);
            newSource.volume = Mathf.Lerp(0, volMusic, fadeProgress);

            if (fadeProgress >= 1f)
            {
                activeSource.Stop();
                EndTransition();
            }
        }

        // Transiciones de background
        if (isBackgroundTransitioning)
        {
            fadeTimer += Time.deltaTime;
            float fadeProgress = Mathf.Clamp01(fadeTimer / FadeDuration);

            backgroundSoundSource.volume = Mathf.Lerp(0, volSounds, fadeProgress);

            if (fadeProgress >= 1f)
            {
                isBackgroundTransitioning = false;
            }
        }
    }

    private void UpdateMusicVolume()
    {
        if (audioSourceA != null) audioSourceA.volume = volMusic;
        if (audioSourceB != null) audioSourceB.volume = volMusic;
        //Debug.Log($"Volumen de m�sica actualizado: {volMusic}");
    }

    private void UpdateSoundVolume()
    {
        foreach (var source in soundPool)
        {
            if (!source.isPlaying) continue;
            source.volume = volSounds;
        }
        if (backgroundSoundSource != null)
        {
            backgroundSoundSource.volume = volSounds;
        }
        //Debug.Log($"Volumen de sonidos actualizado: {volSounds}");
    }

    private AudioSource CreateAudioSource(bool loop)
    {
        var source = gameObject.AddComponent<AudioSource>();
        source.loop = loop;
        source.volume = volMusic;
        return source;
    }

    private void InitializeSoundPool()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            var source = CreateAudioSource(false);
            source.playOnAwake = false;
            source.volume = volSounds;
            source.enabled = false;
            soundPool.Add(source);
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in soundPool)
        {
            if (!source.isPlaying)
            {
                source.enabled = true;
                source.volume = volSounds;
                return source;
            }
        }

        var newSource = CreateAudioSource(false);
        newSource.playOnAwake = false;
        soundPool.Add(newSource);
        return newSource;
    }

    private void EndTransition()
    {
        isTransitioning = false;
        isSongTransitioning = false;
        fadeTimer = 0f;
        isPlayingA = !isPlayingA;

        if (!string.IsNullOrEmpty(queuedSongID))
        {
            SetSong(queuedSongID);
            queuedSongID = null;
        }
        else if (queuedPhase >= 0)
        {
            StartTransition(queuedPhase, queuedStartTime);
            queuedPhase = -1;
        }
    }

    public void PlayBackgroundSound(string soundName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Sonidos/{soundName}");

        if (clip == null)
        {
            Debug.LogError($"El sonido de fondo '{soundName}' no se encontr� en la carpeta Resources/Sonidos.");
            return;
        }

        if (backgroundSoundSource.isPlaying)
        {
            StartBackgroundTransition(clip);
        }
        else
        {
            backgroundSoundSource.clip = clip;
            backgroundSoundSource.loop = true;
            backgroundSoundSource.volume = 0f;
            backgroundSoundSource.Play();
            isBackgroundTransitioning = true;
            fadeTimer = 0f;
        }
    }

    private void StartBackgroundTransition(AudioClip newClip)
    {
        backgroundSoundSource.Stop();
        backgroundSoundSource.clip = newClip;
        backgroundSoundSource.loop = true;
        backgroundSoundSource.volume = 0f;
        backgroundSoundSource.Play();
        isBackgroundTransitioning = true;
        fadeTimer = 0f;
    }

    public void SetPhase(int phase)
    {
        if (isTransitioning || isSongTransitioning || currentClips == null || phase == currentPhase) return;

        if (phase >= 0 && phase < currentClips.Length)
        {
            float currentTime = (isPlayingA ? audioSourceA : audioSourceB).time;
            StartTransition(phase, currentTime);
        }
    }

    public void SetSong(string songID)
    {
        if (isSongTransitioning || isTransitioning)
        {
            queuedSongID = songID;
            return;
        }

        if (!songs.TryGetValue(songID, out var clips))
        {
            Debug.LogError($"La canci�n '{songID}' no existe");
            return;
        }

        currentClips = clips;
        currentPhase = 0;
        PreloadClips(currentClips);
        StartSongTransition(0);
    }

    private void PreloadClips(AudioClip[] clips)
    {
        foreach (var clip in clips)
        {
            if (clip.loadState != AudioDataLoadState.Loaded)
            {
                clip.LoadAudioData();
            }
        }
    }

    private void StartSongTransition(float startTime)
    {
        AudioSource newSource = isPlayingA ? audioSourceB : audioSourceA;

        if (currentClips == null || currentClips.Length == 0)
        {
            Debug.LogError("No hay clips disponibles para la transici�n.");
            return;
        }

        newSource.clip = currentClips[0];
        startTime = Mathf.Clamp(startTime, 0, newSource.clip.length);
        newSource.time = startTime;
        newSource.volume = volMusic;
        newSource.Play();

        isSongTransitioning = true;
        fadeTimer = 0f;
    }

    private void StartTransition(int newPhase, float startTime)
    {
        AudioSource newSource = isPlayingA ? audioSourceB : audioSourceA;

        if (currentClips == null || newPhase < 0 || newPhase >= currentClips.Length)
        {
            Debug.LogError("No hay clips disponibles para la transici�n o el �ndice est� fuera de rango.");
            return;
        }

        newSource.clip = currentClips[newPhase];
        startTime = Mathf.Min(startTime, newSource.clip.length - 0.1f);
        newSource.time = startTime;
        newSource.volume = volMusic;
        newSource.Play();

        isTransitioning = true;
        fadeTimer = 0f;
        currentPhase = newPhase;
    }

    public void PlaySound(string soundName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Sonidos/{soundName}");

        if (clip == null)
        {
            Debug.LogError($"El sonido '{soundName}' no se encontr� en la carpeta Resources/Sonidos.");
            return;
        }

        AudioSource audioSource = GetAvailableAudioSource();
        audioSource.pitch = 1.0f;
        audioSource.clip = clip;
        audioSource.volume = volSounds;
        audioSource.Play();
    }

    public void PlaySoundPitch(string soundName)
    {
        PlaySoundPitch(soundName, 0.15f);
    }


    public void PlaySoundPitch(string soundName, float pitchVariation = 0.1f)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Sonidos/{soundName}");

        if (clip == null)
        {
            Debug.LogError($"El sonido '{soundName}' no se encontr� en la carpeta Resources/Sonidos.");
            return;
        }

        pitchVariation = Mathf.Clamp(pitchVariation, 0f, 1f);

        AudioSource audioSource = GetAvailableAudioSource();
        audioSource.clip = clip;

        float minPitch = 1f - pitchVariation;
        float maxPitch = 1f + pitchVariation;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.volume = volSounds;
        audioSource.Play();
    }


    public void MuteSong()
    {
        if (isPlayingA && audioSourceA.isPlaying)
        {
            audioSourceA.Pause();
        }
        else if (!isPlayingA && audioSourceB.isPlaying)
        {
            audioSourceB.Pause();
        }
    }

    public void ResumeSong()
    {
        if (isPlayingA && audioSourceA.clip != null)
        {
            audioSourceA.UnPause();
        }
        else if (!isPlayingA && audioSourceB.clip != null)
        {
            audioSourceB.UnPause();
        }
    }

    public void MuteBackground()
    {
        if (backgroundSoundSource.isPlaying)
        {
            backgroundSoundSource.Pause();
        }
    }

    public void ResumeBackground()
    {
        if (backgroundSoundSource.clip != null)
        {
            backgroundSoundSource.UnPause();
        }
    }

    public void MenuChange(int newMenuIndex)
    {
        var menuMappings = new Dictionary<int, (int phase, string sound, string soundEffect)>
    {
        { 0, (0, "amb_underwater", "aceptar2") }, // InitialScreenLogIn
        { 1, (0, "amb_underwater", "aceptar2") }, // MainMenu
        { 2, (1, "amb_underwater", "aceptar2") }, // Settings
        { 3, (0, "amb_underwater", "aceptar2") }, // Credits
        { 4, (0, "amb_underwater", "aceptar2") }, // LogOut
        { 5, (2, "amb_underwater", "snd_entershop") }, // Shop
        { 6, (1, "amb_beach", "snd_jugar") }, // PlayMenu
        { 7, (1, "amb_beach", "aceptar2") }, // Lobby
        { 8, (1, "amb_beach", "aceptar2") }, // CharacterSelection
        { 9, (3, "amb_underwater", "snd_enterpve") } // Singleplayer
    };

        if (menuMappings.TryGetValue(newMenuIndex, out var action))
        {
            SetPhase(action.phase);              // Cambia la fase de m�sica
            PlaySound(action.soundEffect);       // Reproduce efecto de sonido
            if (!IsBackgroundSoundPlaying(action.sound))
            {
                PlayBackgroundSound(action.sound); // Reproduce el sonido de fondo
            }

            // Actualiza la AudioReverbZone
            SetReverbZone(action.phase, 5f, 20f); // Configura preset y distancias
        }
        else
        {
            Debug.LogWarning($"El �ndice {newMenuIndex} no tiene acciones definidas.");
        }
    }


    private bool IsBackgroundSoundPlaying(string soundName)
    {
        // Aseg�rate de que el AudioSource no sea nulo y est� reproduciendo algo
        if (backgroundSoundSource != null && backgroundSoundSource.isPlaying)
        {
            // Compara el nombre del clip actual con el sonido solicitado
            return backgroundSoundSource.clip != null && backgroundSoundSource.clip.name == soundName;
        }

        return false; // No est� sonando nada o el AudioSource no est� configurado
    }


    public void SetReverbZone(int phase, float minDistance, float maxDistance)
    {
        if (reverbZone == null)
        {
            Debug.LogError("No se puede configurar la AudioReverbZone porque no est� asignada.");
            return;
        }

        // Ajustar las distancias de la zona
        reverbZone.minDistance = minDistance;
        reverbZone.maxDistance = maxDistance;

        // Usar el preset personalizado para ajustar par�metros manualmente
        reverbZone.reverbPreset = AudioReverbPreset.User;

        //Debug.Log("PHASE: " + phase);

        // Configuraci�n seg�n la fase
        switch (phase)
        {
            case 0: // Fondo del mar (Underwater)
                reverbZone.room = -100; // Reverberaci�n suave y oscura
                reverbZone.roomHF = -2000; // Atenuaci�n intensa de frecuencias altas
                reverbZone.decayTime = 5.0f; // Decaimiento lento para simular profundidad
                reverbZone.decayHFRatio = 0.3f; // Bajas frecuencias dominantes
                reverbZone.reflections = -300; // Reflexiones d�biles
                reverbZone.reverb = 200; // Reverberaci�n posterior moderada
                break;

            case 1: // Playa
                reverbZone.room = -500; // Reverberaci�n ligera
                reverbZone.roomHF = -2000; // Atenuaci�n media de altas frecuencias
                reverbZone.decayTime = 2.5f; // Decaimiento m�s r�pido
                reverbZone.decayHFRatio = 0.6f; // Proporci�n equilibrada
                reverbZone.reflections = -100; // Reflexiones m�s claras
                reverbZone.reverb = 100; // Reverberaci�n posterior ligera
                break;

            case 2: // Cueva
                reverbZone.room = -200; // Reverberaci�n fuerte
                reverbZone.roomHF = -1500; // Atenuaci�n media de altas frecuencias
                reverbZone.decayTime = 3.0f; // Decaimiento prolongado
                reverbZone.decayHFRatio = 0.5f; // Decaimiento equilibrado
                reverbZone.reflections = -100; // Reflexiones audibles pero suaves
                reverbZone.reverb = 150; // Reverberaci�n posterior intensa
                break;

            case 3: // Pozo pve
                reverbZone.room = -300; // Reverberaci�n marcada
                reverbZone.roomHF = -1000; // Atenuaci�n ligera de altas frecuencias
                reverbZone.decayTime = 2.0f; // Decaimiento corto
                reverbZone.decayHFRatio = 0.8f; // Altas frecuencias ligeramente dominantes
                reverbZone.reflections = 100; // Reflexiones claras
                reverbZone.reverb = 400; // Reverberaci�n posterior definida
                break;

            case 4: // Sushi
                reverbZone.room = -1000; // Sin reverberaci�n
                reverbZone.roomHF = -10000; // Sin frecuencias altas
                reverbZone.decayTime = 0.1f; // Decaimiento m�nimo
                reverbZone.decayHFRatio = 0.1f;
                reverbZone.reflections = -10000;
                reverbZone.reverb = -10000;
                break;

            default: // Sin reverberaci�n
                reverbZone.room = -10000; // Sin reverberaci�n
                reverbZone.roomHF = -10000; // Sin frecuencias altas
                reverbZone.decayTime = 0.1f; // Decaimiento m�nimo
                reverbZone.decayHFRatio = 0.1f;
                reverbZone.reflections = -10000;
                reverbZone.reverb = -10000;
                break;
        }

        Debug.Log($"ReverbZone actualizada para la fase {phase}: MinDistance={minDistance}, MaxDistance={maxDistance}");
    }


}
