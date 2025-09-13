using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroAutoStart : MonoBehaviour
{
    [Header("Refs")]
    public TypewriterTMP typewriter;         // Drag TMP object with TypewriterTMP
    public AudioSource musicSource;          // Drag your background music AudioSource here

    [Header("Lines")]
    [TextArea] public string line1 = "You look so down… You know what will cheer you up? A GAME!";
    [TextArea] public string line2 = "What are you willing to do for some sweet, sweet cash?!";
    [TextArea] public string line3 = "Do you want to play?";

    [Header("Continue Prompt")]
    [TextArea] public string continuePrompt = "Click to continue…";

    [Header("Next Scene")]
    [Tooltip("Scene to load after the player clicks. Must be in Build Settings.")]
    public string nextSceneName = "";
    public float postClickDelay = 0.2f;    // small delay for feel

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
        // Skip typing while animating
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && typewriter && typewriter.IsTyping)
        {
            typewriter.Skip();
            return;
        }

        // After the prompt shows, wait for click/space to proceed
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
                // stop the music right after the last dialogue line finishes
                if (musicSource) musicSource.Stop();

                typewriter.Play(continuePrompt);
                waitingForClick = true;
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
