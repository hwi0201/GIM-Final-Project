using UnityEngine;

public class DesktopManager : MonoBehaviour
{
    [Header("공용 창 프레임")]
    public GameObject baseWindow;

    [Header("앱 내용물 (Contents)")]
    public GameObject appMail;
    public GameObject appMemo;
    public GameObject appInternet;
    public GameObject appDocx;

    [Header("비디오 윈도우 (BaseWindow 별개)")]
    public GameObject videoWindow;

    void Start()
    {
        OpenDocxApp();
    }

    public void OpenMailApp()
    {
        baseWindow.SetActive(true);
        appMail.SetActive(true);
        appMemo.SetActive(false);
        if (appInternet != null) appInternet.SetActive(false);
        if (appDocx     != null) appDocx.SetActive(false);
    }

    public void OpenMemoApp()
    {
        baseWindow.SetActive(true);
        appMail.SetActive(false);
        appMemo.SetActive(true);
        if (appInternet != null) appInternet.SetActive(false);
        if (appDocx     != null) appDocx.SetActive(false);
    }

    public void OpenInternetApp()
    {
        baseWindow.SetActive(true);
        appMail.SetActive(false);
        appMemo.SetActive(false);
        if (appInternet != null) appInternet.SetActive(true);
        if (appDocx     != null) appDocx.SetActive(false);
    }

    public void OpenDocxApp()
    {
        baseWindow.SetActive(true);
        appMail.SetActive(false);
        appMemo.SetActive(false);
        if (appInternet != null) appInternet.SetActive(false);
        if (appDocx     != null) appDocx.SetActive(true);
    }

    public void CloseWindow()
    {
        baseWindow.SetActive(false);
    }

    public void OpenVideoWindow()
    {
        if (videoWindow != null) videoWindow.SetActive(true);
    }

    public void CloseVideoWindow()
    {
        if (videoWindow != null) videoWindow.SetActive(false);
    }
}
