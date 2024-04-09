using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementInput : MonoBehaviour, InputManager.IPlayerMovementActionMapActions
{
    private InputManager InputManagerInstance;
    Vector2 Movement;
    public void OnMoveLeft(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.started)
        {

        }
        if (context.canceled)
        {

        }
    }

    public void OnMoveRight(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.started)
        {

        }
        if (context.canceled)
        {

        }
    }

    public void OnMoveForward(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.started)
        {

        }
        if (context.canceled)
        {

        }
    }

    public void OnMoveBackwards(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.started)
        {

        }
        if (context.canceled)
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (InputManagerInstance == null)
        {
            InputManagerInstance = new InputManager();
            InputManagerInstance.PlayerMovementActionMap.Enable();
            InputManagerInstance.PlayerMovementActionMap.SetCallbacks(this);

        }

        
    }



    private void OnDestroy()
    {
        InputManagerInstance.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMovementInput(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Movement = context.ReadValue<Vector2>();
        Debug.Log(Movement);
    }
}
