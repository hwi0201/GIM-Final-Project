using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float acceleration = 10f;       // how fast you reach full speed
    public float deceleration = 15f;       // how fast you stop (higher = snappier stop)
    public float airControl = 0.3f;        // how much control in the air

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    [Header("Head Bob")]
    public float bobFrequency = 2.4f;      // step speed
    public float bobAmplitudeY = 0.06f;    // up/down bounce
    public float bobAmplitudeX = 0.03f;    // side to side sway
    public float bobReturnSpeed = 8f;      // how fast camera recenters when still

    [Header("Landing Bob")]
    public float landBobAmount = 0.12f;    // extra dip when landing
    public float landBobSpeed = 10f;

    [Header("Footstep Tilt")]
    public float tiltAmount = 1.5f;        // camera roll side to side while walking
    public float tiltSpeed = 8f;

    private Rigidbody rb;
    private Transform cameraHolder;
    private float verticalRotation = 0f;
    private float bobTimer = 0f;
    private Vector3 cameraDefaultPos;
    private bool isGrounded;
    private bool wasGrounded;
    private float landBobOffset = 0f;
    private float currentTilt = 0f;
    private Vector3 currentVelocity; // for smooth acceleration

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraHolder = transform.Find("CameraHolder");
        cameraDefaultPos = cameraHolder.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleHeadBob();
        HandleLandingBob();
        HandleTilt();
    }

    void FixedUpdate()
    {
        CheckGrounded();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(0, mouseX, 0);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        cameraHolder.localEulerAngles = new Vector3(verticalRotation, 0, 0);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 targetDirection = (transform.right * moveX + transform.forward * moveZ).normalized;
        Vector3 targetVelocity = targetDirection * walkSpeed;

        // choose accel or decel based on if player is pressing a key
        bool isMoving = targetDirection.magnitude > 0.1f;
        float rate = isMoving ? acceleration : deceleration;

        // reduce control in the air
        if (!isGrounded) rate *= airControl;

        // smoothly move current horizontal velocity toward target
        Vector3 currentHorizontal = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 smoothed = Vector3.MoveTowards(currentHorizontal, targetVelocity, rate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector3(smoothed.x, rb.linearVelocity.y, smoothed.z);
    }

    void HandleHeadBob()
    {
        float horizontalSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        if (isGrounded && horizontalSpeed > 0.5f)
        {
            // bob speed scales with movement speed
            bobTimer += Time.deltaTime * bobFrequency * (horizontalSpeed / walkSpeed);

            float bobY = Mathf.Abs(Mathf.Sin(bobTimer)) * bobAmplitudeY;

            // X sway is half frequency so it goes left/right each step
            float bobX = Mathf.Sin(bobTimer * 0.5f) * bobAmplitudeX;

            Vector3 bobOffset = new Vector3(bobX, bobY, 0);
            cameraHolder.localPosition = Vector3.Lerp(
                cameraHolder.localPosition,
                cameraDefaultPos + bobOffset,
                Time.deltaTime * 12f
            );
        }
        else
        {
            // smoothly return to center when idle
            bobTimer = 0f;
            cameraHolder.localPosition = Vector3.Lerp(
                cameraHolder.localPosition,
                cameraDefaultPos,
                Time.deltaTime * bobReturnSpeed
            );
        }
    }

    void HandleLandingBob()
    {
        // detect the moment of landing
        if (isGrounded && !wasGrounded)
        {
            landBobOffset = -landBobAmount; // dip the camera down on landing
        }

        // spring the landing dip back to zero
        landBobOffset = Mathf.Lerp(landBobOffset, 0f, Time.deltaTime * landBobSpeed);
        cameraHolder.localPosition += new Vector3(0, landBobOffset, 0);

        wasGrounded = isGrounded;
    }

    void HandleTilt()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        // tilt camera slightly in the direction of movement (like leaning into a step)
        float targetTilt = -moveX * tiltAmount;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);

        // apply tilt as Z rotation on top of existing vertical rotation
        cameraHolder.localEulerAngles = new Vector3(verticalRotation, 0, currentTilt);
    }

    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.2f);
    }
}