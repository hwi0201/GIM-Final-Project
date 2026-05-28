using UnityEngine;

public class DesktopManager : MonoBehaviour
{
    [Header("공용 창 프레임")]
    public GameObject baseWindow;

    [Header("앱 내용물 (Contents)")]
    public GameObject appMail;
    public GameObject appMemo;

    void Start()
    {
        CloseWindow();
    }

    public void OpenMailApp()
    {
        baseWindow.SetActive(true);
        appMail.SetActive(true);
        appMemo.SetActive(false);
    }

    public void OpenMemoApp()
    {
        baseWindow.SetActive(true);
        appMail.SetActive(false);
        appMemo.SetActive(true);
    }

    public void CloseWindow()
    {
        baseWindow.SetActive(false);
    }
}
