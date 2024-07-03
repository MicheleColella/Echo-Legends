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

        private InputActions playerInputActions;
        private Vector2 moveInput;

        private void Awake()
        {
            playerInputActions = new InputActions();
        }

        private void OnEnable()
        {
            playerInputActions.Player.Enable();
            playerInputActions.Player.Move.performed += OnMove;
            playerInputActions.Player.Move.canceled += OnMove;
        }

        private void OnDisable()
        {
            playerInputActions.Player.Disable();
            playerInputActions.Player.Move.performed -= OnMove;
            playerInputActions.Player.Move.canceled -= OnMove;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None; // Non bloccare il cursore
            Cursor.visible = true; // Assicura che il cursore sia visibile
        }

        private void Update()
        {
            HandleCharacterInput();
            HandleRaycast(); // Aggiungi la chiamata a HandleRaycast qui
        }

        private void HandleRaycast()
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPosition = hit.point;

                hitPosition.y = 0;

                Debug.Log("Raycast Position: " + hitPosition);

                // Pass the raycast position to the character
                Character.SetRaycastPosition(hitPosition);
            }
        }


        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            characterInputs.MoveAxisForward = moveInput.y;
            characterInputs.MoveAxisRight = moveInput.x;

            Character.SetInputs(ref characterInputs);
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }
}
