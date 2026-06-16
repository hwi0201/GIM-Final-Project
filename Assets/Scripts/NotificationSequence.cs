using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using StarterAssets;

public class NotificationSequence : MonoBehaviour
{
    [Header("카메라 연출")]
    public MonitorInteraction monitorInteraction;
    public Transform monitorLookTarget;
    public float lookDuration    = 0.6f;
    public float returnDuration  = 0.5f;
    public float holdDuration    = 3.0f;

    [Header("메일 알림 팝업 (오른쪽 하단 UI)")]
    public GameObject mailNotifyPopup;
    public float popupFadeDuration = 0.3f;

    [Header("독백 자막 UI (MonologueArea 오브젝트 연결)")]
    public GameObject monologueArea;
    public TextMeshProUGUI monologueText;
    public CanvasGroup monologueGroup;

    [Header("독백 내용 (여러 줄 가능)")]
    [TextArea(2, 5)]
    public string monologue = "...이 시간에 누구지?";
    public float typeSpeed       = 0.04f;
    public float monologueDelay  = 0.8f;

    private Camera cam;
    private FirstPersonController fpsController;
    private StarterAssetsInputs starterInput;

    private Quaternion originalRot;
    private bool isPlaying = false;

    void Start()
    {
        cam           = Camera.main;
        fpsController = FindFirstObjectByType<FirstPersonController>();
        starterInput  = FindFirstObjectByType<StarterAssetsInputs>();

        if (mailNotifyPopup != null) mailNotifyPopup.SetActive(false);
        if (monologueArea   != null) monologueArea.SetActive(false);
        if (monologueText   != null) monologueText.text = "";
    }

    public void Play()
    {
        if (!isPlaying)
            StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        isPlaying = true;

        // 플레이어 입력 잠금
        if (fpsController       != null) fpsController.enabled       = false;
        if (monitorInteraction  != null) monitorInteraction.enabled  = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        originalRot = cam.transform.rotation;

        // 1. 카메라 모니터 쪽으로 회전
        if (monitorLookTarget != null)
        {
            Quaternion lookRot = Quaternion.LookRotation(
                monitorLookTarget.position - cam.transform.position);
            cam.transform.DORotateQuaternion(lookRot, lookDuration).SetEase(Ease.OutCubic);
        }
        yield return new WaitForSeconds(lookDuration);

        // 2. 메일 알림 팝업
        if (mailNotifyPopup != null)
        {
            mailNotifyPopup.SetActive(true);
            CanvasGroup pg = mailNotifyPopup.GetComponent<CanvasGroup>();
            if (pg == null) pg = mailNotifyPopup.AddComponent<CanvasGroup>();
            pg.alpha = 0f;
            pg.DOFade(1f, popupFadeDuration);
        }

        // 3. 독백 자막 (딜레이 후)
        yield return new WaitForSeconds(monologueDelay);

        if (monologueArea != null) monologueArea.SetActive(true);
        if (monologueGroup != null)
        {
            monologueGroup.alpha = 0f;
            monologueGroup.DOFade(1f, 0.3f);
        }

        if (monologueText != null)
            yield return StartCoroutine(TypeText(monologue));

        // 4. 잠깐 유지
        yield return new WaitForSeconds(holdDuration);

        // 5. 자막 + 알림창 동시 페이드아웃
        if (monologueGroup != null)
            monologueGroup.DOFade(0f, 0.4f);

        if (mailNotifyPopup != null)
        {
            CanvasGroup pg = mailNotifyPopup.GetComponent<CanvasGroup>();
            if (pg != null) pg.DOFade(0f, 0.4f);
        }

        yield return new WaitForSeconds(0.4f);
        if (monologueArea   != null) monologueArea.SetActive(false);
        if (mailNotifyPopup != null) mailNotifyPopup.SetActive(false);

        // 6. 카메라 원위치
        cam.transform.DORotateQuaternion(originalRot, returnDuration).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(returnDuration);

        // 7. 조작 복귀
        if (fpsController      != null) fpsController.enabled      = true;
        if (monitorInteraction != null) monitorInteraction.enabled = true;
        if (starterInput       != null) starterInput.cursorLocked   = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        isPlaying = false;
    }

    IEnumerator TypeText(string text)
    {
        monologueText.text = "";
        foreach (char c in text)
        {
            monologueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }
}
