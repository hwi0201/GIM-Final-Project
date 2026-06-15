using UnityEngine;

public class DoorPrompt : MonoBehaviour
{
    public GameObject doorPromptPanel;
    public Transform door;          // drag the actual door mesh here
    public float showDistance = 2.5f;

    void Start()
    {
        doorPromptPanel.SetActive(false);
    }

    void Update()
    {
        if (door == null) return;

        float dist = Vector3.Distance(transform.position, door.position);
        Debug.Log("Distance: " + dist);
        doorPromptPanel.SetActive(dist <= showDistance);
    }
}