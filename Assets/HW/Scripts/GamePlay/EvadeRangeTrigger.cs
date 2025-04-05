using UnityEngine;

public class EvadeRangeTrigger : MonoBehaviour
{
    private PlayerController player;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("EvadeRange Trigger Enter: " + other.gameObject.name);
        if (other.CompareTag("Bullet") && !player.evadeSuccess)
        {
            player.evadeSuccess = true;
            player.OnEvadeSuccess(other.gameObject);
        }
    }
}