using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public GameObject settingCamera;
    private GameObject _cinemachineBar;
    private PlayerController _playerController;
    [SerializeField] private float returnTime;

    private void Start()
    {
        _cinemachineBar = FindAnyObjectByType<CinemachineBar>().gameObject;
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
        _cinemachineBar.GetComponent<Canvas>().enabled = true;
        settingCamera.SetActive(true);
        _playerController.SetMoveable(true);
        yield return new WaitForSeconds(returnTime);
        _playerController.SetMoveable(false);
        settingCamera.SetActive(false);
        gameObject.SetActive(false);
        _cinemachineBar.GetComponent<Canvas>().enabled = false;
    }
}
