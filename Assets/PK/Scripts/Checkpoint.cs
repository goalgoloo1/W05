using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static List<Checkpoint> checkpoints;
    
    void Start()
    {
        checkpoints.Add(this);
        Debug.Log("Currently this scene has " + checkpoints.Count + " checkpoints");
    }

    void OnTriggerEnter(Collider col)
    {
        PlayerController playerController = col.GetComponent<PlayerController>();
        if(playerController)
        {
            Debug.Log("Player has reached checkpoint");
            GameManager.Instance.SetCheckPoint(transform.position);
        }        
    }
}
