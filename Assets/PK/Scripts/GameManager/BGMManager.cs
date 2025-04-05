using System.Collections;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance; // if there are no Menu UI manager on this game, add
    AudioSource bgmAudioSource;
    private float fadeDuration = 1.5f;
    float volumeValueCur = 1;

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
        volumeValueCur = volumeValue;
    }

    public IEnumerator SetFadeVolume(bool fadeIn)
    {
        float targetValue = fadeIn ? volumeValueCur : 0; // if it is intended to do fade in, set as 0
        float startValue = fadeIn ? 0 : volumeValueCur;

        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            bgmAudioSource.volume = Mathf.Lerp(startValue, targetValue, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }
    }
}
