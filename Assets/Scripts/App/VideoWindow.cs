using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class VideoWindow : MonoBehaviour
{
    [Header("버튼")]
    public Button enterButton;
    public Button closeButton;

    [Header("씬 전환")]
    public string nextSceneName = "FlashbackScene1";
    public float zoomDuration = 1.0f;
    public float fadeDuration = 0.8f;

    [Header("카메라 줌인 목표 위치 (빈 오브젝트)")]
    public Transform zoomTarget;

    private Camera cam;
    private bool isTransitioning = false;

    void Start()
    {
        cam = Camera.main;
        if (enterButton != null) enterButton.onClick.AddListener(OnEnterClicked);
        if (closeButton != null) closeButton.onClick.AddListener(OnCloseClicked);
    }

    void OnCloseClicked()
    {
        gameObject.SetActive(false);
    }

    void OnEnterClicked()
    {
        if (!isTransitioning)
            StartCoroutine(TransitionToScene());
    }

    IEnumerator TransitionToScene()
    {
        isTransitioning = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 암전 오버레이 먼저 만들기
        Canvas fadeCanvas = new GameObject("FadeCanvas").AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999;
        fadeCanvas.gameObject.AddComponent<CanvasScaler>();

        Image blackScreen = new GameObject("BlackScreen").AddComponent<Image>();
        blackScreen.transform.SetParent(fadeCanvas.transform, false);
        blackScreen.color = new Color(0, 0, 0, 0);
        RectTransform rt = blackScreen.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        // 줌인 + 암전 동시에
        float duration = Mathf.Max(zoomDuration, fadeDuration);
        if (zoomTarget != null)
        {
            cam.transform.DOMove(zoomTarget.position, zoomDuration).SetEase(Ease.InCubic);
            cam.transform.DORotate(zoomTarget.eulerAngles, zoomDuration).SetEase(Ease.InCubic);
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            blackScreen.color = new Color(0, 0, 0, Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        SceneManager.LoadScene(nextSceneName);
    }
}
