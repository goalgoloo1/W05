using UnityEngine;

public class FixCameraManager : MonoBehaviour
{
    private GameObject[] Cameras;

    void Start()
    {
        Cameras = new GameObject[transform.childCount];
        for (int i=0;i < transform.childCount;i++)
        {
            Cameras[i] = transform.GetChild(i).gameObject;
        }
        Cameras[0].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
