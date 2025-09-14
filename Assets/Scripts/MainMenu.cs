using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Next scene to load when Play is pressed")]
    public string nextSceneName = "VideoScene";   // ← set to your video scene’s exact name

    // Called by the Play button OnClick
    public void Play()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            Debug.LogError("[MainMenu] nextSceneName is empty.");
    }

    // (optional) wire these if you add buttons later
    public void Quit()  { Application.Quit(); }
    public void OpenSettings() { /* show settings panel */ }
}
