using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextStageBtn : MonoBehaviour
{
    private string targetStageName;
    private Button button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(GoToNextStage);
    }

    public void SetStage(string stageName)
    {
        targetStageName = stageName;
    }

    // Update is called once per frame
    void GoToNextStage()
    {
        Debug.Log("Going to " + targetStageName);
        SceneManager.LoadScene(targetStageName);
    }
}
