using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using StarterAssets;

public class MonitorInteraction : MonoBehaviour
{
    [Header("Settings")]
    public float interactDistance = 3f;
    public Transform monitorCamTarget;
    public float moveDuration = 1.5f;

    [Header("UI")]
    public GameObject interactTextUI;

    private FirstPersonController fpsController;
    private StarterAssetsInputs starterInput;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool isInteracting = false;

    void Start()
    {
        fpsController = GetComponentInParent<FirstPersonController>();
        starterInput  = GetComponentInParent<StarterAssetsInputs>();
    }

    void Update()
    {
        if (isInteracting)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ExitMonitorMode();
            return;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("Monitor"))
            {
                interactTextUI.SetActive(true);

                if (Cursor.visible)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible   = false;
                }

                if (Input.GetMouseButtonDown(0))
                    EnterMonitorMode();
            }
            else
            {
                interactTextUI.SetActive(false);
            }
        }
        else
        {
            interactTextUI.SetActive(false);
        }
    }

    void EnterMonitorMode()
    {
        isInteracting = true;
        interactTextUI.SetActive(false);

        originalPosition = transform.position;
        originalRotation = transform.rotation;

        if (fpsController != null) fpsController.enabled = false;
        if (starterInput   != null) starterInput.cursorLocked = false;

        transform.DOMove(monitorCamTarget.position, moveDuration).SetEase(Ease.OutCubic);
        transform.DORotate(monitorCamTarget.eulerAngles, moveDuration).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible   = true;
            });
    }

    void ExitMonitorMode()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        transform.DOMove(originalPosition, moveDuration * 0.7f).SetEase(Ease.OutCubic);
        transform.DORotate(originalRotation.eulerAngles, moveDuration * 0.7f).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                if (fpsController != null) fpsController.enabled = true;
                if (starterInput   != null) starterInput.cursorLocked = true;
                isInteracting = false;
            });
    }
}
