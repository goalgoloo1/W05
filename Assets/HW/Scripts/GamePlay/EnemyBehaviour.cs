using Unity.Mathematics;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private void OnDestroy()
    {
        Instantiate((GameObject)Resources.Load("HW/EnemyDeathParticle"), transform.position, quaternion.identity);
    }
}
