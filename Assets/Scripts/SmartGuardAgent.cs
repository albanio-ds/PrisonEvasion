using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;
using Unity.MLAgents.Actuators;
using System.Linq;

public class SmartGuardAgent : Agent
{
    private EnvironmentController EnvironmentScript { get; set; }
    private Vector3 LastPositionObserved;
    private float LastTimeSeen;

    private Vector3 StartPos;
    private Quaternion StartRot;
    public PrisonnerController PlayerControllerInstance { get; private set; }

    public event EventHandler<Transform> OnPlayerRepered;
    public const float detectionDistance = 15f;
    private Rigidbody rb { set; get; }

    private Vector3[] ExitPositions;

    // Start is called before the first frame u
    // pdate
    void Start()
    {
        ExitPositions = transform.parent.GetComponentsInChildren<ExitScript>().Select(exit => exit.transform.localPosition).ToArray();
        UnityEngine.Assertions.Assert.IsNotNull(ExitPositions);
        UnityEngine.Assertions.Assert.AreEqual(2, ExitPositions.Length);
        DecisionRequester = GetComponent<DecisionRequester>();
        Stop();
        UnityEngine.Assertions.Assert.IsNotNull(DecisionRequester);
        rb = GetComponent<Rigidbody>();
        UnityEngine.Assertions.Assert.IsNotNull(rb);
        rb.useGravity = false;
        rb.constraints = (RigidbodyConstraints)(112 + 4);
        EnvironmentScript = transform.GetComponentInParent<EnvironmentController>();
        UnityEngine.Assertions.Assert.IsNotNull(EnvironmentScript);
        PlayerControllerInstance = transform.parent.GetComponentInChildren<PrisonnerController>();
        UnityEngine.Assertions.Assert.IsNotNull(PlayerControllerInstance);

        StartPos = transform.localPosition;
        StartRot = transform.localRotation;
    }

    private DecisionRequester DecisionRequester;

    public void Stop()
    {
        DecisionRequester.enabled = false;
        CanMove = false;
    }

    public void CustomStart()
    {
        DecisionRequester.enabled = true;
        CanMove = true;
        OnEndEpisode();
    }

    public void Init()
    {
        LastPositionObserved = Vector3.negativeInfinity;
        LastTimeSeen = float.NaN;
        transform.localRotation = StartRot;
        transform.localPosition = StartPos;
        rb.velocity = Vector3.zero;
        CanMove = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(ExitPositions[0]);
        sensor.AddObservation(ExitPositions[1]);
        if (LastTimeSeen != 0 && Time.time - LastTimeSeen > 2.0f)
        {
            sensor.AddObservation(LastPositionObserved);
        }
    }


    public override void OnEpisodeBegin()
    {
        Init();
    }

    private void OnEndEpisode()
    {
        CanMove = false;
        //EnvironmentScript.EndEpisode();
        EndEpisode();
    }

    internal void PlayerWin()
    {
        AddReward(-100);
        OnEndEpisode();
        Stop();
    }

    internal void PlayerGameOver()
    {
        SetReward(10);
        OnEndEpisode();
        Stop();
    }

    internal void PlayerOnCamera()
    {
        LastPositionObserved = PlayerControllerInstance.transform.localPosition;
        LastTimeSeen = Time.time;
    }

    public const float MoveSpeed = 3f;
    public const float RotateSpeed = 5f;
    public bool CanMove { get; internal set; } = false;

    public override void OnActionReceived(ActionBuffers actions)
    {
        moveX = actions.ContinuousActions[0];
        moveZ = actions.ContinuousActions[1];

        rotY = actions.ContinuousActions[2];
        rotW = actions.ContinuousActions[3];
        //transform.localPosition += MoveSpeed * Time.deltaTime * new Vector3(moveX, 0, moveZ);
        lookRotation = new Quaternion(0, rotY, 0, rotW);
    }

    float moveX;
    float moveZ;

    float rotY;
    float rotW;

    Quaternion lookRotation;

    private void FixedUpdate()
    {

        if (CanMove && DecisionRequester.enabled)
        {
            //transform.localPosition += MoveSpeed * Time.deltaTime * new Vector3(moveX, 0, moveZ);

            var moveDirection = new Vector3(moveX, 0, moveZ);
            var globalMoveDirection = transform.TransformDirection(moveDirection) * GameSettings.PlayerSpeed;
            rb.velocity = new Vector3(globalMoveDirection.x, rb.velocity.y, globalMoveDirection.z);
            //transform.localEulerAngles += Vector3.up * rotY;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, lookRotation, Time.deltaTime * RotateSpeed);


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
        else
        {
            rb.velocity = Vector3.zero;
        }
    }
}
