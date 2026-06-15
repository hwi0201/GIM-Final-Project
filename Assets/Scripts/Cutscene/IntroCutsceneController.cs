using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
using StarterAssets;

public class IntroCutsceneController : MonoBehaviour
{
    [Header("암전 패널")]
    public CanvasGroup fadePanel;
    public float blackScreenDuration = 3f;
    public float fadeInDuration = 1.5f;
    public float endBlackDuration = 2.0f;

    [Header("조준점")]
    public GameObject crosshair;

    [Header("컷씬 중 숨길 UI 오브젝트")]
    public GameObject[] hideUIDuringCutscene;

    [Header("컷씬 중 끌 상호작용 스크립트")]
    public MonitorInteraction monitorInteraction;
    public CoffeeInteraction  coffeeInteraction;

    [Header("타임라인")]
    public PlayableDirector director;

    private FirstPersonController fpsController;
    private StarterAssetsInputs   starterInput;
    private Behaviour             cinemachineBrain;

    private Vector3    originalCamLocalPos;
    private Quaternion originalCamLocalRot;
    private bool       playerRestored = false;

    void Awake()
    {
        // Sin 복귀면 자기 자신을 꺼버림 → Start()가 아예 실행 안 됨
        if (GameState.returnedFromSin1 || GameState.returnedFromSin2)
        {
            gameObject.SetActive(false);
            return;
        }

        fpsController    = FindFirstObjectByType<FirstPersonController>();
        starterInput     = FindFirstObjectByType<StarterAssetsInputs>();
        cinemachineBrain = Camera.main?.GetComponent("CinemachineBrain") as Behaviour;
    }

    void Start()
    {
        originalCamLocalPos = Camera.main.transform.localPosition;
        originalCamLocalRot = Camera.main.transform.localRotation;

        if (fadePanel != null) fadePanel.alpha = 1f;
        DisablePlayer();
        StartCoroutine(BlinkRoutine());

        if (director != null)
        {
            director.stopped += _ => { if (!playerRestored) StartCoroutine(EndRoutine()); };
            director.Play();
        }
    }

    // ── 눈 깜박임 ────────────────────────────────────────────────────────

    IEnumerator BlinkRoutine()
    {
        if (fadePanel == null) yield break;

        yield return new WaitForSeconds(blackScreenDuration);

        float[] openAlpha = { 0.55f, 0.35f, 0.15f };
        float[] openSpeed = { 0.30f, 0.25f, 0.20f };
        float[] holdTime  = { 0.55f, 0.50f, 0.55f };

        for (int i = 0; i < openAlpha.Length; i++)
        {
            fadePanel.DOFade(openAlpha[i], openSpeed[i]).SetEase(Ease.OutSine);
            yield return new WaitForSeconds(openSpeed[i] + holdTime[i]);
            fadePanel.DOFade(1f, 0.12f).SetEase(Ease.InSine);
            yield return new WaitForSeconds(0.22f);
        }

        fadePanel.DOFade(0.2f, 0.35f).SetEase(Ease.OutSine);
    }

    // ── Timeline Signal 메서드 ────────────────────────────────────────────

    public void Signal_FadeIn()
    {
        if (fadePanel != null)
            fadePanel.DOFade(0f, fadeInDuration);
    }

    public void Signal_EndCutscene()
    {
        StartCoroutine(EndRoutine());
    }

    IEnumerator EndRoutine()
    {
        if (playerRestored) yield break;
        playerRestored = true;

        // 암전
        if (fadePanel != null)
            fadePanel.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(endBlackDuration);

        EnablePlayer();

        // 다시 밝아짐
        if (fadePanel != null)
            fadePanel.DOFade(0f, 1.0f);
    }

    // ─────────────────────────────────────────────────────────────────────

    void DisablePlayer()
    {
        if (cinemachineBrain != null) cinemachineBrain.enabled = false;
        if (fpsController    != null) fpsController.enabled    = false;
        if (starterInput     != null) starterInput.MoveInput(Vector2.zero);
        if (crosshair        != null) crosshair.SetActive(false);

        foreach (var obj in hideUIDuringCutscene)
            if (obj != null) obj.SetActive(false);
        if (monitorInteraction != null) monitorInteraction.enabled = false;
        if (coffeeInteraction  != null) coffeeInteraction.enabled  = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    void EnablePlayer()
    {
        // 카메라 원위치 복구 후 Cinemachine 재활성화
        if (Camera.main != null)
        {
            Camera.main.transform.localPosition = originalCamLocalPos;
            Camera.main.transform.localRotation = originalCamLocalRot;
        }

        if (cinemachineBrain != null) cinemachineBrain.enabled = true;
        if (fpsController    != null) fpsController.enabled    = true;
        if (starterInput     != null) starterInput.cursorLocked = true;
        if (crosshair        != null) crosshair.SetActive(true);

        foreach (var obj in hideUIDuringCutscene)
            if (obj != null) obj.SetActive(true);
        if (monitorInteraction != null) monitorInteraction.enabled = true;
        if (coffeeInteraction  != null) coffeeInteraction.enabled  = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }
}
