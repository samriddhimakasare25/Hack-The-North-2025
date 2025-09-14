using UnityEngine;
using System;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance { get; private set; }

    [Header("Starting Values")]
    [SerializeField] int startHealth = 100;
    [SerializeField] int startMoney  = 0;
    [Range(1, 200)] public int maxHealth = 100;

    public int Health { get; private set; }
    public int Money  { get; private set; }

    public event Action<int,int> OnStatsChanged; // (money, health)

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartNewRun();
    }

    public void StartNewRun()
    {
        Health = Mathf.Clamp(startHealth, 0, maxHealth);
        Money  = Mathf.Max(0, startMoney);
        OnStatsChanged?.Invoke(Money, Health);
    }

    public void ApplyOutcome(int rewardCash, int damageHp)
    {
        Money  = Mathf.Max(0, Money + rewardCash);
        Health = Mathf.Clamp(Health - Mathf.Max(0, damageHp), 0, maxHealth);
        OnStatsChanged?.Invoke(Money, Health);
    }

    public bool IsDead() => Health <= 0;
}
