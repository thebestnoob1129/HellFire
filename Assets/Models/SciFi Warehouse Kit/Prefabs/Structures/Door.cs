using System;
using Mono.Cecil;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool isLocked, isOpen;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    
    private Vector3 currentPosition;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = target.transform.position;
    }

    private void Update()
    {
        if (isLocked) return;
        if (isOpen)
        {
            target.position = originalPosition + offset;
        }
        else
        {
            target.position = originalPosition;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        var player = other.gameObject.GetComponent<Player>();
        if (player == null) { return; }
        if (target == null) { return; }
        isOpen = !isOpen;

    }
    private void OnCollisionExit(Collision other)
    {
        var player = other.gameObject.GetComponent<Player>();
        if (player == null) { return; }
        if (target == null) { return; }
        isOpen = !isOpen;
    }
}
