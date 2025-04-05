using TMPro;
using UnityEngine;

public class GameTimeText : MonoBehaviour
{
    private GameInfoManager gameManager;
    private TextMeshProUGUI gameTimeText;

    private void Start()
    {
        gameTimeText = GetComponent<TextMeshProUGUI>();
        gameManager = GameInfoManager.Instance;
        gameManager.OnSystemTimeChangeAction += ChangeTimeText;
    }

    private void ChangeTimeText(float newValue)
    {
        int minutes = (int)(newValue / 60);
        float remainingSeconds = newValue % 60;
        int seconds = (int)remainingSeconds;
        int deciseconds = (int)((remainingSeconds - seconds) * 100);
        string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", minutes, seconds, deciseconds);
        gameTimeText.text = timeText;
    }
}
