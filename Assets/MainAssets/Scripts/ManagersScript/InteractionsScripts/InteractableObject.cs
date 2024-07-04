// InteractableObject.cs
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public float interactionRange = 3f;
    public GameObject childObject;
    public bool disactiveBillBoard = false;
    private bool hasInteracted = false;

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
        if (disactiveBillBoard && hasInteracted) return;

        //Debug.Log("Interacted with " + gameObject.name);

        if (disactiveBillBoard)
        {
            hasInteracted = true;
            if (childObject != null)
            {
                childObject.SetActive(false);
            }
        }

        var handler = GetComponent<IInteractionHandler>();
        if (handler != null)
        {
            handler.HandleInteraction(this);
        }
    }

    public void SetChildActive(bool isActive)
    {
        if (childObject != null && (!hasInteracted || !disactiveBillBoard))
        {
            childObject.SetActive(isActive);
        }
    }
}
