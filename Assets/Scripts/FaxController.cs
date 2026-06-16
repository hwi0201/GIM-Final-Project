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

    [Header("독백 자막 (백그라운드/그룹 공유)")]
    public CanvasGroup     monologueGroup;
    public GameObject      monologueBackground;
    public float           typeSpeed = 0.045f;

    [Header("독백 텍스트 (영상 볼 때)")]
    public TextMeshProUGUI monologueText;

    [Header("독백 텍스트 (종이 보고 나서 - 1줄)")]
    public TextMeshProUGUI monologueText2;

    [Header("독백 텍스트 (종이 보고 나서 - 2줄)")]
    public TextMeshProUGUI monologueText3;

    [Header("종이 클로즈업 UI")]
    public GameObject paperCloseupPanel;

    [Header("아이콘 (선택)")]
    public GameObject faxIcon;

    private Camera                cam;
    private FirstPersonController fpsController;
    private StarterAssetsInputs   starterInput;
    private Behaviour             cinemachineBrain;
    private float                 closeGuardUntil = 0f;
    private bool                  paperReady    = false;
    private bool                  hasInteracted = false;
    private bool                  monologueShown = false;

    void Awake()
    {
        // IntroCutsceneController.Awake()가 먼저 실행됐을 경우를 대비한 이중 차단
        if (GameState.returnedFromSin1 || GameState.returnedFromSin2)
        {
            IntroCutsceneController cutscene = FindFirstObjectByType<IntroCutsceneController>();
            if (cutscene != null) cutscene.gameObject.SetActive(false);
        }
    }

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
            if (Time.time >= closeGuardUntil && Input.GetMouseButtonDown(1))
                ClosePaperCloseup();
            return;
        }

        if (!paperReady) return;

        float dist    = faxTransform != null ? Vector3.Distance(cam.transform.position, faxTransform.position) : 999f;
        bool  inRange = dist <= interactDistance;

        // 첫 클릭 전에만 아이콘 표시
        if (!hasInteracted && faxIcon != null) faxIcon.SetActive(inRange);

        if (inRange && Input.GetMouseButtonDown(0) && IsLookingAtFax())
        {
            hasInteracted = true;
            if (faxIcon != null) faxIcon.SetActive(false);
            StartCoroutine(OpenPaperCloseup());
        }
    }

    bool IsLookingAtFax()
    {
        if (faxPaper == null) return false;

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            return hit.collider != null &&
                   (hit.collider.gameObject == faxPaper ||
                    hit.collider.transform.IsChildOf(faxPaper.transform));
        }
        return false;
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
        Quaternion originalRot = cam.transform.rotation;
        if (cinemachineBrain != null) cinemachineBrain.enabled = false;

        if (faxLookTarget != null)
        {
            Quaternion lookRot = Quaternion.LookRotation(
                faxLookTarget.position - cam.transform.position);
            cam.transform.DORotateQuaternion(lookRot, camTurnDuration).SetEase(Ease.OutCubic);
        }
        yield return new WaitForSeconds(camTurnDuration);

        // 카메라가 팩스 보는 동안 종이 슬라이드
        if (faxPaper != null)
        {
            faxPaper.SetActive(true);
            faxPaper.transform.localPosition = paperStartLocalPos;
            faxPaper.transform.DOLocalMove(paperEndLocalPos, paperSlideTime).SetEase(Ease.Linear);
        }
        yield return new WaitForSeconds(paperSlideTime + 0.5f);

        // 원래 회전으로 복구
        cam.transform.DORotateQuaternion(originalRot, camReturnDuration).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(camReturnDuration);

        if (cinemachineBrain != null) cinemachineBrain.enabled = true;

        paperReady = true;
    }

    IEnumerator OpenPaperCloseup()
    {
        LockPlayer();
        closeGuardUntil = Time.time + 0.8f;

        if (paperCloseupPanel != null)
        {
            paperCloseupPanel.SetActive(true);
            CanvasGroup cg = paperCloseupPanel.GetComponent<CanvasGroup>();
            if (cg != null) { cg.alpha = 0f; cg.DOFade(1f, 0.5f); }
        }

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator FreezeCameraRoutine(Quaternion rot)
    {
        while (true)
        {
            cam.transform.rotation = rot;
            yield return null;
        }
    }

    public void ClosePaperCloseup()
    {
        if (paperCloseupPanel != null)
            paperCloseupPanel.SetActive(false);

        if (!monologueShown)
        {
            monologueShown = true;
            StartCoroutine(PostCloseMonologueRoutine());
        }
        else
        {
            UnlockPlayer();
        }
    }

    IEnumerator PostCloseMonologueRoutine()
    {
        Quaternion lockedRot = cam.transform.rotation;
        if (cinemachineBrain != null) cinemachineBrain.enabled = false;
        Coroutine freeze = StartCoroutine(FreezeCameraRoutine(lockedRot));

        yield return StartCoroutine(ShowMonologue(
            "...이 이름들, 아까 영상에서 본 가해자들이잖아.",
            monologueText2, monologueGroup, monologueBackground));

        yield return new WaitForSeconds(1.0f);

        yield return StartCoroutine(ShowMonologue(
            "아무래도 기사를 좀 더 찾아봐야겠어.",
            monologueText3, monologueGroup, monologueBackground));

        yield return new WaitForSeconds(1.0f);

        if (monologueGroup != null)
            monologueGroup.DOFade(0f, 0.4f)
                .OnComplete(() => { if (monologueBackground != null) monologueBackground.SetActive(false); });

        StopCoroutine(freeze);
        if (cinemachineBrain != null) cinemachineBrain.enabled = true;

        UnlockPlayer();
    }

    // ── 유틸 ──────────────────────────────────────────────────

    IEnumerator ShowMonologue(string text)
    {
        yield return ShowMonologue(text, monologueText, monologueGroup, monologueBackground);
    }

    IEnumerator ShowMonologue(string text, TextMeshProUGUI targetText, CanvasGroup targetGroup, GameObject targetBackground)
    {
        if (targetGroup == null || targetText == null) yield break;

        // 같은 백그라운드를 쓰는 다른 텍스트는 비워서 겹쳐 보이지 않게 함
        if (monologueText  != null && monologueText  != targetText) monologueText.text  = "";
        if (monologueText2 != null && monologueText2 != targetText) monologueText2.text = "";
        if (monologueText3 != null && monologueText3 != targetText) monologueText3.text = "";

        if (targetBackground != null) targetBackground.SetActive(true);
        targetGroup.alpha = 0f;
        targetGroup.DOFade(1f, 0.3f);

        targetText.text = "";
        foreach (char c in text)
        {
            targetText.text += c;

            if (targetBackground != null)
            {
                RectTransform bgRect = targetBackground.GetComponent<RectTransform>();
                if (bgRect != null) LayoutRebuilder.ForceRebuildLayoutImmediate(bgRect);
            }

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
