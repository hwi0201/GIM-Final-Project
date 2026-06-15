using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class PhoneController : MonoBehaviour
{
    [Header("Phone Object")]
    public GameObject phoneObject;
    public Transform phoneHolder;

    [Header("Camera")]
    public Camera playerCamera;
    public float normalFOV = 60f;
    public float phoneFOV = 35f;

    [Header("Phone Positions")]
    public Vector3 hiddenLocalPosition = new Vector3(0.35f, -1.0f, 0.65f);
    public Vector3 shownLocalPosition = new Vector3(0.05f, -0.15f, 0.45f);

    [Header("Phone Rotation")]
    public Vector3 hiddenLocalRotation = new Vector3(20f, -20f, 0f);
    public Vector3 shownLocalRotation = new Vector3(5f, 0f, 0f);

    [Header("Phone Scale")]
    public Vector3 phoneScale = new Vector3(1f, 1f, 1f);

    [Header("Animation")]
    public float moveSpeed = 8f;
    public float rotateSpeed = 8f;
    public float fovSpeed = 6f;

    [Header("Optional Player Control")]
    public MonoBehaviour playerMoveScript;
    public MonoBehaviour cameraLookScript;
    public bool stopPlayerControlWhilePhoneOpen = true;

    [Header("Timeline Cutscene")]
    public PlayableDirector endCutsceneDirector;
    public Behaviour cinemachineBrain;
    public bool playCutsceneAfterPhoneClose = true;

    [Header("Guide UI")]
    public BlinkingGuideText guideText;

    private bool endCutscenePlayed = false;

    private bool cutsceneStarted = false;
    private bool isPhoneOpen = false;
    private bool isAnimating = false;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera != null)
        {
            normalFOV = playerCamera.fieldOfView;
        }

        if (phoneObject != null)
        {
            phoneObject.SetActive(false);
        }

        if (phoneHolder != null)
        {
            phoneHolder.localPosition = hiddenLocalPosition;
            phoneHolder.localRotation = Quaternion.Euler(hiddenLocalRotation);
            phoneHolder.localScale = phoneScale;
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TogglePhone();
        }
    }

    void TogglePhone()
    {
        if (isAnimating) return;
        if (phoneHolder == null || phoneObject == null || playerCamera == null) return;

        bool closingPhone = isPhoneOpen;
        isPhoneOpen = !isPhoneOpen;

        if (isPhoneOpen)
        {
            phoneObject.SetActive(true);
            if (guideText != null)
            {
                guideText.HideGuide();
            }
        }

        if (stopPlayerControlWhilePhoneOpen)
        {
            SetPlayerControl(!isPhoneOpen);
        }

        Vector3 targetPosition = isPhoneOpen ? shownLocalPosition : hiddenLocalPosition;
        Quaternion targetRotation = Quaternion.Euler(isPhoneOpen ? shownLocalRotation : hiddenLocalRotation);
        float targetFOV = isPhoneOpen ? phoneFOV : normalFOV;

        StartCoroutine(AnimatePhoneView(targetPosition, targetRotation, targetFOV, closingPhone));
    }

    IEnumerator AnimatePhoneView(Vector3 targetPosition, Quaternion targetRotation, float targetFOV, bool closingPhone)
    {
        isAnimating = true;

        while (
            Vector3.Distance(phoneHolder.localPosition, targetPosition) > 0.01f ||
            Quaternion.Angle(phoneHolder.localRotation, targetRotation) > 0.5f ||
            Mathf.Abs(playerCamera.fieldOfView - targetFOV) > 0.1f
        )
        {
            phoneHolder.localPosition = Vector3.Lerp(
                phoneHolder.localPosition,
                targetPosition,
                Time.deltaTime * moveSpeed
            );

            phoneHolder.localRotation = Quaternion.Lerp(
                phoneHolder.localRotation,
                targetRotation,
                Time.deltaTime * rotateSpeed
            );

            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                targetFOV,
                Time.deltaTime * fovSpeed
            );

            if (closingPhone)
            {
                phoneObject.SetActive(false);
                StartEndCutscene();
            }

            yield return null;
        }
        

        phoneHolder.localPosition = targetPosition;
        phoneHolder.localRotation = targetRotation;
        playerCamera.fieldOfView = targetFOV;

        if (!isPhoneOpen)
        {
        phoneObject.SetActive(false);

            if (!endCutscenePlayed && playCutsceneAfterPhoneClose && endCutsceneDirector != null)
                {
                endCutscenePlayed = true;
                if (cinemachineBrain != null)
                {
                    cinemachineBrain.enabled = true;
                }
                SetPlayerControl(false);
                endCutsceneDirector.Play();
                }
        }

        isAnimating = false;
    }
    void StartEndCutscene()
    {
    if (cutsceneStarted) return;
    if (!playCutsceneAfterPhoneClose) return;
    if (endCutsceneDirector == null) return;

        cutsceneStarted = true;

        if (cinemachineBrain != null)
        {
            cinemachineBrain.enabled = true;
        }

        endCutsceneDirector.Play();
    }
    void SetPlayerControl(bool active)
    {
        if (playerMoveScript != null)
        {
            playerMoveScript.enabled = active;
        }

        if (cameraLookScript != null)
        {
            cameraLookScript.enabled = active;
        }

        Cursor.visible = isPhoneOpen;
        Cursor.lockState = isPhoneOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }
}