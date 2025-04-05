using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class Liftingforward : MonoBehaviour
{

    private GameObject _player;
    private Transform[] _targetPos;
    private CinemachineImpulseSource _impulseSource;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        _targetPos = new Transform[2];
        for (int i = 0; i < 2; i++)
        {
            _targetPos[i] = transform.GetChild(i);
        }
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // 플레이어가 가까이와서 줄에 걸렸을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _player = other.gameObject;
            StartCoroutine(PlayerUp_CO());
        }
    }

    // 플레이어를 정해진 위치로 올려주는 코루틴
    IEnumerator PlayerUp_CO()
    {
        float initGravity = _player.GetComponent<PlayerController>().Gravity;
        _player.transform.forward = Vector3.back;
        _player.GetComponent<PlayerController>().Gravity = 0;
        _player.GetComponent<PlayerController>().SetMoveable(true);

        // 플레이어 움직임 막기

        // 3의 위치로 이동 시키기
        //Vector3 target1Dir = (_targetPos[0].position - _player.transform.position).normalized;

        //while (true)
        //{
        //    _player.transform.position += target1Dir * Time.deltaTime * 50;
        //    if (Vector3.Distance(_player.transform.position, _targetPos[0].position) < 0.1)
        //    {
        //        break;
        //    }
        //    yield return null;
        //}
        print("Forward Lift");

        _player.transform.position = _targetPos[0].position;

        Vector3 target2Dir = (_targetPos[1].position - _targetPos[0].position).normalized;

        _impulseSource.GenerateImpulse();

        while (true)
        {
            _player.transform.position += target2Dir * Time.deltaTime * 10;
            if (_targetPos[1].position.z - _player.transform.position.z > 0.1)
            {
                _impulseSource.GenerateImpulse();
                _player.transform.position = _targetPos[1].position + Vector3.forward;
                break;
            }
            yield return null;
        }

        _player.GetComponent<PlayerController>().Gravity = initGravity;
        _player.GetComponent<PlayerController>().SetMoveable(false);
    }
}
