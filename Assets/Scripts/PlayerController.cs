using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : PrisonnerController
{
    private bool PlayerPlaying = false;

    private void Start()
    {
        base.BaseStart();
    }

    internal override void Init()
    {
        base.Init();
        Cursor.lockState = CursorLockMode.Locked;
        PlayerPlaying = true;
        // Positionne la caméra comme enfant du joueur
        playerCamera.transform.SetParent(transform);
        // Définit la position et la rotation initiales de la caméra
        playerCamera.localPosition = new Vector3(0f, 0.5f, 0f);
        playerCamera.localRotation = Quaternion.identity;
    }

    [SerializeField] private float lookSpeed = 5f;
    [SerializeField] private Transform playerCamera;

    private float mouseX;
    private float mouseY;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPlaying = !PlayerPlaying;
            Cursor.lockState = PlayerPlaying ? CursorLockMode.Locked : CursorLockMode.None;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            IsRunning = true;
            RunStartDate = Time.time;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            IsRunning = false;
        }

        if (PlayerPlaying)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            transform.Rotate(Vector3.up, mouseX * lookSpeed);

            float rotationX = playerCamera.localRotation.eulerAngles.x;
            float newRotationX = rotationX - mouseY * lookSpeed;
            //newRotationX = Mathf.Clamp(newRotationX, 0, 90f);
            playerCamera.localRotation = Quaternion.Euler(newRotationX, 0f, 0f);
        }
        base.BaseUpdate();
    }

    private void FixedUpdate()
    {
        if (PlayerPlaying && CanMove)
        {
            //Vector3 movement = new Vector3(horizontal, 0f, vertical) * MoveSpeed * Time.fixedDeltaTime;
            //Vector3 newPosition = rb.position + transform.TransformDirection(movement);
            //rb.MovePosition(newPosition);
            var moveDirection = new Vector3(horizontal, 0, vertical);
            var globalMoveDirection = transform.TransformDirection(moveDirection) * GameSettings.PlayerSpeed * (IsRunning ? 2 : 1);
            rb.velocity = new Vector3(globalMoveDirection.x, rb.velocity.y, globalMoveDirection.z);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }
}