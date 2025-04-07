using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance => _instance;
    static CursorManager _instance;

    private void Awake()
    {
        _instance = this;
    }

    public void SetCursorVisible()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetCursorInvisible()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
