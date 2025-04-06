using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                _instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go); // Will be conserved when scene changes
            }
            return _instance;
        }
    }

    [SerializeField] SaveManager saveManager;
    [SerializeField] AudioManager audioManager;
    [SerializeField] MenuUIManager menuUIManager;
    [SerializeField] List<string> stageTitle;
    private string menuScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeManagers();

        menuScene = SceneManager.GetActiveScene().name;
    }

    private void InitializeManagers()
    {
        saveManager = GameObject.Find("ManagerComposite").GetComponent<SaveManager>();
        audioManager = GameObject.Find("ManagerComposite").GetComponent<AudioManager>();
        menuUIManager = GameObject.Find("ManagerComposite").GetComponent<MenuUIManager>();

        menuUIManager.InitStageSelectGroup(stageTitle);
    }

    public void GoToStage(string stageToGo)
    {
        if (IsSceneInBuild(stageToGo))
        {
            SceneManager.LoadScene(stageToGo);
        }
        else
        {
            Debug.LogWarning("Scene " + stageToGo + " is not in the build settings. Restarting scene...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        StartCoroutine(menuScene);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuScene);
    }

    bool IsSceneInBuild(string sceneName)
    {
        // Get the list of scenes in the build settings
        var scenes = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < scenes; i++)
        {
            // Get the scene path and check if it contains the scene name
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneFileName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneFileName == sceneName)
            {
                return true; // Scene exists in build settings
            }
        }
        return false; // Scene not found
    } 
}
