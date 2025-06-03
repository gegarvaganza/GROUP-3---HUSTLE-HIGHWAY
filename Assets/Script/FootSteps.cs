using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource runLoopSource;     // Looping running footstep sound
    public AudioSource sfxSource;         // One-shot for jump, slide, crouch

    [Header("Footstep Clips")]
    public AudioClip jumpClip;
    public AudioClip slideClip;
    public AudioClip crouchClip;

    void Start()
    {
        // Play running sound on loop from the start
        if (runLoopSource != null && !runLoopSource.isPlaying)
        {
            runLoopSource.loop = true;
            runLoopSource.Play();
        }
    }

    void Update()
    {
        // W = Jump
        if (Input.GetKeyDown(KeyCode.W))
        {
            PlaySFX(jumpClip);
        }

        // A or D = Slide
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            PlaySFX(slideClip);
        }

        // S = Crouch
        if (Input.GetKeyDown(KeyCode.S))
        {
            PlaySFX(crouchClip);
        }
    }

    void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}