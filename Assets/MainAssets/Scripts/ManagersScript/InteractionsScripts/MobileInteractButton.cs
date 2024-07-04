using UnityEngine;
using UnityEngine.UI;

public class MobileInteractButton : MonoBehaviour
{
    public static event System.Action OnInteractButtonPressed;

    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogWarning("Button component not found on this GameObject. Interaction button will not be functional.");
        }
    }

    private void OnButtonClick()
    {
        OnInteractButtonPressed?.Invoke();
    }
}
