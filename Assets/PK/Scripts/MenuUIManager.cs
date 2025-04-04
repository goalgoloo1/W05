using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    private static MenuUIManager _instance;
    public static MenuUIManager Instance // if there are no Menu UI manager on this game, add
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("MenuUIManager");
                _instance = go.AddComponent<MenuUIManager>();
                DontDestroyOnLoad(go); // Will be conserved when scene changes
            }
            return _instance;
        }
    }

    [SerializeField] RectTransform[] menuFrames;
    [SerializeField] Slider[] soundKnobs;
    [SerializeField] TextMeshProUGUI[] soundPercentage;

    //[SerializeField] GameObject 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
    }

    public void SetValueOfKnob(int index)
    {
        soundPercentage[index].text =
        Math.Round(soundKnobs[index].value * 100) + "%";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
