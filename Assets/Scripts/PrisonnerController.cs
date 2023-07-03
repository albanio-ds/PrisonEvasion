using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrisonnerController : MonoBehaviour
{
    public Inventory Inventory = new Inventory();

    protected Rigidbody rb { private set; get; }

    public float MoveSpeed { get; } = 5f;

    public bool CanMove { internal set; get; } = false;

    protected float horizontal;
    protected float vertical;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        UnityEngine.Assertions.Assert.IsNotNull(rb);
        rb.useGravity = false;
        rb.constraints = (RigidbodyConstraints)(112 + 4);
    }

    internal virtual void Init()
    {
        transform.localPosition = new Vector3(-4, 0, -4);
        CanMove = true;
        Inventory = new Inventory();
    }
}
