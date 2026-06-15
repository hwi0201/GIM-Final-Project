using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject firstPersonPlayer;

    [Header("Student Character")]
    public GameObject studentCharacter;      // the 3D student mesh
    public Animator studentAnimator;

    [Header("Teacher")]
    public Animator teacherAnimator;

    [Header("Cameras")]
    public Camera firstPersonCamera;
    public Camera cutsceneCamera;

    [Header("UI")]
    public Image fadePanel;
    public float fadeDuration = 1f;

    private bool cutsceneActive = false;

    public void TriggerCutscene()
    {
        if (!cutsceneActive)
            StartCoroutine(StartCutscene());
    }

    IEnumerator StartCutscene()
    {
        cutsceneActive = true;

        // 1. fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // 2. hide first person player, show student character
        firstPersonPlayer.SetActive(false);
        studentCharacter.SetActive(true);

        // 3. switch cameras
        firstPersonCamera.gameObject.SetActive(false);
        cutsceneCamera.gameObject.SetActive(true);

        // 4. play animations
        teacherAnimator.SetBool("isTalking", true);
        // student plays automatically from default state

        // 5. unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 6. fade back in
        yield return StartCoroutine(Fade(1f, 0f));

        // 7. small pause then start dialogue
        yield return new WaitForSeconds(0.5f);
        FindObjectOfType<DialogueSystem>().StartDialoguePublic();
    }

    public IEnumerator EndCutscene()
    {
        // fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // restore
        teacherAnimator.SetBool("isTalking", false);
        studentCharacter.SetActive(false);
        firstPersonPlayer.SetActive(true);
        firstPersonCamera.gameObject.SetActive(true);
        cutsceneCamera.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // hold on black
        yield return new WaitForSeconds(1f);

        // fade back in
        yield return StartCoroutine(Fade(1f, 0f));

        cutsceneActive = false;
    }

    IEnumerator Fade(float from, float to)
    {
        fadePanel.gameObject.SetActive(true);
        float elapsed = 0f;
        Color c = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

        c.a = to;
        fadePanel.color = c;

        if (to == 0f)
            fadePanel.gameObject.SetActive(false);
    }
}