using UnityEngine;
using TMPro;
using System.Collections;

public class StatUI : MonoBehaviour
{
    [Header("Refs")]
    public TMP_Text healthText;   // assign top-left TMP
    public TMP_Text moneyText;    // assign top-right TMP

    [Header("Flash")]
    public float flashTime = 0.25f;

    int lastMoney, lastHealth;

    void Start()
    {
        GameStats.Instance.OnStatsChanged += UpdateUI;
        UpdateUI(GameStats.Instance.Money, GameStats.Instance.Health);
    }

    void OnDestroy()
    {
        if (GameStats.Instance != null)
            GameStats.Instance.OnStatsChanged -= UpdateUI;
    }

    void UpdateUI(int money, int health)
    {
        if (healthText) healthText.text = $"HP: {health}";
        if (moneyText)  moneyText.text  = $"${money}";

        if (moneyText && money > lastMoney) StartCoroutine(Flash(moneyText, Color.green));
        if (healthText && health < lastHealth) StartCoroutine(Flash(healthText, Color.red));

        lastMoney = money; lastHealth = health;
    }

    IEnumerator Flash(TMP_Text t, Color c)
    {
        var orig = t.color; t.color = c;
        float x = 0f; while (x < flashTime) { x += Time.unscaledDeltaTime; yield return null; }
        t.color = orig;
    }
}
