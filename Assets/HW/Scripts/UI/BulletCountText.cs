using TMPro;
using UnityEngine;

public class BulletCountText : MonoBehaviour
{
    private TextMeshProUGUI bulletCountText;
    private PlayerManager playerManager;

    private void Start()
    {
        bulletCountText = GetComponent<TextMeshProUGUI>();
        playerManager = PlayerManager.Instance;
        playerManager.OnChangeBulletCountAction += ChangeBulletText;
    }

    private void ChangeBulletText(int newBulletCount, int maxBulletCount)
    {
        bulletCountText.text = "Bullet : " + newBulletCount + "/" + maxBulletCount;
    }


}
