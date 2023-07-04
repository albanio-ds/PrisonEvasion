using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrisonnerController : MonoBehaviour
{
    public Inventory Inventory = new Inventory();

    public EventHandler OnPlayerRunning;

    internal Transform Spawn;

    protected Rigidbody rb { private set; get; }

    public float MoveSpeed { get; } = 5f;

    public bool CanMove { internal set; get; } = false;

    protected bool IsRunning = false;
    protected float RunStartDate;

    protected float horizontal;
    protected float vertical;

    private const float RunStepFrequency = 0.15f;
    private float RunLastUpdate;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        UnityEngine.Assertions.Assert.IsNotNull(rb);
        rb.useGravity = false;
        rb.constraints = (RigidbodyConstraints)(112 + 4);
    }

    internal virtual void Init()
    {
        transform.localPosition = Spawn.position;
        transform.localRotation = Quaternion.LookRotation(Spawn.forward);
        CanMove = true;
        IsRunning = false;
        Inventory = new Inventory();
    }

    protected void BaseUpdate()
    {
        if (IsRunning)
        {
            if (Time.time - RunStartDate > RunStepFrequency && Time.time - RunLastUpdate > RunStepFrequency)
            {
                OnPlayerRunning?.Invoke(this, null);
                RunLastUpdate = Time.time;
                Debug.Log("Run sound");
            }
        }
    }
}
