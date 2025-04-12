using UnityEngine;

/// <summary>
/// Manages audio playback, including music and sound effects, for the game.
/// </summary>
public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the AudioManager.
    /// </summary>
    public static AudioManager Instance;

    /// <summary>
    /// The audio source used for playing background music.
    /// </summary>
    [Header("Audio Sources")]
    public AudioSource musicSource;

    /// <summary>
    /// The audio source used for playing sound effects.
    /// </summary>
    public AudioSource sfxSource;

    /// <summary>
    /// Ensures a single instance of AudioManager exists and persists across scenes.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Plays background music using the specified audio clip and volume.
    /// </summary>
    /// <param name="clip">The audio clip to play as background music.</param>
    /// <param name="volume">The volume level for the music (default is 1.0).</param>
    public void PlayMusic(AudioClip clip, float volume = 1.0f)
    {
        if (musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Stops the currently playing background music.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    /// <summary>
    /// Plays a sound effect using the specified audio clip and volume.
    /// </summary>
    /// <param name="clip">The audio clip to play as a sound effect.</param>
    /// <param name="volume">The volume level for the sound effect (default is 1.0).</param>
    public void PlaySFX(AudioClip clip, float volume = 1.0f)
    {
        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    /// <summary>
    /// Adjusts the overall volume of the game's audio.
    /// </summary>
    /// <param name="volume">The new volume level (range: 0.0 to 1.0).</param>
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
