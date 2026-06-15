using System.Collections;
using UnityEngine;

public class KakaoNotificationAudio : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip notificationClip;

    [Header("Repeat Settings")]
    public float startDelay = 1.0f;
    public float interval = 2.0f;

    [Header("Stop Option")]
    public bool stopCurrentSoundImmediately = true;

    private Coroutine notificationRoutine;
    private bool isStopped = false;

    void Start()
    {
        StartNotificationLoop();
    }

    public void StartNotificationLoop()
    {
        if (isStopped) return;

        if (notificationRoutine != null)
        {
            StopCoroutine(notificationRoutine);
        }

        notificationRoutine = StartCoroutine(NotificationLoop());
    }

    IEnumerator NotificationLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (!isStopped)
        {
            if (audioSource != null && notificationClip != null)
            {
                audioSource.PlayOneShot(notificationClip);
            }

            yield return new WaitForSeconds(interval);
        }
    }

    public void StopNotification()
    {
        isStopped = true;

        if (notificationRoutine != null)
        {
            StopCoroutine(notificationRoutine);
            notificationRoutine = null;
        }

        if (stopCurrentSoundImmediately && audioSource != null)
        {
            audioSource.Stop();
        }
    }
}