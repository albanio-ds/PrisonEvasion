using System;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    public event EventHandler<Transform> OnPlayerLeaving;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnPlayerLeaving?.Invoke(this, other.transform);
        }
    }
}
