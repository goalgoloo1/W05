using UnityEngine;

public class IngameMenuComplete : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MenuUIManager.Instance.SetMenuUIComplete(this);
    }

    internal void SetComplete()
    {
        
    }
}
