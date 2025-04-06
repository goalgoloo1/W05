using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    private static MenuUIManager _instance;
    public static MenuUIManager Instance;

    [SerializeField] RectTransform[] menuFrames;
    [SerializeField] Slider[] soundKnobs;
    [SerializeField] TextMeshProUGUI[] soundPercentage;
    [SerializeField] LevelSelectGroup levelSelectGroup;

    [Header("Fade Frames")]
    [SerializeField] GameObject fadeFrames;
    private float fadeDuration = 1f;

    [Header("Ingame Menu")]
    [SerializeField] IngameMenuGameover ingameMenuGameover;
    [SerializeField] IngameMenuComplete ingameMenuComplete;

    void Start()
    {
        Instance = this;
    }

    // Show the menu content of Scene
    public void ShowStageSelect(int index)
    {
        for(int i = 0; i < menuFrames.Length; i++)
        {
            if(menuFrames[i] != null)
            {
                menuFrames[i].gameObject.SetActive(
                    index == i
                );
            }
        }

        AudioManager.Instance.PlayClip(null, "UI_Default");
    }

    // Let Select group to add its content based on gamemanager Content
    public void InitStageSelectGroup(List<string> stages)
    {
        levelSelectGroup.Initialize(stages);
    }

    public void SetValueOfKnob(int index)
    {
        soundPercentage[index].text =
        Math.Round(soundKnobs[index].value * 100) + "%";

        switch (index)
        {
            case 0:
                BGMManager.Instance.SetVolume(soundKnobs[index].value);
                break;
            case 1:
                AudioManager.Instance.SetVolume(soundKnobs[index].value);
                break;
            default:
                Debug.LogError("There are no valid value of it.");
                break;
        }
    }

    public IEnumerator SetFadeImage(bool fadeIn)
    {
        if (fadeFrames == null) yield break;

        if (!fadeIn)
        {
            fadeFrames.SetActive(true);
        }

        float targetAlpha = fadeIn ? 0 : 1; // if it is intended to do fade in, set as 0
        float startAlpha = fadeIn ? 1 : 0;

        Image fadeImage = fadeFrames.GetComponent<Image>();
        Color imageColor = fadeImage.color;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            imageColor.a = alpha;
            fadeImage.color = imageColor;
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 최종 색상 설정
        imageColor.a = targetAlpha;
        fadeImage.color = imageColor;


        // Fade 완료 후 비활성화 (페이드 인일 경우)
        if (fadeIn)
        {
            fadeFrames.gameObject.SetActive(false);
        }
    }

    internal void SetMenuUIGameover(IngameMenuGameover menu)
    {
        ingameMenuGameover = menu;
    }

    internal void ShowMenuUIGameover()
    {
        ingameMenuGameover.SetGameover();
    }

    internal void SetMenuUIComplete(IngameMenuComplete menu)
    {
        ingameMenuComplete = menu;
    }
}
