using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine;

/// <summary>
/// Manager responsible for handling audio sources between scenes
/// 
/// Requires:
/// Resources folder includes:
///     -> GameSounds
///     -> UISounds
///     -> BGM
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance => GameManager.SoundManager;

    [Header("Primary Audio Components")]
    [SerializeField] private AudioMixer _audioMixer = null;
    [SerializeField]
    private AudioSource _voiceSource,  // For card voice lines or narrator
        _gameSoundSource,                               // For in-game sound effects
        _soundEffectSource,                             // For UI and non-diagetic sound effects
        _musicSource;                                   // For background music

    [Header("\nSound Bank")]
    [SerializeField, Tooltip("Sound bank for in-game sounds")]
    private Dictionary<string, AudioClip> GameSoundBank = new Dictionary<string, AudioClip>();

    [SerializeField, Tooltip("Sound bank for UI sounds")]
    private Dictionary<string, AudioClip> UISoundBank = new Dictionary<string, AudioClip>();

    [SerializeField, Tooltip("Sound bank for background music")]
    private Dictionary<string, AudioClip> MusicBank = new Dictionary<string, AudioClip>();

    [SerializeField]
    private GameObject SoundSourcePrefab;
    private GameObject _soundSourceRef;

    // Check whether current music is paused or not
    private bool isPaused = false;

    // Duration for fade in/out effects
    private float fadeDuration = 1.0f;

    public void Awake()
    {

        // Maintain sound sources between scenes
        _soundSourceRef = Instantiate(SoundSourcePrefab);
        AudioSource[] sources = _soundSourceRef.GetComponentsInChildren<AudioSource>();
        _gameSoundSource = sources[0];
        _soundEffectSource = sources[1];
        _musicSource = sources[2];
        _voiceSource = sources[3];

        DontDestroyOnLoad(_soundSourceRef);

        // Load all sound banks from resources
        AudioClip[] clipLoader = null;

        // Game Sounds
        clipLoader = Resources.LoadAll<AudioClip>("Sounds\\GameSounds").ToArray();
        foreach (AudioClip clip in clipLoader)
        {
            GameSoundBank.Add(clip.name, clip);
        }

        // UI Sounds
        clipLoader = Resources.LoadAll<AudioClip>("Sounds\\UISounds").ToArray();
        foreach (AudioClip clip in clipLoader)
        {
            UISoundBank.Add(clip.name, clip);
        }
        // BGM
        clipLoader = Resources.LoadAll<AudioClip>("Sounds\\BGM").ToArray();
        foreach (AudioClip clip in clipLoader)
        {
            MusicBank.Add(clip.name, clip);
        }

        // Voices will be stored on a card-by-card basis
    }

    public void Start()
    {
        if (!_audioMixer)
        {
            Debug.LogError("AudioMixer not assigned");
        }

        // Set volume
        _gameSoundSource.volume = 1f;
        _soundEffectSource.volume = 1f;
        _voiceSource.volume = 1f;
        _musicSource.volume = 0.25f;

        // Setup BGM
        _musicSource.clip = MusicBank["MenuBGM"];
        _musicSource.loop = true;

        // Start the main background music
        StartMusic(MusicBank["MenuBGM"]);
    }

    // ============================================================================================
    // =================================== DIAGETIC SOUNDS ========================================
    // ============================================================================================

    /// <summary>
    /// Access the sound bank of in-game sounds
    /// </summary>
    /// <param name="clipName"> a string with the specific 
    ///     game audio clip's name in the resources folder </param>
    public void PlayGameSound(string clipName)
    {
        if (GameSoundBank.ContainsKey(clipName))
        {
            _gameSoundSource.PlayOneShot(GameSoundBank[clipName]);
        }
        else
        {
            Debug.LogError("AudioClip: " + clipName + " not found");
        }
    }

    /// <summary>
    /// Play a one shot of given audio clip to voice source
    /// </summary>
    /// <param name="audioClip"> Audio clip to play </param>
    public void PlayOnPlayVoiceLine(AudioClip audioClip)
    {
        _voiceSource.PlayOneShot(audioClip);
    }

    // ============================================================================================
    // ================================= NON-DIAGETIC SOUNDS ======================================
    // ============================================================================================

    /// <summary>
    /// Access the sound bank of UI sound effects
    /// </summary>
    /// <param name="clipName"> a string with the specific 
    ///     UI audio clip's name in the resources folder </param>
    public void PlayUISound(string clipName)
    {
        if (UISoundBank.ContainsKey(clipName))
        {
            _soundEffectSource.PlayOneShot(UISoundBank[clipName]);
        }
        else
        {
            Debug.LogError("AudioClip: " + clipName + " not found");
        }
    }

    // ============================================================================================
    // =================================== MUSIC CONTROLLER =======================================
    // ============================================================================================

    public void PlayMusic(string clipName)
    {
        if (MusicBank.ContainsKey(clipName))
        {
            StartMusic(MusicBank[clipName]);
        }
    }

    /// <summary>
    /// Play passed audio clip 
    /// </summary>
    /// <param name="audioClip"></param>
    public void StartMusic(AudioClip audioClip)
    {
        StartCoroutine(StartFade(_musicSource, fadeDuration, 0));
        _musicSource.Stop();
        _musicSource.clip = audioClip;
        _musicSource.Play();
        StartCoroutine(StartFade(_musicSource, fadeDuration, _musicSource.volume));
    }

    /// <summary>
    /// Pause music in buffer
    /// </summary>
    public void PauseMusic()
    {
        if (_musicSource.clip && !isPaused)
        {
            _musicSource.Pause();
            isPaused = true;
        }
    }

    /// <summary>
    /// Unpause current music in buffer
    /// </summary>
    public void UnpauseMusic()
    {
        if (_musicSource.clip && isPaused)
        {
            _musicSource.UnPause();
            isPaused = false;
        }
    }

    public void PlayNextMusicClip()
    {
        //TODO: transition to another song (when we find one less annoying)
    }

    // ============================================================================================
    // ================================= VOLUME CONTROLLERS =======================================
    // ============================================================================================

    public void SetVoiceVolume(float volume)
    {
        _voiceSource.volume = Mathf.Clamp(volume, 0, 1);
    }
    public void SetGameSoundVolume(float volume)
    {
        _gameSoundSource.volume = Mathf.Clamp(volume, 0, 1);
    }
    public void SetSoundEffectVolume(float volume)
    {
        _soundEffectSource.volume = Mathf.Clamp(volume, 0, 1);
    }
    public void SetMusicVolume(float volume)
    {
        _musicSource.volume = Mathf.Clamp(volume, 0, 1);
    }

    // ============================================================================================
    // ================================== HELPER FUNCTIONS ========================================
    // ============================================================================================

    /// <summary>
    /// Audio fade function
    /// </summary>
    /// <param name="audioSource"> Which audio source is being controlled </param>
    /// <param name="duration"> Duration of the fade </param>
    /// <param name="targetVolume"> Target volume to set </param>
    /// <returns></returns>
    // Source: https://johnleonardfrench.com/how-to-fade-audio-in-unity-i-tested-every-method-this-ones-the-best/
    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}

