using UnityEngine;

public class InternetApp : MonoBehaviour
{
    [Header("강제 해킹 (Sin2 트리거)")]
    public Sin2HijackController hijackController;
    public float hijackDelay = 4f;

    private bool hijackTriggered = false;

    void Start()
    {
        TriggerHijackTimer();
    }

    void OnEnable()
    {
        TriggerHijackTimer();
    }

    void TriggerHijackTimer()
    {
        if (!hijackTriggered && hijackController != null)
        {
            hijackTriggered = true;
            StartCoroutine(HijackDelayRoutine());
        }
    }

    System.Collections.IEnumerator HijackDelayRoutine()
    {
        yield return new WaitForSeconds(hijackDelay);
        hijackController.TriggerHijack();
    }
}
