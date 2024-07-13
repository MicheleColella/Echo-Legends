using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace KinematicCharacterController.Examples
{
    public class DashController : MonoBehaviour
    {
        [Header("Dash Settings")]
        public float dashDistance = 5f;
        public float dashCooldown = 1f;
        public float dashCost = 40f;
        public float partialDashMultiplier = 0.5f;
        public float dashDuration = 0.2f;
        public float airDashDeceleration = 10f;

        [Header("Stamina Settings")]
        public float maxStamina = 100f;
        public float staminaRechargeRate = 10f;
        public float rechargeDelay = 4f;
        public float zeroStaminaRechargeDelay = 8f;

        [Header("UI Elements")]
        public MMProgressBar staminaBar;
        public Button dashButton; // Button for mobile
        public RectTransform[] staminaRects;

        private ExampleCharacterController characterController;
        private KinematicCharacterMotor characterMotor;
        private float currentStamina;
        private bool canDash = true;
        private bool isDashing = false;
        private Coroutine rechargeCoroutine;
        private Coroutine dashCoroutine;

        private InputActions playerInputActions;
        private bool useMouseAndKeyboard = true;
        private bool useGamepad = false;

        public bool IsDashing
        {
            get { return isDashing; }
            private set { isDashing = value; }
        }

        private void Awake()
        {
            characterController = GetComponent<ExampleCharacterController>();
            characterMotor = GetComponent<KinematicCharacterMotor>();
            currentStamina = maxStamina;

            if (staminaBar != null)
            {
                staminaBar.UpdateBar(currentStamina, 0f, maxStamina);
            }

            playerInputActions = new InputActions();
            UpdateRectTransforms();
        }

        private void Start()
        {
            staminaBar.TextValueMultiplier = maxStamina;
        }

        private void OnEnable()
        {
            playerInputActions.Player.Enable();
            playerInputActions.Player.Dash.performed += OnDashPerformed; // Use performed instead of canceled
            playerInputActions.Player.SwitchToMouseAndKeyboard.performed += OnSwitchToMouseAndKeyboard;
            playerInputActions.Player.SwitchToGamepad.performed += OnSwitchToGamepad;

            if (dashButton != null)
            {
                dashButton.onClick.AddListener(OnDashButtonClicked); // Add listener for mobile button
            }
        }

        private void OnDisable()
        {
            playerInputActions.Player.Disable();
            playerInputActions.Player.Dash.performed -= OnDashPerformed;
            playerInputActions.Player.SwitchToMouseAndKeyboard.performed -= OnSwitchToMouseAndKeyboard;
            playerInputActions.Player.SwitchToGamepad.performed -= OnSwitchToGamepad;

            if (dashButton != null)
            {
                dashButton.onClick.RemoveListener(OnDashButtonClicked); // Remove listener for mobile button
            }
        }

        private void Update()
        {
            if (staminaBar != null)
            {
                staminaBar.UpdateBar(currentStamina, 0f, maxStamina);
            }
        }

        private void OnDashPerformed(InputAction.CallbackContext context)
        {
            if ((useMouseAndKeyboard && context.control.device is Keyboard) || (useGamepad && context.control.device is Gamepad))
            {
                TryDash();
            }
        }

        private void OnDashButtonClicked()
        {
            TryDash();
        }

        private void TryDash()
        {
            if (canDash && characterMotor.GroundingStatus.IsStableOnGround)
            {
                float dashFactor = 1f;
                if (currentStamina < dashCost)
                {
                    dashFactor = currentStamina / dashCost;
                }

                Vector3 dashDirection = characterMotor.BaseVelocity.normalized; // Use the player's movement direction
                Vector3 dashVector = dashDirection * dashDistance * dashFactor;

                currentStamina -= dashCost * dashFactor;
                currentStamina = Mathf.Max(0, currentStamina);
                canDash = false;
                IsDashing = true; // Set the property to true when dashing

                if (rechargeCoroutine != null)
                {
                    StopCoroutine(rechargeCoroutine);
                }

                if (dashCoroutine != null)
                {
                    StopCoroutine(dashCoroutine);
                }

                dashCoroutine = StartCoroutine(PerformDash(dashVector));
                rechargeCoroutine = StartCoroutine(RechargeStamina());
                StartCoroutine(DashCooldown());
            }
        }

        private IEnumerator PerformDash(Vector3 dashVector)
        {
            float elapsedTime = 0f;
            Vector3 initialVelocity = characterMotor.BaseVelocity;
            while (elapsedTime < dashDuration)
            {
                if (!characterMotor.GroundingStatus.IsStableOnGround)
                {
                    break;
                }

                characterMotor.BaseVelocity = dashVector / dashDuration;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            float decelerationElapsed = 0f;
            while (!characterMotor.GroundingStatus.IsStableOnGround && decelerationElapsed < dashDuration)
            {
                characterMotor.BaseVelocity = Vector3.Lerp(dashVector, Vector3.zero, decelerationElapsed / dashDuration);
                decelerationElapsed += Time.deltaTime * airDashDeceleration;
                yield return null;
            }

            characterMotor.BaseVelocity = initialVelocity; // Reset velocity to the original after dash
            IsDashing = false; // Reset the property to false when the dash is complete
        }

        private IEnumerator DashCooldown()
        {
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }

        private IEnumerator RechargeStamina()
        {
            float delay = currentStamina == 0 ? zeroStaminaRechargeDelay : rechargeDelay;
            yield return new WaitForSeconds(delay);

            while (currentStamina < maxStamina)
            {
                currentStamina += staminaRechargeRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
                UpdateStaminaBar(); // Aggiorna la barra della stamina gradualmente
                yield return null;
            }
        }

        private void UpdateStaminaBar()
        {
            if (staminaBar != null)
            {
                staminaBar.SetBar(currentStamina, 0f, maxStamina);
            }
        }

        private void OnSwitchToMouseAndKeyboard(InputAction.CallbackContext context)
        {
            if (context.control.device is Keyboard)
            {
                useMouseAndKeyboard = true;
                useGamepad = false;
            }
        }

        private void OnSwitchToGamepad(InputAction.CallbackContext context)
        {
            if (context.control.device is Gamepad)
            {
                useMouseAndKeyboard = false;
                useGamepad = true;
            }
        }

        public float GetCurrentStamina()
        {
            return currentStamina;
        }

        public void SetCurrentStamina(float value)
        {
            currentStamina = value;
        }

        void UpdateRectTransforms()
        {
            foreach (RectTransform rect in staminaRects)
            {
                rect.sizeDelta = new Vector2(maxStamina, rect.sizeDelta.y);
            }
        }
    }
}
