using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using StarterAssets;

public class Sin2ReturnSequence : MonoBehaviour
{
    [Header("독백 자막 (공유 가능)")]
    public TextMeshProUGUI monologueText;
    public CanvasGroup     monologueGroup;
    public GameObject      monologueBackground;

    [Header("노크 소리")]
    public AudioSource audioSource;
    public AudioClip   knockSound;

    [Header("문에 낄 봉투")]
    public GameObject envelope;
    public GameObject envelopeIcon;

    [Header("카메라 - 문 쪽 고정")]
    public Transform doorLookTarget;
    public float     camTurnDuration   = 0.6f;
    public float     camReturnDuration = 0.5f;

    private Camera                cam;
    private FirstPersonController fpsController;
    private StarterAssetsInputs   starterInput;
    private Behaviour             cinemachineBrain;

    void Start()
    {
        cam              = Camera.main;
        fpsController    = FindFirstObjectByType<FirstPersonController>();
        starterInput     = FindFirstObjectByType<StarterAssetsInputs>();
        cinemachineBrain = cam?.GetComponent("CinemachineBrain") as Behaviour;

        if (envelope     != null) envelope.SetActive(false);
        if (envelopeIcon != null) envelopeIcon.SetActive(false);

        if (GameState.returnedFromSin2)
        {
            GameState.returnedFromSin2 = false;
            StartCoroutine(ReturnRoutine());
        }
    }

    IEnumerator ReturnRoutine()
    {
        LockPlayer();

        yield return new WaitForSeconds(1.0f);

        if (monologueGroup != null && monologueText != null)
        {
            // 같은 백그라운드를 쓰는 다른 텍스트(FaxController 쪽 등)는 비워서 안 보이게 함
            if (monologueBackground != null)
            {
                var allTexts = monologueBackground.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var t in allTexts)
                    if (t != monologueText) t.text = "";

                monologueBackground.SetActive(true);
            }

            monologueText.text = "......";
            monologueGroup.alpha = 0f;
            monologueGroup.DOFade(1f, 0.3f);
        }

        yield return new WaitForSeconds(2.0f);

        if (monologueGroup != null)
            monologueGroup.DOFade(0f, 0.4f)
                .OnComplete(() => { if (monologueBackground != null) monologueBackground.SetActive(false); });

        yield return new WaitForSeconds(1.0f);

        // 똑똑 노크 소리 먼저
        if (audioSource != null && knockSound != null)
            audioSource.PlayOneShot(knockSound);

        yield return new WaitForSeconds(0.4f);

        // 소리 나고 살짝 뒤에 카메라 문 쪽으로 강제 회전
        Quaternion originalRot = cam.transform.rotation;
        if (cinemachineBrain != null) cinemachineBrain.enabled = false;

        if (doorLookTarget != null)
        {
            Quaternion lookRot = Quaternion.LookRotation(
                doorLookTarget.position - cam.transform.position);
            cam.transform.DORotateQuaternion(lookRot, camTurnDuration).SetEase(Ease.OutCubic);
        }
        yield return new WaitForSeconds(camTurnDuration + 1.0f);

        // 문에 봉투 생김
        if (envelope != null) envelope.SetActive(true);

        // 카메라 원위치 복구
        cam.transform.DORotateQuaternion(originalRot, camReturnDuration).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(camReturnDuration);

        if (cinemachineBrain != null) cinemachineBrain.enabled = true;

        UnlockPlayer();
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
}
