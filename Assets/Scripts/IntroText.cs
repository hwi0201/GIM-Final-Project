using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class IntroText : MonoBehaviour
{
    public CanvasGroup introPanel;   // drag IntroPanel here
    public float displayDuration = 5f;
    public float fadeDuration = 1f;

    void Start()
    {
        StartCoroutine(ShowIntro());
    }

    IEnumerator ShowIntro()
    {
        // start fully visible
        introPanel.alpha = 1f;
        introPanel.gameObject.SetActive(true);

        // hold for display duration
        yield return new WaitForSeconds(displayDuration);

        // fade out
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            introPanel.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        // hide completely
        introPanel.gameObject.SetActive(false);
    }
}