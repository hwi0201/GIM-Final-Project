using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using StarterAssets;

public class FaxController : MonoBehaviour
{
    [Header("팩스 오브젝트")]
    public GameObject faxPaper;
    public float      interactDistance = 2f;
    public Transform  faxTransform;
    public Transform  faxLookTarget;

    [Header("종이 슬라이드 애니메이션")]
    public Vector3 paperStartLocalPos;
    public Vector3 paperEndLocalPos;
    public float   paperSlideTime = 2.5f;

    [Header("카메라 연출")]
    public float camTurnDuration   = 0.6f;
    public float camReturnDuration = 0.5f;

    [Header("사운드")]
    public AudioSource audioSource;
    public AudioClip   faxBeepClip;

    [Header("독백 자막")]
    public TextMeshProUGUI monologueText;
    public CanvasGroup     monologueGroup;
    public GameObject      monologueBackground;
    public float           typeSpeed = 0.045f;

    [Header("종이 클로즈업 UI")]
    public GameObject paperCloseupPanel;

    [Header("아이콘 (선택)")]
    public GameObject faxIcon;

    private Camera                cam;
    private FirstPersonController fpsController;
    private StarterAssetsInputs   starterInput;
    private Behaviour             cinemachineBrain;
    private bool                  paperReady    = false;
    private bool                  hasInteracted = false;

    void Start()
    {
        cam              = Camera.main;
        fpsController    = FindFirstObjectByType<FirstPersonController>();
        starterInput     = FindFirstObjectByType<StarterAssetsInputs>();
        cinemachineBrain = cam?.GetComponent("CinemachineBrain") as Behaviour;

        if (faxPaper          != null) faxPaper.SetActive(false);
        if (faxIcon           != null) faxIcon.SetActive(false);
        if (paperCloseupPanel != null) paperCloseupPanel.SetActive(false);
        if (monologueGroup    != null) monologueGroup.alpha = 0f;
        if (monologueBackground != null) monologueBackground.SetActive(false);

        if (GameState.returnedFromSin1)
        {
            GameState.returnedFromSin1 = false;
            StartCoroutine(Sin1ReturnSequence());
        }
    }

    void Update()
    {
        if (paperCloseupPanel != null && paperCloseupPanel.activeSelf)
        {
            if (Input.GetMouseButtonDown(1))
                ClosePaperCloseup();
            return;
        }

        if (!paperReady) return;

        float dist    = faxTransform != null ? Vector3.Distance(cam.transform.position, faxTransform.position) : 999f;
        bool  inRange = dist <= interactDistance;

        // 첫 클릭 전에만 아이콘 표시
        if (!hasInteracted && faxIcon != null) faxIcon.SetActive(inRange);

        if (inRange && Input.GetMouseButtonDown(0))
        {
            hasInteracted = true;
            if (faxIcon != null) faxIcon.SetActive(false);
            StartCoroutine(OpenPaperCloseup());
        }
    }

    // ── 시퀀스 ────────────────────────────────────────────────

    IEnumerator Sin1ReturnSequence()
    {
        LockPlayer();

        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(ShowMonologue("...이게 뭐지. 왜 나한테 이 영상을?"));

        yield return new WaitForSeconds(1.0f);

        yield return StartCoroutine(FaxPrintRoutine());

        // 팩스 출력 완료 후 조작 복귀
        UnlockPlayer();
    }

    IEnumerator FaxPrintRoutine()
    {
        // 팩스 소리
        if (audioSource != null && faxBeepClip != null)
            audioSource.PlayOneShot(faxBeepClip);

        // 독백 fade out
        if (monologueGroup != null)
            monologueGroup.DOFade(0f, 0.3f)
                .OnComplete(() => { if (monologueBackground != null) monologueBackground.SetActive(false); });

        // 카메라 팩스 쪽으로 강제 회전
        if (faxLookTarget != null)
        {
            Quaternion originalRot = cam.transform.rotation;
            if (cinemachineBrain != null) cinemachineBrain.enabled = false;

            Quaternion lookRot = Quaternion.LookRotation(
                faxLookTarget.position - cam.transform.position);
            cam.transform.DORotateQuaternion(lookRot, camTurnDuration).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(camTurnDuration + paperSlideTime + 0.5f);

            // 원래 회전으로 복구
            cam.transform.DORotateQuaternion(originalRot, camReturnDuration).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(camReturnDuration);

            if (cinemachineBrain != null) cinemachineBrain.enabled = true;
        }

        // 종이 슬라이드
        if (faxPaper != null)
        {
            faxPaper.SetActive(true);
            faxPaper.transform.localPosition = paperStartLocalPos;
            faxPaper.transform.DOLocalMove(paperEndLocalPos, paperSlideTime).SetEase(Ease.Linear);
        }

        yield return new WaitForSeconds(paperSlideTime + 0.3f);

        paperReady = true;
    }

    IEnumerator OpenPaperCloseup()
    {
        LockPlayer();

        if (paperCloseupPanel != null)
        {
            paperCloseupPanel.SetActive(true);
            CanvasGroup cg = paperCloseupPanel.GetComponent<CanvasGroup>();
            if (cg != null) { cg.alpha = 0f; cg.DOFade(1f, 0.5f); }
        }

        yield return new WaitForSeconds(0.5f);
    }

    public void ClosePaperCloseup()
    {
        if (paperCloseupPanel != null)
            paperCloseupPanel.SetActive(false);

        if (monologueGroup != null)
            monologueGroup.DOFade(0f, 0.4f)
                .OnComplete(() => { if (monologueBackground != null) monologueBackground.SetActive(false); });

        UnlockPlayer();
    }

    // ── 유틸 ──────────────────────────────────────────────────

    IEnumerator ShowMonologue(string text)
    {
        if (monologueGroup == null || monologueText == null) yield break;

        if (monologueBackground != null) monologueBackground.SetActive(true);
        monologueText.text = "";
        monologueGroup.alpha = 0f;
        monologueGroup.DOFade(1f, 0.3f);

        foreach (char c in text)
        {
            monologueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    void LockPlayer()
    {
        if (fpsController != null) fpsController.enabled = false;
        if (starterInput  != null) starterInput.MoveInput(Vector2.zero);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void UnlockPlayer()
    {
        if (fpsController != null) fpsController.enabled = true;
        if (starterInput  != null) starterInput.cursorLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    [ContextMenu("테스트: Sin1 복귀 시뮬레이션")]
    void TestSin1Return()
    {
        StartCoroutine(Sin1ReturnSequence());
    }
}
