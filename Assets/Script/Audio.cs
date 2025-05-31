using UnityEngine;

public class Audio : MonoBehaviour
{
    [Header("--------------- Audio Source ----------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("--------------- Audio Clip ----------------")]
    public AudioClip background;
    public AudioClip death;
    public AudioClip wallTouch;
    public AudioClip Jump;
    public AudioClip Crouch;
    public AudioClip Run;

    [Header("--------------- Volume ----------------")]
    [Range(0f, 1f)]
    public float volume = 0.5f;

    private void Start()
    {
        musicSource.volume = volume;
        SFXSource.volume = volume;

        musicSource.clip = background;
        musicSource.Play();
    }

    private void Update()
    {
        musicSource.volume = volume;
        SFXSource.volume = volume;
    }
}
