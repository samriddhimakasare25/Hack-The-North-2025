using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class VideoToNextScene : MonoBehaviour
{
    [Header("Refs")]
    public VideoPlayer videoPlayer;

    [Header("Next Scene")]
    public string nextSceneName = "Clown First Meeting";
    public float postEndDelay = 0.5f;

    IEnumerator Start()
    {
        if (!videoPlayer) videoPlayer = GetComponent<VideoPlayer>();
        if (!videoPlayer) { Debug.LogError("[VideoToNextScene] No VideoPlayer assigned."); yield break; }

        // Make playback reliable
        videoPlayer.isLooping = false;
        videoPlayer.skipOnDrop = false;               // don't skip frames to “catch up”
        videoPlayer.waitForFirstFrame = true;         // ensure first frame is ready
        videoPlayer.playbackSpeed = 1f;

        // Prepare first
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared) yield return null;

        // Now play
        videoPlayer.Play();

        // Extra safety: also listen to the end event
        bool ended = false;
        videoPlayer.loopPointReached += _ => ended = true;

        // Wait until video time reaches clip length (with small epsilon)
        double epsilon = 0.05; // seconds
        double targetLength = videoPlayer.length;     // needs prepared clip
        if (double.IsNaN(targetLength) || targetLength <= 0)
            targetLength = Mathf.Max(1f, (float)videoPlayer.frameCount / Mathf.Max(1, (int)videoPlayer.frameRate));

        while (!ended && videoPlayer.isPlaying && videoPlayer.time < targetLength - epsilon)
            yield return null;

        // Small delay, then load next scene
        yield return new WaitForSecondsRealtime(postEndDelay);

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            Debug.LogWarning("[VideoToNextScene] nextSceneName not set.");
    }
}
