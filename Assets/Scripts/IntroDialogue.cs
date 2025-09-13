using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroClickPerLineMusic : MonoBehaviour
{
    [Header("Refs")]
    public TypewriterTMP typewriter;      
    public AudioSource musicSource;       // background music (looping)

    [Header("Lines")]
    [TextArea] public string line1 = "You look so down… You know what will cheer you up? A GAME!";
    [TextArea] public string line2 = "What are you willing to do for some sweet, sweet cash?!";
    [TextArea] public string line3 = "Do you want to play?";

    [Header("Continue Prompt")]
    [TextArea] public string continuePrompt = "Click to continue…";

    [Header("Next Scene")]
    public string nextSceneName = "";
    public float postClickDelay = 0.15f;

    private enum Phase { Line1, Prompt1, Line2, Prompt2, Line3, Prompt3, Done }
    private Phase phase = Phase.Line1;
    private bool waitingForClick = false;

    private string accumulatedText = "";   // store all text typed so far

    void Start()
    {
        if (!typewriter) { Debug.LogError("TypewriterTMP ref not set."); return; }
        typewriter.OnFinished.AddListener(OnTypeFinished);

        accumulatedText = "";
        PlayCurrentPhase();
        PlayMusic();
    }

    void OnDestroy()
    {
        if (typewriter) typewriter.OnFinished.RemoveListener(OnTypeFinished);
    }

    void Update()
    {
        bool click = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space);
        if (!click) return;

        // If typing, skip to full text
        if (typewriter && typewriter.IsTyping)
        {
            typewriter.Skip();
            return;
        }

        // If waiting for click after prompt
        if (waitingForClick)
        {
            waitingForClick = false;
            AdvancePhase();
        }
    }

    private void OnTypeFinished()
    {
        switch (phase)
        {
            case Phase.Line1:
                phase = Phase.Prompt1;
                PlayCurrentPhase();
                break;

            case Phase.Prompt1:
                waitingForClick = true;
                StopMusic();
                break;

            case Phase.Line2:
                phase = Phase.Prompt2;
                PlayCurrentPhase();
                break;

            case Phase.Prompt2:
                waitingForClick = true;
                StopMusic();
                break;

            case Phase.Line3:
                phase = Phase.Prompt3;
                PlayCurrentPhase();
                break;

            case Phase.Prompt3:
                waitingForClick = true;
                StopMusic();
                break;
        }
    }

    private void AdvancePhase()
    {
        switch (phase)
        {
            case Phase.Prompt1:
                phase = Phase.Line2;
                PlayCurrentPhase();
                PlayMusic();
                break;

            case Phase.Prompt2:
                phase = Phase.Line3;
                PlayCurrentPhase();
                PlayMusic();
                break;

            case Phase.Prompt3:
                phase = Phase.Done;
                Invoke(nameof(StartGame), postClickDelay);
                break;
        }
    }

    private void PlayCurrentPhase()
    {
        switch (phase)
        {
            case Phase.Line1:
                accumulatedText += line1;
                typewriter.Play(accumulatedText);
                break;

            case Phase.Prompt1:
                accumulatedText += "\n" + continuePrompt;
                typewriter.Play(accumulatedText);
                break;

            case Phase.Line2:
                accumulatedText += "\n\n" + line2;
                typewriter.Play(accumulatedText);
                break;

            case Phase.Prompt2:
                accumulatedText += "\n" + continuePrompt;
                typewriter.Play(accumulatedText);
                break;

            case Phase.Line3:
                accumulatedText += "\n\n" + line3;
                typewriter.Play(accumulatedText);
                break;

            case Phase.Prompt3:
                accumulatedText += "\n" + continuePrompt;
                typewriter.Play(accumulatedText);
                break;
        }
    }

    private void StartGame()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            if (Time.timeScale == 0f) Time.timeScale = 1f;
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("Intro finished. (No nextSceneName set.)");
        }
    }

    private void PlayMusic()
    {
        if (musicSource && !musicSource.isPlaying)
            musicSource.Play();
    }

    private void StopMusic()
    {
        if (musicSource && musicSource.isPlaying)
            musicSource.Stop();
    }
}
