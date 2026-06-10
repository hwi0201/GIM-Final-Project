using UnityEngine;

public class MailApp : MonoBehaviour
{
    public GameObject inboxPanel;
    public GameObject detailPanel;

    [Header("영상 윈도우 연결")]
    public DesktopManager desktopManager;

    void OnEnable()
    {
        inboxPanel.SetActive(true);
        detailPanel.SetActive(false);
    }

    public void OpenDetail()
    {
        inboxPanel.SetActive(false);
        detailPanel.SetActive(true);
    }

    public void BackToInbox()
    {
        inboxPanel.SetActive(true);
        detailPanel.SetActive(false);
    }

    public void OpenAttachment()
    {
        if (desktopManager != null) desktopManager.OpenVideoWindow();
    }
}
