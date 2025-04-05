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

    public float systemTime; //���ӽð�
    public int currentStage; //�������� �ѹ�

    private void Start()
    {
        InitializeProperties();
    }

    private void InitializeProperties()
    {
        systemTime = 0f; //���� �ð� -> 0
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
