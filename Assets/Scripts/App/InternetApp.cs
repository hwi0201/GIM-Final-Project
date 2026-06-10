using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InternetApp : MonoBehaviour
{
    [Header("주소창")]
    public TMP_InputField urlBar;
    public Button goButton;

    [Header("페이지 영역")]
    public GameObject pageHome;        // 시작 페이지
    public GameObject pageNotFound;    // 없는 페이지
    public GameObject pageNews;        // 뉴스 기사 페이지 (스토리용)

    [Header("뉴스 페이지 내용")]
    public TextMeshProUGUI newsTitleText;
    public TextMeshProUGUI newsBodyText;

    [TextArea(1,1)]  public string newsUrl   = "news.co.kr/society/death-3people";
    [TextArea(1,1)]  public string newsTitle = "\'의문의 3인 연쇄 사망\' 경찰, 단순 사고사로 최종 결론";
    [TextArea(5,10)] public string newsBody  =
        "경찰은 최근 잇따라 발생한 30대 남녀 3명의 사망 사건에 대해 타살 혐의점이 없다고 밝혔다.\n\n" +
        "사망자 3명은 각각 다른 장소에서 서로 다른 방식으로 숨진 채 발견됐으나, " +
        "경찰은 \'우연의 일치\'라는 입장을 고수하고 있다.\n\n" +
        "그러나 유가족 측은 \"세 명 모두 사망 직전 극도의 불안 증세를 보였으며, " +
        "누군가에게 쫓기는 것 같다고 호소했다\"고 주장하며 재수사를 촉구했다.\n\n" +
        "[댓글 1,204개]";

    void Start()
    {
        if (goButton != null) goButton.onClick.AddListener(OnGoClicked);
        if (urlBar   != null) urlBar.onSubmit.AddListener(_ => OnGoClicked());

        ShowPage(pageHome);
    }

    void OnEnable()
    {
        ShowPage(pageHome);
        if (urlBar != null) urlBar.text = "";
    }

    void OnGoClicked()
    {
        if (urlBar == null) return;
        string url = urlBar.text.Trim().ToLower();

        if (url == newsUrl || url == "www." + newsUrl)
        {
            if (newsTitleText != null) newsTitleText.text = newsTitle;
            if (newsBodyText  != null) newsBodyText.text  = newsBody;
            ShowPage(pageNews);
        }
        else if (string.IsNullOrEmpty(url))
        {
            ShowPage(pageHome);
        }
        else
        {
            ShowPage(pageNotFound);
        }
    }

    void ShowPage(GameObject target)
    {
        if (pageHome     != null) pageHome.SetActive(pageHome         == target);
        if (pageNotFound != null) pageNotFound.SetActive(pageNotFound == target);
        if (pageNews     != null) pageNews.SetActive(pageNews         == target);
    }
}
