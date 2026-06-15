using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    [Header("Interaction")]
    public float triggerDistance = 3f;
    public Transform playerCamera;

    [Header("Dialogue Panel")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public Button dialogueNextButton;

    [Header("Narration Panel")]
    public GameObject narrationPanel;
    public TextMeshProUGUI narrationText;
    public Button narrationNextButton;

    [Header("Cutscene")]
    public CutsceneManager cutsceneManager;

    private bool isDialogueActive = false;
    private bool playerInRange = false;
    private int currentLine = 0;

    private enum LineType { NPC, Player, Narration }

    private struct DialogueLine
    {
        public LineType type;
        public string speaker;
        public string text;
        public DialogueLine(LineType t, string s, string txt)
        {
            type = t; speaker = s; text = txt;
        }
    }

    private DialogueLine[] lines = new DialogueLine[]
    {
        new DialogueLine(LineType.NPC,      "선생님", "뭐야. 늦게까지 뭐 했어?"),
        new DialogueLine(LineType.Player,   "피해자", "선생님… 드릴 말씀이 있어서요."),
        new DialogueLine(LineType.NPC,      "선생님", "말해봐. 빨리, 나 오늘 바빠."),
        new DialogueLine(LineType.Narration,"",       "빨리. 지금 말 안 하면 또 못 한다."),
        new DialogueLine(LineType.Player,   "피해자", "저… 반에서 저 괴롭히는 애들이 있어요. 계속요. 때리고, 따돌리고, 카톡에서도—"),
        new DialogueLine(LineType.NPC,      "선생님", "잠깐."),
        new DialogueLine(LineType.NPC,      "선생님", "때렸다고? 증거 있어?"),
        new DialogueLine(LineType.Player,   "피해자", "…증거요?"),
        new DialogueLine(LineType.NPC,      "선생님", "사진, 영상. 아니면 다친 데라도. 뭔가 있어야 내가 움직이지 않겠어?"),
        new DialogueLine(LineType.NPC,      "선생님", "겉으로 보기엔 멀쩡한데."),
        new DialogueLine(LineType.Player,   "피해자", "…선생님이 좀 불러서 얘기해주시면 안 돼요? 저 너무 무서워요."),
        new DialogueLine(LineType.NPC,      "선생님", "○○이가?"),
        new DialogueLine(LineType.NPC,      "선생님", "걔가 그럴 애야? 내가 몇 년을 봐왔는데."),
        new DialogueLine(LineType.Player,   "피해자", "선생님, 저 거짓말 안 해요. 진짜예요—"),
        new DialogueLine(LineType.NPC,      "선생님", "거짓말이라는 게 아니야."),
        new DialogueLine(LineType.NPC,      "선생님", "근데 걔는 반장이고, 수행평가도 열심히 내고, 선생님한테 인사도 잘 하거든. 근데 그런 애가 갑자기 누굴 패? 말이 돼?"),
        new DialogueLine(LineType.NPC,      "선생님", "네가 먼저 건드린 거 없어? 솔직하게 말해봐."),
        new DialogueLine(LineType.Narration,"",       "[숨이 막힌다. 내가 먼저. 내가 먼저 건드렸냐고.]"),
        new DialogueLine(LineType.Player,   "피해자", "아니요. 저는 아무것도 안 했어요. 그냥 같은 반이라는 이유만으로—"),
        new DialogueLine(LineType.NPC,      "선생님", "목소리 낮춰."),
        new DialogueLine(LineType.NPC,      "선생님", "내가 증거도 없이 걔를 불렀다가 걔 부모가 가만있을 것 같아? 나도 입장이 있다고."),
        new DialogueLine(LineType.Player,   "피해자", "그럼 저는요? 저는 매일 봐야 하는데—"),
        new DialogueLine(LineType.NPC,      "선생님", "야."),
        new DialogueLine(LineType.NPC,      "선생님", "선생님도 이걸 그냥 넘기고 싶은 게 아니야. 근데 이게 진짜 폭력인지, 그냥 애들 사이 장난인지 내가 판단을 해야 할 거 아니야. 어?"),
        new DialogueLine(LineType.Narration,"",       "장난. 장난이라고 했다. 이 사람한테 이건 아직도 장난이다."),
        new DialogueLine(LineType.NPC,      "선생님", "솔직하게 말할게."),
        new DialogueLine(LineType.NPC,      "선생님", "남자들끼리는 원래 그래. 부딪히고 싸우면서 관계가 만들어지는 거야. 선생님도 그렇게 컸어."),
        new DialogueLine(LineType.NPC,      "선생님", "일 더 커지기 전에 네가 먼저 좀 다가가봐. 사과할 게 있으면 사과하고."),
        new DialogueLine(LineType.Narration,"",       "내가 사과를 해야 한다고. 내가."),
        new DialogueLine(LineType.Player,   "피해자", "선생님, 저 진짜 무서워서 학교 못 오겠어요. 장난이 아니라고요. 제발 믿어주세요!"),
        new DialogueLine(LineType.NPC,      "선생님", "뭔가 확실한 게 생기면 그때 다시 와. 사진이든 뭐든. 없으면 나도 어떻게 해줄 수가 없어."),
        new DialogueLine(LineType.NPC,      "선생님", "들어가 봐."),
        new DialogueLine(LineType.Narration,"",       "말했다. 드디어 말했는데…"),
    };

    void Start()
    {
        dialoguePanel.SetActive(false);
        narrationPanel.SetActive(false);

        dialogueNextButton.onClick.AddListener(NextLine);
        narrationNextButton.onClick.AddListener(NextLine);
    }

    void Update()
    {
        CheckPlayerDistance();

        if (playerInRange && !isDialogueActive && Input.GetKeyDown(KeyCode.F))
            FindObjectOfType<CutsceneManager>().TriggerCutscene();
    }

    void CheckPlayerDistance()
    {
        if (playerCamera == null) { playerInRange = true; return; }
        float dist = Vector3.Distance(transform.position, playerCamera.position);
        playerInRange = dist <= triggerDistance;
    }

    // called by CutsceneManager after fade in
    public void StartDialoguePublic()
    {
        isDialogueActive = true;
        currentLine = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        dialoguePanel.SetActive(false);
        narrationPanel.SetActive(false);
        ShowLine(currentLine);
    }

    void ShowLine(int index)
    {
        if (index >= lines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = lines[index];

        if (line.type == LineType.Narration)
        {
            dialoguePanel.SetActive(false);
            narrationPanel.SetActive(true);
            narrationText.text = line.text;
        }
        else
        {
            narrationPanel.SetActive(false);
            dialoguePanel.SetActive(true);

            speakerNameText.text = line.speaker;
            dialogueText.text = line.text;

            if (line.type == LineType.NPC)
                speakerNameText.color = new Color(1f, 0.85f, 0.3f);
            else
                speakerNameText.color = new Color(0.6f, 0.85f, 1f);
        }
    }

    public void NextLine()
    {
        currentLine++;
        ShowLine(currentLine);
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        narrationPanel.SetActive(false);

        // trigger end cutscene fade to black
        if (cutsceneManager != null)
            StartCoroutine(cutsceneManager.EndCutscene());
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}