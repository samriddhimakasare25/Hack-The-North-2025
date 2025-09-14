using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class GameFlow : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text promptText;   // main prompt TMP
    public TMP_Text roundText;    // optional "Round X/5"
    public ChoiceUI choiceUI;     // ChoicesPanel with ChoiceUI

    [Header("Ending UI (optional)")]
    public GameObject resultPanel;
    public TMP_Text resultText;

    const int ROUNDS = 5;
    const int MIN_WIN_MONEY = 14000;

    [System.Serializable]
    public class Question
    {
        public string prompt;
        public List<ChoiceUI.ChoiceData> choices;
    }

    List<Question> questions;
    int qIndex;

    void Start()
    {
        choiceUI.Init(OnChoicePicked);
        BuildQuestions();
        GameStats.Instance.StartNewRun();
        if (resultPanel) resultPanel.SetActive(false);
        ShowQuestion(0);
    }

    void ShowQuestion(int index)
    {
        qIndex = index;
        var q = questions[qIndex];
        if (promptText) promptText.text = q.prompt;
        if (roundText)  roundText.text  = $"Round {qIndex + 1}/{ROUNDS}";
        choiceUI.ShowChoices(q.choices);
    }

    void OnChoicePicked(ChoiceUI.ChoiceData c)
    {
        GameStats.Instance.ApplyOutcome(c.rewardCash, c.damageHp);

        if (GameStats.Instance.IsDead())
        {
            SceneManager.LoadScene("GameOver"); // ensure "GameOver" is in Build Settings
            return;
        }

        int next = qIndex + 1;
        if (next < questions.Count)
        {
            ShowQuestion(next);
        }
        else
        {
            if (GameStats.Instance.Money >= MIN_WIN_MONEY)
                EndRun($"You WIN.\nYou saved your mom.\nFinal: ${GameStats.Instance.Money}, HP {GameStats.Instance.Health}");
            else
                EndRun($"You LOSE.\nNeeded at least ${MIN_WIN_MONEY}.\nFinal: ${GameStats.Instance.Money}, HP {GameStats.Instance.Health}");
        }
    }

    void EndRun(string message)
    {
        choiceUI.Clear();
        if (resultPanel) resultPanel.SetActive(true);
        if (resultText)  resultText.text = message;
        if (promptText)  promptText.text = "Game Over";
        if (roundText)   roundText.text  = "";
    }

    // --- Rounds 1–5 only ---
    void BuildQuestions()
    {
        questions = new List<Question>
        {
            new Question {
                prompt = "Would you rather…",
                choices = new List<ChoiceUI.ChoiceData> {
                    new ChoiceUI.ChoiceData { id="1A", text="Slam your hand in a drawer.",              rewardCash=1200,  damageHp=3  },
                    new ChoiceUI.ChoiceData { id="1B", text="Break your pinky with pliers.",            rewardCash=4500,  damageHp=7  },
                    new ChoiceUI.ChoiceData { id="1C", text="Let a stranger smash a bottle over your head.", rewardCash=6800, damageHp=15 },
                }
            },
            new Question {
                prompt = "Would you rather…",
                choices = new List<ChoiceUI.ChoiceData> {
                    new ChoiceUI.ChoiceData { id="2A", text="Hold your hand over a candle for 5 seconds.", rewardCash=2000, damageHp=4  },
                    new ChoiceUI.ChoiceData { id="2B", text="Step on a bed of nails barefoot.",            rewardCash=3800, damageHp=8  },
                    new ChoiceUI.ChoiceData { id="2C", text="Take a taser shock to the chest.",            rewardCash=3500, damageHp=13 },
                }
            },
            new Question {
                prompt = "Would you rather…",
                choices = new List<ChoiceUI.ChoiceData> {
                    new ChoiceUI.ChoiceData { id="3A", text="Get slapped repeatedly for 30 seconds.",     rewardCash=1500, damageHp=2  },
                    new ChoiceUI.ChoiceData { id="3B", text="Get punched in the ribs until bruised.",     rewardCash=5200, damageHp=9  },
                    new ChoiceUI.ChoiceData { id="3C", text="Break your own nose.",                        rewardCash=9000, damageHp=21 },
                }
            },
            new Question {
                prompt = "Would you rather…",
                choices = new List<ChoiceUI.ChoiceData> {
                    new ChoiceUI.ChoiceData { id="4A", text="Eat a handful of broken glass (crushed fine).", rewardCash=2500, damageHp=5  },
                    new ChoiceUI.ChoiceData { id="4B", text="Let someone burn you lightly with a cigarette.", rewardCash=4000, damageHp=6  },
                    new ChoiceUI.ChoiceData { id="4C", text="Break your kneecap.",                           rewardCash=6000, damageHp=24 },
                }
            },
            new Question {
                prompt = "Would you rather…",
                choices = new List<ChoiceUI.ChoiceData> {
                    new ChoiceUI.ChoiceData { id="5A", text="Punch a wall until your knuckles bleed.",    rewardCash=3000, damageHp=4  },
                    new ChoiceUI.ChoiceData { id="5B", text="Dislocate your shoulder.",                   rewardCash=6200, damageHp=15 },
                    new ChoiceUI.ChoiceData { id="5C", text="Carve into your arm with a knife.",          rewardCash=5500, damageHp=19 },
                }
            },
        };
    }
}
