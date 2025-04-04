using Unity.Cinemachine;
using UnityEngine;

public class StartCart : MonoBehaviour
{
    public GameObject player;
    CinemachineSplineCart _cart;

    void Start()
    {
        _cart = GetComponent<CinemachineSplineCart>();

        // Ȥ�� CharacterController ������ ����
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
    }

    void Update()
    {
        _cart.SplinePosition += Time.deltaTime * 0.1f;

        // ��ġ, ȸ�� ���󰡰�
        player.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
