using UnityEngine;
using UnityEngine.UI;

public class MobileFireButton : MonoBehaviour
{
    public static event System.Action OnFireButtonPressed;

    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogWarning("Button component not found on this GameObject. Fire button will not be functional.");
        }
    }

    private void OnButtonClick()
    {
        Debug.Log("Fire button clicked!");
        OnFireButtonPressed?.Invoke();
    }
}
