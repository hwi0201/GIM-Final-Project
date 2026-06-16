using System.Collections;
using UnityEngine;

public class Sin2HijackController : MonoBehaviour
{
    [Header("강제로 닫을 데스크탑")]
    public DesktopManager desktopManager;

    [Header("Sin2 비디오 창 (VideoWindow 컴포넌트 붙어있는 오브젝트)")]
    public GameObject sin2VideoWindow;

    [Header("깜빡임 연출")]
    public CanvasGroup blackPanel;
    public int   flickerCount = 5;
    public float flickerSpeed = 0.06f;

    private bool triggered = false;

    void Start()
    {
        if (blackPanel != null)
        {
            blackPanel.alpha = 0f;
            blackPanel.blocksRaycasts = false;
        }
    }

    public void TriggerHijack()
    {
        if (triggered) return;
        triggered = true;
        StartCoroutine(HijackRoutine());
    }

    IEnumerator HijackRoutine()
    {
        if (blackPanel != null)
        {
            blackPanel.blocksRaycasts = true;

            for (int i = 0; i < flickerCount; i++)
            {
                blackPanel.alpha = 1f;
                yield return new WaitForSeconds(flickerSpeed);
                blackPanel.alpha = 0f;
                yield return new WaitForSeconds(flickerSpeed);
            }

            blackPanel.alpha = 1f;
        }

        if (desktopManager != null) desktopManager.CloseWindow();

        if (blackPanel != null)
            yield return new WaitForSeconds(0.2f);

        if (sin2VideoWindow != null) sin2VideoWindow.SetActive(true);

        if (blackPanel != null)
        {
            blackPanel.alpha = 0f;
            blackPanel.blocksRaycasts = false;
        }
    }
}
