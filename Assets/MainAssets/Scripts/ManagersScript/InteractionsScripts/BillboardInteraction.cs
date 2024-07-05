using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController.Examples; // Importa il namespace corretto

public class BillboardInteraction : MonoBehaviour
{
    [SerializeField] private BillboardType billBoardType;

    [Header("Lock Rotation")]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    [Header("Sprites")]
    [SerializeField] private Sprite mouseAndKeyboardSprite;
    [SerializeField] private Sprite gamepadSprite;
    [SerializeField] private Sprite mobileSprite;

    private SpriteRenderer spriteRenderer;
    private Vector3 originalRotation;

    public enum BillboardType { LookAtCamera, CameraForward };

    private void Awake()
    {
        originalRotation = transform.rotation.eulerAngles;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSpriteBasedOnInput(CheckInputManager.Instance.GetCurrentInputState());
    }

    private void Update()
    {
        // Non necessario aggiornare lo sprite continuamente su mobile
        if (PlatformChecker.isMobile)
        {
            return;
        }

        // Verifica lo stato di input corrente dal CheckInputManager
        CheckInputManager.InputState currentState = CheckInputManager.Instance.GetCurrentInputState();
        UpdateSpriteBasedOnInput(currentState);
    }

    private void LateUpdate()
    {
        switch (billBoardType)
        {
            case BillboardType.LookAtCamera:
                transform.LookAt(Camera.main.transform.position, Vector3.up);
                break;

            case BillboardType.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;

            default:
                break;
        }

        Vector3 rotation = transform.rotation.eulerAngles;
        if (lockX) { rotation.x = originalRotation.x; }
        if (lockY) { rotation.y = originalRotation.y; }
        if (lockZ) { rotation.z = originalRotation.z; }
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void UpdateSprite(Sprite newSprite)
    {
        if (spriteRenderer != null && spriteRenderer.sprite != newSprite)
        {
            spriteRenderer.sprite = newSprite;
        }
    }

    private void UpdateSpriteBasedOnInput(CheckInputManager.InputState inputState)
    {
        switch (inputState)
        {
            case CheckInputManager.InputState.MouseAndKeyboard:
                UpdateSprite(mouseAndKeyboardSprite);
                break;

            case CheckInputManager.InputState.Gamepad:
                UpdateSprite(gamepadSprite);
                break;

            case CheckInputManager.InputState.VirtualJoysticks:
                UpdateSprite(mobileSprite);
                break;

            case CheckInputManager.InputState.None:
            default:
                // Gestisci uno stato di input sconosciuto se necessario
                break;
        }
    }
}
