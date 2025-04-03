using UnityEngine;
using UnityEngine.Playables;

public class CutSceneTrigger : MonoBehaviour
{

    private PlayableDirector _playableDirector;

    private void Start()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }
    private void OnTriggerEnter(Collider other)
    {
        _playableDirector.Play();
    }
}
