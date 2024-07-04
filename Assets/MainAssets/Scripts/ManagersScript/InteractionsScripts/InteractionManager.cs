using UnityEngine;
using System.Collections.Generic;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }
    private List<InteractableObject> interactableObjects = new List<InteractableObject>();
    private InteractableObject nearestObject;
    private Vector3 playerPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdatePlayerPosition(Vector3 newPosition)
    {
        playerPosition = newPosition;
        //Debug.Log("Updated player position: " + playerPosition);
    }

    private void Update()
    {
        UpdateNearestObject();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            InteractWithNearestObject();
        }
    }

    public void RegisterInteractable(InteractableObject interactable)
    {
        if (!interactableObjects.Contains(interactable))
        {
            interactableObjects.Add(interactable);
            //Debug.Log("Registered interactable: " + interactable.gameObject.name);
        }
    }

    public void UnregisterInteractable(InteractableObject interactable)
    {
        if (interactableObjects.Contains(interactable))
        {
            interactableObjects.Remove(interactable);
            //Debug.Log("Unregistered interactable: " + interactable.gameObject.name);
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
            //Debug.Log("Nearest object: " + nearestObject.gameObject.name);
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
