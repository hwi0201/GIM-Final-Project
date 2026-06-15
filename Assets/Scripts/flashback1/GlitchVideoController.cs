using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class GlitchVideoController : MonoBehaviour
{
    [Header("Glitch Objects")]
    public GameObject glitchCanvas;
    public VideoPlayer videoPlayer;

    [Header("Glitch Audio")]
    public AudioSource audioSource;
    public AudioClip glitchSound;

    [Header("Duration")]
    public float glitchDuration = 1.0f;

    [Header("Scene Change")]
    public bool loadSceneAfterGlitch = true;
    public string nextSceneName;

    private Coroutine glitchRoutine;

    void Start()
    {
        if (glitchCanvas != null)
        {
            glitchCanvas.SetActive(false);
        }

        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.Stop();
        }

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }

    public void PlayGlitch()
    {
        if (glitchRoutine != null)
        {
            StopCoroutine(glitchRoutine);
        }

        glitchRoutine = StartCoroutine(GlitchRoutine());
    }

    IEnumerator GlitchRoutine()
    {
        if (glitchCanvas != null)
        {
            glitchCanvas.SetActive(true);
        }

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.time = 0;
            videoPlayer.Play();
        }

        if (audioSource != null && glitchSound != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(glitchSound);
        }

        yield return new WaitForSeconds(glitchDuration);

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (loadSceneAfterGlitch && !string.IsNullOrEmpty(nextSceneName))
        {
            GameState.returnedFromSin1 = true;
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            if (glitchCanvas != null)
            {
                glitchCanvas.SetActive(false);
            }
        }
    }
}