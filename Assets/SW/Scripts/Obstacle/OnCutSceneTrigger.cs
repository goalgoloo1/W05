using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class OnCutSceneTrigger : MonoBehaviour
{
    private PlayableDirector _playableDirector;
    private PlayerController _playerController;
    private GameObject _cinemachineBar;
    private double _playTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playableDirector = GetComponent<PlayableDirector>();
        _cinemachineBar = FindAnyObjectByType<CinemachineBar>().gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            _playerController = other.GetComponent<PlayerController>();
            _playTime = _playableDirector.playableAsset.duration;
            StartCoroutine(StartTimeline(_playTime));
            _playableDirector.Play();
        }
    }

    IEnumerator StartTimeline(double timing)
    {
        _cinemachineBar.GetComponent<Canvas>().enabled = true;
        _playerController.SetMoveable(true);
        yield return new WaitForSeconds((float)timing);
        _playerController.SetMoveable(false);
        gameObject.SetActive(false);
        _cinemachineBar.GetComponent<Canvas>().enabled = false;
    }
}
