using TMPro;
using UnityEngine;

public class BulletCountText : MonoBehaviour
{
    private TextMeshProUGUI bulletCountText;
    private TextMeshProUGUI bulletCountMaxText;

    private PlayerManager playerManager;
    private int previousBulletCount;

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

        if(newBulletCount > previousBulletCount) //increase
        {
            ChangeTextColor(Color.blue);
        }
        else if(newBulletCount < previousBulletCount)//decrease
        {
            ChangeTextColor(Color.red);
        }

            previousBulletCount = newBulletCount;
    }

    private void OnDestroy()
    {
        playerManager.OnChangeBulletCountAction -= ChangeBulletText;
    }

    private void ChangeTextColor(Color color)
    {
        bulletCountText.color = color;

        Invoke("RecoverColor", 0.3f);
    }

    private void RecoverColor()
    {
        bulletCountText.color = Color.white;
    }
}
