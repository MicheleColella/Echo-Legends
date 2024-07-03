using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine.InputSystem;

namespace KinematicCharacterController.Examples
{
    public class ExamplePlayer : MonoBehaviour
    {
        public ExampleCharacterController Character;
        public FloatingJoystick floatingJoystick; // Riferimento al joystick

        private InputActions playerInputActions;
        private Vector2 keyboardMoveInput;
        private Vector2 lookInput;
        private bool isUsingGamepad;

        private void Awake()
        {
            playerInputActions = new InputActions();
        }

        private void OnEnable()
        {
            playerInputActions.Player.Enable();
            playerInputActions.Player.Move.performed += OnMove;
            playerInputActions.Player.Move.canceled += OnMove;
            playerInputActions.Player.Look.performed += OnLook;
            playerInputActions.Player.Look.canceled += OnLook;
        }

        private void OnDisable()
        {
            playerInputActions.Player.Disable();
            playerInputActions.Player.Move.performed -= OnMove;
            playerInputActions.Player.Move.canceled -= OnMove;
            playerInputActions.Player.Look.performed -= OnLook;
            playerInputActions.Player.Look.canceled -= OnLook;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None; // Non bloccare il cursore
            Cursor.visible = true; // Assicura che il cursore sia visibile
        }

        private void Update()
        {
            HandleCharacterInput();

            if (isUsingGamepad)
            {
                HandleRotation();
            }
            else
            {
                HandleRaycast();
            }
        }

        private void HandleRaycast()
        {
            if (Mouse.current != null)
            {
                Vector3 mousePosition = Mouse.current.position.ReadValue();
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 hitPosition = hit.point;
                    hitPosition.y = 0;

                    // Pass the raycast position to the character
                    Character.SetRaycastPosition(hitPosition);
                }
            }
        }

        private void HandleRotation()
        {
            if (lookInput.sqrMagnitude > 0.01f)
            {
                // Calcola l'angolo di rotazione basato sull'input della levetta destra
                float targetAngle = Mathf.Atan2(lookInput.x, lookInput.y) * Mathf.Rad2Deg;
                Character.SetLookAngle(targetAngle);
            }
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
            Vector2 moveInput = keyboardMoveInput;

            // Aggiungi l'input del joystick virtuale se è assegnato
            if (floatingJoystick != null)
            {
                moveInput += new Vector2(floatingJoystick.Horizontal, floatingJoystick.Vertical);
            }

            characterInputs.MoveAxisForward = moveInput.y;
            characterInputs.MoveAxisRight = moveInput.x;
            characterInputs.CameraRotation = Camera.main.transform.rotation;

            Character.SetInputs(ref characterInputs);
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            keyboardMoveInput = context.ReadValue<Vector2>();
            if (context.control.device is Gamepad)
            {
                isUsingGamepad = true;
                Character.SetIsUsingGamepad(true);
            }
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
            if (context.control.device is Gamepad)
            {
                isUsingGamepad = true;
                Character.SetIsUsingGamepad(true);
            }
            else
            {
                isUsingGamepad = false;
                Character.SetIsUsingGamepad(false);
            }
        }
    }
}
