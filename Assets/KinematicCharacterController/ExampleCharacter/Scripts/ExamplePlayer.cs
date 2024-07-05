using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace KinematicCharacterController.Examples
{
    public class ExamplePlayer : MonoBehaviour
    {
        public GameObject Player;
        public ExampleCharacterController Character;
        public FloatingJoystick floatingJoystick; // Riferimento al joystick sinistro
        public FloatingJoystick rightStickJoystick; // Riferimento al joystick destro

        public bool useMouseAndKeyboard = true;
        public bool useGamepad = true;
        public bool useVirtualJoysticks = true;

        private InputActions playerInputActions;
        private Vector2 keyboardMoveInput;
        private Vector2 lookInput;

        private enum InputState
        {
            MouseAndKeyboard,
            Gamepad,
            VirtualJoysticks,
            None
        }

        private InputState currentState = InputState.None;

        private void Awake()
        {
            playerInputActions = new InputActions();

            if (PlatformChecker.isPC)
            {
                useMouseAndKeyboard = true;
                useGamepad = true;
                useVirtualJoysticks = false;
                currentState = InputState.MouseAndKeyboard;
            }
            else if (PlatformChecker.isMobile)
            {
                useMouseAndKeyboard = false;
                useGamepad = false;
                useVirtualJoysticks = true;
                currentState = InputState.VirtualJoysticks;
            }
            else if (PlatformChecker.isEditor)
            {
                useMouseAndKeyboard = true;
                useGamepad = true;
                useVirtualJoysticks = false;
                currentState = InputState.MouseAndKeyboard;
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
            playerInputActions.Player.SwitchToMouseAndKeyboard.performed += OnSwitchToMouseAndKeyboard;
            playerInputActions.Player.SwitchToGamepad.performed += OnSwitchToGamepad;
        }

        private void OnDisable()
        {
            playerInputActions.Player.Disable();
            playerInputActions.Player.Move.performed -= OnMove;
            playerInputActions.Player.Move.canceled -= OnMove;
            playerInputActions.Player.Look.performed -= OnLook;
            playerInputActions.Player.Look.canceled -= OnLook;
            playerInputActions.Player.SwitchToMouseAndKeyboard.performed -= OnSwitchToMouseAndKeyboard;
            playerInputActions.Player.SwitchToGamepad.performed -= OnSwitchToGamepad;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None; // Non bloccare il cursore
            Cursor.visible = true; // Assicura che il cursore sia visibile
        }

        private void Update()
        {
            InteractionManager.Instance.UpdatePlayerPosition(Player.transform.position);

            if (useVirtualJoysticks && currentState == InputState.VirtualJoysticks)
            {
                HandleCharacterInputWithJoysticks();
            }
            else if (currentState == InputState.MouseAndKeyboard)
            {
                HandleCharacterInput();
                HandleRaycast();
            }
            else if (currentState == InputState.Gamepad)
            {
                HandleCharacterInput();
                HandleRotation();
                Cursor.visible = false; // Hide cursor when using gamepad
                Cursor.lockState = CursorLockMode.Locked; // Lock cursor in place
            }
        }

        private void HandleRaycast()
        {
            if (Mouse.current != null && currentState == InputState.MouseAndKeyboard)
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
            if (currentState != InputState.MouseAndKeyboard && currentState != InputState.Gamepad)
            {
                return;
            }

            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
            Vector2 moveInput = keyboardMoveInput;

            if (currentState == InputState.Gamepad)
            {
                moveInput = Gamepad.current.leftStick.ReadValue();
            }

            characterInputs.MoveAxisForward = moveInput.y;
            characterInputs.MoveAxisRight = moveInput.x;
            characterInputs.CameraRotation = Camera.main.transform.rotation;

            Character.SetInputs(ref characterInputs);
        }

        private void HandleCharacterInputWithJoysticks()
        {
            if (currentState != InputState.VirtualJoysticks)
            {
                return;
            }

            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
            Vector2 moveInput = new Vector2(floatingJoystick.Horizontal, floatingJoystick.Vertical);

            characterInputs.MoveAxisForward = moveInput.y;
            characterInputs.MoveAxisRight = moveInput.x;
            characterInputs.CameraRotation = Camera.main.transform.rotation;

            Character.SetInputs(ref characterInputs);
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            if (currentState == InputState.MouseAndKeyboard && context.control.device is Mouse)
            {
                keyboardMoveInput = context.ReadValue<Vector2>();
            }
            else if (currentState == InputState.Gamepad && context.control.device is Gamepad)
            {
                keyboardMoveInput = context.ReadValue<Vector2>();
            }
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();

            if (currentState == InputState.Gamepad && context.control.device is Gamepad)
            {
                lookInput = input;
            }
            else if (currentState == InputState.MouseAndKeyboard && context.control.device is Mouse)
            {
                lookInput = input;
            }
        }

        private void OnSwitchToMouseAndKeyboard(InputAction.CallbackContext context)
        {
            if (context.control.device is Mouse && (PlatformChecker.isPC || PlatformChecker.isEditor))
            {
                currentState = InputState.MouseAndKeyboard;
                Character.SetIsUsingGamepad(false);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void OnSwitchToGamepad(InputAction.CallbackContext context)
        {
            if (context.control.device is Gamepad && (PlatformChecker.isPC || PlatformChecker.isEditor))
            {
                currentState = InputState.Gamepad;
                Character.SetIsUsingGamepad(true);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void LateUpdate()
        {
            if (useVirtualJoysticks && rightStickJoystick != null && currentState == InputState.VirtualJoysticks)
            {
                if (rightStickJoystick.Horizontal != 0 || rightStickJoystick.Vertical != 0)
                {
                    lookInput = new Vector2(rightStickJoystick.Horizontal, rightStickJoystick.Vertical);
                }
            }
        }
    }
}
