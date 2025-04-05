using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using System.Collections;

public class LevelSelectBtn : MonoBehaviour
{
    private string stageToGo;
    private int stageIndex;
    private Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void DefineLevel(string stageTarget, int stageIndexInput)
    {
        stageToGo = stageTarget;
        stageIndex = stageIndexInput;

        button = GetComponent<Button>();
        button.onClick.AddListener(GoToStage);
        
        if(SaveManager.Instance.currentData.stageProgress < stageIndex)
        {
            Debug.Log("Stage has been locked.");
            LockStage();
        }
    }

    public void LockStage()
    {
        button.interactable = false;
        transform.GetComponentInChildren<TextMeshProUGUI>().color = button.colors.disabledColor;
    }

    // Update is called once per frame
    void GoToStage()
    {
        StartCoroutine(GoToStageCo());
    }

    IEnumerator GoToStageCo()
    {
        AudioManager.Instance.PlayClip(null, "UI_Confirm");
        StartCoroutine(MenuUIManager.Instance.SetFadeImage(false));

        yield return new WaitForSeconds(1.6f);
        Debug.Log("Going to stage: " + stageToGo);

        // Check integrity of scene
        SceneManager.LoadScene(stageToGo);
    }
}
