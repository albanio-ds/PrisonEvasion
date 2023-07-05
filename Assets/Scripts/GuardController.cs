using System;
using UnityEngine;

public class GuardController : MonoBehaviour
{
    public event EventHandler<Transform> OnPlayerRepered;
    public const float moveSpeed = 3f;
    public const float rotateSpeed = 5f;
    public Transform[] patrolPoints { private get; set; }
    public const float detectionDistance = 15f;

    private int currentPoint = 0;

    public PrisonnerController PlayerControllerInstance { get; private set; }
    public bool CanMove { get; internal set; } = false;

    private void Start()
    {
        PlayerControllerInstance = transform.parent.GetComponentInChildren<PrisonnerController>();
        UnityEngine.Assertions.Assert.IsNotNull(PlayerControllerInstance);
    }

    internal void Init(Transform[] checkpts, uint index)
    {
        patrolPoints = checkpts;
        CanMove = true;
        currentPoint = (int)(index % checkpts.Length);

        transform.position = patrolPoints[index % checkpts.Length].position;
        // orientation initiale du garde vers la deuxième boîte
        Vector3 direction = patrolPoints[(index + 1) % checkpts.Length].position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation;
    }

    private void FixedUpdate()
    {
        if (CanMove)
        {

            MovementProcess();

            if (Physics.Raycast(transform.position, (PlayerControllerInstance.transform.position - transform.position).normalized, out RaycastHit raycastHit, detectionDistance))
            //if (Vector3.Distance(transform.position, PlayerControllerInstance.transform.position) < detectionDistance)
            {
                if (raycastHit.transform.CompareTag("Player"))
                {
                    if (Vector3.Dot(transform.forward, (raycastHit.transform.position - transform.position).normalized) > .55f)
                    {
                        //Debug.Log("Found");
                        OnPlayerRepered?.Invoke(this, raycastHit.transform);
                    }
                }
            }
        }
    }
    private const float epsilon = .05f;

    private void MovementProcess()
    {// orientation du garde vers la prochaine boîte
        Vector3 direction = patrolPoints[currentPoint].position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        float angle = Quaternion.Angle(transform.localRotation, lookRotation);
        if (angle < 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, patrolPoints[currentPoint].position, GameSettings.PlayerSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, patrolPoints[currentPoint].position) < 0.1f)
            {
                currentPoint++;

                if (currentPoint >= patrolPoints.Length)
                {
                    currentPoint = 0;
                }
            }
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, lookRotation, Time.deltaTime * rotateSpeed);
        }
    }

    internal void PlayerGameOver()
    {
        CanMove = false;
        transform.LookAt(PlayerControllerInstance.transform);
    }
}
