// ExampleInteractionHandler.cs
using UnityEngine;

public class ExampleInteractionHandler : MonoBehaviour, IInteractionHandler
{
    public string interactionMessage;

    public void HandleInteraction(InteractableObject interactableObject)
    {
        CustomFunction();
    }

    private void CustomFunction()
    {
        Debug.Log(interactionMessage);
    }
}
