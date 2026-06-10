using UnityEngine;
using UnityEngine.Events;

public class CoffeeInteraction : MonoBehaviour
{
    public enum CoffeeState { Idle, HoldingEmpty, Pouring, FilledReady, HoldingFilled, Done }
    public CoffeeState State { get; private set; } = CoffeeState.Idle;

    [Header("상호작용 마우스 버튼 (0=왼쪽, 1=오른쪽)")]
    public int mouseButton = 0;

    [Header("--- 빈 컵 (책상) ---")]
    public GameObject coffeeCup;
    public GameObject cupIcon;
    public float cupInteractDistance = 2f;

    [Header("--- 손의 컵 ---")]
    public GameObject cupInHand;

    [Header("--- 커피머신 ---")]
    public GameObject coffeeMachine;
    public GameObject machineIcon;
    public float machineInteractDistance = 2f;
    public GameObject cupOnMachine;
    public GameObject cupOnMachineIcon;

    [Header("--- 커피 붓기 이펙트 ---")]
    public CoffeePourEffect pourEffect;

    [Header("--- 자리 (컵 내려놓는 곳) ---")]
    public GameObject deskDropZone;
    public GameObject deskDropIcon;
    public float deskInteractDistance = 2f;
    public GameObject cupOnDesk;

    [Header("--- 알림음 ---")]
    public AudioSource notificationAudio;

    [Header("--- 완료 이벤트 ---")]
    public UnityEvent onCoffeeDone;

    private Camera cam;
    private bool isBusy = false;

    void Start()
    {
        cam = Camera.main;

        if (cupIcon          != null) cupIcon.SetActive(false);
        if (machineIcon      != null) machineIcon.SetActive(false);
        if (cupOnMachine     != null) cupOnMachine.SetActive(false);
        if (cupOnMachineIcon != null) cupOnMachineIcon.SetActive(false);
        if (cupInHand        != null) cupInHand.SetActive(false);
        if (deskDropIcon     != null) deskDropIcon.SetActive(false);
        if (deskDropZone     != null) deskDropZone.SetActive(false);
        if (cupOnDesk        != null) cupOnDesk.SetActive(false);
    }

    void Update()
    {
        if (isBusy) return;

        switch (State)
        {
            case CoffeeState.Idle:
                CheckCupPickup();
                break;
            case CoffeeState.HoldingEmpty:
                CheckMachineUse();
                break;
            case CoffeeState.FilledReady:
                CheckFilledCupPickup();
                break;
            case CoffeeState.HoldingFilled:
                CheckDeskDrop();
                break;
        }
    }

    // ── 빈 컵 집기 ─────────────────────────────

    void CheckCupPickup()
    {
        if (coffeeCup == null) return;
        bool inRange = IsInRange(coffeeCup.transform, cupInteractDistance);
        if (cupIcon != null) cupIcon.SetActive(inRange);

        if (inRange && Input.GetMouseButtonDown(mouseButton) && IsLookingAt(coffeeCup))
            PickUpEmpty();
    }

    void PickUpEmpty()
    {
        State = CoffeeState.HoldingEmpty;
        if (coffeeCup != null) coffeeCup.SetActive(false);
        if (cupIcon   != null) cupIcon.SetActive(false);
        if (cupInHand != null) cupInHand.SetActive(true);
    }

    // ── 머신 사용 ──────────────────────────────

    void CheckMachineUse()
    {
        if (coffeeMachine == null) return;
        bool inRange = IsInRange(coffeeMachine.transform, machineInteractDistance);
        if (machineIcon != null) machineIcon.SetActive(inRange);

        if (inRange && Input.GetMouseButtonDown(mouseButton) && IsLookingAt(coffeeMachine))
            UseMachine();
    }

    void UseMachine()
    {
        isBusy = true;
        State  = CoffeeState.Pouring;

        if (cupInHand    != null) cupInHand.SetActive(false);
        if (machineIcon  != null) machineIcon.SetActive(false);
        if (cupOnMachine != null) cupOnMachine.SetActive(true);

        if (pourEffect != null)
        {
            pourEffect.StartPour(() =>
            {
                isBusy = false;
                State  = CoffeeState.FilledReady;
                if (cupOnMachineIcon != null) cupOnMachineIcon.SetActive(true);
            });
        }
        else
        {
            isBusy = false;
            State  = CoffeeState.FilledReady;
            if (cupOnMachineIcon != null) cupOnMachineIcon.SetActive(true);
        }
    }

    // ── 차오른 컵 집기 ─────────────────────────

    void CheckFilledCupPickup()
    {
        if (cupOnMachine == null) return;
        bool inRange = IsInRange(cupOnMachine.transform, machineInteractDistance);
        if (cupOnMachineIcon != null) cupOnMachineIcon.SetActive(inRange);

        if (inRange && Input.GetMouseButtonDown(mouseButton) && IsLookingAt(cupOnMachine))
            PickUpFilled();
    }

    void PickUpFilled()
    {
        State = CoffeeState.HoldingFilled;
        if (cupOnMachine     != null) cupOnMachine.SetActive(false);
        if (cupOnMachineIcon != null) cupOnMachineIcon.SetActive(false);
        if (cupInHand        != null) cupInHand.SetActive(true);
        if (deskDropZone     != null) deskDropZone.SetActive(true);
    }

    // ── 자리에 컵 내려놓기 ─────────────────────

    void CheckDeskDrop()
    {
        if (deskDropZone == null) return;
        bool inRange = IsInRange(deskDropZone.transform, deskInteractDistance);
        if (deskDropIcon != null) deskDropIcon.SetActive(inRange);

        if (inRange && Input.GetMouseButtonDown(mouseButton) && IsLookingAt(deskDropZone))
            DropAtDesk();
    }

    void DropAtDesk()
    {
        State = CoffeeState.Done;

        if (cupInHand    != null) cupInHand.SetActive(false);
        if (deskDropIcon != null) deskDropIcon.SetActive(false);
        if (cupOnDesk    != null) cupOnDesk.SetActive(true);

        if (notificationAudio != null) notificationAudio.Play();

        onCoffeeDone?.Invoke();
    }

    // ──────────────────────────────────────────

    bool IsInRange(Transform target, float distance)
    {
        return Vector3.Distance(cam.transform.position, target.position) <= distance;
    }

    bool IsLookingAt(GameObject target)
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            return hit.collider != null &&
                   (hit.collider.gameObject == target ||
                    hit.collider.transform.IsChildOf(target.transform));
        }
        return false;
    }
}
