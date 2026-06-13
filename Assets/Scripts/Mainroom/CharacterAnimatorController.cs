using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimatorController : MonoBehaviour
{
    public float animationDampTime = 0.1f;

    private Animator animator;
    private CharacterController characterController;

    void Start()
    {
        animator = GetComponent<Animator>();

        characterController = GetComponentInParent<CharacterController>();
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (characterController == null) return;

        Vector3 horizontalVelocity = characterController.velocity;
        horizontalVelocity.y = 0f;
        float speed = horizontalVelocity.magnitude;

        float damp = speed > 0.01f ? animationDampTime : 0f;
        animator.SetFloat("Speed", speed, damp, Time.deltaTime);
    }
}
