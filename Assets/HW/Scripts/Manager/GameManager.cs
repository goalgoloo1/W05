using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => _instance;
    static GameManager _instance;

    public Action<float> OnSystemTimeChangeAction;

    private void Awake()
    {
        _instance = this;
    }

    public float systemTime; //게임시간
    public int currentStage; //스테이지 넘버

    private void Start()
    {
        InitializeProperties();
    }

    private void InitializeProperties()
    {
        systemTime = 0f; //게임 시간 -> 0
    }

    private void Update()
    {
        UpdateTime();
            

    }

    private void UpdateTime()
    {
        systemTime += Time.deltaTime;
        OnSystemTimeChangeAction?.Invoke(systemTime); //게임 시간 업데이트 액션.
    }
}
