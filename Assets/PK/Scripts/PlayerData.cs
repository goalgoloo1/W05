using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int stageProgress;

    // 기본값 설정 (선택적)
    public PlayerData()
    {
        playerName = "Player";
        stageProgress = 0;
    }
}