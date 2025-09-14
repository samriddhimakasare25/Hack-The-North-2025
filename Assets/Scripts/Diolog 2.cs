using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingDialogue : MonoBehaviour
{
    [Header("Refs")]
    public TypewriterTMP typewriter;      
    public AudioSource musicSource;       

    [Header("Lines")]
    [TextArea] public string line1 = "Nice, you won some money!";
    [TextArea] public string line2 = "Now it’s your end of the deal…";
    [TextArea] public string continuePrompt = "Click to continue…";

    [Header("Next Scene")]
    public string nextSceneName = "G2";   // 🔥 default set to G2
    public float postClickDelay = 0.15f;

    [Header("Audio Options")]
    public bool fadeOutOnClick = true;
    public float fadeOutSeconds = 0.8f;
    public bool stopAllAudioOnClick = true;

    private int stage = 0;
    private bool waitingForClick = false;

    void Start()
    {
        if (!typewriter) { Debug.LogError("[EndingDialogue] TypewriterTMP ref not set."); return; }

        typewriter.OnFinished.AddListener(OnLineFinished);
        stage = 0;

        if (musicSource && !musicSource.isPlaying)
        {
            musicSource.volume = Mathf.Clamp01(musicSource.volume);
            musicSource.Play();
        }

        typewriter.Play(line1);
    }

    void OnDestroy()
    {
        if (typewriter) typewriter.OnFinished.RemoveListener(OnLineFinished);
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && typewriter && typewriter.IsTyping)
        {
            typewriter.Skip();
            return;
        }

        if (waitingForClick && !typewriter.IsTyping && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            waitingForClick = false;
            StartCoroutine(HandleContinue());
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
                typewriter.Play(continuePrompt);
                waitingForClick = true;
                break;
        }
    }

    private IEnumerator HandleContinue()
    {
        if (musicSource)
        {
            if (fadeOutOnClick)
                yield return StartCoroutine(FadeOutAndStop(musicSource, fadeOutSeconds));
            else
                musicSource.Stop();
        }

        if (stopAllAudioOnClick)
        {
            var all = FindObjectsOfType<AudioSource>(includeInactive: true);
            foreach (var a in all)
            {
                if (a && a.isPlaying) a.Stop();
            }
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
            Debug.Log("[EndingDialogue] Finished. (No nextSceneName set)");
        }
    }

    private IEnumerator FadeOutAndStop(AudioSource src, float seconds)
    {
        if (!src) yield break;
        float startVol = src.volume;
        float t = 0f;
        float dur = Mathf.Max(0.01f, seconds);

        if (!src.isPlaying) { src.Stop(); yield break; }

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = 1f - Mathf.Clamp01(t / dur);
            src.volume = startVol * k;
            yield return null;
        }

        src.Stop();
        src.volume = startVol;
    }
}
