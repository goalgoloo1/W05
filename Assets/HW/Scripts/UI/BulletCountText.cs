using TMPro;
using UnityEngine;

public class BulletCountText : MonoBehaviour
{
    private TextMeshProUGUI bulletCountText;
    private TextMeshProUGUI bulletCountMaxText;

    private PlayerManager playerManager;

    private void Start()
    {
        bulletCountText = GetComponent<TextMeshProUGUI>();
        bulletCountMaxText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        playerManager = PlayerManager.Instance;
        playerManager.OnChangeBulletCountAction += ChangeBulletText;
    }

    private void ChangeBulletText(int newBulletCount, int maxBulletCount)
    {
        bulletCountText.text = newBulletCount + "";
        bulletCountMaxText.text = "/" + maxBulletCount + "";
    }

    private void OnDestroy()
    {
        playerManager.OnChangeBulletCountAction -= ChangeBulletText;
    }
}
