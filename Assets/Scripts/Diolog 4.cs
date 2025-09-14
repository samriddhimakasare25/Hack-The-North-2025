using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathDialogueYell : MonoBehaviour
{
    [Header("Refs")]
    public TypewriterTMP typewriter;        // assign your TypewriterTMP
    public AudioSource musicSource;         // optional bg music to stop/fade
    public AudioSource sfxSource;           // optional – plays yellSfx on start

    [Header("Lines")]
    [TextArea] public string line1 = "NO! NO! NO! YOU SHOULD BE DEAD!";
    [TextArea] public string continuePrompt = "Click to continue…";

    [Header("Next Scene")]
    public string nextSceneName = "GameOver";  // scene to load after click
    public float postClickDelay = 0.15f;

    [Header("Audio Options")]
    public bool fadeOutOnClick = true;
    public float fadeOutSeconds = 0.8f;
    public bool stopAllAudioOnClick = true;
    public AudioClip yellSfx;                 // optional one-shot on start

    int stage = 0;
    bool waitingForClick = false;

    void Start()
    {
        if (!typewriter) { Debug.LogError("[DeathDialogueYell] TypewriterTMP ref not set."); return; }

        typewriter.OnFinished.AddListener(OnLineFinished);

        if (musicSource && !musicSource.isPlaying) musicSource.Play();
        if (sfxSource && yellSfx) sfxSource.PlayOneShot(yellSfx, 0.9f);

        typewriter.Play(line1);
    }

    void OnDestroy()
    {
        if (typewriter) typewriter.OnFinished.RemoveListener(OnLineFinished);
    }

    void Update()
    {
        // allow skip while typing
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && typewriter && typewriter.IsTyping)
        {
            typewriter.Skip();
            return;
        }

        // wait for click AFTER continue prompt
        if (waitingForClick && !typewriter.IsTyping && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            waitingForClick = false;
            StartCoroutine(HandleContinue());
        }
    }

    void OnLineFinished()
    {
        stage++;
        if (stage == 1)
        {
            typewriter.Play(continuePrompt);
            waitingForClick = true;
        }
    }

    IEnumerator HandleContinue()
    {
        // stop music AFTER click (optional fade)
        if (musicSource)
        {
            if (fadeOutOnClick) yield return StartCoroutine(FadeOutAndStop(musicSource, fadeOutSeconds));
            else musicSource.Stop();
        }

        if (stopAllAudioOnClick)
        {
            foreach (var a in FindObjectsOfType<AudioSource>(true))
                if (a && a.isPlaying) a.Stop();
            AudioListener.pause = false;
        }

        yield return new WaitForSecondsRealtime(postClickDelay);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            if (Time.timeScale == 0f) Time.timeScale = 1f;
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("[DeathDialogueYell] Finished. (No nextSceneName set)");
        }
    }

    IEnumerator FadeOutAndStop(AudioSource src, float seconds)
    {
        if (!src) yield break;
        float startVol = src.volume;
        float t = 0f, dur = Mathf.Max(0.01f, seconds);
        if (!src.isPlaying) { src.Stop(); yield break; }

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(startVol, 0f, t / dur);
            yield return null;
        }
        src.Stop();
        src.volume = startVol; // restore for next time
    }
}
