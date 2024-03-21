using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : MonoBehaviour
{
    //public static TestControls inputActions = new TestControls();
    public static PlayerControls inputActions = new PlayerControls();
    public static event Action<InputActionMap> changeActionMap;

    public bool isGamepad;
    public bool isKeyboard;

    public string _currentControlScheme;

    public static bool HasDevice<T>(PlayerInput input) where T : InputDevice
    {
        for (int i = 0; i < input.devices.Count; i++)
        {
            if (input.devices[i] is T)
            {
                return true;
            }
        }

        return false;
    }

    public void OnControlsChanged(PlayerInput input)
    {
        isGamepad = input.currentControlScheme.Equals("Gamepad");
        isKeyboard = input.currentControlScheme.Equals("Keyboard");
    }

    void Start()
    {
        ToggleActionMap(inputActions.UI);
    }

    public static void ToggleActionMap(InputActionMap actionMap)
    {
        if (actionMap.enabled)
        {
            return;
        }

        inputActions.Disable();
        changeActionMap?.Invoke(actionMap);
        actionMap.Enable();
    }

    private void Update()
    {
        if (isGamepad)
        {
            Debug.Log("Input Manager: Gamepad connected.");
        }
        else if (isKeyboard)
        {
            Debug.Log("Input Manager: Keyboard connected.");
        }
    }
}
