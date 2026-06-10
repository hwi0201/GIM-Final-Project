using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PuzzleManager : MonoBehaviour
{
    [Header("퍼즐 조각 (9개, 인덱스 0~8 순서로)")]
    public PuzzlePiece[] pieces;

    [Header("슬롯 (9개, 인덱스 0~8 순서로)")]
    public PuzzleSlot[] slots;

    [Header("스냅 인식 거리")]
    public float snapDistance = 60f;

    [Header("사운드")]
    public AudioSource audioSource;
    public AudioClip   snapClip;
    public AudioClip   completeClip;
    public AudioClip   glitchClip;

    [Header("퍼즐 패널")]
    public GameObject puzzlePanel;

    [Header("완성 연출")]
    public GameObject completionPanel;
    public Image      completionImage;
    public Sprite     completedSprite;
    public TextMeshProUGUI decryptText;

    [Header("폴더 패널")]
    public GameObject folderPanel;

    private int placedCount = 0;

    void Start()
    {
        foreach (var p in pieces)
            p.SetHome();
    }

    public void OpenPuzzle()
    {
        puzzlePanel.SetActive(true);
        placedCount = 0;
        foreach (var p in pieces)
        {
            p.isPlaced  = false;
            p.ReturnHome();
        }
        foreach (var s in slots)
            s.isFilled = false;
    }

    // PuzzlePiece가 드롭됐을 때 가장 가까운 올바른 슬롯 반환
    public PuzzleSlot GetNearestSlot(Vector3 worldPos, int correctIndex)
    {
        foreach (var slot in slots)
        {
            if (slot.isFilled) continue;
            if (slot.slotIndex != correctIndex) continue;

            float dist = Vector2.Distance(worldPos, slot.transform.position);
            if (dist <= snapDistance)
                return slot;
        }
        return null;
    }

    public void OnPiecePlaced()
    {
        if (audioSource && snapClip)
            audioSource.PlayOneShot(snapClip);

        placedCount++;
        if (placedCount >= pieces.Length)
            StartCoroutine(CompletionRoutine());
    }

    IEnumerator CompletionRoutine()
    {
        yield return new WaitForSeconds(0.3f);

        if (audioSource && completeClip)
            audioSource.PlayOneShot(completeClip);

        // 완성된 X 표시 사진 표시
        puzzlePanel.SetActive(false);
        completionPanel.SetActive(true);

        CanvasGroup cg = completionPanel.GetComponent<CanvasGroup>();
        if (cg != null) { cg.alpha = 0f; cg.DOFade(1f, 0.6f); }

        if (completionImage && completedSprite)
            completionImage.sprite = completedSprite;

        yield return new WaitForSeconds(2.5f);

        // 해독 완료 텍스트
        if (decryptText != null)
        {
            decryptText.gameObject.SetActive(true);
            decryptText.text = "";
            string msg = "해독 완료. 숨겨진 데이터를 추출합니다...";
            foreach (char c in msg)
            {
                decryptText.text += c;
                yield return new WaitForSeconds(0.04f);
            }
        }

        if (audioSource && glitchClip)
            audioSource.PlayOneShot(glitchClip);

        yield return new WaitForSeconds(1.5f);

        // 폴더 패널 오픈
        completionPanel.SetActive(false);
        folderPanel.SetActive(true);
    }
}
