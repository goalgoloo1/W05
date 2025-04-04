using Unity.Cinemachine;
using UnityEngine;

public class StartCart : MonoBehaviour
{
    public GameObject player;
    CinemachineSplineCart _cart;

    void Start()
    {
        _cart = GetComponent<CinemachineSplineCart>();

        // 혹시 CharacterController 있으면 끄기
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
    }

    void Update()
    {
        _cart.SplinePosition += Time.deltaTime * 0.1f;

        // 위치, 회전 따라가게
        player.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
