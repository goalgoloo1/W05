using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Vertical");
        float y = Input.GetAxis("Horizontal");

        transform.position += (Vector3.right * y + Vector3.forward * x).normalized * Time.deltaTime * 3;
    }
}
