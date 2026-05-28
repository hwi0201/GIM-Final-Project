using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDrag : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public RectTransform windowRect;

    private RectTransform parentRect;
    private Vector2 pointerOffset;

    void Awake()
    {
        if (windowRect == null)
            windowRect = transform.parent.GetComponent<RectTransform>();

        parentRect = windowRect.parent.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        windowRect.SetAsLastSibling();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out pointerOffset
        );

        pointerOffset = (Vector2)windowRect.localPosition - pointerOffset;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        Vector2 localPointerPosition;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition))
        {
            Vector2 targetPosition = localPointerPosition + pointerOffset;

            float minX = parentRect.rect.xMin + (windowRect.rect.width * windowRect.pivot.x);
            float maxX = parentRect.rect.xMax - (windowRect.rect.width * (1f - windowRect.pivot.x));
            float minY = parentRect.rect.yMin + (windowRect.rect.height * windowRect.pivot.y);
            float maxY = parentRect.rect.yMax - (windowRect.rect.height * (1f - windowRect.pivot.y));

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

            windowRect.localPosition = targetPosition;
        }
    }
}
