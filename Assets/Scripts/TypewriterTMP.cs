using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TypewriterTMP : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text textUI;

    [Header("Speed")]
    [Tooltip("Characters shown per second.")]
    [SerializeField] private float charsPerSecond = 40f;
    [Tooltip("Extra pause after punctuation like . , ! ? : ;")]
    [SerializeField] private float punctuationPause = 0.25f;

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tickClip;
    [Tooltip("Seconds between ticks so it doesn't play every single char.")]
    [SerializeField] private float tickCooldown = 0.03f;

    [Header("Events")]
    public UnityEvent OnFinished;

    private string _fullText = "";
    private Coroutine _routine;
    private bool _isTyping;
    private float _tickTimer;

    public bool IsTyping => _isTyping;

    void Reset()
    {
        textUI = GetComponent<TMP_Text>();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>Start typing the given string from the beginning.</summary>
    public void Play(string text)
    {
        _fullText = text ?? "";
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(TypeRoutine(_fullText));
    }

    /// <summary>Immediately reveals the full text.</summary>
    public void Skip()
    {
        if (!_isTyping) return;
        if (_routine != null) StopCoroutine(_routine);
        textUI.text = _fullText;
        _isTyping = false;
        OnFinished?.Invoke();
    }

    /// <summary>Stops typing and clears text (optional helper).</summary>
    public void StopAndClear()
    {
        if (_routine != null) StopCoroutine(_routine);
        _isTyping = false;
        textUI.text = "";
    }

    private IEnumerator TypeRoutine(string source)
    {
        _isTyping = true;
        textUI.text = "";
        var sb = new StringBuilder(source.Length);
        float baseDelay = (charsPerSecond > 0f) ? 1f / charsPerSecond : 0f;
        _tickTimer = 0f;

        for (int i = 0; i < source.Length; i++)
        {
            char c = source[i];

            // If we hit a TMP rich-text tag like <b>, <color=...>, etc, copy it instantly (not revealed char-by-char)
            if (c == '<')
            {
                int close = source.IndexOf('>', i);
                if (close >= 0)
                {
                    sb.Append(source, i, close - i + 1);
                    i = close;
                    textUI.text = sb.ToString();
                    continue;
                }
            }

            // Append one visible character
            sb.Append(c);
            textUI.text = sb.ToString();

            // Optional tick sound with cooldown
            if (tickClip && audioSource)
            {
                if (_tickTimer <= 0f)
                {
                    audioSource.PlayOneShot(tickClip);
                    _tickTimer = tickCooldown;
                }
            }

            // Delay logic
            float delay = baseDelay;

            // Slight pause on punctuation
            if (IsPunctuation(c))
                delay += punctuationPause;

            // Avoid stalling on whitespace (feels snappier)
            if (char.IsWhiteSpace(c))
                delay *= 0.25f;

            // Wait
            float t = 0f;
            while (t < delay)
            {
                t += Time.unscaledDeltaTime;         // unscaled so it ignores timescale changes
                _tickTimer -= Time.unscaledDeltaTime;
                yield return null;
            }
        }

        _isTyping = false;
        OnFinished?.Invoke();
    }

    private static bool IsPunctuation(char c)
    {
        return c == '.' || c == ',' || c == '!' || c == '?' || c == ':' || c == ';';
    }
}
