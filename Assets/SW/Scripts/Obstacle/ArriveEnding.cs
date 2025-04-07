using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ArriveEnding : MonoBehaviour
{
    private Coroutine _end_Co;
    public GameObject endImage1;
    public GameObject endImage2;
    public string sceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (_end_Co== null)
            {
                GameManager.Instance.ResetCheckPoint();
                _end_Co = StartCoroutine(End_Co());
            }
        }
    }
    
    IEnumerator End_Co()
    {
        while (true)
        {
            endImage1.transform.localScale = Vector3.Lerp(endImage1.transform.localScale, Vector3.one * 30, Time.deltaTime * 5);
            endImage2.transform.localScale = Vector3.Lerp(endImage2.transform.localScale, Vector3.one * 30, Time.deltaTime * 5);
            if (Vector3.Distance(endImage1.transform.localScale, Vector3.one * 30) < 0.5f)
            {
                break;
            }
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }
}
