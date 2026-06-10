using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int correctIndex;
    public bool isPlaced = false;

    private RectTransform  rt;
    private CanvasGroup    cg;
    private Canvas         rootCanvas;
    private Vector2        homePosition;
    private PuzzleManager  manager;

    void Awake()
    {
        rt          = GetComponent<RectTransform>();
        cg          = GetComponent<CanvasGroup>();
        rootCanvas  = GetComponentInParent<Canvas>();
        manager     = FindFirstObjectByType<PuzzleManager>();
    }

    public void SetHome()
    {
        homePosition = rt.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (isPlaced) return;
        cg.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData e)
    {
        if (isPlaced) return;
        rt.anchoredPosition += e.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (isPlaced) return;
        cg.blocksRaycasts = true;

        PuzzleSlot nearest = manager.GetNearestSlot(rt.position, correctIndex);

        if (nearest != null)
        {
            PlaceInSlot(nearest);
        }
        else
        {
            rt.DOAnchorPos(homePosition, 0.25f).SetEase(Ease.OutBack);
        }
    }

    void PlaceInSlot(PuzzleSlot slot)
    {
        isPlaced = true;
        slot.isFilled = true;
        rt.DOAnchorPos(slot.GetComponent<RectTransform>().anchoredPosition, 0.15f)
          .SetEase(Ease.OutQuad)
          .OnComplete(() => manager.OnPiecePlaced());
    }

    public void ReturnHome()
    {
        rt.DOAnchorPos(homePosition, 0.25f).SetEase(Ease.OutBack);
    }
}
