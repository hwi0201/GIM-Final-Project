using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public float slideDistance = 2.5f;
    public float slideSpeed = 3f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = transform.position + Vector3.forward * slideDistance;
        Debug.Log("Door initialized. Closed: " + closedPosition + " Open: " + openPosition);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
            Debug.Log("E pressed. Door isOpen: " + isOpen);
        }

        Vector3 target = isOpen ? openPosition : closedPosition;
        transform.position = Vector3.MoveTowards(transform.position, target, slideSpeed * Time.deltaTime);
    }
}