using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public ChoiceUI choiceUI;

    void Start()
    {
        choiceUI.Init(OnChoicePicked);
        ShowRound1();
    }

    void ShowRound1()
    {
        var choices = new List<ChoiceUI.ChoiceData> {
            new ChoiceUI.ChoiceData { id="onion", text="Eat a raw onion.", rewardCash=10, damageHp=0 },
            new ChoiceUI.ChoiceData { id="breath", text="Hold your breath for 2 minutes.", rewardCash=200, damageHp=10 },
            new ChoiceUI.ChoiceData { id="steal", text="Steal from a store.", rewardCash=500, damageHp=5 },
        };

        choiceUI.ShowChoices(choices);
    }

    void OnChoicePicked(ChoiceUI.ChoiceData choice)
    {
        GameStats.Instance.ApplyOutcome(choice.rewardCash, choice.damageHp);

        if (GameStats.Instance.IsDead())
        {
            Debug.Log("Game Over! You died chasing money.");
            // SceneManager.LoadScene("GameOver");
        }
        else
        {
            Debug.Log($"Chose {choice.id}. Money=${GameStats.Instance.Money}, HP={GameStats.Instance.Health}");
            // Load next round, or branch story here
        }
    }
}
