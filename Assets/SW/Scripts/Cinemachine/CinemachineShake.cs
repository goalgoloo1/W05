using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    private CinemachineCamera _camera;
    private CinemachineBasicMultiChannelPerlin _noise;
    private Coroutine _shakeCoroutine;

    void Start()
    {
        _camera = GetComponent<CinemachineCamera>();
        _noise = _camera?.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        print("Do before3");
    }

    public void ShakeCamera(float intensity, float duration)
    {
        print("Do before");
        if (_noise == null) return;

        // 기존에 쉐이크 중이면 멈추기
        if (_shakeCoroutine != null)
        {
            print("Do before2");
            StopCoroutine(_shakeCoroutine);
        }

        _shakeCoroutine = StartCoroutine(DoShake(intensity, duration));
    }

    private IEnumerator DoShake(float intensity, float duration)
    {
        print("DoShake");
        _noise.AmplitudeGain = intensity;

        yield return new WaitForSeconds(duration);

        _noise.AmplitudeGain = 0f;
        _shakeCoroutine = null;
    }
}
