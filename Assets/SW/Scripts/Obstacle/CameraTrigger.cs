using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public GameObject settingCamera;
    private PlayerController _playerController;
    [SerializeField] private float returnTime;

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _playerController = other.GetComponent<PlayerController>();
            StartTrigger();
        }
    }

    public void StartTrigger()
    {
        StartCoroutine(startTrigger_CO());

    }
    IEnumerator startTrigger_CO()
    {
        settingCamera.SetActive(true);
        _playerController.SetMoveable(true);
        yield return new WaitForSeconds(returnTime);
        _playerController.SetMoveable(false);
        settingCamera.SetActive(false);
        gameObject.SetActive(false);
    }
}
