using UnityEngine.UI;
using UnityEngine;

public class RestartButton : MonoBehaviour
{
    private Button button;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ResetScene);
    }

    void ResetScene()
    {
        GameManager.Instance.ResetScene();
    }
}
