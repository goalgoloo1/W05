using System;
using UnityEngine;

public class GameInfoManager : MonoBehaviour
{
    public static GameInfoManager Instance => _instance;
    static GameInfoManager _instance;

    public Action<float> OnSystemTimeChangeAction;

    private void Awake()
    {
        _instance = this;

        // Load Ingame UI
    }

    public float systemTime; //���ӽð�
    public int currentStage; //�������� �ѹ�

    private void Start()
    {
        InitializeProperties();
        IngameInfoScreen.Instance = FindAnyObjectByType<IngameInfoScreen>();
    }

    private void InitializeProperties()
    {
        systemTime = GameManager.Instance.GetElapsedTime();
    }

    private void Update()
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        systemTime += Time.deltaTime;
        OnSystemTimeChangeAction?.Invoke(systemTime); //���� �ð� ������Ʈ �׼�.
    }
}
