using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance => _instance;
    static PlayerManager _instance;

    private void Awake()
    {
        _instance = this;
    }
    [Space(10)]
    [Header("Bullet/ Shoot gun")]
    /// <summary>
    /// Properties.
    /// </summary>
    public float fireCoolDown = 1f;
    public float fireCoolDownDelta;
    public float reloadingTime = 3f; //fixed reloading waiting time.
    public float reloadingTimeDelta; //processing Time of Reloading.
    public int maxBulletCount;
    public int initialBulletCount = 3; //count of bullet at start of game.
    public int remainingBullet; //Remaining bullet count.
    public float evadeTimeout = 1.5f;
    public float evadeTimeoutDelta;

    /// <summary>
    /// Actions.
    /// </summary>
    public Action<int, int> OnChangeBulletCountAction; //current, maximum
    public Action<float, float> OnChangeFireCoolDownAction; //current, maximum
    public Action<float, float> OnChangeReloadingTimeAction; //current, maximum
    public Action<float, float> OnChangeEvadeCoolDownAction; //maximum - current, maximum

    [Space(10)]
    [Header("References")]
    /// <summary>
    /// References.
    /// </summary>
    [SerializeField] Slider reloadingTimeSlider;
    [SerializeField] Slider fireCoolDownSlider;

    private void Start()
    {
        InitializeProperties();
    }

    private void InitializeProperties()
    {
        //initialBullet Count Settings.
        remainingBullet = initialBulletCount; //initialize count of bullet.
        OnChangeBulletCountAction?.Invoke(remainingBullet, maxBulletCount); //start count update.

        //firecooldown Settings.
        fireCoolDownDelta = fireCoolDown;
        reloadingTimeDelta = 0f;

        ////Slider Settings.
        //reloadingTimeSlider.minValue = 0;
        //reloadingTimeSlider.maxValue = reloadingTime;
    }

    private void Update()
    {
        UpdateFireCoolDown();
        UpdateReloadingCoolDown();
        UpdateEvadeCoolDown();
    }

    private void UpdateEvadeCoolDown()
    {
        if(evadeTimeoutDelta >= 0f)
        {
            evadeTimeoutDelta -= Time.deltaTime;
            OnChangeEvadeCoolDownAction?.Invoke(evadeTimeout - evadeTimeoutDelta, evadeTimeout);
        }
    }

    private void UpdateFireCoolDown()
    {
        if(fireCoolDownDelta >= 0f)
        {
            fireCoolDownDelta -= Time.deltaTime;

            OnChangeFireCoolDownAction?.Invoke(fireCoolDownDelta, fireCoolDown);
        }
 
    }

    private void UpdateReloadingCoolDown()
    {
        if (reloadingTimeDelta >= 0f && !(remainingBullet == maxBulletCount))
        {
            reloadingTimeDelta -= Time.deltaTime;

            OnChangeReloadingTimeAction?.Invoke(reloadingTimeDelta, reloadingTime);
        }
        else
        {
            IncreaseBulletCount(1);
            reloadingTimeDelta = reloadingTime; //reset reloading time.

            OnChangeReloadingTimeAction?.Invoke(reloadingTimeDelta, reloadingTime);
        }
    }

    public void Shot()
    { 
        DecreaseBulletCount(1); //decrease bullet count by 1.

        fireCoolDownDelta = fireCoolDown; //reset time of firerate.
    }



    private void SetBulletCount(int newBulletCount)
    {
        remainingBullet = newBulletCount;
        remainingBullet = Mathf.Clamp(remainingBullet, 0, maxBulletCount);
        OnChangeBulletCountAction?.Invoke(remainingBullet, maxBulletCount);
    }

    private void IncreaseBulletCount(int countToAdd)
    {
        remainingBullet += countToAdd;
        remainingBullet = Mathf.Clamp(remainingBullet, 0, maxBulletCount);
        OnChangeBulletCountAction?.Invoke(remainingBullet, maxBulletCount);
    }

    private void DecreaseBulletCount(int countToDecrease)
    {
        remainingBullet -= countToDecrease;
        remainingBullet = Mathf.Clamp(remainingBullet, 0, maxBulletCount);
        OnChangeBulletCountAction?.Invoke(remainingBullet, maxBulletCount);
    }


}
