using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // if there are no Menu UI manager on this game, add

    [SerializeField] AudioClipPackType audioClipPack;
    public AudioSource mainAudioSource;

    void Start()
    {
        Instance = this;
        mainAudioSource = GetComponent<AudioSource>();
    }

    public void PlayClip(AudioSource source, string audioClipCode)
    {
        if(source == null)
        {
            mainAudioSource.PlayOneShot(audioClipPack.GetClip(audioClipCode));
        }
        else
        {
            source.PlayOneShot(audioClipPack.GetClip(audioClipCode));
        }
    }

    public void SetVolume(float volumeValue)
    {
        mainAudioSource.volume = volumeValue;
    }
}
