using UnityEngine.UI;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    private Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(GoToMenu);
    }

    void GoToMenu()
    {
        GameManager.Instance.ResetScene();
    }
}
