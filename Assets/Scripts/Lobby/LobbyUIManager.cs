using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LobbyUIManager : MonoBehaviour
{
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
