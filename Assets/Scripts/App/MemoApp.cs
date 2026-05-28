using UnityEngine;
using TMPro;

public class MemoApp : MonoBehaviour
{
    public TMP_InputField memoInputField;

    private const string MemoSaveKey = "EasterEgg_SecretMemo";

    void Awake()
    {
        if (memoInputField == null)
            memoInputField = GetComponentInChildren<TMP_InputField>();
    }

    void OnEnable()
    {
        if (memoInputField == null) return;

        memoInputField.text = PlayerPrefs.HasKey(MemoSaveKey)
            ? PlayerPrefs.GetString(MemoSaveKey)
            : "";
    }

    void OnDisable()
    {
        if (memoInputField == null) return;

        PlayerPrefs.SetString(MemoSaveKey, memoInputField.text);
        PlayerPrefs.Save();
    }
}
