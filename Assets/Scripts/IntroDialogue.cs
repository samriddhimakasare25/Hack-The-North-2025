using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroAutoStart : MonoBehaviour
{
    [Header("Refs")]
    public TypewriterTMP typewriter;      // Drag your TMP object with TypewriterTMP
    public AudioSource musicSource;       // Drag your background music AudioSource here

    [Header("Lines")]
    [TextArea] public string line1 = "You look so down… You know what will cheer you up? A GAME!";
    [TextArea] public string line2 = "What are you willing to do for some sweet, sweet cash?!";
    [TextArea] public string line3 = "Do you want to play?";

    [Header("Continue Prompt")]
    [TextArea] public string continuePrompt = "Click to continue…";

    [Header("Next Scene")]
    public string nextSceneName = "";
    public float postClickDelay = 0.2f;

    private int stage = 0;
    private bool waitingForClick = false;

    void Start()
    {
        if (!typewriter) { Debug.LogError("TypewriterTMP ref not set."); return; }

        typewriter.OnFinished.AddListener(OnLineFinished);
        stage = 0;
        typewriter.Play(line1);
    }

    void OnDestroy()
    {
        if (typewriter) typewriter.OnFinished.RemoveListener(OnLineFinished);
    }

    void Update()
    {
        // Skip reveal while typing
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && typewriter && typewriter.IsTyping)
        {
            typewriter.Skip();
            return;
        }

        // After prompt is visible, wait for player input
        if (waitingForClick && !typewriter.IsTyping && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            waitingForClick = false;
            Invoke(nameof(StartGame), postClickDelay);
        }
    }

    private void OnLineFinished()
    {
        stage++;
        switch (stage)
        {
            case 1:
                typewriter.Play(line2);
                break;

            case 2:
                typewriter.Play(line3);
                break;

            case 3:
                typewriter.Play(continuePrompt);
                waitingForClick = true;
                break;

            case 4:
                // 🔊 After "Click to continue…" finishes typing → stop music
                if (musicSource) musicSource.Stop();
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
            Debug.Log("Game start! (No nextSceneName set)");
        }
    }
}
