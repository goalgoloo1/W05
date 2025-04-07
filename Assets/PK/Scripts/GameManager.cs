using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private Vector3 resetPos; // If null, starting on the position where the player has been spawnned
    private bool useResetPos; // If null, starting on the position where the player has been spawnned
    private GameObject playerSelf;
    private float elapsedTimeOnStage;

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

        SetPlayerLocation();
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

    internal void SetCheckPoint(Vector3 position)
    {
        useResetPos = true;
        resetPos = position;
        elapsedTimeOnStage = GameInfoManager.Instance.systemTime;
    }

    internal void ResetCheckPoint()
    {
        useResetPos = false;
    }

    internal float GetElapsedTime()
    {
        return useResetPos ? elapsedTimeOnStage : 0f;
    }

    internal void SetPlayerLocation()
    {
        if(useResetPos)
        {
            playerSelf = GameObject.FindGameObjectWithTag("Player");
            if (playerSelf != null)
            {
                StartCoroutine(setPositionPlayer());
            }
            else
            {
                Debug.LogError("There are no players");
            }
        }
    }

    IEnumerator setPositionPlayer()
    {
        yield return new WaitForSeconds(1f);
        playerSelf = GameObject.FindGameObjectWithTag("Player");
        playerSelf.transform.position = resetPos + (Vector3.up * 1.2f);
    }
}
