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

        private float airTimeThreshold = 0.2f; // Tempo in secondi per considerare il giocatore non a terra
        private float airTime = 0.0f; // Timer per il tempo trascorso in aria
        private bool isGrounded = true;

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
            UpdateGroundedStatus();
            UpdateAnimator();
            UpdateLookDirection();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            // Handle look input
        }

        private void UpdateGroundedStatus()
        {
            bool isCurrentlyGrounded = characterController.Motor.GroundingStatus.IsStableOnGround;

            if (isCurrentlyGrounded)
            {
                airTime = 0.0f;
                isGrounded = true;
            }
            else
            {
                airTime += Time.deltaTime;
                if (airTime > airTimeThreshold)
                {
                    isGrounded = false;
                }
            }
        }

        private void UpdateAnimator()
        {
            float moveX = moveInput.x;
            float moveY = moveInput.y;
            bool isMoving = moveX != 0 || moveY != 0;

            bool isCurrentlyGrounded = characterController.Motor.GroundingStatus.IsStableOnGround;

            if (isCurrentlyGrounded)
            {
                airTime = 0.0f;
                isGrounded = true;
            }
            else
            {
                airTime += Time.deltaTime;
                if (airTime > airTimeThreshold)
                {
                    isGrounded = false;
                }
            }

            animator.SetFloat("MoveX", moveX);
            animator.SetFloat("MoveY", moveY);
            animator.SetBool("isMoving", isMoving);
            animator.SetBool("isGrounded", isGrounded);
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
                    lookDirection.y = 0; // Keep only the Y-axis rotation

                    if (lookDirection.sqrMagnitude > 0.01f)
                    {
                        float targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
                        transform.rotation = Quaternion.Euler(0, targetAngle, 0);

                        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
                        Vector3 forward = transform.forward;
                        Vector3 right = transform.right;

                        float adjustedMoveX = Vector3.Dot(moveDirection, right);
                        float adjustedMoveY = Vector3.Dot(moveDirection, forward);

                        animator.SetFloat("MoveX", adjustedMoveX);
                        animator.SetFloat("MoveY", adjustedMoveY);
                    }
                }
            }
        }
    }
}
