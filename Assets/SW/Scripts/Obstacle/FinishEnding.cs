using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class FinishEnding : MonoBehaviour
{
    private Coroutine _end_Co;
    public TMP_Text endText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (_end_Co == null)
            {
                GameManager.Instance.ResetCheckPoint();
                _end_Co = StartCoroutine(End_Co());
            }
        }
    }

    IEnumerator End_Co()
    {
        print("Finish");
        float alpha = 0;
        while (true)
        {
            alpha += Time.deltaTime;

            endText.color = new Color(endText.color.r, endText.color.g, endText.color.b, alpha);

            if (alpha > 225)
            {
                break;
            }
            yield return null;
        }
    }
}
