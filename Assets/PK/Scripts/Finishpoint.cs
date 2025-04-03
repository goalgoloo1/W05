using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using System.Collections;

public class Finishpoint : MonoBehaviour
{
    [SerializeField] Scene newScene;

    void OnTriggerEnter(Collider col)
    {
        PlayerController playerController = col.GetComponent<PlayerController>();
        if(playerController)
        {
            Debug.Log("Player has reached finish point, going to " + newScene);
        }

        StartCoroutine(ShowNextScene());
    }

    IEnumerator ShowNextScene()
    {
        Debug.Log("Starting scene transition");
        yield return new WaitForSeconds(4f);
        Debug.Log("Finishing scene transition");
    }
}
