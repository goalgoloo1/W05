using UnityEngine;

public class IngameInfoScreen : MonoBehaviour
{
    public static IngameInfoScreen Instance;
    void Start()
    {
        Instance = this;
        Debug.Log("Instance has been loaded");
    }

    // Update is called once per frame
    public void SetVisible(bool value)
    {
        GetComponent<CanvasGroup>().alpha = value ? 1 : 0;
    }
}
