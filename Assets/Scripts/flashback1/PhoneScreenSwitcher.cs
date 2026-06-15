using UnityEngine;

public class PhoneScreenSwitcher : MonoBehaviour
{
    [Header("Panels")]
    public GameObject lockScreenPanel;
    public GameObject kakaoPanel;

    [Header("Optional Chat")]
    public KakaoChatController kakaoChatController;

    [Header("Audio")]
    public KakaoNotificationAudio kakaoNotificationAudio;

    void Start()
    {
        ShowLockScreen();
    }

    public void ShowLockScreen()
    {
        if (lockScreenPanel != null)
        {
            lockScreenPanel.SetActive(true);
        }

        if (kakaoPanel != null)
        {
            kakaoPanel.SetActive(false);
        }
    }

    public void ShowKakaoScreen()
    {
        if (lockScreenPanel != null)
        {
            lockScreenPanel.SetActive(false);
        }

        if (kakaoPanel != null)
        {
            kakaoPanel.SetActive(true);
        }

        if (kakaoChatController != null)
        {
            kakaoChatController.PlayChat();
        }

        if (kakaoNotificationAudio != null)
        {
            kakaoNotificationAudio.StopNotification();
        }
    }
}