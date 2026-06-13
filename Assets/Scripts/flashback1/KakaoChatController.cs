using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KakaoChatController : MonoBehaviour
{
    [Header("Scroll")]
    public ScrollRect scrollRect;
    public RectTransform content;

    [Header("Prefab")]
    public GameObject messagePrefab; // 상대 메시지 프리팹 하나만 사용

    [Header("Messages")]
    [TextArea]
    public string[] messages;

    public float messageInterval = 1.2f;

    private Coroutine chatRoutine;

    public void PlayChat()
    {
        Debug.Log("PlayChat 호출됨");

        if (scrollRect == null)
        {
            Debug.LogError("ScrollRect가 연결되지 않았습니다.");
            return;
        }

        if (content == null)
        {
            Debug.LogError("Content가 연결되지 않았습니다.");
            return;
        }

        if (messagePrefab == null)
        {
            Debug.LogError("Message Prefab이 연결되지 않았습니다.");
            return;
        }

        if (messages == null || messages.Length == 0)
        {
            Debug.LogError("Messages 배열이 비어 있습니다.");
            return;
        }

        if (chatRoutine != null)
        {
            StopCoroutine(chatRoutine);
        }

        ClearChat();
        chatRoutine = StartCoroutine(PlayChatRoutine());
    }

    IEnumerator PlayChatRoutine()
    {
        Debug.Log("채팅 코루틴 시작");

        for (int i = 0; i < messages.Length; i++)
        {
            yield return new WaitForSeconds(messageInterval);
            AddMessage(messages[i]);
        }
    }

    public void AddMessage(string text)
    {
        GameObject msgObj = Instantiate(messagePrefab, content);
        msgObj.SetActive(true);

        TMP_Text msgText = msgObj.GetComponentInChildren<TMP_Text>();
        if (msgText != null)
        {
            msgText.text = text;
        }
        else
        {
            Debug.LogWarning("메시지 프리팹 안에 TMP_Text가 없습니다.");
        }

        Debug.Log("메시지 생성됨: " + text);

        StartCoroutine(ScrollToBottomNextFrame());
    }

    IEnumerator ScrollToBottomNextFrame()
    {
        yield return null;

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);

        yield return null;

        scrollRect.verticalNormalizedPosition = 0f;
    }

    void ClearChat()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
}