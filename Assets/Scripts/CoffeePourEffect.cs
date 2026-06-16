using System.Collections;
using UnityEngine;

public class CoffeePourEffect : MonoBehaviour
{
    [Header("커피 줄기 (얇은 실린더)")]
    public GameObject coffeeStream;

    [Header("컵 안 커피 (실린더 - 씬에 세팅해둔 크기가 MAX)")]
    public GameObject coffeeFill;
    public float fillDuration = 2.0f;

    [Header("완료 후 줄기 유지 시간")]
    public float streamHoldTime = 0.3f;

    [Header("쪼르륵 사운드")]
    public AudioSource pourAudio;

    private Vector3 fillMaxScale;

    void Start()
    {
        if (coffeeStream != null) coffeeStream.SetActive(false);

        if (coffeeFill != null)
        {
            fillMaxScale = coffeeFill.transform.localScale;
            coffeeFill.transform.localScale = new Vector3(fillMaxScale.x, 0f, fillMaxScale.z);
            coffeeFill.SetActive(false);
        }
    }

    public void StartPour(System.Action onComplete = null)
    {
        StartCoroutine(PourRoutine(onComplete));
    }

    IEnumerator PourRoutine(System.Action onComplete)
    {
        if (coffeeStream != null) coffeeStream.SetActive(true);
        if (coffeeFill   != null) coffeeFill.SetActive(true);
        if (pourAudio    != null) pourAudio.Play();

        float elapsed = 0f;
        while (elapsed < fillDuration)
        {
            elapsed += Time.deltaTime;
            float t      = Mathf.Clamp01(elapsed / fillDuration);
            float smooth = Mathf.SmoothStep(0f, fillMaxScale.y, t);

            if (coffeeFill != null)
                coffeeFill.transform.localScale = new Vector3(fillMaxScale.x, smooth, fillMaxScale.z);

            yield return null;
        }

        yield return new WaitForSeconds(streamHoldTime);
        if (coffeeStream != null) coffeeStream.SetActive(false);
        if (pourAudio    != null) pourAudio.Stop();

        onComplete?.Invoke();
    }

    public void Reset()
    {
        if (coffeeStream != null) coffeeStream.SetActive(false);
        if (coffeeFill   != null)
        {
            coffeeFill.transform.localScale = new Vector3(fillMaxScale.x, 0f, fillMaxScale.z);
            coffeeFill.SetActive(false);
        }
    }
}
