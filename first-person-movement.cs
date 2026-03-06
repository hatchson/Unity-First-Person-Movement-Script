using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController Controller;
    [SerializeField] private Camera PlayerCamera;

    [Header("Movement")]
    [SerializeField] private float Speed = 5f;
    [SerializeField] private float SneakSpeed = 2.5f;
    [SerializeField] private float JumpForce = 5f;
    [SerializeField] private float Gravity = 9.81f;

    [Header("Camera")]
    [SerializeField] private float Sensitivity = 2f;
    [SerializeField] private float xRotation = 0f;

    [Header("Sliding / Crouch")]
    [SerializeField] private float SlideSpeed = 12f;
    [SerializeField] private float SlideDuration = 0.8f;
    [SerializeField] private float CrouchHeight = 0.5f;
    [SerializeField] private float StandingHeight = 2f;
    [SerializeField] private float CameraCrouchOffset = 0.5f;
    [SerializeField] private bool isSliding = false;
    [SerializeField] private float slideTimer = 0f;
    [SerializeField] private KeyCode SlideKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode CrouchKey = KeyCode.LeftControl;

    [Header("Player States")]
    [SerializeField] private Vector3 Velocity;
    [SerializeField] private Vector2 PlayerMovementInput;
    [SerializeField] private Vector2 PlayerMouseInput;
    [SerializeField] private bool Sneaking = false;

    void Update()
    {
        HandleInput();
        MovePlayer();
        MoveCamera();
    }

    private void HandleInput()
    {
        PlayerMovementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Sneaking = Input.GetKey(CrouchKey);

        if (Input.GetKeyDown(SlideKey) && PlayerMovementInput.magnitude > 0 && Controller.isGrounded && !isSliding)
        {
            isSliding = true;
            slideTimer = SlideDuration;
        }
    }

    private void MovePlayer()
    {
        Vector3 moveVector = transform.TransformDirection(new Vector3(PlayerMovementInput.x, 0, PlayerMovementInput.y));

        if (Controller.isGrounded)
        {
            Velocity.y = -1f;
            if (Input.GetKeyDown(KeyCode.Space) && !Sneaking && !isSliding)
            {
                Velocity.y = JumpForce;
            }
        }
        else
        {
            Velocity.y -= Gravity * Time.deltaTime;
        }

        if (isSliding)
        {
            Controller.Move(moveVector * SlideSpeed * Time.deltaTime);
            Controller.height = Mathf.Lerp(Controller.height, CrouchHeight, 10f * Time.deltaTime);
            Controller.center = new Vector3(0, Controller.height / 2f, 0);
            PlayerCamera.transform.localPosition = Vector3.Lerp(PlayerCamera.transform.localPosition,
                new Vector3(0, CameraCrouchOffset, 0), 10f * Time.deltaTime);

            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
            {
                isSliding = false;
            }
        }
        else
        {
            float currentSpeed = Sneaking ? SneakSpeed : Speed;
            Controller.Move(moveVector * currentSpeed * Time.deltaTime);
            Controller.height = Mathf.Lerp(Controller.height, StandingHeight, 10f * Time.deltaTime);
            Controller.center = new Vector3(0, Controller.height / 2f, 0);
            PlayerCamera.transform.localPosition = Vector3.Lerp(PlayerCamera.transform.localPosition,
                new Vector3(0, 1f, 0), 10f * Time.deltaTime);
        }

        Controller.Move(Velocity * Time.deltaTime);
    }

    private void MoveCamera()
    {
        xRotation -= PlayerMouseInput.y * Sensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.Rotate(0f, PlayerMouseInput.x * Sensitivity, 0f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
