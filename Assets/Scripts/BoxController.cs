using System;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public bool HaveKey { get; internal set; } = false;
    public event EventHandler<Transform> OnKeyFound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (HaveKey)
            {
                HaveKey = false;
                OnKeyFound?.Invoke(this, other.transform);
            }
        }
    }
}
