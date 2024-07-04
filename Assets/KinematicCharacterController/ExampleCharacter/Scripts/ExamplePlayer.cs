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
        public FloatingJoystick floatingJoystick; // Riferimento al joystick sinistro
        public FloatingJoystick rightStickJoystick; // Riferimento al joystick destro

        public bool useMouseAndKeyboard = true;
        public bool useGamepad = true;
        public bool useVirtualJoysticks = true;

        private InputActions playerInputActions;
        private Vector2 keyboardMoveInput;
        private Vector2 lookInput;
        private bool isUsingGamepad;
        private bool isUsingVirtualJoystick;
        private bool isUsingMouse;

        private void Awake()
        {
            playerInputActions = new InputActions();

            if (PlatformChecker.isPC)
            {
                useMouseAndKeyboard = true;
                useGamepad = true;
                useVirtualJoysticks = false;
            }
            else if (PlatformChecker.isMobile)
            {
                useMouseAndKeyboard = false;
                useGamepad = false;
                useVirtualJoysticks = true;
            }
            else if (PlatformChecker.isEditor)
            {
                useMouseAndKeyboard = true;
                useGamepad = true;
                useVirtualJoysticks = false;
            }
            else
            {
                Debug.Log("Piattaforma non riconoscita");
            }
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
            if (useVirtualJoysticks)
            {
                HandleCharacterInputWithJoysticks();
            }
            else
            {
                HandleCharacterInput();
            }

            if (isUsingGamepad && useGamepad)
            {
                HandleRotation();
            }
            else if (isUsingVirtualJoystick && useVirtualJoysticks)
            {
                HandleRotation();
            }
            else if (isUsingMouse && useMouseAndKeyboard)
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
                // Calcola l'angolo di rotazione basato sull'input del look
                float targetAngle = Mathf.Atan2(lookInput.x, lookInput.y) * Mathf.Rad2Deg;
                Character.SetLookAngle(targetAngle);
            }
        }

        private void HandleCharacterInput()
        {
            if (!useMouseAndKeyboard)
            {
                return;
            }

            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
            Vector2 moveInput = keyboardMoveInput;

            characterInputs.MoveAxisForward = moveInput.y;
            characterInputs.MoveAxisRight = moveInput.x;
            characterInputs.CameraRotation = Camera.main.transform.rotation;

            Character.SetInputs(ref characterInputs);
        }

        private void HandleCharacterInputWithJoysticks()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
            Vector2 moveInput = keyboardMoveInput;

            // Aggiungi l'input del joystick sinistro se è assegnato e abilitato
            if (useVirtualJoysticks && floatingJoystick != null)
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
            if (useMouseAndKeyboard)
            {
                keyboardMoveInput = context.ReadValue<Vector2>();
            }
            if (context.control.device is Gamepad && useGamepad)
            {
                isUsingGamepad = true;
                isUsingMouse = false;
                isUsingVirtualJoystick = false;
                Character.SetIsUsingGamepad(true);
            }
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();

            if (context.control.device is Gamepad && useGamepad)
            {
                isUsingGamepad = true;
                isUsingMouse = false;
                isUsingVirtualJoystick = false;
                lookInput = input;
                Character.SetIsUsingGamepad(true);
            }
            else if (useMouseAndKeyboard)
            {
                isUsingMouse = true;
                isUsingGamepad = false;
                isUsingVirtualJoystick = false;
                lookInput = input;
                Character.SetIsUsingGamepad(false);
            }
        }

        private void LateUpdate()
        {
            // Controlla l'input del joystick virtuale destro
            if (useVirtualJoysticks && rightStickJoystick != null && (rightStickJoystick.Horizontal != 0 || rightStickJoystick.Vertical != 0))
            {
                isUsingVirtualJoystick = true;
                isUsingGamepad = false;
                isUsingMouse = false;
                lookInput = new Vector2(rightStickJoystick.Horizontal, rightStickJoystick.Vertical);
            }
        }
    }
}
