using UnityEngine;

public class NewEmptyCSharpScript : MonoBehaviour
{
    public AudioClip clip;
    public AudioSource audioSource;

    void Start()
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is not assigned.");
        }
    }
}
