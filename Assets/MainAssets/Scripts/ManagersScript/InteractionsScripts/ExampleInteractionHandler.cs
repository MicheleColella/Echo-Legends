using UnityEngine;

public class ExampleInteractionHandler : MonoBehaviour
{
    public string interactionMessage;

    public void HandleInteraction(InteractableObject interactableObject)
    {
        Debug.Log(interactionMessage);
    }
}
