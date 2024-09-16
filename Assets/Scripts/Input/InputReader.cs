using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Control;

[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    private Control controls;
    public event Action<bool> PrimaryFireEvent;
    public event Action<Vector2> MoveEvent;
    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Control();
            controls.Player.SetCallbacks(this);
        }
        controls.Player.Enable();

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        MoveEvent?.Invoke(dir);
    }
    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrimaryFireEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            PrimaryFireEvent?.Invoke(false);

        }
    }
}
