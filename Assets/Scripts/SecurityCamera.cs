using System;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    private bool Active = false;
    private const float Frequency = 1.0f;
    public const float DetectionDistance = 15f;
    private float LastFoundDate;

    public PrisonnerController PlayerControllerInstance { get; private set; }
    public bool Movable = false;

    [SerializeField]
    private Transform CameraHead;

    public event EventHandler<Transform> OnPlayerRepered;

    private void Start()
    {
        PlayerControllerInstance = transform.parent.GetComponentInChildren<PrisonnerController>();
        UnityEngine.Assertions.Assert.IsNotNull(PlayerControllerInstance);
    }

    public void Init()
    {
        Active = true;
        LastFoundDate = Time.time + 2.0f * Frequency;
    }

    internal void PlayerGameOver()
    {
        Active = false;
    }

    private const int MaxRotations = 250;
    private const int StaticRotation = 35;
    private int CurrentRotations = (StaticRotation + MaxRotations) / 2;
    private bool PositiveRotation = true;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Active && (Time.time - LastFoundDate > Frequency) )
        {
            if (Physics.Raycast(CameraHead.transform.position, (PlayerControllerInstance.transform.position - CameraHead.transform.position).normalized, out RaycastHit raycastHit, DetectionDistance))
            {
                if (raycastHit.transform.CompareTag("Player"))
                {
                    if (Vector3.Dot(CameraHead.transform.forward, (raycastHit.transform.position - CameraHead.transform.position).normalized) > .65f)
                    {
                        OnPlayerRepered?.Invoke(this, raycastHit.transform);
                        LastFoundDate = Time.time;
                    }
                }
            }
        }
        if (Movable)
        {
            Rotate();
        }
    }
    private const float CameraSpeed = 25.0f;
    private void Rotate()
    {
        if (CurrentRotations > StaticRotation)
        {
            transform.localEulerAngles += Vector3.up * Time.deltaTime * (PositiveRotation ? CameraSpeed : -CameraSpeed);
            if (CurrentRotations % MaxRotations == 0)
            {
                PositiveRotation = !PositiveRotation;
                CurrentRotations = 0;
            }
        }
        CurrentRotations++;
    }
}