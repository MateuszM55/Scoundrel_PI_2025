using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        // Ensure a single instance of AudioManager
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

    // Play background music
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

    // Stop background music
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    // Play a sound effect
    public void PlaySFX(AudioClip clip, float volume = 1.0f)
    {
        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    // Adjust the overall volume
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
