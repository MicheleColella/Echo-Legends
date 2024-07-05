using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using KinematicCharacterController.Examples;
public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }
    private List<InteractableObject> interactableObjects = new List<InteractableObject>();
    private InteractableObject nearestObject;
    private Vector3 playerPosition;

    private InputActions inputActions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            inputActions = new InputActions();
            inputActions.Player.Interact.performed += OnInteractPerformed;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        if (CheckInputManager.Instance.GetCurrentInputState() == CheckInputManager.InputState.VirtualJoysticks)
        {
            MobileInteractButton.OnInteractButtonPressed += InteractWithNearestObject;
        }
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        if (CheckInputManager.Instance.GetCurrentInputState() == CheckInputManager.InputState.VirtualJoysticks)
        {
            MobileInteractButton.OnInteractButtonPressed -= InteractWithNearestObject;
        }
    }

    public void UpdatePlayerPosition(Vector3 newPosition)
    {
        playerPosition = newPosition;
        // Debug.Log("Updated player position: " + playerPosition);
    }

    private void Update()
    {
        UpdateNearestObject();
    }

    public void RegisterInteractable(InteractableObject interactable)
    {
        if (!interactableObjects.Contains(interactable))
        {
            interactableObjects.Add(interactable);
            // Debug.Log("Registered interactable: " + interactable.gameObject.name);
        }
    }

    public void UnregisterInteractable(InteractableObject interactable)
    {
        if (interactableObjects.Contains(interactable))
        {
            interactableObjects.Remove(interactable);
            // Debug.Log("Unregistered interactable: " + interactable.gameObject.name);
        }
    }

    private void UpdateNearestObject()
    {
        float minDistance = float.MaxValue;
        nearestObject = null;

        foreach (var obj in interactableObjects)
        {
            if (obj == null) continue;

            float distance = Vector3.Distance(playerPosition, obj.transform.position);
            if (distance < minDistance && distance <= obj.interactionRange)
            {
                minDistance = distance;
                nearestObject = obj;
            }
        }

        foreach (var obj in interactableObjects)
        {
            if (obj != null)
            {
                obj.SetChildActive(obj == nearestObject);
            }
        }

        if (nearestObject != null)
        {
            // Debug.Log("Nearest object: " + nearestObject.gameObject.name);
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        var currentState = CheckInputManager.Instance.GetCurrentInputState();
        if ((currentState == CheckInputManager.InputState.MouseAndKeyboard && context.control.device is Keyboard) ||
            (currentState == CheckInputManager.InputState.MouseAndKeyboard && context.control.device is Mouse) ||
            (currentState == CheckInputManager.InputState.Gamepad && context.control.device is Gamepad))
        {
            InteractWithNearestObject();
        }
    }

    private void InteractWithNearestObject()
    {
        if (nearestObject != null)
        {
            nearestObject.Interact();
        }
    }
}
