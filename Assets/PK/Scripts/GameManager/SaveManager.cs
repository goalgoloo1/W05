using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SaveManager");
                _instance = go.AddComponent<SaveManager>();
                DontDestroyOnLoad(go); // Will be conserved when scene changes
            }
            return _instance;
        }
    }

    private string saveFilePath;
    public PlayerData currentData;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Set path of saving
        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        Debug.Log("Saving in " + Application.persistentDataPath);
        Initialize();
    }

    // When there are no data, initialize
    public void Initialize()
    {
        if (File.Exists(saveFilePath))
        {
            Load();
        }
        else
        {
            currentData = new PlayerData();
            Save();
            Debug.Log("Initialized new save data at: " + saveFilePath);
        }
    }

    // Load saved data
    public void Load()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                currentData = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log("Data loaded: " + JsonUtility.ToJson(currentData));
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load data: " + e.Message);
                currentData = new PlayerData(); // 로드 실패 시 기본값
            }
        }
        
        else
        {
            Debug.LogWarning("Save file not found at: " + saveFilePath);
            currentData = new PlayerData();
        }
    }

    // Save current data
    public void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(currentData, true); // for clean output
            File.WriteAllText(saveFilePath, json);
            Debug.Log("Data saved to: " + saveFilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save data: " + e.Message);
        }
    }

    // For test
    public void SetTestData(string name, int progress)
    {
        currentData.playerName = name;
        currentData.stageProgress = progress;
    }

    // Set new stage save progress
    public void SetProgress(int progress)
    {
        if(progress > currentData.stageProgress){
            currentData.stageProgress = progress;
        }
    }
}