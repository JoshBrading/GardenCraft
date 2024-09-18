using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [Header("Rigid Body")]
    public Rigidbody body;

    [Header("Camera")]
    public Camera camera;

    [Range(0.1f, 4f)]
    public float lookSensitivity = 0.5f;
    public float maxLookAngle = 85f;
    private float currentLookAngle;
    private bool canLook = true;

    [Header("Movement")]
    public float walkSpeed = 5f;
    [Range(1f, 3f)]
    public float sprintScale = 1f;
    [Range(0.01f, 1f)]
    public float moveAcceleration = 1f;
    public float jumpForce = 10f;
    public float gravityMultiplier = 1f;
    Vector2 DeltaPointer;
    bool isSprinting = false;

    public PlayerInputActions playerControls;

    private InputAction move;
    private InputAction jump;
    private InputAction sprint;
    private InputAction look;

    private bool isGrounded = true;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 currentVelocity = Vector3.zero;

    //This is supposed to dictate if the player can pick up an item
    public static bool handFull;

    //Water Particle
    public ParticleSystem waterParticle;

    public static int SlimeState;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    //This is the fundamentally better way but I can't figure it out
    /*public void SetState(Slime state)
    {
        Slime = state;
    }*/

     private void OnEnable() // Initialize player inputs
     {

        move = playerControls.Player.Move;
        move.Enable();

        jump = playerControls.Player.Jump;
        jump.Enable();
        jump.performed += Jump;

        sprint = playerControls.Player.Sprint;
        sprint.Enable();
        sprint.performed += ToggleSprint;

        look = playerControls.Player.Look;
        look.Enable();
     }

     public override void OnNetworkSpawn()
     {
         if (IsOwner)
         {
             this.transform.GetChild(0).GetComponent<Camera>().enabled = true;
         }
        base.OnNetworkSpawn();
     }

     private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        sprint.Disable();
        look.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();

        if (canLook)
        {
            DeltaPointer = look.ReadValue<Vector2>();

            camera.transform.Rotate(new Vector3(0f, DeltaPointer.x * lookSensitivity, 0f), Space.Self);
            // TODO: Add rotation for mesh here when we have a player model

            currentLookAngle += -DeltaPointer.y * lookSensitivity;
            currentLookAngle = Mathf.Clamp(currentLookAngle, -maxLookAngle, maxLookAngle);

            camera.transform.localRotation = Quaternion.Euler(currentLookAngle, camera.transform.localRotation.eulerAngles.y, 0f);
        }
    }

    public void FixedUpdate()
    {
        if (!IsOwner) return;

        float sprintSpeed = isSprinting ? sprintScale : 1.0f;

        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;

        cameraForward.y = 0f;   // Zero out y component
        cameraRight.y = 0f;     // Since we only move on x and z axis

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection = (cameraForward * moveDirection.y + cameraRight * moveDirection.x).normalized;
        Vector3 targetVelocity = moveDirection * walkSpeed * sprintSpeed;


        // Accelerate and Decelerate velocity
        if (moveDirection.magnitude > 0)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, moveAcceleration);
        }
        else
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, moveAcceleration);
        }

        body.velocity = new Vector3(currentVelocity.x, body.velocity.y, currentVelocity.z);

        // Modify gravity for player, this helps the player feel more "weighty"
        body.AddForce(Physics.gravity * (gravityMultiplier - 1) * body.mass); 
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            body.AddForce(Vector3.up * body.mass * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    private void ToggleSprint(InputAction.CallbackContext context)
    {
        isSprinting = !isSprinting;
    }

    public void toggleLook()
    {
        canLook = !canLook;
    }

    public void SlimeActive()
    {
        if (SlimeState == 0)
        {
            //Normal Values
            walkSpeed = 10f;
            moveAcceleration = 1;
        }
        else
        {
            //Slippery Values
            walkSpeed = 20f;
            moveAcceleration = 0.08f;
        }

    }
}