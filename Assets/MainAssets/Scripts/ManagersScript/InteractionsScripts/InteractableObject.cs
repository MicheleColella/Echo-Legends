using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public float interactionRange = 3f;
    public GameObject childObject;

    private void Start()
    {
        InteractionManager.Instance.RegisterInteractable(this);
    }

    private void OnDestroy()
    {
        InteractionManager.Instance.UnregisterInteractable(this);
        //Debug.Log("Destroyed interactable: " + gameObject.name);
    }

    public void Interact()
    {
        //Debug.Log("Interacted with " + gameObject.name);
        var handler = GetComponent<ExampleInteractionHandler>();
        if (handler != null)
        {
            handler.HandleInteraction(this);
        }
    }

    public void SetChildActive(bool isActive)
    {
        if (childObject != null)
        {
            childObject.SetActive(isActive);
        }
    }
}
