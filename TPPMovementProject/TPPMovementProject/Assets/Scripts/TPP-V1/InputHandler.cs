using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace TPP_V1
{
    public class InputHandler : MonoBehaviour
    {
        public PlayerControls inputActions;
        public float vertical;
        public float horizontal;
        public bool walkPressed;
        public bool jumpPressed;
        public bool jumpReleased;

        Vector2 movementInput;

        private void OnEnable()
        {
            if(inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
                inputActions.PlayerMovement.Walk.performed += ctx => walkPressed = !walkPressed;
                inputActions.PlayerMovement.Jump.performed += ctx =>
                {
                    //var button = ctx.control as ButtonControl;
                    //if (button.wasPressedThisFrame)
                    //    jumpPressed = true;
                    //else
                    //    jumpPressed = false;

                    //if (button.wasReleasedThisFrame)
                    //    jumpReleased = true;
                    //else
                    //    jumpReleased = false;
                    jumpPressed = ctx.action.triggered && ctx.action.ReadValue<float>() > 0;
                    jumpReleased = ctx.action.triggered && ctx.action.ReadValue<float>() == default;
                };
            }
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.PlayerMovement.Movement.performed -= ctx => movementInput = ctx.ReadValue<Vector2>();
            inputActions.PlayerMovement.Walk.performed -= ctx => walkPressed = !walkPressed;
            inputActions.PlayerMovement.Jump.performed -= ctx =>
            {
                jumpPressed = ctx.action.triggered && ctx.action.ReadValue<float>() > 0;
                jumpReleased = ctx.action.triggered && ctx.action.ReadValue<float>() == default;
            };
            inputActions.Disable();
        }


        private void MovementInputCalculator(float delta)
        {
            vertical = movementInput.y;
            horizontal = movementInput.x;            
        }

        public void UpdateInput(float delta)
        {
            MovementInputCalculator(delta);
        }

        //private void Update()
        //{
        //    Debug.Log("Walk pressed = " + walkPressed);
        //    Debug.Log("Jump pressed = " + jumpPressed);
        //    Debug.Log("Jump released = " + jumpReleased);
        //    Debug.Log("Move input = " + movementInput);
        //}
    }
}
