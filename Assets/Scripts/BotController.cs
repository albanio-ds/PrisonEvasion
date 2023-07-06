using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : PrisonnerController
{

    private int RandomIterations;
    private const int RandomIterationsMax = 100;

    private BoxController[] Boxes;
    private ExitScript[] Exits;
    private int BoxesOrder = 0;
    private int ExitChoosen = 0;

    private void Start()
    {
        base.BaseStart();
        Boxes = transform.parent.GetComponentsInChildren<BoxController>();
        Exits = transform.parent.GetComponentsInChildren<ExitScript>();
    }

    internal override void Init()
    {
        RandomIterations = 0;
        BoxesOrder = Random.Range(0, Boxes.Length);
        ExitChoosen = Random.Range(0, Exits.Length);
        base.Init();
    }

    private void FixedUpdate()
    {
        if (CanMove)
        {
            MoveMethod();
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void MoveMethod()
    {
        if (RandomIterations < RandomIterationsMax)
        {
            RandomIterations++;
            horizontal = Random.Range(-1, 1);
            vertical = Random.Range(-1, 1);
        }
        else if (Inventory.Contains(typeof(ExitKey)))
        {
            Vector3 target = Exits[ExitChoosen].transform.localPosition - transform.localPosition;
            horizontal = target.normalized.x;
            vertical = target.normalized.z;
        }
        else
        {
            Vector3 target = Boxes[BoxesOrder].transform.localPosition - transform.localPosition;
            horizontal = target.normalized.x;
            vertical = target.normalized.z;
            if (BoxesOrder < Boxes.Length)
            {
                if (Vector3.Distance(transform.localPosition, Boxes[BoxesOrder].transform.localPosition) < .7f)
                {
                    Debug.Log("Too close");
                    BoxesOrder = (BoxesOrder + 1) % Boxes.Length;
                }
            }
        }
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
    }
}
