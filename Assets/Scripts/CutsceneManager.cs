using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CutsceneManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject firstPersonPlayer;

    [Header("Student")]
    public GameObject studentCharacter;
    public Animator studentAnimator;

    [Header("Teacher")]
    public Animator teacherAnimator;

    [Header("Cameras")]
    public Camera firstPersonCamera;
    public Camera camWide;
    public Camera camTeacher;
    public Camera camStudent;

    [Header("UI")]
    public Image fadePanel;
    public float fadeDuration = 1f;

    // 0 = camWide, 1 = camTeacher, 2 = camStudent
    private Dictionary<int, int> cameraCuts = new Dictionary<int, int>()
    {
        { 0,  0 },   // 선생님: 뭐야               → wide
        { 1,  2 },   // 피해자: 드릴 말씀이          → student
        { 2,  1 },   // 선생님: 말해봐              → teacher
        { 3,  2 },   // NARRATION: 빨리             → student
        { 4,  2 },   // 피해자: 괴롭히는 애들         → student
        { 5,  1 },   // 선생님: 잠깐                → teacher
        { 6,  1 },   // 선생님: 때렸다고             → teacher
        { 7,  2 },   // 피해자: 증거요               → student
        { 8,  1 },   // 선생님: 사진 영상            → teacher
        { 9,  0 },   // 선생님: 겉으로 보기엔         → wide
        { 10, 2 },   // 피해자: 불러서 얘기해주시면    → student
        { 11, 1 },   // 선생님: ○○이가             → teacher
        { 12, 1 },   // 선생님: 걔가 그럴 애야        → teacher
        { 13, 2 },   // 피해자: 거짓말 안 해요        → student
        { 14, 1 },   // 선생님: 거짓말이라는 게        → teacher
        { 15, 0 },   // 선생님: 근데 걔는            → wide
        { 16, 1 },   // 선생님: 네가 먼저 건드린       → teacher
        { 17, 2 },   // NARRATION: 숨이 막힌다       → student
        { 18, 2 },   // 피해자: 아니요               → student
        { 19, 1 },   // 선생님: 목소리 낮춰           → teacher
        { 20, 0 },   // 선생님: 내가 증거도           → wide
        { 21, 2 },   // 피해자: 그럼 저는요           → student
        { 22, 1 },   // 선생님: 야                  → teacher
        { 23, 0 },   // 선생님: 선생님도 이걸          → wide
        { 24, 2 },   // NARRATION: 장난이라고 했다    → student
        { 25, 1 },   // 선생님: 솔직하게             → teacher
        { 26, 0 },   // 선생님: 남자들끼리            → wide
        { 27, 1 },   // 선생님: 일 더 커지기 전에      → teacher
        { 28, 2 },   // NARRATION: 내가 사과를        → student
        { 29, 2 },   // 피해자: 제발 믿어주세요        → student
        { 30, 1 },   // 선생님: 확실한 게             → teacher
        { 31, 1 },   // 선생님: 들어가 봐             → teacher
        { 32, 2 },   // NARRATION: 말했다            → student
    };

    private bool cutsceneActive = false;
    private Camera currentCamera;

    public void TriggerCutscene()
    {
        if (!cutsceneActive)
            StartCoroutine(StartCutscene());
    }

    IEnumerator StartCutscene()
    {
        cutsceneActive = true;

        // fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // hide first person player
        firstPersonPlayer.SetActive(false);
        firstPersonCamera.gameObject.SetActive(false);

        // show student character
        studentCharacter.SetActive(true);
        studentAnimator.SetBool("isTalking", true);

        // start teacher animation
        teacherAnimator.SetBool("isTalking", true);

        // start on wide shot
        SwitchToCamera(0);

        // unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // fade back in
        yield return StartCoroutine(Fade(1f, 0f));
        yield return new WaitForSeconds(0.5f);

        // start dialogue
        FindObjectOfType<DialogueSystem>().StartDialoguePublic();
    }

    // called by DialogueSystem every time a new line shows
    public void OnDialogueLine(int lineIndex)
    {
        if (cameraCuts.ContainsKey(lineIndex))
            SwitchToCamera(cameraCuts[lineIndex]);
    }

    void SwitchToCamera(int camIndex)
    {
        // turn off all cutscene cameras first
        camWide.gameObject.SetActive(false);
        camTeacher.gameObject.SetActive(false);
        camStudent.gameObject.SetActive(false);

        // turn on the right one
        switch (camIndex)
        {
            case 0: camWide.gameObject.SetActive(true);    currentCamera = camWide;    break;
            case 1: camTeacher.gameObject.SetActive(true); currentCamera = camTeacher; break;
            case 2: camStudent.gameObject.SetActive(true); currentCamera = camStudent; break;
        }
    }

    public IEnumerator EndCutscene()
    {
        // fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // stop animations
        teacherAnimator.SetBool("isTalking", false);
        studentAnimator.SetBool("isTalking", false);

        // hide student
        studentCharacter.SetActive(false);

        // turn off all cutscene cameras
        camWide.gameObject.SetActive(false);
        camTeacher.gameObject.SetActive(false);
        camStudent.gameObject.SetActive(false);

        // restore first person player
        firstPersonPlayer.SetActive(true);
        firstPersonCamera.gameObject.SetActive(true);

        // lock cursor
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