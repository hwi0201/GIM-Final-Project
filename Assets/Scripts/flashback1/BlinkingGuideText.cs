using System.Collections;
using UnityEngine;

public class BlinkingGuideText : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup canvasGroup;

    [Header("Delay")]
    public float showDelay = 4f;

    [Header("Blink")]
    public float minAlpha = 0.25f;
    public float maxAlpha = 1f;
    public float blinkSpeed = 2f;

    private bool isShowing = false;
    private Coroutine blinkRoutine;

    void Start()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        StartCoroutine(ShowAfterDelay());
    }

    IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(showDelay);

        ShowGuide();
    }

    public void ShowGuide()
    {
        if (canvasGroup == null) return;

        isShowing = true;

        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
        }

        blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    public void HideGuide()
    {
        isShowing = false;

        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    IEnumerator BlinkRoutine()
    {
        while (isShowing)
        {
            float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            canvasGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

            yield return null;
        }
    }
}