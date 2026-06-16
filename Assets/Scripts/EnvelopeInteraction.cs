using UnityEngine;
using StarterAssets;

public class EnvelopeInteraction : MonoBehaviour
{
    [Header("봉투")]
    public GameObject envelope;
    public GameObject envelopeIcon;
    public float       interactDistance = 2f;

    [Header("종이 - 앞면 / 뒷면 (같은 위치, 켜고 끄는 방식)")]
    public GameObject paperFront;
    public GameObject paperBack;

    private Camera               cam;
    private FirstPersonController fpsController;
    private StarterAssetsInputs   starterInput;

    private bool isViewing = false;
    private bool isFlipped = false;

    void Start()
    {
        cam           = Camera.main;
        fpsController = FindFirstObjectByType<FirstPersonController>();
        starterInput  = FindFirstObjectByType<StarterAssetsInputs>();

        if (envelopeIcon != null) envelopeIcon.SetActive(false);
        if (paperFront   != null) paperFront.SetActive(false);
        if (paperBack    != null) paperBack.SetActive(false);
    }

    void Update()
    {
        if (isViewing)
        {
            if (Input.GetMouseButtonDown(0)) FlipPaper();
            if (Input.GetMouseButtonDown(1)) ClosePaperView();
            return;
        }

        if (envelope == null || !envelope.activeSelf) return;

        float dist    = Vector3.Distance(cam.transform.position, envelope.transform.position);
        bool  inRange = dist <= interactDistance;

        if (envelopeIcon != null) envelopeIcon.SetActive(inRange);

        if (inRange && Input.GetMouseButtonDown(0) && IsLookingAtEnvelope())
            OpenPaperView();
    }

    bool IsLookingAtEnvelope()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            return hit.collider != null && hit.collider.gameObject == envelope;
        return false;
    }

    void OpenPaperView()
    {
        isViewing = true;
        isFlipped = false;
        if (envelopeIcon != null) envelopeIcon.SetActive(false);
        LockPlayer();

        if (paperFront != null) paperFront.SetActive(true);
        if (paperBack  != null) paperBack.SetActive(false);
    }

    void FlipPaper()
    {
        isFlipped = !isFlipped;
        if (paperFront != null) paperFront.SetActive(!isFlipped);
        if (paperBack  != null) paperBack.SetActive(isFlipped);
    }

    void ClosePaperView()
    {
        isViewing = false;
        if (paperFront != null) paperFront.SetActive(false);
        if (paperBack  != null) paperBack.SetActive(false);

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
