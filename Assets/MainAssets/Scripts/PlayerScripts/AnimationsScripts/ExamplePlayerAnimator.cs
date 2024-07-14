using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using UnityEngine.InputSystem;

namespace KinematicCharacterController.Examples
{
    public class ExamplePlayerAnimator : MonoBehaviour
    {
        public Animator animator;
        public ExampleCharacterController characterController;
        private InputActions playerInputActions;
        private Vector2 moveInput;
        private Camera mainCamera;

        private void Awake()
        {
            playerInputActions = new InputActions();
            mainCamera = Camera.main;
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

        private void Update()
        {
            UpdateAnimator();
            UpdateLookDirection();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            // Gestione dell'input di rotazione
        }

        private void UpdateAnimator()
        {
            float moveX = moveInput.x;
            float moveY = moveInput.y;

            // Imposta i parametri dell'Animator in base all'input di movimento
            animator.SetFloat("MoveX", moveX);
            animator.SetFloat("MoveY", moveY);
        }

        private void UpdateLookDirection()
        {
            if (Mouse.current != null)
            {
                Vector3 mousePosition = Mouse.current.position.ReadValue();
                Ray ray = mainCamera.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 lookDirection = hit.point - transform.position;
                    lookDirection.y = 0; // Mantieni solo la rotazione sull'asse Y

                    if (lookDirection.sqrMagnitude > 0.01f)
                    {
                        // Calcola l'angolo di rotazione basato sulla posizione del mouse
                        float targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
                        transform.rotation = Quaternion.Euler(0, targetAngle, 0);

                        // Calcola l'input di movimento rispetto alla direzione di guardo
                        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

                        // Assicurati che il movimento in avanti sia corretto rispetto alla direzione di guardo
                        Vector3 forward = transform.forward;
                        Vector3 right = transform.right;

                        float adjustedMoveX = Vector3.Dot(moveDirection, right);
                        float adjustedMoveY = Vector3.Dot(moveDirection, forward);

                        // Imposta i parametri dell'Animator in base alla direzione di movimento
                        animator.SetFloat("MoveX", adjustedMoveX);
                        animator.SetFloat("MoveY", adjustedMoveY);
                    }
                }
            }
        }
    }
}
