using UnityEngine;
using System.Collections.Generic; // Required for Dictionary

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource musicAudioSource;

    // New: Array to hold all your AudioClips, assign these in the Inspector
    [SerializeField] private SoundClip[] soundClips;

    // New: Dictionary for quick lookup of AudioClips by name
    private Dictionary<string, AudioClip> audioClipDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize the dictionary and populate it from the soundClips array
        audioClipDictionary = new Dictionary<string, AudioClip>();
        foreach (SoundClip sc in soundClips)
        {
            if (sc.clip != null)
            {
                if (!audioClipDictionary.ContainsKey(sc.name))
                {
                    audioClipDictionary.Add(sc.name, sc.clip);
                }
                else
                {
                    Debug.LogWarning($"Duplicate sound clip name '{sc.name}' found in AudioManager. Only the first will be used.");
                }
            }
        }

        if (sfxAudioSource == null)
        {
            sfxAudioSource = GetComponent<AudioSource>();
            if (sfxAudioSource == null)
            {
                Debug.LogError("SFX AudioSource not found or assigned on AudioManager!");
            }
            else
            {
                sfxAudioSource.playOnAwake = false;
            }
        }

        if (musicAudioSource == null)
        {
            AudioSource[] audioSources = GetComponents<AudioSource>();
            if (audioSources.Length > 1)
            {
                musicAudioSource = audioSources[1];
            }
            if (musicAudioSource == null || musicAudioSource == sfxAudioSource)
            {
                Debug.LogError("Music AudioSource not found or assigned on AudioManager!");
            }
            else
            {
                musicAudioSource.playOnAwake = false;
                musicAudioSource.loop = true;
            }
        }
    }

    // Modified: Now takes a string name instead of AudioClip
    public void PlaySound(string clipName, float volume = 1f)
    {
        if (audioClipDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            if (sfxAudioSource != null)
            {
                sfxAudioSource.PlayOneShot(clip, volume);
            }
            else
            {
                Debug.LogWarning($"SFX AudioSource is null. Cannot play sound '{clipName}'.");
            }
        }
        else
        {
            Debug.LogWarning($"Sound clip with name '{clipName}' not found in AudioManager.");
        }
    }

    // Modified: Now takes a string name instead of AudioClip
    public void PlayMusic(string clipName, float volume = 1f, bool loop = true)
    {
        if (audioClipDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            if (musicAudioSource != null)
            {
                musicAudioSource.Stop();
                musicAudioSource.clip = clip;
                musicAudioSource.volume = volume;
                musicAudioSource.loop = loop;
                musicAudioSource.Play();
            }
            else
            {
                Debug.LogWarning($"Music AudioSource is null. Cannot play music '{clipName}'.");
            }
        }
        else
        {
            Debug.LogWarning($"Music clip with name '{clipName}' not found in AudioManager.");
        }
    }

    public void StopMusic()
    {
        if (musicAudioSource != null && musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (musicAudioSource != null && musicAudioSource.isPlaying)
        {
            musicAudioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicAudioSource != null && !musicAudioSource.isPlaying && musicAudioSource.time > 0)
        {
            musicAudioSource.UnPause();
        }
    }

    public void SetSfxVolume(float volume)
    {
        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = volume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = volume;
        }
    }
}

// New: A serializable class to pair a name with an AudioClip in the Inspector
[System.Serializable]
public class SoundClip
{
    public string name;
    public AudioClip clip;
}
