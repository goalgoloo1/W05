using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance; // if there are no Menu UI manager on this game, add
    AudioSource bgmAudioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        bgmAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void SetVolume(float volumeValue)
    {
        bgmAudioSource.volume = volumeValue;
    }
}
