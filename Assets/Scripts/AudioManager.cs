using UnityEngine;
using System.Collections.Generic;

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
    /// The list of available music tracks.
    /// </summary>
    [Header("Music Tracks")]
    public List<AudioClip> musicTracks;

    /// <summary>
    /// Sound effects for different card types.
    /// </summary>
    [Header("Card Type Sound Effects")]
    public AudioClip monsterSound;
    public AudioClip weaponSound;
    public AudioClip healingPotionSound;

    /// <summary>
    /// Sound effects for different actions.
    /// </summary>
    [Header("Action Sound Effects")]
    public AudioClip runSound; // Add a sound effect for running

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

        // Play random music on awake
        PlayRandomMusic();
    }

    /// <summary>
    /// Continuously plays random music when the current track finishes.
    /// </summary>
    private void Update()
    {
        if (musicSource != null && !musicSource.isPlaying && musicSource.clip != null)
        {
            PlayRandomMusic();
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

    /// <summary>
    /// Plays a random song from the list of available music tracks.
    /// </summary>
    public void PlayRandomMusic(float volume = 1.0f)
    {
        if (musicTracks != null && musicTracks.Count > 0)
        {
            // Select a random track
            int randomIndex = Random.Range(0, musicTracks.Count);
            AudioClip randomTrack = musicTracks[randomIndex];

            // Play the selected track
            PlayMusic(randomTrack, volume);
        }
        else
        {
            Debug.LogWarning("No music tracks available to play.");
        }
    }

    /// <summary>
    /// Plays a sound effect based on the card type.
    /// </summary>
    /// <param name="cardType">The type of the card.</param>
    public void PlayCardTypeSound(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Monster:
                PlaySFX(monsterSound);
                break;
            case CardType.Weapon:
                PlaySFX(weaponSound);
                break;
            case CardType.HealingPotion:
                PlaySFX(healingPotionSound);
                break;
            default:
                Debug.LogWarning("No sound effect assigned for this card type.");
                break;
        }
    }
}
