using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene2Controller : MonoBehaviour
{
    [Header("Timing")]
    public float duration = 3f;                 // how long Scene 2 lasts

    [Header("Music (optional)")]
    public AudioSource musicSource;             // short clip; optional

    [Header("Navigation")]
    [Tooltip("If -1, loads the scene after this one in Build Settings. Otherwise loads this build index.")]
    public int nextSceneBuildIndex = -1;

    float _t;
    bool _running;

    void Start()
    {
        if (musicSource) musicSource.Play();
        _running = true;

        // Sanity log to help debug build index issues
        Debug.Log($"[Scene2Controller] Current buildIndex={SceneManager.GetActiveScene().buildIndex}. " +
                  $"Next={(nextSceneBuildIndex == -1 ? "auto (current+1)" : nextSceneBuildIndex.ToString())}");
    }

    void Update()
    {
        if (!_running) return;

        _t += Time.deltaTime;
        if (_t >= duration)
        {
            _running = false;
            if (musicSource && musicSource.isPlaying) musicSource.Stop();
            LoadNext();
        }
    }

    void LoadNext()
    {
        int total = SceneManager.sceneCountInBuildSettings;
        int current = SceneManager.GetActiveScene().buildIndex;

        int target = nextSceneBuildIndex >= 0 ? nextSceneBuildIndex : current + 1;

        if (target < 0 || target >= total)
        {
            Debug.LogError($"[Scene2Controller] Target build index {target} is out of range (0..{total - 1}). " +
                           "Add scenes to File → Build Settings and check their order.");
            return;
        }

        SceneManager.LoadScene(target);
    }
}
